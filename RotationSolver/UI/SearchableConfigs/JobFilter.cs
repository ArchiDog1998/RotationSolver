using ECommons.ExcelServices;

namespace RotationSolver.UI.SearchableConfigs;

internal readonly struct JobFilter
{
    public JobFilter(JobFilterType type)
    {
        switch (type)
        {
            case JobFilterType.NoJob:
                JobRoles = [];
                break;

            case JobFilterType.NoHealer:
                JobRoles =
                [
                    JobRole.Tank,
                    JobRole.Melee,
                    JobRole.RangedMagical,
                    JobRole.RangedPhysical,
                ];
                break;

            case JobFilterType.Healer:
                JobRoles =
                [
                    JobRole.Healer,
                ];
                break;

            case JobFilterType.Raise:
                JobRoles =
                [
                    JobRole.Healer,
                ];
                Jobs =
                [
                    Job.RDM,
                    Job.SMN,
                ];
                break;

            case JobFilterType.Interrupt:
                JobRoles =
                [
                    JobRole.Tank,
                    JobRole.Melee,
                    JobRole.RangedPhysical,
                ];
                break;

            case JobFilterType.Dispel:
                JobRoles =
                [
                    JobRole.Healer,
                ];
                Jobs =
                [
                    Job.BRD,
                ];
                break;

            case JobFilterType.Tank:
                JobRoles =
                [
                    JobRole.Tank,
                ];
                break;

            case JobFilterType.Melee:
                JobRoles =
                [
                    JobRole.Melee,
                ];
                break;
        }
    }

    /// <summary>
    /// Only these job roles can get this setting.
    /// </summary>
    public JobRole[]? JobRoles { get; init; }

    /// <summary>
    /// Or these jobs.
    /// </summary>
    public Job[]? Jobs { get; init; }

    public bool CanDraw
    {
        get
        {
            var canDraw = true;

            if (JobRoles != null)
            {
                var role = DataCenter.RightNowRotation?.Role;
                if (role.HasValue)
                {
                    canDraw = JobRoles.Contains(role.Value);
                }
            }

            if (Jobs != null)
            {
                canDraw |= Jobs.Contains(DataCenter.Job);
            }
            return canDraw;
        }
    }

    public Job[] AllJobs => (JobRoles ?? []).SelectMany(JobRoleExtension.ToJobs).Union(Jobs ?? []).ToArray();
}