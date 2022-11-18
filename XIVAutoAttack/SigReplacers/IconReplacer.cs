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
using XIVAutoAttack.Actions.BaseAction;
using XIVAutoAttack.Combos.CustomCombo;
using XIVAutoAttack.Combos.Script;
using XIVAutoAttack.Combos.Script.Actions;
using XIVAutoAttack.Data;
using XIVAutoAttack.Updaters;

namespace XIVAutoAttack.SigReplacers;

internal sealed class IconReplacer : IDisposable
{
    public record CustomComboGroup(ClassJobID jobId, ClassJobID[] classJobIds, ICustomCombo[] combos);

    private delegate ulong IsIconReplaceableDelegate(uint actionID);

    private delegate uint GetIconDelegate(IntPtr actionManager, uint actionID);

    private delegate IntPtr GetActionCooldownSlotDelegate(IntPtr actionManager, int cooldownGroup);

    public static byte RightNowTargetToHostileType
    {
        get
        {
            if (Service.ClientState.LocalPlayer == null) return 0;
            var id = Service.ClientState.LocalPlayer.ClassJob.Id;
            if (Service.Configuration.TargetToHostileTypes.TryGetValue(id, out var type))
            {
                return type;
            }

            var role = Service.DataManager.GetExcelSheet<ClassJob>().GetRow(id).GetJobRole();
            return role == JobRole.Tank ? (byte)1 : (byte)2;
        }
        set
        {
            if (Service.ClientState.LocalPlayer == null) return;
            var id = Service.ClientState.LocalPlayer.ClassJob.Id;
            Service.Configuration.TargetToHostileTypes[id] = value;
        }
    }

    public static ICustomCombo RightNowCombo
    {
        get
        {
            if (Service.ClientState.LocalPlayer == null) return null;

            foreach (var combos in CustomCombos)
            {
                if (!combos.classJobIds.Contains((ClassJobID)Service.ClientState.LocalPlayer.ClassJob.Id)) continue;
                return GetChooseCombo(combos);
            }

            return null;
        }
    }

    internal static ICustomCombo GetChooseCombo(CustomComboGroup group)
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

    internal static BaseAction[] RightComboBaseActions
    {
        get
        {
            var combo = RightNowCombo;
            if (combo == null) return new BaseAction[0];
            return combo.AllActions;
        }
    }


    private static List<ICustomCombo> _combos = new List<ICustomCombo>();
    private static SortedList<JobRole, CustomComboGroup[]> _customCombosDict;
    internal static SortedList<JobRole, CustomComboGroup[]> CustomCombosDict
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
    private static CustomComboGroup[] _customCombos;
    private static CustomComboGroup[] CustomCombos
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

    internal static Dictionary<ClassJobID, Type> _customScriptCombos;

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


    private static void GetAllCombos()
    {
        _combos = (from t in Assembly.GetAssembly(typeof(ICustomCombo)).GetTypes()
                   where t.GetInterfaces().Contains(typeof(ICustomCombo)) &&
                        !t.GetInterfaces().Contains(typeof(IScriptCombo)) && !t.IsAbstract && !t.IsInterface
                   select (ICustomCombo)Activator.CreateInstance(t)).ToList();
    }

    private static void MaintenceCombos()
    {
        _customCombos = (from combo in _combos
                         group combo by combo.JobIDs[0] into comboGrp
                         select new CustomComboGroup(comboGrp.Key, comboGrp.First().JobIDs, SetCombos(comboGrp.ToArray())))
                        .ToArray();

        _customCombosDict = new SortedList<JobRole, CustomComboGroup[]>
            (_customCombos.GroupBy(g => g.combos[0].Job.GetJobRole()).ToDictionary(set => set.Key, set => set.OrderBy(i => i.jobId).ToArray()));
    }

    public static IScriptCombo AddScripCombo(ClassJobID id, bool update = true)
    {
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
        if (Directory.Exists(Service.Configuration.ScriptComboFolder))
        {
            foreach (var path in Directory.EnumerateFiles(Service.Configuration.ScriptComboFolder, "*.json"))
            {
                var set = JsonConvert.DeserializeObject<ComboSet>(File.ReadAllText(path));

                if (set == null) continue;

                var combo = AddScripCombo(set.JobID, false);

                combo.Set = set;
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

    private static ICustomCombo[] SetCombos(ICustomCombo[] combos)
    {
        var result = new List<ICustomCombo>(combos.Length);

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

    internal static bool AutoAttackConfig(string str, string str1)
    {
        switch (str)
        {
            case "setall":
                {
                    foreach (var item in CustomCombos)
                    {
                        foreach (var combo in item.combos)
                        {
                            combo.IsEnabled = true;
                        }
                    }
                    Service.ChatGui.Print("All SET");
                    Service.Configuration.Save();
                    break;
                }
            case "unsetall":
                {
                    foreach (var item in CustomCombos)
                    {
                        foreach (var combo in item.combos)
                        {
                            combo.IsEnabled = false;
                        }
                    }
                    Service.ChatGui.Print("All UNSET");
                    Service.Configuration.Save();
                    break;
                }
            default:
                return true;
        }
        Service.Configuration.Save();
        return false;
    }
}
