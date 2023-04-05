using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.Objects.SubKinds;
using FFXIVClientStructs.FFXIV.Client.Game;
using RotationSolver.Commands;
using RotationSolver.Localization;

namespace RotationSolver.Updaters;

internal static class ActionUpdater
{
    static DateTime _startCombatTime = DateTime.MinValue;

    static  RandomDelay _GCDDelay = new RandomDelay(() => (Service.Config.WeaponDelayMin, Service.Config.WeaponDelayMax));

    internal static IAction NextAction { get; private set; }
    internal static IBaseAction NextGCDAction { get; private set; }

    internal static Exception exception;

    internal static void UpdateNextAction()
    {
        PlayerCharacter localPlayer = Service.Player;
        if (localPlayer == null) return;

        try
        {
            var customRotation = RotationUpdater.RightNowRotation;

            if (customRotation?.TryInvoke(out var newAction, out var gcdAction) ?? false)
            {
                NextAction = newAction;

                if (gcdAction is IBaseAction GcdAction)
                {
                    NextGCDAction = GcdAction;

                    if (GcdAction.EnemyPositional != EnemyPositional.None && GcdAction.Target.HasPositional()
                         && !localPlayer.HasStatus(true, StatusID.TrueNorth))
                    {
                        if (CheckAction(GcdAction.ID))
                        {
                            string positional = GcdAction.EnemyPositional.ToName();
                            if (Service.Config.SayPositional) SpeechHelper.Speak(positional);
                            if (Service.Config.FlytextPositional) Service.ToastGui.ShowQuest(" " + positional, new Dalamud.Game.Gui.Toast.QuestToastOptions()
                            {
                                IconId = GcdAction.IconID,
                            });
                        }
                    }
                }
                else
                {
                    NextGCDAction = null;
                }
                return;
            }
            NextAction = NextGCDAction = null;
        }
        catch (Exception ex)
        {
            exception = ex;
        }

        NextAction = NextGCDAction = null;
    }

    static uint _lastSayingGCDAction;
    static DateTime lastTime;
    static bool CheckAction(uint actionID)
    {
        if ((_lastSayingGCDAction != actionID || DateTime.Now - lastTime > new TimeSpan(0, 0, 3)) && DataCenter.StateType != StateCommandType.Cancel)
        {
            _lastSayingGCDAction = actionID;
            lastTime = DateTime.Now;
            return true;
        }
        else return false;
    }
    internal unsafe static void UpdateActionInfo()
    {
        UpdateWeaponTime();
        UpdateTimeInfo();
    }

    private unsafe static void UpdateTimeInfo()
    {
        var last = DataCenter.InCombat;
        DataCenter.InCombat = Service.Conditions[ConditionFlag.InCombat];
        if(!last && DataCenter.InCombat)
        {
            _startCombatTime = DateTime.Now;
        }
        else if(last && !DataCenter.InCombat)
        {
            _startCombatTime = DateTime.MinValue;
        }
        if (_startCombatTime == DateTime.MinValue)
        {
            DataCenter.CombatTime = 0;
        }
        else
        {
            DataCenter.CombatTime = (float)(DateTime.Now - _startCombatTime).TotalSeconds;
        }

        for (int i = 0; i < DataCenter.BluSlots.Length; i++)
        {
            DataCenter.BluSlots[i] = ActionManager.Instance()->GetActiveBlueMageActionInSlot(i);
        }
        UpdateMPTimer();
    }

    private static unsafe void UpdateWeaponTime()
    {
        var player = Service.Player;
        if (player == null) return;

        var instance = ActionManager.Instance();

        var castTotal = player.TotalCastTime;

        var weaponTotal = instance->GetRecastTime(ActionType.Spell, 11);
        if (castTotal > 0) castTotal += 0.1f;
        if (player.IsCasting) weaponTotal = Math.Max(castTotal, weaponTotal);

        DataCenter.WeaponElapsed = instance->GetRecastTimeElapsed(ActionType.Spell, 11);
        DataCenter.WeaponRemain = DataCenter.WeaponElapsed == 0 ? player.TotalCastTime - player.CurrentCastTime
            : Math.Max(weaponTotal - DataCenter.WeaponElapsed, player.TotalCastTime - player.CurrentCastTime);

        //确定读条时间。
        if (DataCenter.WeaponElapsed < 0.3) DataCenter.CastingTotal = castTotal;

        //确认能力技的相关信息
        var interval = Service.Config.AbilitiesInterval;
        if (DataCenter.WeaponRemain < interval + Service.Config.ActionAhead * 2 
            || DataCenter.WeaponElapsed == 0)
        {
            DataCenter.AbilityRemain = 0;
            if (DataCenter.WeaponRemain > 0)
            {
                DataCenter.AbilityRemain = DataCenter.WeaponRemain + interval;
            }
            DataCenter.AbilityRemainCount = 0;
        }
        else if (DataCenter.WeaponRemain < 2 * interval + Service.Config.ActionAhead * 2)
        {
            DataCenter.AbilityRemain = DataCenter.WeaponRemain - interval;
            DataCenter.AbilityRemainCount = 1;
        }
        else
        {
            var abilityWhole = (int)(weaponTotal / Service.Config.AbilitiesInterval - 1);
            DataCenter.AbilityRemain = interval - DataCenter.WeaponElapsed % interval;
            DataCenter.AbilityRemainCount = (byte)(abilityWhole - (int)(DataCenter.WeaponElapsed / interval));
        }

        if (weaponTotal > 0) DataCenter.WeaponTotal = weaponTotal;
    }

    static uint _lastMP = 0;
    static DateTime _lastMPUpdate = DateTime.Now;
    /// <summary>
    /// 跳蓝经过时间
    /// </summary>
    internal static float MPUpdateElapsed => (float)(DateTime.Now - _lastMPUpdate).TotalSeconds % 3;

    private static void UpdateMPTimer()
    {
        var player = Service.Player;
        if (player == null) return;

        //不是黑魔不考虑啊
        if (player.ClassJob.Id != (uint)ClassJobID.BlackMage) return;

        //有醒梦，就算了啊
        if (player.HasStatus(true, StatusID.LucidDreaming)) return;

        if (_lastMP < player.CurrentMp)
        {
            _lastMPUpdate = DateTime.Now;
        }
        _lastMP = player.CurrentMp;
    }

    internal unsafe static void DoAction()
    {
        if (Service.Conditions[ConditionFlag.OccupiedInQuestEvent]
            || Service.Conditions[ConditionFlag.OccupiedInCutSceneEvent]
            || Service.Conditions[ConditionFlag.Occupied33]
            || Service.Conditions[ConditionFlag.Occupied38]
            || Service.Conditions[ConditionFlag.Jumping61]
            || Service.Conditions[ConditionFlag.BetweenAreas]
            || Service.Conditions[ConditionFlag.BetweenAreas51]
            || Service.Conditions[ConditionFlag.Mounted]
            //|| Service.Conditions[ConditionFlag.SufferingStatusAffliction] //Because of BLU30!
            || Service.Conditions[ConditionFlag.SufferingStatusAffliction2]
            || Service.Conditions[ConditionFlag.RolePlaying]
            || Service.Conditions[ConditionFlag.InFlight]
            || ActionManager.Instance()->ActionQueued
            || Service.Player.CurrentHp == 0) return;

        //GCD
        var canUseGCD = DataCenter.WeaponRemain <= Service.Config.ActionAhead;
        if (_GCDDelay.Delay(canUseGCD)) RSCommands.DoAnAction(true);
        if (canUseGCD) return;

        //More then gcd.
        if (DataCenter.WeaponRemain < Service.Config.AbilitiesInterval
            || DataCenter.WeaponElapsed < Service.Config.AbilitiesInterval)
        {
            return;
        }

        //Skip when casting
        if (DataCenter.WeaponElapsed <= DataCenter.CastingTotal) return;

        //The last one.
        if (DataCenter.WeaponRemain < 2 * Service.Config.AbilitiesInterval)
        {
            if (DataCenter.WeaponRemain > Service.Config.AbilitiesInterval + Service.Config.ActionAhead * 2) return;
            RSCommands.DoAnAction(false);
        }
        else if ((DataCenter.WeaponElapsed - DataCenter.CastingTotal) % Service.Config.AbilitiesInterval <= Service.Config.ActionAhead)
        {
            RSCommands.DoAnAction(false);
        }
    }
}
