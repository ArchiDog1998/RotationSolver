namespace RotationSolver.Basic.Rotations.Basic;

partial class ScholarRotation
{
    /// <inheritdoc/>
    public override MedicineType MedicineType => MedicineType.Mind;

    #region Job Gauge
    /// <summary>
    /// 
    /// </summary>
    public static byte FairyGauge => JobGauge.FairyGauge;

    /// <summary>
    /// 
    /// </summary>
    public static bool HasAetherflow => JobGauge.Aetherflow > 0;

    static float SeraphTimeRaw => JobGauge.SeraphTimer / 1000f;

    /// <summary>
    /// 
    /// </summary>
    public static float SeraphTime => SeraphTimeRaw - DataCenter.DefaultGCDRemain;
    #endregion
    private sealed protected override IBaseAction Raise => ResurrectionPvE;

    static partial void ModifyAdloquiumPvE(ref ActionSetting setting)
    {
        setting.StatusFromSelf = false;
        setting.StatusProvide =
        [
            StatusID.EukrasianDiagnosis,
            StatusID.EukrasianPrognosis,
            StatusID.Galvanize
        ];
        setting.UnlockedByQuestID = 66633;
    }

    static partial void ModifySuccorPvE(ref ActionSetting setting)
    {
        setting.StatusFromSelf = false;
        setting.StatusProvide = [StatusID.Galvanize];
        setting.UnlockedByQuestID = 66634;
    }

    static partial void ModifyLustratePvE(ref ActionSetting setting)
    {
        setting.ActionCheck = () => HasAetherflow;
        setting.UnlockedByQuestID = 66637;
    }

    static partial void ModifySacredSoilPvE(ref ActionSetting setting)
    {
        setting.ActionCheck = () => HasAetherflow && !IsMoving;
        setting.UnlockedByQuestID = 66638;
    }

    static partial void ModifyIndomitabilityPvE(ref ActionSetting setting)
    {
        setting.ActionCheck = () => HasAetherflow;
        setting.UnlockedByQuestID = 67208;
    }

    static partial void ModifyExcogitationPvE(ref ActionSetting setting)
    {
        setting.ActionCheck = () => HasAetherflow;
    }

    static partial void ModifyConsolationPvE(ref ActionSetting setting)
    {
        setting.ActionCheck = () => SeraphTime > 0;
    }

    static partial void ModifyBioPvE(ref ActionSetting setting)
    {
        setting.TargetStatusProvide = [StatusID.Bio, StatusID.BioIi, StatusID.Biolysis];
    }

    static partial void ModifyEnergyDrainPvE(ref ActionSetting setting)
    {
        setting.ActionCheck = () => HasAetherflow;
    }

    static partial void ModifySummonSeraphPvE(ref ActionSetting setting)
    {
        setting.ActionCheck = () => DataCenter.HasPet;
    }

    static partial void ModifySummonEosPvE(ref ActionSetting setting)
    {
        setting.ActionCheck = () => !DataCenter.HasPet && (!Player.HasStatus(true, StatusID.Dissipation)// || Dissipation.WillHaveOneCharge(30) && Dissipation.EnoughLevel
        );
    }

    static partial void ModifyWhisperingDawnPvE(ref ActionSetting setting)
    {
        setting.ActionCheck = () => DataCenter.HasPet;
    }

    static partial void ModifyFeyIlluminationPvE(ref ActionSetting setting)
    {
        setting.ActionCheck = () => DataCenter.HasPet;
    }

    static partial void ModifyDissipationPvE(ref ActionSetting setting)
    {
        setting.StatusProvide = [StatusID.Dissipation];
        setting.ActionCheck = () => !HasAetherflow && SeraphTime <= 0 && InCombat && DataCenter.HasPet;
        setting.UnlockedByQuestID = 67212;
    }

    static partial void ModifyAetherpactPvE(ref ActionSetting setting)
    {
        setting.ActionCheck = () => FairyGauge >= 10 && DataCenter.HasPet && SeraphTime <= 0;
        setting.UnlockedByQuestID = 68463;
    }

    static partial void ModifyFeyBlessingPvE(ref ActionSetting setting)
    {
        setting.ActionCheck = () => SeraphTime <= 0 && DataCenter.HasPet;
    }

    static partial void ModifyAetherflowPvE(ref ActionSetting setting)
    {
        setting.ActionCheck = () => InCombat && !HasAetherflow;
    }

    static partial void ModifyChainStratagemPvE(ref ActionSetting setting)
    {
        setting.ActionCheck = () => InCombat;
        setting.CreateConfig = () => new()
        {
            TimeToKill = 10,
        };
    }

    static partial void ModifyArtOfWarPvE(ref ActionSetting setting)
    {
        setting.CreateConfig = () => new()
        {
            AoeCount = 2,
        };
    }

    static partial void ModifyDeploymentTacticsPvE(ref ActionSetting setting)
    {
        setting.TargetStatusNeed = [StatusID.Galvanize];
        setting.UnlockedByQuestID = 67210;
    }

    static partial void ModifyBroilPvE(ref ActionSetting setting)
    {
        setting.UnlockedByQuestID = 67209;
    }

    /// <inheritdoc/>
    [RotationDesc(ActionID.ExpedientPvE)]
    protected override bool SpeedAbility(IAction nextGCD, out IAction? act)
    {
        if (InCombat && ExpedientPvE.CanUse(out act, usedUp: true)) return true;
        return base.SpeedAbility(nextGCD, out act);
    }
}