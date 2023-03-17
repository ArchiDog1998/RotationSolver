using Lumina.Excel.GeneratedSheets;

namespace RotationSolver.Basic.Data;

public enum JobRole : byte
{
    None = 0,
    Tank = 1,
    Melee = 2,
    Ranged = 3,
    Healer = 4,
    RangedPhysical = 5,
    RangedMagical = 6,
    DiscipleOfTheLand = 7,
    DiscipleOfTheHand = 8,
}

public static class JobRoleExtension
{
    public static JobRole GetJobRole(this ClassJob job)
    {
        var role = (JobRole)job.Role;

        if (role is JobRole.Ranged or JobRole.None)
        {
            role = job.ClassJobCategory.Row switch
            {
                30 => JobRole.RangedPhysical,
                31 => JobRole.RangedMagical,
                32 => JobRole.DiscipleOfTheLand,
                33 => JobRole.DiscipleOfTheHand,
                _ => JobRole.None,
            };
        }
        return role;
    }
}
