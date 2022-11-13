using Dalamud.Game.ClientState.JobGauge.Types;
using System;
using XIVAutoAttack.Actions;
using XIVAutoAttack.Actions.BaseAction;
using XIVAutoAttack.Combos.CustomCombo;
using XIVAutoAttack.Data;
using XIVAutoAttack.Helpers;
using XIVAutoAttack.Updaters;

namespace XIVAutoAttack.Combos.Basic;

internal abstract class WARCombo_Base<TCmd> : JobGaugeCombo<WARGauge, TCmd> where TCmd : Enum
{

    public sealed override ClassJobID[] JobIDs => new ClassJobID[] { ClassJobID.Warrior, ClassJobID.Marauder };
    internal sealed override bool HaveShield => Player.HaveStatus(true, StatusID.Defiance);
    private sealed protected override BaseAction Shield => Defiance;

    /// <summary>
    /// ÊØ»¤
    /// </summary>
    public static BaseAction Defiance { get; } = new(ActionID.Defiance, shouldEndSpecial: true);

    /// <summary>
    /// ÖØÅü
    /// </summary>
    public static BaseAction HeavySwing { get; } = new(ActionID.HeavySwing);

    /// <summary>
    /// Ð×²ÐÁÑ
    /// </summary>
    public static BaseAction Maim { get; } = new(ActionID.Maim);

    /// <summary>
    /// ±©·çÕ¶ ÂÌ¸«
    /// </summary>
    public static BaseAction StormsPath { get; } = new(ActionID.StormsPath);

    /// <summary>
    /// ±©·çËé ºì¸«
    /// </summary>
    public static BaseAction StormsEye { get; } = new(ActionID.StormsEye)
    {
        OtherCheck = b => Player.WillStatusEndGCD(3, 0, true, StatusID.SurgingTempest),
    };

    /// <summary>
    /// ·É¸«
    /// </summary>
    public static BaseAction Tomahawk { get; } = new(ActionID.Tomahawk)
    {
        FilterForTarget = b => TargetFilter.ProvokeTarget(b),
    };

    /// <summary>
    /// ÃÍ¹¥
    /// </summary>
    public static BaseAction Onslaught { get; } = new(ActionID.Onslaught, shouldEndSpecial: true)
    {
        ChoiceTarget = TargetFilter.FindTargetForMoving,
    };

    /// <summary>
    /// ¶¯ÂÒ    
    /// </summary>
    public static BaseAction Upheaval { get; } = new(ActionID.Upheaval)
    {
        BuffsNeed = new StatusID[] { StatusID.SurgingTempest },
    };

    /// <summary>
    /// ³¬Ñ¹¸«
    /// </summary>
    public static BaseAction Overpower { get; } = new(ActionID.Overpower);

    /// <summary>
    /// ÃØÒø±©·ç
    /// </summary>
    public static BaseAction MythrilTempest = new(ActionID.MythrilTempest);

    /// <summary>
    /// ÈºÉ½Â¡Æð
    /// </summary>
    public static BaseAction Orogeny { get; } = new(ActionID.Orogeny);

    /// <summary>
    /// Ô­³õÖ®»ê
    /// </summary>
    public static BaseAction InnerBeast { get; } = new(ActionID.InnerBeast)
    {
        OtherCheck = b => !Player.WillStatusEndGCD(3, 0, true, StatusID.SurgingTempest) && (JobGauge.BeastGauge >= 50 || Player.HaveStatus(true, StatusID.InnerRelease)),
    };

    /// <summary>
    /// ¸ÖÌúÐý·ç
    /// </summary>
    public static BaseAction SteelCyclone { get; } = new(ActionID.SteelCyclone)
    {
        OtherCheck = InnerBeast.OtherCheck,
    };

    /// <summary>
    /// Õ½º¿
    /// </summary>
    public static BaseAction Infuriate { get; } = new(ActionID.Infuriate)
    {
        BuffsProvide = new[] { StatusID.InnerRelease },
        OtherCheck = b => TargetFilter.GetObjectInRadius(TargetUpdater.HostileTargets, 5).Length > 0 && JobGauge.BeastGauge < 50,
    };
    /// <summary>
    /// ¿ñ±©
    /// </summary>
    public static BaseAction Berserk { get; } = new(ActionID.Berserk)
    {
        OtherCheck = b => TargetFilter.GetObjectInRadius(TargetUpdater.HostileTargets, 5).Length > 0,
    };

    /// <summary>
    /// Õ½Àõ
    /// </summary>
    public static BaseAction ThrillofBattle { get; } = new(ActionID.ThrillofBattle);

    /// <summary>
    /// Ì©È»×ÔÈô
    /// </summary>
    public static BaseAction Equilibrium { get; } = new(ActionID.Equilibrium);

    /// <summary>
    /// Ô­³õµÄÓÂÃÍ
    /// </summary>
    public static BaseAction NascentFlash { get; } = new(ActionID.NascentFlash)
    {
        ChoiceTarget = TargetFilter.FindAttackedTarget,
    };

    /// <summary>
    /// ¸´³ð
    /// </summary>
    public static BaseAction Vengeance { get; } = new(ActionID.Vengeance)
    {
        BuffsProvide = Rampart.BuffsProvide,
        OtherCheck = BaseAction.TankDefenseSelf,
    };

    /// <summary>
    /// Ô­³õµÄÖ±¾õ
    /// </summary>
    public static BaseAction RawIntuition { get; } = new(ActionID.RawIntuition)
    {
        BuffsProvide = Rampart.BuffsProvide,
        OtherCheck = BaseAction.TankDefenseSelf,
    };

    /// <summary>
    /// °ÚÍÑ
    /// </summary>
    public static BaseAction ShakeItOff { get; } = new(ActionID.ShakeItOff, true);
    
    /// <summary>
    /// ËÀ¶·
    /// </summary>
    public static BaseAction Holmgang { get; } = new(ActionID.Holmgang);

    /// <summary>
    /// Âù»Ä±ÀÁÑ
    /// </summary>
    public static BaseAction PrimalRend { get; } = new(ActionID.PrimalRend)
    {
        BuffsNeed = new[] { StatusID.PrimalRendReady }
    };

    private protected override bool EmergercyAbility(byte abilityRemain, IAction nextGCD, out IAction act)
    {
        //ËÀ¶· Èç¹ûÑª²»¹»ÁË¡£
        if (Holmgang.ShouldUse(out act) && BaseAction.TankBreakOtherCheck(Holmgang.Target)) return true;

        return base.EmergercyAbility(abilityRemain, nextGCD, out act);
    }
}
