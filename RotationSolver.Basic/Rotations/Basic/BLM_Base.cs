namespace RotationSolver.Basic.Rotations.Basic;

public abstract partial class BLM_Base : CustomRotation
{
    public override MedicineType MedicineType => MedicineType.Intelligence;

    public sealed override ClassJobID[] JobIDs => new ClassJobID[] { ClassJobID.BlackMage, ClassJobID.Thaumaturge };

    #region Job Gauge
    static BLMGauge JobGauge => Service.JobGauges.Get<BLMGauge>();

    protected static byte UmbralIceStacks => JobGauge.UmbralIceStacks;

    protected static byte AstralFireStacks => JobGauge.AstralFireStacks;

    protected static byte PolyglotStacks => JobGauge.PolyglotStacks;

    protected static byte UmbralHearts => JobGauge.UmbralHearts;

    protected static bool IsParadoxActive => JobGauge.IsParadoxActive;

    protected static bool InUmbralIce => JobGauge.InUmbralIce;

    protected static bool InAstralFire => JobGauge.InAstralFire;

    protected static bool IsEnochianActive => JobGauge.IsEnochianActive;

    protected static bool IsPolyglotStacksMaxed => Xenoglossy.EnoughLevel ? PolyglotStacks == 2 : PolyglotStacks == 1;

    static float ElementTime => JobGauge.EnochianTimer / 1000f;

    protected static bool EnchinaEndAfter(float time)
    {
        return EndAfter(JobGauge.EnochianTimer / 1000f, time);
    }

    protected static bool EnchinaEndAfterGCD(uint gctCount = 0, float offset = 0)
    {
        return EndAfterGCD(JobGauge.EnochianTimer / 1000f, gctCount, offset);
    }

    protected static bool ElementTimeEndAfter(float time)
    {
        return EndAfter(ElementTime, time);
    }

    protected static bool ElementTimeEndAfterGCD(uint gctCount = 0, float offset = 0)
    {
        return EndAfterGCD(ElementTime, gctCount, offset);
    }

    #endregion
    protected static bool HasFire => Player.HasStatus(true, StatusID.Firestarter);

    protected static bool HasThunder => Player.HasStatus(true, StatusID.Thundercloud);

    public class ThunderAction : BaseAction
    {
        public override uint MPNeed => HasThunder ? 0 : base.MPNeed;

        internal ThunderAction(ActionID actionID)
            : base(actionID)
        {
        }
    }

    public class Fire3Action : BaseAction
    {
        public override uint MPNeed => HasFire ? 0 : base.MPNeed;

        internal Fire3Action(ActionID actionID)
            : base(actionID)
        {
        }
    }

    public class ElementAction : BaseAction
    {
        internal ElementAction(ActionID actionID) : base(actionID)
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

    #region Attack Single
    public static IBaseAction Fire { get; } = new BaseAction(ActionID.Fire);

    public static IBaseAction Fire3 { get; } = new Fire3Action(ActionID.Fire3)
    {
        ActionCheck = b => !IsLastGCD(ActionID.Fire3),
    };
    public static IBaseAction Fire4 { get; } = new ElementAction(ActionID.Fire4)
    {
        ActionCheck = b => InAstralFire && !ElementTimeEndAfter(Fire4.CastTime),
    };

    public static IBaseAction Despair { get; } = new ElementAction(ActionID.Despair)
    {
        ActionCheck = b => InAstralFire && !ElementTimeEndAfter(Despair.CastTime),
    };

    public static IBaseAction Blizzard { get; } = new BaseAction(ActionID.Blizzard);

    public static IBaseAction Blizzard3 { get; } = new BaseAction(ActionID.Blizzard3)
    {
        ActionCheck = b => !IsLastGCD(ActionID.Blizzard3),
    };
    public static IBaseAction Blizzard4 { get; } = new ElementAction(ActionID.Blizzard4)
    {
        ActionCheck = b => InUmbralIce && !ElementTimeEndAfter(Blizzard4.CastTime),
    };

    public static IBaseAction Thunder { get; } = new ThunderAction(ActionID.Thunder);

    public static IBaseAction Xenoglossy { get; } = new BaseAction(ActionID.Xenoglossy) 
    { 
        ActionCheck = b => PolyglotStacks > 0 
    };

    public static IBaseAction Paradox { get; } = new BaseAction(ActionID.Paradox)
    {
        ActionCheck = b => IsParadoxActive,
    };
    public static IBaseAction Scathe { get; } = new BaseAction(ActionID.Scathe);
    #endregion

    #region Attack Area
    public static IBaseAction Fire2 { get; } = new BaseAction(ActionID.Fire2);

    public static IBaseAction Flare { get; } = new ElementAction(ActionID.Flare)
    {
        ActionCheck = b => InAstralFire && !ElementTimeEndAfter(Flare.CastTime),
    };

    public static IBaseAction Blizzard2 { get; } = new BaseAction(ActionID.Blizzard2);

    public static IBaseAction Freeze { get; } = new ElementAction(ActionID.Freeze);

    public static IBaseAction Thunder2 { get; } = new ThunderAction(ActionID.Thunder2);

    public static IBaseAction Foul { get; } = new BaseAction(ActionID.Foul) 
    { 
        ActionCheck = Xenoglossy.ActionCheck,
    };
    #endregion

    #region Support
    public static IBaseAction AetherialManipulation { get; } = new BaseAction(ActionID.AetherialManipulation, ActionOption.Friendly)
    {
        ChoiceTarget = TargetFilter.FindTargetForMoving,
    };

    public static IBaseAction Amplifier { get; } = new BaseAction(ActionID.Amplifier) 
    { 
        ActionCheck = b => !EnchinaEndAfter(10) && PolyglotStacks < 2 
    };

    public static IBaseAction Manaward { get; } = new BaseAction(ActionID.Manaward, ActionOption.Defense);

    public static IBaseAction Manafont { get; } = new BaseAction(ActionID.Manafont)
    {
        ActionCheck = b => Player.CurrentMp <= 7000,
    };

    public static IBaseAction LeyLines { get; } = new BaseAction(ActionID.LeyLines, ActionOption.Buff | ActionOption.EndSpecial)
    {
        StatusProvide = new[] { StatusID.LeyLines, },
    };

    public static IBaseAction BetweenTheLines { get; } = new BaseAction(ActionID.BetweenTheLines, ActionOption.Buff | ActionOption.EndSpecial)
    {
        StatusNeed = LeyLines.StatusProvide,
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

    public static IBaseAction Transpose { get; } = new BaseAction(ActionID.Transpose) 
    { 
        ActionCheck = b => DataCenter.ActionRemain.IsLessThan(ElementTime) 
    };

    public static IBaseAction UmbralSoul { get; } = new BaseAction(ActionID.UmbralSoul) 
    { 
        ActionCheck = b => JobGauge.InUmbralIce && Transpose.ActionCheck(b) 
    };
    #endregion


    protected static float Fire4Time { get; private set; }
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
    protected override bool DefenseAreaAbility(out IAction act)
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
