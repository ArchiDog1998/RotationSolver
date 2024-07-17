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
    public static float LOTDTime => LOTDTimeRaw - DataCenter.DefaultGCDRemain;

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

    //Job

    static partial void ModifyVorpalThrustPvE(ref ActionSetting setting)
    {
        setting.ComboIds = [ActionID.TrueThrustPvE, ActionID.RaidenThrustPvE];
    }

    static partial void ModifyLifeSurgePvE(ref ActionSetting setting)
    {
        setting.StatusProvide = [StatusID.LifeSurge];
        setting.ActionCheck = () => !IsLastAbility(ActionID.LifeSurgePvE);
    }

    static partial void ModifyPiercingTalonPvE(ref ActionSetting setting)
    {
        setting.SpecialType = SpecialActionType.MeleeRange;
        setting.UnlockedByQuestID = 65591;
    }

    static partial void ModifyDisembowelPvE(ref ActionSetting setting)
    {
        setting.ComboIds = [ActionID.TrueThrustPvE, ActionID.RaidenThrustPvE];
        setting.StatusProvide = [StatusID.PowerSurge];
    }

    static partial void ModifyFullThrustPvE(ref ActionSetting setting)
    {
        setting.ComboIds = [ActionID.VorpalThrustPvE];
    }

    static partial void ModifyLanceChargePvE(ref ActionSetting setting)
    {
        setting.CreateConfig = () => new()
        {
            TimeToKill = 10,
        };
        setting.StatusProvide = [StatusID.LanceCharge];
        setting.UnlockedByQuestID = 65975;
    }

    static partial void ModifyJumpPvE(ref ActionSetting setting)
    {
        setting.StatusProvide = [StatusID.DiveReady];
        setting.UnlockedByQuestID = 66603;
    }

    static partial void ModifyElusiveJumpPvE(ref ActionSetting setting)
    {
        setting.UnlockedByQuestID = 66604;
    }

    static partial void ModifyDoomSpikePvE(ref ActionSetting setting)
    {
        setting.UnlockedByQuestID = 66605;
        setting.CreateConfig = () => new()
        {
            AoeCount = 2,
        };
    }

    static partial void ModifyWingedGlidePvE(ref ActionSetting setting)
    {
        setting.SpecialType = SpecialActionType.MovingForward;
        setting.UnlockedByQuestID = 66607;
    }

    static partial void ModifyChaosThrustPvE(ref ActionSetting setting)
    {
        setting.ComboIds = [ActionID.DisembowelPvE];
        setting.TargetStatusProvide = [StatusID.ChaoticSpring];
    }

    //Class

    static partial void ModifyDragonfireDivePvE(ref ActionSetting setting)
    {
        setting.UnlockedByQuestID = 66608;
        setting.StatusProvide = [StatusID.DragonsFlight];
        setting.CreateConfig = () => new()
        {
            AoeCount = 1,
        };
    }

    static partial void ModifyBattleLitanyPvE(ref ActionSetting setting)
    {
        setting.CreateConfig = () => new()
        {
            TimeToKill = 10,
        };
        setting.UnlockedByQuestID = 67226;
        setting.StatusProvide = [StatusID.BattleLitany];
    }

    static partial void ModifyFangAndClawPvE(ref ActionSetting setting)
    {
        setting.ComboIds = [ActionID.HeavensThrustPvE];
        setting.UnlockedByQuestID = 67229;
    }


    static partial void ModifyWheelingThrustPvE(ref ActionSetting setting)
    {
        setting.ComboIds = [ActionID.ChaoticSpringPvE];
        setting.UnlockedByQuestID = 67230;
    }

    static partial void ModifyGeirskogulPvE(ref ActionSetting setting)
    {
        setting.UnlockedByQuestID = 67231;
        setting.StatusProvide = [StatusID.NastrondReady, StatusID.LifeOfTheDragon];
        setting.CreateConfig = () => new()
        {
            AoeCount = 1,
        };
    }

    static partial void ModifySonicThrustPvE(ref ActionSetting setting)
    {
        setting.ComboIds = [ActionID.DraconianFuryPvE, ActionID.DoomSpikePvE];
        setting.StatusProvide = [StatusID.PowerSurge];
        setting.CreateConfig = () => new()
        {
            AoeCount = 2,
        };
    }

    static partial void ModifyDrakesbanePvE(ref ActionSetting setting) //aka Kendrick Lamar
    {
        setting.ComboIds = [ActionID.WheelingThrustPvE, ActionID.FangAndClawPvE];
        setting.StatusProvide = [StatusID.DraconianFire];
    }

    static partial void ModifyMirageDivePvE(ref ActionSetting setting)
    {
        setting.StatusNeed = [StatusID.DiveReady];
    }

    static partial void ModifyNastrondPvE(ref ActionSetting setting)
    {
        setting.StatusNeed = [StatusID.NastrondReady];
        setting.UnlockedByQuestID = 68450;
        setting.CreateConfig = () => new()
        {
            AoeCount = 1,
        };
    }

    static partial void ModifyCoerthanTormentPvE(ref ActionSetting setting)
    {
        setting.StatusProvide = [StatusID.DraconianFire];
        setting.ComboIds = [ActionID.SonicThrustPvE];
        setting.CreateConfig = () => new()
        {
            AoeCount = 2,
        };
    }

    static partial void ModifyHighJumpPvE(ref ActionSetting setting)
    {
        setting.StatusProvide = [StatusID.DiveReady];
    }

    static partial void ModifyRaidenThrustPvE(ref ActionSetting setting)
    {
        setting.StatusNeed = [StatusID.DraconianFire];
    }

    static partial void ModifyStardiverPvE(ref ActionSetting setting)
    {
        setting.ActionCheck = () => JobGauge.IsLOTDActive;
        setting.StatusProvide = [StatusID.StarcrossReady];
        setting.CreateConfig = () => new()
        {
            AoeCount = 1,
        };
    }

    static partial void ModifyDraconianFuryPvE(ref ActionSetting setting)
    {
        setting.StatusNeed = [StatusID.DraconianFire];
        setting.CreateConfig = () => new()
        {
            AoeCount = 2,
        };
    }

    static partial void ModifyHeavensThrustPvE(ref ActionSetting setting)
    {
        setting.ComboIds = [ActionID.LanceBarragePvE];
    }

    static partial void ModifyChaoticSpringPvE(ref ActionSetting setting)
    {
        setting.ComboIds = [ActionID.SpiralBlowPvE];
        setting.TargetStatusProvide = [StatusID.ChaoticSpring];
    }

    static partial void ModifyWyrmwindThrustPvE(ref ActionSetting setting)
    {
        setting.ActionCheck = () => FocusCount == 2;
        setting.CreateConfig = () => new()
        {
            AoeCount = 1,
        };
    }

    static partial void ModifyRiseOfTheDragonPvE(ref ActionSetting setting)
    {
        setting.UnlockedByQuestID = 66608;
        setting.StatusNeed = [StatusID.DragonsFlight];
        setting.CreateConfig = () => new()
        {
            AoeCount = 1,
        };
    }

    static partial void ModifyLanceBarragePvE(ref ActionSetting setting)
    {
        setting.ComboIds = [ActionID.TrueThrustPvE, ActionID.RaidenThrustPvE];
    }

    static partial void ModifySpiralBlowPvE(ref ActionSetting setting)
    {
        setting.ComboIds = [ActionID.TrueThrustPvE, ActionID.RaidenThrustPvE];
        setting.StatusProvide = [StatusID.PowerSurge];
    }

    static partial void ModifyStarcrossPvE(ref ActionSetting setting)
    {
        setting.StatusNeed = [StatusID.StarcrossReady];
        setting.CreateConfig = () => new()
        {
            AoeCount = 1,
        };
    }

    // PvP

    static partial void ModifyHighJumpPvP(ref ActionSetting setting)
    {
        setting.SpecialType = SpecialActionType.MovingForward;
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
        if (ElusiveJumpPvE.CanUse(out act)) return true;
        return base.MoveBackAbility(nextGCD, out act);
    }
}