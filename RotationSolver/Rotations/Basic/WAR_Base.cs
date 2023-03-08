using Dalamud.Game.ClientState.JobGauge.Types;
using RotationSolver.Actions;
using RotationSolver.Actions.BaseAction;
using RotationSolver.Attributes;
using RotationSolver.Data;
using RotationSolver.Helpers;
using RotationSolver.Rotations.CustomRotation;

namespace RotationSolver.Rotations.Basic;

internal abstract class WAR_Base : CustomRotation.CustomRotation
{
    private static WARGauge JobGauge => Service.JobGauges.Get<WARGauge>();
    public override MedicineType MedicineType => MedicineType.Strength;


    public sealed override ClassJobID[] JobIDs => new ClassJobID[] { ClassJobID.Warrior, ClassJobID.Marauder };
    private sealed protected override IBaseAction Shield => Defiance;

    /// <summary>
    /// ÊØ»¤
    /// </summary>
    public static IBaseAction Defiance { get; } = new BaseAction(ActionID.Defiance, shouldEndSpecial: true);

    /// <summary>
    /// ÖØÅü
    /// </summary>
    public static IBaseAction HeavySwing { get; } = new BaseAction(ActionID.HeavySwing);

    /// <summary>
    /// Ð×²ÐÁÑ
    /// </summary>
    public static IBaseAction Maim { get; } = new BaseAction(ActionID.Maim);

    /// <summary>
    /// ±©·çÕ¶ ÂÌ¸«
    /// </summary>
    public static IBaseAction StormsPath { get; } = new BaseAction(ActionID.StormsPath);

    /// <summary>
    /// ±©·çËé ºì¸«
    /// </summary>
    public static IBaseAction StormsEye { get; } = new BaseAction(ActionID.StormsEye)
    {
        ActionCheck = b => Player.WillStatusEndGCD(3, 0, true, StatusID.SurgingTempest),
    };

    /// <summary>
    /// ·É¸«
    /// </summary>
    public static IBaseAction Tomahawk { get; } = new BaseAction(ActionID.Tomahawk)
    {
        FilterForHostiles = TargetFilter.TankRangeTarget,
    };

    /// <summary>
    /// ÃÍ¹¥
    /// </summary>
    public static IBaseAction Onslaught { get; } = new BaseAction(ActionID.Onslaught, shouldEndSpecial: true)
    {
        ChoiceTarget = TargetFilter.FindTargetForMoving,
    };

    /// <summary>
    /// ¶¯ÂÒ    
    /// </summary>
    public static IBaseAction Upheaval { get; } = new BaseAction(ActionID.Upheaval)
    {
        StatusNeed = new StatusID[] { StatusID.SurgingTempest },
    };

    /// <summary>
    /// ³¬Ñ¹¸«
    /// </summary>
    public static IBaseAction Overpower { get; } = new BaseAction(ActionID.Overpower);

    /// <summary>
    /// ÃØÒø±©·ç
    /// </summary>
    public static IBaseAction MythrilTempest { get; } = new BaseAction(ActionID.MythrilTempest);

    /// <summary>
    /// ÈºÉ½Â¡Æð
    /// </summary>
    public static IBaseAction Orogeny { get; } = new BaseAction(ActionID.Orogeny);

    /// <summary>
    /// Ô­³õÖ®»ê
    /// </summary>
    public static IBaseAction InnerBeast { get; } = new BaseAction(ActionID.InnerBeast)
    {
        ActionCheck = b => JobGauge.BeastGauge >= 50 || Player.HasStatus(true, StatusID.InnerRelease),
    };

    /// <summary>
    /// Ô­³õµÄ½â·Å
    /// </summary>
    public static IBaseAction InnerRelease { get; } = new BaseAction(ActionID.InnerRelease)
    {
        ActionCheck = InnerBeast.ActionCheck,
    };

    /// <summary>
    /// ¸ÖÌúÐý·ç
    /// </summary>
    public static IBaseAction SteelCyclone { get; } = new BaseAction(ActionID.SteelCyclone)
    {
        ActionCheck = InnerBeast.ActionCheck,
    };

    /// <summary>
    /// Õ½º¿
    /// </summary>
    public static IBaseAction Infuriate { get; } = new BaseAction(ActionID.Infuriate)
    {
        StatusProvide = new[] { StatusID.InnerRelease },
        ActionCheck = b => HasHostilesInRange && JobGauge.BeastGauge < 50 && InCombat,
    };

    /// <summary>
    /// ¿ñ±©
    /// </summary>
    public static IBaseAction Berserk { get; } = new BaseAction(ActionID.Berserk)
    {
        ActionCheck = b => HasHostilesInRange && !InnerRelease.IsCoolingDown,
    };

    /// <summary>
    /// Õ½Àõ
    /// </summary>
    public static IBaseAction ThrillofBattle { get; } = new BaseAction(ActionID.ThrillofBattle, true, isTimeline: true);

    /// <summary>
    /// Ì©È»×ÔÈô
    /// </summary>
    public static IBaseAction Equilibrium { get; } = new BaseAction(ActionID.Equilibrium, true, isTimeline: true);

    /// <summary>
    /// Ô­³õµÄÓÂÃÍ
    /// </summary>
    public static IBaseAction NascentFlash { get; } = new BaseAction(ActionID.NascentFlash, isTimeline: true)
    {
        ChoiceTarget = TargetFilter.FindAttackedTarget,
    };

    /// <summary>
    /// ¸´³ð
    /// </summary>
    public static IBaseAction Vengeance { get; } = new BaseAction(ActionID.Vengeance, isTimeline: true)
    {
        StatusProvide = Rampart.StatusProvide,
        ActionCheck = BaseAction.TankDefenseSelf,
    };

    /// <summary>
    /// Ô­³õµÄÖ±¾õ
    /// </summary>
    public static IBaseAction RawIntuition { get; } = new BaseAction(ActionID.RawIntuition, isTimeline: true)
    {
        ActionCheck = BaseAction.TankDefenseSelf,
    };

    /// <summary>
    /// °ÚÍÑ
    /// </summary>
    public static IBaseAction ShakeItOff { get; } = new BaseAction(ActionID.ShakeItOff, true, isTimeline: true);

    /// <summary>
    /// ËÀ¶·
    /// </summary>
    public static IBaseAction Holmgang { get; } = new BaseAction(ActionID.Holmgang, isTimeline: true)
    {
        ChoiceTarget = (tars, mustUse) => Player,
    };

    /// <summary>
    /// Âù»Ä±ÀÁÑ
    /// </summary>
    public static IBaseAction PrimalRend { get; } = new BaseAction(ActionID.PrimalRend)
    {
        StatusNeed = new[] { StatusID.PrimalRendReady }
    };

    protected override bool EmergencyAbility(byte abilitiesRemaining, IAction nextGCD, out IAction act)
    {
        //ËÀ¶· Èç¹ûÑª²»¹»ÁË¡£
        if (Holmgang.CanUse(out act) && BaseAction.TankBreakOtherCheck(JobIDs[0])) return true;
        return base.EmergencyAbility(abilitiesRemaining, nextGCD, out act);
    }

    [RotationDesc(ActionID.Onslaught)]
    protected sealed override bool MoveForwardAbility(byte abilitiesRemaining, out IAction act, bool recordTarget = true)
    {
        if (Onslaught.CanUse(out act, emptyOrSkipCombo: true, recordTarget: recordTarget)) return true;
        return false;
    }
}
