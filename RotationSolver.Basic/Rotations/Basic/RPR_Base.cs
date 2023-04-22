namespace RotationSolver.Basic.Rotations.Basic;

public abstract class RPR_Base : CustomRotation
{
    public override MedicineType MedicineType => MedicineType.Strength;
    public sealed override ClassJobID[] JobIDs => new ClassJobID[] { ClassJobID.Reaper };
    protected static bool Enshrouded => Player.HasStatus(true, StatusID.Enshrouded);

    protected static bool SoulReaver => Player.HasStatus(true, StatusID.SoulReaver);

    #region JobGauge
    static RPRGauge JobGauge => Service.JobGauges.Get<RPRGauge>();

    protected static byte Soul => JobGauge.Soul;

    protected static byte Shroud => JobGauge.Shroud;

    protected static byte LemureShroud => JobGauge.LemureShroud;

    protected static byte VoidShroud => JobGauge.VoidShroud;
    #endregion

    #region Attack Single
    /// <summary>
    /// 1
    /// </summary>
    public static IBaseAction Slice { get; } = new BaseAction(ActionID.Slice)
    {
        ActionCheck = b => !Enshrouded && !SoulReaver,
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

        public static IBaseAction ShadowOfDeath { get; } = new BaseAction(ActionID.ShadowOfDeath, ActionOption.Dot)
    {
        TargetStatus = new[] { StatusID.DeathsDesign },
        ActionCheck = b => !SoulReaver,
    };

    public static IBaseAction SoulSlice { get; } = new BaseAction(ActionID.SoulSlice)
    {
        ActionCheck = b => !Enshrouded && !SoulReaver && Soul <= 50,
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

    public static IBaseAction WhorlOfDeath { get; } = new BaseAction(ActionID.WhorlOfDeath, ActionOption.Dot)
    {
        TargetStatus = new[] { StatusID.DeathsDesign },
        ActionCheck = ShadowOfDeath.ActionCheck,
        AOECount = 2,
    };

    public static IBaseAction SoulScythe { get; } = new BaseAction(ActionID.SoulScythe)
    {
        ActionCheck = SoulSlice.ActionCheck,
    };
    #endregion
    #region Soul Reaver
    public static IBaseAction Gibbet { get; } = new BaseAction(ActionID.Gibbet)
    {
        StatusNeed = new[] { StatusID.SoulReaver }
    };

    public static IBaseAction Gallows { get; } = new BaseAction(ActionID.Gallows)
    {
        StatusNeed = new[] { StatusID.SoulReaver }
    };

    public static IBaseAction Guillotine { get; } = new BaseAction(ActionID.Guillotine)
    {
        StatusNeed = new[] { StatusID.SoulReaver }
    };
    #endregion

    #region Soul
    public static IBaseAction BloodStalk { get; } = new BaseAction(ActionID.BloodStalk)
    {
        StatusProvide = new[] { StatusID.SoulReaver },
        ActionCheck = b => !SoulReaver && !Enshrouded && Soul >= 50
    };

    public static IBaseAction GrimSwathe { get; } = new BaseAction(ActionID.GrimSwathe)
    {
        StatusProvide = new[] { StatusID.SoulReaver },
        ActionCheck = BloodStalk.ActionCheck,
    };

    public static IBaseAction Gluttony { get; } = new BaseAction(ActionID.Gluttony)
    {
        StatusProvide = new[] { StatusID.SoulReaver },
        ActionCheck = BloodStalk.ActionCheck,
    };
    #endregion

    #region Burst
    public static IBaseAction ArcaneCircle { get; } = new BaseAction(ActionID.ArcaneCircle, ActionOption.Buff)
    {
        StatusProvide = new[] { StatusID.CircleOfSacrifice, StatusID.BloodSownCircle }
    };

    public static IBaseAction PlentifulHarvest { get; } = new BaseAction(ActionID.PlentifulHarvest)
    {
        StatusNeed = new[] { StatusID.ImmortalSacrifice },
        ActionCheck = b => !Player.HasStatus(true, StatusID.BloodSownCircle)
    };
    #endregion

    #region Shroud
    public static IBaseAction Enshroud { get; } = new BaseAction(ActionID.Enshroud)
    {
        StatusProvide = new[] { StatusID.Enshrouded },
        ActionCheck = b => Shroud >= 50 && !SoulReaver && !Enshrouded
    };

    public static IBaseAction Communio { get; } = new BaseAction(ActionID.Communio)
    {
        StatusNeed = new[] { StatusID.Enshrouded },
        ActionCheck = b => LemureShroud == 1
    };

    public static IBaseAction LemuresSlice { get; } = new BaseAction(ActionID.LemuresSlice)
    {
        StatusNeed = new[] { StatusID.Enshrouded },
        ActionCheck = b => VoidShroud >= 2,
    };

    public static IBaseAction LemuresScythe { get; } = new BaseAction(ActionID.LemuresScythe)
    {
        StatusNeed = new[] { StatusID.Enshrouded },
        ActionCheck = b => VoidShroud >= 2,
    };

    public static IBaseAction VoidReaping { get; } = new BaseAction(ActionID.VoidReaping)
    {
        StatusNeed = new[] { StatusID.Enshrouded },
    };

    public static IBaseAction CrossReaping { get; } = new BaseAction(ActionID.CrossReaping)
    {
        StatusNeed = new[] { StatusID.Enshrouded },
    };

    public static IBaseAction GrimReaping { get; } = new BaseAction(ActionID.GrimReaping)
    {
        StatusNeed = new[] { StatusID.Enshrouded },
    };
    #endregion

    #region Others
    public static IBaseAction Harpe { get; } = new BaseAction(ActionID.Harpe)
    {
        ActionCheck = b => !SoulReaver && !IsLastAction(IActionHelper.MovingActions),
        FilterForHostiles = TargetFilter.MeleeRangeTargetFilter,
    };

    public static IBaseAction HellsIngress { get; } = new BaseAction(ActionID.HellsIngress)
    {
        StatusProvide = new[] { StatusID.EnhancedHarpe },
        ActionCheck = b => !Player.HasStatus(true, StatusID.Bind1, StatusID.Bind2)
    };

    public static IBaseAction HellsEgress { get; } = new BaseAction(ActionID.HellsEgress)
    {
        StatusProvide = HellsIngress.StatusProvide,
        ActionCheck = HellsIngress.ActionCheck
    };

    public static IBaseAction SoulSow { get; } = new BaseAction(ActionID.SoulSow)
    {
        StatusProvide = new[] { StatusID.SoulSow },
        ActionCheck = b => !InCombat,
    };

    public static IBaseAction HarvestMoon { get; } = new BaseAction(ActionID.HarvestMoon)
    {
        StatusNeed = new[] { StatusID.SoulSow },
    };

    public static IBaseAction ArcaneCrest { get; } = new BaseAction(ActionID.ArcaneCrest, ActionOption.Defense);
    #endregion

    [RotationDesc(ActionID.HellsIngress)]
    protected sealed override bool MoveForwardAbility(out IAction act)
    {
        if (HellsIngress.CanUse(out act)) return true;
        return base.MoveForwardAbility(out act);
    }

    [RotationDesc(ActionID.Feint)]
    protected sealed override bool DefenseAreaAbility(out IAction act)
    {
        if (!SoulReaver && !Enshrouded && Feint.CanUse(out act)) return true;
        return base.DefenseAreaAbility(out act);
    }

    [RotationDesc(ActionID.ArcaneCrest)]
    protected override bool DefenseSingleAbility(out IAction act)
    {
        if (!SoulReaver && !Enshrouded && ArcaneCrest.CanUse(out act)) return true;
        return base.DefenseSingleAbility(out act);
    }
}
