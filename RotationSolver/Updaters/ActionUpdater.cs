using Dalamud.Game.ClientState.Objects.SubKinds;
using FFXIVClientStructs.FFXIV.Client.Game;
using RotationSolver.Actions;
using RotationSolver.Commands;
using RotationSolver.Data;
using RotationSolver.Helpers;
using RotationSolver.SigReplacers;
using System;
using System.ComponentModel;
using System.Diagnostics;

namespace RotationSolver.Updaters;

internal static class ActionUpdater
{
    private static unsafe IntPtr ComboTimer => (IntPtr)((byte*)ActionManager.Instance() + 0x60);
    public static unsafe float ComboTime => *(float*)ComboTimer;
    private static IntPtr LastComboMove => ComboTimer + 4;
    public static unsafe ActionID LastComboAction => *(ActionID*)LastComboMove;


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
            var customCombo = IconReplacer.RightNowCombo;

            if (customCombo?.TryInvoke(out var newAction) ?? false)
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
        InCombat = Service.Conditions[Dalamud.Game.ClientState.Conditions.ConditionFlag.InCombat];

        for (int i = 0; i < BluSlots.Length; i++)
        {
            BluSlots[i] = ActionManager.Instance()->GetActiveBlueMageActionInSlot(i);
        }
        UpdateWeaponTime();
        UPdateMPTimer();
    }

    private static unsafe void UpdateWeaponTime()
    {
        var player = Service.ClientState.LocalPlayer;
        if (player == null) return;

        var instance = ActionManager.Instance();

        var castTotal = player.TotalCastTime;
        castTotal = castTotal > 2.5f ? castTotal + 0.1f : castTotal;

        var weapontotal = instance->GetRecastTime(ActionType.Spell, 11);
        if (player.IsCasting) weapontotal = Math.Max(weapontotal, castTotal);

        WeaponElapsed = instance->GetRecastTimeElapsed(ActionType.Spell, 11);
        WeaponRemain = Math.Max(weapontotal - WeaponElapsed, player.TotalCastTime - player.CurrentCastTime);

        //确定读条时间。
        if (WeaponElapsed < 0.3) _lastCastingTotal = castTotal;


        //确认能力技的相关信息
        var interval = Service.Configuration.WeaponInterval;
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
            var abilityWhole = (int)(weapontotal / Service.Configuration.WeaponInterval - 1);
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
        if (player.ClassJob.Id != 25) return;

        //有醒梦，就算了啊
        if (player.HasStatus(true, StatusID.LucidDreaming)) return;

        if (_lastMP < player.CurrentMp)
        {
            _lastMPUpdate = DateTime.Now;
        }
        _lastMP = player.CurrentMp;
    }

    static readonly Stopwatch _weaponDelayStopwatch = new Stopwatch();
    static long _weaponRandomDelay = 0;
    internal static float _lastCastingTotal = 0;
    internal unsafe static void DoAction()
    {
        if (Service.Conditions[Dalamud.Game.ClientState.Conditions.ConditionFlag.OccupiedInQuestEvent]
            || Service.Conditions[Dalamud.Game.ClientState.Conditions.ConditionFlag.OccupiedInCutSceneEvent]
            || Service.Conditions[Dalamud.Game.ClientState.Conditions.ConditionFlag.Occupied33]
            || Service.Conditions[Dalamud.Game.ClientState.Conditions.ConditionFlag.Occupied38]
            || Service.Conditions[Dalamud.Game.ClientState.Conditions.ConditionFlag.Jumping61]
            || Service.Conditions[Dalamud.Game.ClientState.Conditions.ConditionFlag.BetweenAreas]
            || Service.Conditions[Dalamud.Game.ClientState.Conditions.ConditionFlag.BetweenAreas51]
            || Service.Conditions[Dalamud.Game.ClientState.Conditions.ConditionFlag.Mounted]
            || Service.Conditions[Dalamud.Game.ClientState.Conditions.ConditionFlag.SufferingStatusAffliction]
            || Service.Conditions[Dalamud.Game.ClientState.Conditions.ConditionFlag.SufferingStatusAffliction2]
            || Service.Conditions[Dalamud.Game.ClientState.Conditions.ConditionFlag.RolePlaying]
            || Service.Conditions[Dalamud.Game.ClientState.Conditions.ConditionFlag.InFlight]
            //避免技能队列激活的时候反复按。
            || *(bool*)((IntPtr)ActionManager.Instance() + 0x68)) return;

        //GCD
        if (WeaponRemain <= Service.Configuration.WeaponFaster)
        {
            if (!_weaponDelayStopwatch.IsRunning)
            {
                _weaponDelayStopwatch.Start();
                return;
            }
            else if (_weaponDelayStopwatch.ElapsedMilliseconds > _weaponRandomDelay)
            {
                _weaponDelayStopwatch.Stop();
                _weaponDelayStopwatch.Reset();

                RSCommands.DoAnAction(true);

                Random ran = new Random(DateTime.Now.Millisecond);
                _weaponRandomDelay = (long)(ran.NextDouble() * Service.Configuration.WeaponDelay * 1000);

                return;
            }
            return;
        }

        //要超出GCD了，那就不放技能了。
        if (WeaponRemain < Service.Configuration.WeaponInterval
            || WeaponElapsed < Service.Configuration.WeaponInterval)
        {
            return;
        }

        //还在咏唱，就不放技能了。
        if (WeaponElapsed <= _lastCastingTotal) return;

        //只剩下最后一个能力技了，然后卡最后！
        if (WeaponRemain < 2 * Service.Configuration.WeaponInterval)
        {
            if (WeaponRemain > Service.Configuration.WeaponInterval + Service.Configuration.WeaponFaster) return;
            RSCommands.DoAnAction(false);

            return;
        }
        else if ((WeaponElapsed - _lastCastingTotal) % Service.Configuration.WeaponInterval <= Service.Configuration.WeaponFaster)
        {
            RSCommands.DoAnAction(false);
        }
    }

    public static void Dispose()
    {
        _weaponDelayStopwatch?.Stop();
    }
}
