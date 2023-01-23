using Lumina.Excel.GeneratedSheets;

namespace RotationSolver.Data;

public enum JobRole : byte
{
    None = 0,
    Tank = 1,
    Melee = 2,
    Ranged = 3,
    Healer = 4,
    RangedPhysical = 5,
    RangedMagicial = 6,
    DiscipleoftheLand = 7,
    DiscipleoftheHand = 8,
}

internal static class JobRoleExtension
{
    public static JobRole GetJobRole(this ClassJob job)
    {
        var role = (JobRole)job.Role;

        if (role is JobRole.Ranged or JobRole.None)
        {
            role = job.ClassJobCategory.Row switch
            {
                30 => JobRole.RangedPhysical,
                31 => JobRole.RangedMagicial,
                32 => JobRole.DiscipleoftheLand,
                33 => JobRole.DiscipleoftheHand,
                _ => JobRole.None,
            };
        }
        return role;
    }
}
