using ECommons.DalamudServices;
using ECommons.ExcelServices;
using RotationSolver.Basic.Traits;

namespace RotationSolver.Basic.Rotations.Basic;

public abstract class BRD_Base : CustomRotation
{
    public override MedicineType MedicineType => MedicineType.Dexterity;

    public sealed override Job[] Jobs => new[] { ECommons.ExcelServices.Job.BRD, ECommons.ExcelServices.Job.ARC };

    #region Job Gauge
    static BRDGauge JobGauge => Svc.Gauges.Get<BRDGauge>();

    protected static byte Repertoire => JobGauge.Repertoire;

    protected static Song Song => JobGauge.Song;

    protected static Song LastSong => JobGauge.LastSong;

    protected static byte SoulVoice => JobGauge.SoulVoice;
    static float SongTime => JobGauge.SongTimer / 1000f;

    protected static bool SongEndAfter(float time) => EndAfter(SongTime, time);

    protected static bool SongEndAfterGCD(uint gctCount = 0, float offset = 0)
        => EndAfterGCD(SongTime, gctCount, offset);
    #endregion

    #region Attack Single
    public static IBaseAction HeavyShoot { get; } = new BaseAction(ActionID.HeavyShoot)
    {
        StatusProvide = new[] { StatusID.StraightShotReady }
    };

    public static IBaseAction StraitShoot { get; } = new BaseAction(ActionID.StraitShoot)
    {
        StatusNeed = new[] { StatusID.StraightShotReady }
    };

    public static IBaseAction VenomousBite { get; } = new BaseAction(ActionID.VenomousBite, ActionOption.Dot)
    {
        TargetStatus = new[] { StatusID.VenomousBite, StatusID.CausticBite }
    };

    public static IBaseAction WindBite { get; } = new BaseAction(ActionID.WindBite, ActionOption.Dot)
    {
        TargetStatus = new[] { StatusID.WindBite, StatusID.StormBite }
    };

    public static IBaseAction IronJaws { get; } = new BaseAction(ActionID.IronJaws, ActionOption.Dot)
    {
        TargetStatus = VenomousBite.TargetStatus.Union(WindBite.TargetStatus).ToArray(),
        ActionCheck = (b, m) => b.HasStatus(true, VenomousBite.TargetStatus) & b.HasStatus(true, WindBite.TargetStatus),
    };

    public static IBaseAction EmpyrealArrow { get; } = new BaseAction(ActionID.EmpyrealArrow);

    public static IBaseAction PitchPerfect { get; } = new BaseAction(ActionID.PitchPerfect)
    {
        ActionCheck = (b, m) => Song == Song.WANDERER && Repertoire > 0,
    };

    public static IBaseAction Bloodletter { get; } = new BaseAction(ActionID.Bloodletter);

    public static IBaseAction Sidewinder { get; } = new BaseAction(ActionID.Sidewinder);
    #endregion

    #region Attack Area
    public static IBaseAction QuickNock { get; } = new BaseAction(ActionID.QuickNock)
    {
        StatusProvide = new[] { StatusID.ShadowBiteReady }
    };

    public static IBaseAction ShadowBite { get; } = new BaseAction(ActionID.ShadowBite)
    {
        StatusNeed = new[] { StatusID.ShadowBiteReady }
    };

    public static IBaseAction ApexArrow { get; } = new BaseAction(ActionID.ApexArrow)
    {
        ActionCheck = (b, m) => SoulVoice >= 20 && !Player.HasStatus(true, StatusID.BlastArrowReady),
    };

    public static IBaseAction BlastArrow { get; } = new BaseAction(ActionID.BlastArrow)
    {
        ActionCheck = (b, m) => Player.HasStatus(true, StatusID.BlastArrowReady),
    };

    public static IBaseAction RainOfDeath { get; } = new BaseAction(ActionID.RainOfDeath)
    {
        AOECount = 2,
    };
    #endregion

    #region Support
    public static IBaseAction BattleVoice { get; } = new BaseAction(ActionID.BattleVoice, ActionOption.Buff);

    public static IBaseAction RadiantFinale { get; } = new BaseAction(ActionID.RadiantFinale, ActionOption.Buff)
    {
        ActionCheck = (b, m) => JobGauge.Coda.Any(s => s != Song.NONE),
    };

    public static IBaseAction Barrage { get; } = new BaseAction(ActionID.Barrage);

    public static IBaseAction RagingStrikes { get; } = new BaseAction(ActionID.RagingStrikes);

    public static IBaseAction WanderersMinuet { get; } = new BaseAction(ActionID.WanderersMinuet);

    public static IBaseAction MagesBallad { get; } = new BaseAction(ActionID.MagesBallad);

    public static IBaseAction ArmysPaeon { get; } = new BaseAction(ActionID.ArmysPaeon);
    #endregion

    #region Heal Defense
    public static IBaseAction WardensPaean { get; } = new BaseAction(ActionID.WardensPaean, ActionOption.Heal);

    public static IBaseAction NaturesMinne { get; } = new BaseAction(ActionID.NaturesMinne, ActionOption.Heal);

    public static IBaseAction Troubadour { get; } = new BaseAction(ActionID.Troubadour, ActionOption.Defense)
    {
        ActionCheck = (b, m) => !Player.HasStatus(false, StatusID.Troubadour,
            StatusID.Tactician1, StatusID.Tactician2, StatusID.ShieldSamba),
    };
    #endregion

    #region Traits
    protected static IBaseTrait HeavierShot { get; } = new BaseTrait(17);
    protected static IBaseTrait IncreasedActionDamage { get; } = new BaseTrait(18);
    protected static IBaseTrait IncreasedActionDamage2 { get; } = new BaseTrait(20);
    protected static IBaseTrait BiteMastery { get; } = new BaseTrait(168);
    protected static IBaseTrait EnhancedEmpyrealArrow    { get; } = new BaseTrait(169);
    protected static IBaseTrait StraightShotMastery    { get; } = new BaseTrait(282);
    protected static IBaseTrait EnhancedQuickNock    { get; } = new BaseTrait(283);
    protected static IBaseTrait BiteMastery2 { get; } = new BaseTrait(284);
    protected static IBaseTrait HeavyShotMastery    { get; } = new BaseTrait(285);
    protected static IBaseTrait EnhancedArmysPaeon    { get; } = new BaseTrait(286);
    protected static IBaseTrait SoulVoiceTraits    { get; } = new BaseTrait(287);
    protected static IBaseTrait QuickNockMastery    { get; } = new BaseTrait(444);
    protected static IBaseTrait EnhancedBloodletter    { get; } = new BaseTrait(445);
    protected static IBaseTrait EnhancedApexArrow    { get; } = new BaseTrait(446);
    protected static IBaseTrait EnhancedTroubadour    { get; } = new BaseTrait(447);
    protected static IBaseTrait MinstrelsCoda    { get; } = new BaseTrait(448);

    #endregion

    protected override bool EmergencyAbility(IAction nextGCD, out IAction act)
    {
        //Esuna
        if (DataCenter.SpecialType == SpecialCommandType.EsunaStanceNorth && DataCenter.WeakenPeople.Any() || DataCenter.DyingPeople.Any())
        {
            if (WardensPaean.CanUse(out act, CanUseOption.MustUse)) return true;
        }
        return base.EmergencyAbility(nextGCD, out act);
    }

    [RotationDesc(ActionID.Troubadour)]
    protected sealed override bool DefenseAreaAbility(out IAction act)
    {
        if (Troubadour.CanUse(out act)) return true;
        return false;
    }

    [RotationDesc(ActionID.NaturesMinne)]
    protected sealed override bool HealSingleAbility(out IAction act)
    {
        if (NaturesMinne.CanUse(out act)) return true;
        return base.HealSingleAbility(out act);
    }
}
