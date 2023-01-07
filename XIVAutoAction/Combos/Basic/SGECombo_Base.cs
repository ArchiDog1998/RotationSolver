using Dalamud.Game.ClientState.JobGauge.Types;
using System;
using System.Linq;
using XIVAutoAttack.Actions.BaseAction;
using XIVAutoAttack.Combos.CustomCombo;
using XIVAutoAttack.Data;
using XIVAutoAttack.Helpers;
using XIVAutoAttack.Updaters;

namespace XIVAutoAttack.Combos.Basic;

internal abstract class SGECombo_Base<TCmd> : CustomCombo<TCmd> where TCmd : Enum
{
    private static SGEGauge JobGauge => Service.JobGauges.Get<SGEGauge>();

    /// <summary>
    /// �Ƿ��о��⣿
    /// </summary>
    protected static bool HasEukrasia => JobGauge.Eukrasia;

    /// <summary>
    /// ��������������ɶ�����ˡ�
    /// </summary>
    protected static byte Addersgall => JobGauge.Addersgall;

    /// <summary>
    /// �����ö�������������ɶ�����ˡ�
    /// </summary>
    protected static byte Addersting => JobGauge.Addersting;

    /// <summary>
    /// ���ӵ���ʱ���ж������һ�Ű�
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    protected static bool AddersgallEndAfter(float time)
    {
        return EndAfter(JobGauge.AddersgallTimer / 1000f, time);
    }

    /// <summary>
    /// ���ӵ���ʱ���ж������һ�Ű�
    /// </summary>
    /// <param name="abilityCount"></param>
    /// <param name="gctCount"></param>
    /// <returns></returns>
    protected static bool AddersgallEndAfterGCD(uint gctCount = 0, uint abilityCount = 0)
    {
        return EndAfterGCD(JobGauge.AddersgallTimer / 1000f, gctCount, abilityCount);
    }

    public sealed override ClassJobID[] JobIDs => new ClassJobID[] { ClassJobID.Sage };
    private sealed protected override BaseAction Raise => Egeiro;

    /// <summary>
    /// ����
    /// </summary>
    public static BaseAction Egeiro { get; } = new(ActionID.Egeiro, true);

    /// <summary>
    /// עҩ
    /// </summary>
    public static BaseAction Dosis { get; } = new(ActionID.Dosis);

    /// <summary>
    /// ����עҩ
    /// </summary>
    public static BaseAction EukrasianDosis { get; } = new(ActionID.EukrasianDosis, isEot: true)
    {
        TargetStatus = new StatusID[]
        {
             StatusID.EukrasianDosis,
             StatusID.EukrasianDosis2,
             StatusID.EukrasianDosis3
        },
    };

    /// <summary>
    /// ����
    /// </summary>
    public static BaseAction Phlegma { get; } = new(ActionID.Phlegma);

    /// <summary>
    /// ����2
    /// </summary>
    public static BaseAction Phlegma2 { get; } = new(ActionID.Phlegma2);

    /// <summary>
    /// ����3
    /// </summary>
    public static BaseAction Phlegma3 { get; } = new(ActionID.Phlegma3);

    /// <summary>
    /// ���
    /// </summary>
    public static BaseAction Diagnosis { get; } = new(ActionID.Diagnosis, true);

    /// <summary>
    /// �Ĺ�
    /// </summary>
    public static BaseAction Kardia { get; } = new(ActionID.Kardia, true)
    {
        BuffsProvide = new StatusID[] { StatusID.Kardia },
        ChoiceTarget = (Targets, mustUse) =>
        {
            var targets = Targets.GetJobCategory(JobRole.Tank);
            targets = targets.Any() ? targets : Targets;

            if (!targets.Any()) return null;

            return TargetFilter.FindAttackedTarget(targets, mustUse);
        },
        ActionCheck = b => !b.HasStatus(true, StatusID.Kardion),
    };

    /// <summary>
    /// Ԥ��
    /// </summary>
    public static BaseAction Prognosis { get; } = new(ActionID.Prognosis, true, shouldEndSpecial: true);

    /// <summary>
    /// ����
    /// </summary>
    public static BaseAction Physis { get; } = new(ActionID.Physis, true);

    ///// <summary>
    ///// ����2
    ///// </summary>
    //public static BaseAction Physis2 { get; } = new(ActionID.Physis2, true, isTimeline: true);

    /// <summary>
    /// ����
    /// </summary>
    public static BaseAction Eukrasia { get; } = new(ActionID.Eukrasia, true)
    {
        ActionCheck = b => !JobGauge.Eukrasia,
    };

    /// <summary>
    /// ����
    /// </summary>
    public static BaseAction Soteria { get; } = new(ActionID.Soteria, true);

    /// <summary>
    /// ����
    /// </summary>
    public static BaseAction Icarus { get; } = new(ActionID.Icarus, shouldEndSpecial: true)
    {
        ChoiceTarget = TargetFilter.FindTargetForMoving,
    };

    /// <summary>
    /// ������֭
    /// </summary>
    public static BaseAction Druochole { get; } = new(ActionID.Druochole, true)
    {
        ActionCheck = b => JobGauge.Addersgall > 0,
    };

    /// <summary>
    /// ʧ��
    /// </summary>
    public static BaseAction Dyskrasia { get; } = new(ActionID.Dyskrasia);

    /// <summary>
    /// �����֭
    /// </summary>
    public static BaseAction Kerachole { get; } = new(ActionID.Kerachole, true)
    {
        ActionCheck = b => JobGauge.Addersgall > 0,
    };

    /// <summary>
    /// ������֭
    /// </summary>
    public static BaseAction Ixochole { get; } = new(ActionID.Ixochole, true)
    {
        ActionCheck = b => JobGauge.Addersgall > 0,
    };

    /// <summary>
    /// �
    /// </summary>
    public static BaseAction Zoe { get; } = new(ActionID.Zoe);

    /// <summary>
    /// ��ţ��֭
    /// </summary>
    public static BaseAction Taurochole { get; } = new(ActionID.Taurochole)
    {
        ChoiceTarget = TargetFilter.FindAttackedTarget,
        ActionCheck = b => JobGauge.Addersgall > 0,
    };

    /// <summary>
    /// ����
    /// </summary>
    public static BaseAction Toxikon { get; } = new(ActionID.Toxikon)
    {
        ActionCheck = b => JobGauge.Addersting > 0,
    };

    /// <summary>
    /// ��Ѫ
    /// </summary>
    public static BaseAction Haima { get; } = new(ActionID.Haima, true)
    {
        ChoiceTarget = TargetFilter.FindAttackedTarget,
    };

    /// <summary>
    /// �������
    /// </summary>
    public static BaseAction EukrasianDiagnosis { get; } = new(ActionID.EukrasianDiagnosis, true)
    {
        ChoiceTarget = TargetFilter.FindAttackedTarget,
    };

    /// <summary>
    /// ����Ԥ��
    /// </summary>
    public static BaseAction EukrasianPrognosis { get; } = new(ActionID.EukrasianPrognosis, true);

    /// <summary>
    /// ����
    /// </summary>
    public static BaseAction Rhizomata { get; } = new(ActionID.Rhizomata)
    {
        ActionCheck = b => JobGauge.Addersgall < 3,
    };

    /// <summary>
    /// ������
    /// </summary>
    public static BaseAction Holos { get; } = new(ActionID.Holos, true);

    /// <summary>
    /// ����Ѫ
    /// </summary>
    public static BaseAction Panhaima { get; } = new(ActionID.Panhaima, true);

    /// <summary>
    /// ���
    /// </summary>
    public static BaseAction Krasis { get; } = new(ActionID.Krasis, true);

    /// <summary>
    /// �����Ϣ
    /// </summary>
    public static BaseAction Pneuma { get; } = new(ActionID.Pneuma);

    /// <summary>
    /// ����
    /// </summary>
    public static BaseAction Pepsis { get; } = new(ActionID.Pepsis, true)
    {
        ActionCheck = b =>
        {
            foreach (var chara in TargetUpdater.PartyMembers)
            {
                if (chara.HasStatus(true, StatusID.EukrasianDiagnosis, StatusID.EukrasianPrognosis)
                && b.WillStatusEndGCD(2, 0, true, StatusID.EukrasianDiagnosis, StatusID.EukrasianPrognosis)
                && chara.GetHealthRatio() < 0.9) return true;
            }

            return false;
        },
    };
}
