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
    public static float OverheatTime => OverheatTimeRemainingRaw - DataCenter.WeaponRemain;

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
    }

    static partial void ModifyAutoCrossbowPvE(ref ActionSetting setting)
    {
        setting.ActionCheck = () => IsOverheated && !OverheatedEndAfterGCD();
        setting.CreateConfig = () => new()
        {
            AoeCount = 2,
        };
    }

    static partial void ModifyRookAutoturretPvE(ref ActionSetting setting)
    {
        setting.ActionCheck = () => Battery >= 50 && !JobGauge.IsRobotActive;
        setting.CreateConfig = () => new()
        {
            TimeToKill = 10,
        };
    }

    static partial void ModifyReassemblePvE(ref ActionSetting setting)
    {
        setting.StatusProvide = [StatusID.Reassembled];
        setting.ActionCheck = () => HasHostilesInRange;
    }

    static partial void ModifyChainSawPvE(ref ActionSetting setting)
    {
        setting.CreateConfig = () => new()
        {
            AoeCount = 1,
        };
    }

    static partial void ModifyHyperchargePvE(ref ActionSetting setting)
    {
        setting.ActionCheck = () => !IsOverheated && Heat >= 50;
        setting.CreateConfig = () => new()
        {
            TimeToKill = 10,
        };
    }

    static partial void ModifyRicochetPvE(ref ActionSetting setting)
    {
        setting.CreateConfig = () => new()
        {
            AoeCount = 1,
            SingleTargetAoEOverride = true,
        };
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
        setting.ActionCheck = () => Heat <= 50 && InCombat;
    }

    static partial void ModifyTacticianPvE(ref ActionSetting setting)
    {
        setting.StatusProvide = StatusHelper.RangePhysicalDefense;
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
