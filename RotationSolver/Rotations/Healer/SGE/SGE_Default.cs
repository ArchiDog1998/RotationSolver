using RotationSolver.Actions;
using RotationSolver.Actions.BaseAction;
using RotationSolver.Attributes;
using RotationSolver.Configuration.RotationConfig;
using RotationSolver.Data;
using RotationSolver.Helpers;
using RotationSolver.Rotations.Basic;
using RotationSolver.Rotations.CustomRotation;
using RotationSolver.Updaters;
using System.Collections.Generic;
using System.Linq;

namespace RotationSolver.Rotations.Healer.SGE;

[DefaultRotation]
internal sealed class SGE_Default : SGE_Base
{
    public override string GameVersion => "6.18";

    public override string RotationName => "Default";


    /// <summary>
    /// 自用均衡诊断
    /// </summary>
    private static BaseAction MEukrasianDiagnosis { get; } = new(ActionID.EukrasianDiagnosis, true)
    {
        ChoiceTarget = (Targets, mustUse) =>
        {
            var targets = Targets.GetJobCategory(JobRole.Tank);
            if (!targets.Any()) return null;
            return targets.First();
        },
        ActionCheck = b =>
        {
            if (InCombat) return false;
            if (b == Player) return false;
            if (b.HasStatus(false, StatusID.EukrasianDiagnosis, StatusID.EukrasianPrognosis, StatusID.Galvanize)) return false;
            return true;
        }
    };

    protected override bool CanHealSingleSpell => base.CanHealSingleSpell && (Configs.GetBool("GCDHeal") || TargetUpdater.PartyHealers.Count() < 2);
    protected override bool CanHealAreaSpell => base.CanHealAreaSpell && (Configs.GetBool("GCDHeal") || TargetUpdater.PartyHealers.Count() < 2);

    private protected override IRotationConfigSet CreateConfiguration()
    {
        return base.CreateConfiguration().SetBool("GCDHeal", false, "Auto Use GCD to heal.");
    }

    private protected override bool AttackAbility(byte abilitiesRemaining, out IAction act)
    {
        act = null!;
        return false;
    }

    private protected override bool EmergencyAbility(byte abilitiesRemaining, IAction nextGCD, out IAction act)
    {
        if (base.EmergencyAbility(abilitiesRemaining, nextGCD, out act)) return true;

        //下个技能是
        if (nextGCD.IsTheSameTo(false, Pneuma, EukrasianDiagnosis,
            EukrasianPrognosis, Diagnosis, Prognosis))
        {
            //活化
            if (Zoe.CanUse(out act)) return true;
        }

        if (nextGCD == Diagnosis)
        {
            //混合
            if (Krasis.CanUse(out act)) return true;
        }

        return base.EmergencyAbility(abilitiesRemaining, nextGCD, out act);
    }

    [RotationDesc(ActionID.Haima, ActionID.Taurochole)]
    private protected override bool DefenceSingleAbility(byte abilitiesRemaining, out IAction act)
    {
        if (Addersgall == 0 || Dyskrasia.CanUse(out _))
        {
            if (Haima.CanUse(out act)) return true;
        }

        //白牛清汁
        if (Taurochole.CanUse(out act) && Taurochole.Target.GetHealthRatio() < 0.8) return true;

        return base.DefenceSingleAbility(abilitiesRemaining, out act);
    }

    [RotationDesc(ActionID.EukrasianDiagnosis)]
    private protected override bool DefenseSingleGCD(out IAction act)
    {
        //诊断
        if (EukrasianDiagnosis.CanUse(out act))
        {
            if (EukrasianDiagnosis.Target.HasStatus(false,
                StatusID.EukrasianDiagnosis,
                StatusID.EukrasianPrognosis,
                StatusID.Galvanize
            )) return false;

            //均衡
            if (Eukrasia.CanUse(out act)) return true;

            act = EukrasianDiagnosis;
            return true;
        }

        return base.DefenseSingleGCD(out act);
    }

    [RotationDesc(ActionID.Panhaima, ActionID.Kerachole, ActionID.Holos)]
    private protected override bool DefenceAreaAbility(byte abilityRemain, out IAction act)
    {
        //泛输血
        if (Addersgall == 0 && TargetUpdater.PartyMembersAverHP < 0.7)
        {
            if (Panhaima.CanUse(out act)) return true;
        }

        //坚角清汁
        if (Kerachole.CanUse(out act)) return true;

        //整体论
        if (Holos.CanUse(out act)) return true;

        return base.DefenceAreaAbility(abilityRemain, out act);
    }

    [RotationDesc(ActionID.EukrasianPrognosis)]
    private protected override bool DefenseAreaGCD(out IAction act)
    {
        //预后
        if (EukrasianPrognosis.CanUse(out act))
        {
            if (EukrasianDiagnosis.Target.HasStatus(false,
                StatusID.EukrasianDiagnosis,
                StatusID.EukrasianPrognosis,
                StatusID.Galvanize
            )) return false;

            //均衡
            if (Eukrasia.CanUse(out act)) return true;

            act = EukrasianPrognosis;
            return true;
        }

        return base.DefenseAreaGCD(out act);
    }

    private protected override bool GeneralAbility(byte abilitiesRemaining, out IAction act)
    {
        //心关
        if (Kardia.CanUse(out act)) return true;

        //根素
        if (Addersgall == 0 && Rhizomata.CanUse(out act)) return true;

        //拯救
        if (Soteria.CanUse(out act) && TargetUpdater.PartyMembers.Any(b => b.HasStatus(true, StatusID.Kardion) && b.GetHealthRatio() < Service.Configuration.HealthSingleAbility)) return true;

        //消化
        if (Pepsis.CanUse(out act)) return true;

        act = null!;
        return false;
    }

    private protected override bool GeneralGCD(out IAction act)
    {
        //发炎 留一层走位
        if (Phlegma3.CanUse(out act, mustUse: true, emptyOrSkipCombo: IsMoving || Dyskrasia.CanUse(out _))) return true;
        if (!Phlegma3.EnoughLevel && Phlegma2.CanUse(out act, mustUse: true, emptyOrSkipCombo: IsMoving || Dyskrasia.CanUse(out _))) return true;
        if (!Phlegma2.EnoughLevel && Phlegma.CanUse(out act, mustUse: true, emptyOrSkipCombo: IsMoving || Dyskrasia.CanUse(out _))) return true;

        //失衡
        if (Dyskrasia.CanUse(out act)) return true;

        if (EukrasianDosis.CanUse(out var enAct))
        {
            //补上Dot
            if (Eukrasia.CanUse(out act)) return true;
            act = enAct;
            return true;
        }

        //注药
        if (Dosis.CanUse(out act)) return true;

        //箭毒
        if (Toxikon.CanUse(out act, mustUse: true)) return true;

        //脱战给T刷单盾嫖豆子
        if (MEukrasianDiagnosis.CanUse(out _))
        {
            //均衡
            if (Eukrasia.CanUse(out act)) return true;

            act = MEukrasianDiagnosis;
            return true;
        }
        if (Eukrasia.CanUse(out act)) return true;

        return false;
    }

    [RotationDesc(ActionID.Taurochole, ActionID.Druochole, ActionID.Holos, ActionID.Physis, ActionID.Panhaima)]
    private protected override bool HealSingleAbility(byte abilitiesRemaining, out IAction act)
    {
        //白牛青汁
        if (Taurochole.CanUse(out act)) return true;

        //灵橡清汁
        if (Druochole.CanUse(out act)) return true;

        //当资源不足时加入范围治疗缓解压力
        var tank = TargetUpdater.PartyTanks;
        var isBoss = Dosis.Target.IsBoss();
        if (Addersgall == 0 && tank.Count() == 1 && tank.Any(t => t.GetHealthRatio() < 0.6f) && !isBoss)
        {
            //整体论
            if (Holos.CanUse(out act)) return true;

            //自生
            if (Physis.CanUse(out act)) return true;

            //泛输血
            if (Panhaima.CanUse(out act)) return true;
        }

        return base.HealSingleAbility(abilitiesRemaining, out act);
    }

    [RotationDesc(ActionID.Diagnosis)]
    private protected override bool HealSingleGCD(out IAction act)
    {
        if (Diagnosis.CanUse(out act)) return true;
        return false;
    }

    [RotationDesc(ActionID.Pneuma, ActionID.Prognosis, ActionID.EukrasianPrognosis)]
    private protected override bool HealAreaGCD(out IAction act)
    {
        if (TargetUpdater.PartyMembersAverHP < 0.65f || Dyskrasia.CanUse(out _) && TargetUpdater.PartyTanks.Any(t => t.GetHealthRatio() < 0.6f))
        {
            //魂灵风息
            if (Pneuma.CanUse(out act, mustUse: true)) return true;
        }

        //预后
        if (EukrasianPrognosis.Target.HasStatus(false, StatusID.EukrasianDiagnosis, StatusID.EukrasianPrognosis, StatusID.Galvanize))
        {
            if (Prognosis.CanUse(out act)) return true;
        }

        if (EukrasianPrognosis.CanUse(out _))
        {
            //均衡
            if (Eukrasia.CanUse(out act)) return true;

            act = EukrasianPrognosis;
            return true;
        }

        act = null;
        return false;
    }

    [RotationDesc(ActionID.Kerachole, ActionID.Physis, ActionID.Holos, ActionID.Ixochole)]
    private protected override bool HealAreaAbility(byte abilitiesRemaining, out IAction act)
    {
        //坚角清汁
        if (Kerachole.CanUse(out act) && Level >= 78) return true;

        //自生
        if (Physis.CanUse(out act)) return true;

        //整体论
        if (Holos.CanUse(out act) && TargetUpdater.PartyMembersAverHP < 0.65f) return true;

        //寄生清汁
        if (Ixochole.CanUse(out act)) return true;

        return false;
    }
}
