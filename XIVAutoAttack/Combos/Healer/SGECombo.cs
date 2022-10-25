using Dalamud.Game.ClientState.JobGauge.Types;
using Dalamud.Game.ClientState.Objects.Types;
using Lumina.Excel.GeneratedSheets;
using System.Collections.Generic;
using System.Linq;
using XIVAutoAttack.Actions;
using XIVAutoAttack.Actions.BaseAction;
using XIVAutoAttack.Combos.CustomCombo;
using XIVAutoAttack.Configuration;
using XIVAutoAttack.Helpers;
using XIVAutoAttack.Helpers.TargetHelper;

namespace XIVAutoAttack.Combos.Healer;

internal class SGECombo : JobGaugeCombo<SGEGauge>
{
    internal override uint JobID => 40;
    internal static byte level => Service.ClientState.LocalPlayer!.Level;

    private protected override BaseAction Raise => Actions.Egeiro;
    protected override bool CanHealSingleSpell => base.CanHealSingleSpell && (Config.GetBoolByName("GCDHeal") || TargetUpdater.PartyHealers.Length < 2);
    protected override bool CanHealAreaSpell => base.CanHealAreaSpell && (Config.GetBoolByName("GCDHeal") || TargetUpdater.PartyHealers.Length < 2);
    internal struct Actions
    {
        public static readonly BaseAction
            //复苏
            Egeiro = new(24287),

            //注药
            Dosis = new(24283),

            //均衡注药
            EukrasianDosis = new (24283, isDot: true)
            {
                TargetStatus = new ushort[] 
                { 
                    ObjectStatus.EukrasianDosis, 
                    ObjectStatus.EukrasianDosis2, 
                    ObjectStatus.EukrasianDosis3
                },
            },

            //发炎
            Phlegma = new(24289),
            //发炎2
            Phlegma2 = new(24307),
            //发炎3
            Phlegma3 = new(24313),

            //诊断
            Diagnosis = new(24284, true),

            //心关
            Kardia = new(24285, true)
            {
                BuffsProvide = new ushort[] { ObjectStatus.Kardia },
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
            Prognosis = new(24286, true, shouldEndSpecial: true),

            //自生
            Physis = new(24288, true),

            //自生2
            Physis2 = new(24302, true),

            //均衡
            Eukrasia = new(24290)
            {
                OtherCheck = b => !JobGauge.Eukrasia,
            },

            //拯救
            Soteria = new(24294, true)
            {
                ChoiceTarget = Targets =>
                {
                    foreach (var friend in Targets)
                    {
                        if (friend.HaveStatus(ObjectStatus.Kardion))
                        {
                            return friend;
                        }
                    }
                    return null;
                },
                OtherCheck = b => b.GetHealthRatio() < 0.7,
            },

            //神翼
            Icarus = new(24295, shouldEndSpecial: true)
            {
                ChoiceTarget = TargetFilter.FindMoveTarget,
            },

            //灵橡清汁
            Druochole = new(24296, true)
            {
                OtherCheck = b => JobGauge.Addersgall > 0 && HealHelper.SingleHeal(b, 600, 0.9, 0.85),
            },

            //失衡
            Dyskrasia = new(24297),

            //坚角清汁
            Kerachole = new(24298, true)
            {
                OtherCheck = b => JobGauge.Addersgall > 0,
            },

            //寄生清汁
            Ixochole = new(24299, true)
            {
                OtherCheck = b => JobGauge.Addersgall > 0,
            },

            //活化
            Zoe = new(24300),

            //白牛清汁
            Taurochole = new(24303, true)
            {
                ChoiceTarget = TargetFilter.FindAttackedTarget,
                OtherCheck = b => JobGauge.Addersgall > 0,
            },

            //箭毒
            Toxikon = new(24304),

            //输血
            Haima = new(24305, true)
            {
                ChoiceTarget = TargetFilter.FindAttackedTarget,
            },

            //均衡诊断
            EukrasianDiagnosis = new(24291, true)
            {
                ChoiceTarget = TargetFilter.FindAttackedTarget,
            },

            //均衡预后
            EukrasianPrognosis = new(24292, true)
            {
                ChoiceTarget = TargetFilter.FindAttackedTarget,
            },

            //根素
            Rhizomata = new(24309),

            //整体论
            Holos = new(24310, true),

            //泛输血
            Panhaima = new(24311, true),

            //混合
            Krasis = new(24317, true),

            //魂灵风息
            Pneuma = new(24318),

            //消化
            Pepsis = new(24301, true)
            {
                OtherCheck = b =>
                {
                    foreach (var chara in TargetUpdater.PartyMembers)
                    {
                        if (chara.StatusList.Select(s => s.StatusId).Intersect(new uint[]
                        {
                            ObjectStatus.EukrasianDiagnosis,
                            ObjectStatus.EukrasianPrognosis,
                        }).Any()
                        && StatusHelper.FindStatusTime(b, ObjectStatus.EukrasianDiagnosis, ObjectStatus.EukrasianPrognosis) < 3
                        && chara.GetHealthRatio() < 0.9) return true;
                    }

                    return false;
                },
            };

    }

    private protected override ActionConfiguration CreateConfiguration()
    {
        return base.CreateConfiguration().SetBool("GCDHeal", false, "自动用GCD奶");
    }

    internal override SortedList<DescType, string> Description => new()
    {
        {DescType.范围治疗, $"GCD: {Actions.Prognosis.Action.Name}\n                     能力: {Actions.Holos.Action.Name}, {Actions.Ixochole.Action.Name}, {Actions.Physis2.Action.Name}, {Actions.Physis.Action.Name}"},
        {DescType.单体治疗, $"GCD: {Actions.Diagnosis.Action.Name}\n                     能力: {Actions.Druochole.Action.Name}"},
        {DescType.范围防御, $"{Actions.Panhaima.Action.Name}, {Actions.Kerachole.Action.Name}, {Actions.Prognosis.Action.Name}"},
        {DescType.单体防御, $"GCD: {Actions.Diagnosis.Action.Name}\n                     能力: {Actions.Haima.Action.Name}, {Actions.Taurochole.Action.Name}"},
        {DescType.移动, $"{Actions.Icarus.Action.Name}，目标为面向夹角小于30°内最远目标。"},
    };
    private protected override bool ForAttachAbility(byte abilityRemain, out IAction act)
    {
        act = null!;
        return false;
    }

    private protected override bool EmergercyAbility(byte abilityRemain, IAction nextGCD, out IAction act)
    {
        if (base.EmergercyAbility(abilityRemain, nextGCD, out act)) return true;

        //下个技能是
        if (nextGCD.IsAnySameAction(false, Actions.Pneuma , Actions.EukrasianDiagnosis, 
            Actions.EukrasianPrognosis , Actions.Diagnosis , Actions.Prognosis))
        {
            //活化
            if (Actions.Zoe.ShouldUse(out act)) return true;
        }

        if (nextGCD == Actions.Diagnosis)
        {
            //混合
            if (Actions.Krasis.ShouldUse(out act)) return true;
        }

        act = null;
        return false;

    }

    private protected override bool DefenceSingleAbility(byte abilityRemain, out IAction act)
    {

        if (JobGauge.Addersgall == 0)
        {
            //输血
            if (Actions.Haima.ShouldUse(out act)) return true;
        }

        //白牛清汁
        if (Actions.Taurochole.ShouldUse(out act)) return true;

        act = null!;
        return false;
    }

    private protected override bool DefenseSingleGCD(uint lastComboActionID, out IAction act)
    {
        //诊断
        if (Actions.EukrasianDiagnosis.ShouldUse(out act))
        {
            if (Actions.EukrasianDiagnosis.Target.StatusList.Select(s => s.StatusId).Intersect(new uint[]
            {
                ObjectStatus.EukrasianDiagnosis,
                ObjectStatus.EukrasianPrognosis,
                ObjectStatus.Galvanize,
            }).Count() > 0) return false;

            //均衡
            if (Actions.Eukrasia.ShouldUse(out act)) return true;

            act = Actions.EukrasianDiagnosis;
            return true;
        }

        act = null!;
        return false;
    }

    private protected override bool DefenceAreaAbility(byte abilityRemain, out IAction act)
    {
        //泛输血
        if (JobGauge.Addersgall == 0 && TargetUpdater.PartyMembersAverHP < 0.7)
        {
            if (Actions.Panhaima.ShouldUse(out act)) return true;
        }

        //坚角清汁
        if (Actions.Kerachole.ShouldUse(out act)) return true;

        //整体论
        if (Actions.Holos.ShouldUse(out act)) return true;

        act = null!;
        return false;
    }

    private protected override bool DefenseAreaGCD(uint lastComboActionID, out IAction act)
    {
        //预后
        if (Actions.EukrasianPrognosis.ShouldUse(out act))
        {
            if (Actions.EukrasianPrognosis.Target.StatusList.Select(s => s.StatusId).Intersect(new uint[]
            {
                ObjectStatus.EukrasianDiagnosis,
                ObjectStatus.EukrasianPrognosis,
                ObjectStatus.Galvanize,
            }).Count() > 0) return false;

            //均衡
            if (Actions.Eukrasia.ShouldUse(out act)) return true;

            act = Actions.EukrasianPrognosis;
            return true;
        }

        act = null!;
        return false;
    }

    private protected override bool MoveAbility(byte abilityRemain, out IAction act)
    {
        //神翼
        if (Actions.Icarus.ShouldUse(out act)) return true;
        return false;
    }

    private protected override bool GeneralAbility(byte abilityRemain, out IAction act)
    {
        //心关
        if (Actions.Kardia.ShouldUse(out act)) return true;

        //根素
        if (JobGauge.Addersgall == 0 && Actions.Rhizomata.ShouldUse(out act)) return true;

        //拯救
        if (Actions.Soteria.ShouldUse(out act)) return true;

        //消化
        if (Actions.Pepsis.ShouldUse(out act)) return true;

        act = null!;
        return false;
    }

    private protected override bool GeneralGCD(uint lastComboActionID, out IAction act)
    {
        //箭毒
        if (JobGauge.Addersting == 3 && Actions.Toxikon.ShouldUse(out act, mustUse: true)) return true;

        var level = Level;
        //发炎
        if (Actions.Phlegma3.ShouldUse(out act, mustUse: Actions.Phlegma3.RecastTimeRemain < 4, emptyOrSkipCombo: true)) return true;
        if (level < Actions.Phlegma3.Level && Actions.Phlegma2.ShouldUse(out act, mustUse: Actions.Phlegma2.RecastTimeRemain < 4, emptyOrSkipCombo: true)) return true;
        if (level < Actions.Phlegma2.Level && Actions.Phlegma.ShouldUse(out act, mustUse: Actions.Phlegma.RecastTimeRemain < 4, emptyOrSkipCombo: true)) return true;

        //失衡
        if (Actions.Dyskrasia.ShouldUse(out act)) return true;

        if(Actions.EukrasianDosis.ShouldUse(out var enAct))
        {
            //补上Dot
            if (Actions.Eukrasia.ShouldUse(out act)) return true;
            act = enAct;
            return true;
        }
        else if (JobGauge.Eukrasia)
        {
            if (DefenseAreaGCD(lastComboActionID, out act)) return true;
            if (DefenseSingleGCD(lastComboActionID, out act)) return true;
        }

        //注药
        if (Actions.Dosis.ShouldUse(out act)) return true;

        //发炎
        if (Actions.Phlegma3.ShouldUse(out act, mustUse: true)) return true;
        if (level < Actions.Phlegma3.Level && Actions.Phlegma2.ShouldUse(out act, mustUse: true)) return true;
        if (level < Actions.Phlegma2.Level && Actions.Phlegma.ShouldUse(out act, mustUse: true)) return true;

        //箭毒
        if (JobGauge.Addersting > 0 && Actions.Toxikon.ShouldUse(out act, mustUse: true)) return true;

        //脱战给T刷单盾嫖豆子
        if (!InBattle)
        {
            var tank = TargetUpdater.PartyTanks;
            if (tank.Length == 1 && Actions.EukrasianDiagnosis.Target == tank.First() && Actions.EukrasianDiagnosis.ShouldUse(out act))
            {
                if (tank.First().StatusList.Select(s => s.StatusId).Intersect(new uint[]
                {
                ObjectStatus.EukrasianDiagnosis,
                ObjectStatus.EukrasianPrognosis,
                ObjectStatus.Galvanize,
                }).Any()) return false;

                //均衡
                if (Actions.Eukrasia.ShouldUse(out act)) return true;

                act = Actions.EukrasianDiagnosis;
                return true;
            }
            if (Actions.Eukrasia.ShouldUse(out act)) return true;
        }

        return false;
    }

    private protected override bool HealSingleAbility(byte abilityRemain, out IAction act)
    {
        //白牛青汁
        if (Actions.Taurochole.ShouldUse(out act)) return true;

        //灵橡清汁
        if (Actions.Druochole.ShouldUse(out act)) return true;

        //当资源不足时加入范围治疗缓解压力
        var tank = TargetUpdater.PartyTanks;
        var isBoss = Actions.Dosis.Target.IsBoss();
        if (JobGauge.Addersgall == 0 && tank.Length == 1 && tank.Any(t => t.GetHealthRatio() < 0.6f) && !isBoss)
        {
            //整体论
            if (Actions.Holos.ShouldUse(out act)) return true;

            //自生2
            if (Actions.Physis2.ShouldUse(out act)) return true;
            //自生
            if (Level < Actions.Physis2.Level && Actions.Physis.ShouldUse(out act)) return true;

            //泛输血
            if (Actions.Panhaima.ShouldUse(out act)) return true;
        }

        act = null!;
        return false;
    }

    private protected override bool HealSingleGCD(uint lastComboActionID, out IAction act)
    {
        //诊断
        if (Actions.EukrasianDiagnosis.ShouldUse(out act))
        {
            if (Actions.EukrasianDiagnosis.Target.StatusList.Select(s => s.StatusId).Intersect(new uint[]
            {
                //均衡诊断
                ObjectStatus.EukrasianDiagnosis,
                ObjectStatus.EukrasianPrognosis,
                ObjectStatus.Galvanize,
            }).Count() > 0)
            {
                if (Actions.Diagnosis.ShouldUse(out act)) return true;
            }

            //均衡
            if (Actions.Eukrasia.ShouldUse(out act)) return true;

            act = Actions.EukrasianDiagnosis;
            return true;
        }

        //诊断
        if (Actions.Diagnosis.ShouldUse(out act)) return true;
        return false;
    }

    private protected override bool HealAreaGCD(uint lastComboActionID, out IAction act)
    {
        if (TargetUpdater.PartyMembersAverHP < 0.55f)
        {
            //魂灵风息
            if (Actions.Pneuma.ShouldUse(out act, mustUse: true)) return true;
        }

        if (Actions.EukrasianPrognosis.ShouldUse(out act))
        {
            if (Actions.EukrasianPrognosis.Target.StatusList.Select(s => s.StatusId).Intersect(new uint[]
            {
                //均衡诊断
                ObjectStatus.EukrasianDiagnosis,
                ObjectStatus.EukrasianPrognosis,
                ObjectStatus.Galvanize,
            }).Count() > 0)
            {
                if (Actions.Prognosis.ShouldUse(out act)) return true;
            }

            //均衡
            if (Actions.Eukrasia.ShouldUse(out act)) return true;

            act = Actions.EukrasianPrognosis;
            return true;
        }

        //预后
        if (Actions.Prognosis.ShouldUse(out act)) return true;
        return false;
    }
    private protected override bool HealAreaAbility(byte abilityRemain, out IAction act)
    {
        var level = Level;

        //坚角清汁
        if (Actions.Kerachole.ShouldUse(out act) && level >= 78) return true;

        //自生2
        if (Actions.Physis2.ShouldUse(out act)) return true;
        //自生
        if (level < Actions.Physis2.Level && Actions.Physis.ShouldUse(out act)) return true;

        //整体论
        if (Actions.Holos.ShouldUse(out act) && TargetUpdater.PartyMembersAverHP < 0.65f) return true;

        //寄生清汁
        if (Actions.Ixochole.ShouldUse(out act)) return true;

        return false;
    }
}
