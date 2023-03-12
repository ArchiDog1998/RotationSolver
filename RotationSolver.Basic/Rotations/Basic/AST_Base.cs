using Dalamud.Game.ClientState.JobGauge.Enums;
using Dalamud.Game.ClientState.JobGauge.Types;
using RotationSolver.Actions;
using RotationSolver.Actions.BaseAction;
using RotationSolver.Basic;
using RotationSolver.Data;
using RotationSolver.Helpers;
using RotationSolver.Rotations.CustomRotation;

namespace RotationSolver.Rotations.Basic;

public abstract class AST_Base : CustomRotation.CustomRotation
{
    private static ASTGauge JobGauge => Service.JobGauges.Get<ASTGauge>();

    public override MedicineType MedicineType => MedicineType.Mind;

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

    private sealed protected override IBaseAction Raise => Ascend;

    /// <summary>
    /// 生辰
    /// </summary>
    public static IBaseAction Ascend { get; } = new BaseAction(ActionID.Ascend, true);

    /// <summary>
    /// 凶星
    /// </summary>
    public static IBaseAction Malefic { get; } = new BaseAction(ActionID.Malefic);

    /// <summary>
    /// 烧灼
    /// </summary>
    public static IBaseAction Combust { get; } = new BaseAction(ActionID.Combust, isEot: true)
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
    public static IBaseAction Gravity { get; } = new BaseAction(ActionID.Gravity);

    /// <summary>
    /// 吉星
    /// </summary>
    public static IBaseAction Benefic { get; } = new BaseAction(ActionID.Benefic, true, isTimeline: true);

    /// <summary>
    /// 福星
    /// </summary>
    public static IBaseAction Benefic2 { get; } = new BaseAction(ActionID.Benefic2, true, isTimeline: true);

    /// <summary>
    /// 吉星相位
    /// </summary>
    public static IBaseAction AspectedBenefic { get; } = new BaseAction(ActionID.AspectedBenefic, true, isEot: true, isTimeline: true)
    {
        TargetStatus = new StatusID[] { StatusID.AspectedBenefic },
    };

    /// <summary>
    /// 先天禀赋
    /// </summary>
    public static IBaseAction EssentialDignity { get; } = new BaseAction(ActionID.EssentialDignity, true, isTimeline: true);

    /// <summary>
    /// 星位合图
    /// </summary>
    public static IBaseAction Synastry { get; } = new BaseAction(ActionID.Synastry, true, isTimeline: true);

    /// <summary>
    /// 天星交错
    /// </summary>
    public static IBaseAction CelestialIntersection { get; } = new BaseAction(ActionID.CelestialIntersection, true, isTimeline: true)
    {
        ChoiceTarget = TargetFilter.FindAttackedTarget,

        TargetStatus = new StatusID[] { StatusID.Intersection },
    };

    /// <summary>
    /// 擢升
    /// </summary>
    public static IBaseAction Exaltation { get; } = new BaseAction(ActionID.Exaltation, true, isTimeline: true)
    {
        ChoiceTarget = TargetFilter.FindAttackedTarget,
    };

    /// <summary>
    /// 阳星
    /// </summary>
    public static IBaseAction Helios { get; } = new BaseAction(ActionID.Helios, true, isTimeline: true);

    /// <summary>
    /// 阳星相位
    /// </summary>
    public static IBaseAction AspectedHelios { get; } = new BaseAction(ActionID.AspectedHelios, true, isEot: true, isTimeline: true)
    {
        ActionCheck = b => !IsLastGCD(ActionID.AspectedHelios),
        StatusProvide = new StatusID[] { StatusID.AspectedHelios },
    };

    /// <summary>
    /// 天星冲日
    /// </summary>
    public static IBaseAction CelestialOpposition { get; } = new BaseAction(ActionID.CelestialOpposition, true, isTimeline: true);

    /// <summary>
    /// 地星
    /// </summary>
    public static IBaseAction EarthlyStar { get; } = new BaseAction(ActionID.EarthlyStar, true, isTimeline: true);

    /// <summary>
    /// 命运之轮 减伤，手动放。
    /// </summary>
    public static IBaseAction CollectiveUnconscious { get; } = new BaseAction(ActionID.CollectiveUnconscious, true, isTimeline: true);

    /// <summary>
    /// 天宫图
    /// </summary>
    public static IBaseAction Horoscope { get; } = new BaseAction(ActionID.Horoscope, true, isTimeline: true);

    /// <summary>
    /// 光速
    /// </summary>
    public static IBaseAction Lightspeed { get; } = new BaseAction(ActionID.Lightspeed);

    /// <summary>
    /// 中间学派
    /// </summary>
    public static IBaseAction NeutralSect { get; } = new BaseAction(ActionID.NeutralSect, isTimeline: true);

    /// <summary>
    /// 大宇宙
    /// </summary>
    public static IBaseAction Macrocosmos { get; } = new BaseAction(ActionID.Macrocosmos, isTimeline: true);

    /// <summary>
    /// 星力
    /// </summary>
    public static IBaseAction Astrodyne { get; } = new BaseAction(ActionID.Astrodyne)
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
    public static IBaseAction Divination { get; } = new BaseAction(ActionID.Divination, true);

    /// <summary>
    /// 抽卡
    /// </summary>
    public static IBaseAction Draw { get; } = new BaseAction(ActionID.Draw)
    {
        ActionCheck = b => DrawnCard == CardType.NONE,
    };

    /// <summary>
    /// 重抽
    /// </summary>
    public static IBaseAction Redraw { get; } = new BaseAction(ActionID.Redraw)
    {
        StatusNeed = new[] { StatusID.ClarifyingDraw },
        ActionCheck = b => DrawnCard != CardType.NONE && Seals.Contains(GetCardSeal(DrawnCard)),
    };

    /// <summary>
    /// 小奥秘卡
    /// </summary>
    public static IBaseAction MinorArcana { get; } = new BaseAction(ActionID.MinorArcana)
    {
        ActionCheck = b => InCombat && DrawnCrownCard == CardType.NONE,
    };

    ///// <summary>
    ///// 出王冠卡
    ///// </summary>
    //public static IBaseAction CrownPlay { get; } = new BaseAction(ActionID.CrownPlay)
    //{
    //    ActionCheck = b => DrawnCrownCard is CardType.LADY or CardType.LORD,
    //};

    /// <summary>
    /// 太阳神之衡
    /// </summary>
    private static IBaseAction Balance { get; } = new BaseAction(ActionID.Balance)
    {
        ChoiceTarget = TargetFilter.ASTMeleeTarget,
        ActionCheck = b => DrawnCard == CardType.BALANCE,
    };

    /// <summary>
    /// 放浪神之箭
    /// </summary>
    private static IBaseAction Arrow { get; } = new BaseAction(ActionID.Arrow)
    {
        ChoiceTarget = TargetFilter.ASTMeleeTarget,
        ActionCheck = b => DrawnCard == CardType.ARROW,
    };

    /// <summary>
    /// 战争神之枪
    /// </summary>
    private static IBaseAction Spear { get; } = new BaseAction(ActionID.Spear)
    {
        ChoiceTarget = TargetFilter.ASTMeleeTarget,
        ActionCheck = b => DrawnCard == CardType.SPEAR,
    };

    /// <summary>
    /// 世界树之干
    /// </summary>
    private static IBaseAction Bole { get; } = new BaseAction(ActionID.Bole)
    {
        ChoiceTarget = TargetFilter.ASTRangeTarget,
        ActionCheck = b => DrawnCard == CardType.BOLE,
    };

    /// <summary>
    /// 河流神之瓶
    /// </summary>
    private static IBaseAction Ewer { get; } = new BaseAction(ActionID.Ewer)
    {
        ChoiceTarget = TargetFilter.ASTRangeTarget,
        ActionCheck = b => DrawnCard == CardType.EWER,

    };

    /// <summary>
    /// 建筑神之塔
    /// </summary>
    private static IBaseAction Spire { get; } = new BaseAction(ActionID.Spire)
    {
        ChoiceTarget = TargetFilter.ASTRangeTarget,
        ActionCheck = b => DrawnCard == CardType.SPIRE,
    };

    protected static bool PlayCard(out IAction act)
    {
        act = null;
        if (!Seals.Contains(SealType.NONE)) return false;

        if (Balance.CanUse(out act)) return true;
        if (Arrow.CanUse(out act)) return true;
        if (Spear.CanUse(out act)) return true;
        if (Bole.CanUse(out act)) return true;
        if (Ewer.CanUse(out act)) return true;
        if (Spire.CanUse(out act)) return true;

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
