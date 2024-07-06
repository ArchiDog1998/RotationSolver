namespace RotationSolver.Basic.Rotations.Basic;

partial class AstrologianRotation
{
    #region JobGauge

    /// <summary>
    /// 
    /// </summary>
    public override MedicineType MedicineType => MedicineType.Mind;

    /// <summary>
    /// 
    /// </summary>
    protected static CardType[] DrawnCard => JobGauge.DrawnCards;

    /// <summary>
    /// 
    /// </summary>
    protected static CardType DrawnCrownCard => JobGauge.DrawnCrownCard;

    /// <summary>
    /// 
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

    static partial void ModifyCelestialOppositionPvE(ref ActionSetting setting)
    {
        setting.UnlockedByQuestID = 67561;
    }

    static partial void ModifyExaltationPvE(ref ActionSetting setting)
    {
        setting.TargetStatusProvide = [StatusID.Exaltation];
    }

    static partial void ModifyCollectiveUnconsciousPvE(ref ActionSetting setting)
    {
        setting.TargetStatusProvide = [StatusID.CollectiveUnconscious];
        setting.UnlockedByQuestID = 67560;
    }

    static partial void ModifyMinorArcanaPvE(ref ActionSetting setting)
    {
        setting.ActionCheck = () => InCombat;
        setting.UnlockedByQuestID = 67949;
    }

    static partial void ModifyAspectedHeliosPvE(ref ActionSetting setting)
    {
        setting.StatusProvide = [StatusID.AspectedHelios];
    }

    static partial void ModifyAspectedBeneficPvE(ref ActionSetting setting)
    {
        setting.StatusProvide = [StatusID.AspectedBenefic];
    }

    static partial void ModifyDivinationPvE(ref ActionSetting setting)
    {
        setting.CreateConfig = () => new()
        {
            TimeToKill = 10,
        };
    }

    static partial void ModifyEarthlyStarPvE(ref ActionSetting setting)
    {
        setting.CreateConfig = () => new()
        {
            TimeToKill = 10,
        };
    }

    static partial void ModifyGravityPvE(ref ActionSetting setting)
    {
        setting.CreateConfig = () => new()
        {
            AoeCount = 2,
        };
        setting.UnlockedByQuestID = 67553;
    }

      /*static partial void ModifyTheArrowPvE(ref ActionSetting setting)
      {
          setting.TargetStatusProvide = StatusHelper.AstCardStatus;
          setting.StatusFromSelf = false;
          setting.TargetType = TargetType.Melee;
          setting.ActionCheck = () => DrawnCard == CardType.ARROW;
      }

      static partial void ModifyTheBalancePvE(ref ActionSetting setting)
      {
          setting.TargetStatusProvide = StatusHelper.AstCardStatus;
          setting.StatusFromSelf = false;
          setting.TargetType = TargetType.Melee;
          setting.ActionCheck = () => DrawnCard == CardType.BALANCE;
      }

      static partial void ModifyTheBolePvE(ref ActionSetting setting)
      {
          setting.TargetStatusProvide = StatusHelper.AstCardStatus;
          setting.StatusFromSelf = false;
          setting.TargetType = TargetType.Range;
          setting.ActionCheck = () => DrawnCard == CardType.BOLE;
      }

      static partial void ModifyTheEwerPvE(ref ActionSetting setting)
      {
          setting.TargetStatusProvide = StatusHelper.AstCardStatus;
          setting.StatusFromSelf = false;
          setting.TargetType = TargetType.Range;
          setting.ActionCheck = () => DrawnCard == CardType.EWER;
      }

      static partial void ModifyTheSpearPvE(ref ActionSetting setting)
      {
          setting.TargetStatusProvide = StatusHelper.AstCardStatus;
          setting.StatusFromSelf = false;
          setting.TargetType = TargetType.Melee;
          setting.ActionCheck = () => DrawnCard == CardType.SPEAR;
      }

      static partial void ModifyTheSpirePvE(ref ActionSetting setting)
      {
          setting.TargetStatusProvide = StatusHelper.AstCardStatus;
          setting.StatusFromSelf = false;
          setting.TargetType = TargetType.Range;
          setting.ActionCheck = () => DrawnCard == CardType.SPIRE;
      }*/

    static partial void ModifyLightspeedPvE(ref ActionSetting setting)
    {
        setting.CreateConfig = () => new()
        {
            TimeToKill = 10,
        };
    }

    static partial void ModifyNeutralSectPvE(ref ActionSetting setting)
    {
        setting.CreateConfig = () => new()
        {
            TimeToKill = 15,
        };
    }

    static partial void ModifySynastryPvE(ref ActionSetting setting)
    {
        setting.UnlockedByQuestID = 67554;
    }

    static partial void ModifyMaleficIiPvE(ref ActionSetting setting)
    {
        setting.UnlockedByQuestID = 67558;
    }
   /* static  GetCardSeal(CardType card)
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
 }*/
}
