namespace RotationSolver.Basic.Rotations.Basic;

public abstract class AST_Base : CustomRotation
{
    private static ASTGauge JobGauge => Service.JobGauges.Get<ASTGauge>();

    public override MedicineType MedicineType => MedicineType.Mind;

    private static CardType DrawnCard => JobGauge.DrawnCard;

    protected static CardType DrawnCrownCard => JobGauge.DrawnCrownCard;

    protected static SealType[] Seals => JobGauge.Seals;

    public sealed override ClassJobID[] JobIDs => new ClassJobID[] { ClassJobID.Astrologian };

    private sealed protected override IBaseAction Raise => Ascend;

    public static IBaseAction Ascend { get; } = new BaseAction(ActionID.Ascend, true);

    public static IBaseAction Malefic { get; } = new BaseAction(ActionID.Malefic);

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

    public static IBaseAction Gravity { get; } = new BaseAction(ActionID.Gravity);

    public static IBaseAction Benefic { get; } = new BaseAction(ActionID.Benefic, true, isTimeline: true);

    public static IBaseAction Benefic2 { get; } = new BaseAction(ActionID.Benefic2, true, isTimeline: true);

    public static IBaseAction AspectedBenefic { get; } = new BaseAction(ActionID.AspectedBenefic, true, isEot: true, isTimeline: true)
    {
        TargetStatus = new StatusID[] { StatusID.AspectedBenefic },
    };

    public static IBaseAction EssentialDignity { get; } = new BaseAction(ActionID.EssentialDignity, true, isTimeline: true);

    public static IBaseAction Synastry { get; } = new BaseAction(ActionID.Synastry, true, isTimeline: true);

    public static IBaseAction CelestialIntersection { get; } = new BaseAction(ActionID.CelestialIntersection, true, isTimeline: true)
    {
        ChoiceTarget = TargetFilter.FindAttackedTarget,

        TargetStatus = new StatusID[] { StatusID.Intersection },
    };

    public static IBaseAction Exaltation { get; } = new BaseAction(ActionID.Exaltation, true, isTimeline: true)
    {
        ChoiceTarget = TargetFilter.FindAttackedTarget,
    };

    public static IBaseAction Helios { get; } = new BaseAction(ActionID.Helios, true, isTimeline: true);

    public static IBaseAction AspectedHelios { get; } = new BaseAction(ActionID.AspectedHelios, true, isEot: true, isTimeline: true)
    {
        ActionCheck = b => !IsLastGCD(ActionID.AspectedHelios),
        StatusProvide = new StatusID[] { StatusID.AspectedHelios },
    };

    public static IBaseAction CelestialOpposition { get; } = new BaseAction(ActionID.CelestialOpposition, true, isTimeline: true);

    public static IBaseAction EarthlyStar { get; } = new BaseAction(ActionID.EarthlyStar, true, isTimeline: true);

    public static IBaseAction CollectiveUnconscious { get; } = new BaseAction(ActionID.CollectiveUnconscious, true, isTimeline: true);

    public static IBaseAction Horoscope { get; } = new BaseAction(ActionID.Horoscope, true, isTimeline: true);

    public static IBaseAction Lightspeed { get; } = new BaseAction(ActionID.Lightspeed);

    public static IBaseAction NeutralSect { get; } = new BaseAction(ActionID.NeutralSect, isTimeline: true);

    public static IBaseAction Macrocosmos { get; } = new BaseAction(ActionID.Macrocosmos, isTimeline: true);

    public static IBaseAction Astrodyne { get; } = new BaseAction(ActionID.Astrodyne)
    {
        ActionCheck = b =>
        {
            if (JobGauge.Seals.Length != 3) return false;
            if (JobGauge.Seals.Contains(SealType.NONE)) return false;
            return true;
        },
    };

    public static IBaseAction Divination { get; } = new BaseAction(ActionID.Divination, true);

    public static IBaseAction Draw { get; } = new BaseAction(ActionID.Draw)
    {
        ActionCheck = b => DrawnCard == CardType.NONE,
    };

    public static IBaseAction Redraw { get; } = new BaseAction(ActionID.Redraw)
    {
        StatusNeed = new[] { StatusID.ClarifyingDraw },
        ActionCheck = b => DrawnCard != CardType.NONE && Seals.Contains(GetCardSeal(DrawnCard)),
    };

    public static IBaseAction MinorArcana { get; } = new BaseAction(ActionID.MinorArcana)
    {
        ActionCheck = b => InCombat,
    };

    //public static IBaseAction CrownPlay { get; } = new BaseAction(ActionID.CrownPlay)
    //{
    //    ActionCheck = b => DrawnCrownCard is CardType.LADY or CardType.LORD,
    //};

    private static IBaseAction Balance { get; } = new BaseAction(ActionID.Balance)
    {
        ChoiceTarget = TargetFilter.ASTMeleeTarget,
        ActionCheck = b => DrawnCard == CardType.BALANCE,
    };

    private static IBaseAction Arrow { get; } = new BaseAction(ActionID.Arrow)
    {
        ChoiceTarget = TargetFilter.ASTMeleeTarget,
        ActionCheck = b => DrawnCard == CardType.ARROW,
    };

    private static IBaseAction Spear { get; } = new BaseAction(ActionID.Spear)
    {
        ChoiceTarget = TargetFilter.ASTMeleeTarget,
        ActionCheck = b => DrawnCard == CardType.SPEAR,
    };

    private static IBaseAction Bole { get; } = new BaseAction(ActionID.Bole)
    {
        ChoiceTarget = TargetFilter.ASTRangeTarget,
        ActionCheck = b => DrawnCard == CardType.BOLE,
    };

    private static IBaseAction Ewer { get; } = new BaseAction(ActionID.Ewer)
    {
        ChoiceTarget = TargetFilter.ASTRangeTarget,
        ActionCheck = b => DrawnCard == CardType.EWER,

    };

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
