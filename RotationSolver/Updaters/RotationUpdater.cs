using Dalamud.Logging;
using RotationSolver.Localization;

namespace RotationSolver.Updaters;

internal static class RotationUpdater
{
    public record CustomRotationGroup(ClassJobID jobId, ClassJobID[] classJobIds, ICustomRotation[] rotations);

    internal static SortedList<JobRole, CustomRotationGroup[]> CustomRotationsDict { get; private set; } = new SortedList<JobRole, CustomRotationGroup[]>();

    internal static SortedList<string, string> AuthorHashes { get; private set; } = new SortedList<string, string>();
    static CustomRotationGroup[] _customRotations { get; set; } = new CustomRotationGroup[0];

    static readonly string[] _locs = new string[] { "RotationSolver.dll", "RotationSolver.Basic.dll" };


    public static void GetAllCustomRotations()
    {
        var directories =  Service.Config.OtherLibs
            .Select(s => s.Trim()).Append(Path.GetDirectoryName(Assembly.GetAssembly(typeof(ICustomRotation)).Location));

        var assemblies = from dir in directories
                         where Directory.Exists(dir)
                         from l in Directory.GetFiles(dir, "*.dll")
                         where !_locs.Any(l.Contains)
                         select RotationLoadContext.LoadFrom(l);

        PluginLog.Log("Try to load rotations from these assemblies.", assemblies.Select(a => a.FullName));

        AuthorHashes = new SortedList<string, string>(
            (from a in assemblies
             select (a, a.GetCustomAttribute<AuthorHashAttribute>()) into author
             where author.Item2 != null
             group author by author.Item2 into gr
             select (gr.Key.Hash, string.Join(", ", gr.Select(i => i.a.GetAuthor() + " - " + i.a.GetName().Name))))
             .ToDictionary(i => i.Hash, i => i.Item2));

        _customRotations = (
            from a in assemblies
            from t in a.GetTypes()
            where t.GetInterfaces().Contains(typeof(ICustomRotation))
                 && !t.IsAbstract && !t.IsInterface
            select GetRotation(t) into rotation
            where rotation != null
            group rotation by rotation.JobIDs[0] into rotationGrp
            select new CustomRotationGroup(rotationGrp.Key, rotationGrp.First().JobIDs, CreateRotationSet(rotationGrp.ToArray()))).ToArray();

        CustomRotationsDict = new SortedList<JobRole, CustomRotationGroup[]>
            (_customRotations.GroupBy(g => g.rotations[0].Job.GetJobRole())
            .ToDictionary(set => set.Key, set => set.OrderBy(i => i.jobId).ToArray()));
    }

    private static ICustomRotation GetRotation(Type t)
    {
        try
        {
            return (ICustomRotation)Activator.CreateInstance(t);
        }
        catch 
        {
            PluginLog.LogError($"Failed to load the rotation: {t.Name}");
            return null; 
        }
    }

    private static ICustomRotation[] CreateRotationSet(ICustomRotation[] combos)
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

    public static ICustomRotation RightNowRotation { get; private set; }

    public static IEnumerable<IGrouping<string, IAction>> AllGroupedActions
        => RightNowRotation?.AllActions.GroupBy(a =>
            {
                if (a is IBaseAction act)
                {
                    string result;

                    if (act.IsRealGCD)
                    {
                        result = "GCD";
                    }
                    else
                    {
                        result = LocalizationManager.RightLang.Timeline_Ability;
                    }

                    if (act.IsFriendly)
                    {
                        result += "-" + LocalizationManager.RightLang.Action_Friendly;
                        if (act.IsEot)
                        {
                            result += "-Hot";
                        }
                    }
                    else
                    {
                        result += "-" + LocalizationManager.RightLang.Action_Attack;

                        if (act.IsEot)
                        {
                            result += "-Dot";
                        }
                    }
                    return result;
                }
                else if (a is IBaseItem)
                {
                    return "Item";
                }
                return string.Empty;

            }).OrderBy(g => g.Key);

    public static IBaseAction[] RightRotationBaseActions { get; private set; } = new IBaseAction[0];

    public static void UpdateRotation()
    {
        var nowJob = (ClassJobID)Service.Player.ClassJob.Id;

        foreach (var group in _customRotations)
        {
            if (!group.classJobIds.Contains(nowJob)) continue;

            RightNowRotation = GetChooseRotation(group);
            RightRotationBaseActions = RightNowRotation.AllBaseActions;
            return;
        }
        RightNowRotation = null;
        RightRotationBaseActions = new IBaseAction[0];
    }

    internal static ICustomRotation GetChooseRotation(CustomRotationGroup group)
    {
        Service.Config.RotationChoices.TryGetValue((uint)group.jobId, out var name);
       
        var rotation = group.rotations.FirstOrDefault(r => r.GetType().FullName == name);
        rotation ??= group.rotations.FirstOrDefault(RotationHelper.IsDefault);
        rotation ??= group.rotations.FirstOrDefault(r => r.IsAllowed(out _));
        rotation ??= group.rotations.FirstOrDefault();
        return rotation;
    }
}
