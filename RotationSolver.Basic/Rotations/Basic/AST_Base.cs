namespace RotationSolver.Basic.Rotations.Basic;

public abstract class AST_Base : CustomRotation
{
    public override MedicineType MedicineType => MedicineType.Mind;

    public sealed override ClassJobID[] JobIDs => new ClassJobID[] { ClassJobID.Astrologian };

    static ASTGauge JobGauge => Service.JobGauges.Get<ASTGauge>();

    protected static CardType DrawnCard => JobGauge.DrawnCard;

    protected static CardType DrawnCrownCard => JobGauge.DrawnCrownCard;

    protected static SealType[] Seals => JobGauge.Seals;

    #region Attack
    public static IBaseAction Malefic { get; } = new BaseAction(ActionID.Malefic);

    public static IBaseAction Combust { get; } = new BaseAction(ActionID.Combust, ActionOption.Dot)
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
    #endregion

    #region Heal Single
    private sealed protected override IBaseAction Raise => Ascend;

    public static IBaseAction Ascend { get; } = new BaseAction(ActionID.Ascend, ActionOption.Friendly);

    public static IBaseAction Benefic { get; } = new BaseAction(ActionID.Benefic, ActionOption.Heal);

    public static IBaseAction Benefic2 { get; } = new BaseAction(ActionID.Benefic2, ActionOption.Heal);

    public static IBaseAction AspectedBenefic { get; } = new BaseAction(ActionID.AspectedBenefic, ActionOption.Hot)
    {
        TargetStatus = new StatusID[] { StatusID.AspectedBenefic },
    };

    public static IBaseAction EssentialDignity { get; } = new BaseAction(ActionID.EssentialDignity, ActionOption.Heal);

    public static IBaseAction Synastry { get; } = new BaseAction(ActionID.Synastry, ActionOption.Heal);
    #endregion

    #region Heal Area
    public static IBaseAction Helios { get; } = new BaseAction(ActionID.Helios, ActionOption.Heal);

    public static IBaseAction AspectedHelios { get; } = new BaseAction(ActionID.AspectedHelios, ActionOption.Hot)
    {
        ActionCheck = b => !IsLastGCD(ActionID.AspectedHelios),
        StatusProvide = new StatusID[] { StatusID.AspectedHelios },
    };

    public static IBaseAction CelestialOpposition { get; } = new BaseAction(ActionID.CelestialOpposition, ActionOption.Heal);

    public static IBaseAction EarthlyStar { get; } = new BaseAction(ActionID.EarthlyStar, ActionOption.Heal);

    public static IBaseAction Horoscope { get; } = new BaseAction(ActionID.Horoscope, ActionOption.Heal);

    public static IBaseAction Macrocosmos { get; } = new BaseAction(ActionID.Macrocosmos, ActionOption.Heal);

    #endregion

    #region Defense Single

    public static IBaseAction CelestialIntersection { get; } = new BaseAction(ActionID.CelestialIntersection, ActionOption.Defense)
    {
        ChoiceTarget = TargetFilter.FindAttackedTarget,
        TargetStatus = new StatusID[] { StatusID.Intersection },
    };

    public static IBaseAction Exaltation { get; } = new BaseAction(ActionID.Exaltation, ActionOption.Heal)
    {
        ChoiceTarget = TargetFilter.FindAttackedTarget,
    };
    #endregion

    #region Defense Area

    public static IBaseAction CollectiveUnconscious { get; } = new BaseAction(ActionID.CollectiveUnconscious, ActionOption.Defense);

    #endregion


    #region Support
    public static IBaseAction Lightspeed { get; } = new BaseAction(ActionID.Lightspeed);

    public static IBaseAction NeutralSect { get; } = new BaseAction(ActionID.NeutralSect, ActionOption.Heal);

    public static IBaseAction Astrodyne { get; } = new BaseAction(ActionID.Astrodyne)
    {
        ActionCheck = b =>
        {
            if (JobGauge.Seals.Length != 3) return false;
            if (JobGauge.Seals.Contains(SealType.NONE)) return false;
            return true;
        },
    };

    public static IBaseAction Divination { get; } = new BaseAction(ActionID.Divination, ActionOption.Buff);

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
        ActionCheck = b => InCombat && DrawnCrownCard == CardType.NONE,
    };

    static IBaseAction Balance { get; } = new BaseAction(ActionID.Balance)
    {
        ChoiceTarget = TargetFilter.ASTMeleeTarget,
        ActionCheck = b => DrawnCard == CardType.BALANCE,
    };

    static IBaseAction Arrow { get; } = new BaseAction(ActionID.Arrow)
    {
        ChoiceTarget = TargetFilter.ASTMeleeTarget,
        ActionCheck = b => DrawnCard == CardType.ARROW,
    };

    static IBaseAction Spear { get; } = new BaseAction(ActionID.Spear)
    {
        ChoiceTarget = TargetFilter.ASTMeleeTarget,
        ActionCheck = b => DrawnCard == CardType.SPEAR,
    };

    static IBaseAction Bole { get; } = new BaseAction(ActionID.Bole)
    {
        ChoiceTarget = TargetFilter.ASTRangeTarget,
        ActionCheck = b => DrawnCard == CardType.BOLE,
    };

    static IBaseAction Ewer { get; } = new BaseAction(ActionID.Ewer)
    {
        ChoiceTarget = TargetFilter.ASTRangeTarget,
        ActionCheck = b => DrawnCard == CardType.EWER,

    };

    static IBaseAction Spire { get; } = new BaseAction(ActionID.Spire)
    {
        ChoiceTarget = TargetFilter.ASTRangeTarget,
        ActionCheck = b => DrawnCard == CardType.SPIRE,
    };
    #endregion

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
