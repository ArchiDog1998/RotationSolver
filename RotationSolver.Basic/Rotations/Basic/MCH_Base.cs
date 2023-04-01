using Dalamud.Game.ClientState.JobGauge.Types;
using RotationSolver.Actions.BaseAction;
using RotationSolver.Basic.Actions;
using RotationSolver.Basic.Attributes;
using RotationSolver.Basic.Data;
using RotationSolver.Basic.Helpers;
using RotationSolver.Rotations.CustomRotation;

namespace RotationSolver.Basic.Rotations.Basic;

public abstract class MCH_Base : CustomRotation
{
    private static MCHGauge JobGauge => Service.JobGauges.Get<MCHGauge>();

    public override MedicineType MedicineType => MedicineType.Dexterity;

    /// <summary>
    /// 处于过热中
    /// </summary>
    protected static bool IsOverheated => JobGauge.IsOverheated;

    /// <summary>
    /// 热量还有多少
    /// </summary>
    protected static byte Heat => JobGauge.Heat;

    /// <summary>
    /// 点量还有多少
    /// </summary>
    protected static byte Battery => JobGauge.Battery;

    /// <summary>
    /// 过热在这么久后，还有吗
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    protected static bool OverheatedEndAfter(float time)
    {
        return EndAfter(JobGauge.OverheatTimeRemaining / 1000f, time);
    }

    /// <summary>
    /// 过热在这么久后，还有吗
    /// </summary>
    /// <param name="abilityCount"></param>
    /// <param name="gctCount"></param>
    /// <returns></returns>
    protected static bool OverheatedEndAfterGCD(uint gctCount = 0, uint abilityCount = 0)
    {
        return EndAfterGCD(JobGauge.OverheatTimeRemaining / 1000f, gctCount, abilityCount);
    }

    public sealed override ClassJobID[] JobIDs => new ClassJobID[] { ClassJobID.Machinist };

    /// <summary>
    /// 分裂弹
    /// </summary>
    public static IBaseAction SplitShot { get; } = new BaseAction(ActionID.SplitShot);

    /// <summary>
    /// 独头弹
    /// </summary>
    public static IBaseAction SlugShot { get; } = new BaseAction(ActionID.SlugShot)
    {
        ComboIds = new[] { ActionID.HeatedSplitShot },
    };

    /// <summary>
    /// 狙击弹
    /// </summary>
    public static IBaseAction CleanShot { get; } = new BaseAction(ActionID.CleanShot)
    {
        ComboIds = new[] { ActionID.HeatedSlugShot },
    };

    /// <summary>
    /// 热冲击
    /// </summary>
    public static IBaseAction HeatBlast { get; } = new BaseAction(ActionID.HeatBlast)
    {
        ActionCheck = b => JobGauge.IsOverheated
        && !OverheatedEndAfterGCD(),
    };

    /// <summary>
    /// 散射
    /// </summary>
    public static IBaseAction SpreadShot { get; } = new BaseAction(ActionID.SpreadShot);

    /// <summary>
    /// 自动弩
    /// </summary>
    public static IBaseAction AutoCrossbow { get; } = new BaseAction(ActionID.AutoCrossbow)
    {
        ActionCheck = HeatBlast.ActionCheck,
    };

    /// <summary>
    /// 热弹
    /// </summary>
    public static IBaseAction HotShot { get; } = new BaseAction(ActionID.HotShot);

    /// <summary>
    /// 空气锚
    /// </summary>
    public static IBaseAction AirAnchor { get; } = new BaseAction(ActionID.AirAnchor);

    /// <summary>
    /// 钻头
    /// </summary>
    public static IBaseAction Drill { get; } = new BaseAction(ActionID.Drill);

    /// <summary>
    /// 回转飞锯
    /// </summary>
    public static IBaseAction ChainSaw { get; } = new BaseAction(ActionID.ChainSaw);

    /// <summary>
    /// 毒菌冲击
    /// </summary>
    public static IBaseAction BioBlaster { get; } = new BaseAction(ActionID.BioBlaster, isEot: true);

    /// <summary>
    /// 整备
    /// </summary>
    public static IBaseAction Reassemble { get; } = new BaseAction(ActionID.Reassemble)
    {
        StatusProvide = new StatusID[] { StatusID.Reassemble },
        ActionCheck = b => HasHostilesInRange,
    };

    /// <summary>
    /// 超荷
    /// </summary>
    public static IBaseAction Hypercharge { get; } = new BaseAction(ActionID.Hypercharge)
    {
        ActionCheck = b => !JobGauge.IsOverheated && JobGauge.Heat >= 50,
    };

    /// <summary>
    /// 野火
    /// </summary>
    public static IBaseAction Wildfire { get; } = new BaseAction(ActionID.Wildfire);

    /// <summary>
    /// 虹吸弹
    /// </summary>
    public static IBaseAction GaussRound { get; } = new BaseAction(ActionID.GaussRound);

    /// <summary>
    /// 弹射
    /// </summary>
    public static IBaseAction Ricochet { get; } = new BaseAction(ActionID.Ricochet);

    /// <summary>
    /// 枪管加热
    /// </summary>
    public static IBaseAction BarrelStabilizer { get; } = new BaseAction(ActionID.BarrelStabilizer)
    {
        ActionCheck = b => JobGauge.Heat <= 50 && InCombat,
    };

    /// <summary>
    /// 车式浮空炮塔
    /// </summary>
    public static IBaseAction RookAutoturret { get; } = new BaseAction(ActionID.RookAutoturret)
    {
        ActionCheck = b => JobGauge.Battery >= 50 && !JobGauge.IsRobotActive,
    };

    /// <summary>
    /// 策动
    /// </summary>
    public static IBaseAction Tactician { get; } = new BaseAction(ActionID.Tactician, true, isTimeline: true)
    {
        ActionCheck = b => !Player.HasStatus(false, StatusID.Troubadour,
            StatusID.Tactician1,
            StatusID.Tactician2,
            StatusID.ShieldSamba),
    };

    public static IBaseAction Dismantle { get; } = new BaseAction(ActionID.Dismantle, true, isTimeline: true);

    [RotationDesc(ActionID.Tactician, ActionID.Dismantle)]
    protected sealed override bool DefenseAreaAbility(byte abilitiesRemaining, out IAction act)
    {
        if (Tactician.CanUse(out act, CanUseOption.MustUse)) return true;
        if (Dismantle.CanUse(out act, CanUseOption.MustUse)) return true;
        return false;
    }
}
