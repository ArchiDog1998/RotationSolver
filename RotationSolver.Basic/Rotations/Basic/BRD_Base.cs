namespace RotationSolver.Basic.Rotations.Basic;

public abstract class BRD_Base : CustomRotation
{
    private static BRDGauge JobGauge => Service.JobGauges.Get<BRDGauge>();

    public override MedicineType MedicineType => MedicineType.Dexterity;

    protected static byte Repertoire => JobGauge.Repertoire;

    protected static Song Song => JobGauge.Song;

    protected static Song LastSong => JobGauge.LastSong;

    protected static byte SoulVoice => JobGauge.SoulVoice;

    protected static bool SongEndAfter(float time) => EndAfter(SongTime, time);

    protected static bool SongEndAfterGCD(uint gctCount = 0, int abilityCount = 0)
        => EndAfterGCD(SongTime, gctCount, abilityCount);

    private static float SongTime => JobGauge.SongTimer / 1000f;

    public sealed override ClassJobID[] JobIDs => new[] { ClassJobID.Bard, ClassJobID.Archer };

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
        ActionCheck = b => b.HasStatus(true, VenomousBite.TargetStatus) & b.HasStatus(true, WindBite.TargetStatus),
    };

    public static IBaseAction WanderersMinuet { get; } = new BaseAction(ActionID.WanderersMinuet);

    public static IBaseAction MagesBallad { get; } = new BaseAction(ActionID.MagesBallad);

    public static IBaseAction ArmysPaeon { get; } = new BaseAction(ActionID.ArmysPaeon);

    public static IBaseAction BattleVoice { get; } = new BaseAction(ActionID.BattleVoice, ActionOption.Buff);

    public static IBaseAction RagingStrikes { get; } = new BaseAction(ActionID.RagingStrikes, ActionOption.Buff);

    public static IBaseAction RadiantFinale { get; } = new BaseAction(ActionID.RadiantFinale, ActionOption.Buff)
    {
        ActionCheck = b => JobGauge.Coda.Any(s => s != Song.NONE),
    };

    public static IBaseAction Barrage { get; } = new BaseAction(ActionID.Barrage);

    public static IBaseAction EmpyrealArrow { get; } = new BaseAction(ActionID.EmpyrealArrow);

    public static IBaseAction PitchPerfect { get; } = new BaseAction(ActionID.PitchPerfect)
    {
        ActionCheck = b => JobGauge.Song == Song.WANDERER,
    };

    public static IBaseAction Bloodletter { get; } = new BaseAction(ActionID.Bloodletter);

    public static IBaseAction RainOfDeath { get; } = new BaseAction(ActionID.RainOfDeath)
    {
        AOECount = 2,
    };

    public static IBaseAction QuickNock { get; } = new BaseAction(ActionID.QuickNock)
    {
        StatusProvide = new[] { StatusID.ShadowBiteReady }
    };

    public static IBaseAction ShadowBite { get; } = new BaseAction(ActionID.ShadowBite)
    {
        StatusNeed = new[] { StatusID.ShadowBiteReady }
    };

    public static IBaseAction WardensPaean { get; } = new BaseAction(ActionID.WardensPaean, ActionOption.Heal);

    public static IBaseAction NaturesMinne { get; } = new BaseAction(ActionID.NaturesMinne, ActionOption.Heal);

    public static IBaseAction Sidewinder { get; } = new BaseAction(ActionID.Sidewinder);

    public static IBaseAction ApexArrow { get; } = new BaseAction(ActionID.ApexArrow)
    {
        ActionCheck = b => JobGauge.SoulVoice >= 20 && !Player.HasStatus(true, StatusID.BlastArrowReady),
    };

    public static IBaseAction BlastArrow { get; } = new BaseAction(ActionID.BlastArrow)
    {
        ActionCheck = b => Player.HasStatus(true, StatusID.BlastArrowReady),
    };

    public static IBaseAction Troubadour { get; } = new BaseAction(ActionID.Troubadour, ActionOption.Defense)
    {
        ActionCheck = b => !Player.HasStatus(false, StatusID.Troubadour,
            StatusID.Tactician1,
            StatusID.Tactician2,
            StatusID.ShieldSamba),
    };

    protected override bool EmergencyAbility(float nextAbilityToNextGCD, IAction nextGCD, out IAction act)
    {
        //Esuna
        if (DataCenter.SpecialType == SpecialCommandType.EsunaStanceNorth && DataCenter.WeakenPeople.Any() || DataCenter.DyingPeople.Any())
        {
            if (WardensPaean.CanUse(out act, CanUseOption.MustUse)) return true;
        }
        return base.EmergencyAbility(nextAbilityToNextGCD, nextGCD, out act);
    }

    [RotationDesc(ActionID.Troubadour)]
    protected sealed override bool DefenseAreaAbility(float nextAbilityToNextGCD, out IAction act)
    {
        if (Troubadour.CanUse(out act)) return true;
        return false;
    }

    [RotationDesc(ActionID.NaturesMinne)]
    protected sealed override bool HealSingleAbility(float nextAbilityToNextGCD, out IAction act)
    {
        if (NaturesMinne.CanUse(out act)) return true;
        return false;
    }
}
