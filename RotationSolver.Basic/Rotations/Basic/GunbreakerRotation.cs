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

    static partial void ModifyKeenEdgePvE(ref ActionSetting setting)
    {

    }

    static partial void ModifyNoMercyPvE(ref ActionSetting setting)
    {
        setting.StatusProvide = [StatusID.ReadyToBreak];
        setting.CreateConfig = () => new()
        {
            TimeToKill = 10,
        };
    }

    static partial void ModifyBrutalShellPvE(ref ActionSetting setting)
    {
        setting.ComboIds = [ActionID.KeenEdgePvE];
    }

    static partial void ModifyCamouflagePvE(ref ActionSetting setting)
    {
        setting.ActionCheck = () => Player.IsTargetOnSelf();
    }

    static partial void ModifyDemonSlicePvE(ref ActionSetting setting)
    {
        setting.CreateConfig = () => new()
        {
            AoeCount = 2,
        };
    }

    private protected sealed override IBaseAction TankStance => RoyalGuardPvE;

    static partial void ModifyLightningShotPvE(ref ActionSetting setting)
    {
        setting.SpecialType = SpecialActionType.MeleeRange;
    }

    static partial void ModifyDangerZonePvE(ref ActionSetting setting)
    {
        
    }

    static partial void ModifySolidBarrelPvE(ref ActionSetting setting)
    {
        setting.ComboIds = [ActionID.BrutalShellPvE];
    }

    static partial void ModifyBurstStrikePvE(ref ActionSetting setting)
    {
        setting.StatusProvide = [StatusID.ReadyToBlast, StatusID.ReadyToBlast_3041];
        setting.ActionCheck = () => Ammo > 0;
    }

    static partial void ModifyNebulaPvE(ref ActionSetting setting)
    {
        setting.StatusProvide = StatusHelper.RampartStatus;
        setting.ActionCheck = () => Player.IsTargetOnSelf();
    }

    static partial void ModifyDemonSlaughterPvE(ref ActionSetting setting)
    {
        setting.ComboIds = [ActionID.DemonSlicePvE];
        setting.CreateConfig = () => new()
        {
            AoeCount = 2,
        };
    }

    static partial void ModifyAuroraPvE(ref ActionSetting setting)
    {
        setting.TargetStatusProvide = [StatusID.Aurora];
    }

    static partial void ModifySuperbolidePvE(ref ActionSetting setting)
    {
        setting.StatusProvide = [StatusID.Superbolide];
        setting.ActionCheck = () => Player.IsTargetOnSelf();
    }

    static partial void ModifySonicBreakPvE(ref ActionSetting setting)
    {
        setting.StatusNeed = [StatusID.ReadyToBreak];
        setting.TargetStatusProvide = [StatusID.SonicBreak];
    }

    static partial void ModifyTrajectoryPvE(ref ActionSetting setting)
    {
        setting.SpecialType = SpecialActionType.MovingForward;
    }

    static partial void ModifyGnashingFangPvE(ref ActionSetting setting)
    {
        setting.ActionCheck = () => AmmoComboStep == 0 && Ammo > 0;
        setting.StatusProvide = [StatusID.ReadyToRip];
    }

    static partial void ModifySavageClawPvE(ref ActionSetting setting)
    {
        setting.ActionCheck = () => Service.GetAdjustedActionId(ActionID.GnashingFangPvE) == ActionID.SavageClawPvE;
        setting.ComboIds = [ActionID.GnashingFangPvE];
        setting.StatusProvide = [StatusID.ReadyToTear];
    }

    static partial void ModifyWickedTalonPvE(ref ActionSetting setting)
    {
        setting.ActionCheck = () => Service.GetAdjustedActionId(ActionID.GnashingFangPvE) == ActionID.WickedTalonPvE;
        setting.ComboIds = [ActionID.SavageClawPvE];
        setting.StatusProvide = [StatusID.ReadyToGouge];
    }

    static partial void ModifyBowShockPvE(ref ActionSetting setting)
    {
        setting.TargetStatusProvide = [StatusID.BowShock];
    }

    static partial void ModifyHeartOfLightPvE(ref ActionSetting setting)
    {
        setting.StatusProvide = [StatusID.HeartOfLight];
        setting.ActionCheck = () => Player.IsTargetOnSelf();
    }

    static partial void ModifyHeartOfStonePvE(ref ActionSetting setting)
    {
        setting.StatusProvide = [StatusID.HeartOfStone];
        setting.ActionCheck = () => Player.IsParty() || Player.IsTargetOnSelf();
    }

    static partial void ModifyContinuationPvE(ref ActionSetting setting)
    {
        setting.UnlockedByQuestID = 68802;
    }

    static partial void ModifyJugularRipPvE(ref ActionSetting setting)
    {
        setting.ActionCheck = () => Service.GetAdjustedActionId(ActionID.ContinuationPvE) == ActionID.JugularRipPvE;
        setting.ComboIds = [ActionID.GnashingFangPvE];
        setting.StatusNeed = [StatusID.ReadyToRip];
    }

    static partial void ModifyAbdomenTearPvE(ref ActionSetting setting)
    {
        setting.ActionCheck = () => Service.GetAdjustedActionId(ActionID.ContinuationPvE) == ActionID.AbdomenTearPvE;
        setting.ComboIds = [ActionID.SavageClawPvE];
        setting.StatusNeed = [StatusID.ReadyToTear];
    }

    static partial void ModifyEyeGougePvE(ref ActionSetting setting)
    {
        setting.ActionCheck = () => Service.GetAdjustedActionId(ActionID.ContinuationPvE) == ActionID.EyeGougePvE;
        setting.ComboIds = [ActionID.WickedTalonPvE];
        setting.StatusNeed = [StatusID.ReadyToGouge];
    }

    static partial void ModifyFatedCirclePvE(ref ActionSetting setting)
    {
        setting.StatusProvide = [StatusID.ReadyToRaze];
        setting.ActionCheck = () => Ammo > 0;
    }

    static partial void ModifyBloodfestPvE(ref ActionSetting setting)
    {
        setting.StatusProvide = [StatusID.ReadyToReign];
        setting.ActionCheck = () => MaxAmmo - Ammo > 1;
    }

    static partial void ModifyBlastingZonePvE(ref ActionSetting setting)
    {

    }

    static partial void ModifyHeartOfCorundumPvE(ref ActionSetting setting)
    {
        setting.StatusProvide = [StatusID.CatharsisOfCorundum, StatusID.ClarityOfCorundum];
        setting.ActionCheck = () => Player.IsParty() || Player.IsTargetOnSelf();
    }

    static partial void ModifyHypervelocityPvE(ref ActionSetting setting)
    {
        setting.ActionCheck = () => Service.GetAdjustedActionId(ActionID.ContinuationPvE) == ActionID.HypervelocityPvE;
        setting.ComboIds = [ActionID.BurstStrikePvE];
        setting.StatusNeed = [StatusID.ReadyToBlast];
    }

    static partial void ModifyDoubleDownPvE(ref ActionSetting setting)
    {
        setting.ActionCheck = () => Ammo > 1;
        setting.CreateConfig = () => new()
        {
            AoeCount = 1,
        };
    }

    static partial void ModifyGreatNebulaPvE(ref ActionSetting setting)
    {
        setting.StatusProvide = StatusHelper.RampartStatus;
        setting.ActionCheck = () => Player.IsTargetOnSelf();
    }

    static partial void ModifyFatedBrandPvE(ref ActionSetting setting)
    {
        setting.ComboIds = [ActionID.FatedCirclePvE];
        setting.StatusNeed = [StatusID.ReadyToRaze];
        setting.CreateConfig = () => new ActionConfig()
        {
            AoeCount = 2,
        };
    }

    static partial void ModifyReignOfBeastsPvE(ref ActionSetting setting)
    {
        setting.StatusNeed = [StatusID.ReadyToReign];
        setting.CreateConfig = () => new ActionConfig()
        {
            AoeCount = 1,
        };
    }

    static partial void ModifyNobleBloodPvE(ref ActionSetting setting)
    {
        setting.ActionCheck = () => Service.GetAdjustedActionId(ActionID.ReignOfBeastsPvE) == ActionID.NobleBloodPvE;
        setting.ComboIds = [ActionID.ReignOfBeastsPvE];
        setting.CreateConfig = () => new ActionConfig()
        {
            AoeCount = 1,
        };
    }

    static partial void ModifyLionHeartPvE(ref ActionSetting setting)
    {
        setting.ActionCheck = () => Service.GetAdjustedActionId(ActionID.ReignOfBeastsPvE) == ActionID.LionHeartPvE;
        setting.ComboIds = [ActionID.NobleBloodPvE];
        setting.CreateConfig = () => new ActionConfig()
        {
            AoeCount = 1,
        };
    }

    // PVP
    static partial void ModifyRoughDividePvP(ref ActionSetting setting)
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
    [RotationDesc(ActionID.TrajectoryPvE)]
    protected sealed override bool MoveForwardAbility(IAction nextGCD, out IAction? act)
    {
        if (TrajectoryPvE.CanUse(out act)) return true;
        return false;
    }
}
