using Dalamud.Game.ClientState.JobGauge.Types;
using System;
using System.Linq;
using AutoAction.Actions;
using AutoAction.Actions.BaseAction;
using AutoAction.Combos.CustomCombo;
using AutoAction.Data;
using AutoAction.Helpers;

namespace AutoAction.Combos.Basic;
internal abstract class DNCCombo_Base<TCmd> : CustomCombo<TCmd> where TCmd : Enum
{
    private static DNCGauge JobGauge => Service.JobGauges.Get<DNCGauge>();

    /// <summary>
    /// ��������
    /// </summary>
    protected static bool IsDancing => JobGauge.IsDancing;

    /// <summary>
    /// ����
    /// </summary>
    protected static byte Esprit => JobGauge.Esprit;

    /// <summary>
    /// ������
    /// </summary>
    protected static byte Feathers => JobGauge.Feathers;

    protected static byte CompletedSteps => JobGauge.CompletedSteps;

    public sealed override ClassJobID[] JobIDs => new ClassJobID[] { ClassJobID.Dancer };

    /// <summary>
    /// ��к
    /// </summary>
    public static BaseAction Cascade { get; } = new(ActionID.Cascade)
    {
        BuffsProvide = new[] { StatusID.SilkenSymmetry }
    };

    /// <summary>
    /// ��Ȫ
    /// </summary>
    public static BaseAction Fountain { get; } = new(ActionID.Fountain)
    {
        BuffsProvide = new[] { StatusID.SilkenFlow }
    };

    /// <summary>
    /// ����к
    /// </summary>
    public static BaseAction ReverseCascade { get; } = new(ActionID.ReverseCascade)
    {
        BuffsNeed = new[] { StatusID.SilkenSymmetry, StatusID.SilkenSymmetry2 },
    };

    /// <summary>
    /// ׹��Ȫ
    /// </summary>
    public static BaseAction Fountainfall { get; } = new(ActionID.Fountainfall)
    {
        BuffsNeed = new[] { StatusID.SilkenFlow, StatusID.SilkenFlow2 }
    };

    /// <summary>
    /// ���衤��
    /// </summary>
    public static BaseAction FanDance { get; } = new(ActionID.FanDance)
    {
        ActionCheck = b => JobGauge.Feathers > 0,
        BuffsProvide = new[] { StatusID.ThreefoldFanDance },
    };

    /// <summary>
    /// �糵
    /// </summary>
    public static BaseAction Windmill { get; } = new(ActionID.Windmill)
    {
        BuffsProvide = Cascade.BuffsProvide,
    };

    /// <summary>
    /// ������
    /// </summary>
    public static BaseAction Bladeshower { get; } = new(ActionID.Bladeshower)
    {
        BuffsProvide = Fountain.BuffsProvide,
    };

    /// <summary>
    /// ���糵
    /// </summary>
    public static BaseAction RisingWindmill { get; } = new(ActionID.RisingWindmill)
    {
        BuffsNeed = ReverseCascade.BuffsNeed,
    };

    /// <summary>
    /// ��Ѫ��
    /// </summary>
    public static BaseAction Bloodshower { get; } = new(ActionID.Bloodshower)
    {
        AOECount = 2,
        BuffsNeed = Fountainfall.BuffsNeed,
    };

    /// <summary>
    /// ���衤��
    /// </summary>
    public static BaseAction FanDance2 { get; } = new(ActionID.FanDance2)
    {
        ActionCheck = b => Feathers > 0,
        AOECount = 2,
        BuffsProvide = new[] { StatusID.ThreefoldFanDance },
    };

    /// <summary>
    /// ���衤��
    /// </summary>
    public static BaseAction FanDance3 { get; } = new(ActionID.FanDance3)
    {
        BuffsNeed = FanDance2.BuffsProvide,
    };

    /// <summary>
    /// ���衤��
    /// </summary>
    public static BaseAction FanDance4 { get; } = new(ActionID.FanDance4)
    {
        BuffsNeed = new[] { StatusID.FourfoldFanDance },
    };

    /// <summary>
    /// ����
    /// </summary>
    public static BaseAction SaberDance { get; } = new(ActionID.SaberDance)
    {
        ActionCheck = b => Esprit >= 50,
    };

    /// <summary>
    /// ������
    /// </summary>
    public static BaseAction StarfallDance { get; } = new(ActionID.StarfallDance)
    {
        BuffsNeed = new[] { StatusID.FlourishingStarfall },
    };

    /// <summary>
    /// ǰ�岽
    /// </summary>
    public static BaseAction EnAvant { get; } = new(ActionID.EnAvant, true, shouldEndSpecial: true);

    /// <summary>
    /// Ǿޱ���Ų�
    /// </summary>
    private static BaseAction Emboite { get; } = new(ActionID.Emboite, true)
    {
        ActionCheck = b => (ActionID)JobGauge.NextStep == ActionID.Emboite,
    };

    /// <summary>
    /// С�񽻵���
    /// </summary>
    private static BaseAction Entrechat { get; } = new(ActionID.Entrechat, true)
    {
        ActionCheck = b => (ActionID)JobGauge.NextStep == ActionID.Entrechat,
    };

    /// <summary>
    /// ��ҶС����
    /// </summary>
    private static BaseAction Jete { get; } = new(ActionID.Jete, true)
    {
        ActionCheck = b => (ActionID)JobGauge.NextStep == ActionID.Jete,
    };

    /// <summary>
    /// ���ֺ��ת
    /// </summary>
    private static BaseAction Pirouette { get; } = new(ActionID.Pirouette, true)
    {
        ActionCheck = b => (ActionID)JobGauge.NextStep == ActionID.Pirouette,
    };

    /// <summary>
    /// ��׼�貽
    /// </summary>
    public static BaseAction StandardStep { get; } = new(ActionID.StandardStep)
    {
        BuffsProvide = new[]
        {
            StatusID.StandardStep,
            StatusID.TechnicalStep,
        },
    };

    /// <summary>
    /// �����貽
    /// </summary>
    public static BaseAction TechnicalStep { get; } = new(ActionID.TechnicalStep)
    {
        BuffsNeed = new[]
        {
            StatusID.StandardFinish,
        },
        BuffsProvide = StandardStep.BuffsProvide,
    };

    /// <summary>
    /// ��׼�貽����
    /// </summary>
    private static BaseAction StandardFinish { get; } = new(ActionID.StandardFinish)
    {
        BuffsNeed = new[] { StatusID.StandardStep },
        ActionCheck = b => IsDancing && JobGauge.CompletedSteps == 2,
    };

    /// <summary>
    /// �����貽����
    /// </summary>
    private static BaseAction TechnicalFinish { get; } = new(ActionID.TechnicalFinish)
    {
        BuffsNeed = new[] { StatusID.TechnicalStep },
        ActionCheck = b => IsDancing && JobGauge.CompletedSteps == 4,
    };

    /// <summary>
    /// ����֮ɣ��
    /// </summary>
    public static BaseAction ShieldSamba { get; } = new(ActionID.ShieldSamba, true)
    {
        ActionCheck = b => !Player.HasStatus(false, StatusID.Troubadour,
            StatusID.Tactician1,
            StatusID.Tactician2,
            StatusID.ShieldSamba),
    };

    /// <summary>
    /// ����֮������
    /// </summary>
    public static BaseAction CuringWaltz { get; } = new(ActionID.CuringWaltz, true);

    /// <summary>
    /// ��ʽ����
    /// </summary>
    public static BaseAction ClosedPosition { get; } = new(ActionID.ClosedPosition, true)
    {
        ChoiceTarget = (Targets, mustUse) =>
        {
            Targets = Targets.Where(b => b.ObjectId != Player.ObjectId && b.CurrentHp != 0 &&
            //Remove Weak
            !b.HasStatus(false, StatusID.Weakness, StatusID.BrinkofDeath)
            //Remove other partner.
            && (!b.HasStatus(false, StatusID.ClosedPosition2) | b.HasStatus(true, StatusID.ClosedPosition2))
            ).ToArray();

            return Targets.GetJobCategory(JobRole.Melee, JobRole.RangedMagicial, JobRole.RangedPhysical).FirstOrDefault();
        },
    };

    /// <summary>
    /// ����֮̽��
    /// </summary>
    public static BaseAction Devilment { get; } = new(ActionID.Devilment, true);

    /// <summary>
    /// �ٻ�����
    /// </summary>
    public static BaseAction Flourish { get; } = new(ActionID.Flourish, true)
    {
        BuffsNeed = new[] { StatusID.StandardFinish },
        BuffsProvide = new[]
        {
            StatusID.ThreefoldFanDance,
            StatusID.FourfoldFanDance,
        },
        ActionCheck = b => InCombat,
    };

    /// <summary>
    /// ���˱���
    /// </summary>
    public static BaseAction Improvisation { get; } = new(ActionID.Improvisation, true);

    /// <summary>
    /// ������
    /// </summary>
    public static BaseAction Tillana { get; } = new(ActionID.Tillana)
    {
        BuffsNeed = new[] { StatusID.FlourishingFinish },
    };

    /// <summary>
    /// �����貽
    /// </summary>
    /// <param name="act"></param>
    /// <returns></returns>
    protected static bool FinishStepGCD(out IAction act)
    {
        act = null;
        if (!IsDancing) return false;

        //��׼�貽����
        if (Player.HasStatus(true, StatusID.StandardStep) && Player.WillStatusEnd(1, true, StatusID.StandardStep) || StandardFinish.ShouldUse(out _, mustUse: true))
        {
            act = StandardStep;
            return true;
        }

        //�����貽����
        if (Player.HasStatus(true, StatusID.TechnicalStep) && Player.WillStatusEnd(1, true, StatusID.TechnicalStep) || TechnicalFinish.ShouldUse(out _, mustUse: true))
        {
            act = TechnicalStep;
            return true;
        }

        return false;
    }

    /// <summary>
    /// ִ���貽
    /// </summary>
    /// <param name="act"></param>
    /// <returns></returns>
    protected static bool ExcutionStepGCD(out IAction act)
    {
        act = null;
        if (!Player.HasStatus(true, StatusID.StandardStep, StatusID.TechnicalStep)) return false;
        if (Player.HasStatus(true, StatusID.StandardStep) && CompletedSteps == 2) return false;
        if (Player.HasStatus(true, StatusID.TechnicalStep) && CompletedSteps == 4) return false;

        if (Emboite.ShouldUse(out act)) return true;
        if (Entrechat.ShouldUse(out act)) return true;
        if (Jete.ShouldUse(out act)) return true;
        if (Pirouette.ShouldUse(out act)) return true;
        return false;
    }
}
