namespace RotationSolver.Basic.Rotations.Basic;

partial class ReaperRotation
{
    /// <inheritdoc/>
    public override MedicineType MedicineType => MedicineType.Strength;

    /// <summary>
    /// 
    /// </summary>
    public static bool HasEnshrouded => Player.HasStatus(true, StatusID.Enshrouded);

    /// <summary>
    /// 
    /// </summary>
    public static bool HasSoulReaver => Player.HasStatus(true, StatusID.SoulReaver);

    /// <summary>
    /// 
    /// </summary>
    public static bool HasExecutioner => Player.HasStatus(true, StatusID.Executioner);

    /// <summary>
    /// 
    /// </summary>
    public static bool HasIdealHost => Player.HasStatus(true, StatusID.IdealHost);

    /// <summary>
    /// 
    /// </summary>
    public static bool HasOblatio => Player.HasStatus(true, StatusID.Oblatio);

    /// <summary>
    /// 
    /// </summary>
    public static bool HasPerfectioParata => Player.HasStatus(true, StatusID.PerfectioParata);

    #region JobGauge
    /// <summary>
    /// 
    /// </summary>
    public static byte Soul => JobGauge.Soul;

    /// <summary>
    /// 
    /// </summary>
    public static byte Shroud => JobGauge.Shroud;

    /// <summary>
    /// 
    /// </summary>
    public static byte LemureShroud => JobGauge.LemureShroud;

    /// <summary>
    /// 
    /// </summary>
    public static byte VoidShroud => JobGauge.VoidShroud;
    #endregion

    static partial void ModifySlicePvE(ref ActionSetting setting)
    {
        setting.ActionCheck = () => !HasEnshrouded && !HasSoulReaver;
    }

    static partial void ModifyWaxingSlicePvE(ref ActionSetting setting)
    {
        setting.ActionCheck = () => !HasEnshrouded && !HasSoulReaver;
    }

    static partial void ModifyInfernalSlicePvE(ref ActionSetting setting)
    {
        setting.ActionCheck = () => !HasEnshrouded && !HasSoulReaver;
    }

    static partial void ModifyShadowOfDeathPvE(ref ActionSetting setting)
    {
        setting.TargetStatusProvide = [StatusID.DeathsDesign];
        setting.ActionCheck = () => !HasSoulReaver;
    }

    static partial void ModifySoulSlicePvE(ref ActionSetting setting)
    {
        setting.ActionCheck = () => !HasEnshrouded && !HasSoulReaver && Soul <= 50;
    }

    static partial void ModifySpinningScythePvE(ref ActionSetting setting)
    {
        setting.ActionCheck = () => !HasEnshrouded && !HasSoulReaver;
    }

    static partial void ModifyNightmareScythePvE(ref ActionSetting setting)
    {
        setting.ActionCheck = () => !HasEnshrouded && !HasSoulReaver;
    }

    static partial void ModifyWhorlOfDeathPvE(ref ActionSetting setting)
    {
        setting.ActionCheck = () => !HasSoulReaver;
        setting.TargetStatusProvide = [StatusID.DeathsDesign];
        setting.CreateConfig = () => new()
        {
            AoeCount = 2,
        };
    }

    static partial void ModifySoulScythePvE(ref ActionSetting setting)
    {
        setting.ActionCheck = () => !HasEnshrouded && !HasSoulReaver && Soul <= 50;
    }

    static partial void ModifyGibbetPvE(ref ActionSetting setting)
    {
        setting.StatusNeed = [StatusID.SoulReaver];
    }

    static partial void ModifyExecutionersGibbetPvE(ref ActionSetting setting)
    {
        setting.StatusNeed = [StatusID.Executioner];
    }

    static partial void ModifyGallowsPvE(ref ActionSetting setting)
    {
        setting.StatusNeed = [StatusID.SoulReaver];
    }

    static partial void ModifyExecutionersGallowsPvE(ref ActionSetting setting)
    {
        setting.StatusNeed = [StatusID.Executioner];
    }

    static partial void ModifyGuillotinePvE(ref ActionSetting setting)
    {
        setting.StatusNeed = [StatusID.SoulReaver];
    }

    static partial void ModifyExecutionersGuillotinePvE(ref ActionSetting setting)
    {
        setting.StatusNeed = [StatusID.Executioner];
    }

    static partial void ModifyBloodStalkPvE(ref ActionSetting setting)
    {
        setting.StatusProvide = [StatusID.SoulReaver];
        setting.ActionCheck = () => !HasEnshrouded && !HasSoulReaver && Soul >= 50;
    }

    static partial void ModifyGrimSwathePvE(ref ActionSetting setting)
    {
        setting.StatusProvide = [StatusID.SoulReaver];
        setting.ActionCheck = () => !HasEnshrouded && !HasSoulReaver && Soul >= 50;
    }

    static partial void ModifyGluttonyPvE(ref ActionSetting setting)
    {
        setting.StatusProvide = [StatusID.SoulReaver, StatusID.Executioner];
        setting.ActionCheck = () => !HasEnshrouded && !HasSoulReaver && Soul >= 50;
    }

    static partial void ModifyArcaneCirclePvE(ref ActionSetting setting)
    {
        setting.StatusProvide = [StatusID.BloodsownCircle_2972];
        setting.CreateConfig = () => new()
        {
            TimeToKill = 10,
        };
    }

    static partial void ModifyPlentifulHarvestPvE(ref ActionSetting setting)
    {
        setting.StatusNeed = [StatusID.ImmortalSacrifice];
        setting.ActionCheck = () => !Player.HasStatus(true, StatusID.BloodsownCircle_2972);
    }

    static partial void ModifyEnshroudPvE(ref ActionSetting setting)
    {
        setting.StatusProvide = [StatusID.Enshrouded];
        setting.ActionCheck = () => !HasEnshrouded && !HasSoulReaver && Soul >= 50;
        setting.UnlockedByQuestID = 69614;
    }

    static partial void ModifySacrificiumPvE(ref ActionSetting setting)
    {
        setting.ActionCheck = () => HasEnshrouded && HasOblatio;
    }

    static partial void ModifyCommunioPvE(ref ActionSetting setting)
    {
        setting.StatusProvide = [StatusID.PerfectioParata];
        setting.StatusNeed = [StatusID.Enshrouded];
        setting.ActionCheck = () => LemureShroud == 1;
    }

    static partial void ModifyPerfectioPvE(ref ActionSetting setting)
    {
        setting.ActionCheck = () => HasPerfectioParata;
    }

    static partial void ModifyLemuresSlicePvE(ref ActionSetting setting)
    {
        setting.StatusNeed = [StatusID.Enshrouded];
        setting.ActionCheck = () => VoidShroud >= 2;
    }

    static partial void ModifyLemuresScythePvE(ref ActionSetting setting)
    {
        setting.StatusNeed = [StatusID.Enshrouded];
        setting.ActionCheck = () => VoidShroud >= 2;
    }

    static partial void ModifyVoidReapingPvE(ref ActionSetting setting)
    {
        setting.StatusNeed = [StatusID.Enshrouded];
    }

    static partial void ModifyCrossReapingPvE(ref ActionSetting setting)
    {
        setting.StatusNeed = [StatusID.Enshrouded];
    }

    static partial void ModifyGrimReapingPvE(ref ActionSetting setting)
    {
        setting.StatusNeed = [StatusID.Enshrouded];
    }

    static partial void ModifyHarpePvE(ref ActionSetting setting)
    {
        setting.SpecialType = SpecialActionType.MeleeRange;
    }

    static partial void ModifyHellsIngressPvE(ref ActionSetting setting)
    {
        setting.StatusProvide = [StatusID.EnhancedHarpe, StatusID.Bind];
    }

    static partial void ModifyHellsEgressPvE(ref ActionSetting setting)
    {
        setting.StatusProvide = [StatusID.EnhancedHarpe, StatusID.Bind];
    }

    static partial void ModifySoulsowPvE(ref ActionSetting setting)
    {
        setting.StatusProvide = [StatusID.Soulsow];
        setting.ActionCheck = () => !InCombat;
    }

    static partial void ModifyHarvestMoonPvE(ref ActionSetting setting)
    {
        setting.StatusNeed = [StatusID.Soulsow];
    }

    static partial void ModifyHellsIngressPvP(ref ActionSetting setting)
    {
        setting.SpecialType = SpecialActionType.MovingForward;
    }

    /// <inheritdoc/>
    [RotationDesc(ActionID.HellsIngressPvE)]
    protected sealed override bool MoveForwardAbility(IAction nextGCD, out IAction? act)
    {
        if (HellsIngressPvE.CanUse(out act)) return true;
        return base.MoveForwardAbility(nextGCD, out act);
    }

    /// <inheritdoc/>
    [RotationDesc(ActionID.FeintPvE)]
    protected sealed override bool DefenseAreaAbility(IAction nextGCD, out IAction? act)
    {
        if (!HasSoulReaver && !HasEnshrouded && !HasExecutioner && FeintPvE.CanUse(out act)) return true;
        return base.DefenseAreaAbility(nextGCD, out act);
    }

    /// <inheritdoc/>
    [RotationDesc(ActionID.ArcaneCrestPvE)]
    protected override bool DefenseSingleAbility(IAction nextGCD, out IAction? act)
    {
        if (!HasSoulReaver && !HasEnshrouded && !HasExecutioner && ArcaneCrestPvE.CanUse(out act)) return true;
        return base.DefenseSingleAbility(nextGCD, out act);
    }

    ///<inheritdoc/>
    [RotationDesc(ActionID.HellsEgressPvE)]
    protected override bool MoveBackAbility(IAction nextGCD, out IAction? act)
    {
        if (HellsEgressPvE.CanUse(out act)) return true;
        return base.MoveBackAbility(nextGCD, out act);
    }
}