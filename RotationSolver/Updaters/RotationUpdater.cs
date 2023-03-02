using Lumina.Data.Parsing;
using RotationSolver.Actions;
using RotationSolver.Attributes;
using RotationSolver.Data;
using RotationSolver.Rotations.CustomRotation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RotationSolver.Updaters;

internal static class RotationUpdater
{
    public record CustomRotationGroup(ClassJobID jobId, ClassJobID[] classJobIds, ICustomRotation[] rotations);

    #region All Rotations
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
        _customRotations = (from t in Assembly.GetAssembly(typeof(ICustomRotation)).GetTypes()
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
    #endregion

    public static ICustomRotation RightNowRotation { get; private set; }

    public static IBaseAction[] RightRotationBaseActions { get; private set; } = new IBaseAction[0];

    static ClassJobID _job;
    static string _rotationName;
    public static void UpdateRotation()
    {
        var nowJob = (ClassJobID)Service.ClientState.LocalPlayer.ClassJob.Id;
        Service.Configuration.RotationChoices.TryGetValue((uint)nowJob, out var newName);

        if (_job == nowJob && _rotationName == newName) return;

        _job = nowJob;
        _rotationName = newName;

        foreach (var group in CustomRotations)
        {
            if (!group.classJobIds.Contains(_job)) continue;

            RightNowRotation = GetChoosedRotation(group, _rotationName);
            RightRotationBaseActions = RightNowRotation.AllBaseActions;
            break;
        }
    }

    internal static ICustomRotation GetChoosedRotation(CustomRotationGroup group, string name)
    {
        var rotation = group.rotations.FirstOrDefault(r => r.RotationName == name);
        rotation ??= group.rotations.FirstOrDefault(r => r.GetType().GetCustomAttribute<DefaultRotationAttribute>() != null);
        rotation ??= group.rotations.FirstOrDefault(r => r.GetType().Name.Contains("Default"));
        rotation ??= group.rotations.FirstOrDefault();
        return rotation;
    }
}
