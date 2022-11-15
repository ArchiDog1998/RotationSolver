using Lumina.Excel.GeneratedSheets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XIVAutoAttack.Data
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

            if(role is JobRole.Ranged or JobRole.None)
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
            JobRole.None => "采集制作",
            JobRole.Tank => "防护",
            JobRole.Melee => "近战",
            JobRole.Ranged => "远程",
            JobRole.Healer => "治疗",
            JobRole.RangedPhysical => "远敏",
            JobRole.RangedMagicial => "魔法",
            JobRole.DiscipleoftheLand => "大地使者",
            JobRole.DiscipleoftheHand => "能工巧匠",
            _ => "Unknown",
        };
    }
}
