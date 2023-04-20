namespace RotationSolver.Basic.Rotations.Basic;
public abstract class DRK_Base : CustomRotation
{
    private static DRKGauge JobGauge => Service.JobGauges.Get<DRKGauge>();

    public override MedicineType MedicineType => MedicineType.Strength;

    private static float DarkSideTimeRemaining => JobGauge.DarksideTimeRemaining / 1000f;

    protected static byte Blood => JobGauge.Blood;

    protected static bool HasDarkArts => JobGauge.HasDarkArts;

    protected static bool DarkSideEndAfter(float time)
    {
        return EndAfter(DarkSideTimeRemaining, time);
    }

    protected static bool DarkSideEndAfterGCD(uint gctCount = 0, int abilityCount = 0)
    {
        return EndAfterGCD(DarkSideTimeRemaining, gctCount, abilityCount);
    }

    public sealed override ClassJobID[] JobIDs => new ClassJobID[] { ClassJobID.DarkKnight };

    private sealed protected override IBaseAction TankStance => Grit;

    public static IBaseAction HardSlash { get; } = new BaseAction(ActionID.HardSlash);

    public static IBaseAction SyphonStrike { get; } = new BaseAction(ActionID.SyphonStrike);

    public static IBaseAction Unleash { get; } = new BaseAction(ActionID.Unleash);

    public static IBaseAction Grit { get; } = new BaseAction(ActionID.Grit, ActionOption.EndSpecial);

    public static IBaseAction Unmend { get; } = new BaseAction(ActionID.Unmend)
    {
        FilterForHostiles = TargetFilter.TankRangeTarget,
        ActionCheck = b => !IsLastAction(IActionHelper.MovingActions),
    };

    public static IBaseAction Souleater { get; } = new BaseAction(ActionID.Souleater);

    public static IBaseAction FloodOfDarkness { get; } = new BaseAction(ActionID.FloodOfDarkness);

    public static IBaseAction EdgeOfDarkness { get; } = new BaseAction(ActionID.EdgeOfDarkness);

    public static IBaseAction BloodWeapon { get; } = new BaseAction(ActionID.BloodWeapon);

    public static IBaseAction ShadowWall { get; } = new BaseAction(ActionID.ShadowWall, ActionOption.Defense)
    {
        StatusProvide = Rampart.StatusProvide,
        ActionCheck = BaseAction.TankDefenseSelf,
    };

    public static IBaseAction DarkMind { get; } = new BaseAction(ActionID.DarkMind, ActionOption.Defense)
    {
        ActionCheck = BaseAction.TankDefenseSelf,
    };

    public static IBaseAction LivingDead { get; } = new BaseAction(ActionID.LivingDead, ActionOption.Defense);

    public static IBaseAction SaltedEarth { get; } = new BaseAction(ActionID.SaltedEarth);

    public static IBaseAction Plunge { get; } = new BaseAction(ActionID.Plunge, ActionOption.EndSpecial)
    {
        ChoiceTarget = TargetFilter.FindTargetForMoving
    };

    public static IBaseAction AbyssalDrain { get; } = new BaseAction(ActionID.AbyssalDrain);

    public static IBaseAction CarveandSpit { get; } = new BaseAction(ActionID.CarveandSpit);

    public static IBaseAction BloodSpiller { get; } = new BaseAction(ActionID.BloodSpiller)
    {
        ActionCheck = b => JobGauge.Blood >= 50 || Player.HasStatus(true, StatusID.Delirium),
    };

    public static IBaseAction Quietus { get; } = new BaseAction(ActionID.Quietus)
    {
        ActionCheck = BloodSpiller.ActionCheck,
    };

    public static IBaseAction Delirium { get; } = new BaseAction(ActionID.Delirium);

    public static IBaseAction TheBlackestNight { get; } = new BaseAction(ActionID.TheBlackestNight, ActionOption.Defense)
    {
        ChoiceTarget = TargetFilter.FindAttackedTarget,
    };

    public static IBaseAction StalwartSoul { get; } = new BaseAction(ActionID.StalwartSoul);

    public static IBaseAction DarkMissionary { get; } = new BaseAction(ActionID.DarkMissionary, ActionOption.Defense);

    public static IBaseAction LivingShadow { get; } = new BaseAction(ActionID.LivingShadow)
    {
        ActionCheck = b => JobGauge.Blood >= 50,
    };

    public static IBaseAction Oblation { get; } = new BaseAction(ActionID.Oblation, ActionOption.Defense)
    {
        ActionCheck = b => !b.HasStatus(true, StatusID.Oblation),
        ChoiceTarget = TargetFilter.FindAttackedTarget,
    };

    public static IBaseAction ShadowBringer { get; } = new BaseAction(ActionID.ShadowBringer)
    {
        ActionCheck = b => JobGauge.DarksideTimeRemaining > 0,
    };

    public static IBaseAction SaltandDarkness { get; } = new BaseAction(ActionID.SaltandDarkness)
    {
        StatusNeed = new[] { StatusID.SaltedEarth },
    };

    protected override bool EmergencyAbility(float nextAbilityToNextGCD, IAction nextGCD, out IAction act)
    {
        if (LivingDead.CanUse(out act) && BaseAction.TankBreakOtherCheck(JobIDs[0])) return true;
        return base.EmergencyAbility(nextAbilityToNextGCD, nextGCD, out act);
    }

    [RotationDesc(ActionID.Plunge)]
    protected sealed override bool MoveForwardAbility(float nextAbilityToNextGCD, out IAction act, CanUseOption option = CanUseOption.None)
    {
        if (Plunge.CanUse(out act, CanUseOption.EmptyOrSkipCombo | option)) return true;

        return false;
    }
}