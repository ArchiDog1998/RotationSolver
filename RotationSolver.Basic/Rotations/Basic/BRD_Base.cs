using ECommons.DalamudServices;
using ECommons.ExcelServices;
using RotationSolver.Basic.Traits;

namespace RotationSolver.Basic.Rotations.Basic;

/// <summary>
/// The base class of Bard.
/// </summary>
public abstract class BRD_Base : CustomRotation
{
    /// <summary>
    /// 
    /// </summary>
    public override MedicineType MedicineType => MedicineType.Dexterity;

    /// <summary>
    /// 
    /// </summary>
    public sealed override Job[] Jobs => new[] { Job.BRD, Job.ARC };

    #region Job Gauge
    static BRDGauge JobGauge => Svc.Gauges.Get<BRDGauge>();

    /// <summary>
    /// 
    /// </summary>
    public static byte Repertoire => JobGauge.Repertoire;

    /// <summary>
    /// 
    /// </summary>
    protected static Song Song => JobGauge.Song;

    /// <summary>
    /// 
    /// </summary>
    protected static Song LastSong => JobGauge.LastSong;

    /// <summary>
    /// 
    /// </summary>
    public static byte SoulVoice => JobGauge.SoulVoice;
    static float SongTimeRaw => JobGauge.SongTimer / 1000f;

    /// <summary>
    /// 
    /// </summary>
    public static float SongTime => SongTimeRaw - DataCenter.WeaponRemain;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    protected static bool SongEndAfter(float time) => SongTime <= time;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="gctCount"></param>
    /// <param name="offset"></param>
    /// <returns></returns>
    protected static bool SongEndAfterGCD(uint gctCount = 0, float offset = 0)
        => SongEndAfter(GCDTime(gctCount, offset));
    #endregion

    #region Attack Single
    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction HeavyShoot { get; } = new BaseAction(ActionID.HeavyShot)
    {
        StatusProvide = [StatusID.StraightShotReady]
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction StraitShoot { get; } = new BaseAction(ActionID.StraightShot)
    {
        StatusNeed = new[] { StatusID.StraightShotReady }
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction VenomousBite { get; } = new BaseAction(ActionID.VenomousBite, ActionOption.Dot)
    {
        TargetStatus = new[] { StatusID.VenomousBite, StatusID.CausticBite },
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction WindBite { get; } = new BaseAction(ActionID.Windbite, ActionOption.Dot)
    {
        TargetStatus = new[] { StatusID.Windbite, StatusID.Stormbite },
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction IronJaws { get; } = new BaseAction(ActionID.IronJaws, ActionOption.Dot)
    {
        TargetStatus = VenomousBite.TargetStatus.Union(WindBite.TargetStatus).ToArray(),
        ActionCheck = (b, m) => b.HasStatus(true, VenomousBite.TargetStatus) & b.HasStatus(true, WindBite.TargetStatus),
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction EmpyrealArrow { get; } = new BaseAction(ActionID.EmpyrealArrow);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction PitchPerfect { get; } = new BaseAction(ActionID.PitchPerfect)
    {
        ActionCheck = (b, m) => Song == Song.WANDERER && Repertoire > 0,
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction Bloodletter { get; } = new BaseAction(ActionID.Bloodletter);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction Sidewinder { get; } = new BaseAction(ActionID.Sidewinder);
    #endregion

    #region Attack Area
    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction QuickNock { get; } = new BaseAction(ActionID.QuickNock)
    {
        StatusProvide = [StatusID.ShadowbiteReady],
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction ShadowBite { get; } = new BaseAction(ActionID.Shadowbite)
    {
        StatusNeed = [StatusID.ShadowbiteReady],
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction ApexArrow { get; } = new BaseAction(ActionID.ApexArrow, ActionOption.UseResources)
    {
        ActionCheck = (b, m) => SoulVoice >= 20 && !Player.HasStatus(true, StatusID.BlastArrowReady),
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction BlastArrow { get; } = new BaseAction(ActionID.BlastArrow)
    {
        ActionCheck = (b, m) => Player.HasStatus(true, StatusID.BlastArrowReady),
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction RainOfDeath { get; } = new BaseAction(ActionID.RainOfDeath);
    #endregion

    #region Support
    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction BattleVoice { get; } = new BaseAction(ActionID.BattleVoice, ActionOption.Buff)
    {
        ActionCheck = (b, m) => IsLongerThan(10),
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction RadiantFinale { get; } = new BaseAction(ActionID.RadiantFinale, ActionOption.Buff)
    {
        ActionCheck = (b, m) => JobGauge.Coda.Any(s => s != Song.NONE)
        && IsLongerThan(10),
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction Barrage { get; } = new BaseAction(ActionID.Barrage);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction RagingStrikes { get; } = new BaseAction(ActionID.RagingStrikes)
    {
        ActionCheck = (b, m) => IsLongerThan(10),
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction WanderersMinuet { get; } = new BaseAction(ActionID.TheWanderersMinuet);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction MagesBallad { get; } = new BaseAction(ActionID.MagesBallad);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction ArmysPaeon { get; } = new BaseAction(ActionID.ArmysPaeon);
    #endregion

    #region Heal Defense
    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction WardensPaean { get; } = new BaseAction(ActionID.TheWardensPaean, ActionOption.Heal)
    {
        ChoiceTarget = (tars, mustUse) =>
        {
            if (DyingPeople.Any())
            {
                return DyingPeople.OrderBy(ObjectHelper.DistanceToPlayer).First();
            }
            else if (WeakenPeople.Any())
            {
                return WeakenPeople.OrderBy(ObjectHelper.DistanceToPlayer).First();
            }
            return null;
        },
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction NaturesMinne { get; } = new BaseAction(ActionID.NaturesMinne, ActionOption.Heal);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction Troubadour { get; } = new BaseAction(ActionID.Troubadour, ActionOption.Defense)
    {
        ActionCheck = (b, m) => !Player.HasStatus(false, StatusID.Troubadour,
            StatusID.Tactician_1951, StatusID.Tactician_2177, StatusID.ShieldSamba),
    };
    #endregion

    #region Traits
    /// <summary>
    /// 
    /// </summary>
    public static IBaseTrait HeavierShot { get; } = new BaseTrait(17);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseTrait IncreasedActionDamage { get; } = new BaseTrait(18);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseTrait IncreasedActionDamage2 { get; } = new BaseTrait(20);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseTrait BiteMastery { get; } = new BaseTrait(168);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseTrait EnhancedEmpyrealArrow { get; } = new BaseTrait(169);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseTrait StraightShotMastery { get; } = new BaseTrait(282);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseTrait EnhancedQuickNock { get; } = new BaseTrait(283);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseTrait BiteMastery2 { get; } = new BaseTrait(284);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseTrait HeavyShotMastery { get; } = new BaseTrait(285);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseTrait EnhancedArmysPaeon { get; } = new BaseTrait(286);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseTrait SoulVoiceTraits { get; } = new BaseTrait(287);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseTrait QuickNockMastery { get; } = new BaseTrait(444);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseTrait EnhancedBloodletter { get; } = new BaseTrait(445);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseTrait EnhancedApexArrow { get; } = new BaseTrait(446);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseTrait EnhancedTroubadour { get; } = new BaseTrait(447);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseTrait MinstrelsCoda { get; } = new BaseTrait(448);

    #endregion

    private protected override IBaseAction LimitBreak => SagittariusArrow;

    /// <summary>
    /// LB
    /// </summary>
    public static IBaseAction SagittariusArrow { get; } = new BaseAction(ActionID.SagittariusArrow)
    {
        ActionCheck = (b, m) => LimitBreakLevel == 3,
    };

    /// <summary>
    /// 
    /// </summary>
    /// <param name="nextGCD"></param>
    /// <param name="act"></param>
    /// <returns></returns>
    protected override bool EmergencyAbility(IAction nextGCD, out IAction act)
    {
        //Esuna
        if (DataCenter.IsEsunaStanceNorth && DataCenter.WeakenPeople.Any() || DataCenter.DyingPeople.Any())
        {
            if (WardensPaean.CanUse(out act, CanUseOption.MustUse)) return true;
        }
        return base.EmergencyAbility(nextGCD, out act);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="act"></param>
    /// <returns></returns>
    [RotationDesc(ActionID.Troubadour)]
    protected sealed override bool DefenseAreaAbility(out IAction act)
    {
        if (Troubadour.CanUse(out act)) return true;
        return false;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="act"></param>
    /// <returns></returns>
    [RotationDesc(ActionID.NaturesMinne)]
    protected sealed override bool HealSingleAbility(out IAction act)
    {
        if (NaturesMinne.CanUse(out act)) return true;
        return base.HealSingleAbility(out act);
    }
}
