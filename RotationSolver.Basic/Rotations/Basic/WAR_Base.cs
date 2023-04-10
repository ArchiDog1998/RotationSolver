namespace RotationSolver.Basic.Rotations.Basic;

public abstract class WAR_Base : CustomRotation
{
    private static WARGauge JobGauge => Service.JobGauges.Get<WARGauge>();
    public override MedicineType MedicineType => MedicineType.Strength;

    protected static byte BeastGauge => JobGauge.BeastGauge;
    public sealed override ClassJobID[] JobIDs => new ClassJobID[] { ClassJobID.Warrior, ClassJobID.Marauder };
    private sealed protected override IBaseAction TankStance => Defiance;

    public static IBaseAction Defiance { get; } = new BaseAction(ActionID.Defiance, true, shouldEndSpecial: true);

    public static IBaseAction HeavySwing { get; } = new BaseAction(ActionID.HeavySwing);

    public static IBaseAction Maim { get; } = new BaseAction(ActionID.Maim);

    public static IBaseAction StormsPath { get; } = new BaseAction(ActionID.StormsPath);

    public static IBaseAction StormsEye { get; } = new BaseAction(ActionID.StormsEye)
    {
        ActionCheck = b => Player.WillStatusEndGCD(9, 0, true, StatusID.SurgingTempest),
    };

    public static IBaseAction Tomahawk { get; } = new BaseAction(ActionID.Tomahawk)
    {
        FilterForHostiles = TargetFilter.TankRangeTarget,
        ActionCheck = b => !IsLastAction(IActionHelper.MovingActions),
    };

    public static IBaseAction Onslaught { get; } = new BaseAction(ActionID.Onslaught, shouldEndSpecial: true)
    {
        ChoiceTarget = TargetFilter.FindTargetForMoving,
    };

    public static IBaseAction Upheaval { get; } = new BaseAction(ActionID.Upheaval)
    {
        StatusNeed = new StatusID[] { StatusID.SurgingTempest },
    };

    public static IBaseAction Overpower { get; } = new BaseAction(ActionID.Overpower);

    public static IBaseAction MythrilTempest { get; } = new BaseAction(ActionID.MythrilTempest);

    public static IBaseAction Orogeny { get; } = new BaseAction(ActionID.Orogeny);

    public static IBaseAction InnerBeast { get; } = new BaseAction(ActionID.InnerBeast)
    {
        ActionCheck = b => JobGauge.BeastGauge >= 50 || Player.HasStatus(true, StatusID.InnerRelease),
    };

    public static IBaseAction SteelCyclone { get; } = new BaseAction(ActionID.SteelCyclone)
    {
        ActionCheck = InnerBeast.ActionCheck,
    };

    public static IBaseAction Infuriate { get; } = new BaseAction(ActionID.Infuriate)
    {
        StatusProvide = new[] { StatusID.NascentChaos },
        ActionCheck = b => HasHostilesInRange && JobGauge.BeastGauge <= 50 && InCombat,
    };

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static IBaseAction InnerRelease { get; } = new BaseAction(ActionID.InnerRelease);

    public static IBaseAction Berserk { get; } = new BaseAction(ActionID.Berserk)
    {
        ActionCheck = b => HasHostilesInRange && !InnerRelease.IsCoolingDown,
    };

    public static IBaseAction ThrillOfBattle { get; } = new BaseAction(ActionID.ThrillOfBattle, true, isTimeline: true);

    public static IBaseAction Equilibrium { get; } = new BaseAction(ActionID.Equilibrium, true, isTimeline: true);



    #region Defense Ability
    public static IBaseAction Vengeance { get; } = new BaseAction(ActionID.Vengeance, true, isTimeline: true)
    {
        StatusProvide = Rampart.StatusProvide,
        ActionCheck = BaseAction.TankDefenseSelf,
    };

    public static IBaseAction RawIntuition { get; } = new BaseAction(ActionID.RawIntuition, true, isTimeline: true)
    {
        ActionCheck = BaseAction.TankDefenseSelf,
    };

    public static IBaseAction NascentFlash { get; } = new BaseAction(ActionID.NascentFlash, true, isTimeline: true)
    {
        ChoiceTarget = TargetFilter.FindAttackedTarget,
    };

    public static IBaseAction ShakeItOff { get; } = new BaseAction(ActionID.ShakeItOff, true, isTimeline: true);

    public static IBaseAction Holmgang { get; } = new BaseAction(ActionID.Holmgang, true, isTimeline: true)
    {
        ChoiceTarget = (tars, mustUse) => Player,
    };
    #endregion


    public static IBaseAction PrimalRend { get; } = new BaseAction(ActionID.PrimalRend)
    {
        StatusNeed = new[] { StatusID.PrimalRendReady }
    };

    protected override bool EmergencyAbility(byte abilitiesRemaining, IAction nextGCD, out IAction act)
    {
        if (Holmgang.CanUse(out act) && BaseAction.TankBreakOtherCheck(JobIDs[0])) return true;
        return base.EmergencyAbility(abilitiesRemaining, nextGCD, out act);
    }

    [RotationDesc(ActionID.Onslaught)]
    protected sealed override bool MoveForwardAbility(byte abilitiesRemaining, out IAction act, CanUseOption option = CanUseOption.None)
    {
        if (Onslaught.CanUse(out act, CanUseOption.EmptyOrSkipCombo | option)) return true;
        return false;
    }

    [RotationDesc(ActionID.PrimalRend)]
    protected sealed override bool MoveForwardGCD(out IAction act)
    {
        if (PrimalRend.CanUse(out act, CanUseOption.MustUse)) return true;
        return false;
    }
}
