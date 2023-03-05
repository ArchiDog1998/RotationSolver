using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.Objects.SubKinds;
using FFXIVClientStructs.FFXIV.Client.Game;
using RotationSolver.Actions;
using RotationSolver.Commands;
using RotationSolver.Data;
using RotationSolver.Helpers;
using RotationSolver.SigReplacers;
using System;

namespace RotationSolver.Updaters;

internal static class ActionUpdater
{
    public static unsafe float ComboTime => ActionManager.Instance()->Combo.Timer;
    public static unsafe ActionID LastComboAction => (ActionID)ActionManager.Instance()->Combo.Action;

    static DateTime _startCombatTime = DateTime.MinValue;
    public static TimeSpan CombatTime
    {
        get
        {
            if(_startCombatTime == DateTime.MinValue) return TimeSpan.Zero;
            return DateTime.Now - _startCombatTime;
        }
    }

    static  RandomDelay _GCDDelay = new RandomDelay(() => (Service.Configuration.WeaponDelayMin, Service.Configuration.WeaponDelayMax));

    internal static float WeaponRemain { get; private set; } = 0;

    internal static float WeaponTotal { get; private set; } = 0;

    internal static float WeaponElapsed { get; private set; } = 0;

    internal static bool InCombat { get; private set; } = false;

    internal static byte AbilityRemainCount { get; private set; } = 0;

    internal static float AbilityRemain { get; private set; } = 0;

    internal static uint[] BluSlots { get; private set; } = new uint[24];

    internal static IAction NextAction { get; private set; }

#if DEBUG
    internal static Exception exception;
#endif

    internal static void UpdateNextAction()
    {

        PlayerCharacter localPlayer = Service.ClientState.LocalPlayer;
        if (localPlayer == null) return;

        try
        {
            var customRotation = RotationUpdater.RightNowRotation;

            if (customRotation?.TryInvoke(out var newAction) ?? false)
            {
                NextAction = newAction;
                return;
            }
        }
#if DEBUG
        catch (Exception ex)
        {
            exception = ex;
        }
#else
        catch { }
#endif

        NextAction = null;
    }

    internal unsafe static void UpdateActionInfo()
    {
        var last = InCombat;
        InCombat = Service.Conditions[ConditionFlag.InCombat];
        if(!last && InCombat)
        {
            _startCombatTime = DateTime.Now;
        }
        else if(last && !InCombat)
        {
            _startCombatTime = DateTime.MinValue;
        }

        for (int i = 0; i < BluSlots.Length; i++)
        {
            BluSlots[i] = ActionManager.Instance()->GetActiveBlueMageActionInSlot(i);
        }
        UPdateMPTimer();
    }

    internal static unsafe void UpdateWeaponTime()
    {
        var player = Service.ClientState.LocalPlayer;
        if (player == null) return;

        var instance = ActionManager.Instance();

        var castTotal = player.TotalCastTime;
        castTotal = castTotal > 2.5f ? castTotal + 0.1f : castTotal;

        var weapontotal = instance->GetRecastTime(ActionType.Spell, 11);
        if (player.IsCasting) weapontotal = Math.Max(weapontotal, castTotal);

        WeaponElapsed = instance->GetRecastTimeElapsed(ActionType.Spell, 11);
        WeaponRemain = WeaponElapsed == 0 ? player.TotalCastTime - player.CurrentCastTime
            : Math.Max(weapontotal - WeaponElapsed, player.TotalCastTime - player.CurrentCastTime);

        //确定读条时间。
        if (WeaponElapsed < 0.3) _lastCastingTotal = castTotal;

        //确认能力技的相关信息
        var interval = Service.Configuration.AbilitiesInterval;
        if (WeaponRemain < interval || WeaponElapsed == 0)
        {
            AbilityRemain = 0;
            if (WeaponRemain > 0)
            {
                AbilityRemain = WeaponRemain + interval;
            }
            AbilityRemainCount = 0;
        }
        else if (WeaponRemain < 2 * interval)
        {
            AbilityRemain = WeaponRemain - interval;
            AbilityRemainCount = 1;
        }
        else
        {
            var abilityWhole = (int)(weapontotal / Service.Configuration.AbilitiesInterval - 1);
            AbilityRemain = interval - WeaponElapsed % interval;
            AbilityRemainCount = (byte)(abilityWhole - (int)(WeaponElapsed / interval));
        }

        if (weapontotal > 0) WeaponTotal = weapontotal;
    }

    static uint _lastMP = 0;
    static DateTime _lastMPUpdate = DateTime.Now;
    /// <summary>
    /// 跳蓝经过时间
    /// </summary>
    internal static float MPUpdateElapsed => (float)(DateTime.Now - _lastMPUpdate).TotalSeconds % 3;

    private static void UPdateMPTimer()
    {
        var player = Service.ClientState.LocalPlayer;
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

    internal static float _lastCastingTotal = 0;
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
            || ActionManager.Instance()->ActionQueued) return;

        //GCD
        var canUseGCD = WeaponRemain <= Service.Configuration.ActionAhead;
        if (_GCDDelay.Delay(canUseGCD)) RSCommands.DoAnAction(true);
        if (canUseGCD) return;

        //要超出GCD了，那就不放技能了。
        if (WeaponRemain < Service.Configuration.AbilitiesInterval
            || WeaponElapsed < Service.Configuration.AbilitiesInterval)
        {
            return;
        }

        //还在咏唱，就不放技能了。
        if (WeaponElapsed <= _lastCastingTotal) return;

        //只剩下最后一个能力技了，然后卡最后！
        if (WeaponRemain < 2 * Service.Configuration.AbilitiesInterval)
        {
            if (WeaponRemain > Service.Configuration.AbilitiesInterval + Service.Configuration.ActionAhead) return;
            RSCommands.DoAnAction(false);
        }
        else if ((WeaponElapsed - _lastCastingTotal) % Service.Configuration.AbilitiesInterval <= Service.Configuration.ActionAhead)
        {
            RSCommands.DoAnAction(false);
        }
    }
}
