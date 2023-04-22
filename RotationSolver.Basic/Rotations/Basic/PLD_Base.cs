namespace RotationSolver.Basic.Rotations.Basic;

public abstract class PLD_Base : CustomRotation
{
    public override MedicineType MedicineType => MedicineType.Strength;

    public sealed override ClassJobID[] JobIDs => new ClassJobID[] { ClassJobID.Paladin, ClassJobID.Gladiator };

    protected override bool CanHealSingleSpell => DataCenter.PartyMembers.Count() == 1 && base.CanHealSingleSpell;

    protected override bool CanHealAreaAbility => false;

    protected static bool HasDivineMight => !Player.WillStatusEndGCD(0, 0, true, StatusID.DivineMight);

    protected static bool HasFightOrFlight => !Player.WillStatusEndGCD(0, 0, true, StatusID.FightOrFlight);

    #region Job Gauge
    private static PLDGauge JobGauge => Service.JobGauges.Get<PLDGauge>();

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

    public static IBaseAction GoringBlade { get; } = new BaseAction(ActionID.GoringBlade);

    public static IBaseAction Atonement { get; } = new BaseAction(ActionID.Atonement)
    {
        StatusNeed = new[] { StatusID.SwordOath },
    };

    public static IBaseAction ShieldBash { get; } = new BaseAction(ActionID.ShieldBash, ActionOption.Timeline)
    {
        FilterForHostiles = LowBlow.FilterForHostiles,
        ActionCheck = b => LowBlow.IsCoolingDown,
        StatusProvide = new StatusID[] { StatusID.Stun },
    };

    public static IBaseAction ShieldLob { get; } = new BaseAction(ActionID.ShieldLob)
    {
        FilterForHostiles = TargetFilter.TankRangeTarget,
        ActionCheck = b => !IsLastAction(IActionHelper.MovingActions),
    };

    public static IBaseAction HolySpirit { get; } = new BaseAction(ActionID.HolySpirit);

    public static IBaseAction Intervene { get; } = new BaseAction(ActionID.Intervene, ActionOption.EndSpecial)
    {
        ChoiceTarget = TargetFilter.FindTargetForMoving,
    };

    public static IBaseAction SpiritsWithin { get; } = new BaseAction(ActionID.SpiritsWithin);

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

    public static IBaseAction CircleOfScorn { get; } = new BaseAction(ActionID.CircleOfScorn);

    public static IBaseAction HolyCircle { get; } = new BaseAction(ActionID.HolyCircle);

    public static IBaseAction Confiteor { get; } = new BaseAction(ActionID.Confiteor);

    #endregion

    #region Heal
    private sealed protected override IBaseAction TankStance => IronWill;
    public static IBaseAction IronWill { get; } = new BaseAction(ActionID.IronWill, ActionOption.Friendly);
    #endregion

    #region Support
    public static IBaseAction Requiescat { get; } = new BaseAction(ActionID.Requiescat, ActionOption.Buff);

    public static IBaseAction FightOrFlight { get; } = new BaseAction(ActionID.FightOrFlight, ActionOption.Buff);

    public static IBaseAction HallowedGround { get; } = new BaseAction(ActionID.HallowedGround, ActionOption.Defense);

    public static IBaseAction DivineVeil { get; } = new BaseAction(ActionID.DivineVeil, ActionOption.Defense);

    public static IBaseAction Sentinel { get; } = new BaseAction(ActionID.Sentinel, ActionOption.Defense)
    {
        StatusProvide = Rampart.StatusProvide,
        ActionCheck = BaseAction.TankDefenseSelf,
    };

    public static IBaseAction PassageOfArms { get; } = new BaseAction(ActionID.PassageOfArms, ActionOption.Defense);


    public static IBaseAction Cover { get; } = new BaseAction(ActionID.Cover, ActionOption.Defense)
    {
        ChoiceTarget = TargetFilter.FindAttackedTarget,
        ActionCheck = b => OathGauge >= 50,
    };

    public static IBaseAction Intervention { get; } = new BaseAction(ActionID.Intervention, ActionOption.Defense)
    {
        ActionCheck = Cover.ActionCheck,
        ChoiceTarget = TargetFilter.FindAttackedTarget,
    };

    public static IBaseAction Sheltron { get; } = new BaseAction(ActionID.Sheltron, ActionOption.Defense)
    {
        ActionCheck = b => BaseAction.TankDefenseSelf(b) && Cover.ActionCheck(b),
    };

    public static IBaseAction Bulwark { get; } = new BaseAction(ActionID.Bulwark, ActionOption.Defense)
    {
        StatusProvide = Rampart.StatusProvide,
        ActionCheck = BaseAction.TankDefenseSelf,
    };
    public static IBaseAction Clemency { get; } = new BaseAction(ActionID.Clemency, ActionOption.Defense | ActionOption.EndSpecial);
    #endregion

    protected override bool EmergencyAbility(IAction nextGCD, out IAction act)
    {
        if (HallowedGround.CanUse(out act) && BaseAction.TankBreakOtherCheck(JobIDs[0])) return true;
        return base.EmergencyAbility(nextGCD, out act);
    }

    [RotationDesc(ActionID.Intervene)]
    protected sealed override bool MoveForwardAbility(out IAction act)
    {
        if (Intervene.CanUse(out act)) return true;
        return base.MoveForwardAbility(out act);
    }

    [RotationDesc(ActionID.Clemency)]
    protected sealed override bool HealSingleGCD(out IAction act)
    {
        if (Clemency.CanUse(out act)) return true;
        return base.HealSingleGCD(out act);
    }
}