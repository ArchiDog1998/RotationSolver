using ECommons.DalamudServices;
using ECommons.ExcelServices;
using RotationSolver.Basic.Traits;

namespace RotationSolver.Basic.Rotations.Basic;

/// <summary>
/// The base class of SMN.
/// </summary>
public abstract class SMN_Base : CustomRotation
{
    /// <summary>
    /// 
    /// </summary>
    public override MedicineType MedicineType => MedicineType.Intelligence;

    /// <summary>
    /// 
    /// </summary>
    public sealed override Job[] Jobs => new[] { Job.SMN, Job.ACN };

    /// <summary>
    /// 
    /// </summary>
    public override bool CanHealSingleSpell => false;

    /// <summary>
    /// 
    /// </summary>
    public static bool InBahamut => Service.GetAdjustedActionId(ActionID.AstralFlow) == ActionID.DeathFlare;

    /// <summary>
    /// 
    /// </summary>
    public static bool InPhoenix => Service.GetAdjustedActionId(ActionID.AstralFlow) == ActionID.Rekindle;

    #region JobGauge
    static SMNGauge JobGauge => Svc.Gauges.Get<SMNGauge>();

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
    public static float SummonTime => SummonTimeRaw - DataCenter.WeaponRemain;

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
    public static float AttunmentTime => AttunmentTimeRaw - DataCenter.WeaponRemain;

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

    /// <summary>
    /// 
    /// </summary>
    public override void DisplayStatus()
    {
        ImGui.Text("AttunmentTime: " + AttunmentTimeRaw.ToString());
        ImGui.Text("SummonTime: " + SummonTimeRaw.ToString());
        ImGui.Text("Pet: " + DataCenter.HasPet.ToString());
    }

    #region Summon
    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction SummonRuby { get; } = new BaseAction(ActionID.SummonRuby)
    {
        StatusProvide = new[] { StatusID.IfritsFavor },
        ActionCheck = (b, m) => HasSummon && IsIfritReady
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction SummonTopaz { get; } = new BaseAction(ActionID.SummonTopaz)
    {
        ActionCheck = (b, m) => HasSummon && IsTitanReady,
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction SummonEmerald { get; } = new BaseAction(ActionID.SummonEmerald)
    {
        StatusProvide = new[] { StatusID.GarudasFavor },
        ActionCheck = (b, m) => HasSummon && IsGarudaReady,
    };

    static RandomDelay _carbuncleDelay = new RandomDelay(() => (2, 2));
    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction SummonCarbuncle { get; } = new BaseAction(ActionID.SummonCarbuncle)
    {
        ActionCheck = (b, m) => _carbuncleDelay.Delay(!DataCenter.HasPet && AttunmentTimeRaw == 0 && SummonTimeRaw == 0),
    };
    #endregion

    #region Summon Actions
    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction Gemshine { get; } = new BaseAction(ActionID.Gemshine)
    {
        ActionCheck = (b, m) => Attunement > 0 && !AttunmentTimeEndAfter(Gemshine.CastTime),
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction PreciousBrilliance { get; } = new BaseAction(ActionID.PreciousBrilliance)
    {
        ActionCheck = (b, m) => Attunement > 0 && !AttunmentTimeEndAfter(PreciousBrilliance.CastTime),
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction AetherCharge { get; } = new BaseAction(ActionID.AetherCharge)
    {
        ActionCheck = (b, m) => InCombat && HasSummon
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction SummonBahamut { get; } = new BaseAction(ActionID.SummonBahamut)
    {
        ActionCheck = AetherCharge.ActionCheck
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction EnkindleBahamut { get; } = new BaseAction(ActionID.EnkindleBahamut)
    {
        ActionCheck = (b, m) => InBahamut || InPhoenix,
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction DeathFlare { get; } = new BaseAction(ActionID.DeathFlare)
    {
        ActionCheck = (b, m) => InBahamut,
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction Rekindle { get; } = new BaseAction(ActionID.Rekindle, ActionOption.Buff)
    {
        ActionCheck = (b, m) => InPhoenix,
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction CrimsonCyclone { get; } = new BaseAction(ActionID.CrimsonCyclone)
    {
        StatusNeed = new[] { StatusID.IfritsFavor },
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction CrimsonStrike { get; } = new BaseAction(ActionID.CrimsonStrike);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction MountainBuster { get; } = new BaseAction(ActionID.MountainBuster)
    {
        StatusNeed = new[] { StatusID.TitansFavor },
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction Slipstream { get; } = new BaseAction(ActionID.Slipstream)
    {
        StatusNeed = new[] { StatusID.GarudasFavor },
    };
    #endregion

    #region Basic
    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction Ruin { get; } = new BaseAction(ActionID.RuinSMN);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction RuinIV { get; } = new BaseAction(ActionID.RuinIV)
    {
        StatusNeed = new[] { StatusID.FurtherRuin },
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction Outburst { get; } = new BaseAction(ActionID.Outburst);
    #endregion

    #region Abilities
    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction SearingLight { get; } = new BaseAction(ActionID.SearingLight, ActionOption.Buff)
    {
        StatusProvide = new[] { StatusID.SearingLight },
        ActionCheck = (b, m) => InCombat && IsLongerThan(15),
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction RadiantAegis { get; } = new BaseAction(ActionID.RadiantAegis, ActionOption.Heal)
    {
        ActionCheck = (b, m) => HasSummon
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction EnergyDrain { get; } = new BaseAction(ActionID.EnergyDrainSMN)
    {
        StatusProvide = new[] { StatusID.FurtherRuin },
        ActionCheck = (b, m) => !HasAetherflowStacks
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction Fester { get; } = new BaseAction(ActionID.Fester, ActionOption.UseResources)
    {
        ActionCheck = (b, m) => HasAetherflowStacks
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction EnergySiphon { get; } = new BaseAction(ActionID.EnergySiphon)
    {
        StatusProvide = new[] { StatusID.FurtherRuin },
        ActionCheck = EnergyDrain.ActionCheck,
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction PainFlare { get; } = new BaseAction(ActionID.PainFlare)
    {
        ActionCheck = Fester.ActionCheck,
    };
    #endregion

    #region Heal
    private protected sealed override IBaseAction Raise => Resurrection;

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction Resurrection { get; } = new BaseAction(ActionID.ResurrectionSMN, ActionOption.Friendly);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction Physick { get; } = new BaseAction(ActionID.Physick, ActionOption.Heal);
    #endregion

    #region Traits
    /// <summary>
    /// 
    /// </summary>
    public static IBaseTrait MaimAndMend { get; } = new BaseTrait(66);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseTrait MaimAndMend2 { get; } = new BaseTrait(69);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseTrait EnhancedDreadwyrmTrance { get; } = new BaseTrait(178);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseTrait RuinMastery { get; } = new BaseTrait(217);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseTrait EnhancedAethercharge { get; } = new BaseTrait(466);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseTrait EnhancedAethercharge2 { get; } = new BaseTrait(467);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseTrait RubySummoningMastery { get; } = new BaseTrait(468);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseTrait TopazSummoningMastery { get; } = new BaseTrait(469);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseTrait EmeraldSummoningMastery { get; } = new BaseTrait(470);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseTrait Enkindle { get; } = new BaseTrait(471);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseTrait RuinMastery2 { get; } = new BaseTrait(473);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseTrait AetherchargeMastery { get; } = new BaseTrait(474);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseTrait EnhancedEnergySiphon { get; } = new BaseTrait(475);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseTrait RuinMastery3 { get; } = new BaseTrait(476);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseTrait OutburstMastery { get; } = new BaseTrait(477);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseTrait OutburstMastery2 { get; } = new BaseTrait(478);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseTrait RuinMastery4 { get; } = new BaseTrait(479);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseTrait EnhancedRadiantAegis { get; } = new BaseTrait(480);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseTrait Enkindle2 { get; } = new BaseTrait(481);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseTrait EnhancedSummonBahamut { get; } = new BaseTrait(502);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseTrait ElementalMastery { get; } = new BaseTrait(503);
    #endregion

    /// <summary>
    /// 
    /// </summary>
    /// <param name="act"></param>
    /// <returns></returns>
    [RotationDesc(ActionID.RadiantAegis)]
    protected sealed override bool DefenseSingleAbility(out IAction act)
    {
        if (RadiantAegis.CanUse(out act)) return true;
        return base.DefenseSingleAbility(out act);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="act"></param>
    /// <returns></returns>
    [RotationDesc(ActionID.Physick)]
    protected sealed override bool HealSingleGCD(out IAction act)
    {
        if (Physick.CanUse(out act)) return true;
        return base.HealSingleGCD(out act);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="act"></param>
    /// <returns></returns>
    [RotationDesc(ActionID.Addle)]
    protected override bool DefenseAreaAbility(out IAction act)
    {
        if (Addle.CanUse(out act)) return true;
        return base.DefenseAreaAbility(out act);
    }
}