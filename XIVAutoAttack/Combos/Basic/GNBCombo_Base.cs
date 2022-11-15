using Dalamud.Game.ClientState.JobGauge.Types;
using System;
using XIVAutoAttack.Actions;
using XIVAutoAttack.Actions.BaseAction;
using XIVAutoAttack.Combos.CustomCombo;
using XIVAutoAttack.Data;
using XIVAutoAttack.Helpers;

namespace XIVAutoAttack.Combos.Basic;


internal abstract class GNBCombo_Base<TCmd> : CustomCombo<TCmd> where TCmd : Enum
{
    protected static GNBGauge JobGauge => Service.JobGauges.Get<GNBGauge>();

    public sealed override ClassJobID[] JobIDs => new ClassJobID[] { ClassJobID.Gunbreaker };
    internal sealed override bool HaveShield => Player.HasStatus(true, StatusID.RoyalGuard);
    private sealed protected override BaseAction Shield => RoyalGuard;

    protected override bool CanHealSingleSpell => false;
    protected override bool CanHealAreaSpell => false;

    protected static byte MaxAmmo => Level >= 88 ? (byte)3 : (byte)2;

    /// <summary>
    /// ÍõÊÒÇ×ÎÀ
    /// </summary>
    public static BaseAction RoyalGuard { get; } = new(ActionID.RoyalGuard, shouldEndSpecial: true);

    /// <summary>
    /// ÀûÈÐÕ¶
    /// </summary>
    public static BaseAction KeenEdge { get; } = new(ActionID.KeenEdge);

    /// <summary>
    /// ÎÞÇé
    /// </summary>
    public static BaseAction NoMercy { get; } = new(ActionID.NoMercy);

    /// <summary>
    /// ²Ð±©µ¯
    /// </summary>
    public static BaseAction BrutalShell { get; } = new(ActionID.BrutalShell);

    /// <summary>
    /// Î±×°
    /// </summary>
    public static BaseAction Camouflage { get; } = new(ActionID.Camouflage, true)
    {
        OtherCheck = BaseAction.TankDefenseSelf,
    };

    /// <summary>
    /// ¶ñÄ§ÇÐ
    /// </summary>
    public static BaseAction DemonSlice { get; } = new(ActionID.DemonSlice);

    /// <summary>
    /// ÉÁÀ×µ¯
    /// </summary>
    public static BaseAction LightningShot { get; } = new(ActionID.LightningShot);

    /// <summary>
    /// Î£ÏÕÁìÓò
    /// </summary>
    public static BaseAction DangerZone { get; } = new(ActionID.DangerZone);

    /// <summary>
    /// Ñ¸Á¬Õ¶
    /// </summary>
    public static BaseAction SolidBarrel { get; } = new(ActionID.SolidBarrel);

    /// <summary>
    /// ±¬·¢»÷
    /// </summary>
    public static BaseAction BurstStrike { get; } = new(ActionID.BurstStrike)
    {
        OtherCheck = b => JobGauge.Ammo > 0,
    };

    /// <summary>
    /// ÐÇÔÆ
    /// </summary>
    public static BaseAction Nebula { get; } = new(ActionID.Nebula, true)
    {
        BuffsProvide = Rampart.BuffsProvide,
        OtherCheck = BaseAction.TankDefenseSelf,
    };

    /// <summary>
    /// ¶ñÄ§É±
    /// </summary>
    public static BaseAction DemonSlaughter { get; } = new(ActionID.DemonSlaughter);

    /// <summary>
    /// ¼«¹â
    /// </summary>
    public static BaseAction Aurora { get; } = new BaseAction(ActionID.Aurora, true);

    /// <summary>
    /// ³¬»ðÁ÷ÐÇ
    /// </summary>
    public static BaseAction Superbolide { get; } = new(ActionID.Superbolide, true);

    /// <summary>
    /// ÒôËÙÆÆ
    /// </summary>
    public static BaseAction SonicBreak { get; } = new(ActionID.SonicBreak);

    /// <summary>
    /// ´Ö·ÖÕ¶
    /// </summary>
    public static BaseAction RoughDivide { get; } = new(ActionID.RoughDivide, shouldEndSpecial: true)
    {
        ChoiceTarget = TargetFilter.FindTargetForMoving
    };

    /// <summary>
    /// ÁÒÑÀ
    /// </summary>
    public static BaseAction GnashingFang { get; } = new(ActionID.GnashingFang)
    {
        OtherCheck = b => JobGauge.AmmoComboStep == 0 && JobGauge.Ammo > 0,
    };

    /// <summary>
    /// ¹­ÐÎ³å²¨
    /// </summary>
    public static BaseAction BowShock { get; } = new(ActionID.BowShock);

    /// <summary>
    /// ¹âÖ®ÐÄ
    /// </summary>
    public static BaseAction HeartofLight { get; } = new(ActionID.HeartofLight, true);

    /// <summary>
    /// Ê¯Ö®ÐÄ
    /// </summary>
    public static BaseAction HeartofStone { get; } = new(ActionID.HeartofStone, true)
    {
        ChoiceTarget = TargetFilter.FindAttackedTarget,
    };

    /// <summary>
    /// ÃüÔËÖ®»·
    /// </summary>
    public static BaseAction FatedCircle { get; } = new(ActionID.FatedCircle)
    {
        OtherCheck = b => JobGauge.Ammo > 0,
    };

    /// <summary>
    /// ÑªÈÀ
    /// </summary>
    public static BaseAction Bloodfest { get; } = new(ActionID.Bloodfest, true)
    {
        OtherCheck = b =>¡¡MaxAmmo - JobGauge.Ammo > 1,
    };

    /// <summary>
    /// ±¶¹¥
    /// </summary>
    public static BaseAction DoubleDown { get; } = new(ActionID.DoubleDown)
    {
        OtherCheck = b => JobGauge.Ammo > 1,
    };

    /// <summary>
    /// ÃÍÊÞ×¦
    /// </summary>
    public static BaseAction SavageClaw { get; } = new(ActionID.SavageClaw)
    {
        OtherCheck = b => Service.IconReplacer.OriginalHook(ActionID.GnashingFang) == ActionID.SavageClaw,
    };

    /// <summary>
    /// Ð×ÇÝ×¦
    /// </summary>
    public static BaseAction WickedTalon { get; } = new(ActionID.WickedTalon)
    {
        OtherCheck = b => Service.IconReplacer.OriginalHook(ActionID.GnashingFang) 
        == ActionID.WickedTalon,
    };

    /// <summary>
    /// Ëººí
    /// </summary>
    public static BaseAction JugularRip { get; } = new(ActionID.JugularRip)
    {
        OtherCheck = b => Service.IconReplacer.OriginalHook(ActionID.Continuation)
        == ActionID.JugularRip,
    };

    /// <summary>
    /// ÁÑÌÅ
    /// </summary>
    public static BaseAction AbdomenTear { get; } = new(ActionID.AbdomenTear)
    {
        OtherCheck = b => Service.IconReplacer.OriginalHook(ActionID.Continuation)
        == ActionID.AbdomenTear,
    };

    /// <summary>
    /// ´©Ä¿
    /// </summary>
    public static BaseAction EyeGouge { get; } = new(ActionID.EyeGouge)
    {
        OtherCheck = b => Service.IconReplacer.OriginalHook(ActionID.Continuation)
        == ActionID.EyeGouge,
    };

    /// <summary>
    /// ³¬¸ßËÙ
    /// </summary>
    public static BaseAction Hypervelocity { get; } = new(ActionID.Hypervelocity)
    {
        OtherCheck = b => Service.IconReplacer.OriginalHook(ActionID.Continuation)
        == ActionID.Hypervelocity,
    };

    private protected override bool EmergercyAbility(byte abilityRemain, IAction nextGCD, out IAction act)
    {
        //³¬»ðÁ÷ÐÇ Èç¹ûÐ»²»¹»ÁË¡£
        if (Superbolide.ShouldUse(out act) && BaseAction.TankBreakOtherCheck(Superbolide.Target)) return true;
        return base.EmergercyAbility(abilityRemain, nextGCD, out act);
    }
}

