using Dalamud.Game.ClientState.JobGauge.Types;
using System;
using System.Linq;
using RotationSolver.Actions.BaseAction;
using RotationSolver.Updaters;
using RotationSolver.Actions;
using RotationSolver.Helpers;
using RotationSolver.Data;

namespace RotationSolver.Rotations.Basic;

internal abstract class RDM_Base : CustomRotation.CustomRotation
{
    private static RDMGauge JobGauge => Service.JobGauges.Get<RDMGauge>();

    /// <summary>
    /// °×Ä§Ôª
    /// </summary>
    protected static byte WhiteMana => JobGauge.WhiteMana;

    /// <summary>
    /// ºÚÄ§Ôª
    /// </summary>
    protected static byte BlackMana => JobGauge.BlackMana;


    /// <summary>
    /// Ä§¾§Êý
    /// </summary>
    protected static byte ManaStacks => JobGauge.ManaStacks;

    public sealed override ClassJobID[] JobIDs => new ClassJobID[] { ClassJobID.RedMage };
    protected override bool CanHealSingleSpell => TargetUpdater.PartyMembers.Count() == 1 && base.CanHealSingleSpell;

    private sealed protected override IBaseAction Raise => Verraise;

    /// <summary>
    /// ³à¸´»î
    /// </summary>
    public static IBaseAction Verraise { get; } = new BaseAction(ActionID.Verraise, true);

    /// <summary>
    /// Õðµ´
    /// </summary>
    public static IBaseAction Jolt { get; } = new BaseAction(ActionID.Jolt)
    {
        StatusProvide = Swiftcast.StatusProvide.Union(new[] { StatusID.Acceleration }).ToArray(),
    };

    /// <summary>
    /// »Ø´Ì
    /// </summary>
    public static IBaseAction Riposte { get; } = new BaseAction(ActionID.Riposte)
    {
        ActionCheck = b => JobGauge.BlackMana >= 20 && JobGauge.WhiteMana >= 20,
    };

    /// <summary>
    /// ³àÉÁÀ×
    /// </summary>
    public static IBaseAction Verthunder { get; } = new BaseAction(ActionID.Verthunder)
    {
        StatusNeed = Jolt.StatusProvide,
    };

    /// <summary>
    /// ¶Ì±øÏà½Ó
    /// </summary>
    public static IBaseAction CorpsAcorps { get; } = new BaseAction(ActionID.CorpsAcorps, shouldEndSpecial: true)
    {
        ChoiceTarget = TargetFilter.FindTargetForMoving,
    };

    /// <summary>
    /// ³à¼²·ç
    /// </summary>
    public static IBaseAction Veraero { get; } = new BaseAction(ActionID.Veraero)
    {
        StatusNeed = Jolt.StatusProvide,
    };

    /// <summary>
    /// É¢Ëé
    /// </summary>
    public static IBaseAction Scatter { get; } = new BaseAction(ActionID.Scatter)
    {
        StatusNeed = Jolt.StatusProvide,
    };

    /// <summary>
    /// ³àÕðÀ×
    /// </summary>
    public static IBaseAction Verthunder2 { get; } = new BaseAction(ActionID.Verthunder2)
    {
        StatusProvide = Jolt.StatusProvide,
    };

    /// <summary>
    /// ³àÁÒ·ç
    /// </summary>
    public static IBaseAction Veraero2 { get; } = new BaseAction(ActionID.Veraero2)
    {
        StatusProvide = Jolt.StatusProvide,
    };

    /// <summary>
    /// ³à»ðÑ×
    /// </summary>
    public static IBaseAction Verfire { get; } = new BaseAction(ActionID.Verfire)
    {
        StatusNeed = new[] { StatusID.VerfireReady },
        StatusProvide = Jolt.StatusProvide,
    };

    /// <summary>
    /// ³à·ÉÊ¯
    /// </summary>
    public static IBaseAction Verstone { get; } = new BaseAction(ActionID.Verstone)
    {
        StatusNeed = new[] { StatusID.VerstoneReady },
        StatusProvide = Jolt.StatusProvide,
    };

    /// <summary>
    /// ½»»÷Õ¶
    /// </summary>
    public static IBaseAction Zwerchhau { get; } = new BaseAction(ActionID.Zwerchhau)
    {
        ActionCheck = b => BlackMana >= 15 && WhiteMana >= 15,
    };

    /// <summary>
    /// ½»½£
    /// </summary>
    public static IBaseAction Engagement { get; } = new BaseAction(ActionID.Engagement);

    /// <summary>
    /// ·É½£
    /// </summary>
    public static IBaseAction Fleche { get; } = new BaseAction(ActionID.Fleche);

    /// <summary>
    /// Á¬¹¥
    /// </summary>
    public static IBaseAction Redoublement { get; } = new BaseAction(ActionID.Redoublement)
    {
        ActionCheck = b => BlackMana >= 15 && WhiteMana >= 15,
    };


    /// <summary>
    /// ´Ù½ø
    /// </summary>
    public static IBaseAction Acceleration { get; } = new BaseAction(ActionID.Acceleration, true)
    {
        StatusProvide = new[] { StatusID.Acceleration },
    };

    /// <summary>
    /// »®Ô²Õ¶
    /// </summary>
    public static IBaseAction Moulinet { get; } = new BaseAction(ActionID.Moulinet)
    {
        ActionCheck = b => BlackMana >= 20 && WhiteMana >= 20,
    };

    /// <summary>
    /// ³àÖÎÁÆ
    /// </summary>
    public static IBaseAction Vercure { get; } = new BaseAction(ActionID.Vercure, true)
    {
        StatusProvide = Swiftcast.StatusProvide.Union(Acceleration.StatusProvide).ToArray(),
    };

    /// <summary>
    /// Áù·Ö·´»÷
    /// </summary>
    public static IBaseAction ContreSixte { get; } = new BaseAction(ActionID.ContreSixte);

    /// <summary>
    /// ¹ÄÀø
    /// </summary>
    public static IBaseAction Embolden { get; } = new BaseAction(ActionID.Embolden, true);

    /// <summary>
    /// ¿¹ËÀ
    /// </summary>
    public static IBaseAction MagickBarrier { get; } = new BaseAction(ActionID.MagickBarrier, true, isTimeline: true);

    /// <summary>
    /// ³àºË±¬
    /// </summary>
    public static IBaseAction Verflare { get; } = new BaseAction(ActionID.Verflare);

    /// <summary>
    /// ³àÉñÊ¥
    /// </summary>
    public static IBaseAction Verholy { get; } = new BaseAction(ActionID.Verholy);

    /// <summary>
    /// ½¹ÈÈ
    /// </summary>
    public static IBaseAction Scorch { get; } = new BaseAction(ActionID.Scorch)
    {
        ComboIds = new[] { ActionID.Verholy },
    };

    /// <summary>
    /// ¾ö¶Ï
    /// </summary>
    public static IBaseAction Resolution { get; } = new BaseAction(ActionID.Resolution);

    /// <summary>
    /// Ä§Ôª»¯
    /// </summary>
    public static IBaseAction Manafication { get; } = new BaseAction(ActionID.Manafication)
    {
        ActionCheck = b => WhiteMana <= 50 && BlackMana <= 50 && InCombat && ManaStacks == 0,
        ComboIdsNot = new[] { ActionID.Riposte, ActionID.Zwerchhau, ActionID.Scorch, ActionID.Verflare, ActionID.Verholy },
    };

    private protected override bool HealSingleGCD(out IAction act)
    {
        if (Vercure.CanUse(out act, mustUse: true)) return true;
        return false;
    }

    private protected override bool MoveForwardAbility(byte abilityRemain, out IAction act)
    {
        if (CorpsAcorps.CanUse(out act, emptyOrSkipCombo: true)) return true;
        return false;
    }
}
