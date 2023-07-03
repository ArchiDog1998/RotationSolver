using ECommons.DalamudServices;
using ECommons.ExcelServices;
using RotationSolver.Basic.Traits;

namespace RotationSolver.Basic.Rotations.Basic;

public abstract class SMN_Base : CustomRotation
{
    public override MedicineType MedicineType => MedicineType.Intelligence;
    public sealed override Job[] Jobs => new [] { Job.SMN, Job.ACN };
    protected override bool CanHealSingleSpell => false;
    protected static bool InBahamut => Service.GetAdjustedActionId(ActionID.AstralFlow) == ActionID.DeathFlare;

    protected static bool InPhoenix => Service.GetAdjustedActionId(ActionID.AstralFlow) == ActionID.Rekindle;
    #region JobGauge
    static SMNGauge JobGauge => Svc.Gauges.Get<SMNGauge>();

    protected static bool HasAetherflowStacks => JobGauge.HasAetherflowStacks;

    protected static byte Attunement => JobGauge.Attunement;

    protected static bool IsIfritReady => JobGauge.IsIfritReady;

    protected static bool IsTitanReady => JobGauge.IsTitanReady;

    protected static bool IsGarudaReady => JobGauge.IsGarudaReady;

    protected static bool InIfrit => JobGauge.IsIfritAttuned;

    protected static bool InTitan => JobGauge.IsTitanAttuned;

    protected static bool InGaruda => JobGauge.IsGarudaAttuned;

    private static float SummonTimerRemainingRaw => JobGauge.SummonTimerRemaining / 1000f;
    protected static float SummonTimerRemaining => SummonTimerRemainingRaw - DataCenter.WeaponRemain;

    protected static bool SummonTimeEndAfter(float time) => SummonTimerRemaining <= time;

    protected static bool SummonTimeEndAfterGCD(uint gcdCount = 0, float offset = 0)
        => SummonTimeEndAfter(GCDTime(gcdCount, offset));

    private static float AttunmentTimerRemainingRaw => JobGauge.AttunmentTimerRemaining / 1000f;
    protected static float AttunmentTimerRemaining => AttunmentTimerRemainingRaw - DataCenter.WeaponRemain;

    protected static bool AttunmentTimeEndAfter(float time) => AttunmentTimerRemaining <= time;

    protected static bool AttunmentTimeEndAfterGCD(uint gcdCount = 0, float offset = 0)
        => AttunmentTimeEndAfter(GCDTime(gcdCount, offset));

    private static bool HasSummon => DataCenter.HasPet && SummonTimeEndAfterGCD();
    #endregion

    public override void DisplayStatus()
    {
        ImGui.Text("AttunmentTime: " + AttunmentTimerRemainingRaw.ToString());
        ImGui.Text("SummonTime: " + SummonTimerRemainingRaw.ToString());
        ImGui.Text("Pet: " + DataCenter.HasPet.ToString());
    }

    #region Summon
    public static IBaseAction SummonRuby { get; } = new BaseAction(ActionID.SummonRuby)
    {
        StatusProvide = new[] { StatusID.IfritsFavor },
        ActionCheck = (b, m) => HasSummon && IsIfritReady
    };

    public static IBaseAction SummonTopaz { get; } = new BaseAction(ActionID.SummonTopaz)
    {
        ActionCheck = (b, m) => HasSummon && IsTitanReady,
    };

    public static IBaseAction SummonEmerald { get; } = new BaseAction(ActionID.SummonEmerald)
    {
        StatusProvide = new[] { StatusID.GarudasFavor },
        ActionCheck = (b, m) => HasSummon && IsGarudaReady,
    };

    static RandomDelay _carbuncleDelay = new RandomDelay(() => (1, 1));
    public static IBaseAction SummonCarbuncle { get; } = new BaseAction(ActionID.SummonCarbuncle)
    {
        ActionCheck = (b, m) => _carbuncleDelay.Delay(!DataCenter.HasPet && AttunmentTimerRemainingRaw == 0 && SummonTimerRemainingRaw == 0),
    };
    #endregion

    #region Summon Actions
    public static IBaseAction Gemshine { get; } = new BaseAction(ActionID.Gemshine)
    {
        ActionCheck = (b, m) => Attunement > 0 && !AttunmentTimeEndAfter(Gemshine.CastTime),
    };

    public static IBaseAction PreciousBrilliance { get; } = new BaseAction(ActionID.PreciousBrilliance)
    {
        ActionCheck = (b, m) => Attunement > 0 && !AttunmentTimeEndAfter(PreciousBrilliance.CastTime),
    };

    public static IBaseAction AetherCharge { get; } = new BaseAction(ActionID.AetherCharge)
    {
        ActionCheck = (b, m) => InCombat && HasSummon
    };

    public static IBaseAction SummonBahamut { get; } = new BaseAction(ActionID.SummonBahamut)
    {
        ActionCheck = AetherCharge.ActionCheck
    };

    public static IBaseAction EnkindleBahamut { get; } = new BaseAction(ActionID.EnkindleBahamut)
    {
        ActionCheck = (b, m) => InBahamut || InPhoenix,
    };

    public static IBaseAction DeathFlare { get; } = new BaseAction(ActionID.DeathFlare)
    {
        ActionCheck = (b, m) => InBahamut,
    };

    public static IBaseAction Rekindle { get; } = new BaseAction(ActionID.Rekindle, ActionOption.Buff)
    {
        ActionCheck = (b, m) => InPhoenix,
    };

    public static IBaseAction CrimsonCyclone { get; } = new BaseAction(ActionID.CrimsonCyclone)
    {
        StatusNeed = new[] { StatusID.IfritsFavor },
    };

    public static IBaseAction CrimsonStrike { get; } = new BaseAction(ActionID.CrimsonStrike);

    public static IBaseAction MountainBuster { get; } = new BaseAction(ActionID.MountainBuster)
    {
        StatusNeed = new[] { StatusID.TitansFavor },
    };

    public static IBaseAction Slipstream { get; } = new BaseAction(ActionID.Slipstream)
    {
        StatusNeed = new[] { StatusID.GarudasFavor },
    };
    #endregion

    #region Basic
    public static IBaseAction Ruin { get; } = new BaseAction(ActionID.RuinSMN);

    public static IBaseAction RuinIV { get; } = new BaseAction(ActionID.RuinIV)
    {
        StatusNeed = new[] { StatusID.FurtherRuin },
    };

    public static IBaseAction Outburst { get; } = new BaseAction(ActionID.Outburst);
    #endregion

    #region Abilities
    public static IBaseAction SearingLight { get; } = new BaseAction(ActionID.SearingLight, ActionOption.Buff)
    {
        StatusProvide = new[] { StatusID.SearingLight },
        ActionCheck = (b, m) => InCombat,
    };

    public static IBaseAction RadiantAegis { get; } = new BaseAction(ActionID.RadiantAegis, ActionOption.Heal)
    {
        ActionCheck = (b, m) => HasSummon
    };

    public static IBaseAction EnergyDrain { get; } = new BaseAction(ActionID.EnergyDrainSMN)
    {
        StatusProvide = new[] { StatusID.FurtherRuin },
        ActionCheck = (b, m) => !HasAetherflowStacks
    };

    public static IBaseAction Fester { get; } = new BaseAction(ActionID.Fester)
    {
        ActionCheck = (b, m) => HasAetherflowStacks
    };

    public static IBaseAction EnergySiphon { get; } = new BaseAction(ActionID.EnergySiphon)
    {
        StatusProvide = new[] { StatusID.FurtherRuin },
        ActionCheck = EnergyDrain.ActionCheck,
    };

    public static IBaseAction PainFlare { get; } = new BaseAction(ActionID.PainFlare)
    {
        ActionCheck = Fester.ActionCheck,
    };
    #endregion

    #region Heal
    private sealed protected override IBaseAction Raise => Resurrection;

    public static IBaseAction Resurrection { get; } = new BaseAction(ActionID.ResurrectionSMN, ActionOption.Friendly);

    public static IBaseAction Physick { get; } = new BaseAction(ActionID.Physick, ActionOption.Heal);
    #endregion

    #region Traits
    protected static IBaseTrait MaimAndMend    { get; } = new BaseTrait(66);
    protected static IBaseTrait MaimAndMend2    { get; } = new BaseTrait(69);
    protected static IBaseTrait EnhancedDreadwyrmTrance    { get; } = new BaseTrait(178);
    protected static IBaseTrait RuinMastery    { get; } = new BaseTrait(217);
    protected static IBaseTrait EnhancedAethercharge    { get; } = new BaseTrait(466);
    protected static IBaseTrait EnhancedAethercharge2    { get; } = new BaseTrait(467);
    protected static IBaseTrait RubySummoningMastery    { get; } = new BaseTrait(468);
    protected static IBaseTrait TopazSummoningMastery    { get; } = new BaseTrait(469);
    protected static IBaseTrait EmeraldSummoningMastery    { get; } = new BaseTrait(470);
    protected static IBaseTrait Enkindle    { get; } = new BaseTrait(471);
    protected static IBaseTrait RuinMastery2    { get; } = new BaseTrait(473);
    protected static IBaseTrait AetherchargeMastery    { get; } = new BaseTrait(474);
    protected static IBaseTrait EnhancedEnergySiphon    { get; } = new BaseTrait(475);
    protected static IBaseTrait RuinMastery3    { get; } = new BaseTrait(476);
    protected static IBaseTrait OutburstMastery    { get; } = new BaseTrait(477);
    protected static IBaseTrait OutburstMastery2    { get; } = new BaseTrait(478);
    protected static IBaseTrait RuinMastery4    { get; } = new BaseTrait(479);
    protected static IBaseTrait EnhancedRadiantAegis    { get; } = new BaseTrait(480);
    protected static IBaseTrait Enkindle2    { get; } = new BaseTrait(481);
    protected static IBaseTrait EnhancedSummonBahamut    { get; } = new BaseTrait(502);
    protected static IBaseTrait ElementalMastery    { get; } = new BaseTrait(503);
    #endregion

    [RotationDesc(ActionID.RadiantAegis)]
    protected sealed override bool DefenseSingleAbility(out IAction act)
    {
        if (RadiantAegis.CanUse(out act)) return true;
        return base.DefenseSingleAbility(out act);
    }

    [RotationDesc(ActionID.Physick)]
    protected sealed override bool HealSingleGCD(out IAction act)
    {
        if (Physick.CanUse(out act)) return true;
        return base.HealSingleGCD(out act);
    }

    [RotationDesc(ActionID.Addle)]
    protected override bool DefenseAreaAbility(out IAction act)
    {
        if (Addle.CanUse(out act)) return true;
        return base.DefenseAreaAbility(out act);
    }
}