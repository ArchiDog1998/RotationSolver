namespace RotationSolver.Basic.Rotations.Basic;

public abstract class PLD_Base : CustomRotation
{
    private static PLDGauge JobGauge => Service.JobGauges.Get<PLDGauge>();
    public override MedicineType MedicineType => MedicineType.Strength;

    protected static bool HasDivineMight => !Player.WillStatusEndGCD(0, 0, true, StatusID.DivineMight);

    protected static bool HasFightOrFlight => !Player.WillStatusEndGCD(0, 0, true, StatusID.FightOrFlight);

    protected static byte OathGauge => JobGauge.OathGauge;

    public sealed override ClassJobID[] JobIDs => new ClassJobID[] { ClassJobID.Paladin, ClassJobID.Gladiator };

    private sealed protected override IBaseAction TankStance => IronWill;

    protected override bool CanHealSingleSpell => DataCenter.PartyMembers.Count() == 1 && base.CanHealSingleSpell;
    protected override bool CanHealAreaAbility => false;

    public static IBaseAction IronWill { get; } = new BaseAction(ActionID.IronWill, ActionOption.Friendly);

    public static IBaseAction FastBlade { get; } = new BaseAction(ActionID.FastBlade);

    public static IBaseAction RiotBlade { get; } = new BaseAction(ActionID.RiotBlade);

    public static IBaseAction GoringBlade { get; } = new BaseAction(ActionID.GoringBlade);

    public static IBaseAction RageOfHalone { get; } = new BaseAction(ActionID.RageOfHalone);

    public static IBaseAction ShieldLob { get; } = new BaseAction(ActionID.ShieldLob)
    {
        FilterForHostiles = TargetFilter.TankRangeTarget,
        ActionCheck = b => !IsLastAction(IActionHelper.MovingActions),
    };

    public static IBaseAction FightOrFlight { get; } = new BaseAction(ActionID.FightOrFlight, ActionOption.Buff);

    public static IBaseAction TotalEclipse { get; } = new BaseAction(ActionID.TotalEclipse);

    public static IBaseAction Prominence { get; } = new BaseAction(ActionID.Prominence);

    public static IBaseAction Sentinel { get; } = new BaseAction(ActionID.Sentinel, ActionOption.Defense)
    {
        StatusProvide = Rampart.StatusProvide,
        ActionCheck = BaseAction.TankDefenseSelf,
    };

    public static IBaseAction CircleOfScorn { get; } = new BaseAction(ActionID.CircleOfScorn);

    public static IBaseAction SpiritsWithin { get; } = new BaseAction(ActionID.SpiritsWithin);

    public static IBaseAction HallowedGround { get; } = new BaseAction(ActionID.HallowedGround, ActionOption.Defense);

    public static IBaseAction DivineVeil { get; } = new BaseAction(ActionID.DivineVeil, ActionOption.Defense);

    public static IBaseAction Clemency { get; } = new BaseAction(ActionID.Clemency, ActionOption.Defense | ActionOption.EndSpecial);

    public static IBaseAction Intervene { get; } = new BaseAction(ActionID.Intervene, ActionOption.EndSpecial)
    {
        ChoiceTarget = TargetFilter.FindTargetForMoving,
    };

    public static IBaseAction Atonement { get; } = new BaseAction(ActionID.Atonement)
    {
        StatusNeed = new[] { StatusID.SwordOath },
    };

    public static IBaseAction Expiacion { get; } = new BaseAction(ActionID.Expiacion);

    public static IBaseAction Requiescat { get; } = new BaseAction(ActionID.Requiescat, ActionOption.Buff);

    public static IBaseAction Confiteor { get; } = new BaseAction(ActionID.Confiteor);

    public static IBaseAction HolyCircle { get; } = new BaseAction(ActionID.HolyCircle);

    public static IBaseAction HolySpirit { get; } = new BaseAction(ActionID.HolySpirit);

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

    protected override bool EmergencyAbility(byte abilitiesRemaining, IAction nextGCD, out IAction act)
    {
        if (HallowedGround.CanUse(out act) && BaseAction.TankBreakOtherCheck(JobIDs[0])) return true;
        return base.EmergencyAbility(abilitiesRemaining, nextGCD, out act);
    }

    [RotationDesc(ActionID.Intervene)]
    protected sealed override bool MoveForwardAbility(byte abilitiesRemaining, out IAction act, CanUseOption option = CanUseOption.None)
    {
        if (Intervene.CanUse(out act, CanUseOption.EmptyOrSkipCombo | option)) return true;
        return false;
    }

    [RotationDesc(ActionID.Clemency)]
    protected sealed override bool HealSingleGCD(out IAction act)
    {
        if (Clemency.CanUse(out act)) return true;
        return false;
    }
}