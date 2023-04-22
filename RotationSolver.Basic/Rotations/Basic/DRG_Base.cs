namespace RotationSolver.Basic.Rotations.Basic;

public abstract class DRG_Base : CustomRotation
{
    public override MedicineType MedicineType => MedicineType.Strength;

    public sealed override ClassJobID[] JobIDs => new ClassJobID[] { ClassJobID.Dragoon, ClassJobID.Lancer };


    #region Job Gauge
    static DRGGauge JobGauge => Service.JobGauges.Get<DRGGauge>();

    protected static byte EyeCount => JobGauge.EyeCount;

    /// <summary>
    /// Firstminds Count
    /// </summary>
    protected static byte FocusCount => JobGauge.FirstmindsFocusCount;

    static float LOTDTime => JobGauge.LOTDTimer / 1000f;

    protected static bool SongEndAfter(float time) => EndAfter(LOTDTime, time);

    protected static bool SongEndAfterGCD(uint gctCount = 0, float offset = 0)
        => EndAfterGCD(LOTDTime, gctCount, offset);
    #endregion

    #region Attack Single
    /// <summary>
    /// 1
    /// </summary>
    public static IBaseAction TrueThrust { get; } = new BaseAction(ActionID.TrueThrust);

    /// <summary>
    /// 2
    /// </summary>
    public static IBaseAction VorpalThrust { get; } = new BaseAction(ActionID.VorpalThrust)
    {
        ComboIds = new[] { ActionID.RaidenThrust }
    };

    /// <summary>
    /// 3
    /// </summary>
    public static IBaseAction FullThrust { get; } = new BaseAction(ActionID.FullThrust);

    /// <summary>
    /// 3
    /// </summary>
    [Obsolete("Please use Full Thrust instead.")]
    public static IBaseAction HeavensThrust { get; } = new BaseAction(ActionID.HeavensThrust);

    public static IBaseAction Disembowel { get; } = new BaseAction(ActionID.Disembowel)
    {
        ComboIds = new[] { ActionID.RaidenThrust }
    };

    public static IBaseAction ChaosThrust { get; } = new BaseAction(ActionID.ChaosThrust);

    public static IBaseAction FangandClaw { get; } = new BaseAction(ActionID.FangandClaw)
    {
        StatusNeed = new StatusID[] { StatusID.SharperFangandClaw },
    };

    public static IBaseAction WheelingThrust { get; } = new BaseAction(ActionID.WheelingThrust)
    {
        StatusNeed = new StatusID[] { StatusID.EnhancedWheelingThrust },
    };

    public static IBaseAction PiercingTalon { get; } = new BaseAction(ActionID.PiercingTalon)
    {
        FilterForHostiles = TargetFilter.MeleeRangeTargetFilter,
        ActionCheck = b => !IsLastAction(IActionHelper.MovingActions),
    };

    public static IBaseAction SpineShatterDive { get; } = new BaseAction(ActionID.SpineShatterDive);

    public static IBaseAction Jump { get; } = new BaseAction(ActionID.Jump)
    {
        StatusProvide = new StatusID[] { StatusID.DiveReady },
    };

    public static IBaseAction HighJump { get; } = new BaseAction(ActionID.HighJump)
    {
        StatusProvide = Jump.StatusProvide,
    };

    public static IBaseAction MirageDive { get; } = new BaseAction(ActionID.MirageDive)
    {
        StatusNeed = Jump.StatusProvide,
    };
    #endregion

    #region Attack Area
    /// <summary>
    /// 1
    /// </summary>
    public static IBaseAction DoomSpike { get; } = new BaseAction(ActionID.DoomSpike);

    /// <summary>
    /// 2
    /// </summary>
    public static IBaseAction SonicThrust { get; } = new BaseAction(ActionID.SonicThrust)
    {
        ComboIds = new[] { ActionID.DraconianFury }
    };

    /// <summary>
    /// 3
    /// </summary>
    public static IBaseAction CoerthanTorment { get; } = new BaseAction(ActionID.CoerthanTorment);

    public static IBaseAction DragonFireDive { get; } = new BaseAction(ActionID.DragonFireDive);

    public static IBaseAction Geirskogul { get; } = new BaseAction(ActionID.Geirskogul);

    public static IBaseAction Nastrond { get; } = new BaseAction(ActionID.Nastrond)
    {
        ActionCheck = b => JobGauge.IsLOTDActive,
    };

    public static IBaseAction StarDiver { get; } = new BaseAction(ActionID.StarDiver)
    {
        ActionCheck = b => JobGauge.IsLOTDActive,
    };

    public static IBaseAction WyrmwindThrust { get; } = new BaseAction(ActionID.WyrmwindThrust)
    {
        ActionCheck = b => FocusCount == 2,
    };
    #endregion

    #region Support
    public static IBaseAction LifeSurge { get; } = new BaseAction(ActionID.LifeSurge)
    {
        StatusProvide = new[] { StatusID.LifeSurge },
        ActionCheck = b => !IsLastAbility(true, LifeSurge),
    };

    public static IBaseAction LanceCharge { get; } = new BaseAction(ActionID.LanceCharge);

    public static IBaseAction DragonSight { get; } = new BaseAction(ActionID.DragonSight, ActionOption.Buff)
    {
        ChoiceTarget = (Targets, mustUse) =>
        {
            Targets = Targets.Where(b => b.ObjectId != Player.ObjectId &&
            !b.HasStatus(false, StatusID.Weakness, StatusID.BrinkOfDeath)).ToArray();

            if (Targets.Count() == 0) return Player;

            return Targets.GetJobCategory(JobRole.Melee, JobRole.RangedMagical, JobRole.RangedPhysical, JobRole.Tank).FirstOrDefault();
        },
    };

    public static IBaseAction BattleLitany { get; } = new BaseAction(ActionID.BattleLitany, ActionOption.Buff);
    #endregion

    public static IBaseAction ElusiveJump { get; } = new BaseAction(ActionID.ElusiveJump);

    [RotationDesc(ActionID.Feint)]
    protected sealed override bool DefenseAreaAbility(out IAction act)
    {
        if (Feint.CanUse(out act)) return true;
        return false;
    }

    [RotationDesc(ActionID.ElusiveJump)]
    protected override bool MoveBackAbility(out IAction act)
    {
        if(ElusiveJump.CanUse(out act, CanUseOption.IgnoreClippingCheck)) return true;
        return base.MoveBackAbility(out act);
    }
}
