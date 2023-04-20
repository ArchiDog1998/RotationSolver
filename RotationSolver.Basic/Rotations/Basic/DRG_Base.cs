namespace RotationSolver.Basic.Rotations.Basic;

public abstract class DRG_Base : CustomRotation
{
    private static DRGGauge JobGauge => Service.JobGauges.Get<DRGGauge>();

    public override MedicineType MedicineType => MedicineType.Strength;

    public sealed override ClassJobID[] JobIDs => new ClassJobID[] { ClassJobID.Dragoon, ClassJobID.Lancer };

    public static IBaseAction TrueThrust { get; } = new BaseAction(ActionID.TrueThrust);

    public static IBaseAction VorpalThrust { get; } = new BaseAction(ActionID.VorpalThrust)
    {
        ComboIds = new[] { ActionID.RaidenThrust }
    };

    public static IBaseAction HeavensThrust { get; } = new BaseAction(ActionID.HeavensThrust);

    public static IBaseAction FullThrust { get; } = new BaseAction(ActionID.FullThrust);

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

    public static IBaseAction DoomSpike { get; } = new BaseAction(ActionID.DoomSpike);

    public static IBaseAction SonicThrust { get; } = new BaseAction(ActionID.SonicThrust)
    {
        ComboIds = new[] { ActionID.DraconianFury }
    };

    public static IBaseAction CoerthanTorment { get; } = new BaseAction(ActionID.CoerthanTorment);

    public static IBaseAction SpineShatterDive { get; } = new BaseAction(ActionID.SpineShatterDive);

    public static IBaseAction DragonFireDive { get; } = new BaseAction(ActionID.DragonFireDive);

    public static IBaseAction ElusiveJump { get; } = new BaseAction(ActionID.ElusiveJump);

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
        ActionCheck = b => JobGauge.FirstmindsFocusCount == 2,
    };

    public static IBaseAction LifeSurge { get; } = new BaseAction(ActionID.LifeSurge, ActionOption.Buff)
    {
        StatusProvide = new[] { StatusID.LifeSurge },

        ActionCheck = b => !IsLastAbility(true, LifeSurge),
    };

    public static IBaseAction LanceCharge { get; } = new BaseAction(ActionID.LanceCharge, ActionOption.Buff);

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

    [RotationDesc(ActionID.Feint)]
    protected sealed override bool DefenseAreaAbility(out IAction act)
    {
        if (Feint.CanUse(out act)) return true;
        return false;
    }
}
