using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Hooking;
using Dalamud.Logging;
using FFXIVClientStructs.FFXIV.Client.Game;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using XIVAutoAttack.Actions;
using XIVAutoAttack.Actions.BaseAction;
using XIVAutoAttack.Combos.CustomCombo;
using XIVAutoAttack.Data;
using XIVAutoAttack.Helpers;
using XIVAutoAttack.Updaters;

namespace XIVAutoAttack.SigReplacers;

internal sealed class IconReplacer : IDisposable
{
    private delegate ulong IsIconReplaceableDelegate(uint actionID);

    private delegate uint GetIconDelegate(IntPtr actionManager, uint actionID);

    private delegate IntPtr GetActionCooldownSlotDelegate(IntPtr actionManager, int cooldownGroup);

    private static SortedList<Role, ICustomCombo[]> _customCombosDict;
    internal static SortedList<Role, ICustomCombo[]> CustomCombosDict
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
    private static ICustomCombo[] _customCombos;
    internal static ICustomCombo[] CustomCombos
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
            getIconHook = Hook<GetIconDelegate>.FromAddress((IntPtr)ActionManager.fpGetAdjustedActionId, GetIconDetour);
        }
        isIconReplaceableHook = Hook<IsIconReplaceableDelegate>.FromAddress(Service.Address.IsActionIdReplaceable, IsIconReplaceableDetour);

        getIconHook.Enable();
        isIconReplaceableHook.Enable();
    }



    private static void SetStaticValues()
    {
        _customCombos = (from t in Assembly.GetAssembly(typeof(ICustomCombo)).GetTypes()
                         where t.GetInterfaces().Contains(typeof(ICustomCombo)) && !t.IsAbstract && !t.IsInterface
                         select (ICustomCombo)Activator.CreateInstance(t)).ToArray();

        _customCombosDict = new SortedList<Role, ICustomCombo[]>
            (_customCombos.GroupBy(g => g.Role).ToDictionary(set => set.Key, set => set.OrderBy(i => i.JobID).ToArray()));
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

    internal static BaseAction KeyActionID => CustomCombo<Enum>.GeneralActions.Repose;

    private uint RemapActionID(uint actionID)
    {
        PlayerCharacter localPlayer = Service.ClientState.LocalPlayer;

        if (localPlayer == null || actionID != KeyActionID.ID || Service.Configuration.NeverReplaceIcon)
            return OriginalHook(actionID);

        return ActionUpdater.NextAction?.AdjustedID ?? OriginalHook(actionID);

    }

    private ulong IsIconReplaceableDetour(uint actionID)
    {
        return 1uL;
    }
}
