using Dalamud.Game.ClientState.JobGauge.Types;
using System.Linq;
using System.Numerics;

namespace XIVComboPlus.Combos;

internal class PLDCombo : CustomComboJob<PLDGauge>
{
    internal override uint JobID => 19;

    internal override  bool HaveShield => BaseAction.HaveStatusSelfFromSelf(ObjectStatus.IronWill);

    private protected override BaseAction Shield => new BaseAction(28);

    protected override bool CanHealSingleSpell => false;
    protected override bool CanHealAreaSpell => false;

    internal struct Actions
    {
        public static readonly BaseAction
            //ÏÈ·æ½£
            FastBlade = new BaseAction(9),

            //±©ÂÒ½£
            RiotBlade = new BaseAction(15),

            //Á¤Ñª½£
            GoringBlade = new BaseAction(3538)
            {
                TargetStatus = new ushort[]
                {
                    ObjectStatus.GoringBlade,
                    ObjectStatus.BladeofValor,
                }
            },

            //Õ½Å®ÉñÖ®Å­
            RageofHalone = new BaseAction(21),

            //Í¶¶Ü
            ShieldLob = new BaseAction(24),

            //Õ½ÌÓ·´Ó¦
            FightorFlight = new BaseAction(20),

            //È«Ê´Õ¶
            TotalEclipse = new BaseAction(7381),

            //ÈÕçíÕ¶
            Prominence = new BaseAction(16457),

            //Ô¤¾¯
            Sentinel = new BaseAction(17)
            {
                BuffsProvide = GeneralActions.Rampart.BuffsProvide,
            },

            //¶òÔËÁ÷×ª
            CircleofScorn = new BaseAction(23),

            //Éî°ÂÖ®Áé
            SpiritsWithin = new BaseAction(29),

            //ÉñÊ¥ÁìÓò
            HallowedGround = new BaseAction(30)
            {
                OtherCheck = b => (float)Service.ClientState.LocalPlayer.CurrentHp / Service.ClientState.LocalPlayer.MaxHp < 0.15,
            },

            //Ê¥¹âÄ»Á±
            DivineVeil = new BaseAction(3540),

            //ÉîÈÊºñÔó
            Clemency = new BaseAction(3541, true),

            //¸ÉÔ¤
            Intervention = new BaseAction(7382, true),

            //µ÷Í£
            Intervene = new BaseAction(16461),

            //Êê×ï½£
            Atonement = new BaseAction(16460)
            {
                BuffsNeed = new ushort[] { ObjectStatus.SwordOath },
            },

            //³¥Êê½£
            Expiacion = new BaseAction(25747),

            //Ó¢ÓÂÖ®½£
            BladeofValor = new BaseAction(25750),

            //ÕæÀíÖ®½£
            BladeofTruth = new BaseAction(25749),

            //ÐÅÄîÖ®½£
            BladeofFaith = new BaseAction(25748),

            //°²»êÆíµ»
            Requiescat = new BaseAction(7383),

            //»Ú×ï
            Confiteor = new BaseAction(16459),

            //Ê¥»·
            HolyCircle = new BaseAction(16458),

            //Ê¥Áé
            HolySpirit = new BaseAction(7384),

            //Îä×°ÊùÎÀ
            PassageofArms = new BaseAction(7385),

            //¶ÜÕó
            Sheltron = new BaseAction(3542);
        //¶ÜÅÆÃÍ»÷
        //ShieldBash = new BaseAction(16),
    }

    private protected override bool GeneralGCD(uint lastComboActionID, out BaseAction act)
    {
        //Èý¸ö´óÕÐ
        if (Actions.BladeofValor.ShouldUseAction(out act, lastComboActionID, mustUse:true)) return true;
        if (Actions.BladeofTruth.ShouldUseAction(out act, lastComboActionID, mustUse: true)) return true;
        if (Actions.BladeofFaith.ShouldUseAction(out act, lastComboActionID, mustUse: true)) return true;

        //Ä§·¨ÈýÖÖ×ËÊÆ
        var status = BaseAction.FindStatusFromSelf(Service.ClientState.LocalPlayer).Where(status => status.StatusId == ObjectStatus.Requiescat);
        if(status != null && status.Count() > 0)
        {
            var s = status.First();
            if ((s.StackCount == 1 || s.RemainingTime < 2.5) && 
                Actions.Confiteor.ShouldUseAction(out act, mustUse: true)) return true;
            if (Actions.HolyCircle.ShouldUseAction(out act)) return true;
            if (Actions.HolySpirit.ShouldUseAction(out act)) return true;
        }

        //AOE ¶þÁ¬
        if (Actions.Prominence.ShouldUseAction(out act, lastComboActionID)) return true;
        if (Actions.TotalEclipse.ShouldUseAction(out act, lastComboActionID)) return true;

        //Êê×ï½£
        if (Actions.Atonement.ShouldUseAction(out act)) return true;

        //µ¥ÌåÈýÁ¬
        if (Actions.GoringBlade.ShouldUseAction(out act, lastComboActionID)) return true;
        if (Actions.RageofHalone.ShouldUseAction(out act, lastComboActionID)) return true;
        if (Actions.RiotBlade.ShouldUseAction(out act, lastComboActionID)) return true;
        if (Actions.FastBlade.ShouldUseAction(out act, lastComboActionID)) return true;

        //Í¶¶Ü
        if (Actions.ShieldLob.ShouldUseAction(out act)) return true;

        return false;
    }

    private protected override bool MoveAbility(byte abilityRemain, out BaseAction act)
    {
        //µ÷Í£
        if (Actions.Intervene.ShouldUseAction(out act, Empty:true)) return true;

        return false;
    }

    private protected override bool HealSingleGCD(uint lastComboActionID, out BaseAction act)
    {
        //ÉîÈÊºñÔó
        if (Actions.Clemency.ShouldUseAction(out act)) return true;

        return false;
    }

    private protected override bool DefenceAreaAbility(byte abilityRemain, out BaseAction act)
    {
        //Ê¥¹âÄ»Á±
        if (Actions.DivineVeil.ShouldUseAction(out act)) return true;

        //Îä×°ÊùÎÀ
        if (Actions.PassageofArms.ShouldUseAction(out act)) return true;
        return false;
    }

    private protected override bool ForAttachAbility(byte abilityRemain, out BaseAction act)
    {
        //Õ½ÌÓ·´Ó¦ ¼ÓBuff
        if (Actions.FightorFlight.ShouldUseAction(out act)) return true;

        //¶òÔËÁ÷×ª
        if (Actions.CircleofScorn.ShouldUseAction(out act, mustUse:true)) return true;

        //³¥Êê½£
        if (Actions.Expiacion.ShouldUseAction(out act, mustUse: true)) return true;

        //°²»êÆíµ»
        if (BaseAction.FindStatusSelfFromSelf(ObjectStatus.GoringBlade, ObjectStatus.BladeofValor).Max() > 10 &&
            Actions.Requiescat.ShouldUseAction(out act, mustUse: true)) return true;

        //Éî°ÂÖ®Áé
        if (Actions.SpiritsWithin.ShouldUseAction(out act)) return true;

        //¸ã¸ã¹¥»÷
        if (Actions.Intervene.ShouldUseAction(out act))
        {
            if (Vector3.Distance(Service.ClientState.LocalPlayer.Position, Actions.Intervene.Target.Position) - Actions.Intervene.Target.HitboxRadius < 1)
            {
                return true;
            }
        }

        return false;
    }

    private protected override bool DefenceSingleAbility(byte abilityRemain, out BaseAction act)
    {
        if (abilityRemain == 1)
        {
            //ËÀ¶· Èç¹ûÐ»²»¹»ÁË¡£
            if (Actions.HallowedGround.ShouldUseAction(out act)) return true;

            //Ô¤¾¯£¨¼õÉË30%£©
            if (Actions.Sentinel.ShouldUseAction(out act)) return true;

            //Ìú±Ú£¨¼õÉË20%£©
            if (GeneralActions.Rampart.ShouldUseAction(out act)) return true;

            //¸ÉÔ¤£¨¼õÉË10%£©
            if (Actions.Intervention.ShouldUseAction(out act)) return true;

            //½µµÍ¹¥»÷
            //Ñ©³ð
            if (GeneralActions.Reprisal.ShouldUseAction(out act)) return true;

            //¶ÜÕó
            if (Actions.Sheltron.ShouldUseAction(out act)) return true;

        }

        act = null;
        return false;
    }
}
