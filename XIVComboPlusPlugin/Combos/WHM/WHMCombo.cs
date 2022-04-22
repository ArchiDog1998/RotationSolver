using Dalamud.Game.ClientState.JobGauge.Types;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Game.ClientState.Objects.Types;
using System.Linq;
namespace XIVComboPlus.Combos;

internal abstract class WHMCombo : CustomComboJob<WHMGauge>
{

    internal struct Actions
    {

        public static readonly BaseAction
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
                OtherCheck = () => JobGauge.BloodLily == 3,
            },
            //神圣
            Holy = new BaseAction(139),

            //治疗
            Cure = new BaseAction(120, true),
            //救疗
            Cure2 = new BaseAction(135, true),
            //神名
            Tetragrammaton = new BaseAction(3570, true),
            //安慰之心 800
            AfflatusSolace = new BaseAction(16531, true)
            {
                OtherCheck = () => JobGauge.Lily > 0,
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
            Benediction = new BaseAction(140, true),

            //医治 群奶最基础的。300
            Medica = new BaseAction(124, true),
            //愈疗 600
            Cure3 = new BaseAction(131, true),
            //医济 群奶加Hot。
            Medica2 = new BaseAction(133, true),
            //庇护所
            Asylum = new BaseAction(3569, true),
            //法令
            Assize = new BaseAction(3571, true),
            //狂喜之心 400
            AfflatusRapture = new BaseAction(16534, true)
            {
                OtherCheck = () => JobGauge.Lily > 0,
            },
            //礼仪之铃
            LiturgyoftheBell = new BaseAction(25862, true),

            //复活
            Raise = new BaseAction(125, true)
            {
                OtherCheck = () => TargetHelper.DeathPeopleAll.Length > 0,
            },

            //神速咏唱
            PresenseOfMind = new BaseAction(136),
            //无中生有
            ThinAir = new BaseAction(7430),

            //全大赦
            PlenaryIndulgence = new BaseAction(7433),
            //节制
            Temperance = new BaseAction(16536);
    }

    private protected override bool HealAreaGCD(byte level, uint lastComboActionID, out BaseAction act)
    {

        //狂喜之心
        if (Actions.AfflatusRapture.TryUseAction(level, out act)) return true;
        //加Hot
        if (Actions.Medica2.TryUseAction(level, out act)) return true;

        float cure3 = TargetHelper.GetBestHeal(Actions.Cure3.Action, 600);
        float medica = TargetHelper.GetBestHeal(Actions.Medica.Action, 300);

        //愈疗
        if (cure3 > medica && Actions.Cure3.TryUseAction(level, out act)) return true;
        if (Actions.Medica.TryUseAction(level, out act)) return true;

        return false;
    }

    private protected override bool ForAttachAbility(byte level, byte abilityRemain, out BaseAction act)
    {
        //加个神祝祷
        if (Actions.DivineBenison.TryUseAction(level, out act)) return true;

        //加个无中生有
        if (Actions.ThinAir.TryUseAction(level, out act)) return true;

        //加个神速咏唱
        if (Actions.PresenseOfMind.TryUseAction(level, out act)) return true;

        return false;
    }

    private protected override bool EmergercyGCD(byte level, uint lastComboActionID, out BaseAction act)
    {
        //有某些非常危险的状态。
        if (TargetHelper.DyingPeople.Length > 0)
        {
            if (GeneralActions.Esuna.TryUseAction(level, out act, mustUse: true)) return true;
        }

        //有人死了，看看能不能救。
        if (TargetHelper.DeathPeopleParty.Length > 0)
        {
            //如果有人倒了，赶紧即刻拉人！
            if (!GeneralActions.Swiftcast.CoolDown.IsCooldown || HaveSwift)
            {
                if (Actions.Raise.TryUseAction(level, out act, mustUse: true)) return true;
            }
        }

        act = null;
        return false;
    }

    private protected override bool HealAreaAbility(byte level, byte abilityRemain, out BaseAction act)
    {
        //庇护所
        if (Actions.Asylum.TryUseAction(level, out act)) return true;

        //加个法令
        if (Actions.Assize.TryUseAction(level, out act)) return true;

        return false;
    }

    private protected override bool HealSingleAbility(byte level, byte abilityRemain, out BaseAction act)
    {
        //神名
        if (Actions.Tetragrammaton.TryUseAction(level, out act)) return true;

        return false;
    }

    private protected override bool HealSingleGCD(byte level, uint lastComboActionID, out BaseAction act)
    {
        //安慰之心
        if (Actions.AfflatusSolace.TryUseAction(level, out act)) return true;

        //再生
        if (Actions.Regen.TryUseAction(level, out act)) return true;

        //救疗
        if (Actions.Cure2.TryUseAction(level, out act)) return true;

        //治疗
        if (Actions.Cure.TryUseAction(level, out act)) return true;

        return false;
    }

    private protected override bool FirstActionAbility(byte level, byte abilityRemain, BaseAction nextGCD, out BaseAction act)
    {
        if (base.FirstActionAbility(level, abilityRemain, nextGCD, out act)) return true;

        if (nextGCD.ActionID == Actions.Medica.ActionID || nextGCD.ActionID == Actions.Medica2.ActionID ||
            nextGCD.ActionID == Actions.Cure3.ActionID || nextGCD.ActionID == Actions.AfflatusRapture.ActionID)
        {
            //加个全大赦
            if (Actions.PlenaryIndulgence.TryUseAction(level, out act)) return true;
        }

        return false;
    }

    private protected override bool GeneralAbility(byte level, byte abilityRemain, out BaseAction act)
    {
        //加个醒梦
        if (GeneralActions.LucidDreaming.TryUseAction(level, out act)) return true;

        return false;
    }

    private protected override bool GeneralGCD(byte level, uint lastComboActionID, out BaseAction act)
    {
        //苦难之心
        if (Actions.AfflatusMisery.OtherCheck() && Actions.AfflatusMisery.TryUseAction(level, out act, mustUse: true)) return true;

        //群体输出
        if (Actions.Holy.TryUseAction(level, out act)) return true;

        //单体输出
        if (Actions.Aero.TryUseAction(level, out act)) return true;
        if (Actions.Stone.TryUseAction(level, out act)) return true;

        act = null;
        return false;
    }

    internal static bool UseBenediction()
    {
        TargetHelper.GetDangerousTanks(out float[] times);
        if (times.Length > 0)
        {
            foreach (var time in times)
            {
                if (time < 2) return true;
            }
            return false;
        }
        foreach (var member in TargetHelper.PartyMembers)
        {
            //如果没有人要搞死自己，那么就看看有没有人快死了。
            if ((float)member.CurrentHp / member.MaxHp < 0.15 && member.CurrentHp != 0 && TargetHelper.PartyMembersDifferHP > 0.2)
            {
                return true;
            }
        }
        return false;
    }
}
