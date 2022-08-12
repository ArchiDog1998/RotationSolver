using Dalamud.Game.ClientState.JobGauge.Types;
using System.Collections.Generic;
using System.Numerics;

namespace XIVAutoAttack.Combos.Tank;

internal class GNBCombo : CustomComboJob<GNBGauge>
{
    internal override uint JobID => 37;
    internal override bool HaveShield => BaseAction.HaveStatusSelfFromSelf(ObjectStatus.RoyalGuard);
    private protected override BaseAction Shield => Actions.RoyalGuard;

    protected override bool CanHealSingleSpell => false;
    protected override bool CanHealAreaSpell => false;

    internal struct Actions
    {
        public static readonly BaseAction
            //Õı “«◊Œ¿
            RoyalGuard = new BaseAction(16142, shouldEndSpecial: true),

            //¿˚»–’∂
            KeenEdge = new BaseAction(16137),

            //Œﬁ«È
            NoMercy = new BaseAction(16138),

            //≤–±©µØ
            BrutalShell = new BaseAction(16139),

            //Œ±◊∞
            Camouflage = new BaseAction(16140)
            {
                BuffsProvide = GeneralActions.Rampart.BuffsProvide,
            },

            //∂Òƒß«–
            DemonSlice = new BaseAction(16141),

            //…¡¿◊µØ
            LightningShot = new BaseAction(16143),

            //Œ£œ’¡Ï”Ú
            DangerZone = new BaseAction(16144),

            //—∏¡¨’∂
            SolidBarrel = new BaseAction(16145),

            //±¨∑¢ª˜
            BurstStrike = new BaseAction(16162),

            //–«‘∆
            Nebula = new BaseAction(16148)
            {
                BuffsProvide = GeneralActions.Rampart.BuffsProvide,
            },

            //∂Òƒß…±
            DemonSlaughter = new BaseAction(16149),

            //º´π‚
            Aurora = new BaseAction(16151, true)
            {
                BuffsProvide = new ushort[] { ObjectStatus.Aurora },
            },

            //≥¨ª¡˜–«
            Superbolide = new BaseAction(16152)
            {
                OtherCheck = b => (float)Service.ClientState.LocalPlayer.CurrentHp / Service.ClientState.LocalPlayer.MaxHp < Service.Configuration.HealthForDyingTank,
            },

            //“ÙÀŸ∆∆
            SonicBreak = new BaseAction(16153),

            //¥÷∑÷’∂
            RoughDivide = new BaseAction(16154, shouldEndSpecial: true),

            //¡“—¿
            GnashingFang = new BaseAction(16146),

            //π≠–Œ≥Â≤®
            BowShock = new BaseAction(16159),

            //π‚÷Æ–ƒ
            HeartofLight = new BaseAction(16160, true),

            // Ø÷Æ–ƒ
            HeartofStone = new BaseAction(16161, true)
            {
                BuffsProvide = GeneralActions.Rampart.BuffsProvide,
            },

            //√¸‘À÷Æª∑
            FatedCircle = new BaseAction(16163),

            //—™»¿
            Bloodfest = new BaseAction(16164)
            {
                OtherCheck = b => JobGauge.Ammo == 0,
            },

            //±∂π•
            DoubleDown = new BaseAction(25760),

            //√Õ ﬁ◊¶
            SavageClaw = new BaseAction(16147),

            //–◊«›◊¶
            WickedTalon = new BaseAction(16150),

            //À∫∫Ì
            JugularRip = new BaseAction(16156)
            {
                OtherCheck = b => Service.IconReplacer.OriginalHook(16155) == JugularRip.ID,
            },

            //¡—Ã≈
            AbdomenTear = new BaseAction(16157)
            {
                OtherCheck = b => Service.IconReplacer.OriginalHook(16155) == AbdomenTear.ID,
            },

            //¥©ƒø
            EyeGouge = new BaseAction(16158)
            {
                OtherCheck = b => Service.IconReplacer.OriginalHook(16155) == EyeGouge.ID,
            },

            //≥¨∏ﬂÀŸ
            Hypervelocity = new BaseAction(25759)
            {
                OtherCheck = b => Service.IconReplacer.OriginalHook(16155) == Hypervelocity.ID,
            };
    }
    internal override SortedList<DescType, string> Description => new SortedList<DescType, string>()
    {
        {DescType.µ•ÃÂ÷Œ¡∆, $"{Actions.Aurora.Action.Name}"},
        {DescType.∑∂Œß∑¿”˘, $"{Actions.HeartofLight.Action.Name}"},
        {DescType.µ•ÃÂ∑¿”˘, $"{Actions.HeartofStone.Action.Name}, {Actions.Nebula.Action.Name}, {Actions.Camouflage.Action.Name}"},
        {DescType.“∆∂Ø, $"{Actions.RoughDivide.Action.Name}"},
    };
    private protected override bool GeneralGCD(uint lastComboActionID, out IAction act)
    {
        // π”√æßƒ“
        bool useAmmo = JobGauge.Ammo > (Service.ClientState.LocalPlayer.Level >= Actions.DoubleDown.Level ? 2 : 0);

        uint remap = Service.IconReplacer.OriginalHook(Actions.GnashingFang.ID);
        if (remap == Actions.WickedTalon.ID && Actions.WickedTalon.ShouldUseAction(out act)) return true;
        if (remap == Actions.SavageClaw.ID && Actions.SavageClaw.ShouldUseAction(out act)) return true;

        //AOE
        if (useAmmo)
        {
            if (Actions.DoubleDown.ShouldUseAction(out act, mustUse: true)) return true;
            if (Actions.FatedCircle.ShouldUseAction(out act)) return true;
        }
        if ( Actions.DemonSlaughter.ShouldUseAction(out act, lastComboActionID)) return true;
        if ( Actions.DemonSlice.ShouldUseAction(out act, lastComboActionID)) return true;

        //µ•ÃÂ
        if (useAmmo)
        {
            if (Actions.GnashingFang.ShouldUseAction(out act)) return true;
            if (Actions.BurstStrike.ShouldUseAction(out act)) return true;
        }
        if (Actions.SonicBreak.ShouldUseAction(out act)) return true;

        //µ•ÃÂ»˝¡¨
        if (Actions.SolidBarrel.ShouldUseAction(out act, lastComboActionID)) return true;
        if (Actions.BrutalShell.ShouldUseAction(out act, lastComboActionID)) return true;
        if (Actions.KeenEdge.ShouldUseAction(out act, lastComboActionID)) return true;

        if (IconReplacer.Move && MoveAbility(1, out act)) return true;
        if (Actions.LightningShot.ShouldUseAction(out act)) return true;

        return false;
    }

    private protected override bool EmergercyAbility(byte abilityRemain, IAction nextGCD, out IAction act)
    {
        //…Ò •¡Ï”Ú »Áπ˚–ª≤ªπª¡À°£
        if (Actions.Superbolide.ShouldUseAction(out act)) return true;
        return false;
    }

    private protected override bool ForAttachAbility(byte abilityRemain, out IAction act)
    {
        if (Actions.JugularRip.ShouldUseAction(out act)) return true;
        if (Actions.AbdomenTear.ShouldUseAction(out act)) return true;
        if (Actions.EyeGouge.ShouldUseAction(out act)) return true;
        if (Actions.Hypervelocity.ShouldUseAction(out act)) return true;


        if (Actions.NoMercy.ShouldUseAction(out act)) return true;
        if (Actions.Bloodfest.ShouldUseAction(out act)) return true;
        if (Actions.BowShock.ShouldUseAction(out act, mustUse: true)) return true;
        if (Actions.DangerZone.ShouldUseAction(out act)) return true;

        //∏„∏„π•ª˜
        if (Actions.RoughDivide.ShouldUseAction(out act) && !IsMoving)
        {
            if (BaseAction.DistanceToPlayer(Actions.RoughDivide.Target) < 2)
            {
                return true;
            }
        }
        return false;
    }

    private protected override bool DefenceAreaAbility(byte abilityRemain, out IAction act)
    {
        if (Actions.HeartofLight.ShouldUseAction(out act, emptyOrSkipCombo: true)) return true;
        return false;
    }

    private protected override bool MoveAbility(byte abilityRemain, out IAction act)
    {
        //ÕªΩ¯
        if (Actions.RoughDivide.ShouldUseAction(out act, emptyOrSkipCombo: true)) return true;
        return false;
    }
    private protected override bool DefenceSingleAbility(byte abilityRemain, out IAction act)
    {
        if (abilityRemain == 1)
        {

            //ºı…À10%£©
            if (Actions.HeartofStone.ShouldUseAction(out act)) return true;

            //–«‘∆£®ºı…À30%£©
            if (Actions.Nebula.ShouldUseAction(out act)) return true;

            //Ã˙±⁄£®ºı…À20%£©
            if (GeneralActions.Rampart.ShouldUseAction(out act)) return true;

            //Œ±◊∞£®ºı…À10%£©
            if (Actions.Camouflage.ShouldUseAction(out act)) return true;

            //ΩµµÕπ•ª˜
            //—©≥
            if (GeneralActions.Reprisal.ShouldUseAction(out act)) return true;
        }

        act = null;
        return false;
    }

    private protected override bool HealSingleAbility(byte abilityRemain, out IAction act)
    {
        if (Actions.Aurora.ShouldUseAction(out act, emptyOrSkipCombo: true) && abilityRemain == 1) return true;

        return false;
    }
}
