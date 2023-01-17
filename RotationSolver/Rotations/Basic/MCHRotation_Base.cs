using Dalamud.Game.ClientState.JobGauge.Types;
using System;
using RotationSolver.Actions.BaseAction;
using RotationSolver.Combos.CustomCombo;
using RotationSolver.Helpers;
using RotationSolver.Data;

namespace RotationSolver.Combos.Basic;

internal abstract class MCHRotation_Base : CustomCombo.CustomRotation
{
    private static MCHGauge JobGauge => Service.JobGauges.Get<MCHGauge>();

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
    public static BaseAction SplitShot { get; } = new(ActionID.SplitShot);

    /// <summary>
    /// 独头弹
    /// </summary>
    public static BaseAction SlugShot { get; } = new(ActionID.SlugShot)
    {
        OtherIDsCombo = new[] { ActionID.HeatedSplitShot },
    };

    /// <summary>
    /// 狙击弹
    /// </summary>
    public static BaseAction CleanShot { get; } = new(ActionID.CleanShot)
    {
        OtherIDsCombo = new[] { ActionID.HeatedSlugShot },
    };

    /// <summary>
    /// 热冲击
    /// </summary>
    public static BaseAction HeatBlast { get; } = new(ActionID.HeatBlast)
    {
        ActionCheck = b => JobGauge.IsOverheated
        && !OverheatedEndAfterGCD(),
    };

    /// <summary>
    /// 散射
    /// </summary>
    public static BaseAction SpreadShot { get; } = new(ActionID.SpreadShot);

    /// <summary>
    /// 自动弩
    /// </summary>
    public static BaseAction AutoCrossbow { get; } = new(ActionID.AutoCrossbow)
    {
        ActionCheck = HeatBlast.ActionCheck,
    };

    /// <summary>
    /// 热弹
    /// </summary>
    public static BaseAction HotShot { get; } = new(ActionID.HotShot);

    /// <summary>
    /// 空气锚
    /// </summary>
    public static BaseAction AirAnchor { get; } = new(ActionID.AirAnchor);

    /// <summary>
    /// 钻头
    /// </summary>
    public static BaseAction Drill { get; } = new(ActionID.Drill);

    /// <summary>
    /// 回转飞锯
    /// </summary>
    public static BaseAction ChainSaw { get; } = new(ActionID.ChainSaw);

    /// <summary>
    /// 毒菌冲击
    /// </summary>
    public static BaseAction Bioblaster { get; } = new(ActionID.Bioblaster, isEot: true);

    /// <summary>
    /// 整备
    /// </summary>
    public static BaseAction Reassemble { get; } = new(ActionID.Reassemble)
    {
        BuffsProvide = new StatusID[] { StatusID.Reassemble },
        ActionCheck = b => HaveHostilesInRange,
    };

    /// <summary>
    /// 超荷
    /// </summary>
    public static BaseAction Hypercharge { get; } = new(ActionID.Hypercharge)
    {
        ActionCheck = b => !JobGauge.IsOverheated && JobGauge.Heat >= 50,
    };

    /// <summary>
    /// 野火
    /// </summary>
    public static BaseAction Wildfire { get; } = new(ActionID.Wildfire);

    /// <summary>
    /// 虹吸弹
    /// </summary>
    public static BaseAction GaussRound { get; } = new(ActionID.GaussRound);

    /// <summary>
    /// 弹射
    /// </summary>
    public static BaseAction Ricochet { get; } = new(ActionID.Ricochet);

    /// <summary>
    /// 枪管加热
    /// </summary>
    public static BaseAction BarrelStabilizer { get; } = new(ActionID.BarrelStabilizer)
    {
        ActionCheck = b => JobGauge.Heat <= 50 && InCombat,
    };

    /// <summary>
    /// 车式浮空炮塔
    /// </summary>
    public static BaseAction RookAutoturret { get; } = new(ActionID.RookAutoturret)
    {
        ActionCheck = b => JobGauge.Battery >= 50 && !JobGauge.IsRobotActive,
    };

    /// <summary>
    /// 策动
    /// </summary>
    public static BaseAction Tactician { get; } = new(ActionID.Tactician, true, isTimeline: true)
    {
        ActionCheck = b => !Player.HasStatus(false, StatusID.Troubadour,
            StatusID.Tactician1,
            StatusID.Tactician2,
            StatusID.ShieldSamba),
    };
}
