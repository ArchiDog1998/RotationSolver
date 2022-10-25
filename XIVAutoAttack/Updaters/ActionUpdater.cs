using FFXIVClientStructs.FFXIV.Client.Game;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XIVAutoAttack.Updaters
{
    internal static class ActionUpdater
    {
        [EditorBrowsable(EditorBrowsableState.Never)]
        internal static float WeaponRemain { get; private set; } = 0;

        [EditorBrowsable(EditorBrowsableState.Never)]
        internal static float WeaponTotal { get; private set; } = 0;

        [EditorBrowsable(EditorBrowsableState.Never)]
        internal static float WeaponElapsed { get; private set; } = 0;

        [EditorBrowsable(EditorBrowsableState.Never)]
        internal static bool InBattle { get; private set; } = false;
        internal static byte AbilityRemainCount { get; private set; } = 0;


        internal static unsafe void UpdateWeaponTime()
        {
            InBattle = Service.Conditions[Dalamud.Game.ClientState.Conditions.ConditionFlag.InCombat];

            var instance = ActionManager.Instance();
            var spell = ActionType.Spell;

            var weapontotal = instance->GetRecastTime(spell, 11);
            WeaponElapsed = instance->GetRecastTimeElapsed(spell, 11);
            WeaponRemain = Math.Max(weapontotal - WeaponElapsed,
                Service.ClientState.LocalPlayer.TotalCastTime - Service.ClientState.LocalPlayer.CurrentCastTime);

            var min = Math.Max(weapontotal - Service.Configuration.WeaponInterval, 0);
            AbilityRemainCount = (byte)(Math.Min(WeaponRemain, min) / Service.Configuration.WeaponInterval);

            if(weapontotal > 0)
            {
                WeaponTotal = weapontotal;
            }
        }


        static readonly Stopwatch _weaponDelayStopwatch = new Stopwatch();
        //static readonly Stopwatch _weaponAbilityStopwatch = new Stopwatch();
        static long _weaponRandomDelay = 0;
        static float _lastCastingTotal = 0;
        internal static void DoAction()
        {
            if (Service.Conditions[Dalamud.Game.ClientState.Conditions.ConditionFlag.OccupiedInCutSceneEvent]
                || Service.Conditions[Dalamud.Game.ClientState.Conditions.ConditionFlag.OccupiedInQuestEvent]
                || Service.Conditions[Dalamud.Game.ClientState.Conditions.ConditionFlag.Occupied33]
                || Service.Conditions[Dalamud.Game.ClientState.Conditions.ConditionFlag.Occupied38]
                || Service.Conditions[Dalamud.Game.ClientState.Conditions.ConditionFlag.Jumping61]
                || Service.Conditions[Dalamud.Game.ClientState.Conditions.ConditionFlag.Mounted]
                || Service.Conditions[Dalamud.Game.ClientState.Conditions.ConditionFlag.SufferingStatusAffliction]
                || Service.Conditions[Dalamud.Game.ClientState.Conditions.ConditionFlag.SufferingStatusAffliction2]
                || Service.Conditions[Dalamud.Game.ClientState.Conditions.ConditionFlag.InFlight]) return;

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

                    Service.IconReplacer.DoAnAction(true);

                    Random ran = new Random(DateTime.Now.Millisecond);
                    _weaponRandomDelay = (long)(ran.NextDouble() * Service.Configuration.WeaponDelay * 1000);

                    return;
                }

                return;
            }

            //确定读条时间。
            if (WeaponElapsed < 0.3)
            {
                //能力技就不用提前了。
                _lastCastingTotal = Service.ClientState.LocalPlayer.TotalCastTime;

                //补上读条税
                if (_lastCastingTotal > 0) _lastCastingTotal += 0.1f + Service.Configuration.WeaponFaster;
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
            if (AbilityRemainCount == 1)
            {
                if (WeaponRemain > Service.Configuration.WeaponInterval + Service.Configuration.WeaponFaster) return;
                Service.IconReplacer.DoAnAction(false);
                return;
            }

            else if ((WeaponElapsed - _lastCastingTotal) % Service.Configuration.WeaponInterval <= Service.Configuration.WeaponFaster)
            {
                Service.IconReplacer.DoAnAction(false);
                return;
            }
        }

        public static void Dispose()
        {
            _weaponDelayStopwatch.Stop();
        }
    }
}
