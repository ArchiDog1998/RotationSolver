using Dalamud.Game.ClientState.Objects.SubKinds;
using Lumina.Data.Parsing;
using Lumina.Excel.GeneratedSheets;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using XIVAutoAttack.Actions;
using XIVAutoAttack.Actions.BaseAction;
using XIVAutoAttack.Combos.CustomCombo;
using XIVAutoAttack.Data;
using XIVAutoAttack.Helpers;
using XIVAutoAttack.SigReplacers;
using XIVAutoAttack.Updaters;
using XIVAutoAttack.Windows;
using static XIVAutoAttack.SigReplacers.PluginAddressResolver;

namespace XIVAutoAttack
{
    internal static class CommandController
    {
        private static DateTime _fastClickStopwatch = DateTime.Now;
        private static DateTime _specialStateStartTime = DateTime.MinValue;

        private static BaseAction _nextAction;
        private static TimeSpan _actionTime = TimeSpan.Zero;
        private static DateTime _actionAddTime = DateTime.Now;
        internal static BaseAction NextAction 
        {
            get
            {
                var time = DateTime.Now - _actionAddTime;
                if (time > _actionTime) _nextAction = null;
                if (IActionHelper.IsLastAction(true, _nextAction)) _nextAction = null;
                return _nextAction;
            }
        }


        #region UI
        private static string _stateString = "Off";
        private static string _specialString = string.Empty;
        internal static string StateString => _stateString + (string.IsNullOrEmpty(_specialString) ? string.Empty : " - " + _specialString);
        private static void UpdateToast()
        {
            if (!Service.Configuration.UseToast) return;

            Service.ToastGui.ShowQuest(" " + StateString, new Dalamud.Game.Gui.Toast.QuestToastOptions()
            {
                IconId = 101,
            });
        }
        #endregion

        #region Settings
        private static bool _autoAttack = false;
        internal static bool AutoAttack
        {
            get => _autoAttack;
            private set
            {
                if (_autoAttack != value)
                {
                    _autoAttack = value;
                    if (!value)
                    {
                        OverlayWindow.ShouldLocation = EnemyLocation.None;
                        if (Service.Configuration.AutoSayingOut) Watcher.Speak("Cancel");
                        _stateString = "Off";
                        UpdateToast();
                    }
                }
            }
        }

        private static bool _autoTarget = true;
        internal static bool AutoTarget
        {
            get => _autoTarget;
            set
            {
                if (!value)
                {
                    if (Service.Configuration.AutoSayingOut) Watcher.Speak("Manual");
                    _stateString = "Manual";
                    UpdateToast();
                }
                if (_autoTarget != value)
                {
                    _autoTarget = value;
                }
            }
        }

        internal static bool HealArea { get; private set; } = false;
        private static void StartHealArea()
        {
            bool last = HealArea;
            ResetSpecial(last);
            if (!last)
            {
                _specialStateStartTime = DateTime.Now;
                if (Service.Configuration.AutoSayingOut) Watcher.Speak("Start Heal Area");
                _specialString = "Heal Area";
                HealArea = true;
                UpdateToast();

            }
        }
        internal static bool HealSingle { get; private set; } = false;
        private static void StartHealSingle()
        {
            bool last = HealSingle;
            ResetSpecial(last);
            if (!last)
            {
                _specialStateStartTime = DateTime.Now;
                if (Service.Configuration.AutoSayingOut) Watcher.Speak("Start Heal Single");
                _specialString = "Heal Single";
                HealSingle = true;
                UpdateToast();

            }
        }
        internal static bool DefenseArea { get; private set; } = false;
        private static void StartDefenseArea()
        {
            bool last = DefenseArea;
            ResetSpecial(last);
            if (!last)
            {
                _specialStateStartTime = DateTime.Now;
                if (Service.Configuration.AutoSayingOut) Watcher.Speak("Start Defense Area");
                _specialString = "Defense Area";
                DefenseArea = true;
                UpdateToast();

            }
        }
        internal static bool DefenseSingle { get; private set; } = false;
        private static void StartDefenseSingle()
        {
            bool last = DefenseSingle;
            ResetSpecial(last);
            if (!last)
            {
                _specialStateStartTime = DateTime.Now;
                if (Service.Configuration.AutoSayingOut) Watcher.Speak("Start Defense Single");
                _specialString = "Defense Single";
                DefenseSingle = true;
                UpdateToast();

            }
        }
        internal static bool EsunaOrShield { get; private set; } = false;
        private static void StartEsunaOrShield()
        {
            bool last = EsunaOrShield;
            ResetSpecial(last);
            if (!last)
            {
                _specialStateStartTime = DateTime.Now;
                Role role = (Role)XIVAutoAttackPlugin.AllJobs.First(job => job.RowId == Service.ClientState.LocalPlayer.ClassJob.Id).Role;
                string speak = role == Role.防护 ? "Shield" : "Esuna";
                if (Service.Configuration.AutoSayingOut) Watcher.Speak("Start " + speak);
                _specialString = speak;
                EsunaOrShield = true;
                UpdateToast();

            }
        }
        internal static bool RaiseOrShirk { get; private set; } = false;
        private static void StartRaiseOrShirk()
        {
            bool last = RaiseOrShirk;
            ResetSpecial(last);
            if (!last)
            {
                _specialStateStartTime = DateTime.Now;
                Role role = (Role)XIVAutoAttackPlugin.AllJobs.First(job => job.RowId == Service.ClientState.LocalPlayer.ClassJob.Id).Role;
                string speak = role == Role.防护 ? "Shirk" : "Raise";
                if (Service.Configuration.AutoSayingOut) Watcher.Speak("Start " + speak);
                _specialString = speak;

                RaiseOrShirk = true;
                UpdateToast();

            }
        }
        internal static bool BreakorProvoke { get; private set; } = false;
        private static void StartBreakOrProvoke()
        {
            bool last = BreakorProvoke;
            ResetSpecial(last);
            if (!last)
            {
                _specialStateStartTime = DateTime.Now;
                Role role = (Role)XIVAutoAttackPlugin.AllJobs.First(job => job.RowId == Service.ClientState.LocalPlayer.ClassJob.Id).Role;
                string speak = role == Role.防护 ? "Provoke" : "Break";
                if (Service.Configuration.AutoSayingOut) Watcher.Speak("Start " + speak);
                _specialString = speak;
                BreakorProvoke = true;
                UpdateToast();

            }
        }
        internal static bool AntiRepulsion { get; private set; } = false;
        private static void StartAntiRepulsion()
        {
            bool last = AntiRepulsion;
            ResetSpecial(last);
            if (!last)
            {
                _specialStateStartTime = DateTime.Now;
                if (Service.Configuration.AutoSayingOut) Watcher.Speak("Start Anti repulsion");
                _specialString = "Anti repulsion";
                AntiRepulsion = true;
                UpdateToast();

            }
        }

        internal static bool Move { get; private set; } = false;
        private static void StartMove()
        {
            bool last = Move;
            ResetSpecial(last);
            if (!last)
            {
                _specialStateStartTime = DateTime.Now;
                if (Service.Configuration.AutoSayingOut) Watcher.Speak("Start Move");
                _specialString = "Move";
                Move = true;
                UpdateToast();
            }
        }

        internal static void ResetSpecial(bool sayout)
        {
            _specialStateStartTime = DateTime.MinValue;
            HealArea = HealSingle = DefenseArea = DefenseSingle = EsunaOrShield = RaiseOrShirk = BreakorProvoke
                = AntiRepulsion = Move = false;
            if (sayout && Service.Configuration.AutoSayingOut) Watcher.Speak("End Special");
            _specialString = string.Empty;
        }

        internal static TargetingType RightTargetingType
        {
            get
            {
                if(Service.Configuration.TargetingTypes.Count == 0)
                {
                    Service.Configuration.TargetingTypes.Add(TargetingType.Big);
                    Service.Configuration.Save();
                }

                return Service.Configuration.TargetingTypes[Service.Configuration.TargetingIndex %= Service.Configuration.TargetingTypes.Count];
            }
        }
        private static void StartAttackSmart()
        {
            if (!AutoAttack)
            {
                AutoAttack = true;
            }
            else
            {
                Service.Configuration.TargetingIndex += 1;
                Service.Configuration.TargetingIndex %= Service.Configuration.TargetingTypes.Count;
            }

            string speak = RightTargetingType.ToString();
            if (Service.Configuration.AutoSayingOut) Watcher.Speak("Attack " + speak);
            _stateString = speak;
            AutoTarget = true;

            UpdateToast();
        }
        #endregion

        internal static unsafe void DoAnAction(bool isGCD)
        {
            //停止特殊状态
            if (_specialStateStartTime != DateTime.MinValue &&
                DateTime.Now - _specialStateStartTime > new TimeSpan(0, 0, 0, 0, (int)(Service.Configuration.SpecialDuration * 1000)))
            {
                ResetSpecial(true);
            }

            if (!AutoAttack)
            {
                return;
            }

            var localPlayer = Service.ClientState.LocalPlayer;
            if (localPlayer == null) return;

            //这个避让的功能，可以再斟酌一下
            //if (Watcher.TimeSinceLastAction.TotalSeconds < 0.2) return;

            //0.2s内，不能重复按按钮。
            if (DateTime.Now - _fastClickStopwatch < new TimeSpan(0, 0, 0, 0, 200)) return;
            _fastClickStopwatch = DateTime.Now;

            //Do Action
            var nextAction = ActionUpdater.NextAction;
#if DEBUG
            //if(nextAction is BaseAction acti)
            //Service.ChatGui.Print($"Will Do {acti} {ActionUpdater.WeaponElapsed}");
#endif
            if(nextAction == null) return;
            if (!isGCD && nextAction is BaseAction act1 && act1.IsRealGCD) return;


            if (nextAction.Use())
            {
                if(Service.Configuration.KeyBoardNoise) PreviewUpdater.PulseAtionBar(nextAction.AdjustedID);
                if (nextAction is BaseAction act)
                {
#if DEBUG
                    Service.ChatGui.Print($"{act}, {act.Target.Name}, {ActionUpdater.AbilityRemainCount}, {ActionUpdater.WeaponElapsed}");
#endif
                    //Change Target
                    if (act.Target?.CanAttack() ?? false)
                    {
                        Service.TargetManager.SetTarget(act.Target);
                    }
                }
            }
            return;
        }

        internal static void AttackCancel()
        {
            AutoAttack = false;
        }

        internal static void UpdateAutoAttack()
        {
            //结束战斗，那就关闭。
            if (Service.ClientState.LocalPlayer.CurrentHp == 0
                || Service.Conditions[Dalamud.Game.ClientState.Conditions.ConditionFlag.LoggingOut])
                AttackCancel();

            //Auto start at count Down.
            else if (Service.Configuration.AutoStartCountdown && CountDown.CountDownTime > 0)
            {
                if (!AutoAttack) StartAttackSmart();
            }
        }

        internal static void DoAutoAttack(string str)
        {
            switch (str)
            {
                case "HealArea":
                    StartHealArea();
                    return;
                case "HealSingle":
                    StartHealSingle();
                    return;
                case "DefenseArea":
                    StartDefenseArea();
                    return;
                case "DefenseSingle":
                    StartDefenseSingle();
                    return;
                case "EsunaShield":
                    StartEsunaOrShield();
                    return;
                case "RaiseShirk":
                    StartRaiseOrShirk();
                    return;
                case "Move":
                    StartMove();
                    return;
                case "AntiRepulsion":
                    StartAntiRepulsion();
                    return;
                case "BreakProvoke":
                    StartBreakOrProvoke();
                    return;
                case "AttackSmart":
                    StartAttackSmart();
                    return;
                case "AttackManual":
                    AutoTarget = false;
                    AutoAttack = true;
                    return;
                case "AttackCancel":
                    AttackCancel();
                    return;
                case "AutoBreak":
                    Service.Configuration.AutoBreak = !Service.Configuration.AutoBreak;
                    Service.ChatGui.Print($"修改自动爆发为{Service.Configuration.AutoBreak}");

                    return;

                default:

                    var customCombo = IconReplacer.RightNowCombo;
                    if(customCombo != null)
                    {
                        foreach (var boolean in customCombo.Config.bools)
                        {
                            if (boolean.name == str)
                            {
                                boolean.value = !boolean.value;

                                Service.ChatGui.Print($"修改{boolean.description}为{boolean.value}");

                                return;
                            }
                        }

                        foreach (var combo in customCombo.Config.combos)
                        {
                            if (str.StartsWith(combo.name))
                            {
                                var numStr = str.Substring(combo.name.Length);

                                if(string.IsNullOrEmpty(numStr) || str.Length == 0)
                                {
                                    combo.value = (combo.value + 1) % combo.items.Length;

                                }
                                else if (int.TryParse(numStr, out int num))
                                {
                                    combo.value = num % combo.items.Length;
                                }
                                else
                                {
                                    for (int i = 0; i < combo.items.Length; i++)
                                    {
                                        if (combo.items[i] == str)
                                        {
                                            combo.value = i;
                                        }
                                    }
                                }

                                Service.ChatGui.Print($"修改{combo.description}为{combo.items[combo.value]}");

                                return;
                            }
                        }

                        if (str.StartsWith("Enable"))
                        {
                            var actName = str.Substring(6);

                            foreach (var act in IconReplacer.AllBaseActions)
                            {
                                if(actName == act.Name)
                                {
                                    act.IsEnabled = true;
                                    Service.ChatGui.Print($"启用\"{act.Name}\"");
                                }
                            }
                        }
                        else if (str.StartsWith("Disable"))
                        {
                            var actName = str.Substring(7);

                            foreach (var act in IconReplacer.AllBaseActions)
                            {
                                if (actName == act.Name)
                                {
                                    act.IsEnabled = false;
                                    Service.ChatGui.Print($"关闭\"{act.Name}\"");
                                }
                            }
                        }
                        else if (str.StartsWith("Insert"))
                        {
                            var subStr = str.Substring(6);
                            var strs = subStr.Split('-');

                            if(strs!= null && strs.Length == 2 && double.TryParse(strs[1], out var time))
                            {
                                var actName = strs[0];
                                foreach (var act in IconReplacer.AllBaseActions)
                                {
                                    if (actName == act.Name)
                                    {
                                        _actionTime = new TimeSpan(0, 0, 0, 0, (int)(time * 1000));
                                        _actionAddTime = DateTime.Now;
                                        _nextAction = act;
                                        Service.ChatGui.Print($"将在{time}s 内使用技能\"{act.Name}\"");
                                    }
                                }
                            }
                        }

                        var result = customCombo.OnCommand(str);
                        if (!string.IsNullOrEmpty(result))
                        {
                            Service.ChatGui.Print("修改结果为：" + result);
                            return;
                        }
                        break;
                    }
                    Service.ChatGui.PrintError("无法识别：" + str);
                    Service.ChatGui.PrintError("已开启设置界面");
                    XIVAutoAttackPlugin.OpenConfigWindow();
                    return;
            }
        }

        /// <summary>
        /// Submit text/command to outgoing chat.
        /// Can be used to enter chat commands.
        /// </summary>
        /// <param name="text">Text to submit.</param>
        public unsafe static void SubmitToChat(string text)
        {
            IntPtr uiModule = Service.GameGui.GetUIModule();

            using (ChatPayload payload = new ChatPayload(text))
            {
                IntPtr mem1 = Marshal.AllocHGlobal(400);
                Marshal.StructureToPtr(payload, mem1, false);

                Service.Address.GetChatBox(uiModule, mem1, IntPtr.Zero, 0);

                Marshal.FreeHGlobal(mem1);
            }
        }
    }
}
