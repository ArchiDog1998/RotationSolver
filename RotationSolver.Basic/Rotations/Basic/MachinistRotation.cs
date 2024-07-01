namespace RotationSolver.Basic.Rotations.Basic;

partial class MachinistRotation
{
    /// <inheritdoc/>
    public override MedicineType MedicineType => MedicineType.Dexterity;

    #region Job Gauge
    /// <summary>
    /// 
    /// </summary>
    public static bool IsOverheated => JobGauge.IsOverheated;

    /// <summary>
    /// 
    /// </summary>
    public static byte Heat => JobGauge.Heat;

    /// <summary>
    /// 
    /// </summary>
    public static byte Battery => JobGauge.Battery;

    /// <summary>
    /// 
    /// </summary>
    public static byte LastSummonBatteryPower => JobGauge.LastSummonBatteryPower;

    static float OverheatTimeRemainingRaw => JobGauge.OverheatTimeRemaining / 1000f;

    /// <summary>
    /// 
    /// </summary>
    public static float OverheatTime => OverheatTimeRemainingRaw - DataCenter.DefaultGCDRemain;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    protected static bool OverheatedEndAfter(float time) => OverheatTime <= time;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="gctCount"></param>
    /// <param name="offset"></param>
    /// <returns></returns>
    protected static bool OverheatedEndAfterGCD(uint gctCount = 0, float offset = 0)
        => OverheatedEndAfter(GCDTime(gctCount, offset));
    #endregion

    static partial void ModifySlugShotPvE(ref ActionSetting setting)
    {
        setting.ComboIds = [ActionID.HeatedSplitShotPvE];
    }

    static partial void ModifyCleanShotPvE(ref ActionSetting setting)
    {
        setting.ComboIds = [ActionID.HeatedSlugShotPvE];
    }

    static partial void ModifyHeatBlastPvE(ref ActionSetting setting)
    {
        setting.ActionCheck = () => IsOverheated && !OverheatedEndAfterGCD();
        setting.UnlockedByQuestID = 67234;
    }

    static partial void ModifyAutoCrossbowPvE(ref ActionSetting setting)
    {
        setting.ActionCheck = () => IsOverheated && !OverheatedEndAfterGCD();
        setting.CreateConfig = () => new()
        {
            AoeCount = 2,
        };
        setting.UnlockedByQuestID = 67242;
    }

    static partial void ModifyRookAutoturretPvE(ref ActionSetting setting)
    {
        setting.ActionCheck = () => Battery >= 50 && !JobGauge.IsRobotActive;
        setting.CreateConfig = () => new()
        {
            TimeToKill = 10,
        };
        setting.UnlockedByQuestID = 67235;
    }

    static partial void ModifyReassemblePvE(ref ActionSetting setting)
    {
        setting.StatusProvide = [StatusID.Reassembled];
        setting.ActionCheck = () => HasHostilesInRange;
    }

    static partial void ModifyChainSawPvE(ref ActionSetting setting)
    {
        setting.StatusProvide = [StatusID.ExcavatorReady];
        setting.CreateConfig = () => new()
        {
            AoeCount = 1,
        };
    }

    static partial void ModifyHyperchargePvE(ref ActionSetting setting)
    {
        setting.ActionCheck = () => !IsOverheated && (Heat >= 50 || Player.HasStatus(true, StatusID.Hypercharged));        
        setting.CreateConfig = () => new()
        {
            TimeToKill = 10,
        };
        setting.UnlockedByQuestID = 67233;
    }

    static partial void ModifyRicochetPvE(ref ActionSetting setting)
    {
        setting.CreateConfig = () => new()
        {
            AoeCount = 1,
        };
        setting.UnlockedByQuestID = 67240;
    }

    static partial void ModifyWildfirePvE(ref ActionSetting setting)
    {
        setting.CreateConfig = () => new()
        {
            TimeToKill = 10,
        };
    }

    static partial void ModifyQueenOverdrivePvE(ref ActionSetting setting)
    {
        setting.CreateConfig = () => new()
        {
            TimeToKill = 15,
        };
    }

    static partial void ModifySpreadShotPvE(ref ActionSetting setting)
    {
        setting.CreateConfig = () => new()
        {
            AoeCount = 2,
        };
    }

    static partial void ModifyBarrelStabilizerPvE(ref ActionSetting setting)
    {
        setting.StatusProvide = [StatusID.Hypercharged, StatusID.FullMetalMachinist];
        setting.ActionCheck = () => InCombat;
    }

    static partial void ModifyExcavatorPvE(ref ActionSetting setting)
    {
        setting.ActionCheck = () => Player.HasStatus(true, StatusID.ExcavatorReady);
        setting.CreateConfig = () => new ActionConfig()
        {
            AoeCount = 1,
        };
    }

    static partial void ModifyFullMetalFieldPvE(ref ActionSetting setting)
    {
        setting.ActionCheck = () => Player.HasStatus(true, StatusID.FullMetalMachinist);
        setting.CreateConfig = () => new ActionConfig()
        {
            AoeCount = 1,
        };
    }

    static partial void ModifyTacticianPvE(ref ActionSetting setting)
    {
        setting.StatusProvide = StatusHelper.RangePhysicalDefense;
        setting.UnlockedByQuestID = 67244;
    }

    static partial void ModifyHeatedSplitShotPvE(ref ActionSetting setting)
    {
        setting.UnlockedByQuestID = 67243;
    }

    static partial void ModifyHeatedSlugShotPvE(ref ActionSetting setting)
    {
        setting.UnlockedByQuestID = 67248;
    }

    static partial void ModifyFlamethrowerPvE(ref ActionSetting setting)
    {
        setting.UnlockedByQuestID = 68445;
    }

    static partial void ModifyDrillPvE(ref ActionSetting setting)
    {
        setting.UnlockedByQuestID = 67246;
    }

    /// <inheritdoc/>
    [RotationDesc(ActionID.TacticianPvE, ActionID.DismantlePvE)]
    protected sealed override bool DefenseAreaAbility(IAction nextGCD, out IAction act)
    {
        if (TacticianPvE.CanUse(out act, skipAoeCheck: true)) return true;
        if (DismantlePvE.CanUse(out act, skipAoeCheck: true)) return true;
        return false;
    }
}
