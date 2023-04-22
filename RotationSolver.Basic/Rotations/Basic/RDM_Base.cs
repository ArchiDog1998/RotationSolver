namespace RotationSolver.Basic.Rotations.Basic;

public abstract class RDM_Base : CustomRotation
{
    public override MedicineType MedicineType => MedicineType.Intelligence;
    public sealed override ClassJobID[] JobIDs => new ClassJobID[] { ClassJobID.RedMage };
    protected override bool CanHealSingleSpell => DataCenter.PartyMembers.Count() == 1 && base.CanHealSingleSpell;

    #region Job Gauge
    static RDMGauge JobGauge => Service.JobGauges.Get<RDMGauge>();

    protected static byte WhiteMana => JobGauge.WhiteMana;

    protected static byte BlackMana => JobGauge.BlackMana;

    protected static byte ManaStacks => JobGauge.ManaStacks;

    #endregion

    #region Attack Single
    public static IBaseAction Jolt { get; } = new BaseAction(ActionID.Jolt)
    {
        StatusProvide = Swiftcast.StatusProvide.Union(new[] { StatusID.Acceleration }).ToArray(),
    };

    public static IBaseAction Verfire { get; } = new BaseAction(ActionID.Verfire)
    {
        StatusNeed = new[] { StatusID.VerfireReady },
        StatusProvide = Jolt.StatusProvide,
    };

    public static IBaseAction Verstone { get; } = new BaseAction(ActionID.Verstone)
    {
        StatusNeed = new[] { StatusID.VerstoneReady },
        StatusProvide = Jolt.StatusProvide,
    };


    public static IBaseAction Verthunder { get; } = new BaseAction(ActionID.Verthunder)
    {
        StatusNeed = Jolt.StatusProvide,
    };
    public static IBaseAction Veraero { get; } = new BaseAction(ActionID.Veraero)
    {
        StatusNeed = Jolt.StatusProvide,
    };

    public static IBaseAction Riposte { get; } = new BaseAction(ActionID.Riposte)
    {
        ActionCheck = b => JobGauge.BlackMana >= 20 && JobGauge.WhiteMana >= 20,
    };
    public static IBaseAction Zwerchhau { get; } = new BaseAction(ActionID.Zwerchhau)
    {
        ActionCheck = b => BlackMana >= 15 && WhiteMana >= 15,
    };

    public static IBaseAction Redoublement { get; } = new BaseAction(ActionID.Redoublement)
    {
        ActionCheck = b => BlackMana >= 15 && WhiteMana >= 15,
    };

    public static IBaseAction Engagement { get; } = new BaseAction(ActionID.Engagement);
    public static IBaseAction Fleche { get; } = new BaseAction(ActionID.Fleche);

    public static IBaseAction CorpsACorps { get; } = new BaseAction(ActionID.CorpsACorps, ActionOption.EndSpecial)
    {
        ChoiceTarget = TargetFilter.FindTargetForMoving,
    };
    #endregion

    #region Attack Area
    public static IBaseAction Scatter { get; } = new BaseAction(ActionID.Scatter)
    {
        StatusNeed = Jolt.StatusProvide,
        AOECount = 2,
    };
    public static IBaseAction Verthunder2 { get; } = new BaseAction(ActionID.Verthunder2)
    {
        StatusProvide = Jolt.StatusProvide,
    };

    public static IBaseAction Veraero2 { get; } = new BaseAction(ActionID.Veraero2)
    {
        StatusProvide = Jolt.StatusProvide,
    };

    public static IBaseAction Moulinet { get; } = new BaseAction(ActionID.Moulinet)
    {
        ActionCheck = b => BlackMana >= 20 && WhiteMana >= 20,
    };

    public static IBaseAction Verflare { get; } = new BaseAction(ActionID.Verflare);

    public static IBaseAction Verholy { get; } = new BaseAction(ActionID.Verholy);

    public static IBaseAction Scorch { get; } = new BaseAction(ActionID.Scorch)
    {
        ComboIds = new[] { ActionID.Verholy },
    };

    public static IBaseAction Resolution { get; } = new BaseAction(ActionID.Resolution);

    public static IBaseAction ContreSixte { get; } = new BaseAction(ActionID.ContreSixte);

    #endregion

    #region Support
    private sealed protected override IBaseAction Raise => Verraise;
    public static IBaseAction Verraise { get; } = new BaseAction(ActionID.Verraise, ActionOption.Friendly);

    public static IBaseAction Acceleration { get; } = new BaseAction(ActionID.Acceleration, ActionOption.Buff)
    {
        StatusProvide = new[] { StatusID.Acceleration },
    };


    public static IBaseAction Vercure { get; } = new BaseAction(ActionID.Vercure, ActionOption.Heal)
    {
        StatusProvide = Swiftcast.StatusProvide.Union(Acceleration.StatusProvide).ToArray(),
    };

    public static IBaseAction MagickBarrier { get; } = new BaseAction(ActionID.MagickBarrier, ActionOption.Defense);

    public static IBaseAction Embolden { get; } = new BaseAction(ActionID.Embolden, ActionOption.Buff);

    public static IBaseAction Manafication { get; } = new BaseAction(ActionID.Manafication)
    {
        ActionCheck = b => WhiteMana <= 50 && BlackMana <= 50 && InCombat && ManaStacks == 0,
        ComboIdsNot = new[] { ActionID.Riposte, ActionID.Zwerchhau, ActionID.Scorch, ActionID.Verflare, ActionID.Verholy },
    };
    #endregion



    [RotationDesc(ActionID.Vercure)]
    protected sealed override bool HealSingleGCD(out IAction act)
    {
        if (Vercure.CanUse(out act, CanUseOption.MustUse)) return true;
        return base.HealSingleGCD(out act);
    }

    [RotationDesc(ActionID.CorpsACorps)]
    protected sealed override bool MoveForwardAbility(out IAction act)
    {
        if (CorpsACorps.CanUse(out act)) return true;
        return base.MoveForwardAbility(out act);
    }

    [RotationDesc(ActionID.Addle, ActionID.MagickBarrier)]
    protected sealed override bool DefenseAreaAbility(out IAction act)
    {
        if (Addle.CanUse(out act)) return true;
        if (MagickBarrier.CanUse(out act, CanUseOption.MustUse)) return true;
        return base.DefenseAreaAbility(out act);
    }
}