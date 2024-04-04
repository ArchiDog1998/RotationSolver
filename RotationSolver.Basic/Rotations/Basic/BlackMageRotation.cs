﻿namespace RotationSolver.Basic.Rotations.Basic;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
partial class BlackMageRotation
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
{
    /// <inheritdoc/>
    public override MedicineType MedicineType => MedicineType.Intelligence;

    #region Job Gauge
    /// <summary>
    /// 
    /// </summary>
    public static byte UmbralIceStacks => JobGauge.UmbralIceStacks;

    /// <summary>
    /// 
    /// </summary>
    public static byte AstralFireStacks => JobGauge.AstralFireStacks;

    /// <summary>
    /// 
    /// </summary>
    public static byte PolyglotStacks => JobGauge.PolyglotStacks;

    /// <summary>
    /// 
    /// </summary>
    public static byte UmbralHearts => JobGauge.UmbralHearts;

    /// <summary>
    /// 
    /// </summary>
    public static bool IsParadoxActive => JobGauge.IsParadoxActive;

    /// <summary>
    /// 
    /// </summary>
    public static bool InUmbralIce => JobGauge.InUmbralIce;

    /// <summary>
    /// 
    /// </summary>
    public static bool InAstralFire => JobGauge.InAstralFire;

    /// <summary>
    /// 
    /// </summary>
    public static bool IsEnochianActive => JobGauge.IsEnochianActive;

    /// <summary>
    /// 
    /// </summary>
    public static bool IsPolyglotStacksMaxed => EnhancedPolyglotTrait.EnoughLevel ? PolyglotStacks == 2 : PolyglotStacks == 1;

    static float EnochianTimeRaw => JobGauge.EnochianTimer / 1000f;

    /// <summary>
    /// 
    /// </summary>
    public static float EnochianTime => EnochianTimeRaw - DataCenter.WeaponRemain;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    protected static bool EnchinaEndAfter(float time) => EnochianTime <= time;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="gcdCount"></param>
    /// <param name="offset"></param>
    /// <returns></returns>
    protected static bool EnchinaEndAfterGCD(uint gcdCount = 0, float offset = 0)
        => EnchinaEndAfter(GCDTime(gcdCount, offset));

    static float ElementTimeRaw => JobGauge.ElementTimeRemaining / 1000f;

    /// <summary>
    /// 
    /// </summary>
    protected static float ElementTime => ElementTimeRaw - DataCenter.WeaponRemain;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    protected static bool ElementTimeEndAfter(float time) => ElementTime <= time;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="gctCount"></param>
    /// <param name="offset"></param>
    /// <returns></returns>
    protected static bool ElementTimeEndAfterGCD(uint gctCount = 0, float offset = 0)
        => ElementTimeEndAfter(GCDTime(gctCount, offset));
    #endregion


    /// <summary>
    /// 
    /// </summary>
    protected static bool HasFire => Player.HasStatus(true, StatusID.Firestarter);

    /// <summary>
    /// 
    /// </summary>
    protected static bool HasThunder => Player.HasStatus(true, StatusID.Thundercloud);

    static partial void ModifyThunderPvE(ref ActionSetting setting)
    {
        setting.MPOverride = () => HasThunder ? 0 : null;
    }

    static partial void ModifyThunderIiPvE(ref ActionSetting setting)
    {
        setting.MPOverride = () => HasThunder ? 0 : null;
    }

    static partial void ModifyThunderIiiPvE(ref ActionSetting setting)
    {
        setting.MPOverride = () => HasThunder ? 0 : null;
    }

    static partial void ModifyThunderIvPvE(ref ActionSetting setting)
    {
        setting.MPOverride = () => HasThunder ? 0 : null;
    }

    static partial void ModifyFireIiiPvE(ref ActionSetting setting)
    {
        setting.MPOverride = () => HasFire ? 0 : null;
        setting.ActionCheck = () => !IsLastGCD(ActionID.FireIiiPvE);
    }

    static partial void ModifyFireIvPvE(ref ActionSetting setting)
    {
        setting.ActionCheck = () => InAstralFire && !ElementTimeEndAfter(ActionID.FireIvPvE.GetCastTime() - 0.1f);
    }

    static partial void ModifyDespairPvE(ref ActionSetting setting)
    {
        setting.ActionCheck = () => InAstralFire && !ElementTimeEndAfter(ActionID.DespairPvE.GetCastTime() - 0.1f);
    }

    static partial void ModifyBlizzardIiiPvE(ref ActionSetting setting)
    {
        setting.ActionCheck = () => !IsLastGCD(ActionID.BlizzardIvPvE);
    }

    static partial void ModifyBlizzardIvPvE(ref ActionSetting setting)
    {
        setting.ActionCheck = () => InUmbralIce && !ElementTimeEndAfter(ActionID.BlizzardIvPvE.GetCastTime() - 0.1f);
    }

    static partial void ModifyXenoglossyPvE(ref ActionSetting setting)
    {
        setting.ActionCheck = () => PolyglotStacks > 0;
    }

    static partial void ModifyParadoxPvE(ref ActionSetting setting)
    {
        setting.ActionCheck = () => IsParadoxActive;
    }

    static partial void ModifyFlarePvE(ref ActionSetting setting)
    {
        setting.ActionCheck = () => InAstralFire && !ElementTimeEndAfter(ActionID.FlarePvE.GetCastTime() - 0.1f);
    }

    static partial void ModifyFreezePvE(ref ActionSetting setting)
    {
        setting.ActionCheck = () => InUmbralIce && !ElementTimeEndAfter(ActionID.FreezePvE.GetCastTime() - 0.1f);
    }

    static partial void ModifyFoulPvE(ref ActionSetting setting)
    {
        setting.ActionCheck = () => PolyglotStacks > 0;
    }

    static partial void ModifyAmplifierPvE(ref ActionSetting setting)
    {
        setting.ActionCheck = () => !EnchinaEndAfter(10) && PolyglotStacks < 2;
    }


    static partial void ModifyManafontPvE(ref ActionSetting setting)
    {
        setting.ActionCheck = () => DataCenter.CurrentMp <= 7000;
    }

    static partial void ModifyLeyLinesPvE(ref ActionSetting setting)
    {
        setting.StatusProvide = [StatusID.LeyLines];
        setting.CreateConfig = () => new()
        {
            TimeToKill = 15,
        };
    }

    static partial void ModifyBetweenTheLinesPvE(ref ActionSetting setting)
    {
        setting.StatusNeed = [StatusID.LeyLines];
    }

    static partial void ModifySharpcastPvE(ref ActionSetting setting)
    {
        setting.ActionCheck = () => HasHostilesInRange;
        setting.StatusProvide = [StatusID.Sharpcast];
    }

    static partial void ModifyTriplecastPvE(ref ActionSetting setting)
    {
        setting.StatusProvide = StatusHelper.SwiftcastStatus;
    }

    static partial void ModifyTransposePvE(ref ActionSetting setting)
    {
        setting.ActionCheck = () => DataCenter.AnimationLocktime <= ElementTimeRaw;
    }

    static partial void ModifyUmbralSoulPvE(ref ActionSetting setting)
    {
        setting.ActionCheck = () => JobGauge.InUmbralIce && DataCenter.AnimationLocktime <= ElementTimeRaw;
    }

    /// <summary>
    /// 
    /// </summary>
    protected static float Fire4Time { get; private set; }

    /// <summary>
    /// 
    /// </summary>
    protected override void UpdateInfo()
    {
        if (Player.CastActionId == (uint)ActionID.FireIvPvE && Player.CurrentCastTime < 0.2)
        {
            Fire4Time = Player.TotalCastTime;
        }
        base.UpdateInfo();
    }

    /// <inheritdoc/>
    [RotationDesc(ActionID.ManawardPvE)]
    protected sealed override bool DefenseSingleGCD(out IAction? act)
    {
        if (ManawardPvE.CanUse(out act)) return true;
        return base.DefenseSingleGCD(out act);
    }

    /// <inheritdoc/>
    [RotationDesc(ActionID.AddlePvE)]
    protected override bool DefenseAreaAbility(out IAction act)
    {
        if (AddlePvE.CanUse(out act)) return true;
        return false;
    }

    /// <inheritdoc/>
    [RotationDesc(ActionID.AetherialManipulationPvE)]
    protected sealed override bool MoveForwardGCD(out IAction? act)
    {
        if (AetherialManipulationPvE.CanUse(out act)) return true;
        return base.MoveForwardGCD(out act);
    }
}
