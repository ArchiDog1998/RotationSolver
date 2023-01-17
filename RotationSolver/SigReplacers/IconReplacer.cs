using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Hooking;
using FFXIVClientStructs.FFXIV.Client.Game;
using Lumina.Excel.GeneratedSheets;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using RotationSolver.Combos.Script.Conditions;
using RotationSolver.Actions;
using RotationSolver.Data;
using RotationSolver.Combos.Script;
using RotationSolver.Combos.Script.Actions;
using RotationSolver.Combos.CustomCombo;
using RotationSolver.Updaters;
using System.Runtime.CompilerServices;

namespace RotationSolver.SigReplacers;

internal sealed class IconReplacer : IDisposable
{
    public record CustomRotationGroup(ClassJobID jobId, ClassJobID[] classJobIds, ICustomRotation[] combos);

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

    public static ICustomRotation RightNowCombo
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
            foreach (var combo in group.combos)
            {
                if (combo.Author == choice)
                {
                    return combo;
                }
            }
        }

        foreach (var item in group.combos)
        {
            if (item.GetType().Name.Contains("Default"))
            {
                return item;
            }
        }
        return group.combos[0];
    }

    internal static IAction[] RightComboBaseActions
    {
        get
        {
            var combo = RightNowCombo;
            if (combo == null) return new IAction[0];
            return combo.AllActions;
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
                SetStaticValues();
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
                SetStaticValues();
            }
            return _customRotations;
        }
    }

    internal static Dictionary<ClassJobID, Type> _customScriptCombos;

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


    private static void GetAllCombos()
    {
        _combos = (from t in Assembly.GetAssembly(typeof(ICustomRotation)).GetTypes()
                   where t.GetInterfaces().Contains(typeof(ICustomRotation)) &&
                        !t.GetInterfaces().Contains(typeof(IScriptCombo)) && !t.IsAbstract && !t.IsInterface
                   select (ICustomRotation)Activator.CreateInstance(t)).ToList();
    }

    private static void MaintenceCombos()
    {
        _customRotations = (from combo in _combos
                         group combo by combo.JobIDs[0] into comboGrp
                         select new CustomRotationGroup(comboGrp.Key, comboGrp.First().JobIDs, SetCombos(comboGrp.ToArray())))
                        .ToArray();

        _customCombosDict = new SortedList<JobRole, CustomRotationGroup[]>
            (_customRotations.GroupBy(g => g.combos[0].Job.GetJobRole()).ToDictionary(set => set.Key, set => set.OrderBy(i => i.jobId).ToArray()));
    }

    internal static string ScriptRotationFolder => typeof(IconReplacer).Assembly.Location + "/ScriptRotation";

    public static IScriptCombo AddScripCombo(ClassJobID id, bool update = true)
    {
        if (!Directory.Exists(ScriptRotationFolder))
        {
            return null;
        }

        if (_customScriptCombos.TryGetValue(id, out var value))
        {
            var add = (IScriptCombo)Activator.CreateInstance(value);
            add.Set.JobID = id;
            _combos.Add(add);

            if (update) MaintenceCombos();
            return add;
        }
        return null;
    }

    public static void LoadFromFolder()
    {
        if (Directory.Exists(ScriptRotationFolder))
        {
            foreach (var path in Directory.EnumerateFiles(ScriptRotationFolder, "*.json"))
            {
                try
                {
                    var set = JsonConvert.DeserializeObject<ComboSet>(File.ReadAllText(path), new IConditionConverter());

                    if (set == null) continue;

                    var combo = AddScripCombo(set.JobID, false);

                    combo.Set = set;

                    _combos.Add(combo);
                }
                catch { }
            }
        }

        MaintenceCombos();
    }

    private static void SetStaticValues()
    {
        _customScriptCombos = (from t in Assembly.GetAssembly(typeof(IScriptCombo)).GetTypes()
                               where t.GetInterfaces().Contains(typeof(IScriptCombo)) && !t.IsAbstract && !t.IsInterface
                               select t).ToDictionary(t => ((IScriptCombo)Activator.CreateInstance(t)).JobIDs[0]);

        GetAllCombos();
        LoadFromFolder();
    }

    private static ICustomRotation[] SetCombos(ICustomRotation[] combos)
    {
        var result = new List<ICustomRotation>(combos.Length);

        foreach (var combo in combos)
        {
            if (!result.Any(c => c.Author == combo.Author))
            {
                result.Add(combo);
            }
        }
        return result.ToArray();
    }

    public void Dispose()
    {
        foreach (var item in _combos)
        {
            if (item is IScriptCombo com)
            {
                File.WriteAllText(com.Set.GetFolder(), JsonConvert.SerializeObject(com.Set, Formatting.Indented));
            }
        }
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
