using ECommons.ExcelServices;
using Lumina.Excel.GeneratedSheets;

namespace RotationSolver.Basic.Data;

/// <summary>
/// The role of jobs.
/// </summary>
public enum JobRole : byte
{
    /// <summary>
    /// 
    /// </summary>
    None = 0,

    /// <summary>
    /// 
    /// </summary>
    Tank = 1,

    /// <summary>
    /// 
    /// </summary>
    Melee = 2,

    ///// <summary>
    ///// 
    ///// </summary>
    //Ranged = 3,

    /// <summary>
    /// 
    /// </summary>
    Healer = 4,

    /// <summary>
    /// 
    /// </summary>
    RangedPhysical = 5,

    /// <summary>
    /// 
    /// </summary>
    RangedMagical = 6,

    ///// <summary>
    ///// 
    ///// </summary>
    //DiscipleOfTheLand = 7,

    ///// <summary>
    ///// 
    ///// </summary>
    //DiscipleOfTheHand = 8,
}

/// <summary>
/// The extension of the job.
/// </summary>
public static class JobRoleExtension
{
    /// <summary>
    /// Get job role from class.
    /// </summary>
    /// <param name="job"></param>
    /// <returns></returns>
    public static JobRole GetJobRole(this ClassJob job)
    {
        var role = (JobRole)job.Role;

        if (role is (JobRole)3 or JobRole.None)
        {
            role = job.ClassJobCategory.Row switch
            {
                30 => JobRole.RangedPhysical,
                31 => JobRole.RangedMagical,
                //32 => JobRole.DiscipleOfTheLand,
                //33 => JobRole.DiscipleOfTheHand,
                _ => JobRole.None,
            };
        }
        return role;
    }

    /// <summary>
    /// Get Jobs from role.
    /// </summary>
    /// <param name="role"></param>
    /// <returns></returns>
    public static Job[] ToJobs(this JobRole role)
    {
        return role switch
        {
            JobRole.Tank => [Job.WAR, Job.PLD, Job.DRK, Job.GNB],
            JobRole.Healer => [Job.WHM, Job.SCH, Job.AST, Job.SGE],
            JobRole.Melee => [Job.MNK, Job.DRG, Job.NIN, Job.SAM, Job.RPR],
            JobRole.RangedPhysical => [Job.BRD, Job.MCH, Job.DNC],
            JobRole.RangedMagical => [Job.BLM, Job.SMN, Job.RDM, Job.BLU],
            _ => [],
        };
    }
}
