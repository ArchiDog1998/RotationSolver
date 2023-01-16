using Dalamud.Game.ClientState.Objects.SubKinds;
using Lumina.Excel.GeneratedSheets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using RotationSolver.Actions.BaseAction;
using RotationSolver.SigReplacers;
using RotationSolver.Data;
using RotationSolver.Helpers;
using RotationSolver.Updaters;
using RotationSolver.Actions;
using RotationSolver.Windows;
using RotationSolver.Localization;

namespace RotationSolver;

internal static class CommandController
{
    internal record NextAct(BaseAction act, DateTime deadTime);

    private static DateTime _fastClickStopwatch = DateTime.Now;
    private static DateTime _specialStateStartTime = DateTime.MinValue;

    private static List<NextAct> NextActs = new List<NextAct>();
    internal static BaseAction NextAction
    {
        get
        {
            var next = NextActs.FirstOrDefault();

            while (next != null && NextActs.Count > 0 && (next.deadTime < DateTime.Now || IActionHelper.IsLastAction(true, next.act)))
            {
                NextActs.RemoveAt(0);
                next = NextActs.FirstOrDefault();
            }
            return next?.act;
        }
    }

    #region UI
    private static string _stateString = "Off";
    private static string _specialString = string.Empty;
    internal static string StateString => _stateString + (string.IsNullOrEmpty(_specialString) ? string.Empty :
        $" - {_specialString}: {Service.Configuration.SpecialDuration - (DateTime.Now - _specialStateStartTime).TotalSeconds:F2}s");
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
        _specialStateStartTime = DateTime.Now;
        if (!HealArea && Service.Configuration.AutoSayingOut) Watcher.Speak("Start Heal Area");
        _specialString = "Heal Area";
        HealArea = true;
        UpdateToast();
    }

    internal static bool HealSingle { get; private set; } = false;
    private static void StartHealSingle()
    {
        _specialStateStartTime = DateTime.Now;
        if (!HealSingle && Service.Configuration.AutoSayingOut) Watcher.Speak("Start Heal Single");
        _specialString = "Heal Single";
        HealSingle = true;
        UpdateToast();
    }

    internal static bool DefenseArea { get; private set; } = false;
    private static void StartDefenseArea()
    {
        _specialStateStartTime = DateTime.Now;
        if (!DefenseArea && Service.Configuration.AutoSayingOut) Watcher.Speak("Start Defense Area");
        _specialString = "Defense Area";
        DefenseArea = true;
        UpdateToast();
    }
    internal static bool DefenseSingle { get; private set; } = false;
    private static void StartDefenseSingle()
    {
        _specialStateStartTime = DateTime.Now;
        if (!DefenseSingle && Service.Configuration.AutoSayingOut) Watcher.Speak("Start Defense Single");
        _specialString = "Defense Single";
        DefenseSingle = true;
        UpdateToast();
    }
    internal static bool EsunaOrShield { get; private set; } = false;
    private static void StartEsunaOrShield()
    {
        _specialStateStartTime = DateTime.Now;
        var role = Service.DataManager.GetExcelSheet<ClassJob>().GetRow(
            Service.ClientState.LocalPlayer.ClassJob.Id).GetJobRole();

        string speak = role == JobRole.Tank ? "Shield" : "Esuna";
        if (!EsunaOrShield && Service.Configuration.AutoSayingOut) Watcher.Speak("Start " + speak);
        _specialString = speak;
        EsunaOrShield = true;
        UpdateToast();
    }
    internal static bool RaiseOrShirk { get; private set; } = false;
    private static void StartRaiseOrShirk()
    {
        _specialStateStartTime = DateTime.Now;
        var role = Service.DataManager.GetExcelSheet<ClassJob>().GetRow(
            Service.ClientState.LocalPlayer.ClassJob.Id).GetJobRole();

        string speak = role == JobRole.Tank ? "Shirk" : "Raise";
        if (!RaiseOrShirk && Service.Configuration.AutoSayingOut) Watcher.Speak("Start " + speak);
        _specialString = speak;

        RaiseOrShirk = true;
        UpdateToast();
    }
    internal static bool Break { get; private set; } = false;
    private static void StartBreak()
    {
        _specialStateStartTime = DateTime.Now;

        if (!Break && Service.Configuration.AutoSayingOut) Watcher.Speak("Start Break");
        _specialString = "Break";
        Break = true;
        UpdateToast();
    }
    internal static bool AntiRepulsion { get; private set; } = false;
    private static void StartAntiRepulsion()
    {
        _specialStateStartTime = DateTime.Now;
        if (!AntiRepulsion && Service.Configuration.AutoSayingOut) Watcher.Speak("Start Anti repulsion");
        _specialString = "Anti repulsion";
        AntiRepulsion = true;
        UpdateToast();
    }

    internal static bool Move { get; private set; } = false;
    private static void StartMove()
    {
        _specialStateStartTime = DateTime.Now;
        if (!Move && Service.Configuration.AutoSayingOut) Watcher.Speak("Start Move");
        _specialString = "Move";
        Move = true;
        UpdateToast();
    }

    internal static void ResetSpecial(bool sayout)
    {
        _specialStateStartTime = DateTime.MinValue;

        if (sayout && Service.Configuration.AutoSayingOut &&
            (HealArea || HealSingle || DefenseArea || DefenseSingle || EsunaOrShield || RaiseOrShirk || Break
            || AntiRepulsion || Move)) Watcher.Speak("End Special");

        HealArea = HealSingle = DefenseArea = DefenseSingle = EsunaOrShield = RaiseOrShirk = Break
            = AntiRepulsion = Move = false;
        _specialString = string.Empty;
    }

    internal static TargetingType RightTargetingType
    {
        get
        {
            if (Service.Configuration.TargetingTypes.Count == 0)
            {
                Service.Configuration.TargetingTypes.Add(TargetingType.Big);
                Service.Configuration.TargetingTypes.Add(TargetingType.Small);
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

        if (AutoTarget)
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

        //0.2s内，不能重复按按钮。
        if (DateTime.Now - _fastClickStopwatch < new TimeSpan(0, 0, 0, 0, 200)) return;
        _fastClickStopwatch = DateTime.Now;

        //Do Action
        var nextAction = ActionUpdater.NextAction;
#if DEBUG
        //if(nextAction is BaseAction acti)
        //Service.ChatGui.Print($"Will Do {acti} {ActionUpdater.WeaponElapsed}");
#endif
        if (nextAction == null) return;
        if (!isGCD && nextAction is BaseAction act1 && act1.IsRealGCD) return;


        if (nextAction.Use())
        {
            if (Service.Configuration.KeyBoardNoise) PreviewUpdater.PulseAtionBar(nextAction.AdjustedID);
            if (nextAction is BaseAction act)
            {
#if DEBUG
                //Service.ChatGui.Print($"{act}, {act.Target.Name}, {ActionUpdater.AbilityRemainCount}, {ActionUpdater.WeaponElapsed}");
#endif
                //Change Target
                if (Service.TargetManager.Target is not PlayerCharacter && (act.Target?.CanAttack() ?? false))
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
            || Service.Conditions[Dalamud.Game.ClientState.Conditions.ConditionFlag.LoggingOut]
            || Service.Conditions[Dalamud.Game.ClientState.Conditions.ConditionFlag.OccupiedInCutSceneEvent]
            || Service.Conditions[Dalamud.Game.ClientState.Conditions.ConditionFlag.BetweenAreas]
            || Service.Conditions[Dalamud.Game.ClientState.Conditions.ConditionFlag.BetweenAreas51])
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

            case "Break":
                StartBreak();
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

            case "EndSpecial":
                ResetSpecial(true);
                return;

            case "AutoBreak":
                Service.Configuration.AutoBreak = !Service.Configuration.AutoBreak;
                Service.ChatGui.Print(string.Format(LocalizationManager.RightLang.Commands_ChangeAutoBreak, Service.Configuration.AutoBreak));

                return;

            default:
                var customCombo = IconReplacer.RightNowCombo;
                if (customCombo != null)
                {
                    foreach (var boolean in customCombo.Config.bools)
                    {
                        if (boolean.name == str)
                        {
                            boolean.value = !boolean.value;

                            Service.ChatGui.Print(string.Format(LocalizationManager.RightLang.Commands_ChangeSettings, boolean.description, boolean.value));

                            return;
                        }
                    }

                    foreach (var combo in customCombo.Config.combos)
                    {
                        if (str.StartsWith(combo.name))
                        {
                            var numStr = str.Substring(combo.name.Length);

                            if (string.IsNullOrEmpty(numStr) || str.Length == 0)
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

                            Service.ChatGui.Print(string.Format(LocalizationManager.RightLang.Commands_ChangeSettings, combo.description, combo.items[combo.value]));

                            return;
                        }
                    }

                    if (str.StartsWith("Insert"))
                    {
                        var subStr = str.Substring(6);
                        var strs = subStr.Split('-');

                        if (strs != null && strs.Length == 2 && double.TryParse(strs[1], out var time))
                        {
                            var actName = strs[0];
                            foreach (var iAct in IconReplacer.RightComboBaseActions)
                            {
                                if (iAct is not BaseAction act) continue;
                                if (!act.IsTimeline) continue;

                                if (actName == act.Name)
                                {
                                    var index = NextActs.FindIndex(i => i.act.ID == act.ID);
                                    var newItem = new NextAct(act, DateTime.Now.AddSeconds(time));
                                    if (index < 0)
                                    {
                                        NextActs.Add(newItem);
                                    }
                                    else
                                    {
                                        NextActs[index] = newItem;
                                    }
                                    NextActs = NextActs.OrderBy(i => i.deadTime).ToList();

                                    Service.ChatGui.Print(string.Format(LocalizationManager.RightLang.Commands_InsertAction, time, act.Name));
                                    return;
                                }
                            }
                        }
                    }

                    break;
                }

                Service.ChatGui.PrintError(LocalizationManager.RightLang.Commands_CannotFind + ": " + str);
                Service.ChatGui.PrintError(LocalizationManager.RightLang.Commands_OpenSettings);
                RotationSolverPlugin.OpenConfigWindow();
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
