using Dalamud.Game.ClientState.JobGauge.Types;
using System;
using XIVAutoAttack.Actions.BaseAction;
using XIVAutoAttack.Combos.CustomCombo;
using XIVAutoAttack.Data;
using XIVAutoAttack.Helpers;

namespace XIVAutoAttack.Combos.Basic;


internal abstract class GNBCombo_Base<TCmd> : JobGaugeCombo<GNBGauge, TCmd> where TCmd : Enum
{
    public sealed override ClassJobID[] JobIDs => new ClassJobID[] { ClassJobID.Gunbreaker };
    internal sealed override bool HaveShield => Player.HaveStatus(StatusID.RoyalGuard);
    private sealed protected override BaseAction Shield => RoyalGuard;

    protected override bool CanHealSingleSpell => false;
    protected override bool CanHealAreaSpell => false;


    public static readonly BaseAction
        //ÍõÊÒÇ×ÎÀ
        RoyalGuard = new(16142, shouldEndSpecial: true),

        //ÀûÈÐÕ¶
        KeenEdge = new(16137),

        //ÎÞÇé
        NoMercy = new(16138),

        //²Ð±©µ¯
        BrutalShell = new(16139),

        //Î±×°
        Camouflage = new(16140)
        {
            BuffsProvide = Rampart.BuffsProvide,
            OtherCheck = BaseAction.TankDefenseSelf,
        },

        //¶ñÄ§ÇÐ
        DemonSlice = new(16141),

        //ÉÁÀ×µ¯
        LightningShot = new(16143),

        //Î£ÏÕÁìÓò
        DangerZone = new(16144),

        //Ñ¸Á¬Õ¶
        SolidBarrel = new(16145),

        //±¬·¢»÷
        BurstStrike = new(16162)
        {
            OtherCheck = b => JobGauge.Ammo > 0,
        },

        //ÐÇÔÆ
        Nebula = new(16148)
        {
            BuffsProvide = Rampart.BuffsProvide,
            OtherCheck = BaseAction.TankDefenseSelf,
        },

        //¶ñÄ§É±
        DemonSlaughter = new(16149),

        //¼«¹â
        Aurora = new BaseAction(16151, true)
        {
            BuffsProvide = new[] { StatusID.Aurora },
        },

        //³¬»ðÁ÷ÐÇ
        Superbolide = new(16152)
        {
            OtherCheck = BaseAction.TankBreakOtherCheck,
        },

        //ÒôËÙÆÆ
        SonicBreak = new(16153),

        //´Ö·ÖÕ¶
        RoughDivide = new(16154, shouldEndSpecial: true)
        {
            ChoiceTarget = TargetFilter.FindTargetForMoving
        },

        //ÁÒÑÀ
        GnashingFang = new(16146)
        {
            OtherCheck = b => JobGauge.AmmoComboStep == 0 && JobGauge.Ammo > 0,
        },

        //¹­ÐÎ³å²¨
        BowShock = new(16159),

        //¹âÖ®ÐÄ
        HeartofLight = new(16160, true),

        //Ê¯Ö®ÐÄ
        HeartofStone = new(16161, true)
        {
            BuffsProvide = Rampart.BuffsProvide,
            ChoiceTarget = TargetFilter.FindAttackedTarget,
        },

        //ÃüÔËÖ®»·
        FatedCircle = new(16163)
        {
            OtherCheck = b => JobGauge.Ammo > (Level >= 88 ? 2 : 1),
        },

        //ÑªÈÀ
        Bloodfest = new(16164)
        {
            OtherCheck = b => JobGauge.Ammo == 0,
        },

        //±¶¹¥
        DoubleDown = new(25760)
        {
            OtherCheck = b => JobGauge.Ammo >= 2,
        },

        //ÃÍÊÞ×¦
        SavageClaw = new(16147)
        {
            OtherCheck = b => Service.IconReplacer.OriginalHook(GnashingFang.ID) == SavageClaw.ID,
        },

        //Ð×ÇÝ×¦
        WickedTalon = new(16150)
        {
            OtherCheck = b => Service.IconReplacer.OriginalHook(GnashingFang.ID) == WickedTalon.ID,
        },

        //Ëººí
        JugularRip = new(16156)
        {
            OtherCheck = b => Service.IconReplacer.OriginalHook(16155) == JugularRip.ID,
        },

        //ÁÑÌÅ
        AbdomenTear = new(16157)
        {
            OtherCheck = b => Service.IconReplacer.OriginalHook(16155) == AbdomenTear.ID,
        },

        //´©Ä¿
        EyeGouge = new(16158)
        {
            OtherCheck = b => Service.IconReplacer.OriginalHook(16155) == EyeGouge.ID,
        },

        //³¬¸ßËÙ
        Hypervelocity = new(25759)
        {
            OtherCheck = b => Service.IconReplacer.OriginalHook(16155) == Hypervelocity.ID,
        };
}

