﻿namespace RotationSolver.Basic.Rotations.Basic;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
partial class RedMageRotation
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
{
    /// <inheritdoc/>
    public override MedicineType MedicineType => MedicineType.Intelligence;

    /// <inheritdoc/>
    public override bool CanHealSingleSpell => DataCenter.PartyMembers.Count() == 1 && base.CanHealSingleSpell;

    #region Job Gauge
    /// <summary>
    /// 
    /// </summary>
    public static byte WhiteMana => JobGauge.WhiteMana;

    /// <summary>
    /// 
    /// </summary>
    public static byte BlackMana => JobGauge.BlackMana;

    /// <summary>
    /// 
    /// </summary>
    public static byte ManaStacks => JobGauge.ManaStacks;

    /// <summary>
    /// Is <see cref="WhiteMana"/> larger than <see cref="BlackMana"/>
    /// </summary>
    public static bool IsWhiteManaLargerThanBlackMana => WhiteMana > BlackMana;
    #endregion

    private static readonly StatusID[] SwiftcastStatus = [.. StatusHelper.SwiftcastStatus, StatusID.Acceleration];
    static partial void ModifyJoltPvE(ref ActionSetting setting)
    {
        setting.StatusProvide = SwiftcastStatus;
    }

    static partial void ModifyVerfirePvE(ref ActionSetting setting)
    {
        setting.StatusNeed = [StatusID.VerfireReady];
        setting.StatusProvide = SwiftcastStatus;
    }

    static partial void ModifyVerstonePvE(ref ActionSetting setting)
    {
        setting.StatusNeed = [StatusID.VerstoneReady];
        setting.StatusProvide = SwiftcastStatus;
    }

    static partial void ModifyVerthunderPvE(ref ActionSetting setting)
    {
        setting.StatusNeed = SwiftcastStatus;
    }

    static partial void ModifyVeraeroPvE(ref ActionSetting setting)
    {
        setting.StatusNeed = SwiftcastStatus;
    }

    static partial void ModifyRipostePvE(ref ActionSetting setting)
    {
        setting.ActionCheck = () => BlackMana >= 20 && WhiteMana >= 20;
    }

    static partial void ModifyZwerchhauPvE(ref ActionSetting setting)
    {
        setting.ActionCheck = () => BlackMana >= 15 && WhiteMana >= 15;
    }

    static partial void ModifyRedoublementPvE(ref ActionSetting setting)
    {
        setting.ActionCheck = () => BlackMana >= 15 && WhiteMana >= 15;
    }

    static partial void ModifyScatterPvE(ref ActionSetting setting)
    {
        setting.StatusNeed = SwiftcastStatus;
        setting.CreateConfig = () => new()
        {
            AoeCount = 2,
        };
    }

    static partial void ModifyVerthunderIiPvE(ref ActionSetting setting)
    {
        setting.StatusProvide = SwiftcastStatus;
    }

    static partial void ModifyVeraeroIiPvE(ref ActionSetting setting)
    {
        setting.StatusProvide = SwiftcastStatus;
    }

    static partial void ModifyMoulinetPvE(ref ActionSetting setting)
    {
        setting.ActionCheck = () => BlackMana >= 20 && WhiteMana >= 20;
    }

    static partial void ModifyScorchPvE(ref ActionSetting setting)
    {
        setting.ComboIds = [ActionID.VerholyPvE];
    }

    private protected sealed override IBaseAction Raise => VerraisePvE;

    static partial void ModifyAccelerationPvE(ref ActionSetting setting)
    {
        setting.StatusProvide = [StatusID.Acceleration];
    }

    static partial void ModifyVercurePvE(ref ActionSetting setting)
    {
        setting.StatusProvide = SwiftcastStatus;
    }

    static partial void ModifyEmboldenPvE(ref ActionSetting setting)
    {
        setting.CreateConfig = () => new()
        {
            TimeToKill = 10,
        };
    }

    static partial void ModifyManaficationPvE(ref ActionSetting setting)
    {
        setting.ActionCheck = () => WhiteMana <= 50 && BlackMana <= 50 && InCombat && ManaStacks == 0;
        setting.ComboIdsNot = [ActionID.RipostePvE, ActionID.ZwerchhauPvE, ActionID.ScorchPvE, ActionID.VerflarePvE, ActionID.VerholyPvE];

        setting.CreateConfig = () => new()
        {
            TimeToKill = 10,
        };
    }

    static partial void ModifyCorpsacorpsPvE(ref ActionSetting setting)
    {
        setting.SpecialType = SpecialActionType.MovingForward;
    }

    /// <inheritdoc/>
    [RotationDesc(ActionID.VercurePvE)]
    protected sealed override bool HealSingleGCD(out IAction? act)
    {
        if (VercurePvE.CanUse(out act, skipStatusProvideCheck: true)) return true;
        return base.HealSingleGCD(out act);
    }

    /// <inheritdoc/>
    [RotationDesc(ActionID.CorpsacorpsPvE)]
    protected sealed override bool MoveForwardAbility(out IAction? act)
    {
        if (CorpsacorpsPvE.CanUse(out act)) return true;
        return base.MoveForwardAbility(out act);
    }

    /// <inheritdoc/>
    [RotationDesc(ActionID.AddlePvE, ActionID.MagickBarrierPvE)]
    protected sealed override bool DefenseAreaAbility(out IAction? act)
    {
        if (AddlePvE.CanUse(out act)) return true;
        if (MagickBarrierPvE.CanUse(out act, skipAoeCheck: true)) return true;
        return base.DefenseAreaAbility(out act);
    }
}