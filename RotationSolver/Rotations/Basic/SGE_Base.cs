using Dalamud.Game.ClientState.JobGauge.Types;
using System;
using System.Linq;
using RotationSolver.Actions.BaseAction;
using RotationSolver.Updaters;
using RotationSolver.Helpers;
using RotationSolver.Data;
using RotationSolver.Actions;

namespace RotationSolver.Rotations.Basic;

internal abstract class SGE_Base : CustomRotation.CustomRotation
{
    private static SGEGauge JobGauge => Service.JobGauges.Get<SGEGauge>();

    protected static bool HasEukrasia => JobGauge.Eukrasia;
    protected static byte Addersgall => JobGauge.Addersgall;

    protected static byte Addersting => JobGauge.Addersting;

    /// <summary>
    /// 豆子倒计时还有多久能来一颗啊
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    protected static bool AddersgallEndAfter(float time)
    {
        return EndAfter(JobGauge.AddersgallTimer / 1000f, time);
    }

    /// <summary>
    /// 豆子倒计时还有多久能来一颗啊
    /// </summary>
    /// <param name="abilityCount"></param>
    /// <param name="gctCount"></param>
    /// <returns></returns>
    protected static bool AddersgallEndAfterGCD(uint gctCount = 0, uint abilityCount = 0)
    {
        return EndAfterGCD(JobGauge.AddersgallTimer / 1000f, gctCount, abilityCount);
    }

    public sealed override ClassJobID[] JobIDs => new ClassJobID[] { ClassJobID.Sage };
    private sealed protected override IBaseAction Raise => Egeiro;

    /// <summary>
    /// 复苏
    /// </summary>
    public static IBaseAction Egeiro { get; } = new BaseAction(ActionID.Egeiro, true);

    /// <summary>
    /// 注药
    /// </summary>
    public static IBaseAction Dosis { get; } = new BaseAction(ActionID.Dosis);

    /// <summary>
    /// 均衡注药
    /// </summary>
    public static IBaseAction EukrasianDosis { get; } = new BaseAction(ActionID.EukrasianDosis, isEot: true)
    {
        TargetStatus = new StatusID[]
        {
             StatusID.EukrasianDosis,
             StatusID.EukrasianDosis2,
             StatusID.EukrasianDosis3
        },
    };

    /// <summary>
    /// 发炎
    /// </summary>
    public static IBaseAction Phlegma { get; } = new BaseAction(ActionID.Phlegma);

    /// <summary>
    /// 发炎2
    /// </summary>
    public static IBaseAction Phlegma2 { get; } = new BaseAction(ActionID.Phlegma2);

    /// <summary>
    /// 发炎3
    /// </summary>
    public static IBaseAction Phlegma3 { get; } = new BaseAction(ActionID.Phlegma3);

    /// <summary>
    /// 诊断
    /// </summary>
    public static IBaseAction Diagnosis { get; } = new BaseAction(ActionID.Diagnosis, true);

    /// <summary>
    /// 心关
    /// </summary>
    public static IBaseAction Kardia { get; } = new BaseAction(ActionID.Kardia, true)
    {
        StatusProvide = new StatusID[] { StatusID.Kardia },
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
    /// 预后
    /// </summary>
    public static IBaseAction Prognosis { get; } = new BaseAction(ActionID.Prognosis, true, shouldEndSpecial: true, isTimeline: true);

    /// <summary>
    /// 自生
    /// </summary>
    public static IBaseAction Physis { get; } = new BaseAction(ActionID.Physis, true, isTimeline: true);

    /// <summary>
    /// 均衡
    /// </summary>
    public static IBaseAction Eukrasia { get; } = new BaseAction(ActionID.Eukrasia, true, isTimeline: true)
    {
        ActionCheck = b => !JobGauge.Eukrasia,
    };

    /// <summary>
    /// 拯救
    /// </summary>
    public static IBaseAction Soteria { get; } = new BaseAction(ActionID.Soteria, true, isTimeline: true);

    /// <summary>
    /// 神翼
    /// </summary>
    public static IBaseAction Icarus { get; } = new BaseAction(ActionID.Icarus, shouldEndSpecial: true)
    {
        ChoiceTarget = TargetFilter.FindTargetForMoving,
    };

    /// <summary>
    /// 灵橡清汁
    /// </summary>
    public static IBaseAction Druochole { get; } = new BaseAction(ActionID.Druochole, true, isTimeline: true)
    {
        ActionCheck = b => JobGauge.Addersgall > 0,
    };

    /// <summary>
    /// 失衡
    /// </summary>
    public static IBaseAction Dyskrasia { get; } = new BaseAction(ActionID.Dyskrasia);

    /// <summary>
    /// 坚角清汁
    /// </summary>
    public static IBaseAction Kerachole { get; } = new BaseAction(ActionID.Kerachole, true, isTimeline: true)
    {
        ActionCheck = b => JobGauge.Addersgall > 0,
    };

    /// <summary>
    /// 寄生清汁
    /// </summary>
    public static IBaseAction Ixochole { get; } = new BaseAction(ActionID.Ixochole, true, isTimeline: true)
    {
        ActionCheck = b => JobGauge.Addersgall > 0,
    };

    /// <summary>
    /// 活化
    /// </summary>
    public static IBaseAction Zoe { get; } = new BaseAction(ActionID.Zoe, isTimeline: true);

    /// <summary>
    /// 白牛清汁
    /// </summary>
    public static IBaseAction Taurochole { get; } = new BaseAction(ActionID.Taurochole, true, isTimeline: true)
    {
        ChoiceTarget = TargetFilter.FindAttackedTarget,
        ActionCheck = b => JobGauge.Addersgall > 0,
    };

    /// <summary>
    /// 箭毒
    /// </summary>
    public static IBaseAction Toxikon { get; } = new BaseAction(ActionID.Toxikon)
    {
        ActionCheck = b => JobGauge.Addersting > 0,
    };

    /// <summary>
    /// 输血
    /// </summary>
    public static IBaseAction Haima { get; } = new BaseAction(ActionID.Haima, true, isTimeline: true)
    {
        ChoiceTarget = TargetFilter.FindAttackedTarget,
    };

    /// <summary>
    /// 均衡诊断
    /// </summary>
    public static IBaseAction EukrasianDiagnosis { get; } = new BaseAction(ActionID.EukrasianDiagnosis, true, isTimeline: true)
    {
        ChoiceTarget = TargetFilter.FindAttackedTarget,
    };

    /// <summary>
    /// 均衡预后
    /// </summary>
    public static IBaseAction EukrasianPrognosis { get; } = new BaseAction(ActionID.EukrasianPrognosis, true, isTimeline: true);

    /// <summary>
    /// 根素
    /// </summary>
    public static IBaseAction Rhizomata { get; } = new BaseAction(ActionID.Rhizomata, isTimeline: true)
    {
        ActionCheck = b => JobGauge.Addersgall < 3,
    };

    /// <summary>
    /// 整体论
    /// </summary>
    public static IBaseAction Holos { get; } = new BaseAction(ActionID.Holos, true, isTimeline: true);

    /// <summary>
    /// 泛输血
    /// </summary>
    public static IBaseAction Panhaima { get; } = new BaseAction(ActionID.Panhaima, true, isTimeline: true);

    /// <summary>
    /// 混合
    /// </summary>
    public static IBaseAction Krasis { get; } = new BaseAction(ActionID.Krasis, true, isTimeline: true);

    /// <summary>
    /// 魂灵风息
    /// </summary>
    public static IBaseAction Pneuma { get; } = new BaseAction(ActionID.Pneuma, isTimeline: true);

    /// <summary>
    /// 消化
    /// </summary>
    public static IBaseAction Pepsis { get; } = new BaseAction(ActionID.Pepsis, true, isTimeline: true)
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
