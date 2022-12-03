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
    /// 是否有均衡？
    /// </summary>
    protected static bool HasEukrasia => JobGauge.Eukrasia;

    /// <summary>
    /// 豆子数量啊，叫啥我忘了。
    /// </summary>
    protected static byte Addersgall => JobGauge.Addersgall;

    /// <summary>
    /// 毒箭用豆子数量啊，叫啥我忘了。
    /// </summary>
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
    private sealed protected override BaseAction Raise => Egeiro;

    /// <summary>
    /// 复苏
    /// </summary>
    public static BaseAction Egeiro { get; } = new(ActionID.Egeiro, true);

    /// <summary>
    /// 注药
    /// </summary>
    public static BaseAction Dosis { get; } = new(ActionID.Dosis);

    /// <summary>
    /// 均衡注药
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
    /// 发炎
    /// </summary>
    public static BaseAction Phlegma { get; } = new(ActionID.Phlegma);

    /// <summary>
    /// 发炎2
    /// </summary>
    public static BaseAction Phlegma2 { get; } = new(ActionID.Phlegma2);

    /// <summary>
    /// 发炎3
    /// </summary>
    public static BaseAction Phlegma3 { get; } = new(ActionID.Phlegma3);

    /// <summary>
    /// 诊断
    /// </summary>
    public static BaseAction Diagnosis { get; } = new(ActionID.Diagnosis, true);

    /// <summary>
    /// 心关
    /// </summary>
    public static BaseAction Kardia { get; } = new(ActionID.Kardia, true)
    {
        BuffsProvide = new StatusID[] { StatusID.Kardia },
        ChoiceTarget = Targets =>
        {
            var targets = Targets.GetJobCategory(JobRole.Tank);
            targets = targets.Any() ? targets : Targets;

            if (!targets.Any()) return null;

            return TargetFilter.FindAttackedTarget(targets);
        },
        ActionCheck = b => !b.HasStatus(true, StatusID.Kardion),
    };

    /// <summary>
    /// 预后
    /// </summary>
    public static BaseAction Prognosis { get; } = new(ActionID.Prognosis, true, shouldEndSpecial: true, isTimeline: true);

    /// <summary>
    /// 自生
    /// </summary>
    public static BaseAction Physis { get; } = new(ActionID.Physis, true, isTimeline: true);

    ///// <summary>
    ///// 自生2
    ///// </summary>
    //public static BaseAction Physis2 { get; } = new(ActionID.Physis2, true, isTimeline: true);

    /// <summary>
    /// 均衡
    /// </summary>
    public static BaseAction Eukrasia { get; } = new(ActionID.Eukrasia, true, isTimeline: true)
    {
        ActionCheck = b => !JobGauge.Eukrasia,
    };

    /// <summary>
    /// 拯救
    /// </summary>
    public static BaseAction Soteria { get; } = new(ActionID.Soteria, true, isTimeline: true);

    /// <summary>
    /// 神翼
    /// </summary>
    public static BaseAction Icarus { get; } = new(ActionID.Icarus, shouldEndSpecial: true)
    {
        ChoiceTarget = TargetFilter.FindTargetForMoving,
    };

    /// <summary>
    /// 灵橡清汁
    /// </summary>
    public static BaseAction Druochole { get; } = new(ActionID.Druochole, true, isTimeline: true)
    {
        ActionCheck = b => JobGauge.Addersgall > 0,
    };

    /// <summary>
    /// 失衡
    /// </summary>
    public static BaseAction Dyskrasia { get; } = new(ActionID.Dyskrasia);

    /// <summary>
    /// 坚角清汁
    /// </summary>
    public static BaseAction Kerachole { get; } = new(ActionID.Kerachole, true, isTimeline: true)
    {
        ActionCheck = b => JobGauge.Addersgall > 0,
    };

    /// <summary>
    /// 寄生清汁
    /// </summary>
    public static BaseAction Ixochole { get; } = new(ActionID.Ixochole, true, isTimeline: true)
    {
        ActionCheck = b => JobGauge.Addersgall > 0,
    };

    /// <summary>
    /// 活化
    /// </summary>
    public static BaseAction Zoe { get; } = new(ActionID.Zoe, isTimeline: true);

    /// <summary>
    /// 白牛清汁
    /// </summary>
    public static BaseAction Taurochole { get; } = new(ActionID.Taurochole, true, isTimeline: true)
    {
        ChoiceTarget = TargetFilter.FindAttackedTarget,
        ActionCheck = b => JobGauge.Addersgall > 0,
    };

    /// <summary>
    /// 箭毒
    /// </summary>
    public static BaseAction Toxikon { get; } = new(ActionID.Toxikon)
    {
        ActionCheck = b => JobGauge.Addersting > 0,
    };

    /// <summary>
    /// 输血
    /// </summary>
    public static BaseAction Haima { get; } = new(ActionID.Haima, true, isTimeline: true)
    {
        ChoiceTarget = TargetFilter.FindAttackedTarget,
    };

    /// <summary>
    /// 均衡诊断
    /// </summary>
    public static BaseAction EukrasianDiagnosis { get; } = new(ActionID.EukrasianDiagnosis, true, isTimeline: true)
    {
        ChoiceTarget = TargetFilter.FindAttackedTarget,
    };

    /// <summary>
    /// 均衡预后
    /// </summary>
    public static BaseAction EukrasianPrognosis { get; } = new(ActionID.EukrasianPrognosis, true, isTimeline: true);

    /// <summary>
    /// 根素
    /// </summary>
    public static BaseAction Rhizomata { get; } = new(ActionID.Rhizomata, isTimeline: true)
    {
        ActionCheck = b => JobGauge.Addersgall < 3,
    };

    /// <summary>
    /// 整体论
    /// </summary>
    public static BaseAction Holos { get; } = new(ActionID.Holos, true, isTimeline: true);

    /// <summary>
    /// 泛输血
    /// </summary>
    public static BaseAction Panhaima { get; } = new(ActionID.Panhaima, true, isTimeline: true);

    /// <summary>
    /// 混合
    /// </summary>
    public static BaseAction Krasis { get; } = new(ActionID.Krasis, true, isTimeline: true);

    /// <summary>
    /// 魂灵风息
    /// </summary>
    public static BaseAction Pneuma { get; } = new(ActionID.Pneuma, isTimeline: true);

    /// <summary>
    /// 消化
    /// </summary>
    public static BaseAction Pepsis { get; } = new(ActionID.Pepsis, true, isTimeline: true)
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
