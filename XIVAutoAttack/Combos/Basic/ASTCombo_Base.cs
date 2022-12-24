using Dalamud.Game.ClientState.JobGauge.Enums;
using Dalamud.Game.ClientState.JobGauge.Types;
using System;
using System.Linq;
using XIVAutoAttack.Actions;
using XIVAutoAttack.Actions.BaseAction;
using XIVAutoAttack.Combos.CustomCombo;
using XIVAutoAttack.Data;
using XIVAutoAttack.Helpers;

namespace XIVAutoAttack.Combos.Basic;

internal abstract class ASTCombo_Base<TCmd> : CustomCombo<TCmd> where TCmd : Enum
{
    private static ASTGauge JobGauge => Service.JobGauges.Get<ASTGauge>();

    /// <summary>
    /// 抽出来的卡是啥。
    /// </summary>
    private static CardType DrawnCard => JobGauge.DrawnCard;

    /// <summary>
    /// 抽出来的王卡是啥
    /// </summary>
    protected static CardType DrawnCrownCard => JobGauge.DrawnCrownCard;

    /// <summary>
    /// 已经有的星象
    /// </summary>
    protected static SealType[] Seals => JobGauge.Seals;

    public sealed override ClassJobID[] JobIDs => new ClassJobID[] { ClassJobID.Astrologian };

    private sealed protected override BaseAction Raise => Ascend;

    /// <summary>
    /// 生辰
    /// </summary>
    public static BaseAction Ascend { get; } = new(ActionID.Ascend, true);

    /// <summary>
    /// 凶星
    /// </summary>
    public static BaseAction Malefic { get; } = new(ActionID.Malefic);

    /// <summary>
    /// 烧灼
    /// </summary>
    public static BaseAction Combust { get; } = new(ActionID.Combust, isEot: true)
    {
        TargetStatus = new StatusID[]
        {
            StatusID.Combust,
            StatusID.Combust2,
            StatusID.Combust3,
            StatusID.Combust4,
        }
    };

    /// <summary>
    /// 重力    
    /// </summary>
    public static BaseAction Gravity { get; } = new(ActionID.Gravity);

    /// <summary>
    /// 吉星
    /// </summary>
    public static BaseAction Benefic { get; } = new(ActionID.Benefic, true, isTimeline: true);

    /// <summary>
    /// 福星
    /// </summary>
    public static BaseAction Benefic2 { get; } = new(ActionID.Benefic2, true, isTimeline: true);

    /// <summary>
    /// 吉星相位
    /// </summary>
    public static BaseAction AspectedBenefic { get; } = new(ActionID.AspectedBenefic, true, isEot: true, isTimeline: true)
    {
        TargetStatus = new StatusID[] { StatusID.AspectedBenefic },
    };

    /// <summary>
    /// 先天禀赋
    /// </summary>
    public static BaseAction EssentialDignity { get; } = new(ActionID.EssentialDignity, true, isTimeline: true);

    /// <summary>
    /// 星位合图
    /// </summary>
    public static BaseAction Synastry { get; } = new(ActionID.Synastry, true, isTimeline: true);

    /// <summary>
    /// 天星交错
    /// </summary>
    public static BaseAction CelestialIntersection { get; } = new(ActionID.CelestialIntersection, true, isTimeline: true)
    {
        ChoiceTarget = TargetFilter.FindAttackedTarget,

        TargetStatus = new StatusID[] { StatusID.Intersection },
    };

    /// <summary>
    /// 擢升
    /// </summary>
    public static BaseAction Exaltation { get; } = new(ActionID.Exaltation, true, isTimeline: true)
    {
        ChoiceTarget = TargetFilter.FindAttackedTarget,
    };

    /// <summary>
    /// 阳星
    /// </summary>
    public static BaseAction Helios { get; } = new(ActionID.Helios, true, isTimeline: true);

    /// <summary>
    /// 阳星相位
    /// </summary>
    public static BaseAction AspectedHelios { get; } = new(ActionID.AspectedHelios, true, isEot: true, isTimeline: true)
    {
        BuffsProvide = new StatusID[] { StatusID.AspectedHelios },
    };

    /// <summary>
    /// 天星冲日
    /// </summary>
    public static BaseAction CelestialOpposition { get; } = new(ActionID.CelestialOpposition, true, isTimeline: true);

    /// <summary>
    /// 地星
    /// </summary>
    public static BaseAction EarthlyStar { get; } = new(ActionID.EarthlyStar, true, isTimeline: true);

    /// <summary>
    /// 命运之轮 减伤，手动放。
    /// </summary>
    public static BaseAction CollectiveUnconscious { get; } = new(ActionID.CollectiveUnconscious, true, isTimeline: true);

    /// <summary>
    /// 天宫图
    /// </summary>
    public static BaseAction Horoscope { get; } = new(ActionID.Horoscope, true, isTimeline: true);

    /// <summary>
    /// 光速
    /// </summary>
    public static BaseAction Lightspeed { get; } = new(ActionID.Lightspeed);

    /// <summary>
    /// 中间学派
    /// </summary>
    public static BaseAction NeutralSect { get; } = new(ActionID.NeutralSect, isTimeline: true);

    /// <summary>
    /// 大宇宙
    /// </summary>
    public static BaseAction Macrocosmos { get; } = new(ActionID.Macrocosmos, isTimeline: true);

    /// <summary>
    /// 星力
    /// </summary>
    public static BaseAction Astrodyne { get; } = new(ActionID.Astrodyne)
    {
        ActionCheck = b =>
        {
            if (JobGauge.Seals.Length != 3) return false;
            if (JobGauge.Seals.Contains(SealType.NONE)) return false;
            return true;
        },
    };

    /// <summary>
    /// 占卜
    /// </summary>
    public static BaseAction Divination { get; } = new(ActionID.Divination, true);

    /// <summary>
    /// 抽卡
    /// </summary>
    public static BaseAction Draw { get; } = new(ActionID.Draw)
    {
        ActionCheck = b => DrawnCard == CardType.NONE,
    };

    /// <summary>
    /// 重抽
    /// </summary>
    public static BaseAction Redraw { get; } = new(ActionID.Redraw)
    {
        BuffsNeed = new[] { StatusID.ClarifyingDraw },
        ActionCheck = b => DrawnCard != CardType.NONE && Seals.Contains(GetCardSeal(DrawnCard)),
    };

    /// <summary>
    /// 小奥秘卡
    /// </summary>
    public static BaseAction MinorArcana { get; } = new(ActionID.MinorArcana)
    {
        ActionCheck = b => InCombat && DrawnCrownCard == CardType.NONE,
    };

    /// <summary>
    /// 出王冠卡
    /// </summary>
    public static BaseAction CrownPlay { get; } = new(ActionID.CrownPlay)
    {
        ActionCheck = b => DrawnCrownCard is CardType.LADY or CardType.LORD,
    };

    /// <summary>
    /// 太阳神之衡
    /// </summary>
    private static BaseAction Balance { get; } = new(ActionID.Balance)
    {
        ChoiceTarget = TargetFilter.ASTMeleeTarget,
        ActionCheck = b => DrawnCard == CardType.BALANCE,
    };

    /// <summary>
    /// 放浪神之箭
    /// </summary>
    private static BaseAction Arrow { get; } = new(ActionID.Arrow)
    {
        ChoiceTarget = TargetFilter.ASTMeleeTarget,
        ActionCheck = b => DrawnCard == CardType.ARROW,
    };

    /// <summary>
    /// 战争神之枪
    /// </summary>
    private static BaseAction Spear { get; } = new(ActionID.Spear)
    {
        ChoiceTarget = TargetFilter.ASTMeleeTarget,
        ActionCheck = b => DrawnCard == CardType.SPEAR,
    };

    /// <summary>
    /// 世界树之干
    /// </summary>
    private static BaseAction Bole { get; } = new(ActionID.Bole)
    {
        ChoiceTarget = TargetFilter.ASTRangeTarget,
        ActionCheck = b => DrawnCard == CardType.BOLE,
    };

    /// <summary>
    /// 河流神之瓶
    /// </summary>
    private static BaseAction Ewer { get; } = new(ActionID.Ewer)
    {
        ChoiceTarget = TargetFilter.ASTRangeTarget,
        ActionCheck = b => DrawnCard == CardType.EWER,

    };

    /// <summary>
    /// 建筑神之塔
    /// </summary>
    private static BaseAction Spire { get; } = new(ActionID.Spire)
    {
        ChoiceTarget = TargetFilter.ASTRangeTarget,
        ActionCheck = b => DrawnCard == CardType.SPIRE,
    };

    protected static bool PlayCard(out IAction act)
    {
        act = null;
        if (!Seals.Contains(SealType.NONE)) return false;

        if (Balance.ShouldUse(out act)) return true;
        if (Arrow.ShouldUse(out act)) return true;
        if (Spear.ShouldUse(out act)) return true;
        if (Bole.ShouldUse(out act)) return true;
        if (Ewer.ShouldUse(out act)) return true;
        if (Spire.ShouldUse(out act)) return true;

        return false;
    }

    private static SealType GetCardSeal(CardType card)
    {
        switch (card)
        {
            default: return SealType.NONE;

            case CardType.BALANCE:
            case CardType.BOLE:
                return SealType.SUN;

            case CardType.ARROW:
            case CardType.EWER:
                return SealType.MOON;

            case CardType.SPEAR:
            case CardType.SPIRE:
                return SealType.CELESTIAL;
        }
    }
}
