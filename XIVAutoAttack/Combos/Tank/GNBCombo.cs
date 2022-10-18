using Dalamud.Game.ClientState.JobGauge.Types;
using System.Collections.Generic;
using XIVAutoAttack.Actions;
using XIVAutoAttack.Combos.CustomCombo;

namespace XIVAutoAttack.Combos.Tank;

internal class GNBCombo : JobGaugeCombo<GNBGauge>
{
    internal override uint JobID => 37;
    internal override bool HaveShield => StatusHelper.HaveStatusSelfFromSelf(ObjectStatus.RoyalGuard);
    private protected override PVEAction Shield => Actions.RoyalGuard;

    protected override bool CanHealSingleSpell => false;
    protected override bool CanHealAreaSpell => false;

    internal struct Actions
    {
        public static readonly PVEAction
            //Õı “«◊Œ¿
            RoyalGuard = new (16142, shouldEndSpecial: true),

            //¿˚»–’∂
            KeenEdge = new (16137),

            //Œﬁ«È
            NoMercy = new (16138),

            //≤–±©µØ
            BrutalShell = new (16139),

            //Œ±◊∞
            Camouflage = new (16140)
            {
                BuffsProvide = GeneralActions.Rampart.BuffsProvide,
                OtherCheck = PVEAction.TankDefenseSelf,
            },

            //∂Òƒß«–
            DemonSlice = new (16141),

            //…¡¿◊µØ
            LightningShot = new (16143),

            //Œ£œ’¡Ï”Ú
            DangerZone = new (16144),

            //—∏¡¨’∂
            SolidBarrel = new (16145),

            //±¨∑¢ª˜
            BurstStrike = new (16162),

            //–«‘∆
            Nebula = new (16148)
            {
                BuffsProvide = GeneralActions.Rampart.BuffsProvide,
                OtherCheck = PVEAction.TankDefenseSelf,
            },

            //∂Òƒß…±
            DemonSlaughter = new (16149),

            //º´π‚
            Aurora = new PVEAction(16151, true)
            {
                BuffsProvide = new [] { ObjectStatus.Aurora },
            },

            //≥¨ª¡˜–«
            Superbolide = new (16152)
            {
                OtherCheck = PVEAction.TankBreakOtherCheck,
            },

            //“ÙÀŸ∆∆
            SonicBreak = new (16153),

            //¥÷∑÷’∂
            RoughDivide = new (16154, shouldEndSpecial: true)
            {
                ChoiceTarget = TargetFilter.FindMoveTarget
            },

            //¡“—¿
            GnashingFang = new (16146),

            //π≠–Œ≥Â≤®
            BowShock = new (16159),

            //π‚÷Æ–ƒ
            HeartofLight = new (16160, true),

            // Ø÷Æ–ƒ
            HeartofStone = new (16161, true)
            {
                BuffsProvide = GeneralActions.Rampart.BuffsProvide,
                ChoiceTarget = TargetFilter.FindAttackedTarget,
            },

            //√¸‘À÷Æª∑
            FatedCircle = new (16163),

            //—™»¿
            Bloodfest = new (16164)
            {
                OtherCheck = b => JobGauge.Ammo == 0,
            },

            //±∂π•
            DoubleDown = new (25760),

            //√Õ ﬁ◊¶
            SavageClaw = new (16147),

            //–◊«›◊¶
            WickedTalon = new (16150),

            //À∫∫Ì
            JugularRip = new (16156)
            {
                OtherCheck = b => Service.IconReplacer.OriginalHook(16155) == JugularRip.ID,
            },

            //¡—Ã≈
            AbdomenTear = new (16157)
            {
                OtherCheck = b => Service.IconReplacer.OriginalHook(16155) == AbdomenTear.ID,
            },

            //¥©ƒø
            EyeGouge = new (16158)
            {
                OtherCheck = b => Service.IconReplacer.OriginalHook(16155) == EyeGouge.ID,
            },

            //≥¨∏ﬂÀŸ
            Hypervelocity = new (25759)
            {
                OtherCheck = b => Service.IconReplacer.OriginalHook(16155) == Hypervelocity.ID,
            };
    }
    internal override SortedList<DescType, string> Description => new ()
    {
        {DescType.µ•ÃÂ÷Œ¡∆, $"{Actions.Aurora.Action.Name}"},
        {DescType.∑∂Œß∑¿”˘, $"{Actions.HeartofLight.Action.Name}"},
        {DescType.µ•ÃÂ∑¿”˘, $"{Actions.HeartofStone.Action.Name}, {Actions.Nebula.Action.Name}, {Actions.Camouflage.Action.Name}"},
        {DescType.“∆∂Ø, $"{Actions.RoughDivide.Action.Name}"},
    };
    private protected override bool GeneralGCD(uint lastComboActionID, out IAction act)
    {
        // π”√æßƒ“
        bool useAmmo = JobGauge.Ammo > (Level >= Actions.DoubleDown.Level ? 2 : 1);
        bool breakUseAmmo = JobGauge.Ammo >= (Level >= Actions.DoubleDown.Level ? 2 : 1);

        //AOE
        if (breakUseAmmo && Actions.DoubleDown.ShouldUse(out act, mustUse: true)) return true;
        if (useAmmo && Actions.FatedCircle.ShouldUse(out act)) return true;

        if ( Actions.DemonSlaughter.ShouldUse(out act, lastComboActionID)) return true;
        if ( Actions.DemonSlice.ShouldUse(out act, lastComboActionID)) return true;

        uint remap = Service.IconReplacer.OriginalHook(Actions.GnashingFang.ID);
        if (remap == Actions.WickedTalon.ID && Actions.WickedTalon.ShouldUse(out act)) return true;
        if (remap == Actions.SavageClaw.ID && Actions.SavageClaw.ShouldUse(out act)) return true;


        //µ•ÃÂ
        if (breakUseAmmo && Actions.GnashingFang.ShouldUse(out act)) return true;
        if (useAmmo && Actions.BurstStrike.ShouldUse(out act)) return true;

        if (Actions.SonicBreak.ShouldUse(out act)) return true;

        //µ•ÃÂ»˝¡¨
        if (Actions.SolidBarrel.ShouldUse(out act, lastComboActionID)) return true;
        if (Actions.BrutalShell.ShouldUse(out act, lastComboActionID)) return true;
        if (Actions.KeenEdge.ShouldUse(out act, lastComboActionID)) return true;

        if (IconReplacer.Move && MoveAbility(1, out act)) return true;
        if (Actions.LightningShot.ShouldUse(out act)) return true;

        return false;
    }

    private protected override bool EmergercyAbility(byte abilityRemain, IAction nextGCD, out IAction act)
    {
        //…Ò •¡Ï”Ú »Áπ˚–ª≤ªπª¡À°£
        if (Actions.Superbolide.ShouldUse(out act)) return true;
        return false;
    }

    private protected override bool ForAttachAbility(byte abilityRemain, out IAction act)
    {
        if (Actions.JugularRip.ShouldUse(out act)) return true;
        if (Actions.AbdomenTear.ShouldUse(out act)) return true;
        if (Actions.EyeGouge.ShouldUse(out act)) return true;
        if (Actions.Hypervelocity.ShouldUse(out act)) return true;


        if (Actions.NoMercy.ShouldUse(out act)) return true;
        if (Actions.Bloodfest.ShouldUse(out act)) return true;
        if (Actions.BowShock.ShouldUse(out act, mustUse: true)) return true;
        if (Actions.DangerZone.ShouldUse(out act)) return true;

        //∏„∏„π•ª˜
        if (Actions.RoughDivide.ShouldUse(out act) && !IsMoving)
        {
            if (Actions.RoughDivide.Target.DistanceToPlayer() < 1)
            {
                return true;
            }
        }
        return false;
    }

    private protected override bool DefenceAreaAbility(byte abilityRemain, out IAction act)
    {
        if (Actions.HeartofLight.ShouldUse(out act, emptyOrSkipCombo: true)) return true;
        if (GeneralActions.Reprisal.ShouldUse(out act, mustUse: true)) return true;
        return false;
    }

    private protected override bool MoveAbility(byte abilityRemain, out IAction act)
    {
        //ÕªΩ¯
        if (Actions.RoughDivide.ShouldUse(out act, emptyOrSkipCombo: true)) return true;
        return false;
    }
    private protected override bool DefenceSingleAbility(byte abilityRemain, out IAction act)
    {
        if (abilityRemain == 2)
        {

            //ºı…À10%£©
            if (Actions.HeartofStone.ShouldUse(out act)) return true;

            //–«‘∆£®ºı…À30%£©
            if (Actions.Nebula.ShouldUse(out act)) return true;

            //Ã˙±⁄£®ºı…À20%£©
            if (GeneralActions.Rampart.ShouldUse(out act)) return true;

            //Œ±◊∞£®ºı…À10%£©
            if (Actions.Camouflage.ShouldUse(out act)) return true;
        }
        //ΩµµÕπ•ª˜
        //—©≥
        if (GeneralActions.Reprisal.ShouldUse(out act)) return true;

        act = null;
        return false;
    }

    private protected override bool HealSingleAbility(byte abilityRemain, out IAction act)
    {
        if (Actions.Aurora.ShouldUse(out act, emptyOrSkipCombo: true) && abilityRemain == 1) return true;

        return false;
    }
}
