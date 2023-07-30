using ECommons.DalamudServices;
using ECommons.ExcelServices;
using RotationSolver.Basic.Traits;

namespace RotationSolver.Basic.Rotations.Basic;

/// <summary>
/// The <see href="https://na.finalfantasyxiv.com/jobguide/blackmage/">Black Mage</see>
/// </summary>
public abstract partial class BLM_Base : CustomRotation
{
    /// <summary>
    /// 
    /// </summary>
    public override MedicineType MedicineType => MedicineType.Intelligence;

    /// <summary>
    /// 
    /// </summary>
    public sealed override Job[] Jobs => new Job[] { Job.BLM, Job.THM };

    #region Job Gauge
    static BLMGauge JobGauge => Svc.Gauges.Get<BLMGauge>();

    /// <summary>
    /// 
    /// </summary>
    protected static byte UmbralIceStacks => JobGauge.UmbralIceStacks;

    /// <summary>
    /// 
    /// </summary>
    protected static byte AstralFireStacks => JobGauge.AstralFireStacks;

    /// <summary>
    /// 
    /// </summary>
    protected static byte PolyglotStacks => JobGauge.PolyglotStacks;

    /// <summary>
    /// 
    /// </summary>
    protected static byte UmbralHearts => JobGauge.UmbralHearts;

    /// <summary>
    /// 
    /// </summary>
    protected static bool IsParadoxActive => JobGauge.IsParadoxActive;

    /// <summary>
    /// 
    /// </summary>
    protected static bool InUmbralIce => JobGauge.InUmbralIce;

    /// <summary>
    /// 
    /// </summary>
    protected static bool InAstralFire => JobGauge.InAstralFire;

    /// <summary>
    /// 
    /// </summary>
    protected static bool IsEnochianActive => JobGauge.IsEnochianActive;

    /// <summary>
    /// 
    /// </summary>
    protected static bool IsPolyglotStacksMaxed => Xenoglossy.EnoughLevel ? PolyglotStacks == 2 : PolyglotStacks == 1;

    static float EnochianTimeRaw => JobGauge.EnochianTimer / 1000f;

    /// <summary>
    /// 
    /// </summary>
    protected static float EnochianTime => EnochianTimeRaw - DataCenter.WeaponRemain;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    protected static bool EnchinaEndAfter(float time) => EnochianTime <= time;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="gcdCount"></param>
    /// <param name="offset"></param>
    /// <returns></returns>
    protected static bool EnchinaEndAfterGCD(uint gcdCount = 0, float offset = 0)
        => EnchinaEndAfter(GCDTime(gcdCount, offset));

    static float ElementTimeRaw => JobGauge.ElementTimeRemaining / 1000f;

    /// <summary>
    /// 
    /// </summary>
    protected static float ElementTime => ElementTimeRaw - DataCenter.WeaponRemain;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    protected static bool ElementTimeEndAfter(float time) => ElementTime <= time;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="gctCount"></param>
    /// <param name="offset"></param>
    /// <returns></returns>
    protected static bool ElementTimeEndAfterGCD(uint gctCount = 0, float offset = 0)
        => ElementTimeEndAfter(GCDTime(gctCount, offset));


    #endregion
    /// <summary>
    /// 
    /// </summary>
    protected static bool HasFire => Player.HasStatus(true, StatusID.Firestarter);

    /// <summary>
    /// 
    /// </summary>
    protected static bool HasThunder => Player.HasStatus(true, StatusID.Thundercloud);

    /// <summary>
    /// The thunder action for the MP check.
    /// </summary>
    public class ThunderAction : BaseAction
    {
        /// <summary>
        /// Changed the mp.
        /// </summary>
        public override uint MPNeed => HasThunder ? 0 : base.MPNeed;

        internal ThunderAction(ActionID actionID)
            : base(actionID)
        {
        }
    }

    /// <summary>
    /// The fire3 action for the mp check.
    /// </summary>
    public class Fire3Action : BaseAction
    {
        /// <summary>
        /// Changed the mp.
        /// </summary>
        public override uint MPNeed => HasFire ? 0 : base.MPNeed;

        internal Fire3Action(ActionID actionID)
            : base(actionID)
        {
        }
    }

    /// <summary>
    /// The action that needs element time.
    /// </summary>
    public class ElementAction : BaseAction
    {
        internal ElementAction(ActionID actionID) : base(actionID)
        {
        }

        /// <summary>
        /// Can this action be used.
        /// </summary>
        /// <param name="act"></param>
        /// <param name="option"></param>
        /// <param name="gcdCountForAbility"></param>
        /// <returns></returns>
        public override bool CanUse(out IAction act, CanUseOption option = CanUseOption.None, byte aoeCount = 0, byte gcdCountForAbility = 0)
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
    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction Fire { get; } = new BaseAction(ActionID.Fire);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction Fire3 { get; } = new Fire3Action(ActionID.Fire3)
    {
        ActionCheck = (b, m) => !IsLastGCD(ActionID.Fire3),
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction Fire4 { get; } = new ElementAction(ActionID.Fire4)
    {
        ActionCheck = (b, m) => InAstralFire && !ElementTimeEndAfter(Fire4.CastTime),
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction Despair { get; } = new ElementAction(ActionID.Despair)
    {
        ActionCheck = (b, m) => InAstralFire && !ElementTimeEndAfter(Despair.CastTime),
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction Blizzard { get; } = new BaseAction(ActionID.Blizzard);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction Blizzard3 { get; } = new BaseAction(ActionID.Blizzard3)
    {
        ActionCheck = (b, m) => !IsLastGCD(ActionID.Blizzard3),
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction Blizzard4 { get; } = new ElementAction(ActionID.Blizzard4)
    {
        ActionCheck = (b, m) => InUmbralIce && !ElementTimeEndAfter(Blizzard4.CastTime),
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction Thunder { get; } = new ThunderAction(ActionID.Thunder);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction Xenoglossy { get; } = new BaseAction(ActionID.Xenoglossy) 
    { 
        ActionCheck = (b, m) => PolyglotStacks > 0 
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction Paradox { get; } = new BaseAction(ActionID.Paradox)
    {
        ActionCheck = (b, m) => IsParadoxActive,
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction Scathe { get; } = new BaseAction(ActionID.Scathe);
    #endregion

    #region Attack Area
    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction Fire2 { get; } = new BaseAction(ActionID.Fire2);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction Flare { get; } = new ElementAction(ActionID.Flare)
    {
        ActionCheck = (b, m) => InAstralFire && !ElementTimeEndAfter(Flare.CastTime),
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction Blizzard2 { get; } = new BaseAction(ActionID.Blizzard2);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction Freeze { get; } = new ElementAction(ActionID.Freeze);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction Thunder2 { get; } = new ThunderAction(ActionID.Thunder2);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction Foul { get; } = new BaseAction(ActionID.Foul) 
    { 
        ActionCheck = Xenoglossy.ActionCheck,
    };
    #endregion

    #region Support
    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction AetherialManipulation { get; } = new BaseAction(ActionID.AetherialManipulation, ActionOption.Friendly | ActionOption.EndSpecial)
    {
        ChoiceTarget = TargetFilter.FindTargetForMoving,
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction Amplifier { get; } = new BaseAction(ActionID.Amplifier) 
    { 
        ActionCheck = (b, m) => !EnchinaEndAfter(10) && PolyglotStacks < 2 
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction Manaward { get; } = new BaseAction(ActionID.Manaward, ActionOption.Defense);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction Manafont { get; } = new BaseAction(ActionID.Manafont)
    {
        ActionCheck = (b, m) => Player.CurrentMp <= 7000,
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction LeyLines { get; } = new BaseAction(ActionID.LeyLines, ActionOption.Buff | ActionOption.EndSpecial)
    {
        StatusProvide = new[] { StatusID.LeyLines, },
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction BetweenTheLines { get; } = new BaseAction(ActionID.BetweenTheLines, ActionOption.Buff | ActionOption.EndSpecial)
    {
        StatusNeed = LeyLines.StatusProvide,
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction SharpCast { get; } = new BaseAction(ActionID.SharpCast)
    {
        StatusProvide = new[] { StatusID.SharpCast },
        ActionCheck = (b, m) => HasHostilesInRange,
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction TripleCast { get; } = new BaseAction(ActionID.TripleCast)
    {
        StatusProvide = Swiftcast.StatusProvide,
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction Transpose { get; } = new BaseAction(ActionID.Transpose) 
    { 
        ActionCheck = (b, m) => DataCenter.ActionRemain <= ElementTimeRaw,
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction UmbralSoul { get; } = new BaseAction(ActionID.UmbralSoul) 
    { 
        ActionCheck = (b, m) => JobGauge.InUmbralIce && Transpose.ActionCheck(b, m) 
    };
    #endregion

    #region Traits
    /// <summary>
    /// 
    /// </summary>
    protected static IBaseTrait MaimAndMend { get; } = new BaseTrait(29);

    /// <summary>
    /// 
    /// </summary>
    protected static IBaseTrait MaimAndMend2 { get; } = new BaseTrait(31);

    /// <summary>
    /// 
    /// </summary>
    protected static IBaseTrait Firestarter { get; } = new BaseTrait(32);

    /// <summary>
    /// 
    /// </summary>
    protected static IBaseTrait Thundercloud { get; } = new BaseTrait(33);

    /// <summary>
    /// 
    /// </summary>
    protected static IBaseTrait ThunderMastery { get; } = new BaseTrait(171);

    /// <summary>
    /// 
    /// </summary>
    protected static IBaseTrait ThunderMastery2 { get; } = new BaseTrait(172);

    /// <summary>
    /// 
    /// </summary>
    protected static IBaseTrait EnhancedEnochian {get; } = new BaseTrait(174);

    /// <summary>
    /// 
    /// </summary>
    protected static IBaseTrait EnhancedFreeze {get; } = new BaseTrait(295);

    /// <summary>
    /// 
    /// </summary>
    protected static IBaseTrait AspectMastery {get; } = new BaseTrait(296);

    /// <summary>
    /// 
    /// </summary>
    protected static IBaseTrait EnhancedPolyglot {get; } = new BaseTrait(297);

    /// <summary>
    /// 
    /// </summary>
    protected static IBaseTrait EnhancedSharpcast { get; } = new BaseTrait(321);

    /// <summary>
    /// 
    /// </summary>
    protected static IBaseTrait EnhancedEnochian2 { get; } = new BaseTrait(322);

    /// <summary>
    /// 
    /// </summary>
    protected static IBaseTrait AspectMastery2 { get; } = new BaseTrait(458);

    /// <summary>
    /// 
    /// </summary>
    protected static IBaseTrait AspectMastery3 { get; } = new BaseTrait(459);

    /// <summary>
    /// 
    /// </summary>
    protected static IBaseTrait Enochian { get; } = new BaseTrait(460);

    /// <summary>
    /// 
    /// </summary>
    protected static IBaseTrait EnhancedFoul { get; } = new BaseTrait(461);

    /// <summary>
    /// 
    /// </summary>
    protected static IBaseTrait AspectMastery4 { get; } = new BaseTrait(462);

    /// <summary>
    /// 
    /// </summary>
    protected static IBaseTrait EnhancedManafont { get; } = new BaseTrait(463);

    /// <summary>
    /// 
    /// </summary>
    protected static IBaseTrait EnhancedSharpcast2    { get; } = new BaseTrait(464);

    /// <summary>
    /// 
    /// </summary>
    protected static IBaseTrait AspectMastery5 { get; } = new BaseTrait(465);

    /// <summary>
    /// 
    /// </summary>
    protected static IBaseTrait EnhancedEnochian3 { get; } = new BaseTrait(509);

    #endregion
    /// <summary>
    /// 
    /// </summary>
    protected static float Fire4Time { get; private set; }

    /// <summary>
    /// 
    /// </summary>
    protected override void UpdateInfo()
    {
        if (Player.CastActionId == (uint)ActionID.Fire4 && Player.CurrentCastTime < 0.2)
        {
            Fire4Time = Player.TotalCastTime;
        }
        base.UpdateInfo();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="act"></param>
    /// <returns></returns>
    [RotationDesc(ActionID.Manaward)]
    protected sealed override bool DefenseSingleGCD(out IAction act)
    {
        if (Manaward.CanUse(out act)) return true;
        return base.DefenseSingleGCD(out act);
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
        return false;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="act"></param>
    /// <returns></returns>
    [RotationDesc(ActionID.AetherialManipulation)]
    protected sealed override bool MoveForwardGCD(out IAction act)
    {
        if (AetherialManipulation.CanUse(out act, CanUseOption.MustUse)) return true;
        return base.MoveForwardGCD(out act);
    }
}
