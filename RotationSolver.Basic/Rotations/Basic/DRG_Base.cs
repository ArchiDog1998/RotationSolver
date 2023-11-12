using ECommons.DalamudServices;
using ECommons.ExcelServices;
using RotationSolver.Basic.Traits;

namespace RotationSolver.Basic.Rotations.Basic;

/// <summary>
/// The base class of DRG>
/// </summary>
public abstract class DRG_Base : CustomRotation
{
    /// <summary>
    /// 
    /// </summary>
    public override MedicineType MedicineType => MedicineType.Strength;

    /// <summary>
    /// 
    /// </summary>
    public sealed override Job[] Jobs => new[] { Job.DRG, Job.LNC };

    #region Job Gauge
    static DRGGauge JobGauge => Svc.Gauges.Get<DRGGauge>();

    /// <summary>
    /// 
    /// </summary>
    public static byte EyeCount => JobGauge.EyeCount;

    /// <summary>
    /// Firstminds Count
    /// </summary>
    public static byte FocusCount => JobGauge.FirstmindsFocusCount;

    /// <summary>
    /// 
    /// </summary>
    static float LOTDTimeRaw => JobGauge.LOTDTimer / 1000f;

    /// <summary>
    /// 
    /// </summary>
    public static float LOTDTime => LOTDTimeRaw - DataCenter.WeaponRemain;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    protected static bool LOTDEndAfter(float time) => LOTDTime <= time;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="gctCount"></param>
    /// <param name="offset"></param>
    /// <returns></returns>
    protected static bool LOTDEndAfterGCD(uint gctCount = 0, float offset = 0)
        => LOTDEndAfter(GCDTime(gctCount, offset));
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
    /// 
    /// </summary>
    public static IBaseAction Disembowel { get; } = new BaseAction(ActionID.Disembowel)
    {
        ComboIds = new[] { ActionID.RaidenThrust }
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction ChaosThrust { get; } = new BaseAction(ActionID.ChaosThrust);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction FangandClaw { get; } = new BaseAction(ActionID.FangandClaw)
    {
        StatusNeed = new StatusID[] { StatusID.SharperFangandClaw },
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction WheelingThrust { get; } = new BaseAction(ActionID.WheelingThrust)
    {
        StatusNeed = new StatusID[] { StatusID.EnhancedWheelingThrust },
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction PiercingTalon { get; } = new BaseAction(ActionID.PiercingTalon)
    {
        FilterForHostiles = TargetFilter.MeleeRangeTargetFilter,
        ActionCheck = (b, m) => !IsLastAction(IActionHelper.MovingActions),
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction SpineShatterDive { get; } = new BaseAction(ActionID.SpineShatterDive);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction Jump { get; } = new BaseAction(ActionID.Jump)
    {
        StatusProvide = new StatusID[] { StatusID.DiveReady },
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction HighJump { get; } = new BaseAction(ActionID.HighJump)
    {
        StatusProvide = Jump.StatusProvide,
    };

    /// <summary>
    /// 
    /// </summary>
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

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction DragonFireDive { get; } = new BaseAction(ActionID.DragonFireDive);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction Geirskogul { get; } = new BaseAction(ActionID.Geirskogul);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction Nastrond { get; } = new BaseAction(ActionID.Nastrond)
    {
        ActionCheck = (b, m) => JobGauge.IsLOTDActive,
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction StarDiver { get; } = new BaseAction(ActionID.StarDiver)
    {
        ActionCheck = (b, m) => JobGauge.IsLOTDActive,
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction WyrmwindThrust { get; } = new BaseAction(ActionID.WyrmwindThrust, ActionOption.UseResources)
    {
        ActionCheck = (b, m) => FocusCount == 2,
    };
    #endregion

    #region Support
    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction LifeSurge { get; } = new BaseAction(ActionID.LifeSurge)
    {
        StatusProvide = new[] { StatusID.LifeSurge },
        ActionCheck = (b, m) => !IsLastAbility(true, LifeSurge),
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction LanceCharge { get; } = new BaseAction(ActionID.LanceCharge)
    {
        ActionCheck = (b, m) => IsLongerThan(10),
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction DragonSight { get; } = new BaseAction(ActionID.DragonSight, ActionOption.Buff)
    {
        ChoiceTarget = (Targets, mustUse) =>
        {
            Targets = Targets.Where(b => b.ObjectId != Player.ObjectId &&
            !b.HasStatus(false, StatusID.Weakness, StatusID.BrinkOfDeath)).ToArray();

            if (!Targets.Any()) return Player;

            return Targets.GetJobCategory(JobRole.Melee, JobRole.RangedMagical, JobRole.RangedPhysical, JobRole.Tank).FirstOrDefault();
        },
        ActionCheck = (b, m) => IsLongerThan(10),
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction BattleLitany { get; } = new BaseAction(ActionID.BattleLitany, ActionOption.Buff)
    {
        ActionCheck = (b, m) => IsLongerThan(10),
    };
    #endregion

    #region Traits
    /// <summary>
    /// 
    /// </summary>
    public static IBaseTrait LanceMastery { get; } = new BaseTrait(162);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseTrait LifeOfTheDragon { get; } = new BaseTrait(163);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseTrait LanceMastery2 { get; } = new BaseTrait(247);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseTrait JumpMastery { get; } = new BaseTrait(275);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseTrait LifeOfTheDragonMastery { get; } = new BaseTrait(276);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseTrait BloodOfTheDragon { get; } = new BaseTrait(434);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseTrait EnhancedCoerthanTorment { get; } = new BaseTrait(435);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseTrait EnhancedSpineshatterDive { get; } = new BaseTrait(436);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseTrait LanceMastery3 { get; } = new BaseTrait(437);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseTrait EnhancedLifeSurge { get; } = new BaseTrait(438);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseTrait LanceMastery4 { get; } = new BaseTrait(508);

    #endregion
    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction ElusiveJump { get; } = new BaseAction(ActionID.ElusiveJump);

    private protected override IBaseAction LimitBreak => DragonsongDive;

    /// <summary>
    /// LB
    /// </summary>
    public static IBaseAction DragonsongDive { get; } = new BaseAction(ActionID.DragonsongDive)
    {
        ActionCheck = (b, m) => LimitBreakLevel == 3,
    };

    /// <summary>
    /// 
    /// </summary>
    /// <param name="act"></param>
    /// <returns></returns>
    [RotationDesc(ActionID.Feint)]
    protected sealed override bool DefenseAreaAbility(out IAction act)
    {
        if (Feint.CanUse(out act)) return true;
        return false;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="act"></param>
    /// <returns></returns>
    [RotationDesc(ActionID.ElusiveJump)]
    protected override bool MoveBackAbility(out IAction act)
    {
        if (ElusiveJump.CanUse(out act, CanUseOption.IgnoreClippingCheck)) return true;
        return base.MoveBackAbility(out act);
    }
}
