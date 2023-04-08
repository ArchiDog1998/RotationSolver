namespace RotationSolver.Basic.Rotations.Basic;

public abstract partial class BLM_Base : CustomRotation
{
    private static BLMGauge JobGauge => Service.JobGauges.Get<BLMGauge>();

    public override MedicineType MedicineType => MedicineType.Intelligence;

    protected static byte UmbralIceStacks => JobGauge.UmbralIceStacks;

    protected static byte AstralFireStacks => JobGauge.AstralFireStacks;

    protected static byte PolyglotStacks => JobGauge.PolyglotStacks;

    protected static byte UmbralHearts => JobGauge.UmbralHearts;

    protected static bool IsParadoxActive => JobGauge.IsParadoxActive;

    protected static bool InUmbralIce => JobGauge.InUmbralIce;

    protected static bool InAstralFire => JobGauge.InAstralFire;

    protected static bool IsEnochianActive => JobGauge.IsEnochianActive;

    protected static bool EnchinaEndAfter(float time)
    {
        return EndAfter(JobGauge.EnochianTimer / 1000f, time);
    }

    protected static bool EnchinaEndAfterGCD(uint gctCount = 0, uint abilityCount = 0)
    {
        return EndAfterGCD(JobGauge.EnochianTimer / 1000f, gctCount, abilityCount);
    }

    protected static bool ElementTimeEndAfter(float time)
    {
        return EndAfter(JobGauge.ElementTimeRemaining / 1000f, time);
    }

    protected static bool ElementTimeEndAfterGCD(uint gctCount = 0, uint abilityCount = 0)
    {
        return EndAfterGCD(JobGauge.ElementTimeRemaining / 1000f, gctCount, abilityCount);
    }

    public sealed override ClassJobID[] JobIDs => new ClassJobID[] { ClassJobID.BlackMage, ClassJobID.Thaumaturge };

    protected static bool HasFire => Player.HasStatus(true, StatusID.Firestarter);

    protected static bool HasThunder => Player.HasStatus(true, StatusID.Thundercloud);

    protected static bool IsPolyglotStacksMaxed => Xenoglossy.EnoughLevel ? JobGauge.PolyglotStacks == 2 : JobGauge.PolyglotStacks == 1;

    public class ThunderAction : BaseAction
    {
        public override uint MPNeed => HasThunder ? 0 : base.MPNeed;

        internal ThunderAction(ActionID actionID)
            : base(actionID, false, false, true)
        {
        }
    }

    public class Fire3Action : BaseAction
    {
        public override uint MPNeed => HasFire ? 0 : base.MPNeed;

        internal Fire3Action(ActionID actionID)
            : base(actionID, false, false, false)
        {
        }
    }

    public class ElementAction : BaseAction
    {
        internal ElementAction(ActionID actionID, bool isFriendly = false, bool shouldEndSpecial = false, bool isEot = false) : base(actionID, isFriendly, shouldEndSpecial, isEot)
        {
        }

        public override bool CanUse(out IAction act, CanUseOption option = CanUseOption.None, byte gcdCountForAbility = 0)
        {
            if (ElementTimeEndAfter(CastTime - 0.1f))
            {
                act = null;
                return false;
            }
            return base.CanUse(out act, option, gcdCountForAbility);
        }
    }

    public static IBaseAction Thunder { get; } = new ThunderAction(ActionID.Thunder);

    public static IBaseAction Thunder2 { get; } = new ThunderAction(ActionID.Thunder2);

    public static IBaseAction Transpose { get; } = new BaseAction(ActionID.Transpose) { ActionCheck = b => DataCenter.AbilityRemain.IsLessThan(JobGauge.ElementTimeRemaining / 1000f) };

    public static IBaseAction UmbralSoul { get; } = new BaseAction(ActionID.UmbralSoul) { ActionCheck = b => JobGauge.InUmbralIce && Transpose.ActionCheck(b) };

    public static IBaseAction Manaward { get; } = new BaseAction(ActionID.Manaward, true, isTimeline: true);

    public static IBaseAction Manafont { get; } = new BaseAction(ActionID.Manafont)
    {
        ActionCheck = b => Player.CurrentMp <= 7000,
    };

    public static IBaseAction SharpCast { get; } = new BaseAction(ActionID.SharpCast)
    {
        StatusProvide = new[] { StatusID.SharpCast },
        ActionCheck = b => HasHostilesInRange,
    };

    public static IBaseAction TripleCast { get; } = new BaseAction(ActionID.TripleCast)
    {
        StatusProvide = Swiftcast.StatusProvide,
    };

    public static IBaseAction LeyLines { get; } = new BaseAction(ActionID.LeyLines, true, shouldEndSpecial: true)
    {
        StatusProvide = new[] { StatusID.LeyLines, },
    };

    public static IBaseAction BetweenTheLines { get; } = new BaseAction(ActionID.BetweenTheLines, true, shouldEndSpecial: true)
    {
        StatusNeed = LeyLines.StatusProvide,
    };

    public static IBaseAction AetherialManipulation { get; } = new BaseAction(ActionID.AetherialManipulation, true)
    {
        ChoiceTarget = TargetFilter.FindTargetForMoving,
    };

    public static IBaseAction Amplifier { get; } = new BaseAction(ActionID.Amplifier) { ActionCheck = b => JobGauge.EnochianTimer > 10000 && JobGauge.PolyglotStacks < 2 };

    public static IBaseAction Flare { get; } = new ElementAction(ActionID.Flare) { ActionCheck = b => JobGauge.InAstralFire };

    public static IBaseAction Despair { get; } = new ElementAction(ActionID.Despair) { ActionCheck = b => JobGauge.InAstralFire };

    public static IBaseAction Foul { get; } = new BaseAction(ActionID.Foul) { ActionCheck = b => JobGauge.PolyglotStacks != 0 };

    public static IBaseAction Xenoglossy { get; } = new BaseAction(ActionID.Xenoglossy) { ActionCheck = b => JobGauge.PolyglotStacks != 0 };

    public static IBaseAction Scathe { get; } = new BaseAction(ActionID.Scathe);

    public static IBaseAction Paradox { get; } = new BaseAction(ActionID.Paradox)
    {
        ActionCheck = b => JobGauge.IsParadoxActive,
    };

    public static IBaseAction Fire { get; } = new BaseAction(ActionID.Fire);

    public static IBaseAction Fire2 { get; } = new BaseAction(ActionID.Fire2);

    public static IBaseAction Fire3 { get; } = new Fire3Action(ActionID.Fire3)
    {
        ActionCheck = b => !IsLastGCD(ActionID.Fire3),
    };

    public static IBaseAction Fire4 { get; } = new ElementAction(ActionID.Fire4)
    {
        ActionCheck = b => JobGauge.InAstralFire,
    };

    public static IBaseAction Blizzard { get; } = new BaseAction(ActionID.Blizzard);

    public static IBaseAction Blizzard2 { get; } = new BaseAction(ActionID.Blizzard2);

    public static IBaseAction Blizzard3 { get; } = new BaseAction(ActionID.Blizzard3)
    {
        ActionCheck = b => !IsLastGCD(ActionID.Blizzard3),
    };

    public static IBaseAction Blizzard4 { get; } = new ElementAction(ActionID.Blizzard4);

    public static IBaseAction Freeze { get; } = new ElementAction(ActionID.Freeze);

    public static float Fire4Time { get; private set; }
    protected override void UpdateInfo()
    {
        if (Player.CastActionId == (uint)ActionID.Fire4 && Player.CurrentCastTime < 0.2)
        {
            Fire4Time = Player.TotalCastTime;
        }
        base.UpdateInfo();
    }

    [RotationDesc(ActionID.Manaward)]
    protected sealed override bool DefenseSingleGCD(out IAction act)
    {
        if (Manaward.CanUse(out act)) return true;
        return base.DefenseSingleGCD(out act);
    }

    [RotationDesc(ActionID.Addle)]
    protected override bool DefenseAreaAbility(byte abilitiesRemaining, out IAction act)
    {
        if (Addle.CanUse(out act)) return true;
        return false;
    }

    [RotationDesc(ActionID.AetherialManipulation)]
    protected sealed override bool MoveForwardGCD(out IAction act)
    {
        if (AetherialManipulation.CanUse(out act, CanUseOption.MustUse)) return true;
        return base.MoveForwardGCD(out act);
    }
}
