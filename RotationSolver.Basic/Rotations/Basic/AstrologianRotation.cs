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

    private sealed protected override IBaseAction? Raise => AscendPvE;

    private static readonly StatusID[] CombustStatus = 
    [
        StatusID.Combust,
        StatusID.CombustIi,
        StatusID.CombustIii,
        StatusID.CombustIii_2041,
    ];


    static partial void ModifyCombustPvE(ref ActionSetting setting)
    {
        setting.TargetStatusProvide = CombustStatus;
    }

    static partial void ModifyCombustIiPvE(ref ActionSetting setting)
    {
        setting.TargetStatusProvide = CombustStatus;
    }

    static partial void ModifyCombustIiiPvE(ref ActionSetting setting)
    {
        setting.TargetStatusProvide = CombustStatus;
    }

    static partial void ModifyCelestialIntersectionPvE(ref ActionSetting setting)
    {
        setting.TargetStatusProvide = [StatusID.Intersection];
    }

    static partial void ModifyExaltationPvE(ref ActionSetting setting)
    {
        setting.TargetStatusProvide = [StatusID.Exaltation];
    }

    static partial void ModifyCollectiveUnconsciousPvE(ref ActionSetting setting)
    {
        setting.TargetStatusProvide = [StatusID.CollectiveUnconscious];
    }

    static partial void ModifyAstrodynePvE(ref ActionSetting setting)
    {
        setting.ActionCheck = () => !Seals.Contains(SealType.NONE);
    }

    static partial void ModifyDrawPvE(ref ActionSetting setting)
    {
        setting.ActionCheck = () => DrawnCard == CardType.NONE;
    }

    static partial void ModifyRedrawPvE(ref ActionSetting setting)
    {
        setting.ActionCheck = () => DrawnCard != CardType.NONE && Seals.Contains(GetCardSeal(DrawnCard));
    }

    static partial void ModifyMinorArcanaPvE(ref ActionSetting setting)
    {
        setting.ActionCheck = () => InCombat;
    }


    public AstrologianRotation()
    {
        this.AllBaseActions
        //Lightspeed_PvE.ActionCheck = Divination_PvE.ActionCheck = (b, m) => IsLongerThan(10);
        //NeutralSect_PvE.ActionCheck = (b, m) => IsLongerThan(15);

        //Astrodyne_PvE.Option = ActionOption.UseResources;
        //Astrodyne_PvE.ActionCheck = (b, m) => !Seals.Contains(SealType.NONE)
        //    && IsLongerThan(10);

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

    static partial void ModifyTheArrowPvE(ref ActionSetting setting)
    {
        setting.TargetStatusProvide = StatusHelper.AstCardStatus;
        setting.TargetStatusFromSelf = false;
        setting.TargetType = TargetType.Melee;
        setting.ActionCheck = () => DrawnCard == CardType.ARROW;
    }

    static partial void ModifyTheBalancePvE(ref ActionSetting setting)
    {
        setting.TargetStatusProvide = StatusHelper.AstCardStatus;
        setting.TargetStatusFromSelf = false;
        setting.TargetType = TargetType.Melee;
        setting.ActionCheck = () => DrawnCard == CardType.BALANCE;
    }

    static partial void ModifyTheBolePvE(ref ActionSetting setting)
    {
        setting.TargetStatusProvide = StatusHelper.AstCardStatus;
        setting.TargetStatusFromSelf = false;
        setting.TargetType = TargetType.Range;
        setting.ActionCheck = () => DrawnCard == CardType.BOLE;
    }

    static partial void ModifyTheEwerPvE(ref ActionSetting setting)
    {
        setting.TargetStatusProvide = StatusHelper.AstCardStatus;
        setting.TargetStatusFromSelf = false;
        setting.TargetType = TargetType.Range;
        setting.ActionCheck = () => DrawnCard == CardType.EWER;
    }

    static partial void ModifyTheSpearPvE(ref ActionSetting setting)
    {
        setting.TargetStatusProvide = StatusHelper.AstCardStatus;
        setting.TargetStatusFromSelf = false;
        setting.TargetType = TargetType.Melee;
        setting.ActionCheck = () => DrawnCard == CardType.SPEAR;
    }

    static partial void ModifyTheSpirePvE(ref ActionSetting setting)
    {
        setting.TargetStatusProvide = StatusHelper.AstCardStatus;
        setting.TargetStatusFromSelf = false;
        setting.TargetType = TargetType.Range;
        setting.ActionCheck = () => DrawnCard == CardType.SPIRE;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="act"></param>
    /// <returns></returns>
    protected bool PlayCard(out IAction? act)
    {
        act = null;
        if (!Seals.Contains(SealType.NONE)) return false;

        if (TheBalancePvE.CanUse(out act)) return true;
        if (TheArrowPvE.CanUse(out act)) return true;
        if (TheSpearPvE.CanUse(out act)) return true;
        if (TheBolePvE.CanUse(out act)) return true;
        if (TheEwerPvE.CanUse(out act)) return true;
        if (TheSpirePvE.CanUse(out act)) return true;

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
    }
}
