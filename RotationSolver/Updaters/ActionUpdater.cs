using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.Objects.SubKinds;
using ECommons.DalamudServices;
using ECommons.GameHelpers;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using RotationSolver.Commands;

namespace RotationSolver.Updaters;

internal static class ActionUpdater
{
    internal static DateTime AutoCancelTime { get; set; } = DateTime.MinValue;

    static RandomDelay _GCDDelay = new(() => Service.Config.WeaponDelay);

    internal static IAction? NextAction { get; set; }

    private static IBaseAction? _nextGCDAction;
    const float gcdHeight = 5;
    internal static IBaseAction? NextGCDAction 
    {
        get => _nextGCDAction;
        set
        {
            if (_nextGCDAction == value) return;
            _nextGCDAction = value;
        }
    }

    internal static IAction? WrongAction { get; set; }
    static readonly Random _wrongRandom = new();

    internal static void ClearNextAction()
    {
        SetAction(0);
        WrongAction = NextAction = NextGCDAction = null;
    }

    internal static void UpdateNextAction()
    {
        PlayerCharacter localPlayer = Player.Object;
        var customRotation = DataCenter.RightNowRotation;

        try
        {
            if (localPlayer != null && customRotation != null
                && customRotation.TryInvoke(out var newAction, out var gcdAction))
            {
                if (Service.Config.MistakeRatio > 0)
                {
                    var actions = customRotation.AllActions.Where(a =>
                    {
                        if (a.ID == newAction?.ID) return false;
                        if (a is IBaseAction action)
                        {
                            return !action.Setting.IsFriendly && action.Config.IsInMistake
                            && action.Setting.TargetType != TargetType.Move
                            && action.CanUse(out _, usedUp: true, skipStatusProvideCheck: true, skipClippingCheck: true, skipAoeCheck: true);
                        }
                        return false;
                    });

                    var count = actions.Count();
                    WrongAction = count > 0 ? actions.ElementAt(_wrongRandom.Next(count)) : null;
                }

                NextAction = newAction;

                if (gcdAction is IBaseAction GcdAction)
                {
                    NextGCDAction = GcdAction;
                }
                return;
            }
        }
        catch (Exception ex)
        {
            WarningHelper.AddSystemWarning($"Failed to update the next action in the rotation because: {ex.Message}");
            Svc.Log.Error(ex, "Failed to update next action.");
        }

        WrongAction = NextAction = NextGCDAction = null;
    }

    private static void SetAction(uint id) => Svc.PluginInterface.GetOrCreateData("Avarice.ActionOverride", 
        () => new List<uint>() { id })[0] = id;

    internal unsafe static void UpdateActionInfo()
    {
        SetAction(NextGCDAction?.AdjustedID ?? 0);
        //UpdateWeaponTime();
        UpdateCombatTime();
        UpdateSlots();
        UpdateMoving();
        UpdateMPTimer();
    }
    private unsafe static void UpdateSlots()
    {
        for (int i = 0; i < DataCenter.BluSlots.Length; i++)
        {
            DataCenter.BluSlots[i] = ActionManager.Instance()->GetActiveBlueMageActionInSlot(i);
        }
        for (ushort i = 0; i < DataCenter.DutyActions.Length; i++)
        {
            DataCenter.DutyActions[i] = ActionManager.GetDutyActionId(i);
        }
    }

    static DateTime _stopMovingTime = DateTime.MinValue;
    private unsafe static void UpdateMoving()
    {
        var last = DataCenter.IsMoving;
        DataCenter.IsMoving = AgentMap.Instance()->IsPlayerMoving > 0;
        if (last && !DataCenter.IsMoving)
        {
            _stopMovingTime = DateTime.Now;
        }
        else if (DataCenter.IsMoving)
        {
            _stopMovingTime = DateTime.MinValue;
        }

        if (_stopMovingTime == DateTime.MinValue)
        {
            DataCenter.StopMovingRaw = 0;
        }
        else
        {
            DataCenter.StopMovingRaw = (float)(DateTime.Now - _stopMovingTime).TotalSeconds;
        }
    }

    static DateTime _startCombatTime = DateTime.MinValue;
    private static void UpdateCombatTime()
    {
        var last = DataCenter.InCombat;
        DataCenter.InCombat = Svc.Condition[ConditionFlag.InCombat];
        if (!last && DataCenter.InCombat)
        {
            _startCombatTime = DateTime.Now;
        }
        else if (last && !DataCenter.InCombat)
        {
            _startCombatTime = DateTime.MinValue;

            if (Service.Config.AutoOffAfterCombat)
            {
                AutoCancelTime = DateTime.Now.AddSeconds(Service.Config.AutoOffAfterCombatTime);
            }
        }

        if (_startCombatTime == DateTime.MinValue)
        {
            DataCenter.CombatTimeRaw = 0;
        }
        else
        {
            if (DataCenter.CombatTimeRaw == 0)
            {
                foreach (var item in DataCenter.TimelineItems)
                {
                    if (!item.IsInWindow) continue;
                    if (item.Type is not TimelineType.InCombat) continue;

                    item.UpdateRaidTimeOffset();
                    break;
                }
            }
            DataCenter.CombatTimeRaw = (float)(DateTime.Now - _startCombatTime).TotalSeconds;
        }
    }

    //private static unsafe void UpdateWeaponTime()
    //{
    //    var player = Player.Object;
    //    if (player == null) return;

    //    var instance = ActionManager.Instance();

    //    var castTotal = player.TotalCastTime;

    //    var weaponTotal = instance->GetRecastTime(ActionType.Action, 11);
    //    if (castTotal > 0) castTotal += 0.1f;
    //    if (player.IsCasting) weaponTotal = Math.Max(castTotal, weaponTotal);

    //    DataCenter.WeaponElapsed = instance->GetRecastTimeElapsed(ActionType.Action, 11);
    //    DataCenter.WeaponRemain = DataCenter.WeaponElapsed == 0 ? player.TotalCastTime - player.CurrentCastTime
    //        : Math.Max(weaponTotal - DataCenter.WeaponElapsed, player.TotalCastTime - player.CurrentCastTime);

    //    //Casting time.
    //    if (DataCenter.WeaponElapsed < 0.3) DataCenter.CastingTotal = castTotal;
    //    if (weaponTotal > 0 && DataCenter.WeaponElapsed > 0.2) DataCenter.WeaponTotal = weaponTotal;
    //}

    static uint _lastMP = 0;
    static DateTime _lastMPUpdate = DateTime.Now;

    internal static float MPUpdateElapsed => (float)(DateTime.Now - _lastMPUpdate).TotalSeconds % 3;

    private static void UpdateMPTimer()
    {
        var player = Player.Object;
        if (player == null) return;

        //不是黑魔不考虑啊
        if (player.ClassJob.Id != (uint)ECommons.ExcelServices.Job.BLM) return;

        //有醒梦，就算了啊
        if (player.HasStatus(true, StatusID.LucidDreaming)) return;

        if (_lastMP < player.CurrentMp)
        {
            _lastMPUpdate = DateTime.Now;
        }
        _lastMP = player.CurrentMp;
    }

    internal unsafe static bool CanDoAction()
    {
        if (Svc.Condition[ConditionFlag.OccupiedInQuestEvent]
            || Svc.Condition[ConditionFlag.OccupiedInCutSceneEvent]
            || Svc.Condition[ConditionFlag.Occupied33]
            || Svc.Condition[ConditionFlag.Occupied38]
            || Svc.Condition[ConditionFlag.Jumping61]
            || Svc.Condition[ConditionFlag.BetweenAreas]
            || Svc.Condition[ConditionFlag.BetweenAreas51]
            || Svc.Condition[ConditionFlag.Mounted]
            //|| Svc.Condition[ConditionFlag.SufferingStatusAffliction] //Because of BLU30!
            || Svc.Condition[ConditionFlag.SufferingStatusAffliction2]
            || Svc.Condition[ConditionFlag.RolePlaying]
            || Svc.Condition[ConditionFlag.InFlight]
            || ActionManager.Instance()->ActionQueued && NextAction != null
                && ActionManager.Instance()->QueuedActionId != NextAction.AdjustedID
            || Player.Object.CurrentHp == 0) return false;

        var nextAction = NextAction;
        if (nextAction == null) return false;

        //Skip when casting
        if (Player.Object.TotalCastTime - Service.Config.Action4Head > 0) return false;

        //GCD
        var canUseGCD = ActionHelper.CanUseGCD;
        if (canUseGCD)
        {
            return RSCommands.CanDoAnAction(true);
        }
        else
        {
            return RSCommands.CanDoAnAction(false);
        }
    }
}
