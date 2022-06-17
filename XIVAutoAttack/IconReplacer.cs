using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Hooking;
using Dalamud.Logging;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;
using Lumina.Excel.GeneratedSheets;
using XIVAutoAttack.Combos;
using XIVAutoAttack.Combos.Disciplines;
using Action = Lumina.Excel.GeneratedSheets.Action;

namespace XIVAutoAttack;

internal sealed class IconReplacer : IDisposable
{
    private delegate ulong IsIconReplaceableDelegate(uint actionID);

    private delegate uint GetIconDelegate(IntPtr actionManager, uint actionID);

    private static string _stateString = "Off";
    private static string _specialString = string.Empty;
    internal static string StateString => _stateString + (string.IsNullOrEmpty(_specialString) ? string.Empty : " - " + _specialString);

#if DEBUG
    private unsafe delegate bool UseActionDelegate(IntPtr actionManager, ActionType actionType, uint actionID, uint targetID, uint a4, uint a5, uint a6, void* a7);
    private readonly Hook<UseActionDelegate> getActionHook;
#endif

    private delegate IntPtr GetActionCooldownSlotDelegate(IntPtr actionManager, int cooldownGroup);

    private static Stopwatch _fastClickStopwatch = new Stopwatch();
    private static Stopwatch _specialStateStopwatch = new Stopwatch();
    internal static uint LastAction { get; private set; } = 0;

    private static bool _autoAttack = false;
    internal static bool AutoAttack
    {
        get => _autoAttack;
        set
        {
            if (_autoAttack != value)
            {
                _autoAttack = value;
                if (!value)
                {
                    if (Service.Configuration.AutoSayingOut) CustomCombo.Speak("Cancel");
                    _stateString = "Off";
                    UpdateToast();
                }
            }
        }
    }
    private static bool _attackBig = true;

    internal static bool AttackBig
    {
        get => _attackBig;
        set
        {
            string speak = value ? "Big" : "Small";
            if (Service.Configuration.AutoSayingOut) CustomCombo.Speak("Attack " + speak);
            _stateString = speak;
            AutoTarget = true;
            AutoAttack = true;
            if (_attackBig != value)
            {
                _attackBig = value;
            }
            UpdateToast();
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
                if (Service.Configuration.AutoSayingOut) CustomCombo.Speak("Manual");
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
    internal static void StartHealArea()
    {
        bool last = HealArea;
        ResetSpecial(last);
        if (!last)
        {
            _specialStateStopwatch.Start();
            if (Service.Configuration.AutoSayingOut) CustomCombo.Speak("Start Heal Area");
            _specialString = "Heal Area";
            HealArea = true;
            UpdateToast();

        }
    }
    internal static bool HealSingle { get; private set; } = false;
    internal static void StartHealSingle()
    {
        bool last = HealSingle;
        ResetSpecial(last);
        if (!last)
        {
            _specialStateStopwatch.Start();
            if (Service.Configuration.AutoSayingOut) CustomCombo.Speak("Start Heal Single");
            _specialString = "Heal Single";
            HealSingle = true;
            UpdateToast();

        }
    }
    internal static bool DefenseArea { get; private set; } = false;
    internal static void StartDefenseArea()
    {
        bool last = DefenseArea;
        ResetSpecial(last);
        if (!last)
        {
            _specialStateStopwatch.Start();
            if (Service.Configuration.AutoSayingOut) CustomCombo.Speak("Start Defense Area");
            _specialString = "Defense Area";
            DefenseArea = true;
            UpdateToast();

        }
    }
    internal static bool DefenseSingle { get; private set; } = false;
    internal static void StartDefenseSingle()
    {
        bool last = DefenseSingle;
        ResetSpecial(last);
        if (!last)
        {
            _specialStateStopwatch.Start();
            if (Service.Configuration.AutoSayingOut) CustomCombo.Speak("Start Defense Single");
            _specialString = "Defense Single";
            DefenseSingle = true;
            UpdateToast();

        }
    }
    internal static bool EsunaOrShield { get; private set; } = false;
    internal static void StartEsunaOrShield()
    {
        bool last = EsunaOrShield;
        ResetSpecial(last);
        if (!last)
        {
            _specialStateStopwatch.Start();
            Role role = (Role)XIVAutoAttackPlugin.AllJobs.First(job => job.RowId == Service.ClientState.LocalPlayer.ClassJob.Id).Role;
            string speak = role == Role.防护 ? "Shield" : "Esuna";
            if (Service.Configuration.AutoSayingOut) CustomCombo.Speak("Start " + speak);
            _specialString = speak;
            EsunaOrShield = true;
            UpdateToast();

        }
    }
    internal static bool RaiseOrShirk { get; private set; } = false;
    internal static void StartRaiseOrShirk()
    {
        bool last = RaiseOrShirk;
        ResetSpecial(last);
        if (!last)
        {
            _specialStateStopwatch.Start();
            Role role = (Role)XIVAutoAttackPlugin.AllJobs.First(job => job.RowId == Service.ClientState.LocalPlayer.ClassJob.Id).Role;
            string speak = role == Role.防护 ? "Shirk" : "Raise";
            if (Service.Configuration.AutoSayingOut) CustomCombo.Speak("Start " + speak);
            _specialString = speak;

            RaiseOrShirk = true;
            UpdateToast();

        }
    }
    internal static bool BreakorProvoke { get; private set; } = false;
    internal static void StartBreakOrProvoke()
    {
        bool last = BreakorProvoke;
        ResetSpecial(last);
        if (!last)
        {
            _specialStateStopwatch.Start();
            Role role = (Role)XIVAutoAttackPlugin.AllJobs.First(job => job.RowId == Service.ClientState.LocalPlayer.ClassJob.Id).Role;
            string speak = role == Role.防护 ? "Provoke" : "Break";
            if (Service.Configuration.AutoSayingOut) CustomCombo.Speak("Start " + speak);
            _specialString = speak;
            BreakorProvoke = true;
            UpdateToast();

        }
    }
    internal static bool AntiRepulsion { get; private set; } = false;
    internal static void StartAntiRepulsion()
    {
        bool last = AntiRepulsion;
        ResetSpecial(last);
        if (!last)
        {
            _specialStateStopwatch.Start();
            if (Service.Configuration.AutoSayingOut) CustomCombo.Speak("Start Anti repulsion");
            _specialString = "Anti repulsion";
            AntiRepulsion = true;
            UpdateToast();

        }
    }

    internal static bool Move { get; private set; } = false;
    internal static void StartMove()
    {
        bool last = Move;
        ResetSpecial(last);
        if (!last)
        {
            _specialStateStopwatch.Start();
            if (Service.Configuration.AutoSayingOut) CustomCombo.Speak("Start Move");
            _specialString = "Move";
            Move = true;
            UpdateToast();
        }
    }

    private static void UpdateToast()
    {
        if (!Service.Configuration.UseToast) return;

        Service.ToastGui.ShowQuest(" " + StateString, new Dalamud.Game.Gui.Toast.QuestToastOptions()
        {
            IconId = 101,
        });
    }

    internal static void ResetSpecial(bool sayout)
    {
        _specialStateStopwatch.Stop();
        _specialStateStopwatch.Reset();
        HealArea = HealSingle = DefenseArea = DefenseSingle = EsunaOrShield = RaiseOrShirk = BreakorProvoke
            = AntiRepulsion = Move = false;
        if (sayout && Service.Configuration.AutoSayingOut) CustomCombo.Speak("End Special");
        _specialString = string.Empty;
    }

    private static SortedList<Role, CustomCombo[]> _customCombosDict;
    internal static SortedList<Role, CustomCombo[]> CustomCombosDict
    {
        get
        {
            if (_customCombosDict == null)
            {
                SetStaticValues();
            }
            return _customCombosDict;
        }
    }
    private static CustomCombo[] _customCombos;
    internal static CustomCombo[] CustomCombos
    {
        get
        {
            if (_customCombos == null)
            {
                SetStaticValues();
            }
            return _customCombos;
        }
    }

    private readonly Hook<IsIconReplaceableDelegate> isIconReplaceableHook;

    private readonly Hook<GetIconDelegate> getIconHook;

    private IntPtr actionManager = IntPtr.Zero;


    public IconReplacer()
    {
        unsafe
        {
            getIconHook = new((IntPtr)ActionManager.fpGetAdjustedActionId, GetIconDetour);

#if DEBUG
            getActionHook = new((IntPtr)ActionManager.fpUseAction, UseAction);
            getActionHook.Enable();
#endif
        }
        isIconReplaceableHook = new Hook<IsIconReplaceableDelegate>(Service.Address.IsActionIdReplaceable, IsIconReplaceableDetour);
        getIconHook.Enable();
        isIconReplaceableHook.Enable();
    }

#if DEBUG
    private unsafe bool UseAction(IntPtr actionManager, ActionType actionType, uint actionID, uint targetID = 3758096384u, uint a4 = 0u, uint a5 = 0u, uint a6 = 0u, void* a7 = null)
    {
        var a = actionType == ActionType.Spell ? Service.DataManager.GetExcelSheet<Action>().GetRow(actionID)?.Name : Service.DataManager.GetExcelSheet<Item>().GetRow(actionID)?.Name;
        //Service.ChatGui.Print(a + ", " + actionType.ToString() + ", " + actionID.ToString() + ", " + a4.ToString() + ", " + a5.ToString() + ", " + a6.ToString());
        return getActionHook.Original.Invoke(actionManager, actionType, actionID, targetID, a4, a5, a6, a7);
    }
#endif
    private static void SetStaticValues()
    {
        _customCombos = (from t in Assembly.GetAssembly(typeof(CustomCombo)).GetTypes()
                         where t.BaseType?.BaseType == typeof(CustomCombo)
                         select (CustomCombo)Activator.CreateInstance(t)).ToArray();

        _customCombosDict = new SortedList<Role, CustomCombo[]>
            (_customCombos.GroupBy(g => g.Role).ToDictionary(set => set.Key, set => set.OrderBy(i =>i.JobID).ToArray()));
    }

    public void Dispose()
    {
        getIconHook.Dispose();
        isIconReplaceableHook.Dispose();
        _fastClickStopwatch.Stop();
        _specialStateStopwatch.Stop();
#if DEBUG
        getActionHook.Dispose();
#endif
    }

    internal uint OriginalHook(uint actionID)
    {
        return getIconHook.Original.Invoke(actionManager, actionID);
    }

    private unsafe uint GetIconDetour(IntPtr actionManager, uint actionID)
    {
        this.actionManager = actionManager;
        return RemapActionID(actionID);
    }

    internal unsafe void DoAnAction(bool isGCD)
    {
        //停止特殊状态
        if (_specialStateStopwatch.IsRunning && _specialStateStopwatch.ElapsedMilliseconds > Service.Configuration.SpecialDuration * 1000)
        {
            ResetSpecial(true);
        }

        if (!AutoAttack)
        {
            return;
        }

        //0.2s内，不能重复按按钮。
        if (_fastClickStopwatch.IsRunning && _fastClickStopwatch.ElapsedMilliseconds < 200) return;
        _fastClickStopwatch.Restart();

        PlayerCharacter localPlayer = Service.ClientState.LocalPlayer;
        if (localPlayer == null) return;

        foreach (CustomCombo customCombo in CustomCombos)
        {
            if (customCombo.JobID != localPlayer.ClassJob.Id) continue;

            if (!customCombo.TryInvoke(CustomCombo.GeneralActions.Repose.ID, Service.Address.LastComboAction, Service.Address.ComboTime,
                 localPlayer.Level, out var newiAction))
            {
                return;
            }

            if (!isGCD && newiAction is BaseAction act1 && act1.IsRealGCD) return;

#if DEBUG
            //Service.ChatGui.Print(newiAction.ID.ToString());
            //Service.ChatGui.Print(TargetHelper.WeaponRemain.ToString() + newiAction.Action.Name + TargetHelper.AbilityRemainCount.ToString());
#endif
            if (newiAction.Use() && newiAction is BaseAction act)
            {
#if DEBUG
                //Service.ChatGui.Print(newiAction.ID.ToString());
                Service.ChatGui.Print(TargetHelper.WeaponRemain.ToString() + act.Action.Name + TargetHelper.AbilityRemainCount.ToString());
#endif

                if (TargetHelper.CanAttack(act.Target))
                {
                    Service.TargetManager.SetTarget(act.Target);
                }
                if (act.ID != LastAction)
                {
                    LastAction = newiAction.ID;
                    foreach (var item in Service.Configuration.Events)
                    {
                        if (item.Name == act.Action.Name)
                        {
                            if (item.MacroIndex < 0 || item.MacroIndex > 99) return;

                            TargetHelper.Macros.Enqueue(new MacroItem(act.Target, item.IsShared ? RaptureMacroModule.Instance->Shared[item.MacroIndex] :
                                RaptureMacroModule.Instance->Individual[item.MacroIndex]));

                            return;
                        }
                    }
                }
            }
            return;
        }
    }

    private uint RemapActionID(uint actionID)
    {
        try
        {
            PlayerCharacter localPlayer = Service.ClientState.LocalPlayer;
            if (localPlayer == null || Service.Configuration.NeverReplaceIcon)
            {
                return OriginalHook(actionID);
            }

            byte level = localPlayer.Level;
            foreach (CustomCombo customCombo in CustomCombos)
            {
                if (customCombo.JobID != localPlayer.ClassJob.Id) continue;

                if (customCombo.TryInvoke(actionID, Service.Address.LastComboAction, Service.Address.ComboTime, level, out var newAction))
                {
                    if (newAction is BaseAction) return OriginalHook(newAction.ID);
                    //else return 7546;
                }
            }

            return OriginalHook(actionID);
        }
        catch (Exception ex)
        {
            PluginLog.Error(ex, "Don't crash the game", Array.Empty<object>());
            return OriginalHook(actionID);
        }
    }

    private ulong IsIconReplaceableDetour(uint actionID)
    {
        return 1uL;
    }
}
