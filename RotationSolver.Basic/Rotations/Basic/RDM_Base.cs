using ECommons.DalamudServices;
using ECommons.ExcelServices;
using RotationSolver.Basic.Traits;

namespace RotationSolver.Basic.Rotations.Basic;

/// <summary>
/// The base class of RDM.
/// </summary>
public abstract class RDM_Base : CustomRotation
{
    /// <summary>
    /// 
    /// </summary>
    public override MedicineType MedicineType => MedicineType.Intelligence;

    /// <summary>
    /// 
    /// </summary>
    public sealed override Job[] Jobs => new [] { Job.RDM };

    /// <summary>
    /// 
    /// </summary>
    public override bool CanHealSingleSpell => DataCenter.PartyMembers.Count() == 1 && base.CanHealSingleSpell;

    #region Job Gauge
    static RDMGauge JobGauge => Svc.Gauges.Get<RDMGauge>();

    /// <summary>
    /// 
    /// </summary>
    public static byte WhiteMana => JobGauge.WhiteMana;

    /// <summary>
    /// 
    /// </summary>
    public static byte BlackMana => JobGauge.BlackMana;

    /// <summary>
    /// 
    /// </summary>
    public static byte ManaStacks => JobGauge.ManaStacks;
    #endregion

    #region Attack Single
    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction Jolt { get; } = new BaseAction(ActionID.Jolt)
    {
        StatusProvide = Swiftcast.StatusProvide.Union(new[] { StatusID.Acceleration }).ToArray(),
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction Verfire { get; } = new BaseAction(ActionID.Verfire)
    {
        StatusNeed = new[] { StatusID.VerfireReady },
        StatusProvide = Jolt.StatusProvide,
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction Verstone { get; } = new BaseAction(ActionID.Verstone)
    {
        StatusNeed = new[] { StatusID.VerstoneReady },
        StatusProvide = Jolt.StatusProvide,
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction Verthunder { get; } = new BaseAction(ActionID.Verthunder)
    {
        StatusNeed = Jolt.StatusProvide,
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction Veraero { get; } = new BaseAction(ActionID.Veraero)
    {
        StatusNeed = Jolt.StatusProvide,
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction Riposte { get; } = new BaseAction(ActionID.Riposte)
    {
        ActionCheck = (b, m) => JobGauge.BlackMana >= 20 && JobGauge.WhiteMana >= 20,
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction Zwerchhau { get; } = new BaseAction(ActionID.Zwerchhau)
    {
        ActionCheck = (b, m) => BlackMana >= 15 && WhiteMana >= 15,
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction Redoublement { get; } = new BaseAction(ActionID.Redoublement)
    {
        ActionCheck = (b, m) => BlackMana >= 15 && WhiteMana >= 15,
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction Engagement { get; } = new BaseAction(ActionID.Engagement);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction Fleche { get; } = new BaseAction(ActionID.Fleche);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction CorpsACorps { get; } = new BaseAction(ActionID.CorpsACorps, ActionOption.EndSpecial)
    {
        ChoiceTarget = TargetFilter.FindTargetForMoving,
    };
    #endregion

    #region Attack Area
    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction Scatter { get; } = new BaseAction(ActionID.Scatter)
    {
        StatusNeed = Jolt.StatusProvide,
        AOECount = 2,
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction Verthunder2 { get; } = new BaseAction(ActionID.Verthunder2)
    {
        StatusProvide = Jolt.StatusProvide,
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction Veraero2 { get; } = new BaseAction(ActionID.Veraero2)
    {
        StatusProvide = Jolt.StatusProvide,
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction Moulinet { get; } = new BaseAction(ActionID.Moulinet)
    {
        ActionCheck = (b, m) => BlackMana >= 20 && WhiteMana >= 20,
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction Verflare { get; } = new BaseAction(ActionID.Verflare);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction Verholy { get; } = new BaseAction(ActionID.Verholy);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction Scorch { get; } = new BaseAction(ActionID.Scorch)
    {
        ComboIds = new[] { ActionID.Verholy },
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction Resolution { get; } = new BaseAction(ActionID.Resolution);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction ContreSixte { get; } = new BaseAction(ActionID.ContreSixte);
    #endregion

    #region Support
    private protected sealed override IBaseAction Raise => Verraise;

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction Verraise { get; } = new BaseAction(ActionID.Verraise, ActionOption.Friendly);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction Acceleration { get; } = new BaseAction(ActionID.Acceleration, ActionOption.Buff)
    {
        StatusProvide = new[] { StatusID.Acceleration },
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction Vercure { get; } = new BaseAction(ActionID.Vercure, ActionOption.Heal)
    {
        StatusProvide = Swiftcast.StatusProvide.Union(Acceleration.StatusProvide).ToArray(),
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction MagickBarrier { get; } = new BaseAction(ActionID.MagickBarrier, ActionOption.Defense);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction Embolden { get; } = new BaseAction(ActionID.Embolden, ActionOption.Buff);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction Manafication { get; } = new BaseAction(ActionID.Manafication)
    {
        ActionCheck = (b, m) => WhiteMana <= 50 && BlackMana <= 50 && InCombat && ManaStacks == 0,
        ComboIdsNot = new[] { ActionID.Riposte, ActionID.Zwerchhau, ActionID.Scorch, ActionID.Verflare, ActionID.Verholy },
    };
    #endregion

    #region Traits
    /// <summary>
    /// 
    /// </summary>
    protected static IBaseTrait EnhancedJolt { get; } = new BaseTrait(195);

    /// <summary>
    /// 
    /// </summary>
    protected static IBaseTrait MaimAndMend    { get; } = new BaseTrait(200);

    /// <summary>
    /// 
    /// </summary>
    protected static IBaseTrait MaimAndMend2    { get; } = new BaseTrait(201);

    /// <summary>
    /// 
    /// </summary>
    protected static IBaseTrait Dualcast    { get; } = new BaseTrait(216);

    /// <summary>
    /// 
    /// </summary>
    protected static IBaseTrait ScatterMastery    { get; } = new BaseTrait(303);

    /// <summary>
    /// 
    /// </summary>
    protected static IBaseTrait EnhancedDisplacement    { get; } = new BaseTrait(304);

    /// <summary>
    /// 
    /// </summary>
    protected static IBaseTrait EnhancedManafication    { get; } = new BaseTrait(305);

    /// <summary>
    /// 
    /// </summary>
    protected static IBaseTrait RedMagicMastery    { get; } = new BaseTrait(306);

    /// <summary>
    /// 
    /// </summary>
    protected static IBaseTrait ManaStack    { get; } = new BaseTrait(482);

    /// <summary>
    /// 
    /// </summary>
    protected static IBaseTrait RedMagicMastery2    { get; } = new BaseTrait(483);

    /// <summary>
    /// 
    /// </summary>
    protected static IBaseTrait RedMagicMastery3    { get; } = new BaseTrait(484);

    /// <summary>
    /// 
    /// </summary>
    protected static IBaseTrait EnhancedAcceleration    { get; } = new BaseTrait(485);

    /// <summary>
    /// 
    /// </summary>
    protected static IBaseTrait EnhancedManafication2    { get; } = new BaseTrait(486);
    #endregion

    /// <summary>
    /// 
    /// </summary>
    /// <param name="act"></param>
    /// <returns></returns>
    [RotationDesc(ActionID.Vercure)]
    protected sealed override bool HealSingleGCD(out IAction act)
    {
        if (Vercure.CanUse(out act, CanUseOption.MustUse)) return true;
        return base.HealSingleGCD(out act);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="act"></param>
    /// <returns></returns>
    [RotationDesc(ActionID.CorpsACorps)]
    protected sealed override bool MoveForwardAbility(out IAction act)
    {
        if (CorpsACorps.CanUse(out act)) return true;
        return base.MoveForwardAbility(out act);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="act"></param>
    /// <returns></returns>
    [RotationDesc(ActionID.Addle, ActionID.MagickBarrier)]
    protected sealed override bool DefenseAreaAbility(out IAction act)
    {
        if (Addle.CanUse(out act)) return true;
        if (MagickBarrier.CanUse(out act, CanUseOption.MustUse)) return true;
        return base.DefenseAreaAbility(out act);
    }
}