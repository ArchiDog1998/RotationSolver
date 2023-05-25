using ECommons.DalamudServices;
using RotationSolver.Basic.Traits;

namespace RotationSolver.Basic.Rotations.Basic;
public abstract class DRK_Base : CustomRotation
{
    public override MedicineType MedicineType => MedicineType.Strength;

    public sealed override ClassJobID[] JobIDs => new ClassJobID[] { ClassJobID.DarkKnight };

    #region Job Gauge
    private static DRKGauge JobGauge => Svc.Gauges.Get<DRKGauge>();

    protected static byte Blood => JobGauge.Blood;

    protected static bool HasDarkArts => JobGauge.HasDarkArts;

    private static float DarkSideTimeRemaining => JobGauge.DarksideTimeRemaining / 1000f;

    protected static bool DarkSideEndAfter(float time)
    {
        return EndAfter(DarkSideTimeRemaining, time);
    }

    protected static bool DarkSideEndAfterGCD(uint gctCount = 0, float offset = 0)
    {
        return EndAfterGCD(DarkSideTimeRemaining, gctCount, offset);
    }

    private static float ShadowTimeRemaining => JobGauge.ShadowTimeRemaining / 1000f;

    protected static bool ShadowTimeEndAfter(float time)
    {
        return EndAfter(ShadowTimeRemaining, time);
    }

    protected static bool ShadowTimeEndAfterGCD(uint gctCount = 0, float offset = 0)
    {
        return EndAfterGCD(ShadowTimeRemaining, gctCount, offset);
    }
    #endregion

    #region Attack Single
    /// <summary>
    /// 1
    /// </summary>
    public static IBaseAction HardSlash { get; } = new BaseAction(ActionID.HardSlash);

    /// <summary>
    /// 2
    /// </summary>
    public static IBaseAction SyphonStrike { get; } = new BaseAction(ActionID.SyphonStrike);

    /// <summary>
    /// 3
    /// </summary>
    public static IBaseAction Souleater { get; } = new BaseAction(ActionID.Souleater);

    public static IBaseAction EdgeOfDarkness { get; } = new BaseAction(ActionID.EdgeOfDarkness);

    public static IBaseAction CarveAndSpit { get; } = new BaseAction(ActionID.CarveAndSpit);

    public static IBaseAction BloodSpiller { get; } = new BaseAction(ActionID.BloodSpiller)
    {
        ActionCheck = (b, m) => JobGauge.Blood >= 50 || Player.HasStatus(true, StatusID.Delirium),
    };

    public static IBaseAction Unmend { get; } = new BaseAction(ActionID.Unmend)
    {
        FilterForHostiles = TargetFilter.TankRangeTarget,
        ActionCheck = (b, m) => !IsLastAction(IActionHelper.MovingActions),
    };

    public static IBaseAction Plunge { get; } = new BaseAction(ActionID.Plunge, ActionOption.EndSpecial)
    {
        ChoiceTarget = TargetFilter.FindTargetForMoving
    };

    public static IBaseAction LivingShadow { get; } = new BaseAction(ActionID.LivingShadow)
    {
        ActionCheck = (b, m) => JobGauge.Blood >= 50,
    };
    #endregion

    #region Attack Area
    /// <summary>
    /// 1
    /// </summary>
    public static IBaseAction Unleash { get; } = new BaseAction(ActionID.Unleash);

    /// <summary>
    /// 2
    /// </summary>
    public static IBaseAction StalwartSoul { get; } = new BaseAction(ActionID.StalwartSoul);

    public static IBaseAction Quietus { get; } = new BaseAction(ActionID.Quietus)
    {
        ActionCheck = BloodSpiller.ActionCheck,
    };

    public static IBaseAction FloodOfDarkness { get; } = new BaseAction(ActionID.FloodOfDarkness);

    public static IBaseAction SaltedEarth { get; } = new BaseAction(ActionID.SaltedEarth);

    public static IBaseAction AbyssalDrain { get; } = new BaseAction(ActionID.AbyssalDrain);

    public static IBaseAction SaltandDarkness { get; } = new BaseAction(ActionID.SaltandDarkness)
    {
        StatusNeed = new[] { StatusID.SaltedEarth },
    };

    public static IBaseAction ShadowBringer { get; } = new BaseAction(ActionID.ShadowBringer)
    {
        ActionCheck = (b, m) => !DarkSideEndAfterGCD(),
    };
    #endregion

    #region Heal Single
    private sealed protected override IBaseAction TankStance => Grit;
    public static IBaseAction Grit { get; } = new BaseAction(ActionID.Grit, ActionOption.Defense | ActionOption.EndSpecial);
    #endregion

    #region Defense Single
    public static IBaseAction ShadowWall { get; } = new BaseAction(ActionID.ShadowWall, ActionOption.Defense)
    {
        StatusProvide = Rampart.StatusProvide,
        ActionCheck = BaseAction.TankDefenseSelf,
    };

    public static IBaseAction DarkMind { get; } = new BaseAction(ActionID.DarkMind, ActionOption.Defense)
    {
        ActionCheck = BaseAction.TankDefenseSelf,
    };

    public static IBaseAction TheBlackestNight { get; } = new BaseAction(ActionID.TheBlackestNight, ActionOption.Defense)
    {
        ChoiceTarget = TargetFilter.FindAttackedTarget,
    };

    public static IBaseAction Oblation { get; } = new BaseAction(ActionID.Oblation, ActionOption.Defense)
    {
        TargetStatus = new StatusID[] { StatusID.Oblation },
        ChoiceTarget = TargetFilter.FindAttackedTarget,
    };

    public static IBaseAction LivingDead { get; } = new BaseAction(ActionID.LivingDead, ActionOption.Defense);
    #endregion

    #region Defense Area
    public static IBaseAction DarkMissionary { get; } = new BaseAction(ActionID.DarkMissionary, ActionOption.Defense);

    #endregion

    #region Support
    public static IBaseAction BloodWeapon { get; } = new BaseAction(ActionID.BloodWeapon);

    public static IBaseAction Delirium { get; } = new BaseAction(ActionID.Delirium);
    #endregion

    #region Traits
    protected static IBaseTrait Blackblood    { get; } = new BaseTrait(158);
    protected static IBaseTrait EnhancedBlackblood    { get; } = new BaseTrait(159);
    protected static IBaseTrait DarksideMastery    { get; } = new BaseTrait(271);
    protected static IBaseTrait EnhancedPlunge    { get; } = new BaseTrait(272);
    protected static IBaseTrait TankMastery    { get; } = new BaseTrait(319);
    protected static IBaseTrait EnhancedUnmend    { get; } = new BaseTrait(422);
    protected static IBaseTrait EnhancedLivingShadow2    { get; } = new BaseTrait(423);
    protected static IBaseTrait MeleeMastery    { get; } = new BaseTrait(506);
    protected static IBaseTrait EnhancedLivingShadow { get; } = new BaseTrait(511);

    #endregion

    protected override bool EmergencyAbility(IAction nextGCD, out IAction act)
    {
        if (LivingDead.CanUse(out act) && BaseAction.TankBreakOtherCheck(JobIDs[0])) return true;
        return base.EmergencyAbility(nextGCD, out act);
    }

    [RotationDesc(ActionID.Plunge)]
    protected sealed override bool MoveForwardAbility(out IAction act)
    {
        if (Plunge.CanUse(out act)) return true;
        return false;
    }
}