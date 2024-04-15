using static Dalamud.Interface.Utility.Raii.ImRaii;

namespace RotationSolver.Basic.Rotations.Basic;

partial class DarkKnightRotation
{
    /// <inheritdoc/>
    public override MedicineType MedicineType => MedicineType.Strength;

    #region Job Gauge

    /// <summary>
    /// 
    /// </summary>
    public static byte Blood => JobGauge.Blood;

    /// <summary>
    /// 
    /// </summary>
    public static bool HasDarkArts => JobGauge.HasDarkArts;

    static float DarkSideTimeRemainingRaw => JobGauge.DarksideTimeRemaining / 1000f;

    /// <summary>
    /// 
    /// </summary>
    public static float DarkSideTime => DarkSideTimeRemainingRaw - DataCenter.WeaponRemain;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    protected static bool DarkSideEndAfter(float time) => DarkSideTime <= time;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="gctCount"></param>
    /// <param name="offset"></param>
    /// <returns></returns>
    protected static bool DarkSideEndAfterGCD(uint gctCount = 0, float offset = 0)
        => DarkSideEndAfter(GCDTime(gctCount, offset));

    static float ShadowTimeRemainingRaw => JobGauge.ShadowTimeRemaining / 1000f;

    /// <summary>
    /// 
    /// </summary>
    public static float ShadowTime => ShadowTimeRemainingRaw - DataCenter.WeaponRemain;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    protected static bool ShadowTimeEndAfter(float time) => ShadowTimeRemainingRaw <= time;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="gctCount"></param>
    /// <param name="offset"></param>
    /// <returns></returns>
    protected static bool ShadowTimeEndAfterGCD(uint gctCount = 0, float offset = 0)
        => ShadowTimeEndAfter(GCDTime(gctCount, offset));
    #endregion

    static partial void ModifyBloodspillerPvE(ref ActionSetting setting)
    {
        setting.ActionCheck = () => Blood >= 50 || !Player.WillStatusEnd(0, true, StatusID.Delirium_1972);
    }

    static partial void ModifyUnmendPvE(ref ActionSetting setting)
    {
        setting.SpecialType =  SpecialActionType.MeleeRange;
    }

    static partial void ModifyLivingShadowPvE(ref ActionSetting setting)
    {
        setting.ActionCheck = () => Blood >= 50;
    }

    static partial void ModifyQuietusPvE(ref ActionSetting setting)
    {
        setting.ActionCheck = () => Blood >= 50 || !Player.WillStatusEnd(0, true, StatusID.Delirium_1972);
    }

    static partial void ModifyStalwartSoulPvE(ref ActionSetting setting)
    {
        setting.ComboIds = [ActionID.UnleashPvE];
        setting.CreateConfig = () => new()
        {
            AoeCount = 2,
        };
    }

    static partial void ModifySaltAndDarknessPvE(ref ActionSetting setting)
    {
        setting.ActionCheck = () => Service.GetAdjustedActionId(ActionID.SaltedEarthPvE) == ActionID.SaltAndDarknessPvE;
    }

    static partial void ModifyShadowbringerPvE(ref ActionSetting setting)
    {
        setting.ActionCheck = () => !DarkSideEndAfterGCD();
    }

    private protected sealed override IBaseAction TankStance => GritPvE;

    static partial void ModifyShadowWallPvE(ref ActionSetting setting)
    {
        setting.StatusProvide = StatusHelper.RampartStatus;
        setting.ActionCheck = Player.IsTargetOnSelf;
    }

    static partial void ModifyDarkMindPvE(ref ActionSetting setting)
    {
        setting.ActionCheck = Player.IsTargetOnSelf;
    }

    static partial void ModifyOblationPvE(ref ActionSetting setting)
    {
        setting.TargetStatusProvide = [StatusID.Oblation];
    }

    static partial void ModifyBloodWeaponPvE(ref ActionSetting setting)
    {
        setting.CreateConfig = () => new ()
        {
            TimeToKill = 10,
        };
        setting.UnlockedByQuestID = 67591;
    }

    static partial void ModifyDeliriumPvE(ref ActionSetting setting)
    {
        setting.CreateConfig = () => new()
        {
            TimeToKill = 10,
        };
    }

    static partial void ModifyUnleashPvE(ref ActionSetting setting)
    {
        setting.CreateConfig = () => new()
        {
            AoeCount = 2,
        };
    }

    static partial void ModifyPlungePvE(ref ActionSetting setting)
    {
        setting.SpecialType = SpecialActionType.MovingForward;
        setting.UnlockedByQuestID = 67597;
    }

    static partial void ModifyFloodOfDarknessPvE(ref ActionSetting setting)
    {
        setting.UnlockedByQuestID = 67590;
    }

    static partial void ModifyEdgeOfDarknessPvE(ref ActionSetting setting)
    {
        setting.UnlockedByQuestID = 67592;
    }

    static partial void ModifyLivingDeadPvE(ref ActionSetting setting)
    {
        setting.UnlockedByQuestID = 67594;
    }

    static partial void ModifySaltedEarthPvE(ref ActionSetting setting)
    {
        setting.UnlockedByQuestID = 67596;
    }

    static partial void ModifyAbyssalDrainPvE(ref ActionSetting setting)
    {
        setting.UnlockedByQuestID = 67598;
    }

    static partial void ModifyCarveAndSpitPvE(ref ActionSetting setting)
    {
        setting.UnlockedByQuestID = 67600;
    }

    static partial void ModifyTheBlackestNightPvE(ref ActionSetting setting)
    {
        setting.UnlockedByQuestID = 68455;
    }

    /// <inheritdoc/>
    protected override bool EmergencyAbility(IAction nextGCD, out IAction? act)
    {
        if (LivingDeadPvE.CanUse(out act) 
            && Player.GetHealthRatio() <= Service.Config.HealthForDyingTanks) return true;
        return base.EmergencyAbility(nextGCD, out act);
    }

    /// <inheritdoc/>
    [RotationDesc(ActionID.PlungePvE)]
    protected sealed override bool MoveForwardAbility(IAction nextGCD, out IAction? act)
    {
        if (PlungePvE.CanUse(out act)) return true;
        return false;
    }
}
