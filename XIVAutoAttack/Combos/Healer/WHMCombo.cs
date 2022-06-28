using Dalamud.Game.ClientState.JobGauge.Types;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Game.ClientState.Objects.Types;
using System.Collections.Generic;
using System.Linq;

namespace XIVAutoAttack.Combos.Healer;

internal class WHMCombo : CustomComboJob<WHMGauge>
{
    internal override uint JobID => 24;
    private protected override BaseAction Raise => Actions.Raise;
    internal struct Actions
    {
        public static readonly BaseAction
            //复活
            Raise = new BaseAction(125, true),

            //飞石 平A
            Stone = new BaseAction(119),

            //疾风 Dot
            Aero = new BaseAction(121)
            {
                TargetStatus = new ushort[]
                {
                    ObjectStatus.Aero,
                    ObjectStatus.Aero2,
                    ObjectStatus.Dia,
                }
            },
            //苦难之心
            AfflatusMisery = new BaseAction(16535)
            {
                OtherCheck = b => JobGauge.BloodLily == 3,
            },
            //神圣
            Holy = new BaseAction(139),

            //治疗
            Cure = new BaseAction(120, true),
            //救疗
            Cure2 = new BaseAction(135, true) { OtherIDsNot = new uint[] { 135 } },
            //神名
            Tetragrammaton = new BaseAction(3570, true),
            //安慰之心 800
            AfflatusSolace = new BaseAction(16531, true)
            {
                OtherCheck = b => JobGauge.Lily > 0,
            },
            //再生
            Regen = new BaseAction(137, true)
            {
                TargetStatus = new ushort[]
                {
                    ObjectStatus.Regen1,
                    ObjectStatus.Regen2,
                    ObjectStatus.Regen3,
                }
            },
            //水流幕
            Aquaveil = new BaseAction(25861, true),
            //神祝祷
            DivineBenison = new BaseAction(7432, true),
            //天赐
            Benediction = new BaseAction(140, true)
            {
                OtherCheck = b => TargetHelper.PartyMembersHP.Min() < 0.15,
            },

            //医治 群奶最基础的。300
            Medica = new BaseAction(124, true),
            //愈疗 600
            Cure3 = new BaseAction(131, true),
            //医济 群奶加Hot。
            Medica2 = new BaseAction(133, true) { BuffsProvide = new ushort[] { ObjectStatus.Medica2, ObjectStatus.TrueMedica2 } },
            //庇护所
            Asylum = new BaseAction(3569, true),
            //法令
            Assize = new BaseAction(3571, true),
            //狂喜之心 400
            AfflatusRapture = new BaseAction(16534, true)
            {
                OtherCheck = b => JobGauge.Lily > 0,
            },
            //礼仪之铃
            LiturgyoftheBell = new BaseAction(25862, true),

            //神速咏唱
            PresenseOfMind = new BaseAction(136, true),
            //无中生有
            ThinAir = new BaseAction(7430, true),

            //全大赦
            PlenaryIndulgence = new BaseAction(7433, true),
            //节制
            Temperance = new BaseAction(16536, true);
    }
    internal override SortedList<DescType, string> Description => new SortedList<DescType, string>()
    {
        {DescType.范围治疗, $"GCD: {Actions.AfflatusRapture.Action.Name}, {Actions.Medica2.Action.Name}, {Actions.Cure3.Action.Name}, {Actions.Medica.Action.Name}\n                     能力: {Actions.Asylum.Action.Name}, {Actions.Assize.Action.Name}"},
        {DescType.单体治疗, $"GCD: {Actions.AfflatusSolace.Action.Name}, {Actions.Regen.Action.Name}, {Actions.Cure2.Action.Name}, {Actions.Cure.Action.Name}\n                     能力: {Actions.Tetragrammaton.Action.Name}"},
        {DescType.范围防御, $"{Actions.Temperance.Action.Name}, {Actions.LiturgyoftheBell.Action.Name}"},
        {DescType.单体防御, $"{Actions.DivineBenison.Action.Name}, {Actions.Aquaveil.Action.Name}"},
    };
    private protected override bool HealAreaGCD(uint lastComboActionID, out IAction act)
    {
        //狂喜之心
        if (Actions.AfflatusRapture.ShouldUseAction(out act)) return true;
        //加Hot
        if (Actions.Medica2.ShouldUseAction(out act)) return true;

        float cure3 = TargetHelper.GetBestHeal(Actions.Cure3.Action, 600);
        float medica = TargetHelper.GetBestHeal(Actions.Medica.Action, 300);

        //愈疗
        if (cure3 > medica && Actions.Cure3.ShouldUseAction(out act)) return true;
        if (Actions.Medica.ShouldUseAction(out act)) return true;

        return false;
    }

    private protected override bool DefenceSingleAbility(byte abilityRemain, out IAction act)
    {
        //加个神祝祷
        if (Actions.DivineBenison.ShouldUseAction(out act)) return true;
        //水流幕
        if (Actions.Aquaveil.ShouldUseAction(out act)) return true;
        return false;
    }

    private protected override bool DefenceAreaAbility(byte abilityRemain, out IAction act)
    {
        //节制
        if (Actions.Temperance.ShouldUseAction(out act)) return true;
        //礼仪之铃
        if (Actions.LiturgyoftheBell.ShouldUseAction(out act)) return true;
        return false;
    }

    private protected override bool ForAttachAbility(byte abilityRemain, out IAction act)
    {
        //加个神速咏唱
        if (Actions.PresenseOfMind.ShouldUseAction(out act)) return true;

        return false;
    }

    private protected override bool EmergercyAbility(byte abilityRemain, IAction nextGCD, out IAction act)
    {
        //加个无中生有
        if (nextGCD is BaseAction action && action.MPNeed > 500 && Actions.ThinAir.ShouldUseAction(out act)) return true;


        //天赐救人啊！
        if (Actions.Benediction.ShouldUseAction(out act)) return true;

        if (nextGCD.ID == Actions.Medica.ID || nextGCD.ID == Actions.Medica2.ID ||
            nextGCD.ID == Actions.Cure3.ID || nextGCD.ID == Actions.AfflatusRapture.ID)
        {
            //加个全大赦
            if (Actions.PlenaryIndulgence.ShouldUseAction(out act)) return true;
        }


        act = null;
        return false;
    }

    private protected override bool HealAreaAbility(byte abilityRemain, out IAction act)
    {
        //庇护所
        if (!IsMoving && Actions.Asylum.ShouldUseAction(out act)) return true;

        //加个法令
        if (Actions.Assize.ShouldUseAction(out act)) return true;

        return false;
    }

    private protected override bool HealSingleAbility(byte abilityRemain, out IAction act)
    {
        //神名
        if (Actions.Tetragrammaton.ShouldUseAction(out act)) return true;

        return false;
    }

    private protected override bool HealSingleGCD(uint lastComboActionID, out IAction act)
    {
        //安慰之心
        if (Actions.AfflatusSolace.ShouldUseAction(out act)) return true;

        //再生
        if (Actions.Regen.ShouldUseAction(out act)) return true;

        //救疗
        if (Actions.Cure2.ShouldUseAction(out act, lastComboActionID)) return true;

        //治疗
        if (Actions.Cure.ShouldUseAction(out act)) return true;

        return false;
    }

    private protected override bool GeneralGCD(uint lastComboActionID, out IAction act)
    {
        //苦难之心
        if (Actions.AfflatusMisery.ShouldUseAction(out act, mustUse: true)) return true;

        //群体输出
        if (Actions.Holy.ShouldUseAction(out act)) return true;

        //单体输出
        if (Actions.Aero.ShouldUseAction(out act, mustUse: IsMoving && HaveTargetAngle)) return true;
        if (Actions.Stone.ShouldUseAction(out act)) return true;

        act = null;
        return false;
    }
}
