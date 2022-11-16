using Dalamud.Game.ClientState.JobGauge.Types;
using Dalamud.Game.ClientState.Objects.Types;
using Lumina.Excel.GeneratedSheets;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XIVAutoAttack.Actions;
using XIVAutoAttack.Actions.BaseAction;
using XIVAutoAttack.Combos.Attributes;
using XIVAutoAttack.Combos.Basic;
using XIVAutoAttack.Combos.CustomCombo;
using XIVAutoAttack.Configuration;
using XIVAutoAttack.Data;
using XIVAutoAttack.Helpers;
using XIVAutoAttack.Updaters;
using static XIVAutoAttack.Combos.Healer.SGECombos.SGECombo_Default;

namespace XIVAutoAttack.Combos.Healer.SGECombos;

[ComboDevInfo(@"https://github.com/ArchiDog1998/XIVAutoAttack/blob/main/XIVAutoAttack/Combos/Healer/SGECombos/SGECombo_Default.cs")]
internal sealed class SGECombo_Default : SGECombo_Base<CommandType>
{
    public override string Author => "汐ベMoon";

    internal enum CommandType : byte
    {
        None,
    }

    protected override SortedList<CommandType, string> CommandDescription => new SortedList<CommandType, string>()
    {
        //{CommandType.None, "" }, //写好注释啊！用来提示用户的。
    };

    protected override bool CanHealSingleSpell => base.CanHealSingleSpell && (Config.GetBoolByName("GCDHeal") || TargetUpdater.PartyHealers.Length < 2);
    protected override bool CanHealAreaSpell => base.CanHealAreaSpell && (Config.GetBoolByName("GCDHeal") || TargetUpdater.PartyHealers.Length < 2);

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
        if (nextGCD.IsAnySameAction(false, Pneuma, EukrasianDiagnosis,
            EukrasianPrognosis, Diagnosis, Prognosis))
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

        if (Addersgall == 0)
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
            if (EukrasianDiagnosis.Target.HasStatus(true,
                StatusID.EukrasianDiagnosis,
                StatusID.EukrasianPrognosis,
                StatusID.Galvanize
            )) return false;

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
        if (Addersgall == 0 && TargetUpdater.PartyMembersAverHP < 0.7)
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
            if (EukrasianDiagnosis.Target.HasStatus(true,
                StatusID.EukrasianDiagnosis,
                StatusID.EukrasianPrognosis,
                StatusID.Galvanize
            )) return false;

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
        if (Addersgall == 0 && Rhizomata.ShouldUse(out act)) return true;

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
        //三层时并不需要使用，溢出不亏
        //if (JobGauge.Addersting == 3 && Toxikon.ShouldUse(out act, mustUse: true)) return true;

        var level = Level;

        if (EukrasianDosis.ShouldUse(out var enAct))
        {
            //补上Dot
            if (Eukrasia.ShouldUse(out act)) return true;
            act = enAct;
            return true;
        }
        else if (HasEukrasia)
        {
            if (DefenseAreaGCD(out act)) return true;
            if (DefenseSingleGCD(out act)) return true;
        }

        //发炎
        //留一层走位
        if (Phlegma3.ShouldUse(out act, mustUse: Phlegma3.MaxCharges == Phlegma3.CurrentCharges)) return true;
        if (!Phlegma3.EnoughLevel && Phlegma2.ShouldUse(out act, mustUse: Phlegma2.MaxCharges == Phlegma2.CurrentCharges)) return true;
        if (!Phlegma2.EnoughLevel && Phlegma.ShouldUse(out act, mustUse: Phlegma.MaxCharges == Phlegma.CurrentCharges)) return true;

        //失衡
        if (Dyskrasia.ShouldUse(out act)) return true;

        //注药
        if (Dosis.ShouldUse(out act)) return true;

        //发炎
        if (Phlegma3.ShouldUse(out act, mustUse: true)) return true;
        if (!Phlegma3.EnoughLevel && Phlegma2.ShouldUse(out act, mustUse: true)) return true;
        if (!Phlegma2.EnoughLevel && Phlegma.ShouldUse(out act, mustUse: true)) return true;

        //箭毒
        if (Addersting > 0 && Toxikon.ShouldUse(out act, mustUse: true)) return true;

        //脱战给T刷单盾嫖豆子
        if (!InCombat)
        {
            var tank = TargetUpdater.PartyTanks;
            if (tank.Length == 1 && EukrasianDiagnosis.Target == tank.First() && EukrasianDiagnosis.ShouldUse(out act))
            {
                if (tank.First().HasStatus(true,
                    StatusID.EukrasianDiagnosis,
                    StatusID.EukrasianPrognosis,
                    StatusID.Galvanize
                )) return false;

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
        if (Addersgall == 0 && tank.Length == 1 && tank.Any(t => t.GetHealthRatio() < 0.6f) && !isBoss)
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
            if (EukrasianDiagnosis.Target.HasStatus(true,
                StatusID.EukrasianDiagnosis,
                StatusID.EukrasianPrognosis,
                StatusID.Galvanize
            ))
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
            if (EukrasianPrognosis.Target.HasStatus(true,
                StatusID.EukrasianDiagnosis,
                StatusID.EukrasianPrognosis,
                StatusID.Galvanize
            ))
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
        if (Kerachole.ShouldUse(out act)) return true;

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
