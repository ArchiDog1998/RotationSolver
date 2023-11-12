using ECommons.DalamudServices;
using ECommons.ExcelServices;
using RotationSolver.Basic.Traits;

namespace RotationSolver.Basic.Rotations.Basic;

/// <summary>
/// The base class of GNB>
/// </summary>
public abstract class GNB_Base : CustomRotation
{
    /// <summary>
    /// 
    /// </summary>
    public sealed override Job[] Jobs => new[] { Job.GNB };

    /// <summary>
    /// 
    /// </summary>
    public override MedicineType MedicineType => MedicineType.Strength;

    /// <summary>
    /// 
    /// </summary>
    public override bool CanHealSingleSpell => false;

    /// <summary>
    /// 
    /// </summary>
    public override bool CanHealAreaSpell => false;

    #region Job Gauge
    static GNBGauge JobGauge => Svc.Gauges.Get<GNBGauge>();

    /// <summary>
    /// 
    /// </summary>
    public static byte Ammo => JobGauge.Ammo;

    /// <summary>
    /// 
    /// </summary>
    public static byte AmmoComboStep => JobGauge.AmmoComboStep;

    /// <summary>
    /// 
    /// </summary>
    public static byte MaxAmmo => CartridgeCharge2.EnoughLevel ? (byte)3 : (byte)2;
    #endregion

    #region Attack Single
    /// <summary>
    /// 1
    /// </summary>
    public static IBaseAction KeenEdge { get; } = new BaseAction(ActionID.KeenEdge);

    /// <summary>
    /// 2
    /// </summary>
    public static IBaseAction BrutalShell { get; } = new BaseAction(ActionID.BrutalShell);

    /// <summary>
    /// 3
    /// </summary>
    public static IBaseAction SolidBarrel { get; } = new BaseAction(ActionID.SolidBarrel);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction DangerZone { get; } = new BaseAction(ActionID.DangerZone);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction SonicBreak { get; } = new BaseAction(ActionID.SonicBreak);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction BurstStrike { get; } = new BaseAction(ActionID.BurstStrike, ActionOption.UseResources)
    {
        ActionCheck = (b, m) => Ammo > 0,
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction GnashingFang { get; } = new BaseAction(ActionID.GnashingFang, ActionOption.UseResources)
    {
        ActionCheck = (b, m) => AmmoComboStep == 0 && Ammo > 0,
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction SavageClaw { get; } = new BaseAction(ActionID.SavageClaw)
    {
        ActionCheck = (b, m) => Service.GetAdjustedActionId(ActionID.GnashingFang) == ActionID.SavageClaw,
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction WickedTalon { get; } = new BaseAction(ActionID.WickedTalon)
    {
        ActionCheck = (b, m) => Service.GetAdjustedActionId(ActionID.GnashingFang) == ActionID.WickedTalon,
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction JugularRip { get; } = new BaseAction(ActionID.JugularRip)
    {
        ActionCheck = (b, m) => Service.GetAdjustedActionId(ActionID.Continuation) == ActionID.JugularRip,
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction AbdomenTear { get; } = new BaseAction(ActionID.AbdomenTear)
    {
        ActionCheck = (b, m) => Service.GetAdjustedActionId(ActionID.Continuation) == ActionID.AbdomenTear,
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction EyeGouge { get; } = new BaseAction(ActionID.EyeGouge)
    {
        ActionCheck = (b, m) => Service.GetAdjustedActionId(ActionID.Continuation) == ActionID.EyeGouge,
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction Hypervelocity { get; } = new BaseAction(ActionID.Hypervelocity)
    {
        ActionCheck = (b, m) => Service.GetAdjustedActionId(ActionID.Continuation) == ActionID.Hypervelocity,
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction LightningShot { get; } = new BaseAction(ActionID.LightningShot)
    {
        FilterForHostiles = TargetFilter.TankRangeTarget,
        ActionCheck = (b, m) => !IsLastAction(IActionHelper.MovingActions),
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction RoughDivide { get; } = new BaseAction(ActionID.RoughDivide, ActionOption.EndSpecial)
    {
        ChoiceTarget = TargetFilter.FindTargetForMoving,
    };
    #endregion

    #region Attack Area
    /// <summary>
    /// 1
    /// </summary>
    public static IBaseAction DemonSlice { get; } = new BaseAction(ActionID.DemonSlice);

    /// <summary>
    /// 2
    /// </summary>
    public static IBaseAction DemonSlaughter { get; } = new BaseAction(ActionID.DemonSlaughter);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction FatedCircle { get; } = new BaseAction(ActionID.FatedCircle, ActionOption.UseResources)
    {
        ActionCheck = (b, m) => Ammo > 0,
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction DoubleDown { get; } = new BaseAction(ActionID.DoubleDown, ActionOption.UseResources)
    {
        ActionCheck = (b, m) => Ammo > 1,
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction BowShock { get; } = new BaseAction(ActionID.BowShock);
    #endregion

    #region Heal
    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction Aurora { get; } = new BaseAction(ActionID.Aurora, ActionOption.Heal)
    {
        TargetStatus = new StatusID[] { StatusID.Aurora },
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction HeartOfLight { get; } = new BaseAction(ActionID.HeartOfLight, ActionOption.Heal);
    #endregion

    #region Defense Single
    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction Nebula { get; } = new BaseAction(ActionID.Nebula, ActionOption.Defense)
    {
        StatusProvide = Rampart.StatusProvide,
        ActionCheck = BaseAction.TankDefenseSelf,
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction Camouflage { get; } = new BaseAction(ActionID.Camouflage, ActionOption.Defense)
    {
        ActionCheck = BaseAction.TankDefenseSelf,
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction HeartOfStone { get; } = new BaseAction(ActionID.HeartOfStone, ActionOption.Defense)
    {
        ChoiceTarget = TargetFilter.FindAttackedTarget,
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction SuperBolide { get; } = new BaseAction(ActionID.SuperBolide, ActionOption.Defense);
    #endregion

    #region Support
    private protected sealed override IBaseAction TankStance => RoyalGuard;
    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction RoyalGuard { get; } = new BaseAction(ActionID.RoyalGuard, ActionOption.Defense | ActionOption.EndSpecial);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction NoMercy { get; } = new BaseAction(ActionID.NoMercy)
    {
        ActionCheck = (b, m) => IsLongerThan(10),
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction BloodFest { get; } = new BaseAction(ActionID.BloodFest, ActionOption.Buff)
    {
        ActionCheck = (b, m) => MaxAmmo - Ammo > 1,
    };
    #endregion

    #region Traits
    /// <summary>
    /// 
    /// </summary>
    public static IBaseTrait CartridgeCharge { get; } = new BaseTrait(257);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseTrait EnhancedBrutalShell { get; } = new BaseTrait(258);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseTrait DangerZoneMastery { get; } = new BaseTrait(259);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseTrait TankMastery { get; } = new BaseTrait(320);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseTrait HeartOfStoneMastery { get; } = new BaseTrait(424);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseTrait EnhancedAurora { get; } = new BaseTrait(425);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseTrait EnhancedContinuation { get; } = new BaseTrait(426);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseTrait CartridgeCharge2 { get; } = new BaseTrait(427);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseTrait MeleeMastery { get; } = new BaseTrait(507);
    #endregion

    private protected override IBaseAction LimitBreak => GunmetalSoul;

    /// <summary>
    /// LB
    /// </summary>
    public static IBaseAction GunmetalSoul { get; } = new BaseAction(ActionID.GunmetalSoul, ActionOption.Defense)
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
        if (SuperBolide.CanUse(out act) && BaseAction.TankBreakOtherCheck(Jobs[0])) return true;
        return base.EmergencyAbility(nextGCD, out act);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="act"></param>
    /// <returns></returns>
    [RotationDesc(ActionID.RoughDivide)]
    protected sealed override bool MoveForwardAbility(out IAction act)
    {
        if (RoughDivide.CanUse(out act)) return true;
        return false;
    }
}

