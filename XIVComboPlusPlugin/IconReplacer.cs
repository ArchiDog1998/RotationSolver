using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Hooking;
using Dalamud.Logging;
using XIVComboPlus.Combos;
using Action = Lumina.Excel.GeneratedSheets.Action;

namespace XIVComboPlus;

internal sealed class IconReplacer : IDisposable
{
    private delegate ulong IsIconReplaceableDelegate(uint actionID);

    private delegate uint GetIconDelegate(IntPtr actionManager, uint actionID);

    private delegate IntPtr GetActionCooldownSlotDelegate(IntPtr actionManager, int cooldownGroup);

    [StructLayout(LayoutKind.Explicit)]
    internal struct CooldownData
    {
        [FieldOffset(0)]
        public bool IsCooldown;

        [FieldOffset(4)]
        public uint ActionID;

        [FieldOffset(8)]
        public float CooldownElapsed;

        [FieldOffset(12)]
        public float CooldownTotal;

        public float CooldownRemaining
        {
            get
            {
                if (!IsCooldown)
                {
                    return 0f;
                }
                return CooldownTotal - CooldownElapsed;
            }
        }
    }
    private static SortedList<uint, CustomCombo[]> _customCombosDict;
    internal static SortedList<uint, CustomCombo[]> CustomCombosDict
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

    private HashSet<uint> comboActionIDs = new HashSet<uint>();

    private readonly Dictionary<uint, byte> cooldownGroupCache = new Dictionary<uint, byte>();

    private readonly GetActionCooldownSlotDelegate getActionCooldownSlot;

    public IconReplacer()
    {
        getActionCooldownSlot = Marshal.GetDelegateForFunctionPointer<GetActionCooldownSlotDelegate>(Service.Address.GetActionCooldown);
        getIconHook = new Hook<GetIconDelegate>(Service.Address.GetAdjustedActionId, GetIconDetour);
        isIconReplaceableHook = new Hook<IsIconReplaceableDelegate>(Service.Address.IsActionIdReplaceable, IsIconReplaceableDetour);
        getIconHook.Enable();
        isIconReplaceableHook.Enable();
    }

    private static void SetStaticValues()
    {
        _customCombos = (from t in Assembly.GetAssembly(typeof(CustomCombo))!.GetTypes()
                         where t.BaseType.BaseType == typeof(CustomCombo)
                         select (CustomCombo)Activator.CreateInstance(t) into combo
                         orderby combo.JobID, combo.Priority
                         select combo).ToArray();

        foreach (var combo in _customCombos)
        {
            if (Service.Configuration.EnabledActions.Contains(combo.ComboFancyName))
            {
                combo.IsEnabled = true;
            }
        }

        _customCombosDict = new SortedList<uint, CustomCombo[]>
            (_customCombos.GroupBy(g => g.JobID).ToDictionary(set => set.Key, set => set.ToArray()));
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
        try
        {
            PlayerCharacter localPlayer = Service.ClientState.LocalPlayer;
            uint classId = localPlayer.ClassJob.Id;
            if ((GameObject)(object)localPlayer == null || !CustomCombosDict.ContainsKey(classId))
            {
                return OriginalHook(actionID);
            }

            uint lastComboActionID = *(uint*)(void*)Service.Address.LastComboMove;
            float comboTime = *(float*)(void*)Service.Address.ComboTimer;
            byte level = localPlayer.Level;
            foreach (CustomCombo customCombo in CustomCombosDict[classId])
            {
                if (customCombo.TryInvoke(actionID, lastComboActionID, comboTime, level, out var newActionID))
                {
                    return newActionID;
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

    internal CooldownData GetCooldown(uint actionID)
    {
        byte cooldownGroup = GetCooldownGroup(actionID);
        if (actionManager == IntPtr.Zero)
        {
            CooldownData result = default;
            result.ActionID = actionID;
            return result;
        }
        return Marshal.PtrToStructure<CooldownData>(getActionCooldownSlot(actionManager, cooldownGroup - 1));
    }

    private byte GetCooldownGroup(uint actionID)
    {
        if (cooldownGroupCache.TryGetValue(actionID, out var value))
        {
            return value;
        }
        Action row = Service.DataManager.GetExcelSheet<Action>().GetRow(actionID);
        return cooldownGroupCache[actionID] = row.CooldownGroup;
    }

    internal static void SetEnable(string comboName, bool enable)
    {
        foreach (var combo in CustomCombos)
        {
            if(combo.ComboFancyName == comboName)
            {
                combo.IsEnabled = enable;
                return;
            }
        }
    }

    internal static void SetEnable( bool enable)
    {
        foreach (var combo in CustomCombos)
        {
            combo.IsEnabled = enable;
        }
    }
}
