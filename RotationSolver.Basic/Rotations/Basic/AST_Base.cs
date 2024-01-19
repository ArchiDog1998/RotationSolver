using ECommons.DalamudServices;
using ECommons.ExcelServices;
using RotationSolver.Basic.Traits;

namespace RotationSolver.Basic.Rotations.Basic;

/// <summary>
/// Astrologian
/// </summary>
public abstract class AST_Base : CustomRotation
{
    /// <summary>
    /// 
    /// </summary>
    public override MedicineType MedicineType => MedicineType.Mind;

    /// <summary>
    /// 
    /// </summary>
    public sealed override Job[] Jobs => new[] { Job.AST };

    static ASTGauge JobGauge => Svc.Gauges.Get<ASTGauge>();

    /// <summary>
    /// 
    /// </summary>
    protected static CardType DrawnCard => JobGauge.DrawnCard;

    /// <summary>
    /// 
    /// </summary>
    protected static CardType DrawnCrownCard => JobGauge.DrawnCrownCard;

    /// <summary>
    /// 
    /// </summary>
    protected static SealType[] Seals => JobGauge.Seals;

    #region Attack
    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction Malefic { get; } = new BaseAction(ActionID.Malefic);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction Combust { get; } = new BaseAction(ActionID.Combust, ActionOption.Dot)
    {
        TargetStatus =
        [
            StatusID.Combust,
            StatusID.CombustIi,
            StatusID.CombustIii,
            StatusID.CombustIii_2041,
        ],
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction Gravity { get; } = new BaseAction(ActionID.Gravity);
    #endregion

    #region Heal Single
    private sealed protected override IBaseAction Raise => Ascend;

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction Ascend { get; } = new BaseAction(ActionID.Ascend, ActionOption.Friendly);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction Benefic { get; } = new BaseAction(ActionID.Benefic, ActionOption.Heal);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction Benefic2 { get; } = new BaseAction(ActionID.Benefic2, ActionOption.Heal);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction AspectedBenefic { get; } = new BaseAction(ActionID.AspectedBenefic, ActionOption.Hot)
    {
        TargetStatus = new StatusID[] { StatusID.AspectedBenefic },
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction EssentialDignity { get; } = new BaseAction(ActionID.EssentialDignity, ActionOption.Heal);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction Synastry { get; } = new BaseAction(ActionID.Synastry, ActionOption.Heal);
    #endregion

    #region Heal Area
    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction Helios { get; } = new BaseAction(ActionID.Helios, ActionOption.Heal);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction AspectedHelios { get; } = new BaseAction(ActionID.AspectedHelios, ActionOption.Hot)
    {
        ActionCheck = (b, m) => !IsLastGCD(ActionID.AspectedHelios),
        StatusProvide = new StatusID[] { StatusID.AspectedHelios },
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction CelestialOpposition { get; } = new BaseAction(ActionID.CelestialOpposition, ActionOption.Heal);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction EarthlyStar { get; } = new BaseAction(ActionID.EarthlyStar, ActionOption.Heal);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction Horoscope { get; } = new BaseAction(ActionID.Horoscope, ActionOption.Heal);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction Macrocosmos { get; } = new BaseAction(ActionID.Macrocosmos, ActionOption.Heal)
    {
        StatusProvide = new StatusID[] { StatusID.Macrocosmos }
    };
    #endregion

    #region Defense Single
    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction CelestialIntersection { get; } = new BaseAction(ActionID.CelestialIntersection, ActionOption.Defense)
    {
        ChoiceTarget = TargetFilter.FindAttackedTarget,
        TargetStatus = new StatusID[] { StatusID.Intersection },
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction Exaltation { get; } = new BaseAction(ActionID.Exaltation, ActionOption.Heal)
    {
        ChoiceTarget = TargetFilter.FindAttackedTarget,
        TargetStatus = new StatusID[] { StatusID.Exaltation },
    };
    #endregion

    #region Defense Area
    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction CollectiveUnconscious { get; } = new BaseAction(ActionID.CollectiveUnconscious, ActionOption.Defense)
    {
        StatusProvide = new StatusID[] { StatusID.CollectiveUnconscious },
    };

    #endregion

    #region Support
    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction Lightspeed { get; } = new BaseAction(ActionID.Lightspeed)
    {
        ActionCheck = (b, m) => IsLongerThan(10),
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction NeutralSect { get; } = new BaseAction(ActionID.NeutralSect, ActionOption.Heal)
    {
        ActionCheck = (b, m) => IsLongerThan(15),
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction Astrodyne { get; } = new BaseAction(ActionID.Astrodyne, ActionOption.UseResources)
    {
        ActionCheck = (b, m) => !Seals.Contains(SealType.NONE) && IsLongerThan(10),
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction Divination { get; } = new BaseAction(ActionID.Divination, ActionOption.Buff)
    {
        ActionCheck = (b, m) => IsLongerThan(10),
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction Draw { get; } = new BaseAction(ActionID.Draw)
    {
        ActionCheck = (b, m) => DrawnCard == CardType.NONE,
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction Redraw { get; } = new BaseAction(ActionID.Redraw)
    {
        StatusNeed = new[] { StatusID.ClarifyingDraw },
        ActionCheck = (b, m) => DrawnCard != CardType.NONE && Seals.Contains(GetCardSeal(DrawnCard))
        && !Astrodyne.ActionCheck(b, m),
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction MinorArcana { get; } = new BaseAction(ActionID.MinorArcana)
    {
        ActionCheck = (b, m) => InCombat,
    };

    static IBaseAction Balance { get; } = new BaseAction(ActionID.Balance)
    {
        ChoiceTarget = TargetFilter.ASTMeleeTarget,
        ActionCheck = (b, m) => DrawnCard == CardType.BALANCE,
    };

    static IBaseAction Arrow { get; } = new BaseAction(ActionID.Arrow)
    {
        ChoiceTarget = TargetFilter.ASTMeleeTarget,
        ActionCheck = (b, m) => DrawnCard == CardType.ARROW,
    };

    static IBaseAction Spear { get; } = new BaseAction(ActionID.Spear)
    {
        ChoiceTarget = TargetFilter.ASTMeleeTarget,
        ActionCheck = (b, m) => DrawnCard == CardType.SPEAR,
    };

    static IBaseAction Bole { get; } = new BaseAction(ActionID.Bole)
    {
        ChoiceTarget = TargetFilter.ASTRangeTarget,
        ActionCheck = (b, m) => DrawnCard == CardType.BOLE,
    };

    static IBaseAction Ewer { get; } = new BaseAction(ActionID.Ewer)
    {
        ChoiceTarget = TargetFilter.ASTRangeTarget,
        ActionCheck = (b, m) => DrawnCard == CardType.EWER,

    };

    static IBaseAction Spire { get; } = new BaseAction(ActionID.Spire)
    {
        ChoiceTarget = TargetFilter.ASTRangeTarget,
        ActionCheck = (b, m) => DrawnCard == CardType.SPIRE,
    };
    #endregion

    #region Traits
    /// <summary>
    /// 
    /// </summary>
    public static IBaseTrait MaimAndMend { get; } = new BaseTrait(122);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseTrait EnhancedBenefic { get; } = new BaseTrait(124);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseTrait MaimAndMend2 { get; } = new BaseTrait(125);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseTrait CombustMastery { get; } = new BaseTrait(186);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseTrait MaleficMastery { get; } = new BaseTrait(187);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseTrait MaleficMastery2 { get; } = new BaseTrait(188);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseTrait HyperLightspeed { get; } = new BaseTrait(189);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseTrait CombustMastery2 { get; } = new BaseTrait(314);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseTrait MaleficMastery3 { get; } = new BaseTrait(315);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseTrait EnhancedEssentialDignity { get; } = new BaseTrait(316);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseTrait EnhancedDraw { get; } = new BaseTrait(495);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseTrait EnhancedDraw2 { get; } = new BaseTrait(496);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseTrait MaleficMastery4 { get; } = new BaseTrait(497);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseTrait GravityMastery { get; } = new BaseTrait(498);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseTrait EnhancedHealingMagic { get; } = new BaseTrait(499);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseTrait EnhancedCelestialIntersection { get; } = new BaseTrait(500);

    #endregion

    private protected override IBaseAction LimitBreak => AstralStasis;

    /// <summary>
    /// LB
    /// </summary>
    public static IBaseAction AstralStasis { get; } = new BaseAction(ActionID.AstralStasis, ActionOption.Heal)
    {
        ActionCheck = (b, m) => LimitBreakLevel == 3,
    };

    /// <summary>
    /// 
    /// </summary>
    /// <param name="act"></param>
    /// <returns></returns>
    protected static bool PlayCard(out IAction act)
    {
        act = null;
        if (!Seals.Contains(SealType.NONE)) return false;

        if (Balance.CanUse(out act, CanUseOption.OnLastAbility)) return true;
        if (Arrow.CanUse(out act, CanUseOption.OnLastAbility)) return true;
        if (Spear.CanUse(out act, CanUseOption.OnLastAbility)) return true;
        if (Bole.CanUse(out act, CanUseOption.OnLastAbility)) return true;
        if (Ewer.CanUse(out act, CanUseOption.OnLastAbility)) return true;
        if (Spire.CanUse(out act, CanUseOption.OnLastAbility)) return true;

        return false;
    }

    static SealType GetCardSeal(CardType card)
    {
        return card switch
        {
            CardType.BALANCE or CardType.BOLE => SealType.SUN,
            CardType.ARROW or CardType.EWER => SealType.MOON,
            CardType.SPEAR or CardType.SPIRE => SealType.CELESTIAL,
            _ => SealType.NONE,
        };
    }

    /// <summary>
    /// 
    /// </summary>
    public override void DisplayStatus()
    {
        ImGui.Text($"Card: {DrawnCard} : {GetCardSeal(DrawnCard)}");
        ImGui.Text(string.Join(", ", Seals.Select(i => i.ToString())));
        ImGui.Text($"Redraw: {Redraw.ActionCheck(null, false)}");
    }
}
