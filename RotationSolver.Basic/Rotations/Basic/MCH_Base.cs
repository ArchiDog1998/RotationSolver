using ECommons.DalamudServices;
using RotationSolver.Basic.Traits;

namespace RotationSolver.Basic.Rotations.Basic;

public abstract class MCH_Base : CustomRotation
{
    public override MedicineType MedicineType => MedicineType.Dexterity;

    public sealed override ClassJobID[] JobIDs => new ClassJobID[] { ClassJobID.Machinist };

    #region Job Gauge
    static MCHGauge JobGauge => Svc.Gauges.Get<MCHGauge>();

    protected static bool IsOverheated => JobGauge.IsOverheated;

    protected static byte Heat => JobGauge.Heat;

    protected static byte Battery => JobGauge.Battery;

    static float OverheatTimeRemaining => JobGauge.OverheatTimeRemaining / 1000f;

    protected static bool OverheatedEndAfter(float time)
    {
        return EndAfter(OverheatTimeRemaining, time);
    }

    protected static bool OverheatedEndAfterGCD(uint gctCount = 0, float offset = 0)
    {
        return EndAfterGCD(OverheatTimeRemaining, gctCount, offset);
    }
    #endregion

    #region Attack Single
    /// <summary>
    /// 1
    /// </summary>
    public static IBaseAction SplitShot { get; } = new BaseAction(ActionID.SplitShot);

    /// <summary>
    /// 2
    /// </summary>
    public static IBaseAction SlugShot { get; } = new BaseAction(ActionID.SlugShot)
    {
        ComboIds = new[] { ActionID.HeatedSplitShot },
    };

    /// <summary>
    /// 3
    /// </summary>
    public static IBaseAction CleanShot { get; } = new BaseAction(ActionID.CleanShot)
    {
        ComboIds = new[] { ActionID.HeatedSlugShot },
    };

    public static IBaseAction HeatBlast { get; } = new BaseAction(ActionID.HeatBlast)
    {
        ActionCheck = (b, m) => IsOverheated && !OverheatedEndAfterGCD(),
    };

    public static IBaseAction HotShot { get; } = new BaseAction(ActionID.HotShot);

    public static IBaseAction AirAnchor { get; } = new BaseAction(ActionID.AirAnchor);

    public static IBaseAction Drill { get; } = new BaseAction(ActionID.Drill);

    public static IBaseAction GaussRound { get; } = new BaseAction(ActionID.GaussRound);

    #endregion

    #region Attack Area
    /// <summary>
    /// 1
    /// </summary>
    public static IBaseAction SpreadShot { get; } = new BaseAction(ActionID.SpreadShot);

    /// <summary>
    /// 2
    /// </summary>
    public static IBaseAction AutoCrossbow { get; } = new BaseAction(ActionID.AutoCrossbow)
    {
        ActionCheck = HeatBlast.ActionCheck,
    };

    public static IBaseAction ChainSaw { get; } = new BaseAction(ActionID.ChainSaw);

    public static IBaseAction BioBlaster { get; } = new BaseAction(ActionID.BioBlaster, ActionOption.Dot);

    public static IBaseAction Ricochet { get; } = new BaseAction(ActionID.Ricochet);

    public static IBaseAction RookAutoturret { get; } = new BaseAction(ActionID.RookAutoturret)
    {
        ActionCheck = (b, m) => JobGauge.Battery >= 50 && !JobGauge.IsRobotActive,
    };
    #endregion

    #region Support
    public static IBaseAction Reassemble { get; } = new BaseAction(ActionID.Reassemble)
    {
        StatusProvide = new StatusID[] { StatusID.Reassemble },
        ActionCheck = (b, m) => HasHostilesInRange,
    };

    public static IBaseAction Hypercharge { get; } = new BaseAction(ActionID.Hypercharge)
    {
        ActionCheck = (b, m) => !JobGauge.IsOverheated && JobGauge.Heat >= 50,
    };

    public static IBaseAction Wildfire { get; } = new BaseAction(ActionID.Wildfire);

    public static IBaseAction BarrelStabilizer { get; } = new BaseAction(ActionID.BarrelStabilizer)
    {
        ActionCheck = (b, m) => JobGauge.Heat <= 50 && InCombat,
    };
    #endregion

    #region Defense
    public static IBaseAction Tactician { get; } = new BaseAction(ActionID.Tactician, ActionOption.Defense)
    {
        ActionCheck = (b, m) => !Player.HasStatus(false, StatusID.Troubadour, StatusID.Tactician1, StatusID.Tactician2, StatusID.ShieldSamba),
    };

    public static IBaseAction Dismantle { get; } = new BaseAction(ActionID.Dismantle, ActionOption.Defense);
    #endregion

    #region Traits
    protected static IBaseTrait IncreasedActionDamage { get; } = new BaseTrait(117);
    protected static IBaseTrait IncreasedActionDamage2 { get; } = new BaseTrait(119);
    protected static IBaseTrait SplitShotMastery    { get; } = new BaseTrait(288);
    protected static IBaseTrait SlugShotMastery    { get; } = new BaseTrait(289);
    protected static IBaseTrait CleanShotMastery    { get; } = new BaseTrait(290);
    protected static IBaseTrait HotShotMastery    { get; } = new BaseTrait(291);
    protected static IBaseTrait ChargedActionMastery    { get; } = new BaseTrait(292);
    protected static IBaseTrait EnhancedWildfire    { get; } = new BaseTrait(293);
    protected static IBaseTrait Promotion    { get; } = new BaseTrait(294);
    protected static IBaseTrait SpreadShotMastery    { get; } = new BaseTrait(449);
    protected static IBaseTrait EnhancedReassemble    { get; } = new BaseTrait(450);
    protected static IBaseTrait QueensGambit    { get; } = new BaseTrait(451);
    protected static IBaseTrait EnhancedTactician    { get; } = new BaseTrait(452);
    protected static IBaseTrait MarksmansMastery    { get; } = new BaseTrait(517);
    #endregion

    [RotationDesc(ActionID.Tactician, ActionID.Dismantle)]
    protected sealed override bool DefenseAreaAbility(out IAction act)
    {
        if (Tactician.CanUse(out act, CanUseOption.MustUse)) return true;
        if (Dismantle.CanUse(out act, CanUseOption.MustUse)) return true;
        return false;
    }
}
