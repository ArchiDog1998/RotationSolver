namespace RotationSolver.Basic.Rotations.Basic;

public abstract class WAR_Base : CustomRotation
{
    public override MedicineType MedicineType => MedicineType.Strength;

    public sealed override ClassJobID[] JobIDs => new ClassJobID[] { ClassJobID.Warrior, ClassJobID.Marauder };

    static WARGauge JobGauge => Service.JobGauges.Get<WARGauge>();

    protected static byte BeastGauge => JobGauge.BeastGauge;

    #region Attack Single
    /// <summary>
    /// 1
    /// </summary>
    public static IBaseAction HeavySwing { get; } = new BaseAction(ActionID.HeavySwing);

    /// <summary>
    /// 2
    /// </summary>
    public static IBaseAction Maim { get; } = new BaseAction(ActionID.Maim);

    /// <summary>
    /// 3
    /// </summary>
    public static IBaseAction StormsPath { get; } = new BaseAction(ActionID.StormsPath);

    /// <summary>
    /// 4
    /// </summary>
    public static IBaseAction StormsEye { get; } = new BaseAction(ActionID.StormsEye)
    {
        ActionCheck = b => Player.WillStatusEndGCD(9, 0, true, StatusID.SurgingTempest),
    };

    public static IBaseAction InnerBeast { get; } = new BaseAction(ActionID.InnerBeast)
    {
        ActionCheck = b => JobGauge.BeastGauge >= 50 || Player.HasStatus(true, StatusID.InnerRelease),
    };

    public static IBaseAction Tomahawk { get; } = new BaseAction(ActionID.Tomahawk)
    {
        FilterForHostiles = TargetFilter.TankRangeTarget,
        ActionCheck = b => !IsLastAction(IActionHelper.MovingActions),
    };

    public static IBaseAction Onslaught { get; } = new BaseAction(ActionID.Onslaught, ActionOption.EndSpecial)
    {
        ChoiceTarget = TargetFilter.FindTargetForMoving,
    };

    public static IBaseAction Upheaval { get; } = new BaseAction(ActionID.Upheaval)
    {
        //TODO: Why is that status?
        StatusNeed = new StatusID[] { StatusID.SurgingTempest },
    };
    #endregion

    #region Attack Area
    /// <summary>
    /// 1
    /// </summary>
    public static IBaseAction Overpower { get; } = new BaseAction(ActionID.Overpower);

    /// <summary>
    /// 2
    /// </summary>
    public static IBaseAction MythrilTempest { get; } = new BaseAction(ActionID.MythrilTempest);

    public static IBaseAction SteelCyclone { get; } = new BaseAction(ActionID.SteelCyclone)
    {
        ActionCheck = InnerBeast.ActionCheck,
    };

    public static IBaseAction PrimalRend { get; } = new BaseAction(ActionID.PrimalRend)
    {
        StatusNeed = new[] { StatusID.PrimalRendReady }
    };

    public static IBaseAction Orogeny { get; } = new BaseAction(ActionID.Orogeny);

    #endregion

    #region Heal
    private sealed protected override IBaseAction TankStance => Defiance;

    public static IBaseAction Defiance { get; } = new BaseAction(ActionID.Defiance, ActionOption.Defense);
    #endregion


    #region Support
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
    #endregion

    #region Defense Ability
    public static IBaseAction ThrillOfBattle { get; } = new BaseAction(ActionID.ThrillOfBattle, ActionOption.Defense);

    public static IBaseAction Equilibrium { get; } = new BaseAction(ActionID.Equilibrium, ActionOption.Defense);

    public static IBaseAction Vengeance { get; } = new BaseAction(ActionID.Vengeance, ActionOption.Defense)
    {
        StatusProvide = Rampart.StatusProvide,
        ActionCheck = BaseAction.TankDefenseSelf,
    };

    public static IBaseAction RawIntuition { get; } = new BaseAction(ActionID.RawIntuition, ActionOption.Defense)
    {
        ActionCheck = BaseAction.TankDefenseSelf,
    };

    public static IBaseAction NascentFlash { get; } = new BaseAction(ActionID.NascentFlash, ActionOption.Defense)
    {
        ChoiceTarget = TargetFilter.FindAttackedTarget,
    };

    public static IBaseAction ShakeItOff { get; } = new BaseAction(ActionID.ShakeItOff, ActionOption.Defense);

    public static IBaseAction Holmgang { get; } = new BaseAction(ActionID.Holmgang, ActionOption.Defense)
    {
        ChoiceTarget = (tars, mustUse) => Player,
    };
    #endregion

    protected override bool EmergencyAbility(IAction nextGCD, out IAction act)
    {
        if (Holmgang.CanUse(out act) && BaseAction.TankBreakOtherCheck(JobIDs[0])) return true;
        return base.EmergencyAbility(nextGCD, out act);
    }

    [RotationDesc(ActionID.Onslaught)]
    protected sealed override bool MoveForwardAbility(out IAction act)
    {
        if (Onslaught.CanUse(out act)) return true;
        return base.MoveForwardAbility(out act);
    }

    [RotationDesc(ActionID.PrimalRend)]
    protected sealed override bool MoveForwardGCD(out IAction act)
    {
        if (PrimalRend.CanUse(out act, CanUseOption.MustUse)) return true;
        return base.MoveForwardGCD(out act);
    }
}
