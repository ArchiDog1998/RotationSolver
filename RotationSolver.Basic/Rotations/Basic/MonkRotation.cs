namespace RotationSolver.Basic.Rotations.Basic;

partial class MonkRotation
{
    /// <inheritdoc/>
    public override MedicineType MedicineType => MedicineType.Strength;

    #region Job Gauge
    /// <summary>
    /// 
    /// </summary>
    protected static BeastChakra[] BeastChakras => JobGauge.BeastChakra;

    /// <summary>
    /// 
    /// </summary>
    public static byte Chakra => JobGauge.Chakra;

    /// <summary>
    /// 
    /// </summary>
    public static bool HasSolar => JobGauge.Nadi.HasFlag(Nadi.SOLAR);

    /// <summary>
    /// 
    /// </summary>
    public static bool HasLunar => JobGauge.Nadi.HasFlag(Nadi.LUNAR);
    #endregion

    static partial void ModifyDragonKickPvE(ref ActionSetting setting)
    {
        setting.StatusProvide = [StatusID.LeadenFist];
    }

    static partial void ModifyDemolishPvE(ref ActionSetting setting)
    {
        setting.TargetStatusProvide = [StatusID.Demolish];
        setting.CreateConfig = () => new()
        {
            StatusGcdCount = 3,
        };
    }

    static partial void ModifySteelPeakPvE(ref ActionSetting setting)
    {
        setting.ActionCheck = () => InCombat && Chakra == 5;
    }

    static partial void ModifyHowlingFistPvE(ref ActionSetting setting)
    {
        setting.ActionCheck = () => InCombat && Chakra == 5;
    }

    static partial void ModifyMantraPvE(ref ActionSetting setting)
    {
        setting.CreateConfig = () => new()
        {
            TimeToKill = 10,
        };
    }

    static partial void ModifyRiddleOfEarthPvE(ref ActionSetting setting)
    {
        setting.StatusProvide = [StatusID.RiddleOfEarth];
    }

    static partial void ModifyRiddleOfWindPvE(ref ActionSetting setting)
    {
        setting.CreateConfig = () => new()
        {
            TimeToKill = 10,
        };
    }

    static partial void ModifyPerfectBalancePvE(ref ActionSetting setting)
    {
        setting.ActionCheck = () => InCombat && IsLongerThan(5);
    }

    static partial void ModifyFormShiftPvE(ref ActionSetting setting)
    {
        setting.StatusProvide = [StatusID.FormlessFist, StatusID.PerfectBalance];
    }

    static partial void ModifyBrotherhoodPvE(ref ActionSetting setting)
    {
        setting.CreateConfig = () => new()
        {
            TimeToKill = 10,
        };
    }

    static partial void ModifyRiddleOfFirePvE(ref ActionSetting setting)
    {
        setting.CreateConfig = () => new()
        {
            TimeToKill = 10,
        };
    }

    static partial void ModifyThunderclapPvE(ref ActionSetting setting)
    {
        setting.SpecialType = SpecialActionType.MovingForward;
    }

    /// <inheritdoc/>
    [RotationDesc(ActionID.ThunderclapPvE)]
    protected sealed override bool MoveForwardAbility(IAction nextGCD, out IAction? act)
    {
        if (ThunderclapPvE.CanUse(out act)) return true;
        return base.MoveForwardAbility(nextGCD, out act);
    }

    /// <inheritdoc/>
    [RotationDesc(ActionID.FeintPvE)]
    protected sealed override bool DefenseAreaAbility(IAction nextGCD, out IAction? act)
    {
        if (FeintPvE.CanUse(out act)) return true;
        return base.DefenseAreaAbility(nextGCD, out act);
    }

    /// <inheritdoc/>
    [RotationDesc(ActionID.MantraPvE)]
    protected sealed override bool HealAreaAbility(IAction nextGCD, out IAction? act)
    {
        if (MantraPvE.CanUse(out act)) return true;
        return base.HealAreaAbility(nextGCD, out act);
    }

    /// <inheritdoc/>
    [RotationDesc(ActionID.RiddleOfEarthPvE)]
    protected sealed override bool DefenseSingleAbility(IAction nextGCD, out IAction? act)
    {
        if (RiddleOfEarthPvE.CanUse(out act, usedUp: true)) return true;
        return base.DefenseSingleAbility(nextGCD, out act);
    }
}
