﻿namespace RotationSolver.Basic.Rotations.Basic;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
partial class GunbreakerRotation
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
{
    /// <inheritdoc/>
    public override MedicineType MedicineType => MedicineType.Strength;


    /// <summary>
    /// 
    /// </summary>
    public override bool CanHealSingleSpell => false;

    /// <summary>
    /// 
    /// </summary>
    public override bool CanHealAreaSpell => false;

    #region Job Gauge
    /// <summary>
    /// 
    /// </summary>
    public static byte Ammo => JobGauge.Ammo;

    /// <summary>
    /// 
    /// </summary>
    public static byte AmmoComboStep => JobGauge.AmmoComboStep;

    /// <summary>
    /// 
    /// </summary>
    public static byte MaxAmmo => CartridgeChargeIiTrait.EnoughLevel ? (byte)3 : (byte)2;
    #endregion

    static partial void ModifyBurstStrikePvE(ref ActionSetting setting)
    {
        setting.ActionCheck = () => Ammo > 0;
    }

    static partial void ModifyGnashingFangPvE(ref ActionSetting setting)
    {
        setting.ActionCheck = () => AmmoComboStep == 0 && Ammo > 0;
    }

    static partial void ModifySavageClawPvE(ref ActionSetting setting)
    {
        setting.ActionCheck = () => Service.GetAdjustedActionId(ActionID.GnashingFangPvE) == ActionID.SavageClawPvE;
    }

    static partial void ModifyWickedTalonPvE(ref ActionSetting setting)
    {
        setting.ActionCheck = () => Service.GetAdjustedActionId(ActionID.GnashingFangPvE) == ActionID.WickedTalonPvE;
    }

    static partial void ModifyJugularRipPvE(ref ActionSetting setting)
    {
        setting.ActionCheck = () => Service.GetAdjustedActionId(ActionID.ContinuationPvE) == ActionID.JugularRipPvE;
    }

    static partial void ModifyAbdomenTearPvE(ref ActionSetting setting)
    {
        setting.ActionCheck = () => Service.GetAdjustedActionId(ActionID.ContinuationPvE) == ActionID.AbdomenTearPvE;
    }

    static partial void ModifyEyeGougePvE(ref ActionSetting setting)
    {
        setting.ActionCheck = () => Service.GetAdjustedActionId(ActionID.ContinuationPvE) == ActionID.EyeGougePvE;
    }

    static partial void ModifyHypervelocityPvE(ref ActionSetting setting)
    {
        setting.ActionCheck = () => Service.GetAdjustedActionId(ActionID.ContinuationPvE) == ActionID.HypervelocityPvE;
    }

    static partial void ModifyLightningShotPvE(ref ActionSetting setting)
    {
        setting.SpecialType = SpecialActionType.MeleeRange;
    }

    static partial void ModifyFatedCirclePvE(ref ActionSetting setting)
    {
        setting.ActionCheck = () => Ammo > 0;
    }

    static partial void ModifyDoubleDownPvE(ref ActionSetting setting)
    {
        setting.ActionCheck = () => Ammo > 1;
    }

    static partial void ModifyAuroraPvE(ref ActionSetting setting)
    {
        setting.TargetStatusProvide = [StatusID.Aurora];
    }

    static partial void ModifyNebulaPvE(ref ActionSetting setting)
    {
        setting.StatusProvide = StatusHelper.RampartStatus;
        setting.ActionCheck = Player.IsTargetOnSelf;
    }

    static partial void ModifyCamouflagePvE(ref ActionSetting setting)
    {
        setting.ActionCheck = Player.IsTargetOnSelf;
    }

    private protected sealed override IBaseAction TankStance => RoyalGuardPvE;

    static partial void ModifyNoMercyPvE(ref ActionSetting setting)
    {
        setting.CreateConfig = () => new()
        {
            TimeToKill = 10,
        };
    }

    static partial void ModifyBloodfestPvE(ref ActionSetting setting)
    {
        setting.ActionCheck = () => MaxAmmo - Ammo > 1;
    }

    static partial void ModifyDemonSlicePvE(ref ActionSetting setting)
    {
        setting.CreateConfig = () => new()
        {
            AoeCount = 2,
        };
    }

    static partial void ModifyDemonSlaughterPvE(ref ActionSetting setting)
    {
        setting.CreateConfig = () => new()
        {
            AoeCount = 2,
        };
    }

    static partial void ModifyRoughDividePvE(ref ActionSetting setting)
    {
        setting.SpecialType = SpecialActionType.MovingForward;
    }

    /// <inheritdoc/>
    protected override bool EmergencyAbility(IAction nextGCD, out IAction? act)
    {
        if (SuperbolidePvE.CanUse(out act)
            && Player.GetHealthRatio() <= Service.Config.HealthForDyingTanks) return true;
        return base.EmergencyAbility(nextGCD, out act);
    }

    /// <inheritdoc/>
    [RotationDesc(ActionID.RoughDividePvE)]
    protected sealed override bool MoveForwardAbility(out IAction? act)
    {
        if (RoughDividePvE.CanUse(out act)) return true;
        return false;
    }
}