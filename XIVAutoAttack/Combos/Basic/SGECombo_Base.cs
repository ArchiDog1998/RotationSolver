using Dalamud.Game.ClientState.JobGauge.Types;
using System;
using XIVAutoAttack.Actions.BaseAction;
using XIVAutoAttack.Combos.CustomCombo;
using XIVAutoAttack.Data;
using XIVAutoAttack.Helpers;
using XIVAutoAttack.Updaters;

namespace XIVAutoAttack.Combos.Basic;

internal abstract class SGECombo_Base<TCmd> : JobGaugeCombo<SGEGauge, TCmd> where TCmd : Enum
{
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
        OtherCheck = b => !b.HaveStatus(true, StatusID.Kardion),
    };

    /// <summary>
    /// 预后
    /// </summary>
    public static BaseAction Prognosis { get; } = new(ActionID.Prognosis, true, shouldEndSpecial: true);

    /// <summary>
    /// 自生
    /// </summary>
    public static BaseAction Physis { get; } = new(ActionID.Physis, true);

    /// <summary>
    /// 自生2
    /// </summary>
    public static BaseAction Physis2 { get; } = new(ActionID.Physis2, true);

    /// <summary>
    /// 均衡
    /// </summary>
    public static BaseAction Eukrasia { get; } = new(ActionID.Eukrasia)
    {
        OtherCheck = b => !JobGauge.Eukrasia,
    };

    /// <summary>
    /// 拯救
    /// </summary>
    public static BaseAction Soteria { get; } = new(ActionID.Soteria, true)
    {
        ChoiceTarget = Targets =>
        {
            foreach (var friend in Targets)
            {
                if (friend.HaveStatus(true, StatusID.Kardion))
                {
                    return friend;
                }
            }
            return null;
        },
        OtherCheck = b => b.GetHealthRatio() < 0.7,
    };

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
    public static BaseAction Druochole { get; } = new(ActionID.Druochole, true)
    {
        OtherCheck = b => JobGauge.Addersgall > 0 && HealHelper.SingleHeal(b, 600, 0.9, 0.85),
    };

    /// <summary>
    /// 失衡
    /// </summary>
    public static BaseAction Dyskrasia { get; } = new(ActionID.Dyskrasia);

    /// <summary>
    /// 坚角清汁
    /// </summary>
    public static BaseAction Kerachole { get; } = new(ActionID.Kerachole, true)
    {
        OtherCheck = b => JobGauge.Addersgall > 0,
    };

    /// <summary>
    /// 寄生清汁
    /// </summary>
    public static BaseAction Ixochole { get; } = new(ActionID.Ixochole, true)
    {
        OtherCheck = b => JobGauge.Addersgall > 0,
    };

    /// <summary>
    /// 活化
    /// </summary>
    public static BaseAction Zoe { get; } = new(ActionID.Zoe);

    /// <summary>
    /// 白牛清汁
    /// </summary>
    public static BaseAction Taurochole { get; } = new(ActionID.Taurochole, true)
    {
        ChoiceTarget = TargetFilter.FindAttackedTarget,
        OtherCheck = b => JobGauge.Addersgall > 0,
    };

    /// <summary>
    /// 箭毒
    /// </summary>
    public static BaseAction Toxikon { get; } = new(ActionID.Toxikon);

    /// <summary>
    /// 输血
    /// </summary>
    public static BaseAction Haima { get; } = new(ActionID.Haima, true)
    {
        ChoiceTarget = TargetFilter.FindAttackedTarget,
    };

    /// <summary>
    /// 均衡诊断
    /// </summary>
    public static BaseAction EukrasianDiagnosis { get; } = new(ActionID.EukrasianDiagnosis, true)
    {
        ChoiceTarget = TargetFilter.FindAttackedTarget,
    };

    /// <summary>
    /// 均衡预后
    /// </summary>
    public static BaseAction EukrasianPrognosis { get; } = new(ActionID.EukrasianPrognosis, true)
    {
        ChoiceTarget = TargetFilter.FindAttackedTarget,
    };

    /// <summary>
    /// 根素
    /// </summary>
    public static BaseAction Rhizomata { get; } = new(ActionID.Rhizomata);

    /// <summary>
    /// 整体论
    /// </summary>
    public static BaseAction Holos { get; } = new(ActionID.Holos, true);

    /// <summary>
    /// 泛输血
    /// </summary>
    public static BaseAction Panhaima { get; } = new(ActionID.Panhaima, true);

    /// <summary>
    /// 混合
    /// </summary>
    public static BaseAction Krasis { get; } = new(ActionID.Krasis, true);

    /// <summary>
    /// 魂灵风息
    /// </summary>
    public static BaseAction Pneuma { get; } = new(ActionID.Pneuma);

    /// <summary>
    /// 消化
    /// </summary>
    public static BaseAction Pepsis { get; } = new(ActionID.Pepsis, true)
    {
        OtherCheck = b =>
        {
            foreach (var chara in TargetUpdater.PartyMembers)
            {
                if (chara.HaveStatus(true, StatusID.EukrasianDiagnosis, StatusID.EukrasianPrognosis)
                && b.WillStatusEndGCD(2, 0, true, StatusID.EukrasianDiagnosis, StatusID.EukrasianPrognosis)
                && chara.GetHealthRatio() < 0.9) return true;
            }

            return false;
        },
    };
}
