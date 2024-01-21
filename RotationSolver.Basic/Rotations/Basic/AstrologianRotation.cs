using FFXIVClientStructs.FFXIV.Client.Game;

namespace RotationSolver.Basic.Rotations.Basic;


partial class AstrologianRotation
{
    /// <summary>
    /// 
    /// </summary>
    public override MedicineType MedicineType => MedicineType.Mind;

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

    private sealed protected override IBaseAction Raise => AscendPvE;
    private protected override IBaseAction LimitBreak => AstralStasisPvE;

    public AstrologianRotation()
    {
        //Combust_PvE.Option = ActionOption.Dot;
        CombustPvE.TargetStatus =
        [
            StatusID.Combust,
            StatusID.CombustIi,
            StatusID.CombustIii,
            StatusID.CombustIii_2041,
        ];

        //Benefic_PvE.Option = Helios_PvE.Option = ActionOption.Hot;

        //CelestialIntersection_PvE.Option = Exaltation_PvE.Option 
        //   = CollectiveUnconscious_PvE.Option = ActionOption.Defense;

        //CelestialIntersection_PvE.ChoiceTarget = Exaltation_PvE.ChoiceTarget = TargetFilter.FindAttackedTarget;

        //CelestialIntersection_PvE.TargetStatus = [StatusID.Intersection];
        //Exaltation_PvE.TargetStatus = [StatusID.Exaltation];
        //CollectiveUnconscious_PvE.StatusProvide = [StatusID.CollectiveUnconscious];

        //Lightspeed_PvE.ActionCheck = Divination_PvE.ActionCheck = (b, m) => IsLongerThan(10);
        //NeutralSect_PvE.ActionCheck = (b, m) => IsLongerThan(15);

        //Astrodyne_PvE.Option = ActionOption.UseResources;
        //Astrodyne_PvE.ActionCheck = (b, m) => !Seals.Contains(SealType.NONE)
        //    && IsLongerThan(10);

        //Draw_PvE.ActionCheck = (b, m) => DrawnCard == CardType.NONE;
        //Redraw_PvE.ActionCheck = (b, m) => DrawnCard != CardType.NONE && Seals.Contains(GetCardSeal(DrawnCard))
        //&& !Astrodyne_PvE.ActionCheck(b, m);

        //MinorArcana_PvE.ActionCheck = (b, m) => InCombat;

        //TheBalance_PvE.ChoiceTarget = TheArrow_PvE.ChoiceTarget = TheSpear_PvE.ChoiceTarget = TargetFilter.ASTMeleeTarget;
        //TheBole_PvE.ChoiceTarget = TheEwer_PvE.ChoiceTarget = TheSpire_PvE.ChoiceTarget = TargetFilter.ASTRangeTarget;

        //TheBalance_PvE.ActionCheck = (b, m) => DrawnCard == CardType.BALANCE;
        //TheArrow_PvE.ActionCheck = (b, m) => DrawnCard == CardType.ARROW;
        //TheSpear_PvE.ActionCheck = (b, m) => DrawnCard == CardType.SPEAR;
        //TheBole_PvE.ActionCheck = (b, m) => DrawnCard == CardType.BOLE;
        //TheEwer_PvE.ActionCheck = (b, m) => DrawnCard == CardType.EWER;
        //TheSpire_PvE.ActionCheck = (b, m) => DrawnCard == CardType.SPIRE;

        //AstralStasis_PvE.ActionCheck = (b, m) => LimitBreakLevel == 3;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="act"></param>
    /// <returns></returns>
    protected bool PlayCard(out IAction act)
    {
        act = null;
        if (!Seals.Contains(SealType.NONE)) return false;

        if (TheBalancePvE.CanUse(out act, CanUseOption.OnLastAbility)) return true;
        if (TheArrowPvE.CanUse(out act, CanUseOption.OnLastAbility)) return true;
        if (TheSpearPvE.CanUse(out act, CanUseOption.OnLastAbility)) return true;
        if (TheBolePvE.CanUse(out act, CanUseOption.OnLastAbility)) return true;
        if (TheEwerPvE.CanUse(out act, CanUseOption.OnLastAbility)) return true;
        if (TheSpirePvE.CanUse(out act, CanUseOption.OnLastAbility)) return true;

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
        ImGui.Text($"Redraw: {RedrawPvE.ActionCheck(null, false)}");
    }
}
