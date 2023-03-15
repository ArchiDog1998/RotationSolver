using RotationSolver.Basic;
using RotationSolver.Basic.Actions;
using RotationSolver.Basic.Data;
using RotationSolver.Basic.Rotations;
using System.Reflection;
using System.Runtime.Loader;

namespace RotationSolver.Updaters;

internal static class RotationUpdater
{
    public record CustomRotationGroup(ClassJobID jobId, ClassJobID[] classJobIds, ICustomRotation[] rotations);

    internal static SortedList<JobRole, CustomRotationGroup[]> CustomRotationsDict { get; set; } = new SortedList<JobRole, CustomRotationGroup[]>();
    private static CustomRotationGroup[] CustomRotations { get; set; } = new CustomRotationGroup[0];

    static readonly  string[] locs = new string[] { "RotationSolver.dll", "RotationSolver.Basic.dll" };

    public static void GetAllCustomRotations()
    {
        CustomRotations = (
            from l in Directory.GetFiles(Path.GetDirectoryName(Assembly.GetAssembly(typeof(ICustomRotation)).Location), "*.dll")
            where !locs.Any(l.Contains)
            select RotationLoadContext.LoadFrom(l) into a
            from t in a.GetTypes()
            where t.GetInterfaces().Contains(typeof(ICustomRotation))
                 && !t.IsAbstract && !t.IsInterface
            select GetRotation(t) into rotation
            where rotation != null
            group rotation by rotation.JobIDs[0] into rotationGrp
            select new CustomRotationGroup(rotationGrp.Key, rotationGrp.First().JobIDs, CreateRotationSet(rotationGrp.ToArray()))).ToArray();

        CustomRotationsDict = new SortedList<JobRole, CustomRotationGroup[]>
            (CustomRotations.GroupBy(g => g.rotations[0].Job.GetJobRole())
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
            Dalamud.Logging.PluginLog.LogError($"Failed to load the rotation: {t.Name}");
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

    public static IBaseAction[] RightRotationBaseActions { get; private set; } = new IBaseAction[0];

    static ClassJobID _job;
    static string _rotationName;
    public static void UpdateRotation()
    {
        var nowJob = (ClassJobID)Service.Player.ClassJob.Id;
        Service.Config.RotationChoices.TryGetValue((uint)nowJob, out var newName);

        if (_job == nowJob && _rotationName == newName) return;

        _job = nowJob;
        _rotationName = newName;

        foreach (var group in CustomRotations)
        {
            if (!group.classJobIds.Contains(_job)) continue;

            RightNowRotation = GetChooseRotation(group, _rotationName);
            RightRotationBaseActions = RightNowRotation.AllBaseActions;
            break;
        }
    }

    internal static ICustomRotation GetChooseRotation(CustomRotationGroup group, string name)
    {
        var rotation = group.rotations.FirstOrDefault(r => r.RotationName == name);
        rotation ??= group.rotations.FirstOrDefault(RotationHelper.IsDefault);
        rotation ??= group.rotations.FirstOrDefault(r => r.IsAllowed(out _));
        rotation ??= group.rotations.FirstOrDefault();
        return rotation;
    }
}
