using Dalamud.Game.ClientState.JobGauge.Types;
using System.Collections.Generic;
using System.Linq;
using XIVAutoAttack.Actions;
using XIVAutoAttack.Combos.CustomCombo;
using XIVAutoAttack.Configuration;

namespace XIVAutoAttack.Combos.Healer;

internal class SGECombo : JobGaugeCombo<SGEGauge>
{
    internal override uint JobID => 40;

    private protected override BaseAction Raise => Actions.Egeiro;
    protected override bool CanHealSingleSpell => base.CanHealSingleSpell && (Config.GetBoolByName("GCDHeal") || TargetHelper.PartyHealers.Length < 2);
    protected override bool CanHealAreaSpell => base .CanHealAreaSpell && ( Config.GetBoolByName("GCDHeal") || TargetHelper.PartyHealers.Length < 2);
    internal struct Actions
    {
        public static readonly BaseAction
            //复苏
            Egeiro = new (24287),

            //注药
            Dosis = new (24283),

            //发炎
            Phlegma = new (24289),
            //发炎2
            Phlegma2 = new (24307),
            //发炎3
            Phlegma3 = new (24313),

            //诊断
            Diagnosis = new (24284, true),

            //心关
            Kardia = new(24285, true)
            {
                BuffsProvide = new ushort[] {ObjectStatus.Kardia},
                ChoiceTarget = Targets =>
                {
                    var targets = TargetFilter.GetJobCategory(Targets, Role.防护);
                    targets = targets.Length == 0 ? Targets : targets;

                    if (targets.Length == 0) return null;

                    foreach (var tar in targets)
                    {
                        if (tar.TargetObject?.TargetObject?.ObjectId == tar.ObjectId)
                        {
                            return tar;
                        }
                    }

                    return targets[0];
                },
                OtherCheck = b =>
                {
                    foreach (var status in b.StatusList)
                    {
                        if (status.SourceID == Service.ClientState.LocalPlayer.ObjectId
                            && status.StatusId == ObjectStatus.Kardion)
                        {
                            return false;
                        }
                    }
                    return true;
                },
            },

            //预后
            Prognosis = new (24286, true, shouldEndSpecial: true),

            //自生
            Physis = new (24288, true),

            //自生2
            Physis2 = new (24302, true),

            //均衡
            Eukrasia = new (24290)
            {
                OtherCheck = b => !JobGauge.Eukrasia,
            },

            //拯救
            Soteria = new (24294, true)
            {
                ChoiceTarget = Targets =>
                {
                    foreach (var friend in Targets)
                    {
                        var statuses = friend.StatusList.Select(status => status.StatusId);
                        if (statuses.Contains(ObjectStatus.Kardion))
                        {
                            return friend;
                        }
                    }
                    return null;
                },
                OtherCheck = b => (float)b.CurrentHp / b.MaxHp < 0.7,
            },

            //神翼
            Icarus = new (24295, shouldEndSpecial: true)
            {
                ChoiceTarget = TargetFilter.FindMoveTarget,
            },

            //灵橡清汁
            Druochole = new (24296, true),

            //失衡
            Dyskrasia = new (24297),

            //坚角清汁
            Kerachole = new (24298, true),

            //寄生清汁
            Ixochole = new (24299, true),

            //活化
            Zoe = new (24300),

            //白牛清汁
            Taurochole = new (24303, true)
            {
                ChoiceTarget = TargetFilter.FindAttackedTarget,
            },

            //箭毒
            Toxikon = new (24304),

            //输血
            Haima = new (24305, true)
            {
                ChoiceTarget = TargetFilter.FindAttackedTarget,
            },

            //均衡诊断
            EukrasianDiagnosis = new (24291, true) 
            {
                ChoiceTarget = TargetFilter.FindAttackedTarget,
            },

            //均衡诊断
            EukrasianPrognosis = new (24292, true)
            {
                ChoiceTarget = TargetFilter.FindAttackedTarget,
            },

            //根素
            Rhizomata = new (24309),

            //整体论
            Holos = new (24310, true),

            //泛输血
            Panhaima = new (24311, true),

            //混合
            Krasis = new (24317, true),

            //魂灵风息
            Pneuma = new (24318);
    }

    private protected override ActionConfiguration CreateConfiguration()
    {
        return base.CreateConfiguration().SetBool("GCDHeal", false, "自动用GCD奶");
    }

    internal override SortedList<DescType, string> Description => new ()
    {
        {DescType.范围治疗, $"GCD: {Actions.Prognosis.Action.Name}\n                     能力: {Actions.Holos.Action.Name}, {Actions.Ixochole.Action.Name}, {Actions.Physis2.Action.Name}, {Actions.Physis.Action.Name}"},
        {DescType.单体治疗, $"GCD: {Actions.Diagnosis.Action.Name}\n                     能力: {Actions.Druochole.Action.Name}"},
        {DescType.范围防御, $"{Actions.Panhaima.Action.Name}, {Actions.Kerachole.Action.Name}, {Actions.Prognosis.Action.Name}"},
        {DescType.单体防御, $"GCD: {Actions.Diagnosis.Action.Name}\n                     能力: {Actions.Haima.Action.Name}, {Actions.Taurochole.Action.Name}"},
        {DescType.移动, $"{Actions.Icarus.Action.Name}，目标为面向夹角小于30°内最远目标。"},
    };
    private protected override bool ForAttachAbility(byte abilityRemain, out IAction act)
    {
        act = null;
        return false;
    }

    private protected override bool EmergercyAbility(byte abilityRemain, IAction nextGCD, out IAction act)
    {
        if (base.EmergercyAbility(abilityRemain, nextGCD, out act)) return true;

        if (nextGCD.ID == Actions.Diagnosis.ID ||
            nextGCD.ID == Actions.Prognosis.ID)
        {
            //活化
            if (Actions.Zoe.ShouldUseAction(out act)) return true;
        }

        if (nextGCD.ID == Actions.Diagnosis.ID)
        {
            //混合
            if (Actions.Krasis.ShouldUseAction(out act)) return true;
        }

        act = null;
        return false;

    }

    private protected override bool DefenceSingleAbility(byte abilityRemain, out IAction act)
    {
        //输血
        if (Actions.Haima.ShouldUseAction(out act)) return true;

        if (JobGauge.Addersgall > 0)
        {
            //白牛清汁
            if (Actions.Taurochole.ShouldUseAction(out act)) return true;
        }

        return false;
    }

    private protected override bool DefenseSingleGCD(uint lastComboActionID, out IAction act)
    {
        //诊断
        if (Actions.EukrasianDiagnosis.ShouldUseAction(out act))
        {
            if (Actions.EukrasianDiagnosis.Target.StatusList.Select(s => s.StatusId).Intersect(new uint[]
            {
                ObjectStatus.EukrasianDiagnosis,
                ObjectStatus.EukrasianPrognosis,
                ObjectStatus.Galvanize,
            }).Count() > 0) return false;

            //均衡
            if (Actions.Eukrasia.ShouldUseAction(out act)) return true;

            act = Actions.EukrasianDiagnosis;
            return true;
        }

        act = null;
        return false;
    }

    private protected override bool DefenceAreaAbility(byte abilityRemain, out IAction act)
    {
        //泛输血
        if (Actions.Panhaima.ShouldUseAction(out act)) return true;

        if (JobGauge.Addersgall > 0)
        {
            //坚角清汁
            if (Actions.Kerachole.ShouldUseAction(out act)) return true;
        }

        //预后
        if (Actions.EukrasianPrognosis.ShouldUseAction(out act))
        {
            if (Actions.EukrasianPrognosis.Target.StatusList.Select(s => s.StatusId).Intersect(new uint[]
            {
                ObjectStatus.EukrasianDiagnosis,
                ObjectStatus.EukrasianPrognosis,
                ObjectStatus.Galvanize,
            }).Count() > 0) return false;

            //均衡
            if (Actions.Eukrasia.ShouldUseAction(out act)) return true;

            act = Actions.EukrasianPrognosis;
            return true;
        }

        act = null;
        return false;
    }

    private protected override bool DefenseAreaGCD(uint lastComboActionID, out IAction act)
    {
        //预后
        if (Actions.EukrasianPrognosis.ShouldUseAction(out act))
        {
            if (Actions.EukrasianPrognosis.Target.StatusList.Select(s => s.StatusId).Intersect(new uint[]
            {
                ObjectStatus.EukrasianDiagnosis,
                ObjectStatus.EukrasianPrognosis,
                ObjectStatus.Galvanize,
            }).Count() > 0) return false;

            //均衡
            if (Actions.Eukrasia.ShouldUseAction(out act)) return true;

            act = Actions.EukrasianPrognosis;
            return true;
        }

        act = null;
        return false;
    }
    private protected override bool HealSingleAbility(byte abilityRemain, out IAction act)
    {

        if (JobGauge.Addersgall > 1)
        {
            //灵橡清汁
            if (Actions.Druochole.ShouldUseAction(out act)) return true;
        }

        act = null;
        return false;
    }

    private protected override bool MoveAbility(byte abilityRemain, out IAction act)
    {
        //神翼
        if (Actions.Icarus.ShouldUseAction(out act)) return true;
        return false;
    }

    private protected override bool GeneralAbility(byte abilityRemain, out IAction act)
    {
        //心关
        if (Actions.Kardia.ShouldUseAction(out act)) return true;



        //根素
        if (JobGauge.Addersgall < 2 && Actions.Rhizomata.ShouldUseAction(out act)) return true;

        if (Actions.Soteria.ShouldUseAction(out act)) return true;
        act = null;
        return false;
    }

    private protected override bool GeneralGCD(uint lastComboActionID, out IAction act)
    {
        //魂灵风息
        if (Actions.Pneuma.ShouldUseAction(out act, mustUse: true)) return true;

        //箭毒
        if (JobGauge.Addersting == 3 && Actions.Toxikon.ShouldUseAction(out act, mustUse: true)) return true;

        var level = Service.ClientState.LocalPlayer.Level;
        //发炎
        if (Actions.Phlegma3.ShouldUseAction(out act, mustUse : Actions.Phlegma3.RecastTimeRemain < 4, emptyOrSkipCombo: true)) return true;
        if (level < Actions.Phlegma3.Level && Actions.Phlegma2.ShouldUseAction(out act, mustUse: Actions.Phlegma2.RecastTimeRemain < 4, emptyOrSkipCombo: true)) return true;
        if (level < Actions.Phlegma2.Level && Actions.Phlegma.ShouldUseAction(out act, mustUse: Actions.Phlegma.RecastTimeRemain < 4, emptyOrSkipCombo: true)) return true;

        //失衡
        if (Actions.Dyskrasia.ShouldUseAction(out act)) return true;

        Actions.Dosis.ShouldUseAction(out _);
        var times = StatusHelper.FindStatusFromSelf(Actions.Dosis.Target,
            new ushort[] { ObjectStatus.EukrasianDosis, ObjectStatus.EukrasianDosis2, ObjectStatus.EukrasianDosis3 });
        if (times.Length == 0 || times.Max() < 3)
        {
            if (Actions.Dosis.Target != null)
            {
                //补上Dot
                if (Actions.Eukrasia.ShouldUseAction(out act)) return true;
            }
        }
        else if (JobGauge.Eukrasia)
        {
            if (DefenseAreaGCD(lastComboActionID, out act)) return true;
            if (DefenseSingleGCD(lastComboActionID, out act)) return true;
        }


        //注药
        if (Actions.Dosis.ShouldUseAction(out act)) return true;

        //箭毒
        if (JobGauge.Addersting > 0 && Actions.Toxikon.ShouldUseAction(out act, mustUse: true)) return true;

        //发炎
        if (Actions.Phlegma3.ShouldUseAction(out act, mustUse: true)) return true;
        if (level < Actions.Phlegma3.Level && Actions.Phlegma2.ShouldUseAction(out act, mustUse: true)) return true;
        if (level < Actions.Phlegma2.Level && Actions.Phlegma.ShouldUseAction(out act, mustUse: true)) return true;

        return false;
    }
    private protected override bool HealSingleGCD(uint lastComboActionID, out IAction act)
    {
        //诊断
        if (Actions.Diagnosis.ShouldUseAction(out act)) return true;
        return false;
    }

    private protected override bool HealAreaGCD(uint lastComboActionID, out IAction act)
    {
        //预后
        if (Actions.Prognosis.ShouldUseAction(out act)) return true;
        return false;
    }
    private protected override bool HealAreaAbility(byte abilityRemain, out IAction act)
    {
        //整体论
        if (Actions.Holos.ShouldUseAction(out act)) return true;

        if (JobGauge.Addersgall > 0)
        {
            //寄生清汁
            if (Actions.Ixochole.ShouldUseAction(out act)) return true;
        }
        //自生2
        if (Actions.Physis2.ShouldUseAction(out act)) return true;
        //自生
        if (Service.ClientState.LocalPlayer.Level < Actions.Physis2.Level && Actions.Physis.ShouldUseAction(out act)) return true;
        return false;
    }
}
