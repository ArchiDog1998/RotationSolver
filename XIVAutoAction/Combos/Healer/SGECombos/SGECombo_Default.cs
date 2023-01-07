using System.Collections.Generic;
using System.Linq;
using AutoAction.Actions;
using AutoAction.Actions.BaseAction;
using AutoAction.Combos.Basic;
using AutoAction.Combos.CustomCombo;
using AutoAction.Configuration;
using AutoAction.Data;
using AutoAction.Helpers;
using AutoAction.Updaters;
using static AutoAction.Combos.Healer.SGECombos.SGECombo_Default;

namespace AutoAction.Combos.Healer.SGECombos;

internal sealed class SGECombo_Default : SGECombo_Base<CommandType>
{
    public override string GameVersion => "6.18";

    public override string Author => "ϫ��Moon";

    internal enum CommandType : byte
    {
        None,
    }

    protected override SortedList<CommandType, string> CommandDescription => new SortedList<CommandType, string>()
    {
        //{CommandType.None, "" }, //д��ע�Ͱ���������ʾ�û��ġ�
    };

    /// <summary>
    /// ���þ������
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

    protected override bool CanHealSingleSpell => base.CanHealSingleSpell && (Config.GetBoolByName("GCDHeal") || TargetUpdater.PartyHealers.Count() < 2);
    protected override bool CanHealAreaSpell => base.CanHealAreaSpell && (Config.GetBoolByName("GCDHeal") || TargetUpdater.PartyHealers.Count() < 2);

    private protected override ActionConfiguration CreateConfiguration()
    {
        return base.CreateConfiguration().SetBool("GCDHeal", false, "�Զ���GCD��");
    }

    public override SortedList<DescType, string> DescriptionDict => new()
    {
        {DescType.HealArea, $"GCD: {Prognosis}\n                     ����: {Holos}, {Ixochole}, {Physis}"},
        {DescType.HealSingle, $"GCD: {Diagnosis}\n                     ����: {Druochole}"},
        {DescType.DefenseArea, $"{Panhaima}, {Kerachole}, {Prognosis}"},
        {DescType.DefenseSingle, $"GCD: {Diagnosis}\n                     ����: {Haima}, {Taurochole}"},
        {DescType.MoveAction, $"{Icarus}��Ŀ��Ϊ����н�С��30������ԶĿ�ꡣ"},
    };
    private protected override bool AttackAbility(byte abilityRemain, out IAction act)
    {
        act = null!;
        return false;
    }

    private protected override bool EmergencyAbility(byte abilityRemain, IAction nextGCD, out IAction act)
    {
        if (base.EmergencyAbility(abilityRemain, nextGCD, out act)) return true;

        //�¸�������
        if (nextGCD.IsAnySameAction(false, Pneuma, EukrasianDiagnosis,
            EukrasianPrognosis, Diagnosis, Prognosis))
        {
            //�
            if (Zoe.ShouldUse(out act)) return true;
        }

        if (nextGCD == Diagnosis)
        {
            //���
            if (Krasis.ShouldUse(out act)) return true;
        }

        act = null;
        return false;
    }

    private protected override bool DefenceSingleAbility(byte abilityRemain, out IAction act)
    {

        if (Addersgall == 0 || Dyskrasia.ShouldUse(out _))
        {
            //��Ѫ
            if (Haima.ShouldUse(out act)) return true;
        }

        //��ţ��֭
        if (Taurochole.ShouldUse(out act) && Taurochole.Target.GetHealthRatio() < 0.8) return true;

        act = null!;
        return false;
    }

    private protected override bool DefenseSingleGCD(out IAction act)
    {
        //���
        if (EukrasianDiagnosis.ShouldUse(out act))
        {
            if (EukrasianDiagnosis.Target.HasStatus(true,
                StatusID.EukrasianDiagnosis,
                StatusID.EukrasianPrognosis,
                StatusID.Galvanize
            )) return false;

            //����
            if (Eukrasia.ShouldUse(out act)) return true;

            act = EukrasianDiagnosis;
            return true;
        }

        act = null!;
        return false;
    }

    private protected override bool DefenceAreaAbility(byte abilityRemain, out IAction act)
    {
        //����Ѫ
        if (Addersgall == 0 && TargetUpdater.PartyMembersAverHP < 0.7)
        {
            if (Panhaima.ShouldUse(out act)) return true;
        }

        //�����֭
        if (Kerachole.ShouldUse(out act)) return true;

        //������
        if (Holos.ShouldUse(out act)) return true;

        act = null!;
        return false;
    }

    private protected override bool DefenseAreaGCD(out IAction act)
    {
        //Ԥ��
        if (EukrasianPrognosis.ShouldUse(out act))
        {
            if (EukrasianDiagnosis.Target.HasStatus(true,
                StatusID.EukrasianDiagnosis,
                StatusID.EukrasianPrognosis,
                StatusID.Galvanize
            )) return false;

            //����
            if (Eukrasia.ShouldUse(out act)) return true;

            act = EukrasianPrognosis;
            return true;
        }

        act = null!;
        return false;
    }

    private protected override bool MoveAbility(byte abilityRemain, out IAction act)
    {
        //����
        if (Icarus.ShouldUse(out act, emptyOrSkipCombo: true)) return true;
        return false;
    }

    private protected override bool GeneralAbility(byte abilityRemain, out IAction act)
    {
        //�Ĺ�
        if (Kardia.ShouldUse(out act)) return true;

        //����
        if (Addersgall == 0 && Rhizomata.ShouldUse(out act)) return true;

        //����
        if (Soteria.ShouldUse(out act) && TargetUpdater.PartyMembers.Any(b => b.HasStatus(true, StatusID.Kardion) && b.GetHealthRatio() < Service.Configuration.HealthSingleAbility)) return true;

        //����
        if (Pepsis.ShouldUse(out act)) return true;

        act = null!;
        return false;
    }

    private protected override bool GeneralGCD(out IAction act)
    {
        //if (HasEukrasia && InCombat && !EukrasianDosis.ShouldUse(out _))
        //{
        //    if (DefenseAreaGCD(out act)) return true;
        //    if (DefenseSingleGCD(out act)) return true;
        //}

        //���� ��һ����λ
        if (Phlegma3.ShouldUse(out act, mustUse: true, emptyOrSkipCombo: IsMoving || Dyskrasia.ShouldUse(out _))) return true;
        if (!Phlegma3.EnoughLevel && Phlegma2.ShouldUse(out act, mustUse: true, emptyOrSkipCombo: IsMoving || Dyskrasia.ShouldUse(out _))) return true;
        if (!Phlegma2.EnoughLevel && Phlegma.ShouldUse(out act, mustUse: true, emptyOrSkipCombo: IsMoving || Dyskrasia.ShouldUse(out _))) return true;

        //ʧ��
        if (Dyskrasia.ShouldUse(out act)) return true;

        if (EukrasianDosis.ShouldUse(out var enAct))
        {
            //����Dot
            if (Eukrasia.ShouldUse(out act)) return true;
            act = enAct;
            return true;
        }

        //עҩ
        if (Dosis.ShouldUse(out act)) return true;

        //����
        if (Toxikon.ShouldUse(out act, mustUse: true)) return true;

        //��ս��Tˢ�����ζ���
        if (MEukrasianDiagnosis.ShouldUse(out _))
        {
            //����
            if (Eukrasia.ShouldUse(out act)) return true;

            act = MEukrasianDiagnosis;
            return true;
        }
        if (Eukrasia.ShouldUse(out act)) return true;


        return false;
    }

    private protected override bool HealSingleAbility(byte abilityRemain, out IAction act)
    {
        //��ţ��֭
        if (Taurochole.ShouldUse(out act)) return true;

        //������֭
        if (Druochole.ShouldUse(out act)) return true;

        //����Դ����ʱ���뷶Χ���ƻ���ѹ��
        var tank = TargetUpdater.PartyTanks;
        var isBoss = Dosis.Target.IsBoss();
        if (Addersgall == 0 && tank.Count() == 1 && tank.Any(t => t.GetHealthRatio() < 0.6f) && !isBoss)
        {
            //������
            if (Holos.ShouldUse(out act)) return true;

            //����
            if (Physis.ShouldUse(out act)) return true;

            //����Ѫ
            if (Panhaima.ShouldUse(out act)) return true;
        }

        act = null!;
        return false;
    }

    private protected override bool HealSingleGCD(out IAction act)
    {
        if (Diagnosis.ShouldUse(out act)) return true;
        act = null;
        return false;
    }

    private protected override bool HealAreaGCD(out IAction act)
    {
        if (TargetUpdater.PartyMembersAverHP < 0.65f || (Dyskrasia.ShouldUse(out _) && TargetUpdater.PartyTanks.Any(t => t.GetHealthRatio() < 0.6f)))
        {
            //�����Ϣ
            if (Pneuma.ShouldUse(out act, mustUse: true)) return true;
        }

        //Ԥ��
        if (EukrasianPrognosis.Target.HasStatus(false, StatusID.EukrasianDiagnosis, StatusID.EukrasianPrognosis, StatusID.Galvanize))
        {
            if (Prognosis.ShouldUse(out act)) return true;
        }

        if (EukrasianPrognosis.ShouldUse(out _))
        {
            //����
            if (Eukrasia.ShouldUse(out act)) return true;

            act = EukrasianPrognosis;
            return true;
        }

        act = null;
        return false;
    }
    private protected override bool HealAreaAbility(byte abilityRemain, out IAction act)
    {
        //�����֭
        if (Kerachole.ShouldUse(out act) && Level >= 78) return true;

        //����
        if (Physis.ShouldUse(out act)) return true;

        //������
        if (Holos.ShouldUse(out act) && TargetUpdater.PartyMembersAverHP < 0.65f) return true;

        //������֭
        if (Ixochole.ShouldUse(out act)) return true;

        return false;
    }
}
