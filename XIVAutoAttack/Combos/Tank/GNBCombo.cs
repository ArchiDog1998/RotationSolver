using Dalamud.Game.ClientState.JobGauge.Types;
using System.Numerics;

namespace XIVComboPlus.Combos;

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
            //ÍõÊÒÇ×ÎÀ
            RoyalGuard = new BaseAction(16142, shouldEndSpecial: true),

            //ÀûÈÐÕ¶
            KeenEdge = new BaseAction(16137),

            //ÎÞÇé
            NoMercy = new BaseAction(16138),

            //²Ð±©µ¯
            BrutalShell = new BaseAction(16139),

            //Î±×°
            Camouflage = new BaseAction(16140)
            {
                BuffsProvide = GeneralActions.Rampart.BuffsProvide,
            },

            //¶ñÄ§ÇÐ
            DemonSlice = new BaseAction(16141),

            //ÉÁÀ×µ¯
            LightningShot = new BaseAction(16143),

            //Î£ÏÕÁìÓò
            DangerZone = new BaseAction(16144),

            //Ñ¸Á¬Õ¶
            SolidBarrel = new BaseAction(16145),

            //±¬·¢»÷
            BurstStrike = new BaseAction(16162),

            //ÐÇÔÆ
            Nebula = new BaseAction(16148)
            {
                BuffsProvide = GeneralActions.Rampart.BuffsProvide,
            },

            //¶ñÄ§É±
            DemonSlaughter = new BaseAction(16149),

            //¼«¹â
            Aurora = new BaseAction(16151, true)
            {
                BuffsProvide = new ushort[] { ObjectStatus.Aurora },
            },

            //³¬»ðÁ÷ÐÇ
            Superbolide = new BaseAction(16152)
            {
                OtherCheck = b => (float)Service.ClientState.LocalPlayer.CurrentHp / Service.ClientState.LocalPlayer.MaxHp < 0.15,
            },

            //ÒôËÙÆÆ
            SonicBreak = new BaseAction(16153),

            //´Ö·ÖÕ¶
            RoughDivide = new BaseAction(16154, shouldEndSpecial:true),

            //ÁÒÑÀ
            GnashingFang = new BaseAction(16146),

            //¹­ÐÎ³å²¨
            BowShock = new BaseAction(16159),

            //¹âÖ®ÐÄ
            HeartofLight = new BaseAction(16160, true),

            //Ê¯Ö®ÐÄ
            HeartofStone = new BaseAction(16161, true)
            {
                BuffsProvide = GeneralActions.Rampart.BuffsProvide,
            },

            //ÃüÔËÖ®»·
            FatedCircle = new BaseAction(16163),

            //ÑªÈÀ
            Bloodfest = new BaseAction(16164)
            {
                OtherCheck = b => JobGauge.Ammo == 0,
            },

            //±¶¹¥
            DoubleDown = new BaseAction(25760),

            //ÃÍÊÞ×¦
            SavageClaw = new BaseAction(16147),

            //Ð×ÇÝ×¦
            WickedTalon = new BaseAction(16150),

            //Ëººí
            JugularRip = new BaseAction(16156)
            {
                BuffsNeed = new ushort[] { ObjectStatus.ReadyToRip },
            },

            //ÁÑÌÅ
            AbdomenTear = new BaseAction(16157)
            {
                BuffsNeed = new ushort[] { ObjectStatus.ReadyToTear },
            },

            //´©Ä¿
            EyeGouge = new BaseAction(16158)
            {
                BuffsNeed = new ushort[] { ObjectStatus.ReadyToGouge },
            },

            //³¬¸ßËÙ
            Hypervelocity = new BaseAction(25759)
            {
                BuffsNeed = new ushort[] { ObjectStatus.ReadyToBlast },
            };
    }

    private protected override bool GeneralGCD(uint lastComboActionID, out BaseAction act)
    {
        if (Actions.DoubleDown.ShouldUseAction(out act))
        {
            if (JobGauge.Ammo > 1)
            {
                return true;
            }
        }
        else  if (JobGauge.Ammo > 0)
        {
            if (Actions.FatedCircle.ShouldUseAction(out act)) return true;
            if (Actions.GnashingFang.ShouldUseAction(out act)) return true;
            if (Actions.BurstStrike.ShouldUseAction(out act)) return true;
        }
        if (Actions.WickedTalon.ShouldUseAction(out act, lastComboActionID)) return true;
        if (Actions.SavageClaw.ShouldUseAction(out act, lastComboActionID)) return true;



        //AOE
        if (Actions.DemonSlaughter.ShouldUseAction(out act, lastComboActionID)) return true;
        if (Actions.DemonSlice.ShouldUseAction(out act, lastComboActionID)) return true;


        if (Actions.SonicBreak.ShouldUseAction(out act)) return true;

        //µ¥ÌåÈýÁ¬
        if (Actions.SolidBarrel.ShouldUseAction(out act, lastComboActionID)) return true;
        if (Actions.BrutalShell.ShouldUseAction(out act, lastComboActionID)) return true;
        if (Actions.KeenEdge.ShouldUseAction(out act, lastComboActionID)) return true;

        if (IconReplacer.Move && MoveAbility(1, out act)) return true;
        if (Actions.LightningShot.ShouldUseAction(out act)) return true;

        return false;
    }

    private protected override bool ForAttachAbility(byte abilityRemain, out BaseAction act)
    {
        if (Actions.JugularRip.ShouldUseAction(out act)) return true;
        if (Actions.AbdomenTear.ShouldUseAction(out act)) return true;
        if (Actions.EyeGouge.ShouldUseAction(out act)) return true;
        if (Actions.Hypervelocity.ShouldUseAction(out act)) return true;


        if (Actions.NoMercy.ShouldUseAction(out act)) return true;
        if (Actions.Bloodfest.ShouldUseAction(out act)) return true;
        if (Actions.BowShock.ShouldUseAction(out act, mustUse: true)) return true;
        if (Actions.DangerZone.ShouldUseAction(out act)) return true;

        //¸ã¸ã¹¥»÷
        if (Actions.RoughDivide.ShouldUseAction(out act) && !IsMoving)
        {
            if (Vector3.Distance(Service.ClientState.LocalPlayer.Position, Actions.RoughDivide.Target.Position) - Actions.RoughDivide.Target.HitboxRadius < 1)
            {
                return true;
            }
        }
        return false;
    }

    private protected override bool DefenceAreaAbility(byte abilityRemain, out BaseAction act)
    {
        if (Actions.HeartofLight.ShouldUseAction(out act, Empty: true)) return true;
        return false;
    }

    private protected override bool MoveAbility(byte abilityRemain, out BaseAction act)
    {
        //Í»½ø
        if (Actions.RoughDivide.ShouldUseAction(out act, Empty: true)) return true;
        return false;
    }
    private protected override bool DefenceSingleAbility(byte abilityRemain, out BaseAction act)
    {
        if (abilityRemain == 1)
        {
            //ÉñÊ¥ÁìÓò Èç¹ûÐ»²»¹»ÁË¡£
            if (Actions.Superbolide.ShouldUseAction(out act)) return true;

            //¼õÉË10%£©
            if (Actions.HeartofStone.ShouldUseAction(out act)) return true;

            //ÐÇÔÆ£¨¼õÉË30%£©
            if (Actions.Nebula.ShouldUseAction(out act)) return true;

            //Ìú±Ú£¨¼õÉË20%£©
            if (GeneralActions.Rampart.ShouldUseAction(out act)) return true;

            //Î±×°£¨¼õÉË10%£©
            if (Actions.Camouflage.ShouldUseAction(out act)) return true;

            //½µµÍ¹¥»÷
            //Ñ©³ð
            if (GeneralActions.Reprisal.ShouldUseAction(out act)) return true;
        }

        act = null;
        return false;
    }

    private protected override bool HealSingleAbility(byte abilityRemain, out BaseAction act)
    {
        if (Actions.Aurora.ShouldUseAction(out act, Empty:true) && abilityRemain == 1) return true;

        return false;
    }
}
