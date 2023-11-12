using ECommons.DalamudServices;
using ECommons.ExcelServices;
using RotationSolver.Basic.Traits;

namespace RotationSolver.Basic.Rotations.Basic;

/// <summary>
/// The base class of DRK.
/// </summary>
public abstract class DRK_Base : CustomRotation
{
    /// <summary>
    /// 
    /// </summary>
    public override MedicineType MedicineType => MedicineType.Strength;

    /// <summary>
    /// 
    /// </summary>
    public sealed override Job[] Jobs => new[] { Job.DRK };

    #region Job Gauge
    private static DRKGauge JobGauge => Svc.Gauges.Get<DRKGauge>();

    /// <summary>
    /// 
    /// </summary>
    public static byte Blood => JobGauge.Blood;

    /// <summary>
    /// 
    /// </summary>
    public static bool HasDarkArts => JobGauge.HasDarkArts;

    static float DarkSideTimeRemainingRaw => JobGauge.DarksideTimeRemaining / 1000f;

    /// <summary>
    /// 
    /// </summary>
    public static float DarkSideTime => DarkSideTimeRemainingRaw - DataCenter.WeaponRemain;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    protected static bool DarkSideEndAfter(float time) => DarkSideTime <= time;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="gctCount"></param>
    /// <param name="offset"></param>
    /// <returns></returns>
    protected static bool DarkSideEndAfterGCD(uint gctCount = 0, float offset = 0)
        => DarkSideEndAfter(GCDTime(gctCount, offset));

    static float ShadowTimeRemainingRaw => JobGauge.ShadowTimeRemaining / 1000f;

    /// <summary>
    /// 
    /// </summary>
    public static float ShadowTime => ShadowTimeRemainingRaw - DataCenter.WeaponRemain;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    protected static bool ShadowTimeEndAfter(float time) => ShadowTimeRemainingRaw <= time;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="gctCount"></param>
    /// <param name="offset"></param>
    /// <returns></returns>
    protected static bool ShadowTimeEndAfterGCD(uint gctCount = 0, float offset = 0)
        => ShadowTimeEndAfter(GCDTime(gctCount, offset));
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

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction EdgeOfDarkness { get; } = new BaseAction(ActionID.EdgeOfDarkness);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction CarveAndSpit { get; } = new BaseAction(ActionID.CarveAndSpit);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction BloodSpiller { get; } = new BaseAction(ActionID.BloodSpiller)
    {
        ActionCheck = (b, m) => JobGauge.Blood >= 50 || Player.HasStatus(true, StatusID.Delirium),
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction Unmend { get; } = new BaseAction(ActionID.Unmend)
    {
        FilterForHostiles = TargetFilter.TankRangeTarget,
        ActionCheck = (b, m) => !IsLastAction(IActionHelper.MovingActions),
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction Plunge { get; } = new BaseAction(ActionID.Plunge, ActionOption.EndSpecial)
    {
        ChoiceTarget = TargetFilter.FindTargetForMoving
    };

    /// <summary>
    /// 
    /// </summary>
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

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction Quietus { get; } = new BaseAction(ActionID.Quietus)
    {
        ActionCheck = BloodSpiller.ActionCheck,
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction FloodOfDarkness { get; } = new BaseAction(ActionID.FloodOfDarkness);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction SaltedEarth { get; } = new BaseAction(ActionID.SaltedEarth);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction AbyssalDrain { get; } = new BaseAction(ActionID.AbyssalDrain);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction SaltandDarkness { get; } = new BaseAction(ActionID.SaltandDarkness)
    {
        StatusNeed = new[] { StatusID.SaltedEarth },
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction ShadowBringer { get; } = new BaseAction(ActionID.ShadowBringer)
    {
        ActionCheck = (b, m) => !DarkSideEndAfterGCD(),
    };
    #endregion

    #region Heal Single
    private protected sealed override IBaseAction TankStance => Grit;

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction Grit { get; } = new BaseAction(ActionID.Grit, ActionOption.Defense | ActionOption.EndSpecial);
    #endregion

    #region Defense Single
    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction ShadowWall { get; } = new BaseAction(ActionID.ShadowWall, ActionOption.Defense)
    {
        StatusProvide = Rampart.StatusProvide,
        ActionCheck = BaseAction.TankDefenseSelf,
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction DarkMind { get; } = new BaseAction(ActionID.DarkMind, ActionOption.Defense)
    {
        ActionCheck = BaseAction.TankDefenseSelf,
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction TheBlackestNight { get; } = new BaseAction(ActionID.TheBlackestNight, ActionOption.Defense)
    {
        ChoiceTarget = TargetFilter.FindAttackedTarget,
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction Oblation { get; } = new BaseAction(ActionID.Oblation, ActionOption.Defense)
    {
        TargetStatus = new StatusID[] { StatusID.Oblation },
        ChoiceTarget = TargetFilter.FindAttackedTarget,
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction LivingDead { get; } = new BaseAction(ActionID.LivingDead, ActionOption.Defense);
    #endregion

    #region Defense Area
    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction DarkMissionary { get; } = new BaseAction(ActionID.DarkMissionary, ActionOption.Defense);
    #endregion

    #region Support
    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction BloodWeapon { get; } = new BaseAction(ActionID.BloodWeapon)
    {
        ActionCheck = (b, m) => IsLongerThan(10),
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction Delirium { get; } = new BaseAction(ActionID.Delirium)
    {
        ActionCheck = (b, m) => IsLongerThan(10),
    };

    #endregion

    #region Traits
    /// <summary>
    /// 
    /// </summary>
    public static IBaseTrait Blackblood { get; } = new BaseTrait(158);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseTrait EnhancedBlackblood { get; } = new BaseTrait(159);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseTrait DarksideMastery { get; } = new BaseTrait(271);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseTrait EnhancedPlunge { get; } = new BaseTrait(272);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseTrait TankMastery { get; } = new BaseTrait(319);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseTrait EnhancedUnmend { get; } = new BaseTrait(422);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseTrait EnhancedLivingShadow2 { get; } = new BaseTrait(423);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseTrait MeleeMastery { get; } = new BaseTrait(506);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseTrait EnhancedLivingShadow { get; } = new BaseTrait(511);

    #endregion

    private protected override IBaseAction LimitBreak => DarkForce;

    /// <summary>
    /// LB
    /// </summary>
    public static IBaseAction DarkForce { get; } = new BaseAction(ActionID.DarkForce, ActionOption.Defense)
    {
        ActionCheck = (b, m) => LimitBreakLevel == 3,
    };

    /// <summary>
    /// 
    /// </summary>
    /// <param name="nextGCD"></param>
    /// <param name="act"></param>
    /// <returns></returns>
    protected override bool EmergencyAbility(IAction nextGCD, out IAction act)
    {
        if (LivingDead.CanUse(out act) && BaseAction.TankBreakOtherCheck(Jobs[0])) return true;
        return base.EmergencyAbility(nextGCD, out act);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="act"></param>
    /// <returns></returns>
    [RotationDesc(ActionID.Plunge)]
    protected sealed override bool MoveForwardAbility(out IAction act)
    {
        if (Plunge.CanUse(out act)) return true;
        return false;
    }
}