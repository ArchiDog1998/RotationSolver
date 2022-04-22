using Dalamud.Game.ClientState.JobGauge.Types;
using System.Linq;
using System.Numerics;

namespace XIVComboPlus.Combos;

internal abstract class WARCombo : CustomComboJob<WARGauge>
{
    internal static bool HaveShield => BaseAction.HaveStatusSelfFromSelf(ObjectStatus.Defiance);
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
            //ÖØÅü
            HeavySwing = new BaseAction(31),

            //Ð×²ÐÁÑ
            Maim = new BaseAction(37),

            //±©·çÕ¶ ÂÌ¸«
            StormsPath = new BaseAction(42),

            //±©·çËé ºì¸«
            StormsEye = new BaseAction(45)
            {
                OtherCheck = () => BuffTime < 10,
            },

            //·É¸«
            Tomahawk = new BaseAction(46),

            //ÃÍ¹¥
            Onslaught = new BaseAction(7386)
            {
                OtherCheck = () => TargetHelper.DistanceToPlayer(Service.TargetManager.Target) > 5,
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
                OtherCheck = () => TargetHelper.GetObjectInRadius(TargetHelper.HostileTargets, 5).Length > 0 && JobGauge.BeastGauge <= 50,
            },

            //¿ñ±©
            Berserk = new BaseAction(38)
            {
                OtherCheck = () => TargetHelper.GetObjectInRadius(TargetHelper.HostileTargets, 5).Length > 0,
            },

            //Õ½Àõ
            ThrillofBattle = new BaseAction(40),

            //Ì©È»×ÔÈô
            Equilibrium = new BaseAction(3552),

            //Ô­³õµÄÓÂÃÍ
            //NascentFlash = new BaseAction(16464),

            ////Ô­³õµÄÑªÆø
            //Bloodwhetting = new BaseAction(25751),

            //ÊØ»¤
            Defiance = new BaseAction(48),

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
                OtherCheck = () => (float)Service.ClientState.LocalPlayer.CurrentHp / Service.ClientState.LocalPlayer.MaxHp < 0.15,
            },

            ////Ô­³õµÄ½â·Å
            //InnerRelease = new BaseAction(7389),

            //Âù»Ä±ÀÁÑ
            PrimalRend = new BaseAction(25753)
            {
                BuffsNeed = new ushort[] { ObjectStatus.PrimalRendReady },
            };
    }

    private protected override bool EmergercyGCD(byte level, uint lastComboActionID, out BaseAction act, byte actabilityRemain)
    {
        //Èç¹û¾ÈÎÒÒ»¸öT£¬ÄÇ»¹²»¿ª¶Ü×Ë£¿
        if (!HaveShield && TargetHelper.PartyTanks.Length < 2)
        {
            if (Actions.Defiance.TryUseAction(level, out act)) return true;
        }
        act = null;
        return false;
    }

    private protected override bool GeneralGCD(byte level, uint lastComboActionID, out BaseAction act)
    {
        //ÊÞ»êÊä³ö
        if (JobGauge.BeastGauge >= 50 || BaseAction.HaveStatusSelfFromSelf(ObjectStatus.InnerRelease))
        {
            //¸ÖÌúÐý·ç
            if (Actions.SteelCyclone.TryUseAction(level, out act)) return true;
            //Ô­³õÖ®»ê
            if (Actions.InnerBeast.TryUseAction(level, out act)) return true;
        }

        //ÈºÌå
        if (Actions.MythrilTempest.TryUseAction(level, out act, lastComboActionID)) return true;
        if (Actions.Overpower.TryUseAction(level, out act, lastComboActionID)) return true;

        //µ¥Ìå
        if (Actions.StormsEye.TryUseAction(level, out act, lastComboActionID)) return true;
        if (Actions.StormsPath.TryUseAction(level, out act, lastComboActionID)) return true;
        if (Actions.Maim.TryUseAction(level, out act, lastComboActionID)) return true;
        if (Actions.HeavySwing.TryUseAction(level, out act, lastComboActionID)) return true;

        //¹»²»×Å£¬Ëæ±ã´òÒ»¸ö°É¡£
        if (Actions.Tomahawk.TryUseAction(level, out act)) return true;

        return false;
    }

    private protected override bool ForAttachAbility(byte level, byte abilityRemain, out BaseAction act)
    {
        TargetHelper.ProvokeTarget(out bool haveTargetOnme);
        if (!IsMoving && haveTargetOnme)
        {
            //ËÀ¶· Èç¹ûÐ»²»¹»ÁË¡£
            if (Actions.Holmgang.TryUseAction(level, out act)) return true;

            //Ô­³õµÄÖ±¾õ£¨¼õÉË10%£©
            if (Actions.RawIntuition.TryUseAction(level, out act)) return true;

            //½µµÍÉËº¦
            //¸´³ð£¨¼õÉË30%£©
            if (Actions.Vengeance.TryUseAction(level, out act)) return true;

            //Ìú±Ú£¨¼õÉË20%£©
            if (GeneralActions.Rampart.TryUseAction(level, out act)) return true;

            //½µµÍ¹¥»÷
            //Ñ©³ð
            if (GeneralActions.Reprisal.TryUseAction(level, out act)) return true;

            //Ç×Êè×ÔÐÐ
            if (GeneralActions.ArmsLength.TryUseAction(level, out act)) return true;

            //Ôö¼ÓÑªÁ¿
            ////°ÚÍÑ ¶ÓÓÑÌ×¶Ü
            //if (Actions.ShakeItOff.TryUseAction(level, out act)) return true;

        }

        //±¬·¢
        if (BuffTime > 3 || level < Actions.MythrilTempest.Level)
        {
            //Õ½º¿
            if (Actions.Infuriate.TryUseAction(level, out act)) return true;
            //¿ñ±©
            if (!new BaseAction(7389).CoolDown.IsCooldown && Actions.Berserk.TryUseAction(level, out act)) return true;
            //Õ½º¿
            if (Actions.Infuriate.TryUseAction(level, out act, Empty: true)) return true;
        }

        if ((float)Service.ClientState.LocalPlayer.CurrentHp / Service.ClientState.LocalPlayer.MaxHp < 0.6)
        {
            //Õ½Àõ
            if (Actions.ThrillofBattle.TryUseAction(level, out act)) return true;
            //Ì©È»×ÔÈô ×ÔÄÌ°¡£¡
            if (Actions.Equilibrium.TryUseAction(level, out act)) return true;
        }

        //ÄÌ¸ö¶ÓÓÑ°¡¡£
        //if (!haveTargetOnme && Actions.NascentFlash.TryUseAction(level, out act)) return true;

        //ÆÕÍ¨¹¥»÷
        //ÈºÉ½Â¡Æð
        if (Actions.Orogeny.TryUseAction(level, out act)) return true;
        //¶¯ÂÒ 
        if (Actions.Upheaval.TryUseAction(level, out act)) return true;

        //¸ã¸ã¹¥»÷
        var target = Service.TargetManager.Target;
        if(Vector3.Distance( Service.ClientState.LocalPlayer.Position, target.Position) - target.HitboxRadius < 3)
        {
            if (Actions.Onslaught.TryUseAction(level, out act)) return true;
        }

        return false;
    }

}
