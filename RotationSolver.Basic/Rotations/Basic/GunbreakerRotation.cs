namespace RotationSolver.Basic.Rotations.Basic;

partial class GunbreakerRotation
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

    static partial void ModifyNobleBloodPvE(ref ActionSetting setting)
    {
        setting.ComboIds = [ActionID.ReignOfBeastsPvE];
    }

    static partial void ModifyLionHeartPvE(ref ActionSetting setting)
    {
        setting.ComboIds = [ActionID.NobleBloodPvE];
    }

    static partial void ModifyFatedCirclePvE(ref ActionSetting setting)
    {
        setting.StatusProvide = [ StatusID.ReadyToRaze ];
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

    static partial void ModifyGreatNebulaPvE(ref ActionSetting setting)
    {
        setting.StatusProvide = StatusHelper.RampartStatus;
        setting.ActionCheck = Player.IsTargetOnSelf;
    }

    static partial void ModifyCamouflagePvE(ref ActionSetting setting)
    {
        setting.ActionCheck = Player.IsTargetOnSelf;
    }

    static partial void ModifySonicBreakPvE(ref ActionSetting setting)
    {
        setting.StatusNeed = [ StatusID.ReadyToBreak ];
    }

    private protected sealed override IBaseAction TankStance => RoyalGuardPvE;

    static partial void ModifyNoMercyPvE(ref ActionSetting setting)
    {
        setting.StatusProvide = [ StatusID.ReadyToBreak ];
        setting.CreateConfig = () => new()
        {
            TimeToKill = 10,
        };
    }

    static partial void ModifyBloodfestPvE(ref ActionSetting setting)
    {
        setting.ActionCheck = () => MaxAmmo - Ammo > 1;
    }

    static partial void ModifyFatedBrandPvE(ref ActionSetting setting)
    {
        setting.StatusNeed = [StatusID.ReadyToRaze];
        setting.CreateConfig = () => new ActionConfig()
        {
            AoeCount = 2,
        };
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

    static partial void ModifyTrajectoryPvE(ref ActionSetting setting)
    {
        setting.SpecialType = SpecialActionType.MovingForward;
    }

    static partial void ModifyRoughDividePvP(ref ActionSetting setting)
    {
        setting.SpecialType = SpecialActionType.MovingForward;
    }

    static partial void ModifyContinuationPvE(ref ActionSetting setting)
    {
        setting.UnlockedByQuestID = 68802;
    }

    /// <inheritdoc/>
    protected override bool EmergencyAbility(IAction nextGCD, out IAction? act)
    {
        if (SuperbolidePvE.CanUse(out act)
            && Player.GetHealthRatio() <= Service.Config.HealthForDyingTanks) return true;
        return base.EmergencyAbility(nextGCD, out act);
    }

    /// <inheritdoc/>
    [RotationDesc(ActionID.TrajectoryPvE)]
    protected sealed override bool MoveForwardAbility(IAction nextGCD, out IAction? act)
    {
        if (TrajectoryPvE.CanUse(out act)) return true;
        return false;
    }
}