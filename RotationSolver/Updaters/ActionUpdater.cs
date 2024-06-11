using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.Objects.SubKinds;
using ECommons.DalamudServices;
using ECommons.GameHelpers;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using RotationSolver.Commands;
using XIVDrawer.Vfx;

namespace RotationSolver.Updaters;

internal static class ActionUpdater
{
    internal static DateTime AutoCancelTime { get; set; } = DateTime.MinValue;

    static RandomDelay _GCDDelay = new(() => Service.Config.WeaponDelay);

    private static IAction? _nextAction;
    internal static IAction? NextAction 
    { 
        get => _nextAction;
        set
        {
            if (_nextAction == value) return;
            _nextAction = value;
            Service.NextActionID = value?.AdjustedID ?? 0;
        }
    }

    private static StaticVfx? circle, sector, rectangle;
    private static IBaseAction? _nextGCDAction;
    const float gcdHeight = 5;
    internal static IBaseAction? NextGCDAction 
    {
        get => _nextGCDAction;
        set
        {
            UpdateOmen(value);
            if (_nextGCDAction == value) return;
            _nextGCDAction = value;
        }
    }

    private static void UpdateOmen(IBaseAction? value)
    {
        var player = Player.Object;
        if (player == null) return;

        circle ??= new(GroundOmenFriendly.Circle.Omen(), player, new Vector3(0, gcdHeight, 0));
        sector ??= new(GroundOmenFriendly.Fan120.Omen(), player, new Vector3(0, gcdHeight, 0));
        rectangle ??= new(GroundOmenFriendly.Rectangle.Omen(), player, new Vector3(0, gcdHeight, 0));

        circle.Enable = sector.Enable = rectangle.Enable = false;
        circle.Owner = sector.Owner = rectangle.Owner = player;

        if (!Service.Config.UseOverlayWindow) return;
        if (!Service.Config.ShowTarget) return;
        if (value == null) return;

        var target = value.Target.Target ?? player;

        var range = value.Action.EffectRange;
        var size = new Vector3(range, gcdHeight, range);
        switch (value.Action.CastType)
        {
            //case 1:
            case 2 when Service.Config.ShowCircleTarget:
                circle.Owner = target;
                circle.UpdateScale(size);
                circle.Enable = true;
                break;

            case 3 when Service.Config.ShowSectorTarget:
                sector.Target = target;
                sector.UpdateScale(size);
                sector.Enable = true;
                break;

            case 4 when Service.Config.ShowRectangleTarget:
                size.X = value.Action.XAxisModifier / 2;
                rectangle.Target = target;
                rectangle.UpdateScale(size);
                rectangle.Enable = true;
                break;
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

                if(gcdAction == null)
                {
                    NextGCDAction = null;
                }
                if (gcdAction is IBaseAction GcdAction)
                {
                    NextGCDAction = GcdAction;
                }
                return;
            }
        }
        catch (Exception ex)
        {
            Svc.Log.Error(ex, "Failed to update next action.");
        }

        WrongAction = NextAction = NextGCDAction = null;
    }

    private static List<uint>? actionOverrideList;

    private static void SetAction(uint id)
    {
        actionOverrideList ??= Svc.PluginInterface.GetOrCreateData("Avarice.ActionOverride", () => new List<uint>());

        if (actionOverrideList.Count == 0)
        {
            actionOverrideList.Add(id);
        }
        else
        {
            actionOverrideList[0] = id;
        }
    }

    internal unsafe static void UpdateActionInfo()
    {
        SetAction(NextGCDAction?.AdjustedID ?? 0);
        UpdateWeaponTime();
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

    private static unsafe void UpdateWeaponTime()
    {
        var player = Player.Object;
        if (player == null) return;

        var instance = ActionManager.Instance();

        var castTotal = player.TotalCastTime;

        var weaponTotal = instance->GetRecastTime(ActionType.Action, 11);
        if (castTotal > 0) castTotal += 0.1f;
        if (player.IsCasting) weaponTotal = Math.Max(castTotal, weaponTotal);

        DataCenter.WeaponElapsed = instance->GetRecastTimeElapsed(ActionType.Action, 11);
        DataCenter.WeaponRemain = DataCenter.WeaponElapsed == 0 ? player.TotalCastTime - player.CurrentCastTime
            : Math.Max(weaponTotal - DataCenter.WeaponElapsed, player.TotalCastTime - player.CurrentCastTime);

        //Casting time.
        if (DataCenter.WeaponElapsed < 0.3) DataCenter.CastingTotal = castTotal;
        if (weaponTotal > 0 && DataCenter.WeaponElapsed > 0.2) DataCenter.WeaponTotal = weaponTotal;
    }

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

        var maxAhead = Math.Max(DataCenter.MinAnimationLock - DataCenter.Ping, 0.08f);
        var ahead = Math.Min(maxAhead, Service.Config.ActionAhead);

        //GCD
        var canUseGCD = DataCenter.WeaponRemain <= ahead;
        if (_GCDDelay.Delay(canUseGCD))
        {
            return RSCommands.CanDoAnAction(true);
        }
        if (canUseGCD) return false;

        var nextAction = NextAction;
        if (nextAction == null) return false;

        var timeToNext = DataCenter.AnimationLocktime;

        ////No time to use 0gcd
        //if (timeToNext + nextAction.AnimationLockTime
        //    > DataCenter.WeaponRemain) return false;

        //Skip when casting
        if (DataCenter.WeaponElapsed <= DataCenter.CastingTotal) return false;

        //The last one.
        if (timeToNext + nextAction.AnimationLockTime + DataCenter.Ping + DataCenter.MinAnimationLock > DataCenter.WeaponRemain)
        {
            if (DataCenter.WeaponRemain > nextAction.AnimationLockTime + DataCenter.Ping +
                Math.Max(ahead, Service.Config.MinLastAbilityAdvanced)) return false;

            return RSCommands.CanDoAnAction(false);
        }
        else if (timeToNext < ahead)
        {
            return RSCommands.CanDoAnAction(false);
        }

        return false;
    }
}
