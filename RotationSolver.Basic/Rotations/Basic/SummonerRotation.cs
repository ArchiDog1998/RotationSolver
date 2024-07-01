namespace RotationSolver.Basic.Rotations.Basic;

partial class SummonerRotation
{
    /// <inheritdoc/>
    public override MedicineType MedicineType => MedicineType.Intelligence;

    /// <summary>
    /// 
    /// </summary>
    public override bool CanHealSingleSpell => false;

    /// <summary>
    /// 
    /// </summary>
    public static bool InBahamut => Service.GetAdjustedActionId(ActionID.AstralFlowPvE) == ActionID.DeathflarePvE;
    
    /// <summary>
    /// 
    /// </summary>
    public static bool InSolarBahamut => Service.GetAdjustedActionId(ActionID.AstralFlowPvE) == ActionID.SunflarePvE;

    /// <summary>
    /// 
    /// </summary>
    public static bool InPhoenix => Service.GetAdjustedActionId(ActionID.AstralFlowPvE) == ActionID.RekindlePvE;
    private protected sealed override IBaseAction Raise => ResurrectionPvE;

    #region JobGauge
    /// <summary>
    /// 
    /// </summary>
    public static bool HasAetherflowStacks => JobGauge.HasAetherflowStacks;

    /// <summary>
    /// 
    /// </summary>
    public static byte Attunement => JobGauge.Attunement;

    /// <summary>
    /// 
    /// </summary>
    public static bool IsIfritReady => JobGauge.IsIfritReady;

    /// <summary>
    /// 
    /// </summary>
    public static bool IsTitanReady => JobGauge.IsTitanReady;

    /// <summary>
    /// 
    /// </summary>
    public static bool IsGarudaReady => JobGauge.IsGarudaReady;

    /// <summary>
    /// 
    /// </summary>
    public static bool InIfrit => JobGauge.IsIfritAttuned;

    /// <summary>
    /// 
    /// </summary>
    public static bool InTitan => JobGauge.IsTitanAttuned;

    /// <summary>
    /// 
    /// </summary>
    public static bool InGaruda => JobGauge.IsGarudaAttuned;

    private static float SummonTimeRaw => JobGauge.SummonTimerRemaining / 1000f;

    /// <summary>
    /// 
    /// </summary>
    public static float SummonTime => SummonTimeRaw - DataCenter.DefaultGCDRemain;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    protected static bool SummonTimeEndAfter(float time) => SummonTime <= time;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="gcdCount"></param>
    /// <param name="offset"></param>
    /// <returns></returns>
    protected static bool SummonTimeEndAfterGCD(uint gcdCount = 0, float offset = 0)
        => SummonTimeEndAfter(GCDTime(gcdCount, offset));

    private static float AttunmentTimeRaw => JobGauge.AttunmentTimerRemaining / 1000f;

    /// <summary>
    /// 
    /// </summary>
    public static float AttunmentTime => AttunmentTimeRaw - DataCenter.DefaultGCDRemain;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    protected static bool AttunmentTimeEndAfter(float time) => AttunmentTime <= time;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="gcdCount"></param>
    /// <param name="offset"></param>
    /// <returns></returns>
    protected static bool AttunmentTimeEndAfterGCD(uint gcdCount = 0, float offset = 0)
        => AttunmentTimeEndAfter(GCDTime(gcdCount, offset));

    /// <summary>
    /// 
    /// </summary>
    private static bool HasSummon => DataCenter.HasPet && SummonTimeEndAfterGCD();
    #endregion

    /// <inheritdoc/>
    public override void DisplayStatus()
    {
        ImGui.Text("AttunmentTime: " + AttunmentTimeRaw.ToString());
        ImGui.Text("SummonTime: " + SummonTimeRaw.ToString());
        ImGui.Text("Pet: " + DataCenter.HasPet.ToString());
    }

    static partial void ModifySummonRubyPvE(ref ActionSetting setting)
    {
        setting.StatusProvide = [StatusID.IfritsFavor];
        setting.ActionCheck = () => SummonTime <= WeaponRemain && IsIfritReady;
    }

    static partial void ModifySummonTopazPvE(ref ActionSetting setting)
    {
        setting.ActionCheck = () => SummonTime <= WeaponRemain && IsTitanReady;
        setting.UnlockedByQuestID = 66639;
    }

    static partial void ModifySummonEmeraldPvE(ref ActionSetting setting)
    {
        setting.StatusProvide = [StatusID.GarudasFavor];
        setting.ActionCheck = () => SummonTime <= WeaponRemain && IsGarudaReady;
    }

    static RandomDelay _carbuncleDelay = new (() => (2, 2));
    static partial void ModifySummonCarbunclePvE(ref ActionSetting setting)
    {
        setting.ActionCheck = () => _carbuncleDelay.Delay(!DataCenter.HasPet && AttunmentTimeRaw == 0 && SummonTimeRaw == 0) && DataCenter.LastGCD is not ActionID.SummonCarbunclePvE;
    }

    static partial void ModifyGemshinePvE(ref ActionSetting setting)
    {
        setting.ActionCheck = () => Attunement > 0 && !AttunmentTimeEndAfter(ActionID.GemshinePvE.GetCastTime());
    }

    static partial void ModifyPreciousBrilliancePvE(ref ActionSetting setting)
    {
        setting.ActionCheck = () => Attunement > 0 && !AttunmentTimeEndAfter(ActionID.PreciousBrilliancePvE.GetCastTime());
    }

    static partial void ModifyAetherchargePvE(ref ActionSetting setting)
    {
        setting.ActionCheck = () => InCombat && HasSummon;
    }

    static partial void ModifySummonBahamutPvE(ref ActionSetting setting)
    {
        setting.ActionCheck = () => InCombat && SummonTime <= WeaponRemain;
        setting.UnlockedByQuestID = 68165;
    }

    static partial void ModifyEnkindleBahamutPvE(ref ActionSetting setting)
    {
        setting.ActionCheck = () => InBahamut || InPhoenix;
    }

    static partial void ModifyEnkindleSolarBahamutPvE(ref ActionSetting setting)
    {
        setting.ActionCheck = () => InSolarBahamut;
    }

    static partial void ModifyDeathflarePvE(ref ActionSetting setting)
    {
        setting.ActionCheck = () => InBahamut;
    }

    static partial void ModifySunflarePvE(ref ActionSetting setting)
    {
        setting.ActionCheck = () => InSolarBahamut;
    }

    static partial void ModifySearingFlashPvE(ref ActionSetting setting)
    {
        setting.StatusNeed = [StatusID.RefulgentLux];
    }

    static partial void ModifyLuxSolarisPvE(ref ActionSetting setting)
    {
        setting.StatusNeed = [StatusID.RefulgentLux];
    }

    static partial void ModifyRekindlePvE(ref ActionSetting setting)
    {
        setting.ActionCheck = () => InPhoenix;
    }

    static partial void ModifyCrimsonCyclonePvE(ref ActionSetting setting)
    {
        setting.StatusNeed = [StatusID.IfritsFavor];
    }

    static partial void ModifyCrimsonCyclonePvP(ref ActionSetting setting)
    {
        setting.SpecialType = SpecialActionType.MovingForward;
    }

    static partial void ModifyMountainBusterPvE(ref ActionSetting setting)
    {
        setting.StatusNeed = [StatusID.TitansFavor];
    }

    static partial void ModifySlipstreamPvE(ref ActionSetting setting)
    {
        setting.StatusNeed = [StatusID.GarudasFavor];
    }

    static partial void ModifyRuinIvPvE(ref ActionSetting setting)
    {
        setting.StatusNeed = [StatusID.FurtherRuin_2701];
    }

    static partial void ModifySearingLightPvE(ref ActionSetting setting)
    {
        setting.StatusProvide = [StatusID.SearingLight];
        setting.ActionCheck = () => InCombat;
        setting.CreateConfig = () => new()
        {
            TimeToKill = 15,
        };
    }

    static partial void ModifyRadiantAegisPvE(ref ActionSetting setting)
    {
        setting.ActionCheck = () => HasSummon;
    }

    static partial void ModifyEnergyDrainPvE(ref ActionSetting setting)
    {
        setting.StatusProvide = [StatusID.FurtherRuin];
        setting.ActionCheck = () => !HasAetherflowStacks;
    }

    static partial void ModifyFesterPvE(ref ActionSetting setting)
    {
        setting.ActionCheck = () => HasAetherflowStacks;
    }
    
    static partial void ModifyNecrotizePvE(ref ActionSetting setting)
    {
        setting.ActionCheck = () => HasAetherflowStacks;
    }
    
    static partial void ModifyEnergySiphonPvE(ref ActionSetting setting)
    {
        setting.StatusProvide = [StatusID.FurtherRuin];
        setting.ActionCheck = () => !HasAetherflowStacks;
        setting.UnlockedByQuestID = 67637;
    }

    static partial void ModifyPainflarePvE(ref ActionSetting setting)
    {
        setting.ActionCheck = () => HasAetherflowStacks;
        setting.UnlockedByQuestID = 66629;
    }

    static partial void ModifyRuinIiPvE(ref ActionSetting setting)
    {
        setting.UnlockedByQuestID = 65997;
    }

    static partial void ModifySummonIfritPvE(ref ActionSetting setting)
    {
        setting.UnlockedByQuestID = 66627;
    }

    static partial void ModifySummonTitanPvE(ref ActionSetting setting)
    {
        setting.UnlockedByQuestID = 66628;
    }

    static partial void ModifySummonGarudaPvE(ref ActionSetting setting)
    {
        setting.UnlockedByQuestID = 66631;
    }

    static partial void ModifyRuinIiiPvE(ref ActionSetting setting)
    {
        setting.UnlockedByQuestID = 67638;
    }

    static partial void ModifyDreadwyrmTrancePvE(ref ActionSetting setting)
    {
        setting.UnlockedByQuestID = 67640;
    }

    static partial void ModifyAstralFlowPvE(ref ActionSetting setting)
    {
        setting.UnlockedByQuestID = 67641;
    }


    /// <inheritdoc/>
    [RotationDesc(ActionID.RadiantAegisPvE)]
    protected sealed override bool DefenseSingleAbility(IAction nextGCD, out IAction? act)
    {
        if (RadiantAegisPvE.CanUse(out act)) return true;
        return base.DefenseSingleAbility(nextGCD, out act);
    }

    /// <inheritdoc/>
    [RotationDesc(ActionID.PhysickPvE)]
    protected sealed override bool HealSingleGCD(out IAction? act)
    {
        if (PhysickPvE.CanUse(out act)) return true;
        return base.HealSingleGCD(out act);
    }

    /// <inheritdoc/>
    [RotationDesc(ActionID.AddlePvE)]
    protected override bool DefenseAreaAbility(IAction nextGCD, out IAction? act)
    {
        if (AddlePvE.CanUse(out act)) return true;
        return base.DefenseAreaAbility(nextGCD, out act);
    }
    
    /// <inheritdoc/>
    [RotationDesc(ActionID.CrimsonCyclonePvE)]
    protected override bool MoveForwardAbility(IAction nextGCD, out IAction? act)
    {
        if (CrimsonCyclonePvE.CanUse(out act)) return true;
        return base.MoveForwardAbility(nextGCD, out act);
    }
}
