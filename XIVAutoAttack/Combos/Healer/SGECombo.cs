using Dalamud.Game.ClientState.JobGauge.Types;
using Dalamud.Game.ClientState.Objects.Types;
using Lumina.Excel.GeneratedSheets;
using System.Collections.Generic;
using System.Linq;
using XIVAutoAttack.Actions;
using XIVAutoAttack.Actions.BaseAction;
using XIVAutoAttack.Combos.Attributes;
using XIVAutoAttack.Combos.CustomCombo;
using XIVAutoAttack.Configuration;
using XIVAutoAttack.Data;
using XIVAutoAttack.Helpers;
using XIVAutoAttack.Updaters;
using static XIVAutoAttack.Combos.Healer.SGECombo;

namespace XIVAutoAttack.Combos.Healer;

[ComboDevInfo(@"https://github.com/ArchiDog1998/XIVAutoAttack/blob/main/XIVAutoAttack/Combos/Healer/SGECombo.cs",
   ComboAuthor.Armolion)]
internal sealed class SGECombo : JobGaugeCombo<SGEGauge, CommandType>
{
    internal enum CommandType : byte
    {
        None,
    }

    protected override SortedList<CommandType, string> CommandDescription => new SortedList<CommandType, string>()
    {
        //{CommandType.None, "" }, //写好注释啊！用来提示用户的。
    };
    public override uint[] JobIDs => new uint[] { 40 };
    internal static byte level => Service.ClientState.LocalPlayer!.Level;

    private protected override BaseAction Raise => Egeiro;
    protected override bool CanHealSingleSpell => base.CanHealSingleSpell && (Config.GetBoolByName("GCDHeal") || TargetUpdater.PartyHealers.Length < 2);
    protected override bool CanHealAreaSpell => base.CanHealAreaSpell && (Config.GetBoolByName("GCDHeal") || TargetUpdater.PartyHealers.Length < 2);

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
                ChoiceTarget = TargetFilter.FindTargetForMoving,
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
                        && b.WillStatusEndGCD(2, 0, true, ObjectStatus.EukrasianDiagnosis, ObjectStatus.EukrasianPrognosis)
                        && chara.GetHealthRatio() < 0.9) return true;
                    }

                    return false;
                },
            };
    private protected override ActionConfiguration CreateConfiguration()
    {
        return base.CreateConfiguration().SetBool("GCDHeal", false, "自动用GCD奶");
    }

    public override SortedList<DescType, string> DescriptionDict => new()
    {
        {DescType.范围治疗, $"GCD: {Prognosis}\n                     能力: {Holos}, {Ixochole}, {Physis2}, {Physis}"},
        {DescType.单体治疗, $"GCD: {Diagnosis}\n                     能力: {Druochole}"},
        {DescType.范围防御, $"{Panhaima}, {Kerachole}, {Prognosis}"},
        {DescType.单体防御, $"GCD: {Diagnosis}\n                     能力: {Haima}, {Taurochole}"},
        {DescType.移动技能, $"{Icarus}，目标为面向夹角小于30°内最远目标。"},
    };
    private protected override bool AttackAbility(byte abilityRemain, out IAction act)
    {
        act = null!;
        return false;
    }

    private protected override bool EmergercyAbility(byte abilityRemain, IAction nextGCD, out IAction act)
    {
        if (base.EmergercyAbility(abilityRemain, nextGCD, out act)) return true;

        //下个技能是
        if (nextGCD.IsAnySameAction(false, Pneuma , EukrasianDiagnosis, 
            EukrasianPrognosis , Diagnosis , Prognosis))
        {
            //活化
            if (Zoe.ShouldUse(out act)) return true;
        }

        if (nextGCD == Diagnosis)
        {
            //混合
            if (Krasis.ShouldUse(out act)) return true;
        }

        act = null;
        return false;
    }

    private protected override bool DefenceSingleAbility(byte abilityRemain, out IAction act)
    {

        if (JobGauge.Addersgall == 0)
        {
            //输血
            if (Haima.ShouldUse(out act)) return true;
        }

        //白牛清汁
        if (Taurochole.ShouldUse(out act)) return true;

        act = null!;
        return false;
    }

    private protected override bool DefenseSingleGCD(out IAction act)
    {
        //诊断
        if (EukrasianDiagnosis.ShouldUse(out act))
        {
            if (EukrasianDiagnosis.Target.StatusList.Select(s => s.StatusId).Intersect(new uint[]
            {
                ObjectStatus.EukrasianDiagnosis,
                ObjectStatus.EukrasianPrognosis,
                ObjectStatus.Galvanize,
            }).Count() > 0) return false;

            //均衡
            if (Eukrasia.ShouldUse(out act)) return true;

            act = EukrasianDiagnosis;
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
            if (Panhaima.ShouldUse(out act)) return true;
        }

        //坚角清汁
        if (Kerachole.ShouldUse(out act)) return true;

        //整体论
        if (Holos.ShouldUse(out act)) return true;

        act = null!;
        return false;
    }

    private protected override bool DefenseAreaGCD(out IAction act)
    {
        //预后
        if (EukrasianPrognosis.ShouldUse(out act))
        {
            if (EukrasianPrognosis.Target.StatusList.Select(s => s.StatusId).Intersect(new uint[]
            {
                ObjectStatus.EukrasianDiagnosis,
                ObjectStatus.EukrasianPrognosis,
                ObjectStatus.Galvanize,
            }).Count() > 0) return false;

            //均衡
            if (Eukrasia.ShouldUse(out act)) return true;

            act = EukrasianPrognosis;
            return true;
        }

        act = null!;
        return false;
    }

    private protected override bool MoveAbility(byte abilityRemain, out IAction act)
    {
        //神翼
        if (Icarus.ShouldUse(out act)) return true;
        return false;
    }

    private protected override bool GeneralAbility(byte abilityRemain, out IAction act)
    {
        //心关
        if (Kardia.ShouldUse(out act)) return true;

        //根素
        if (JobGauge.Addersgall == 0 && Rhizomata.ShouldUse(out act)) return true;

        //拯救
        if (Soteria.ShouldUse(out act)) return true;

        //消化
        if (Pepsis.ShouldUse(out act)) return true;

        act = null!;
        return false;
    }

    private protected override bool GeneralGCD(out IAction act)
    {
        //箭毒
        if (JobGauge.Addersting == 3 && Toxikon.ShouldUse(out act, mustUse: true)) return true;

        var level = Level;
        //发炎
        if (Phlegma3.ShouldUse(out act, mustUse: Phlegma3.WillHaveOneChargeGCD(2), emptyOrSkipCombo: true)) return true;
        if (!Phlegma3.EnoughLevel && Phlegma2.ShouldUse(out act, mustUse: Phlegma2.WillHaveOneChargeGCD(2), emptyOrSkipCombo: true)) return true;
        if (!Phlegma2.EnoughLevel && Phlegma.ShouldUse(out act, mustUse: Phlegma.WillHaveOneChargeGCD(2), emptyOrSkipCombo: true)) return true;

        //失衡
        if (Dyskrasia.ShouldUse(out act)) return true;

        if(EukrasianDosis.ShouldUse(out var enAct))
        {
            //补上Dot
            if (Eukrasia.ShouldUse(out act)) return true;
            act = enAct;
            return true;
        }
        else if (JobGauge.Eukrasia)
        {
            if (DefenseAreaGCD(out act)) return true;
            if (DefenseSingleGCD(out act)) return true;
        }

        //注药
        if (Dosis.ShouldUse(out act)) return true;

        //发炎
        if (Phlegma3.ShouldUse(out act, mustUse: true)) return true;
        if (!Phlegma3.EnoughLevel && Phlegma2.ShouldUse(out act, mustUse: true)) return true;
        if (!Phlegma2.EnoughLevel && Phlegma.ShouldUse(out act, mustUse: true)) return true;

        //箭毒
        if (JobGauge.Addersting > 0 && Toxikon.ShouldUse(out act, mustUse: true)) return true;

        //脱战给T刷单盾嫖豆子
        if (!InCombat)
        {
            var tank = TargetUpdater.PartyTanks;
            if (tank.Length == 1 && EukrasianDiagnosis.Target == tank.First() && EukrasianDiagnosis.ShouldUse(out act))
            {
                if (tank.First().StatusList.Select(s => s.StatusId).Intersect(new uint[]
                {
                ObjectStatus.EukrasianDiagnosis,
                ObjectStatus.EukrasianPrognosis,
                ObjectStatus.Galvanize,
                }).Any()) return false;

                //均衡
                if (Eukrasia.ShouldUse(out act)) return true;

                act = EukrasianDiagnosis;
                return true;
            }
            if (Eukrasia.ShouldUse(out act)) return true;
        }

        return false;
    }

    private protected override bool HealSingleAbility(byte abilityRemain, out IAction act)
    {
        //白牛青汁
        if (Taurochole.ShouldUse(out act)) return true;

        //灵橡清汁
        if (Druochole.ShouldUse(out act)) return true;

        //当资源不足时加入范围治疗缓解压力
        var tank = TargetUpdater.PartyTanks;
        var isBoss = Dosis.Target.IsBoss();
        if (JobGauge.Addersgall == 0 && tank.Length == 1 && tank.Any(t => t.GetHealthRatio() < 0.6f) && !isBoss)
        {
            //整体论
            if (Holos.ShouldUse(out act)) return true;

            //自生2
            if (Physis2.ShouldUse(out act)) return true;
            //自生
            if (!Physis2.EnoughLevel && Physis.ShouldUse(out act)) return true;

            //泛输血
            if (Panhaima.ShouldUse(out act)) return true;
        }

        act = null!;
        return false;
    }

    private protected override bool HealSingleGCD(out IAction act)
    {
        //诊断
        if (EukrasianDiagnosis.ShouldUse(out act))
        {
            if (EukrasianDiagnosis.Target.StatusList.Select(s => s.StatusId).Intersect(new uint[]
            {
                //均衡诊断
                ObjectStatus.EukrasianDiagnosis,
                ObjectStatus.EukrasianPrognosis,
                ObjectStatus.Galvanize,
            }).Count() > 0)
            {
                if (Diagnosis.ShouldUse(out act)) return true;
            }

            //均衡
            if (Eukrasia.ShouldUse(out act)) return true;

            act = EukrasianDiagnosis;
            return true;
        }

        //诊断
        if (Diagnosis.ShouldUse(out act)) return true;
        return false;
    }

    private protected override bool HealAreaGCD(out IAction act)
    {
        if (TargetUpdater.PartyMembersAverHP < 0.55f)
        {
            //魂灵风息
            if (Pneuma.ShouldUse(out act, mustUse: true)) return true;
        }

        if (EukrasianPrognosis.ShouldUse(out act))
        {
            if (EukrasianPrognosis.Target.StatusList.Select(s => s.StatusId).Intersect(new uint[]
            {
                //均衡诊断
                ObjectStatus.EukrasianDiagnosis,
                ObjectStatus.EukrasianPrognosis,
                ObjectStatus.Galvanize,
            }).Count() > 0)
            {
                if (Prognosis.ShouldUse(out act)) return true;
            }

            //均衡
            if (Eukrasia.ShouldUse(out act)) return true;

            act = EukrasianPrognosis;
            return true;
        }

        //预后
        if (Prognosis.ShouldUse(out act)) return true;
        return false;
    }
    private protected override bool HealAreaAbility(byte abilityRemain, out IAction act)
    {
        //坚角清汁
        if (Kerachole.ShouldUse(out act) && level >= Level) return true;

        //自生2
        if (Physis2.ShouldUse(out act)) return true;
        //自生
        if (!Physis2.EnoughLevel && Physis.ShouldUse(out act)) return true;

        //整体论
        if (Holos.ShouldUse(out act) && TargetUpdater.PartyMembersAverHP < 0.65f) return true;

        //寄生清汁
        if (Ixochole.ShouldUse(out act)) return true;

        return false;
    }
}
