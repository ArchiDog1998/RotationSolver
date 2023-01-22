using Dalamud.Game.ClientState.JobGauge.Types;
using System;
using RotationSolver.Helpers;
using RotationSolver.Actions.BaseAction;
using RotationSolver.Actions;
using RotationSolver.Data;

namespace RotationSolver.Rotations.Basic;

internal abstract class RPR_Base : CustomRotation.CustomRotation
{
    private static RPRGauge JobGauge => Service.JobGauges.Get<RPRGauge>();

    /// <summary>
    /// 红色灵魂
    /// </summary>
    protected static byte Soul => JobGauge.Soul;
    /// <summary>
    /// 蓝色灵魂
    /// </summary>
    protected static byte Shroud => JobGauge.Shroud;
    /// <summary>
    /// 夜游魂
    /// </summary>
    protected static byte LemureShroud => JobGauge.LemureShroud;
    /// <summary>
    /// 虚无魂
    /// </summary>
    protected static byte VoidShroud => JobGauge.VoidShroud;
    /// <summary>
    /// 夜游魂附体
    /// </summary>
    protected static bool Enshrouded => Player.HasStatus(true, StatusID.Enshrouded);
    /// <summary>
    /// 妖异之镰状态
    /// </summary>
    protected static bool SoulReaver => Player.HasStatus(true, StatusID.SoulReaver);

    public sealed override ClassJobID[] JobIDs => new ClassJobID[] { ClassJobID.Reaper };

    public class PRPAction : BaseAction
    {
        internal override EnemyPositional EnermyPositonal => Player.HasStatus(true, StatusID.Enshrouded)
            ? EnemyPositional.None : base.EnermyPositonal;
        internal PRPAction(ActionID actionID, bool isFriendly = false, bool shouldEndSpecial = false)
            : base(actionID, isFriendly, shouldEndSpecial)
        {
        }
    }

    #region 单体
    /// <summary>
    /// 切割
    /// </summary>
    public static IBaseAction Slice { get; } = new BaseAction(ActionID.Slice)
    {
        ActionCheck = b => !Enshrouded && !SoulReaver,
    };

    /// <summary>
    /// 增盈切割
    /// </summary>
    public static IBaseAction WaxingSlice { get; } = new BaseAction(ActionID.WaxingSlice)
    {
        ActionCheck = Slice.ActionCheck,
    };

    /// <summary>
    /// 地狱切割
    /// </summary>
    public static IBaseAction InfernalSlice { get; } = new BaseAction(ActionID.InfernalSlice)
    {
        ActionCheck = Slice.ActionCheck,
    };

    /// <summary>
    /// 死亡之影
    /// </summary>
    public static IBaseAction ShadowofDeath { get; } = new BaseAction(ActionID.ShadowofDeath, isEot: true)
    {
        TargetStatus = new[] { StatusID.DeathsDesign },
        ActionCheck = b => !SoulReaver,
    };

    /// <summary>
    /// 灵魂切割
    /// </summary>
    public static IBaseAction SoulSlice { get; } = new BaseAction(ActionID.SoulSlice)
    {
        ActionCheck = b => !Enshrouded && !SoulReaver && Soul <= 50,
    };
    #endregion
    #region AoE
    /// <summary>
    /// 旋转钐割
    /// </summary>
    public static IBaseAction SpinningScythe { get; } = new BaseAction(ActionID.SpinningScythe)
    {
        ActionCheck = Slice.ActionCheck,
    };

    /// <summary>
    /// 噩梦钐割
    /// </summary>
    public static IBaseAction NightmareScythe { get; } = new BaseAction(ActionID.NightmareScythe)
    {
        ActionCheck = Slice.ActionCheck,
    };

    /// <summary>
    /// 死亡之涡
    /// </summary>
    public static IBaseAction WhorlofDeath { get; } = new BaseAction(ActionID.WhorlofDeath, isEot: true)
    {
        TargetStatus = new[] { StatusID.DeathsDesign },
        ActionCheck = ShadowofDeath.ActionCheck,
    };

    /// <summary>
    /// 灵魂钐割
    /// </summary>
    public static IBaseAction SoulScythe { get; } = new BaseAction(ActionID.SoulScythe)
    {
        ActionCheck = SoulSlice.ActionCheck,
    };
    #endregion
    #region 妖异之镰状态
    /// <summary>
    /// 绞决
    /// </summary>
    public static IBaseAction Gibbet { get; } = new BaseAction(ActionID.Gibbet)
    {
        StatusNeed = new[] { StatusID.SoulReaver }
    };

    /// <summary>
    /// 缢杀
    /// </summary>
    public static IBaseAction Gallows { get; } = new BaseAction(ActionID.Gallows)
    {
        StatusNeed = new[] { StatusID.SoulReaver }
    };

    /// <summary>
    /// 断首
    /// </summary>
    public static IBaseAction Guillotine { get; } = new BaseAction(ActionID.Guillotine)
    {
        StatusNeed = new[] { StatusID.SoulReaver }
    };
    #endregion
    #region 红条50灵魂
    /// <summary>
    /// 隐匿挥割
    /// </summary>
    public static IBaseAction BloodStalk { get; } = new BaseAction(ActionID.BloodStalk)
    {
        StatusProvide = new[] { StatusID.SoulReaver },
        ActionCheck = b => !SoulReaver && !Enshrouded && Soul >= 50
    };

    /// <summary>
    /// 束缚挥割
    /// </summary>
    public static IBaseAction GrimSwathe { get; } = new BaseAction(ActionID.GrimSwathe)
    {
        StatusProvide = new[] { StatusID.SoulReaver },
        ActionCheck = BloodStalk.ActionCheck,
    };

    /// <summary>
    /// 暴食
    /// </summary>
    public static IBaseAction Gluttony { get; } = new BaseAction(ActionID.Gluttony)
    {
        StatusProvide = new[] { StatusID.SoulReaver },
        ActionCheck = BloodStalk.ActionCheck,
    };
    #endregion
    #region 大爆发
    /// <summary>
    /// 神秘环
    /// </summary>
    public static IBaseAction ArcaneCircle { get; } = new BaseAction(ActionID.ArcaneCircle, true)
    {
        StatusProvide = new[] { StatusID.CircleofSacrifice, StatusID.BloodsownCircle }
    };

    /// <summary>
    /// 大丰收
    /// </summary>
    public static IBaseAction PlentifulHarvest { get; } = new BaseAction(ActionID.PlentifulHarvest)
    {
        StatusNeed = new[] { StatusID.ImmortalSacrifice },
        ActionCheck = b => !Player.HasStatus(true, StatusID.BloodsownCircle)
    };
    #endregion
    #region 蓝条50附体
    /// <summary>
    /// 夜游魂衣
    /// </summary>
    public static IBaseAction Enshroud { get; } = new BaseAction(ActionID.Enshroud)
    {
        StatusProvide = new[] { StatusID.Enshrouded },
        ActionCheck = b => Shroud >= 50 && !SoulReaver && !Enshrouded
    };

    /// <summary>
    /// 团契
    /// </summary>
    public static IBaseAction Communio { get; } = new BaseAction(ActionID.Communio)
    {
        StatusNeed = new[] { StatusID.Enshrouded },
        ActionCheck = b => LemureShroud == 1
    };

    /// <summary>
    /// 夜游魂切割
    /// </summary>
    public static IBaseAction LemuresSlice { get; } = new BaseAction(ActionID.LemuresSlice)
    {
        StatusNeed = new[] { StatusID.Enshrouded },
        ActionCheck = b => VoidShroud >= 2,
    };

    /// <summary>
    /// 夜游魂钐割
    /// </summary>
    public static IBaseAction LemuresScythe { get; } = new BaseAction(ActionID.LemuresScythe)
    {
        StatusNeed = new[] { StatusID.Enshrouded },
        ActionCheck = b => VoidShroud >= 2,
    };

    /// <summary>
    /// 虚无收割
    /// </summary>
    public static IBaseAction VoidReaping { get; } = new BaseAction(ActionID.VoidReaping)
    {
        StatusNeed = new[] { StatusID.Enshrouded },
    };

    /// <summary>
    /// 交错收割
    /// </summary>
    public static IBaseAction CrossReaping { get; } = new BaseAction(ActionID.CrossReaping)
    {
        StatusNeed = new[] { StatusID.Enshrouded },
    };

    /// <summary>
    /// 阴冷收割
    /// </summary>
    public static IBaseAction GrimReaping { get; } = new BaseAction(ActionID.GrimReaping)
    {
        StatusNeed = new[] { StatusID.Enshrouded },
    };
    #endregion
    #region 杂项
    /// <summary>
    /// 勾刃
    /// </summary>
    public static IBaseAction Harpe { get; } = new BaseAction(ActionID.Harpe)
    {
        ActionCheck = b => !SoulReaver
    };

    /// <summary>
    /// 地狱入境
    /// </summary>
    public static IBaseAction HellsIngress { get; } = new BaseAction(ActionID.HellsIngress)
    {
        StatusProvide = new[] { StatusID.EnhancedHarpe },
        ActionCheck = b => !Player.HasStatus(true, StatusID.Bind1, StatusID.Bind2)
    };

    /// <summary>
    /// 地狱出境
    /// </summary>
    public static IBaseAction HellsEgress { get; } = new BaseAction(ActionID.HellsEgress)
    {
        StatusProvide = HellsIngress.StatusProvide,
        ActionCheck = HellsIngress.ActionCheck
    };

    /// <summary>
    /// 播魂种
    /// </summary>
    public static IBaseAction Soulsow { get; } = new BaseAction(ActionID.Soulsow)
    {
        StatusProvide = new[] { StatusID.Soulsow },
        ActionCheck = b => !InCombat,
    };

    /// <summary>
    /// 收获月
    /// </summary>
    public static IBaseAction HarvestMoon { get; } = new BaseAction(ActionID.HarvestMoon)
    {
        StatusNeed = new[] { StatusID.Soulsow },
    };

    /// <summary>
    /// 神秘纹 加盾
    /// </summary>
    public static IBaseAction ArcaneCrest { get; } = new BaseAction(ActionID.ArcaneCrest, true, isTimeline: true);
    #endregion


    private protected sealed override bool MoveForwardAbility(byte abilitiesRemaining, out IAction act)
    {
        //E上去
        if (HellsIngress.CanUse(out act, emptyOrSkipCombo: true)) return true;
        return false;
    }
}
