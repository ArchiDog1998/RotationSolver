namespace RotationSolver.Basic.Rotations.Basic;

public abstract class GNB_Base : CustomRotation
{
    private static GNBGauge JobGauge => Service.JobGauges.Get<GNBGauge>();

    public override MedicineType MedicineType => MedicineType.Strength;

    protected static byte Ammo => JobGauge.Ammo;

    protected static byte AmmoComboStep => JobGauge.AmmoComboStep;

    public sealed override ClassJobID[] JobIDs => new ClassJobID[] { ClassJobID.Gunbreaker };
    private sealed protected override IBaseAction TankStance => RoyalGuard;

    protected override bool CanHealSingleSpell => false;
    protected override bool CanHealAreaSpell => false;

    protected static byte MaxAmmo => Level >= 88 ? (byte)3 : (byte)2;

    public static IBaseAction RoyalGuard { get; } = new BaseAction(ActionID.RoyalGuard, ActionOption.EndSpecial);

    public static IBaseAction KeenEdge { get; } = new BaseAction(ActionID.KeenEdge);

    public static IBaseAction NoMercy { get; } = new BaseAction(ActionID.NoMercy);

    public static IBaseAction BrutalShell { get; } = new BaseAction(ActionID.BrutalShell);

    public static IBaseAction Camouflage { get; } = new BaseAction(ActionID.Camouflage, ActionOption.Defense)
    {
        ActionCheck = BaseAction.TankDefenseSelf,
    };

    public static IBaseAction DemonSlice { get; } = new BaseAction(ActionID.DemonSlice)
    {
        AOECount = 2,
    };

    public static IBaseAction LightningShot { get; } = new BaseAction(ActionID.LightningShot)
    {
        FilterForHostiles = TargetFilter.TankRangeTarget,
        ActionCheck = b => !IsLastAction(IActionHelper.MovingActions),
    };

    public static IBaseAction DangerZone { get; } = new BaseAction(ActionID.DangerZone);

    public static IBaseAction SolidBarrel { get; } = new BaseAction(ActionID.SolidBarrel);

    public static IBaseAction BurstStrike { get; } = new BaseAction(ActionID.BurstStrike)
    {
        ActionCheck = b => JobGauge.Ammo > 0,
    };

    public static IBaseAction Nebula { get; } = new BaseAction(ActionID.Nebula, ActionOption.Defense)
    {
        StatusProvide = Rampart.StatusProvide,
        ActionCheck = BaseAction.TankDefenseSelf,
    };

    public static IBaseAction DemonSlaughter { get; } = new BaseAction(ActionID.DemonSlaughter)
    {
        AOECount = 2,
    };

    public static IBaseAction Aurora { get; } = new BaseAction(ActionID.Aurora, ActionOption.Heal)
    {
        ActionCheck = b => !b.HasStatus(true, StatusID.Aurora),
    };

    public static IBaseAction SuperBolide { get; } = new BaseAction(ActionID.SuperBolide, ActionOption.Defense);

    public static IBaseAction SonicBreak { get; } = new BaseAction(ActionID.SonicBreak);

    public static IBaseAction RoughDivide { get; } = new BaseAction(ActionID.RoughDivide, ActionOption.EndSpecial)
    {
        ChoiceTarget = TargetFilter.FindTargetForMoving,
    };

    public static IBaseAction GnashingFang { get; } = new BaseAction(ActionID.GnashingFang)
    {
        ActionCheck = b => JobGauge.AmmoComboStep == 0 && JobGauge.Ammo > 0,
    };

    public static IBaseAction BowShock { get; } = new BaseAction(ActionID.BowShock);

    public static IBaseAction HeartOfLight { get; } = new BaseAction(ActionID.HeartOfLight, ActionOption.Heal);

    public static IBaseAction HeartOfStone { get; } = new BaseAction(ActionID.HeartOfStone, ActionOption.Defense)
    {
        ChoiceTarget = TargetFilter.FindAttackedTarget,
    };

    public static IBaseAction FatedCircle { get; } = new BaseAction(ActionID.FatedCircle)
    {
        ActionCheck = b => JobGauge.Ammo > 0,
    };

    public static IBaseAction BloodFest { get; } = new BaseAction(ActionID.BloodFest, ActionOption.Buff)
    {
        ActionCheck = b => MaxAmmo - JobGauge.Ammo > 1,
    };

    public static IBaseAction DoubleDown { get; } = new BaseAction(ActionID.DoubleDown)
    {
        ActionCheck = b => JobGauge.Ammo > 1,
    };

    public static IBaseAction SavageClaw { get; } = new BaseAction(ActionID.SavageClaw)
    {
        ActionCheck = b => Service.GetAdjustedActionId(ActionID.GnashingFang) == ActionID.SavageClaw,
    };

    public static IBaseAction WickedTalon { get; } = new BaseAction(ActionID.WickedTalon)
    {
        ActionCheck = b => Service.GetAdjustedActionId(ActionID.GnashingFang) == ActionID.WickedTalon,
    };

    public static IBaseAction JugularRip { get; } = new BaseAction(ActionID.JugularRip)
    {
        ActionCheck = b => Service.GetAdjustedActionId(ActionID.Continuation) == ActionID.JugularRip,
    };

    public static IBaseAction AbdomenTear { get; } = new BaseAction(ActionID.AbdomenTear)
    {
        ActionCheck = b => Service.GetAdjustedActionId(ActionID.Continuation) == ActionID.AbdomenTear,
    };

    public static IBaseAction EyeGouge { get; } = new BaseAction(ActionID.EyeGouge)
    {
        ActionCheck = b => Service.GetAdjustedActionId(ActionID.Continuation) == ActionID.EyeGouge,
    };

    public static IBaseAction Hypervelocity { get; } = new BaseAction(ActionID.Hypervelocity)
    {
        ActionCheck = b => Service.GetAdjustedActionId(ActionID.Continuation)
        == ActionID.Hypervelocity,
    };

    protected override bool EmergencyAbility(byte abilitiesRemaining, IAction nextGCD, out IAction act)
    {
        if (SuperBolide.CanUse(out act) && BaseAction.TankBreakOtherCheck(JobIDs[0])) return true;
        return base.EmergencyAbility(abilitiesRemaining, nextGCD, out act);
    }

    [RotationDesc(ActionID.RoughDivide)]
    protected sealed override bool MoveForwardAbility(byte abilitiesRemaining, out IAction act, CanUseOption option = CanUseOption.None)
    {
        if (RoughDivide.CanUse(out act, CanUseOption.EmptyOrSkipCombo | option)) return true;
        return false;
    }
}

