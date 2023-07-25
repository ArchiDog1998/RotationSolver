using ECommons.DalamudServices;
using ECommons.ExcelServices;
using RotationSolver.Basic.Traits;

namespace RotationSolver.Basic.Rotations.Basic;

/// <summary>
/// The base class of PLD.
/// </summary>
public abstract class PLD_Base : CustomRotation
{
    /// <summary>
    /// 
    /// </summary>
    public override MedicineType MedicineType => MedicineType.Strength;

    /// <summary>
    /// 
    /// </summary>
    public sealed override Job[] Jobs => new [] { Job.PLD, Job.GLA };

    /// <summary>
    /// 
    /// </summary>
    protected override bool CanHealSingleSpell => DataCenter.PartyMembers.Count() == 1 && base.CanHealSingleSpell;

    /// <summary>
    /// 
    /// </summary>
    protected override bool CanHealAreaAbility => false;

    /// <summary>
    /// 
    /// </summary>
    protected static bool HasDivineMight => !Player.WillStatusEndGCD(0, 0, true, StatusID.DivineMight);

    /// <summary>
    /// 
    /// </summary>
    protected static bool HasFightOrFlight => !Player.WillStatusEndGCD(0, 0, true, StatusID.FightOrFlight);

    #region Job Gauge
    private static PLDGauge JobGauge => Svc.Gauges.Get<PLDGauge>();

    /// <summary>
    /// 
    /// </summary>
    protected static byte OathGauge => JobGauge.OathGauge;
    #endregion

    #region Attack Single
    /// <summary>
    /// 1
    /// </summary>
    public static IBaseAction FastBlade { get; } = new BaseAction(ActionID.FastBlade);

    /// <summary>
    /// 2
    /// </summary>
    public static IBaseAction RiotBlade { get; } = new BaseAction(ActionID.RiotBlade);

    /// <summary>
    /// 3
    /// </summary>
    public static IBaseAction RageOfHalone { get; } = new BaseAction(ActionID.RageOfHalone);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction GoringBlade { get; } = new BaseAction(ActionID.GoringBlade);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction Atonement { get; } = new BaseAction(ActionID.Atonement)
    {
        StatusNeed = new[] { StatusID.SwordOath },
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction ShieldBash { get; } = new BaseAction(ActionID.ShieldBash)
    {
        FilterForHostiles = LowBlow.FilterForHostiles,
        ActionCheck = (b, m) => LowBlow.IsCoolingDown,
        StatusProvide = new StatusID[] { StatusID.Stun },
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction ShieldLob { get; } = new BaseAction(ActionID.ShieldLob)
    {
        FilterForHostiles = TargetFilter.TankRangeTarget,
        ActionCheck = (b, m) => !IsLastAction(IActionHelper.MovingActions),
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction HolySpirit { get; } = new BaseAction(ActionID.HolySpirit);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction Intervene { get; } = new BaseAction(ActionID.Intervene, ActionOption.EndSpecial)
    {
        ChoiceTarget = TargetFilter.FindTargetForMoving,
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction SpiritsWithin { get; } = new BaseAction(ActionID.SpiritsWithin);

    /// <summary>
    /// 
    /// </summary>
    [Obsolete("Please use SpiritsWithin instead.")]
    public static IBaseAction Expiacion { get; } = new BaseAction(ActionID.Expiacion);
    #endregion

    #region Attack Area
    /// <summary>
    /// 1
    /// </summary>
    public static IBaseAction TotalEclipse { get; } = new BaseAction(ActionID.TotalEclipse);

    /// <summary>
    /// 2
    /// </summary>
    public static IBaseAction Prominence { get; } = new BaseAction(ActionID.Prominence);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction CircleOfScorn { get; } = new BaseAction(ActionID.CircleOfScorn);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction HolyCircle { get; } = new BaseAction(ActionID.HolyCircle);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction Confiteor { get; } = new BaseAction(ActionID.Confiteor);
    #endregion

    #region Heal
    private protected sealed override IBaseAction TankStance => IronWill;

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction IronWill { get; } = new BaseAction(ActionID.IronWill, ActionOption.Friendly);
    #endregion

    #region Support
    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction Requiescat { get; } = new BaseAction(ActionID.Requiescat, ActionOption.Buff);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction FightOrFlight { get; } = new BaseAction(ActionID.FightOrFlight, ActionOption.Buff);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction HallowedGround { get; } = new BaseAction(ActionID.HallowedGround, ActionOption.Defense | ActionOption.EndSpecial);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction DivineVeil { get; } = new BaseAction(ActionID.DivineVeil, ActionOption.Defense);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction Sentinel { get; } = new BaseAction(ActionID.Sentinel, ActionOption.Defense)
    {
        StatusProvide = Rampart.StatusProvide,
        ActionCheck = BaseAction.TankDefenseSelf,
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction PassageOfArms { get; } = new BaseAction(ActionID.PassageOfArms, ActionOption.Defense);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction Cover { get; } = new BaseAction(ActionID.Cover, ActionOption.Defense)
    {
        ChoiceTarget = TargetFilter.FindAttackedTarget,
        ActionCheck = (b, m) => OathGauge >= 50,
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction Intervention { get; } = new BaseAction(ActionID.Intervention, ActionOption.Defense)
    {
        ActionCheck = Cover.ActionCheck,
        ChoiceTarget = TargetFilter.FindAttackedTarget,
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction Sheltron { get; } = new BaseAction(ActionID.Sheltron, ActionOption.Defense)
    {
        ActionCheck = (b, m) => BaseAction.TankDefenseSelf(b, m) && Cover.ActionCheck(b, m),
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction Bulwark { get; } = new BaseAction(ActionID.Bulwark, ActionOption.Defense)
    {
        StatusProvide = Rampart.StatusProvide,
        ActionCheck = BaseAction.TankDefenseSelf,
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction Clemency { get; } = new BaseAction(ActionID.Clemency, ActionOption.Defense | ActionOption.EndSpecial);
    #endregion

    #region Traits
    /// <summary>
    /// 
    /// </summary>
    protected static IBaseTrait DivineMagicMastery    { get; } = new BaseTrait(207);

    /// <summary>
    /// 
    /// </summary>
    protected static IBaseTrait OathMastery    { get; } = new BaseTrait(209);

    /// <summary>
    /// 
    /// </summary>
    protected static IBaseTrait Chivalry    { get; } = new BaseTrait(246);

    /// <summary>
    /// 
    /// </summary>
    protected static IBaseTrait RageOfHaloneMastery    { get; } = new BaseTrait(260);

    /// <summary>
    /// 
    /// </summary>
    protected static IBaseTrait EnhancedProminence    { get; } = new BaseTrait(261);

    /// <summary>
    /// 
    /// </summary>
    protected static IBaseTrait EnhancedSheltron    { get; } = new BaseTrait(262);

    /// <summary>
    /// 
    /// </summary>
    protected static IBaseTrait EnhancedRequiescat    { get; } = new BaseTrait(263);

    /// <summary>
    /// 
    /// </summary>
    protected static IBaseTrait SwordOath    { get; } = new BaseTrait(264);

    /// <summary>
    /// 
    /// </summary>
    protected static IBaseTrait TankMastery    { get; } = new BaseTrait(317);

    /// <summary>
    /// 
    /// </summary>
    protected static IBaseTrait SheltronMastery    { get; } = new BaseTrait(412);

    /// <summary>
    /// 
    /// </summary>
    protected static IBaseTrait EnhancedIntervention    { get; } = new BaseTrait(413);

    /// <summary>
    /// 
    /// </summary>
    protected static IBaseTrait DivineMagicMastery2    { get; } = new BaseTrait(414);

    /// <summary>
    /// 
    /// </summary>
    protected static IBaseTrait SpiritsWithinMastery    { get; } = new BaseTrait(415);

    /// <summary>
    /// 
    /// </summary>
    protected static IBaseTrait EnhancedDivineVeil    { get; } = new BaseTrait(416);

    /// <summary>
    /// 
    /// </summary>
    protected static IBaseTrait MeleeMastery    { get; } = new BaseTrait(504);
    #endregion

    /// <summary>
    /// 
    /// </summary>
    /// <param name="nextGCD"></param>
    /// <param name="act"></param>
    /// <returns></returns>
    protected override bool EmergencyAbility(IAction nextGCD, out IAction act)
    {
        if (HallowedGround.CanUse(out act) && BaseAction.TankBreakOtherCheck(Jobs[0])) return true;
        return base.EmergencyAbility(nextGCD, out act);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="act"></param>
    /// <returns></returns>
    [RotationDesc(ActionID.Intervene)]
    protected sealed override bool MoveForwardAbility(out IAction act)
    {
        if (Intervene.CanUse(out act)) return true;
        return base.MoveForwardAbility(out act);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="act"></param>
    /// <returns></returns>
    [RotationDesc(ActionID.Clemency)]
    protected sealed override bool HealSingleGCD(out IAction act)
    {
        if (Clemency.CanUse(out act)) return true;
        return base.HealSingleGCD(out act);
    }
}