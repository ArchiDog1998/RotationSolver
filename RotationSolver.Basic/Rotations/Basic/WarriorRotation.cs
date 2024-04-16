namespace RotationSolver.Basic.Rotations.Basic;

partial class WarriorRotation
{
    /// <inheritdoc/>
    public override MedicineType MedicineType => MedicineType.Strength;


    /// <summary>
    /// 
    /// </summary>
    public static byte BeastGauge => JobGauge.BeastGauge;

    private sealed protected override IBaseAction TankStance => DefiancePvE;

    static partial void ModifyStormsEyePvE(ref ActionSetting setting)
    {
        setting.CreateConfig = () => new()
        {
            StatusGcdCount = 9,
        };
        setting.StatusProvide = [StatusID.SurgingTempest];
    }

    static partial void ModifyInnerBeastPvE(ref ActionSetting setting)
    {
        setting.ActionCheck = () => BeastGauge >= 50 || Player.HasStatus(true, StatusID.InnerRelease);
        setting.UnlockedByQuestID = 66586;
    }

    static partial void ModifyTomahawkPvE(ref ActionSetting setting)
    {
        setting.SpecialType = SpecialActionType.MeleeRange;
        setting.UnlockedByQuestID = 65852;
    }

    static partial void ModifyUpheavalPvE(ref ActionSetting setting)
    {
        //TODO: Why is that status? Answer: This is Warrior's 10% damage buff. Don't want to cast Upheaval at the start of combat without the buff. 
        setting.StatusNeed = [StatusID.SurgingTempest];
    }

    static partial void ModifySteelCyclonePvE(ref ActionSetting setting)
    {
        setting.ActionCheck = () => BeastGauge >= 50 || Player.HasStatus(true, StatusID.InnerRelease);
        setting.CreateConfig = () => new()
        {
            AoeCount = 2,
        };
        setting.UnlockedByQuestID = 66589;
    }

    static partial void ModifyPrimalRendPvE(ref ActionSetting setting)
    {
        setting.StatusNeed = [StatusID.PrimalRendReady];
        setting.SpecialType = SpecialActionType.MovingForward;
    }

    static partial void ModifyPrimalRendPvP(ref ActionSetting setting)
    {
        setting.SpecialType = SpecialActionType.MovingForward;
    }

    static partial void ModifyInfuriatePvE(ref ActionSetting setting)
    {
        setting.StatusProvide = [StatusID.NascentChaos];
        setting.ActionCheck = () => HasHostilesInRange && BeastGauge <= 50 && InCombat;
        setting.CreateConfig = () => new()
        {
            TimeToKill = 0,
        };
        setting.UnlockedByQuestID = 66590;
    }

    static partial void ModifyInnerReleasePvE(ref ActionSetting setting)
    {
        setting.CreateConfig = () => new()
        {
            TimeToKill = 0,
        };
        setting.UnlockedByQuestID = 68440;
    }

    static partial void ModifyBerserkPvE(ref ActionSetting setting)
    {
        setting.ActionCheck = () => HasHostilesInRange && !ActionID.InnerReleasePvE.IsCoolingDown();
        setting.CreateConfig = () => new()
        {
            TimeToKill = 0,
        };
    }

    static partial void ModifyOverpowerPvE(ref ActionSetting setting)
    {
        setting.CreateConfig = () => new()
        {
            AoeCount = 2,
        };
    }

    static partial void ModifyMythrilTempestPvE(ref ActionSetting setting)
    {
        setting.CreateConfig = () => new()
        {
            AoeCount = 2,
        };
        setting.UnlockedByQuestID = 66587;
    }

    static partial void ModifyVengeancePvE(ref ActionSetting setting)
    {
        setting.StatusProvide = StatusHelper.RampartStatus;
        setting.ActionCheck = Player.IsTargetOnSelf;
    }

    static partial void ModifyRawIntuitionPvE(ref ActionSetting setting)
    {
        setting.ActionCheck = Player.IsTargetOnSelf;
        setting.UnlockedByQuestID = 66132;
    }

    static partial void ModifyHolmgangPvE(ref ActionSetting setting)
    {
        setting.TargetType = TargetType.Self;
    }

    static partial void ModifyFellCleavePvP(ref ActionSetting setting)
    {
        setting.StatusNeed = [StatusID.InnerRelease_1303];
    }

    static partial void ModifyChaoticCyclonePvP(ref ActionSetting setting)
    {
        setting.StatusNeed = [StatusID.NascentChaos_1992];
    }

    static partial void ModifyOnslaughtPvE(ref ActionSetting setting)
    {
        setting.SpecialType = SpecialActionType.MovingForward;
    }

    static partial void ModifyOnslaughtPvP(ref ActionSetting setting)
    {
        setting.SpecialType = SpecialActionType.MovingForward;
    }

    static partial void ModifyNascentFlashPvE(ref ActionSetting setting)
    {
        setting.StatusProvide = [StatusID.NascentFlash];
        setting.StatusProvide = [StatusID.NascentGlint];
    }

    static partial void ModifyThrillOfBattlePvE(ref ActionSetting setting)
    {
        setting.UnlockedByQuestID = 65855;
    }

    static partial void ModifyFellCleavePvE(ref ActionSetting setting)
    {
        setting.UnlockedByQuestID = 66124;
    }

    static partial void ModifyEquilibriumPvE(ref ActionSetting setting)
    {
        setting.UnlockedByQuestID = 66134;
    }

    static partial void ModifyDecimatePvE(ref ActionSetting setting)
    {
        setting.UnlockedByQuestID = 66137;
    }

    /// <inheritdoc/>
    protected override bool EmergencyAbility(IAction nextGCD, out IAction? act)
    {
        if (HolmgangPvE.CanUse(out act)
            && Player.GetHealthRatio() <= Service.Config.HealthForDyingTanks) return true;
        return base.EmergencyAbility(nextGCD, out act);
    }

    /// <inheritdoc/>
    [RotationDesc(ActionID.OnslaughtPvE)]
    protected override bool MoveForwardAbility(IAction nextGCD, out IAction? act)
    {
        if (OnslaughtPvE.CanUse(out act)) return true;
        return base.MoveForwardAbility(nextGCD, out act);
    }

    /// <inheritdoc/>
    [RotationDesc(ActionID.PrimalRendPvE)]
    protected override bool MoveForwardGCD(out IAction? act)
    {
        if (PrimalRendPvE.CanUse(out act, skipAoeCheck: true)) return true;
        return base.MoveForwardGCD(out act);
    }
}
