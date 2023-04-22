namespace RotationSolver.Basic.Rotations.Basic;

public abstract class SMN_Base : CustomRotation
{
    public override MedicineType MedicineType => MedicineType.Strength;
    public sealed override ClassJobID[] JobIDs => new ClassJobID[] { ClassJobID.Summoner, ClassJobID.Arcanist };
    protected override bool CanHealSingleSpell => false;
    protected static bool InBahamut => Service.GetAdjustedActionId(ActionID.AstralFlow) == ActionID.DeathFlare;

    protected static bool InPhoenix => Service.GetAdjustedActionId(ActionID.AstralFlow) == ActionID.Rekindle;
    #region JobGauge
    static SMNGauge JobGauge => Service.JobGauges.Get<SMNGauge>();

    protected static bool HasAetherflowStacks => JobGauge.HasAetherflowStacks;

    protected static byte Attunement => JobGauge.Attunement;

    protected static bool IsIfritReady => JobGauge.IsIfritReady;

    protected static bool IsTitanReady => JobGauge.IsTitanReady;

    protected static bool IsGarudaReady => JobGauge.IsGarudaReady;

    protected static bool InIfrit => JobGauge.IsIfritAttuned;

    protected static bool InTitan => JobGauge.IsTitanAttuned;

    protected static bool InGaruda => JobGauge.IsGarudaAttuned;

    private static float SummonTimerRemaining => JobGauge.SummonTimerRemaining / 1000f;
    protected static bool SummonTimeEndAfter(float time) => EndAfter(SummonTimerRemaining, time);

    protected static bool SummonTimeEndAfterGCD(uint gcdCount = 0, float offset = 0)
        => EndAfterGCD(SummonTimerRemaining, gcdCount, offset);

    private static float AttunmentTimerRemaining => JobGauge.AttunmentTimerRemaining / 1000f;
    protected static bool AttunmentTimeEndAfter(float time) => EndAfter(AttunmentTimerRemaining, time);

    protected static bool AttunmentTimeEndAfterGCD(uint gctCount = 0, float offset = 0)
        => EndAfterGCD(AttunmentTimerRemaining, gctCount, offset);

    private static bool HasSummon => DataCenter.HasPet && SummonTimeEndAfterGCD();
    #endregion

    #region Summon
    public static IBaseAction SummonRuby { get; } = new BaseAction(ActionID.SummonRuby)
    {
        StatusProvide = new[] { StatusID.IfritsFavor },
        ActionCheck = b => HasSummon && IsIfritReady
    };

    public static IBaseAction SummonTopaz { get; } = new BaseAction(ActionID.SummonTopaz)
    {
        ActionCheck = b => HasSummon && IsTitanReady,
    };

    public static IBaseAction SummonEmerald { get; } = new BaseAction(ActionID.SummonEmerald)
    {
        StatusProvide = new[] { StatusID.GarudasFavor },
        ActionCheck = b => HasSummon && IsGarudaReady,
    };

    public static IBaseAction SummonCarbuncle { get; } = new BaseAction(ActionID.SummonCarbuncle)
    {
        ActionCheck = b => !DataCenter.HasPet,
    };
    #endregion

    #region Summon Actions
    public static IBaseAction Gemshine { get; } = new BaseAction(ActionID.Gemshine)
    {
        ActionCheck = b => Attunement > 0,
    };

    public static IBaseAction PreciousBrilliance { get; } = new BaseAction(ActionID.PreciousBrilliance)
    {
        ActionCheck = b => Attunement > 0,
    };

    public static IBaseAction AetherCharge { get; } = new BaseAction(ActionID.AetherCharge)
    {
        ActionCheck = b => InCombat && HasSummon
    };

    public static IBaseAction SummonBahamut { get; } = new BaseAction(ActionID.SummonBahamut)
    {
        ActionCheck = b => InCombat && HasSummon
    };

    public static IBaseAction EnkindleBahamut { get; } = new BaseAction(ActionID.EnkindleBahamut)
    {
        ActionCheck = b => InBahamut || InPhoenix,
    };

    public static IBaseAction DeathFlare { get; } = new BaseAction(ActionID.DeathFlare)
    {
        ActionCheck = b => InBahamut,
    };

    public static IBaseAction Rekindle { get; } = new BaseAction(ActionID.Rekindle, ActionOption.Buff)
    {
        ActionCheck = b => InPhoenix,
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
        ActionCheck = b => InCombat,
    };

    public static IBaseAction RadiantAegis { get; } = new BaseAction(ActionID.RadiantAegis, ActionOption.Heal)
    {
        ActionCheck = b => HasSummon
    };

    public static IBaseAction EnergyDrain { get; } = new BaseAction(ActionID.EnergyDrainSMN)
    {
        StatusProvide = new[] { StatusID.FurtherRuin },
        ActionCheck = b => !HasAetherflowStacks
    };

    public static IBaseAction Fester { get; } = new BaseAction(ActionID.Fester)
    {
        ActionCheck = b => HasAetherflowStacks
    };

    public static IBaseAction EnergySiphon { get; } = new BaseAction(ActionID.EnergySiphon)
    {
        StatusProvide = new[] { StatusID.FurtherRuin },
        ActionCheck = b => !HasAetherflowStacks
    };

    public static IBaseAction PainFlare { get; } = new BaseAction(ActionID.PainFlare)
    {
        ActionCheck = b => HasAetherflowStacks
    };
    #endregion

    #region Heal
    private sealed protected override IBaseAction Raise => Resurrection;

    public static IBaseAction Resurrection { get; } = new BaseAction(ActionID.ResurrectionSMN, ActionOption.Friendly);

    public static IBaseAction Physick { get; } = new BaseAction(ActionID.Physick, ActionOption.Heal);
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