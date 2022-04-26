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
using XIVComboPlus.Combos;
using Action = Lumina.Excel.GeneratedSheets.Action;

namespace XIVComboPlus;

internal sealed class IconReplacer : IDisposable
{
    private delegate ulong IsIconReplaceableDelegate(uint actionID);

    private delegate uint GetIconDelegate(IntPtr actionManager, uint actionID);

    private delegate IntPtr GetActionCooldownSlotDelegate(IntPtr actionManager, int cooldownGroup);

    //[StructLayout(LayoutKind.Explicit)]
    //internal struct CooldownData
    //{
    //    [FieldOffset(0)]
    //    public bool IsCooldown;

    //    [FieldOffset(4)]
    //    public uint ActionID;

    //    [FieldOffset(8)]
    //    public float CooldownElapsed;

    //    [FieldOffset(12)]
    //    public float CooldownTotal;

    //    public float CooldownRemaining
    //    {
    //        get
    //        {
    //            if (!IsCooldown)
    //            {
    //                return 0f;
    //            }
    //            return CooldownTotal - CooldownElapsed;
    //        }
    //    }
    //}

    private static Stopwatch _fastClickStopwatch = new Stopwatch();

    private static Stopwatch _specialStateStopwatch = new Stopwatch();

    internal static uint LastAction { get; private set; } = 0;

    private static bool _autoAttack = false;
    internal static bool AutoAttack
    {
        private get => _autoAttack;
        set
        {
            if (_autoAttack != value)
            {
                _autoAttack = value;
                CustomCombo.Speak(value ? "Attack" : "Cancel");
            }
        }
    }
    private static bool _autoTarget = true;
    internal static bool AutoTarget
    {
        get => _autoTarget;
        set
        {
            if (_autoTarget != value)
            {
                _autoTarget = value;
                CustomCombo.Speak(value ? "Auto" : "Manual");
            }
        }
    }

    internal static bool HealArea { get; private set; } = false;
    internal static void StartHealArea()
    {
        ResetSpecial();
        _specialStateStopwatch.Start();
        CustomCombo.Speak("Start Heal Area");
        HealArea = true;
    }
    internal static bool HealSingle { get; private set; } = false;
    internal static void StartHealSingle()
    {
        ResetSpecial();
        _specialStateStopwatch.Start();
        CustomCombo.Speak("Start Heal Single");
        HealSingle = true;
    }
    internal static bool DefenseArea { get; private set; } = false;
    internal static void StartDefenseArea()
    {
        ResetSpecial();
        _specialStateStopwatch.Start();
        CustomCombo.Speak("Start Defense Area");
        DefenseArea = true;
    }
    internal static bool DefenseSingle { get; private set; } = false;
    internal static void StartDefenseSingle()
    {
        ResetSpecial();
        _specialStateStopwatch.Start();
        CustomCombo.Speak("Start Defense Single");
        DefenseSingle = true;
    }
    internal static bool Esuna { get; private set; } = false;
    internal static void StartEsuna()
    {
        ResetSpecial();
        _specialStateStopwatch.Start();
        CustomCombo.Speak("Start Esuna");
        Esuna = true;
    }
    internal static bool Raise { get; private set; } = false;
    internal static void StartRaise()
    {
        ResetSpecial();
        _specialStateStopwatch.Start();
        CustomCombo.Speak("Start Raise");
        Raise = true;
    }
    internal static bool AntiRepulsion { get; private set; } = false;
    internal static void StartAntiRepulsion()
    {
        ResetSpecial();
        _specialStateStopwatch.Start();
        CustomCombo.Speak("Start Anti repulsion");
        AntiRepulsion = true;
    }

    private static void ResetSpecial()
    {
        _specialStateStopwatch.Stop();
        _specialStateStopwatch.Reset();
        HealArea = HealSingle = DefenseArea = DefenseSingle = Esuna = Raise
            = AntiRepulsion = false;
    }

    private static SortedList<string, CustomCombo[]> _customCombosDict;
    internal static SortedList<string, CustomCombo[]> CustomCombosDict
    {
        get
        {
            if(_customCombosDict == null)
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
        }
        //getIconHook = new Hook<GetIconDelegate>(Service.Address.GetAdjustedActionId, GetIconDetour);
        isIconReplaceableHook = new Hook<IsIconReplaceableDelegate>(Service.Address.IsActionIdReplaceable, IsIconReplaceableDetour);
        getIconHook.Enable();
        isIconReplaceableHook.Enable();
    }

    private static void SetStaticValues()
    {
        _customCombos = (from t in Assembly.GetAssembly(typeof(CustomCombo))!.GetTypes()
                         where t.BaseType.BaseType == typeof(CustomCombo)
                         select (CustomCombo)Activator.CreateInstance(t) into combo
                         orderby combo.JobID
                         select combo).ToArray();

        _customCombosDict = new SortedList<string, CustomCombo[]>
            (_customCombos.GroupBy(g => g.RoleName).ToDictionary(set => set.Key, set => set.Reverse().ToArray()));
    }

    public void Dispose()
    {
        getIconHook.Dispose();
        isIconReplaceableHook.Dispose();
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

    internal void DoAnAction()
    {
        //停止特殊状态
        if (_specialStateStopwatch.IsRunning && _specialStateStopwatch.ElapsedMilliseconds > 5000)
        {
            ResetSpecial();
            CustomCombo.Speak("End Special");
        }

        if (!AutoAttack) return;

        //0.1s内，不能重复按按钮。
        if (_fastClickStopwatch.IsRunning && _fastClickStopwatch.ElapsedMilliseconds < 100) return;

        PlayerCharacter localPlayer = Service.ClientState.LocalPlayer;
        if (localPlayer == null) return;

        foreach (CustomCombo customCombo in CustomCombos)
        {
            if (customCombo.JobID != localPlayer.ClassJob.Id) continue;

            if (!customCombo.TryInvoke(CustomCombo.GeneralActions.Repose.ActionID, Service.Address.LastComboAction, Service.Address.ComboTime,
                 localPlayer.Level, out var newAction)) return;

            if (newAction.UseAction())
            {
                if (TargetHelper.CanAttack(newAction.Target))
                {
                    Service.TargetManager.SetTarget(newAction.Target);
                }
                if(newAction.ActionID != LastAction)
                {
                    LastAction = newAction.ActionID;
                    newAction.SayingOut();
                }
                _fastClickStopwatch.Restart();
            }
            return;
        }
    }

    internal uint RemapActionID(uint actionID)
    {
        try
        {
            PlayerCharacter localPlayer = Service.ClientState.LocalPlayer;
            if (localPlayer == null)
            {
                return OriginalHook(actionID);
            }

            byte level = localPlayer.Level;
            foreach (CustomCombo customCombo in CustomCombos)
            {
                if (customCombo.JobID != localPlayer.ClassJob.Id) continue;

                if (customCombo.TryInvoke(actionID, Service.Address.LastComboAction, Service.Address.ComboTime, level, out var newAction))
                {
                    return OriginalHook(newAction.ActionID);
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

    internal static void SetEnable(string comboName, bool enable)
    {
        foreach (var combo in CustomCombos)
        {
            if(combo.JobName == comboName)
            {
                combo.IsEnabled = enable;
                return;
            }
        }
    }
}
