using Dalamud.Game.ClientState.JobGauge.Types;
using Dalamud.Game.ClientState.Objects.Types;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace XIVComboPlus.Combos;

internal class WARCombo : CustomComboJob<WARGauge>
{
    internal override uint JobID => 21;
    internal override bool HaveShield => BaseAction.HaveStatusSelfFromSelf(ObjectStatus.Defiance);
    private protected override BaseAction Shield => Actions.Defiance;
    internal static float BuffTime
    {
        get
        {
            var time = BaseAction.FindStatusSelfFromSelf(ObjectStatus.SurgingTempest);
            if (time.Length == 0) return 0;
            return time[0];
        }
    }

    internal struct Actions
    {
        public static readonly BaseAction
            //ÊØ»¤
            Defiance =  new BaseAction(48, shouldEndSpecial:true),

            //ÖØÅü
            HeavySwing = new BaseAction(31),

            //Ð×²ÐÁÑ
            Maim = new BaseAction(37),

            //±©·çÕ¶ ÂÌ¸«
            StormsPath = new BaseAction(42),

            //±©·çËé ºì¸«
            StormsEye = new BaseAction(45)
            {
                OtherCheck = b => BuffTime < 10,
            },

            //·É¸«
            Tomahawk = new BaseAction(46)
            {
                FilterForHostile = b => BaseAction.ProvokeTarget(b, out _),
            },

            //ÃÍ¹¥
            Onslaught = new BaseAction(7386)
            {
                OtherCheck = b => BaseAction.DistanceToPlayer(Service.TargetManager.Target) > 5,
            },

            //¶¯ÂÒ    
            Upheaval = new BaseAction(7387),

            //³¬Ñ¹¸«
            Overpower = new BaseAction(41),

            //ÃØÒø±©·ç
            MythrilTempest = new BaseAction(16462),

            //ÈºÉ½Â¡Æð
            Orogeny = new BaseAction(25752),

            //Ô­³õÖ®»ê
            InnerBeast = new BaseAction(49),

            //¸ÖÌúÐý·ç
            SteelCyclone = new BaseAction(51),

            //Õ½º¿
            Infuriate = new BaseAction(52)
            {
                BuffsProvide = new ushort[] { ObjectStatus.InnerRelease },
                OtherCheck = b => BaseAction.GetObjectInRadius(TargetHelper.HostileTargets, 5).Length > 0 && JobGauge.BeastGauge <= 50,
            },

            //¿ñ±©
            Berserk = new BaseAction(38)
            {
                OtherCheck = b => BaseAction.GetObjectInRadius(TargetHelper.HostileTargets, 5).Length > 0,
            },

            //Õ½Àõ
            ThrillofBattle = new BaseAction(40),

            //Ì©È»×ÔÈô
            Equilibrium = new BaseAction(3552),

            //Ô­³õµÄÓÂÃÍ
            NascentFlash = new BaseAction(16464)
            {
                ChoiceFriend = BaseAction.FindAttackedTarget,
            },

            ////Ô­³õµÄÑªÆø
            //Bloodwhetting = new BaseAction(25751),

            //¸´³ð
            Vengeance = new BaseAction(44)
            {
                BuffsProvide = GeneralActions.Rampart.BuffsProvide,
            },

            //Ô­³õµÄÖ±¾õ
            RawIntuition = new BaseAction(3551)
            {
                BuffsProvide = GeneralActions.Rampart.BuffsProvide,
            },

            //°ÚÍÑ
            ShakeItOff = new BaseAction(7388),

            //ËÀ¶·
            Holmgang = new BaseAction(43)
            {
                OtherCheck = b => (float)Service.ClientState.LocalPlayer.CurrentHp / Service.ClientState.LocalPlayer.MaxHp < 0.15,
            },

            ////Ô­³õµÄ½â·Å
            //InnerRelease = new BaseAction(7389),

            //Âù»Ä±ÀÁÑ
            PrimalRend = new BaseAction(25753)
            {
                BuffsNeed = new ushort[] { ObjectStatus.PrimalRendReady },
            };
    }

    private protected override bool DefenceAreaAbility(byte abilityRemain, out BaseAction act)
    {
        //°ÚÍÑ ¶ÓÓÑÌ×¶Ü
        if (Actions.ShakeItOff.ShouldUseAction(out act)) return true;
        return false;
    }

    private protected override bool MoveGCD(uint lastComboActionID, out BaseAction act)
    {
        //·Å¸ö´ó Âù»Ä±ÀÁÑ »áÍùÇ°·É
        if (Actions.PrimalRend.ShouldUseAction(out act)) return true;
        return false;
    }

    private protected override bool MoveAbility(byte abilityRemain, out BaseAction act)
    {
        //Í»½ø
        if (Actions.Onslaught.ShouldUseAction(out act, Empty: true)) return true;
        return false;

    }

    private protected override bool GeneralGCD(uint lastComboActionID, out BaseAction act)
    {
        //ÊÞ»êÊä³ö
        if (JobGauge.BeastGauge >= 50 || BaseAction.HaveStatusSelfFromSelf(ObjectStatus.InnerRelease))
        {
            //¸ÖÌúÐý·ç
            if (Actions.SteelCyclone.ShouldUseAction(out act)) return true;
            //Ô­³õÖ®»ê
            if (Actions.InnerBeast.ShouldUseAction(out act)) return true;
        }

        //ÈºÌå
        if (Actions.MythrilTempest.ShouldUseAction(out act, lastComboActionID)) return true;
        if (Actions.Overpower.ShouldUseAction(out act, lastComboActionID)) return true;

        //µ¥Ìå
        if (Actions.StormsEye.ShouldUseAction(out act, lastComboActionID)) return true;
        if (Actions.StormsPath.ShouldUseAction(out act, lastComboActionID)) return true;
        if (Actions.Maim.ShouldUseAction( out act, lastComboActionID)) return true;
        if (Actions.HeavySwing.ShouldUseAction(out act, lastComboActionID)) return true;

        //¹»²»×Å£¬Ëæ±ã´òÒ»¸ö°É¡£
        if (Actions.Tomahawk.ShouldUseAction(out act)) return true;

        return false;
    }

    private protected override bool DefenceSingleAbility(byte abilityRemain, out BaseAction act)
    {
        if (abilityRemain == 1)
        {
            //ËÀ¶· Èç¹ûÐ»²»¹»ÁË¡£
            if (Actions.Holmgang.ShouldUseAction(out act)) return true;

            //Ô­³õµÄÖ±¾õ£¨¼õÉË10%£©
            if (Actions.RawIntuition.ShouldUseAction(out act)) return true;

            //¸´³ð£¨¼õÉË30%£©
            if (Actions.Vengeance.ShouldUseAction(out act)) return true;

            //Ìú±Ú£¨¼õÉË20%£©
            if (GeneralActions.Rampart.ShouldUseAction(out act)) return true;

            //½µµÍ¹¥»÷
            //Ñ©³ð
            if (GeneralActions.Reprisal.ShouldUseAction(out act)) return true;
        }

        act = null;
        return false;
    }

    private protected override bool ForAttachAbility(byte abilityRemain, out BaseAction act)
    {
        //±¬·¢
        if (BuffTime > 3 || Service.ClientState.LocalPlayer.Level < Actions.MythrilTempest.Level)
        {
            //Õ½º¿
            if (Actions.Infuriate.ShouldUseAction(out act)) return true;
            //¿ñ±©
            if (!new BaseAction(7389).IsCoolDown && Actions.Berserk.ShouldUseAction(out act)) return true;
            //Õ½º¿
            if (Actions.Infuriate.ShouldUseAction(out act, Empty: true)) return true;
        }

        if ((float)Service.ClientState.LocalPlayer.CurrentHp / Service.ClientState.LocalPlayer.MaxHp < 0.6)
        {
            //Õ½Àõ
            if (Actions.ThrillofBattle.ShouldUseAction(out act)) return true;
            //Ì©È»×ÔÈô ×ÔÄÌ°¡£¡
            if (Actions.Equilibrium.ShouldUseAction(out act)) return true;
        }

        //ÄÌ¸ö¶ÓÓÑ°¡¡£
        if (!HaveShield && Actions.NascentFlash.ShouldUseAction(out act)) return true;

        //ÆÕÍ¨¹¥»÷
        //ÈºÉ½Â¡Æð
        if (Actions.Orogeny.ShouldUseAction(out act)) return true;
        //¶¯ÂÒ 
        if (Actions.Upheaval.ShouldUseAction(out act)) return true;

        //¸ã¸ã¹¥»÷
        if (Actions.Onslaught.ShouldUseAction(out act))
        {
            if (Vector3.Distance(Service.ClientState.LocalPlayer.Position, Actions.Onslaught.Target.Position) - Actions.Onslaught.Target.HitboxRadius < 1)
            {
                return true;
            }
        }

        return false;
    }

}
