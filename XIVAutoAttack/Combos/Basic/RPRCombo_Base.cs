using Dalamud.Game.ClientState.JobGauge.Types;
using System;
using XIVAutoAttack.Actions;
using XIVAutoAttack.Actions.BaseAction;
using XIVAutoAttack.Combos.CustomCombo;
using XIVAutoAttack.Data;
using XIVAutoAttack.Helpers;

namespace XIVAutoAttack.Combos.Basic;

internal abstract class RPRCombo_Base<TCmd> : CustomCombo<TCmd> where TCmd : Enum
{
    protected static RPRGauge JobGauge => Service.JobGauges.Get<RPRGauge>();

    public sealed override ClassJobID[] JobIDs => new ClassJobID[] { ClassJobID.Reaper };

    public class PRPAction : BaseAction
    {
        internal override EnemyLocation EnermyLocation => Player.HaveStatus(true, StatusID.Enshrouded)
            ? EnemyLocation.None : base.EnermyLocation;
        internal PRPAction(ActionID actionID, bool isFriendly = false, bool shouldEndSpecial = false)
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

    #region 单体
    /// <summary>
    /// 切割
    /// </summary>
    public static BaseAction Slice { get; } = new(ActionID.Slice)
    {
        OtherCheck = b => !Enshrouded || !SoulReaver,
    };

    /// <summary>
    /// 增盈切割
    /// </summary>
    public static BaseAction WaxingSlice { get; } = new(ActionID.WaxingSlice)
    {
        OtherCheck = Slice.OtherCheck,
    };

    /// <summary>
    /// 地狱切割
    /// </summary>
    public static BaseAction InfernalSlice { get; } = new(ActionID.InfernalSlice)
    {
        OtherCheck = Slice.OtherCheck,
    };

    /// <summary>
    /// 死亡之影
    /// </summary>
    public static BaseAction ShadowofDeath { get; } = new(ActionID.ShadowofDeath, isEot: true)
    {
        TargetStatus = new[] { StatusID.DeathsDesign },
        OtherCheck = b => !SoulReaver,
    };

    /// <summary>
    /// 灵魂切割
    /// </summary>
    public static BaseAction SoulSlice { get; } = new(ActionID.SoulSlice)
    {
        OtherCheck = b => !Enshrouded && !SoulReaver && JobGauge.Soul <= 50,
    };
    #endregion
    #region AoE
    /// <summary>
    /// 旋转钐割
    /// </summary>
    public static BaseAction SpinningScythe { get; } = new(ActionID.SpinningScythe)
    {
        OtherCheck = Slice.OtherCheck,
    };

    /// <summary>
    /// 噩梦钐割
    /// </summary>
    public static BaseAction NightmareScythe { get; } = new(ActionID.NightmareScythe)
    {
        OtherCheck = Slice.OtherCheck,
    };

    /// <summary>
    /// 死亡之涡
    /// </summary>
    public static BaseAction WhorlofDeath { get; } = new(ActionID.WhorlofDeath, isEot: true)
    {
        TargetStatus = new[] { StatusID.DeathsDesign },
        OtherCheck = ShadowofDeath.OtherCheck,
    };
    
    /// <summary>
    /// 灵魂钐割
    /// </summary>
    public static BaseAction SoulScythe { get; } = new(ActionID.SoulScythe)
    {
        OtherCheck = SoulSlice.OtherCheck,
    };
    #endregion
    #region 妖异之镰状态
    /// <summary>
    /// 绞决
    /// </summary>
    public static BaseAction Gibbet { get; } = new(ActionID.Gibbet)
    {
        OtherCheck = b => SoulReaver && EnhancedGibbet,
    };

    /// <summary>
    /// 缢杀
    /// </summary>
    public static BaseAction Gallows { get; } = new(ActionID.Gallows)
    {
        OtherCheck = b => SoulReaver && (EnhancedGallows || !EnhancedGibbet),
    };

    /// <summary>
    /// 断首
    /// </summary>
    public static BaseAction Guillotine { get; } = new(ActionID.Guillotine)
    {
        OtherCheck = b => SoulReaver,
    };
    #endregion
    #region 红条50灵魂
    /// <summary>
    /// 隐匿挥割
    /// </summary>
    public static BaseAction BloodStalk { get; } = new(ActionID.BloodStalk)
    {
        BuffsProvide = new[] { StatusID.SoulReaver },
        OtherCheck = b => !SoulReaver && !Enshrouded &&
                          JobGauge.Soul >= 50 && !PlentifulReady &&
                          ((Gluttony.EnoughLevel && !Gluttony.WillHaveOneChargeGCD(4)) || !Gluttony.EnoughLevel),
    };

    /// <summary>
    /// 束缚挥割
    /// </summary>
    public static BaseAction GrimSwathe { get; } = new(ActionID.GrimSwathe)
    {
        OtherCheck = BloodStalk.OtherCheck,
    };

    /// <summary>
    /// 暴食
    /// </summary>
    public static BaseAction Gluttony { get; } = new(ActionID.Gluttony)
    {
        OtherCheck = b => !SoulReaver && !Enshrouded && JobGauge.Soul >= 50,
    };
    #endregion
    #region 大爆发
    /// <summary>
    /// 神秘环
    /// </summary>
    public static BaseAction ArcaneCircle { get; } = new(ActionID.ArcaneCircle, true)
    {
        OtherCheck = b => InCombat && HaveDeathsDesign
    };

    /// <summary>
    /// 大丰收
    /// </summary>
    public static BaseAction PlentifulHarvest { get; } = new(ActionID.PlentifulHarvest)
    {
        OtherCheck = b => JobGauge.Shroud <= 50 && !SoulReaver && !Enshrouded && PlentifulReady
    };
    #endregion
    #region 蓝条50附体
    /// <summary>
    /// 夜游魂衣
    /// </summary>
    public static BaseAction Enshroud { get; } = new(ActionID.Enshroud)
    {
        OtherCheck = b => !SoulReaver && !Enshrouded && JobGauge.Shroud >= 50,
    };

    /// <summary>
    /// 团契
    /// </summary>
    public static BaseAction Communio { get; } = new(ActionID.Communio)
    {
        OtherCheck = b => Enshrouded && JobGauge.LemureShroud == 1,
    };

    /// <summary>
    /// 夜游魂切割
    /// </summary>
    public static BaseAction LemuresSlice { get; } = new(ActionID.LemuresSlice)
    {
        OtherCheck = b => Enshrouded && JobGauge.VoidShroud >= 2,
    };

    /// <summary>
    /// 夜游魂钐割
    /// </summary>
    public static BaseAction LemuresScythe { get; } = new(ActionID.LemuresScythe)
    {
        OtherCheck = LemuresSlice.OtherCheck,
    };

    /// <summary>
    /// 虚无收割
    /// </summary>
    public static BaseAction VoidReaping { get; } = new(ActionID.VoidReaping)
    {
        OtherCheck = b => Enshrouded && JobGauge.LemureShroud > 1 && EnhancedVoidReaping,
    };

    /// <summary>
    /// 交错收割
    /// </summary>
    public static BaseAction CrossReaping { get; } = new(ActionID.CrossReaping)
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
    };

    /// <summary>
    /// 阴冷收割
    /// </summary>
    public static BaseAction GrimReaping { get; } = new(ActionID.GrimReaping)
    {
        OtherCheck = b => {
            if (Enshrouded)
            {
                if (JobGauge.LemureShroud > 1)
                {
                    return true;
                }
                if (JobGauge.LemureShroud == 1 && !Communio.EnoughLevel)
                {
                    return true;
                }
            }
            return false;
        }
    };
    #endregion
    #region 杂项
    /// <summary>
    /// 勾刃
    /// </summary>
    public static BaseAction Harpe { get; } = new(ActionID.Harpe);
    /// <summary>
    /// 播魂种
    /// </summary>
    public static BaseAction Soulsow { get; } = new(ActionID.Soulsow)
    {
        BuffsProvide = new[] { StatusID.Soulsow },
        OtherCheck = b => !InCombat,
    };

    /// <summary>
    /// 收获月
    /// </summary>
    public static BaseAction HarvestMoon { get; } = new(ActionID.HarvestMoon)
    {
        BuffsNeed = new[] { StatusID.Soulsow },
        OtherCheck = b => InCombat,
    };

    /// <summary>
    /// 神秘纹 加盾
    /// </summary>
    public static BaseAction ArcaneCrest { get; } = new(ActionID.ArcaneCrest, true);
    #endregion

}
