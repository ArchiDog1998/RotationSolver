using Dalamud.Game.ClientState.JobGauge.Types;
using System;
using XIVAutoAttack.Actions;
using XIVAutoAttack.Actions.BaseAction;
using XIVAutoAttack.Combos.CustomCombo;
using XIVAutoAttack.Data;
using XIVAutoAttack.Helpers;

namespace XIVAutoAttack.Combos.Basic;

internal abstract class RPRCombo_Base<TCmd> : JobGaugeCombo<RPRGauge, TCmd> where TCmd : Enum
{
    public sealed override ClassJobID[] JobIDs => new ClassJobID[] { ClassJobID.Reaper };

    public class PRPAction : BaseAction
    {
        internal override EnemyLocation EnermyLocation => Player.HaveStatus(true, StatusID.Enshrouded)
            ? EnemyLocation.None : base.EnermyLocation;
        internal PRPAction(uint actionID, bool isFriendly = false, bool shouldEndSpecial = false)
            : base(actionID, isFriendly, shouldEndSpecial)
        {
        }
    }
    protected static bool Enshrouded => Player.HaveStatus(true, StatusID.Enshrouded);
    protected static bool SoulReaver => Player.HaveStatus(true, StatusID.SoulReaver);
    protected static bool EnhancedGibbet => Player.HaveStatus(true, StatusID.EnhancedGibbet);
    protected static bool EnhancedGallows => Player.HaveStatus(true, StatusID.EnhancedGallows);
    protected static bool EnhancedCrossReaping => Player.HaveStatus(true, StatusID.EnhancedCrossReaping);
    protected static bool EnhancedVoidReaping => Player.HaveStatus(true, StatusID.EnhancedVoidReaping);
    protected static bool PlentifulReady => Player.HaveStatus(true, StatusID.ImmortalSacrifice) && !Player.HaveStatus(true, StatusID.BloodsownCircle);
    protected static bool HaveDeathsDesign => Target.HaveStatus(true, StatusID.DeathsDesign);

    public static readonly BaseAction
    #region 单体
        //切割
        Slice = new(24373)
        {
            OtherCheck = b => !Enshrouded || !SoulReaver,
        },

        //增盈切割
        WaxingSlice = new(24374)
        {
            OtherCheck = Slice.OtherCheck,
        },

        //地狱切割
        InfernalSlice = new(24375)
        {
            OtherCheck = Slice.OtherCheck,
        },

        //死亡之影
        ShadowofDeath = new(24378, isEot: true)
        {
            TargetStatus = new[] { StatusID.DeathsDesign },
            OtherCheck = b => !SoulReaver,
        },

        //灵魂切割
        SoulSlice = new(24380)
        {
            OtherCheck = b => !Enshrouded && !SoulReaver && JobGauge.Soul <= 50,
        },
    #endregion
    #region AoE
        //旋转钐割
        SpinningScythe = new(24376)
        {
            OtherCheck = Slice.OtherCheck,
        },

        //噩梦钐割
        NightmareScythe = new(24377)
        {
            OtherCheck = Slice.OtherCheck,
        },

        //死亡之涡
        WhorlofDeath = new(24379, isEot: true)
        {
            TargetStatus = new[] { StatusID.DeathsDesign },
            OtherCheck = b => !SoulReaver
        },

        //灵魂钐割
        SoulScythe = new(24381)
        {
            OtherCheck = SoulSlice.OtherCheck,
        },
    #endregion
    #region 妖异之镰状态
        //绞决
        Gibbet = new PRPAction(ActionID.Gibbet)
        {
            OtherCheck = b => SoulReaver && EnhancedGibbet,
        },

        //缢杀
        Gallows = new PRPAction(ActionID.Gallows)
        {
            OtherCheck = b => SoulReaver && (EnhancedGallows || !EnhancedGibbet),
        },

        //断首
        Guillotine = new(24384)
        {
            OtherCheck = b => SoulReaver,
        },
    #endregion
    #region 红条50灵魂
        //隐匿挥割
        BloodStalk = new(24389)
        {
            BuffsProvide = new[] { StatusID.SoulReaver },
            OtherCheck = b => !SoulReaver && !Enshrouded &&
                              JobGauge.Soul >= 50 && !PlentifulReady &&
                              (Gluttony.EnoughLevel && !Gluttony.WillHaveOneChargeGCD(4) || !Gluttony.EnoughLevel),
        },

        //束缚挥割
        GrimSwathe = new(24392)
        {
            OtherCheck = BloodStalk.OtherCheck,
        },

        //暴食
        Gluttony = new(24393)
        {
            OtherCheck = b => !SoulReaver && !Enshrouded && JobGauge.Soul >= 50,
        },
    #endregion
    #region 大爆发
        //神秘环
        ArcaneCircle = new(24405, true)
        {
            OtherCheck = b => InCombat && HaveDeathsDesign
        },

        //大丰收
        PlentifulHarvest = new(24385)
        {
            OtherCheck = b => JobGauge.Shroud <= 50 && !SoulReaver && !Enshrouded && PlentifulReady
        },
    #endregion
    #region 蓝条50附体
        //夜游魂衣
        Enshroud = new(24394)
        {
            OtherCheck = b => !SoulReaver && !Enshrouded && JobGauge.Shroud >= 50,
        },

        //团契
        Communio = new(24398)
        {
            OtherCheck = b => Enshrouded && JobGauge.LemureShroud == 1,
        },

        //夜游魂切割
        LemuresSlice = new(24399)
        {
            OtherCheck = b => Enshrouded && JobGauge.VoidShroud >= 2,
        },

        //夜游魂钐割
        LemuresScythe = new(24400)
        {
            OtherCheck = LemuresSlice.OtherCheck,
        },

        //虚无收割
        VoidReaping = new(24395)
        {
            OtherCheck = b => Enshrouded && JobGauge.LemureShroud > 1 && EnhancedVoidReaping,
        },

        //交错收割
        CrossReaping = new(24396)
        {
            OtherCheck = b =>
            {
                if (Enshrouded)
                {
                    if (JobGauge.LemureShroud > 1 && (EnhancedCrossReaping || !EnhancedVoidReaping))
                    {
                        return true;
                    }
                    if (JobGauge.LemureShroud == 1 && !Communio.EnoughLevel && EnhancedCrossReaping)
                    {
                        return true;
                    }
                }
                return false;
            }
        },

        //阴冷收割
        GrimReaping = new(24397)
        {
            OtherCheck = b => Enshrouded,
        },
    #endregion
    #region 杂项
        //勾刃
        Harpe = new(24386),

        //播魂种
        Soulsow = new(24387)
        {
            BuffsProvide = new[] { StatusID.Soulsow },
            OtherCheck = b => !InCombat,
        },

        //收获月
        HarvestMoon = new(24388)
        {
            BuffsNeed = new[] { StatusID.Soulsow },
            OtherCheck = b => InCombat,
        },

        //神秘纹 加盾
        ArcaneCrest = new(24404, true)
        {
            OtherCheck = b => !Enshrouded && !SoulReaver
        };
    #endregion

}
