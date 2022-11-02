using Dalamud.Game.ClientState.JobGauge.Types;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Logging;
using FFXIVClientStructs.FFXIV.Client.Game;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XIVAutoAttack.Actions;
using XIVAutoAttack.Actions.BaseAction;
using XIVAutoAttack.Combos.CustomCombo;
using XIVAutoAttack.Data;
using XIVAutoAttack.Helpers;
using XIVAutoAttack.SigReplacers;

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

        internal static float AbilityRemain { get; private set; } = 0;


        internal static IAction NextAction { get; private set; }
#if DEBUG
        internal static Exception Exception { get; private set; }
#endif
        internal static void UpdateNextAction()
        {
            PlayerCharacter localPlayer = Service.ClientState.LocalPlayer;
            if (localPlayer == null) return;
            NextAction = null;

            try
            {
                foreach (CustomCombo customCombo in IconReplacer.CustomCombos)
                {
                    if (customCombo.JobID != localPlayer.ClassJob.Id) continue;

                    if (customCombo.TryInvoke(Service.Address.LastComboAction, Service.Address.ComboTime, out var newAction))
                    {
                        NextAction = newAction;
                        return;
                    }
                    break;
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                Exception = ex;
                return;
#endif
            }
#if DEBUG
            Exception = null;
#endif
        }

        internal static void UpdateActionInfo()
        {
            //结束战斗，那就关闭。
            if(Service.ClientState.LocalPlayer.CurrentHp == 0) CommandController.AutoAttack = false;
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
            if (player.IsCasting) weapontotal = Math.Max(weapontotal, player.TotalCastTime + 0.1f);

            WeaponElapsed = instance->GetRecastTimeElapsed(ActionType.Spell, 11);
            WeaponRemain = Math.Max(weapontotal - WeaponElapsed, player.TotalCastTime - player.CurrentCastTime);

            //确定读条时间。
            if (WeaponElapsed < 0.3)
            {
                _lastCastingTotal = player.TotalCastTime;

                //能力技就不用提前了。
                //补上读条税
                if (_lastCastingTotal > 0) _lastCastingTotal += 0.1f + Service.Configuration.WeaponFaster;
            }

            //确认能力技的相关信息
            var interval = Service.Configuration.WeaponInterval;
            if (WeaponRemain < interval || WeaponElapsed == 0)
            {
                AbilityRemain = WeaponRemain + interval;
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
        internal static float MPUpdateElapsed => (float)(DateTime.Now - _lastMPUpdate).TotalSeconds % 3;
        [Obsolete]
        public static float MPNextUpInCurrGCD => (3 - (MPUpdateElapsed - WeaponElapsed)) % 3;
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
        internal static long _weaponRandomDelay = 0;
        internal static float _lastCastingTotal = 0;
        internal static void DoAction()
        {
            if (Service.Conditions[Dalamud.Game.ClientState.Conditions.ConditionFlag.OccupiedInCutSceneEvent]
                || Service.Conditions[Dalamud.Game.ClientState.Conditions.ConditionFlag.OccupiedInQuestEvent]
                || Service.Conditions[Dalamud.Game.ClientState.Conditions.ConditionFlag.Occupied33]
                || Service.Conditions[Dalamud.Game.ClientState.Conditions.ConditionFlag.Occupied38]
                || Service.Conditions[Dalamud.Game.ClientState.Conditions.ConditionFlag.Jumping61]
                || Service.Conditions[Dalamud.Game.ClientState.Conditions.ConditionFlag.BetweenAreas]
                || Service.Conditions[Dalamud.Game.ClientState.Conditions.ConditionFlag.BetweenAreas51]
                || Service.Conditions[Dalamud.Game.ClientState.Conditions.ConditionFlag.Mounted]
                || Service.Conditions[Dalamud.Game.ClientState.Conditions.ConditionFlag.SufferingStatusAffliction]
                || Service.Conditions[Dalamud.Game.ClientState.Conditions.ConditionFlag.SufferingStatusAffliction2]
                || Service.Conditions[Dalamud.Game.ClientState.Conditions.ConditionFlag.RolePlaying]
                || Service.Conditions[Dalamud.Game.ClientState.Conditions.ConditionFlag.LoggingOut]
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

            //要超出GCD了，那就不放技能了。
            if (WeaponRemain < Service.Configuration.WeaponInterval
                || WeaponElapsed < Service.Configuration.WeaponInterval)
            {
                return;
            }

            //还在咏唱，就不放技能了。
            if (WeaponElapsed <= _lastCastingTotal) return;

            //只剩下最后一个能力技了，然后卡最后！
            if (AbilityRemainCount == 0)
            {
                if (WeaponRemain > Service.Configuration.WeaponInterval + Service.Configuration.WeaponFaster) return;
                CommandController.DoAnAction(false);

                return;
            }
            else
            if ((WeaponElapsed - _lastCastingTotal) % Service.Configuration.WeaponInterval <= Service.Configuration.WeaponFaster)
            {
                CommandController.DoAnAction(false);
                //Service.ChatGui.Print("Other " + AbilityRemainCount.ToString());
            }
        }

        public static void Dispose()
        {
            _weaponDelayStopwatch?.Stop();
        }
    }
}
