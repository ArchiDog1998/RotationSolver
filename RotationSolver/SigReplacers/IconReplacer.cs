using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Hooking;
using FFXIVClientStructs.FFXIV.Client.Game;
using Lumina.Excel.GeneratedSheets;
using Newtonsoft.Json;
using RotationSolver.Actions;
using RotationSolver.Data;
using RotationSolver.Rotations.CustomRotation;
using RotationSolver.Rotations.Script;
using RotationSolver.Rotations.Script.Conditions;
using RotationSolver.Updaters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace RotationSolver.SigReplacers;

internal sealed class IconReplacer : IDisposable
{
    internal static string TimelineFolder => typeof(IconReplacer).Assembly.Location + "/ScriptRotation";


    public record CustomRotationGroup(ClassJobID jobId, ClassJobID[] classJobIds, ICustomRotation[] rotations);

    private delegate ulong IsIconReplaceableDelegate(uint actionID);

    private delegate uint GetIconDelegate(IntPtr actionManager, uint actionID);

    private delegate IntPtr GetActionCooldownSlotDelegate(IntPtr actionManager, int cooldownGroup);

    public static TargetHostileType RightNowTargetToHostileType
    {
        get
        {
            if (Service.ClientState.LocalPlayer == null) return 0;
            var id = Service.ClientState.LocalPlayer.ClassJob.Id;
            return GetTargetHostileType(Service.DataManager.GetExcelSheet<ClassJob>().GetRow(id));
        }
    }

    public static TargetHostileType GetTargetHostileType(ClassJob classJob)
    {
        if (Service.Configuration.TargetToHostileTypes.TryGetValue(classJob.RowId, out var type))
        {
            return (TargetHostileType)type;
        }

        return classJob.GetJobRole() == JobRole.Tank ? TargetHostileType.AllTargetsCanAttack : TargetHostileType.TargetsHaveTarget;
    }

    public static ICustomRotation RightNowRotation
    {
        get
        {
            if (Service.ClientState.LocalPlayer == null) return null;

            foreach (var combos in CustomRotations)
            {
                if (!combos.classJobIds.Contains((ClassJobID)Service.ClientState.LocalPlayer.ClassJob.Id)) continue;
                return GetChoosedRotation(combos);
            }

            return null;
        }
    }

    internal static ICustomRotation GetChoosedRotation(CustomRotationGroup group)
    {
        var id = group.jobId;
        if (Service.Configuration.ComboChoices.TryGetValue((uint)id, out var choice))
        {
            foreach (var combo in group.rotations)
            {
                if (combo.RotationName == choice)
                {
                    return combo;
                }
            }
        }

        foreach (var item in group.rotations)
        {
            if (item.GetType().Name.Contains("Default"))
            {
                return item;
            }
        }
        return group.rotations[0];
    }

    internal static IBaseAction[] RightRotationBaseActions
    {
        get
        {
            var rotation = RightNowRotation;
            if (rotation == null) return new IBaseAction[0];
            return rotation.AllActions;
        }
    }


    private static List<ICustomRotation> _combos = new List<ICustomRotation>();
    private static SortedList<JobRole, CustomRotationGroup[]> _customCombosDict;
    internal static SortedList<JobRole, CustomRotationGroup[]> CustomRotationsDict
    {
        get
        {
            if (_customCombosDict == null)
            {
                GetAllCustomCombos();
            }
            return _customCombosDict;
        }
    }
    private static CustomRotationGroup[] _customRotations;
    private static CustomRotationGroup[] CustomRotations
    {
        get
        {
            if (_customRotations == null)
            {
                GetAllCustomCombos();
            }
            return _customRotations;
        }
    }

    private readonly Hook<IsIconReplaceableDelegate> isIconReplaceableHook;

    private readonly Hook<GetIconDelegate> getIconHook;

    private IntPtr actionManager = IntPtr.Zero;


    public IconReplacer()
    {
        unsafe
        {
            getIconHook = Hook<GetIconDelegate>.FromAddress((IntPtr)ActionManager.MemberFunctionPointers.GetAdjustedActionId, GetIconDetour);
        }
        isIconReplaceableHook = Hook<IsIconReplaceableDelegate>.FromAddress(Service.Address.IsActionIdReplaceable, IsIconReplaceableDetour);

        getIconHook.Enable();
        isIconReplaceableHook.Enable();
    }

    private static void GetAllCustomCombos()
    {
        _combos = (from t in Assembly.GetAssembly(typeof(ICustomRotation)).GetTypes()
                   where t.GetInterfaces().Contains(typeof(ICustomRotation))
                        && !t.IsAbstract && !t.IsInterface
                   select (ICustomRotation)Activator.CreateInstance(t)).ToList();


        _customRotations = (from combo in _combos
                            group combo by combo.JobIDs[0] into comboGrp
                            select new CustomRotationGroup(comboGrp.Key, comboGrp.First().JobIDs, SetCombos(comboGrp.ToArray())))
                        .ToArray();

        _customCombosDict = new SortedList<JobRole, CustomRotationGroup[]>
            (_customRotations.GroupBy(g => g.rotations[0].Job.GetJobRole()).ToDictionary(set => set.Key, set => set.OrderBy(i => i.jobId).ToArray()));
    }

    private static ICustomRotation[] SetCombos(ICustomRotation[] combos)
    {
        var result = new List<ICustomRotation>(combos.Length);

        foreach (var combo in combos)
        {
            if (!result.Any(c => c.RotationName == combo.RotationName))
            {
                result.Add(combo);
            }
        }
        return result.ToArray();
    }

    public void Dispose()
    {
        getIconHook.Dispose();
        isIconReplaceableHook.Dispose();
    }

    internal ActionID OriginalHook(ActionID actionID)
    {
        return (ActionID)getIconHook.Original.Invoke(actionManager, (uint)actionID);
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

    internal static ActionID KeyActionID => ActionID.Repose;

    private uint RemapActionID(uint actionID)
    {
        PlayerCharacter localPlayer = Service.ClientState.LocalPlayer;

        if (localPlayer == null || actionID != (uint)KeyActionID || Service.Configuration.NeverReplaceIcon)
            return OriginalHook(actionID);

        return ActionUpdater.NextAction?.AdjustedID ?? OriginalHook(actionID);

    }

    private ulong IsIconReplaceableDetour(uint actionID)
    {
        return 1uL;
    }
}
