namespace RotationSolver.Basic.Rotations.Basic;

partial class AstrologianRotation
{
    #region JobGauge

    /// <summary>
    /// 
    /// </summary>
    public override MedicineType MedicineType => MedicineType.Mind;

    /// <summary>
    /// NONE = 0, BALANCE = 1, BOLE = 2, ARROW = 3, SPEAR = 4, EWERS = 5, SPIRE = 6
    /// </summary>
    protected static CardType[] DrawnCard => JobGauge.DrawnCards;

    /// <summary>
    /// Indicates the state of Minor Arcana and which card will be used next when activating Minor Arcana, LORD = 7, LADY = 8
    /// </summary>
    protected static CardType DrawnCrownCard => JobGauge.DrawnCrownCard;

    /// <summary>
    ///  Can use Umbral or Astral draw, active draw matching what the next draw will be, ASTRAL, UMBRAL
    /// </summary>
    protected static DrawType ActiveDraw => JobGauge.ActiveDraw;

    #endregion


    private sealed protected override IBaseAction? Raise => AscendPvE;

    private static readonly StatusID[] CombustStatus =
    [
        StatusID.Combust,
        StatusID.CombustIi,
        StatusID.CombustIii,
        StatusID.CombustIii_2041,
    ];

    static partial void ModifyMaleficPvE(ref ActionSetting setting)
    {
        
    }

    static partial void ModifyBeneficPvE(ref ActionSetting setting)
    {
        setting.IsFriendly = true;
    }

    static partial void ModifyCombustPvE(ref ActionSetting setting)
    {
        setting.TargetStatusProvide = CombustStatus;
    }

    static partial void ModifyLightspeedPvE(ref ActionSetting setting)
    {
        setting.StatusProvide = [StatusID.Lightspeed];
        setting.CreateConfig = () => new()
        {
            TimeToKill = 10,
        };
    }

    static partial void ModifyHeliosPvE(ref ActionSetting setting)
    {
        setting.IsFriendly = true;
    }

    static partial void ModifyAscendPvE(ref ActionSetting setting)
    {
        setting.IsFriendly = true;
    }

    static partial void ModifyEssentialDignityPvE(ref ActionSetting setting)
    {
        setting.IsFriendly = true;
    }

    static partial void ModifyBeneficIiPvE(ref ActionSetting setting)
    {
        setting.IsFriendly = true;
    }

    static partial void ModifyAstralDrawPvE(ref ActionSetting setting)
    {
        setting.ActionCheck = () => ActiveDraw == DrawType.ASTRAL && DrawnCard.All(card => card != CardType.SPEAR);
    }

    static partial void ModifyUmbralDrawPvE(ref ActionSetting setting)
    {
        setting.ActionCheck = () => ActiveDraw == DrawType.UMBRAL && DrawnCard.All(card => card != CardType.BALANCE);
    }

    static partial void ModifyPlayIPvE(ref ActionSetting setting) //37019
    {
        
    }

    static partial void ModifyPlayIiPvE(ref ActionSetting setting) //37020
    {
        
    }

    static partial void ModifyPlayIiiPvE(ref ActionSetting setting) //37021
    {
        
    }

    static partial void ModifyTheBalancePvE(ref ActionSetting setting) 
    {
        setting.ActionCheck = () => DrawnCard.Any(card => card == CardType.BALANCE);
        setting.TargetStatusProvide = [StatusID.TheBalance_3887, StatusID.Weakness,
        StatusID.BrinkOfDeath];
        setting.TargetType = TargetType.Melee;
        setting.IsFriendly = true;
    }

    static partial void ModifyTheArrowPvE(ref ActionSetting setting)
    {
        setting.ActionCheck = () => DrawnCard.Any(card => card == CardType.ARROW);
        setting.TargetStatusProvide = [StatusID.TheArrow_3888, StatusID.Weakness,
        StatusID.BrinkOfDeath];
        setting.TargetType = TargetType.BeAttacked;
        setting.IsFriendly = true;
    }

    static partial void ModifyTheSpirePvE(ref ActionSetting setting)
    {
        setting.ActionCheck = () => DrawnCard.Any(card => card == CardType.SPIRE);
        setting.TargetStatusProvide = [StatusID.TheSpire_3892, StatusID.Weakness,
        StatusID.BrinkOfDeath];
        setting.TargetType = TargetType.BeAttacked;
        setting.IsFriendly = true;
    }

    static partial void ModifyTheSpearPvE(ref ActionSetting setting)
    {
        setting.ActionCheck = () => DrawnCard.Any(card => card == CardType.SPEAR);
        setting.TargetStatusProvide = [StatusID.TheSpear_3889, StatusID.Weakness,
        StatusID.BrinkOfDeath];
        setting.TargetType = TargetType.Range;
        setting.IsFriendly = true;
    }

    static partial void ModifyTheBolePvE(ref ActionSetting setting)
    {
        setting.ActionCheck = () => DrawnCard.Any(card => card == CardType.BOLE);
        setting.TargetStatusProvide = [StatusID.TheBole_3890, StatusID.Weakness,
        StatusID.BrinkOfDeath];
        setting.TargetType = TargetType.BeAttacked;
        setting.IsFriendly = true;
    }

    static partial void ModifyTheEwerPvE(ref ActionSetting setting)
    {
        setting.ActionCheck = () => DrawnCard.Any(card => card == CardType.EWER);
        setting.TargetStatusProvide = [StatusID.TheEwer_3891, StatusID.Weakness,
        StatusID.BrinkOfDeath];
        setting.TargetType = TargetType.BeAttacked;
        setting.IsFriendly = true;
    }

    static partial void ModifyAspectedBeneficPvE(ref ActionSetting setting)
    {
        setting.StatusProvide = [StatusID.AspectedBenefic];
    }

    static partial void ModifyAspectedHeliosPvE(ref ActionSetting setting)
    {
        setting.StatusProvide = [StatusID.AspectedHelios];
        setting.UnlockedByQuestID = 67551;
        setting.IsFriendly = true;
    }

    static partial void ModifyGravityPvE(ref ActionSetting setting)
    {
        setting.CreateConfig = () => new()
        {
            AoeCount = 2,
        };
        setting.UnlockedByQuestID = 67553;
    }

    static partial void ModifyCombustIiPvE(ref ActionSetting setting)
    {
        setting.TargetStatusProvide = CombustStatus;
    }

    static partial void ModifySynastryPvE(ref ActionSetting setting)
    {
        setting.TargetStatusProvide = [StatusID.Synastry_846];
        setting.StatusProvide = [StatusID.Synastry];
        setting.UnlockedByQuestID = 67554;
        setting.IsFriendly = true;
    }

    static partial void ModifyDivinationPvE(ref ActionSetting setting)
    {
        setting.TargetStatusProvide = [StatusID.Divination];
        setting.StatusProvide = [StatusID.Divining]; //need to double check this status
        setting.CreateConfig = () => new()
        {
            TimeToKill = 10,
        };
    }

    static partial void ModifyMaleficIiPvE(ref ActionSetting setting)
    {
        setting.UnlockedByQuestID = 67558;
    }

    static partial void ModifyCollectiveUnconsciousPvE(ref ActionSetting setting)
    {
        setting.TargetStatusProvide = [StatusID.CollectiveUnconscious, StatusID.WheelOfFortune];
        setting.UnlockedByQuestID = 67560;
        setting.IsFriendly = true;
    }

    static partial void ModifyCelestialOppositionPvE(ref ActionSetting setting)
    {
        setting.TargetStatusProvide = [StatusID.Opposition];
        setting.UnlockedByQuestID = 67561;
        setting.IsFriendly = true;
    }

    static partial void ModifyEarthlyStarPvE(ref ActionSetting setting)
    {
        setting.StatusProvide = [StatusID.EarthlyDominance, StatusID.GiantDominance];
        setting.CreateConfig = () => new()
        {
            TimeToKill = 10,
        };
    }

    static partial void ModifyStellarDetonationPvE(ref ActionSetting setting)
    {
        setting.StatusNeed = [StatusID.GiantDominance];
    }

    static partial void ModifyMaleficIiiPvE(ref ActionSetting setting)
    {
        
    }

    static partial void ModifyMinorArcanaPvE(ref ActionSetting setting)
    {

    }

    static partial void ModifyLordOfCrownsPvE(ref ActionSetting setting)
    {
        setting.ActionCheck = () => DrawnCard.All(card => card == CardType.NONE);
        setting.ActionCheck = () => DrawnCrownCard == CardType.LORD;
    }

    static partial void ModifyLadyOfCrownsPvE(ref ActionSetting setting)
    {
        setting.ActionCheck = () => DrawnCard.All(card => card == CardType.NONE);
        setting.ActionCheck = () => DrawnCrownCard == CardType.LADY;
        setting.IsFriendly = true;
    }

    static partial void ModifyCombustIiiPvE(ref ActionSetting setting)
    {
        setting.StatusNeed = CombustStatus;
    }

    static partial void ModifyMaleficIvPvE(ref ActionSetting setting)
    {

    }

    static partial void ModifyCelestialIntersectionPvE(ref ActionSetting setting)
    {
        setting.TargetStatusProvide = [StatusID.Intersection];
        setting.IsFriendly = true;
    }

    static partial void ModifyHoroscopePvE(ref ActionSetting setting)
    {
        setting.TargetStatusProvide = [StatusID.Horoscope];
        setting.IsFriendly = true;
    }

    static partial void ModifyHoroscopePvE_16558(ref ActionSetting setting)
    {
        setting.StatusNeed = [StatusID.Horoscope];
        setting.IsFriendly = true;
    }

    static partial void ModifyNeutralSectPvE(ref ActionSetting setting)
    {
        setting.CreateConfig = () => new()
        {
            TimeToKill = 15,
        };
        setting.IsFriendly = true;
        setting.StatusProvide = [StatusID.NeutralSect, StatusID.Suntouched];
    }

    static partial void ModifyFallMaleficPvE(ref ActionSetting setting)
    {

    }

    static partial void ModifyGravityIiPvE(ref ActionSetting setting)
    {
        setting.CreateConfig = () => new()
        {
            AoeCount = 2,
        };
    }

    static partial void ModifyExaltationPvE(ref ActionSetting setting)
    {
        setting.TargetStatusProvide = [StatusID.Exaltation];
        setting.IsFriendly = true;
    }

    static partial void ModifyMacrocosmosPvE(ref ActionSetting setting)
    {
        setting.TargetStatusProvide = [StatusID.Macrocosmos];
        setting.StatusProvide = [StatusID.Macrocosmos];
    }

    static partial void ModifyMicrocosmosPvE(ref ActionSetting setting)
    {
        setting.StatusNeed = [StatusID.Macrocosmos];
        setting.IsFriendly = true;
    }

    static partial void ModifyOraclePvE(ref ActionSetting setting)
    {
        setting.StatusNeed = [StatusID.Divining];
    }

    static partial void ModifyHeliosConjunctionPvE(ref ActionSetting setting)
    {
        setting.StatusProvide = [StatusID.HeliosConjunction];
        setting.IsFriendly = true;
    }

    static partial void ModifySunSignPvE(ref ActionSetting setting)
    {
        setting.StatusNeed = [StatusID.Suntouched];
        setting.IsFriendly = true;
    }
}
