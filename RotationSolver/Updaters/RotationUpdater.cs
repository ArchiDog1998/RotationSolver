using RotationSolver.Basic;
using RotationSolver.Basic.Actions;
using RotationSolver.Basic.Data;
using RotationSolver.Basic.Rotations;
using System.Reflection;

namespace RotationSolver.Updaters;

internal static class RotationUpdater
{
    public record CustomRotationGroup(ClassJobID jobId, ClassJobID[] classJobIds, ICustomRotation[] rotations);

    private static SortedList<JobRole, CustomRotationGroup[]> _customRotationsDict;
    internal static SortedList<JobRole, CustomRotationGroup[]> CustomRotationsDict
    {
        get
        {
            if (_customRotationsDict == null)
            {
                GetAllCustomRotations();
            }
            return _customRotationsDict;
        }
    }
    private static CustomRotationGroup[] _customRotations;
    private static CustomRotationGroup[] CustomRotations
    {
        get
        {
            if (_customRotations == null)
            {
                GetAllCustomRotations();
            }
            return _customRotations;
        }
    }

    private static void GetAllCustomRotations()
    {
        //var thisPath = Assembly.GetAssembly(typeof(ICustomRotation)).Location;

        //var types = Directory.GetFiles(Path.GetDirectoryName(Assembly.GetAssembly(typeof(ICustomRotation)).Location), "*.dll")
        //    .Where(l => l != thisPath)
        //    .Select(Assembly.LoadFrom)
        //    .SelectMany(a => a.GetTypes());

        foreach (var t in AppDomain.CurrentDomain.GetAssemblies().Where(a => a.Location.Contains("RotationSolver")))
        {
            Service.ChatGui.Print(t.FullName);
        }

        var assemblies = new Assembly[] { typeof(RotationUpdater).Assembly };

        _customRotations = (from a in assemblies
                            from t in a.GetTypes()
                            where t.GetInterfaces().Contains(typeof(ICustomRotation))
                                 && !t.IsAbstract && !t.IsInterface
                            select (ICustomRotation)Activator.CreateInstance(t) into rotation
                            group rotation by rotation.JobIDs[0] into rotationGrp
                            select new CustomRotationGroup(rotationGrp.Key, rotationGrp.First().JobIDs, CreateRotationSet(rotationGrp.ToArray()))).ToArray();

        _customRotationsDict = new SortedList<JobRole, CustomRotationGroup[]>
            (_customRotations.GroupBy(g => g.rotations[0].Job.GetJobRole())
            .ToDictionary(set => set.Key, set => set.OrderBy(i => i.jobId).ToArray()));
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
