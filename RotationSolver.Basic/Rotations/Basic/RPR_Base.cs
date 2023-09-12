using ECommons.DalamudServices;
using ECommons.ExcelServices;
using RotationSolver.Basic.Traits;

namespace RotationSolver.Basic.Rotations.Basic;

/// <summary>
/// The base class of RPR.
/// </summary>
public abstract class RPR_Base : CustomRotation
{
    /// <summary>
    /// 
    /// </summary>
    public override MedicineType MedicineType => MedicineType.Strength;

    /// <summary>
    /// 
    /// </summary>
    public sealed override Job[] Jobs => new [] { Job.RPR };

    /// <summary>
    /// 
    /// </summary>
    public static bool HasEnshrouded => Player.HasStatus(true, StatusID.Enshrouded);

    /// <summary>
    /// 
    /// </summary>
    public static bool HasSoulReaver => Player.HasStatus(true, StatusID.SoulReaver);

    #region JobGauge
    static RPRGauge JobGauge => Svc.Gauges.Get<RPRGauge>();

    /// <summary>
    /// 
    /// </summary>
    public static byte Soul => JobGauge.Soul;

    /// <summary>
    /// 
    /// </summary>
    public static byte Shroud => JobGauge.Shroud;

    /// <summary>
    /// 
    /// </summary>
    public static byte LemureShroud => JobGauge.LemureShroud;

    /// <summary>
    /// 
    /// </summary>
    public static byte VoidShroud => JobGauge.VoidShroud;
    #endregion

    #region Attack Single
    /// <summary>
    /// 1
    /// </summary>
    public static IBaseAction Slice { get; } = new BaseAction(ActionID.Slice)
    {
        ActionCheck = (b, m) => !HasEnshrouded && !HasSoulReaver,
    };

    /// <summary>
    /// 2
    /// </summary>
    public static IBaseAction WaxingSlice { get; } = new BaseAction(ActionID.WaxingSlice)
    {
        ActionCheck = Slice.ActionCheck,
    };

    /// <summary>
    /// 3
    /// </summary>
    public static IBaseAction InfernalSlice { get; } = new BaseAction(ActionID.InfernalSlice)
    {
        ActionCheck = Slice.ActionCheck,
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction ShadowOfDeath { get; } = new BaseAction(ActionID.ShadowOfDeath, ActionOption.Dot)
    {
        TargetStatus = new[] { StatusID.DeathsDesign },
        ActionCheck = (b, m) => !HasSoulReaver,
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction SoulSlice { get; } = new BaseAction(ActionID.SoulSlice)
    {
        ActionCheck = (b, m) => Slice.ActionCheck(b, m) && Soul <= 50,
    };
    #endregion

    #region Attack Area
    /// <summary>
    /// 1
    /// </summary>
    public static IBaseAction SpinningScythe { get; } = new BaseAction(ActionID.SpinningScythe)
    {
        ActionCheck = Slice.ActionCheck,
    };

    /// <summary>
    /// 2
    /// </summary>
    public static IBaseAction NightmareScythe { get; } = new BaseAction(ActionID.NightmareScythe)
    {
        ActionCheck = Slice.ActionCheck,
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction WhorlOfDeath { get; } = new BaseAction(ActionID.WhorlOfDeath, ActionOption.Dot)
    {
        TargetStatus = new[] { StatusID.DeathsDesign },
        ActionCheck = ShadowOfDeath.ActionCheck,
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction SoulScythe { get; } = new BaseAction(ActionID.SoulScythe)
    {
        ActionCheck = SoulSlice.ActionCheck,
    };
    #endregion

    #region Soul Reaver
    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction Gibbet { get; } = new BaseAction(ActionID.Gibbet)
    {
        StatusNeed = new[] { StatusID.SoulReaver }
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction Gallows { get; } = new BaseAction(ActionID.Gallows)
    {
        StatusNeed = new[] { StatusID.SoulReaver }
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction Guillotine { get; } = new BaseAction(ActionID.Guillotine)
    {
        StatusNeed = new[] { StatusID.SoulReaver }
    };
    #endregion

    #region Soul
    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction BloodStalk { get; } = new BaseAction(ActionID.BloodStalk, ActionOption.UseResources)
    {
        StatusProvide = new[] { StatusID.SoulReaver },
        ActionCheck = (b, m) => Slice.ActionCheck(b, m) && Soul >= 50
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction GrimSwathe { get; } = new BaseAction(ActionID.GrimSwathe, ActionOption.UseResources)
    {
        StatusProvide = new[] { StatusID.SoulReaver },
        ActionCheck = BloodStalk.ActionCheck,
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction Gluttony { get; } = new BaseAction(ActionID.Gluttony, ActionOption.UseResources)
    {
        StatusProvide = new[] { StatusID.SoulReaver },
        ActionCheck = BloodStalk.ActionCheck,
    };
    #endregion

    #region Burst
    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction ArcaneCircle { get; } = new BaseAction(ActionID.ArcaneCircle, ActionOption.Buff)
    {
        StatusProvide = new[] { StatusID.CircleOfSacrifice, StatusID.BloodSownCircle },
        ActionCheck = (b, m) => IsLongerThan(10),
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction PlentifulHarvest { get; } = new BaseAction(ActionID.PlentifulHarvest)
    {
        StatusNeed = new[] { StatusID.ImmortalSacrifice },
        ActionCheck = (b, m) => !Player.HasStatus(true, StatusID.BloodSownCircle)
    };
    #endregion

    #region Shroud
    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction Enshroud { get; } = new BaseAction(ActionID.Enshroud, ActionOption.UseResources)
    {
        StatusProvide = new[] { StatusID.Enshrouded },
        ActionCheck = (b, m) => Shroud >= 50 && Slice.ActionCheck(b, m)
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction Communio { get; } = new BaseAction(ActionID.Communio)
    {
        StatusNeed = new[] { StatusID.Enshrouded },
        ActionCheck = (b, m) => LemureShroud == 1
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction LemuresSlice { get; } = new BaseAction(ActionID.LemuresSlice)
    {
        StatusNeed = new[] { StatusID.Enshrouded },
        ActionCheck = (b, m) => VoidShroud >= 2,
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction LemuresScythe { get; } = new BaseAction(ActionID.LemuresScythe)
    {
        StatusNeed = new[] { StatusID.Enshrouded },
        ActionCheck = LemuresSlice.ActionCheck,
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction VoidReaping { get; } = new BaseAction(ActionID.VoidReaping)
    {
        StatusNeed = new[] { StatusID.Enshrouded },
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction CrossReaping { get; } = new BaseAction(ActionID.CrossReaping)
    {
        StatusNeed = new[] { StatusID.Enshrouded },
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction GrimReaping { get; } = new BaseAction(ActionID.GrimReaping)
    {
        StatusNeed = new[] { StatusID.Enshrouded },
    };
    #endregion

    #region Others
    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction Harpe { get; } = new BaseAction(ActionID.Harpe)
    {
        ActionCheck = (b, m) => !HasSoulReaver && !IsLastAction(IActionHelper.MovingActions),
        FilterForHostiles = TargetFilter.MeleeRangeTargetFilter,
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction HellsIngress { get; } = new BaseAction(ActionID.HellsIngress)
    {
        StatusProvide = new[] { StatusID.EnhancedHarpe },
        ActionCheck = (b, m) => !Player.HasStatus(true, StatusID.Bind1, StatusID.Bind2)
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction HellsEgress { get; } = new BaseAction(ActionID.HellsEgress)
    {
        StatusProvide = HellsIngress.StatusProvide,
        ActionCheck = HellsIngress.ActionCheck
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction SoulSow { get; } = new BaseAction(ActionID.SoulSow)
    {
        StatusProvide = new[] { StatusID.SoulSow },
        ActionCheck = (b, m) => !InCombat,
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction HarvestMoon { get; } = new BaseAction(ActionID.HarvestMoon)
    {
        StatusNeed = new[] { StatusID.SoulSow },
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction ArcaneCrest { get; } = new BaseAction(ActionID.ArcaneCrest, ActionOption.Defense);
    #endregion

    #region Traits
    /// <summary>
    /// 
    /// </summary>
    public static IBaseTrait SoulGauge { get; } = new BaseTrait(379);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseTrait DeathScytheMastery    { get; } = new BaseTrait(380);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseTrait EnhancedAvatar    { get; } = new BaseTrait(381);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseTrait Hellsgate    { get; } = new BaseTrait(382);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseTrait TemperedSoul    { get; } = new BaseTrait(383);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseTrait ShroudGauge    { get; } = new BaseTrait(384);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseTrait EnhancedArcaneCrest    { get; } = new BaseTrait(385);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseTrait EnhancedShroud    { get; } = new BaseTrait(386);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseTrait EnhancedArcaneCircle    { get; } = new BaseTrait(387);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseTrait DeathScytheMastery2    { get; } = new BaseTrait(523);
    #endregion

    /// <summary>
    /// 
    /// </summary>
    /// <param name="act"></param>
    /// <returns></returns>
    [RotationDesc(ActionID.HellsIngress)]
    protected sealed override bool MoveForwardAbility(out IAction act)
    {
        if (HellsIngress.CanUse(out act)) return true;
        return base.MoveForwardAbility(out act);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="act"></param>
    /// <returns></returns>
    [RotationDesc(ActionID.Feint)]
    protected sealed override bool DefenseAreaAbility(out IAction act)
    {
        if (!HasSoulReaver && !HasEnshrouded && Feint.CanUse(out act)) return true;
        return base.DefenseAreaAbility(out act);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="act"></param>
    /// <returns></returns>
    [RotationDesc(ActionID.ArcaneCrest)]
    protected override bool DefenseSingleAbility(out IAction act)
    {
        if (!HasSoulReaver && !HasEnshrouded && ArcaneCrest.CanUse(out act)) return true;
        return base.DefenseSingleAbility(out act);
    }
}
