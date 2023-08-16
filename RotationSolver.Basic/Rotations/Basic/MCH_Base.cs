using ECommons.DalamudServices;
using ECommons.ExcelServices;
using RotationSolver.Basic.Traits;

namespace RotationSolver.Basic.Rotations.Basic;

/// <summary>
/// The base Class of MCH.
/// </summary>
public abstract class MCH_Base : CustomRotation
{
    /// <summary>
    /// 
    /// </summary>
    public override MedicineType MedicineType => MedicineType.Dexterity;

    /// <summary>
    /// 
    /// </summary>
    public sealed override Job[] Jobs => new [] { Job.MCH };

    #region Job Gauge
    static MCHGauge JobGauge => Svc.Gauges.Get<MCHGauge>();

    /// <summary>
    /// 
    /// </summary>
    public static bool IsOverheated => JobGauge.IsOverheated;

    /// <summary>
    /// 
    /// </summary>
    public static byte Heat => JobGauge.Heat;

    /// <summary>
    /// 
    /// </summary>
    public static byte Battery => JobGauge.Battery;

    static float OverheatTimeRemainingRaw => JobGauge.OverheatTimeRemaining / 1000f;

    /// <summary>
    /// 
    /// </summary>
    public static float OverheatTime => OverheatTimeRemainingRaw - DataCenter.WeaponRemain;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    protected static bool OverheatedEndAfter(float time) => OverheatTime <= time;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="gctCount"></param>
    /// <param name="offset"></param>
    /// <returns></returns>
    protected static bool OverheatedEndAfterGCD(uint gctCount = 0, float offset = 0)
        => OverheatedEndAfter(GCDTime(gctCount, offset));
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

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction HeatBlast { get; } = new BaseAction(ActionID.HeatBlast)
    {
        ActionCheck = (b, m) => IsOverheated && !OverheatedEndAfterGCD(),
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction HotShot { get; } = new BaseAction(ActionID.HotShot);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction AirAnchor { get; } = new BaseAction(ActionID.AirAnchor);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction Drill { get; } = new BaseAction(ActionID.Drill);

    /// <summary>
    /// 
    /// </summary>
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

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction ChainSaw { get; } = new BaseAction(ActionID.ChainSaw);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction BioBlaster { get; } = new BaseAction(ActionID.BioBlaster, ActionOption.Dot)
    {
        TimeToDie = 10,
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction Ricochet { get; } = new BaseAction(ActionID.Ricochet);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction RookAutoturret { get; } = new BaseAction(ActionID.RookAutoturret)
    {
        ActionCheck = (b, m) => JobGauge.Battery >= 50 && !JobGauge.IsRobotActive,
    };
    #endregion

    #region Support
    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction Reassemble { get; } = new BaseAction(ActionID.Reassemble)
    {
        StatusProvide = new StatusID[] { StatusID.Reassemble },
        ActionCheck = (b, m) => HasHostilesInRange,
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction Hypercharge { get; } = new BaseAction(ActionID.Hypercharge)
    {
        ActionCheck = (b, m) => !JobGauge.IsOverheated && JobGauge.Heat >= 50,
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction Wildfire { get; } = new BaseAction(ActionID.Wildfire);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction Detonator { get; } = new BaseAction(ActionID.Detonator);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction QueenOverdrive { get; } = new BaseAction(ActionID.QueenOverdrive);
    
    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction BarrelStabilizer { get; } = new BaseAction(ActionID.BarrelStabilizer)
    {
        ActionCheck = (b, m) => JobGauge.Heat <= 50 && InCombat,
    };
    #endregion

    #region Defense
    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction Tactician { get; } = new BaseAction(ActionID.Tactician, ActionOption.Defense)
    {
        ActionCheck = (b, m) => !Player.HasStatus(false, StatusID.Troubadour, StatusID.Tactician1, StatusID.Tactician2, StatusID.ShieldSamba),
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction Dismantle { get; } = new BaseAction(ActionID.Dismantle, ActionOption.Defense);
    #endregion

    #region Traits
    /// <summary>
    /// 
    /// </summary>
    protected static IBaseTrait IncreasedActionDamage { get; } = new BaseTrait(117);

    /// <summary>
    /// 
    /// </summary>
    protected static IBaseTrait IncreasedActionDamage2 { get; } = new BaseTrait(119);

    /// <summary>
    /// 
    /// </summary>
    protected static IBaseTrait SplitShotMastery    { get; } = new BaseTrait(288);

    /// <summary>
    /// 
    /// </summary>
    protected static IBaseTrait SlugShotMastery    { get; } = new BaseTrait(289);

    /// <summary>
    /// 
    /// </summary>
    protected static IBaseTrait CleanShotMastery    { get; } = new BaseTrait(290);

    /// <summary>
    /// 
    /// </summary>
    protected static IBaseTrait HotShotMastery    { get; } = new BaseTrait(291);

    /// <summary>
    /// 
    /// </summary>
    protected static IBaseTrait ChargedActionMastery    { get; } = new BaseTrait(292);

    /// <summary>
    /// 
    /// </summary>
    protected static IBaseTrait EnhancedWildfire    { get; } = new BaseTrait(293);

    /// <summary>
    /// 
    /// </summary>
    protected static IBaseTrait Promotion    { get; } = new BaseTrait(294);

    /// <summary>
    /// 
    /// </summary>
    protected static IBaseTrait SpreadShotMastery    { get; } = new BaseTrait(449);

    /// <summary>
    /// 
    /// </summary>
    protected static IBaseTrait EnhancedReassemble    { get; } = new BaseTrait(450);

    /// <summary>
    /// 
    /// </summary>
    protected static IBaseTrait QueensGambit    { get; } = new BaseTrait(451);

    /// <summary>
    /// 
    /// </summary>
    protected static IBaseTrait EnhancedTactician    { get; } = new BaseTrait(452);

    /// <summary>
    /// 
    /// </summary>
    protected static IBaseTrait MarksmansMastery    { get; } = new BaseTrait(517);
    #endregion

    /// <summary>
    /// 
    /// </summary>
    /// <param name="act"></param>
    /// <returns></returns>
    [RotationDesc(ActionID.Tactician, ActionID.Dismantle)]
    protected sealed override bool DefenseAreaAbility(out IAction act)
    {
        if (Tactician.CanUse(out act, CanUseOption.MustUse)) return true;
        if (Dismantle.CanUse(out act, CanUseOption.MustUse)) return true;
        return false;
    }
}
