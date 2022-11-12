using Dalamud.Game.ClientState.JobGauge.Types;
using System;
using System.Collections.Generic;
using XIVAutoAttack.Actions;
using XIVAutoAttack.Actions.BaseAction;
using XIVAutoAttack.Combos.CustomCombo;
using XIVAutoAttack.Data;
using XIVAutoAttack.Helpers;

namespace XIVAutoAttack.Combos.Melee.RPRCombos;

internal abstract class RPRCombo<TCmd> : JobGaugeCombo<RPRGauge, TCmd> where TCmd : Enum
{
    public sealed override ClassJobID[] JobIDs => new ClassJobID[] { ClassJobID.Reaper };

    public class PRPAction : BaseAction
    {
        internal override EnemyLocation EnermyLocation => Player.HaveStatusFromSelf(StatusID.Enshrouded)
            ? EnemyLocation.None : base.EnermyLocation;
        internal PRPAction(uint actionID, bool isFriendly = false, bool shouldEndSpecial = false)
            : base(actionID, isFriendly, shouldEndSpecial)
        {
        }
    }
    protected static byte LemureShroud => JobGauge.LemureShroud;
    protected static bool enshrouded => Player.HaveStatusFromSelf(StatusID.Enshrouded);
    protected static bool soulReaver => Player.HaveStatusFromSelf(StatusID.SoulReaver);
    protected static bool enhancedGibbet => Player.HaveStatusFromSelf(StatusID.EnhancedGibbet);
    protected static bool enhancedGallows => Player.HaveStatusFromSelf(StatusID.EnhancedGallows);
    protected static bool enhancedCrossReaping => Player.HaveStatusFromSelf(StatusID.EnhancedCrossReaping);
    protected static bool enhancedVoidReaping => Player.HaveStatusFromSelf(StatusID.EnhancedVoidReaping);
    protected static bool plentifulReady => Player.HaveStatusFromSelf(StatusID.ImmortalSacrifice) && !Player.HaveStatusFromSelf(StatusID.BloodsownCircle);
    protected static bool haveDeathsDesign => Target.HaveStatusFromSelf(StatusID.DeathsDesign);

    public static readonly BaseAction
    #region 单体
        //切割
        Slice = new(24373)
        {
            OtherCheck = b => !enshrouded || !soulReaver,
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
            OtherCheck = b => !soulReaver,
        },

        //灵魂切割
        SoulSlice = new(24380)
        {
            OtherCheck = b => !enshrouded && !soulReaver && JobGauge.Soul <= 50,
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
            OtherCheck = b => !soulReaver
        },

        //灵魂钐割
        SoulScythe = new(24381)
        {
            OtherCheck = SoulSlice.OtherCheck,
        },
    #endregion
    #region 妖异之镰状态
        //绞决
        Gibbet = new PRPAction(ActionIDs.Gibbet)
        {
            OtherCheck = b => soulReaver && enhancedGibbet,
        },

        //缢杀
        Gallows = new PRPAction(ActionIDs.Gallows)
        {
            OtherCheck = b => soulReaver && (enhancedGallows || !enhancedGibbet),
        },

        //断首
        Guillotine = new(24384)
        {
            OtherCheck = b => soulReaver,
        },
    #endregion
    #region 红条50灵魂
        //隐匿挥割
        BloodStalk = new(24389)
        {
            BuffsProvide = new[] { StatusID.SoulReaver },
            OtherCheck = b => !soulReaver && !enshrouded &&
                              JobGauge.Soul >= 50 && !plentifulReady &&
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
            OtherCheck = b => !soulReaver && !enshrouded && JobGauge.Soul >= 50,
        },
    #endregion
    #region 大爆发
        //神秘环
        ArcaneCircle = new(24405, true)
        {
            OtherCheck = b => InCombat && haveDeathsDesign
        },

        //大丰收
        PlentifulHarvest = new(24385)
        {
            OtherCheck = b => JobGauge.Shroud <= 50 && !soulReaver && !enshrouded && plentifulReady
        },
    #endregion
    #region 蓝条50附体
        //夜游魂衣
        Enshroud = new(24394)
        {
            OtherCheck = b => !soulReaver && !enshrouded && JobGauge.Shroud >= 50,
        },

        //团契
        Communio = new(24398)
        {
            OtherCheck = b => enshrouded && JobGauge.LemureShroud == 1,
        },

        //夜游魂切割
        LemuresSlice = new(24399)
        {
            OtherCheck = b => enshrouded && JobGauge.VoidShroud >= 2,
        },

        //夜游魂钐割
        LemuresScythe = new(24400)
        {
            OtherCheck = LemuresSlice.OtherCheck,
        },

        //虚无收割
        VoidReaping = new(24395)
        {
            OtherCheck = b => enshrouded && JobGauge.LemureShroud > 1 && enhancedVoidReaping,
        },

        //交错收割
        CrossReaping = new(24396)
        {
            OtherCheck = b =>
            {
                if (enshrouded)
                {
                    if (JobGauge.LemureShroud > 1 && (enhancedCrossReaping || !enhancedVoidReaping))
                    {
                        return true;
                    }
                    if (JobGauge.LemureShroud == 1 && !Communio.EnoughLevel && enhancedCrossReaping)
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
            OtherCheck = b => enshrouded,
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
            OtherCheck = b => !enshrouded && !soulReaver
        };
    #endregion

}
