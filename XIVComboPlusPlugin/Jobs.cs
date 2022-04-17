using Dalamud.Game.ClientState.JobGauge.Types;
using Dalamud.Game.ClientState.Objects.SubKinds;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XIVComboPlus
{
    public enum JobType : byte
    {
        None,
        Tank,
        Healer,
        Melee,
        PhysicalRanged,
        MagicalRanged,
    }
    public class ClassJob
    {

        public byte Index { get; }
        public string Name { get; }
        public JobType Type { get; }
        public Type JobGauge { get; }

        public ClassJob(byte index, string name ,JobType type, Type gauge)
        {
            this.Index = index;
            this.Name = name;
            this.Type = type;
            this.JobGauge = gauge;
        }

        public static ClassJob[] AllJobs =  new ClassJob[]
        {
            new ClassJob(0, "冒险者", JobType.None, null),
            new ClassJob(1, "剑术师", JobType.Tank, null),
            new ClassJob(2, "格斗家", JobType.Melee, null),
            new ClassJob(3, "斧术师", JobType.Tank, null),
            new ClassJob(4, "枪术师", JobType.Melee, null),
            new ClassJob(5, "弓箭手", JobType.PhysicalRanged, null),
            new ClassJob(6, "幻术师", JobType.Healer, null),
            new ClassJob(7, "咒术师", JobType.MagicalRanged, null),
            new ClassJob(19, "骑士", JobType.Tank, typeof(PLDGauge)),
            new ClassJob(20, "武僧", JobType.Melee, typeof(MNKGauge)),
            new ClassJob(21, "战士", JobType.Tank, typeof(WARGauge)),
            new ClassJob(22, "龙骑士", JobType.Melee, typeof(DRGGauge)),
            new ClassJob(23, "游吟诗人", JobType.PhysicalRanged, typeof(BRDGauge)),
            new ClassJob(24, "白魔法师", JobType.Healer, typeof(WHMGauge)),
            new ClassJob(25, "黑魔法师", JobType.MagicalRanged, typeof(BLMGauge)),
            new ClassJob(26, "秘术师", JobType.MagicalRanged, null),
            new ClassJob(27, "召唤师", JobType.MagicalRanged, null),
            new ClassJob(28, "学者", JobType.Healer, typeof(SCHGauge)),
            new ClassJob(29, "双剑师", JobType.Melee, null),
            new ClassJob(30, "忍者", JobType.Melee, typeof(NINGauge)),
            new ClassJob(31, "机工士", JobType.PhysicalRanged, typeof(MCHGauge)),
            new ClassJob(32, "暗黑骑士", JobType.Tank, typeof(DRKGauge)),
            new ClassJob(33, "占星术士", JobType.Healer, typeof(ASTGauge)),
            new ClassJob(34, "武士", JobType.Melee, typeof(SAMGauge)),
            new ClassJob(35, "赤魔法师", JobType.MagicalRanged, typeof(RDMGauge)),
            new ClassJob(36, "青魔法师", JobType.MagicalRanged, typeof(BLMGauge)),
            new ClassJob(37, "绝枪战士", JobType.Tank, typeof(GNBGauge)),
            new ClassJob(38, "舞者", JobType.PhysicalRanged, typeof(DNCGauge)),
            new ClassJob(39, "钐镰客", JobType.Melee, typeof(RPRGauge)),
            new ClassJob(40, "贤者", JobType.Healer, typeof(SGEGauge)),
        };

        internal static ClassJob GetJobByGauge<T>() where T : JobGaugeBase
        {
            foreach (var job in AllJobs)
            {
                if(job.JobGauge == typeof(T))
                {
                    return job;
                }
            }
            return null;
        }
    }
}
