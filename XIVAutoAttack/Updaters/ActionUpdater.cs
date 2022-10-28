using Dalamud.Game.ClientState.JobGauge.Types;
using FFXIVClientStructs.FFXIV.Client.Game;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XIVAutoAttack.Controllers;
using XIVAutoAttack.Data;
using XIVAutoAttack.Helpers;

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
        internal static bool InCombat { get; private set; } = false;
        internal static byte AbilityRemainCount { get; private set; } = 0;


        internal static void UpdateActionInfo()
        {
            InCombat = Service.Conditions[Dalamud.Game.ClientState.Conditions.ConditionFlag.InCombat];

            UpdateWeaponTime();
            UPdateMPTimer();
        }

        private static unsafe void UpdateWeaponTime()
        {
            var player = Service.ClientState.LocalPlayer;
            if (player == null) return;

            var instance = ActionManager.Instance();

            var weapontotal = instance->GetRecastTime(ActionType.Spell, 11);
            if (player.IsCasting)
            {
                weapontotal = Math.Max(weapontotal, player.TotalCastTime + 0.1f);
            }

            WeaponElapsed = instance->GetRecastTimeElapsed(ActionType.Spell, 11);
            WeaponRemain = Math.Max(weapontotal - WeaponElapsed,
                player.TotalCastTime - player.CurrentCastTime);

            var min = Math.Max(weapontotal - Service.Configuration.WeaponInterval, 0);
            AbilityRemainCount = (byte)(Math.Min(WeaponRemain, min) / Service.Configuration.WeaponInterval);

            if (weapontotal > 0) WeaponTotal = weapontotal;
        }

        static uint _lastMP = 0;
        static DateTime _lastMPUpdate = DateTime.Now;
        public static double MPUpdateElapsed => (DateTime.Now - _lastMPUpdate).TotalSeconds % 3;
        public static float MPNextUpInCurrGCD => (3 - ((float)MPUpdateElapsed - WeaponElapsed))%3;
        private static void UPdateMPTimer()
        {
            var player = Service.ClientState.LocalPlayer;
            if (player == null) return;

            //不是黑魔不考虑啊
            if (player.ClassJob.Id != 25) return;

            //有醒梦，就算了啊
            if (player.HaveStatus(ObjectStatus.LucidDreaming)) return;

            if(_lastMP < player.CurrentMp)
            {
                _lastMPUpdate = DateTime.Now;
            }
            _lastMP = player.CurrentMp;
        }

        static readonly Stopwatch _weaponDelayStopwatch = new Stopwatch();
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

                    CommandController.DoAnAction(true);

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
                CommandController.DoAnAction(false);
                return;
            }

            else if ((WeaponElapsed - _lastCastingTotal) % Service.Configuration.WeaponInterval <= Service.Configuration.WeaponFaster)
            {
                CommandController.DoAnAction(false);
                return;
            }
        }

        public static void Dispose()
        {
            _weaponDelayStopwatch.Stop();
        }
    }
}
