namespace RotationSolver.Basic.Rotations.Basic;

partial class DragoonRotation
{
    /// <inheritdoc/>
    public override MedicineType MedicineType => MedicineType.Strength;

    /// <summary>
    /// 
    /// </summary>
    public static byte EyeCount => JobGauge.EyeCount;

    /// <summary>
    /// Firstminds Count
    /// </summary>
    public static byte FocusCount => JobGauge.FirstmindsFocusCount;

    /// <summary>
    /// 
    /// </summary>
    static float LOTDTimeRaw => JobGauge.LOTDTimer / 1000f;

    /// <summary>
    /// 
    /// </summary>
    public static float LOTDTime => LOTDTimeRaw - DataCenter.WeaponRemain;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    protected static bool LOTDEndAfter(float time) => LOTDTime <= time;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="gctCount"></param>
    /// <param name="offset"></param>
    /// <returns></returns>
    protected static bool LOTDEndAfterGCD(uint gctCount = 0, float offset = 0)
        => LOTDEndAfter(GCDTime(gctCount, offset));

    static partial void ModifyVorpalThrustPvE(ref ActionSetting setting)
    {
        setting.ComboIds = [ActionID.RaidenThrustPvE];
    }

    static partial void ModifyDisembowelPvE(ref ActionSetting setting)
    {
        setting.ComboIds = [ActionID.RaidenThrustPvE];
    }

    static partial void ModifyFangAndClawPvE(ref ActionSetting setting)
    {
        setting.StatusNeed = [StatusID.FangAndClawBared];
        setting.UnlockedByQuestID = 67229;
    }

    static partial void ModifyWheelingThrustPvE(ref ActionSetting setting)
    {
        setting.StatusNeed = [StatusID.WheelInMotion];
        setting.UnlockedByQuestID = 67230;
    }

    static partial void ModifyPiercingTalonPvE(ref ActionSetting setting)
    {
        setting.SpecialType = SpecialActionType.MeleeRange;
        setting.UnlockedByQuestID = 65591;
    }

    static partial void ModifyJumpPvE(ref ActionSetting setting)
    {
        setting.StatusProvide = [StatusID.DiveReady];
        setting.UnlockedByQuestID = 66603;
    }

    static partial void ModifyHighJumpPvE(ref ActionSetting setting)
    {
        setting.StatusProvide = [StatusID.DiveReady];
    }

    static partial void ModifyMirageDivePvE(ref ActionSetting setting)
    {
        setting.StatusNeed = [StatusID.DiveReady];
    }

    static partial void ModifySonicThrustPvE(ref ActionSetting setting)
    {
        setting.ComboIds = [ActionID.DraconianFuryPvE];
    }

    static partial void ModifyNastrondPvE(ref ActionSetting setting)
    {
        setting.ActionCheck = () => JobGauge.IsLOTDActive;
        setting.UnlockedByQuestID = 68450;
    }

    static partial void ModifyStardiverPvE(ref ActionSetting setting)
    {
        setting.ActionCheck = () => JobGauge.IsLOTDActive;
    }

    static partial void ModifyWyrmwindThrustPvE(ref ActionSetting setting)
    {
        setting.ActionCheck = () => FocusCount == 2;
    }

    static partial void ModifyLifeSurgePvE(ref ActionSetting setting)
    {
        setting.StatusProvide = [StatusID.LifeSurge];
        setting.ActionCheck = () => !IsLastAbility(ActionID.LifeSurgePvE);
    }

    static partial void ModifyLanceChargePvE(ref ActionSetting setting)
    {
        setting.CreateConfig = () => new()
        {
            TimeToKill = 10,
        };
        setting.UnlockedByQuestID = 65975;
    }

    static partial void ModifyDragonSightPvE(ref ActionSetting setting)
    {
        setting.TargetType = TargetType.Melee;
        setting.CanTarget = b => b != Player;
        setting.CreateConfig = () => new()
        {
            TimeToKill = 10,
        };
    }

    static partial void ModifyBattleLitanyPvE(ref ActionSetting setting)
    {
        setting.CreateConfig = () => new()
        {
            TimeToKill = 10,
        };
        setting.UnlockedByQuestID = 67226;
    }

    static partial void ModifyElusiveJumpPvE(ref ActionSetting setting)
    {
        setting.UnlockedByQuestID = 66604;
    }

    static partial void ModifyDoomSpikePvE(ref ActionSetting setting)
    {
        setting.UnlockedByQuestID = 66605;
    }

    static partial void ModifySpineshatterDivePvE(ref ActionSetting setting)
    {
        setting.UnlockedByQuestID = 66607;
    }

    static partial void ModifyDragonfireDivePvE(ref ActionSetting setting)
    {
        setting.UnlockedByQuestID = 66608;
    }

    static partial void ModifyGeirskogulPvE(ref ActionSetting setting)
    {
        setting.UnlockedByQuestID = 67231;
    }

    /// <inheritdoc/>
    [RotationDesc(ActionID.FeintPvE)]
    protected sealed override bool DefenseAreaAbility(IAction nextGCD, out IAction? act)
    {
        if (FeintPvE.CanUse(out act)) return true;
        return false;
    }

    /// <inheritdoc/>
    [RotationDesc(ActionID.ElusiveJumpPvE)]
    protected override bool MoveBackAbility(IAction nextGCD, out IAction? act)
    {
        if (ElusiveJumpPvE.CanUse(out act, skipClippingCheck: true)) return true;
        return base.MoveBackAbility(nextGCD, out act);
    }
}