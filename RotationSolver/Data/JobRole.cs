using Lumina.Excel.GeneratedSheets;
using XIVAutoAction.Localization;

namespace XIVAutoAction.Data
{
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

        public static string ToName(this JobRole role) => role switch
        {
            JobRole.None => LocalizationManager.RightLang.JobRole_None,
            JobRole.Tank => LocalizationManager.RightLang.JobRole_Tank,
            JobRole.Melee => LocalizationManager.RightLang.JobRole_Melee,
            JobRole.Ranged => LocalizationManager.RightLang.JobRole_Ranged,
            JobRole.Healer => LocalizationManager.RightLang.JobRole_Healer,
            JobRole.RangedPhysical => LocalizationManager.RightLang.JobRole_RangedPhysical,
            JobRole.RangedMagicial => LocalizationManager.RightLang.JobRole_RangedMagicial,
            JobRole.DiscipleoftheLand => LocalizationManager.RightLang.JobRole_DiscipleoftheLand,
            JobRole.DiscipleoftheHand => LocalizationManager.RightLang.JobRole_DiscipleoftheHand,
            _ => string.Empty,
        };
    }
}
