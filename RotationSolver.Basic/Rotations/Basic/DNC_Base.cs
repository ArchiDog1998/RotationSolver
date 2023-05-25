using ECommons.DalamudServices;
using RotationSolver.Basic.Traits;

namespace RotationSolver.Basic.Rotations.Basic;
public abstract class DNC_Base : CustomRotation
{
    public override MedicineType MedicineType => MedicineType.Dexterity;
    public sealed override ClassJobID[] JobIDs => new ClassJobID[] { ClassJobID.Dancer };

    #region Job Gauge
    static DNCGauge JobGauge => Svc.Gauges.Get<DNCGauge>();

    protected static bool IsDancing => JobGauge.IsDancing;

    protected static byte Esprit => JobGauge.Esprit;

    protected static byte Feathers => JobGauge.Feathers;

    protected static byte CompletedSteps => JobGauge.CompletedSteps;
    #endregion

    #region Attack Single
    /// <summary>
    /// 1
    /// </summary>
    public static IBaseAction Cascade { get; } = new BaseAction(ActionID.Cascade)
    {
        StatusProvide = new[] { StatusID.SilkenSymmetry }
    };

    /// <summary>
    /// 2
    /// </summary>
    public static IBaseAction Fountain { get; } = new BaseAction(ActionID.Fountain)
    {
        StatusProvide = new[] { StatusID.SilkenFlow }
    };

    /// <summary>
    /// 3
    /// </summary>
    public static IBaseAction ReverseCascade { get; } = new BaseAction(ActionID.ReverseCascade)
    {
        StatusNeed = new[] { StatusID.SilkenSymmetry, StatusID.SilkenSymmetry2 },
    };

    /// <summary>
    /// 4
    /// </summary>
    public static IBaseAction FountainFall { get; } = new BaseAction(ActionID.FountainFall)
    {
        StatusNeed = new[] { StatusID.SilkenFlow, StatusID.SilkenFlow2 }
    };

    public static IBaseAction FanDance { get; } = new BaseAction(ActionID.FanDance)
    {
        ActionCheck = (b, m) => Feathers > 0,
        StatusProvide = new[] { StatusID.ThreefoldFanDance },
    };
    #endregion

    #region Attack Area
    /// <summary>
    /// 1
    /// </summary>
    public static IBaseAction Windmill { get; } = new BaseAction(ActionID.Windmill)
    {
        StatusProvide = Cascade.StatusProvide,
    };

    /// <summary>
    /// 2
    /// </summary>
    public static IBaseAction BladeShower { get; } = new BaseAction(ActionID.BladeShower)
    {
        StatusProvide = Fountain.StatusProvide,
    };

    /// <summary>
    /// 3
    /// </summary>
    public static IBaseAction RisingWindmill { get; } = new BaseAction(ActionID.RisingWindmill)
    {
        StatusNeed = ReverseCascade.StatusNeed,
    };

    /// <summary>
    /// 4
    /// </summary>
    public static IBaseAction BloodShower { get; } = new BaseAction(ActionID.BloodShower)
    {
        AOECount = 2,
        StatusNeed = FountainFall.StatusNeed,
    };

    public static IBaseAction FanDance2 { get; } = new BaseAction(ActionID.FanDance2)
    {
        ActionCheck = (b, m) => Feathers > 0,
        AOECount = 2,
        StatusProvide = new[] { StatusID.ThreefoldFanDance },
    };

    public static IBaseAction FanDance3 { get; } = new BaseAction(ActionID.FanDance3)
    {
        StatusNeed = FanDance2.StatusProvide,
    };

    public static IBaseAction FanDance4 { get; } = new BaseAction(ActionID.FanDance4)
    {
        StatusNeed = new[] { StatusID.FourfoldFanDance },
    };

    public static IBaseAction SaberDance { get; } = new BaseAction(ActionID.SaberDance)
    {
        ActionCheck = (b, m) => Esprit >= 50,
    };

    public static IBaseAction StarFallDance { get; } = new BaseAction(ActionID.StarFallDance)
    {
        StatusNeed = new[] { StatusID.FlourishingStarfall },
    };

    public static IBaseAction Tillana { get; } = new BaseAction(ActionID.Tillana)
    {
        StatusNeed = new[] { StatusID.FlourishingFinish },
    };
    #endregion

    #region Support
    /// <summary>
    /// Moving
    /// </summary>
    public static IBaseAction EnAvant { get; } = new BaseAction(ActionID.EnAvant, ActionOption.Heal | ActionOption.EndSpecial);

    public static IBaseAction ShieldSamba { get; } = new BaseAction(ActionID.ShieldSamba, ActionOption.Defense)
    {
        ActionCheck = (b, m) => !Player.HasStatus(false, StatusID.Troubadour,
            StatusID.Tactician1,
            StatusID.Tactician2,
            StatusID.ShieldSamba),
    };

    public static IBaseAction CuringWaltz { get; } = new BaseAction(ActionID.CuringWaltz, ActionOption.Heal);

    public static IBaseAction Improvisation { get; } = new BaseAction(ActionID.Improvisation, ActionOption.Heal);

    public static IBaseAction ClosedPosition { get; } = new BaseAction(ActionID.ClosedPosition, ActionOption.Buff)
    {
        ChoiceTarget = (Targets, mustUse) =>
        {
            Targets = Targets.Where(b => b.ObjectId != Player.ObjectId && b.CurrentHp != 0 &&
            //Remove Weak
            !b.HasStatus(false, StatusID.Weakness, StatusID.BrinkOfDeath)
            //Remove other partner.
            && (!b.HasStatus(false, StatusID.ClosedPosition2) || b.HasStatus(true, StatusID.ClosedPosition2)));

            return Targets.GetJobCategory(JobRole.Melee, JobRole.RangedMagical, JobRole.RangedPhysical).FirstOrDefault();
        },
    };

    public static IBaseAction Devilment { get; } = new BaseAction(ActionID.Devilment);

    public static IBaseAction Flourish { get; } = new BaseAction(ActionID.Flourish)
    {
        StatusNeed = new[] { StatusID.StandardFinish },
        StatusProvide = new[]
        {
            StatusID.ThreefoldFanDance,
            StatusID.FourfoldFanDance,
        },
        ActionCheck = (b, m) => InCombat,
    };
    #endregion

    #region Step
    public static IBaseAction StandardStep { get; } = new BaseAction(ActionID.StandardStep)
    {
        StatusProvide = new[]
        {
            StatusID.StandardStep,
            StatusID.TechnicalStep,
        },
    };

    public static IBaseAction TechnicalStep { get; } = new BaseAction(ActionID.TechnicalStep)
    {
        StatusNeed = new[]
        {
            StatusID.StandardFinish,
        },
        StatusProvide = StandardStep.StatusProvide,
    };

    protected static IBaseAction StandardFinish { get; } = new BaseAction(ActionID.StandardFinish)
    {
        StatusNeed = new[] { StatusID.StandardStep },
        ActionCheck = (b, m) => IsDancing && CompletedSteps == 2,
    };

    protected static IBaseAction TechnicalFinish { get; } = new BaseAction(ActionID.TechnicalFinish)
    {
        StatusNeed = new[] { StatusID.TechnicalStep },
        ActionCheck = (b, m) => IsDancing && CompletedSteps == 4,
    };

    private static IBaseAction Emboite { get; } = new BaseAction(ActionID.Emboite)
    {
        ActionCheck = (b, m) => (ActionID)JobGauge.NextStep == ActionID.Emboite,
    };

    private static IBaseAction Entrechat { get; } = new BaseAction(ActionID.Entrechat)
    {
        ActionCheck = (b, m) => (ActionID)JobGauge.NextStep == ActionID.Entrechat,
    };

    private static IBaseAction Jete { get; } = new BaseAction(ActionID.Jete)
    {
        ActionCheck = (b, m) => (ActionID)JobGauge.NextStep == ActionID.Jete,
    };

    private static IBaseAction Pirouette { get; } = new BaseAction(ActionID.Pirouette)
    {
        ActionCheck = (b, m) => (ActionID)JobGauge.NextStep == ActionID.Pirouette,
    };

    protected static bool ExecuteStepGCD(out IAction act)
    {
        act = null;
        if (!Player.HasStatus(true, StatusID.StandardStep, StatusID.TechnicalStep)) return false;
        if (Player.HasStatus(true, StatusID.StandardStep) && CompletedSteps == 2) return false;
        if (Player.HasStatus(true, StatusID.TechnicalStep) && CompletedSteps == 4) return false;

        if (Emboite.CanUse(out act)) return true;
        if (Entrechat.CanUse(out act)) return true;
        if (Jete.CanUse(out act)) return true;
        if (Pirouette.CanUse(out act)) return true;
        return false;
    }
    #endregion

    #region Traits
    protected static IBaseTrait IncreasedActionDamage { get; } = new BaseTrait(251);
    protected static IBaseTrait FourfoldFantasy { get; } = new BaseTrait(252);
    protected static IBaseTrait IncreasedActionDamage2 { get; } = new BaseTrait(253);
    protected static IBaseTrait EnhancedEnAvant    { get; } = new BaseTrait(254);
    protected static IBaseTrait EspritTrait { get; } = new BaseTrait(255);
    protected static IBaseTrait EnhancedEnAvant2    { get; } = new BaseTrait(256);
    protected static IBaseTrait EnhancedTechnicalFinish    { get; } = new BaseTrait(453);
    protected static IBaseTrait EnhancedEsprit    { get; } = new BaseTrait(454);
    protected static IBaseTrait EnhancedFlourish    { get; } = new BaseTrait(455);
    protected static IBaseTrait EnhancedShieldSamba    { get; } = new BaseTrait(456);
    protected static IBaseTrait EnhancedDevilment    { get; } = new BaseTrait(457);

    #endregion
    [RotationDesc(ActionID.EnAvant)]
    protected sealed override bool MoveForwardAbility(out IAction act)
    {
        if (EnAvant.CanUse(out act)) return true;
        return false;
    }

    [RotationDesc(ActionID.CuringWaltz, ActionID.Improvisation)]
    protected sealed override bool HealAreaAbility(out IAction act)
    {
        if (CuringWaltz.CanUse(out act, CanUseOption.EmptyOrSkipCombo)) return true;
        if (Improvisation.CanUse(out act, CanUseOption.EmptyOrSkipCombo)) return true;
        return false;
    }

    [RotationDesc(ActionID.ShieldSamba)]
    protected sealed override bool DefenseAreaAbility(out IAction act)
    {
        if (ShieldSamba.CanUse(out act, CanUseOption.EmptyOrSkipCombo)) return true;
        return false;
    }
}
