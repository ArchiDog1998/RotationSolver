namespace RotationSolver.Default.Healer;

[SourceCode("https://github.com/ArchiDog1998/RotationSolver/blob/main/RotationSolver.Default/Healer/SGE_Default.cs")]
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

    protected override bool CanHealSingleSpell => base.CanHealSingleSpell && (Configs.GetBool("GCDHeal") || PartyHealers.Count() < 2);
    protected override bool CanHealAreaSpell => base.CanHealAreaSpell && (Configs.GetBool("GCDHeal") || PartyHealers.Count() < 2);

    protected override IRotationConfigSet CreateConfiguration()
    {
        return base.CreateConfiguration().SetBool("GCDHeal", false, "Auto Use GCD to heal.");
    }

    protected override bool AttackAbility(byte abilitiesRemaining, out IAction act)
    {
        act = null!;
        return false;
    }

    protected override bool EmergencyAbility(byte abilitiesRemaining, IAction nextGCD, out IAction act)
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
    protected override bool DefenseSingleAbility(byte abilitiesRemaining, out IAction act)
    {
        if (Addersgall == 0 || Dyskrasia.CanUse(out _))
        {
            if (Haima.CanUse(out act)) return true;
        }

        //白牛清汁
        if (Taurochole.CanUse(out act) && Taurochole.Target.GetHealthRatio() < 0.8) return true;

        return base.DefenseSingleAbility(abilitiesRemaining, out act);
    }

    [RotationDesc(ActionID.EukrasianDiagnosis)]
    protected override bool DefenseSingleGCD(out IAction act)
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
    protected override bool DefenseAreaAbility(byte abilityRemain, out IAction act)
    {
        //泛输血
        if (Addersgall == 0 && PartyMembersAverHP < 0.7)
        {
            if (Panhaima.CanUse(out act)) return true;
        }

        //坚角清汁
        if (Kerachole.CanUse(out act)) return true;

        //整体论
        if (Holos.CanUse(out act)) return true;

        return base.DefenseAreaAbility(abilityRemain, out act);
    }

    [RotationDesc(ActionID.EukrasianPrognosis)]
    protected override bool DefenseAreaGCD(out IAction act)
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

    protected override bool GeneralAbility(byte abilitiesRemaining, out IAction act)
    {
        //心关
        if (Kardia.CanUse(out act)) return true;

        //根素
        if (Addersgall == 0 && Rhizomata.CanUse(out act)) return true;

        //拯救
        if (Soteria.CanUse(out act) && PartyMembers.Any(b => b.HasStatus(true, StatusID.Kardion) && b.GetHealthRatio() < Service.Config.HealthSingleAbility)) return true;

        //消化
        if (Pepsis.CanUse(out act)) return true;

        act = null!;
        return false;
    }

    protected override bool GeneralGCD(out IAction act)
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
    protected override bool HealSingleAbility(byte abilitiesRemaining, out IAction act)
    {
        //白牛青汁
        if (Taurochole.CanUse(out act)) return true;

        //灵橡清汁
        if (Druochole.CanUse(out act)) return true;

        //当资源不足时加入范围治疗缓解压力
        var tank = PartyTanks;
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
    protected override bool HealSingleGCD(out IAction act)
    {
        if (Diagnosis.CanUse(out act)) return true;
        return false;
    }

    [RotationDesc(ActionID.Pneuma, ActionID.Prognosis, ActionID.EukrasianPrognosis)]
    protected override bool HealAreaGCD(out IAction act)
    {
        if (PartyMembersAverHP < 0.65f || Dyskrasia.CanUse(out _) && PartyTanks.Any(t => t.GetHealthRatio() < 0.6f))
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
    protected override bool HealAreaAbility(byte abilitiesRemaining, out IAction act)
    {
        //坚角清汁
        if (Kerachole.CanUse(out act) && Level >= 78) return true;

        //自生
        if (Physis.CanUse(out act)) return true;

        //整体论
        if (Holos.CanUse(out act) && PartyMembersAverHP < 0.65f) return true;

        //寄生清汁
        if (Ixochole.CanUse(out act)) return true;

        return false;
    }
}
