using Dalamud.Game.ClientState.JobGauge.Enums;
using Dalamud.Game.ClientState.JobGauge.Types;
using RotationSolver.Actions;
using RotationSolver.Actions.BaseAction;
using RotationSolver.Attributes;
using RotationSolver.Data;
using RotationSolver.Helpers;
using RotationSolver.Rotations.CustomRotation;

namespace RotationSolver.Rotations.Basic;

internal abstract class MNK_Base : CustomRotation.CustomRotation
{
    private static MNKGauge JobGauge => Service.JobGauges.Get<MNKGauge>();

    public override MedicineType MedicineType => MedicineType.Strength;


    /// <summary>
    /// 查克拉们
    /// </summary>
    protected static BeastChakra[] BeastChakras => JobGauge.BeastChakra;

    /// <summary>
    /// 查克拉数量
    /// </summary>
    protected static byte Chakra => JobGauge.Chakra;

    protected static bool HasSolar => (JobGauge.Nadi & Nadi.SOLAR) != 0;
    protected static bool HasLunar => (JobGauge.Nadi & Nadi.LUNAR) != 0;

    public sealed override ClassJobID[] JobIDs => new ClassJobID[] { ClassJobID.Monk, ClassJobID.Pugilist };

    /// <summary>
    /// 双龙脚
    /// </summary>
    public static IBaseAction DragonKick { get; } = new BaseAction(ActionID.DragonKick)
    {
        StatusProvide = new[] { StatusID.LeadenFist },
    };

    /// <summary>
    /// 连击
    /// </summary>
    public static IBaseAction Bootshine { get; } = new BaseAction(ActionID.Bootshine);

    /// <summary>
    /// 破坏神冲 aoe
    /// </summary>
    public static IBaseAction ArmoftheDestroyer { get; } = new BaseAction(ActionID.ArmoftheDestroyer);

    /// <summary>
    /// 破坏神脚 aoe
    /// </summary>
    public static IBaseAction ShadowoftheDestroyer { get; } = new BaseAction(ActionID.ShadowoftheDestroyer);

    /// <summary>
    /// 双掌打 伤害提高
    /// </summary>
    public static IBaseAction TwinSnakes { get; } = new BaseAction(ActionID.TwinSnakes, isEot: true);

    /// <summary>
    /// 正拳
    /// </summary>
    public static IBaseAction TrueStrike { get; } = new BaseAction(ActionID.TrueStrike);

    /// <summary>
    /// 四面脚 aoe
    /// </summary>
    public static IBaseAction FourpointFury { get; } = new BaseAction(ActionID.FourpointFury);

    /// <summary>
    /// 破碎拳
    /// </summary>
    public static IBaseAction Demolish { get; } = new BaseAction(ActionID.Demolish, isEot: true)
    {
        TargetStatus = new StatusID[] { StatusID.Demolish },
        GetDotGcdCount = () => 2,
    };

    /// <summary>
    /// 崩拳
    /// </summary>
    public static IBaseAction SnapPunch { get; } = new BaseAction(ActionID.SnapPunch);

    /// <summary>
    /// 地烈劲 aoe
    /// </summary>
    public static IBaseAction Rockbreaker { get; } = new BaseAction(ActionID.Rockbreaker);

    /// <summary>
    /// 斗气
    /// </summary>
    public static IBaseAction Meditation { get; } = new BaseAction(ActionID.Meditation, true);

    /// <summary>
    /// 铁山靠
    /// </summary>
    public static IBaseAction SteelPeak { get; } = new BaseAction(ActionID.SteelPeak)
    {
        ActionCheck = b => InCombat && Chakra == 5,
    };

    /// <summary>
    /// 空鸣拳
    /// </summary>
    public static IBaseAction HowlingFist { get; } = new BaseAction(ActionID.HowlingFist)
    {
        ActionCheck = SteelPeak.ActionCheck,
    };

    /// <summary>
    /// 义结金兰
    /// </summary>
    public static IBaseAction Brotherhood { get; } = new BaseAction(ActionID.Brotherhood, true);

    /// <summary>
    /// 红莲极意 提高dps
    /// </summary>
    public static IBaseAction RiddleofFire { get; } = new BaseAction(ActionID.RiddleofFire, true);

    /// <summary>
    /// 突进技能
    /// </summary>
    public static IBaseAction Thunderclap { get; } = new BaseAction(ActionID.Thunderclap, shouldEndSpecial: true)
    {
        ChoiceTarget = TargetFilter.FindTargetForMoving,
    };

    /// <summary>
    /// 真言
    /// </summary>
    public static IBaseAction Mantra { get; } = new BaseAction(ActionID.Mantra, true, isTimeline: true);

    /// <summary>
    /// 震脚
    /// </summary>
    public static IBaseAction PerfectBalance { get; } = new BaseAction(ActionID.PerfectBalance)
    {
        ActionCheck = b => InCombat,
    };

    /// <summary>
    /// 苍气炮 阴
    /// </summary>
    public static IBaseAction ElixirField { get; } = new BaseAction(ActionID.ElixirField);

    /// <summary>
    /// 爆裂脚 阳
    /// </summary>
    public static IBaseAction FlintStrike { get; } = new BaseAction(ActionID.FlintStrike);

    /// <summary>
    /// 翻天脚 兔
    /// </summary>
    public static IBaseAction CelestialRevolution { get; } = new BaseAction(ActionID.CelestialRevolution);

    /// <summary>
    /// 凤凰舞
    /// </summary>
    public static IBaseAction RisingPhoenix { get; } = new BaseAction(ActionID.RisingPhoenix);

    /// <summary>
    /// 斗魂旋风脚 阴阳
    /// </summary>
    public static IBaseAction TornadoKick { get; } = new BaseAction(ActionID.TornadoKick);
    public static IBaseAction PhantomRush { get; } = new BaseAction(ActionID.PhantomRush);

    /// <summary>
    /// 演武
    /// </summary>
    public static IBaseAction FormShift { get; } = new BaseAction(ActionID.FormShift, true)
    {
        StatusProvide = new[] { StatusID.FormlessFist, StatusID.PerfectBalance },
    };

    /// <summary>
    /// 金刚极意 盾
    /// </summary>
    public static IBaseAction RiddleofEarth { get; } = new BaseAction(ActionID.RiddleofEarth, true, shouldEndSpecial: true, isTimeline: true)
    {
        StatusProvide = new[] { StatusID.RiddleofEarth },
    };

    /// <summary>
    /// 疾风极意
    /// </summary>
    public static IBaseAction RiddleofWind { get; } = new BaseAction(ActionID.RiddleofWind, true);

    [RotationDesc(ActionID.Thunderclap)]
    private protected sealed override bool MoveForwardAbility(byte abilitiesRemaining, out IAction act, bool recordTarget = true)
    {
        if (Thunderclap.CanUse(out act, emptyOrSkipCombo: true, recordTarget: recordTarget)) return true;
        return false;
    }

    [RotationDesc(ActionID.Feint)]
    private protected sealed override bool DefenceAreaAbility(byte abilitiesRemaining, out IAction act)
    {
        if (Feint.CanUse(out act)) return true;
        return false;
    }

    [RotationDesc(ActionID.Mantra)]
    private protected sealed override bool HealAreaAbility(byte abilitiesRemaining, out IAction act)
    {
        if (Mantra.CanUse(out act)) return true;
        return false;
    }

    [RotationDesc(ActionID.RiddleofEarth)]
    private protected sealed override bool DefenceSingleAbility(byte abilitiesRemaining, out IAction act)
    {
        if (RiddleofEarth.CanUse(out act, emptyOrSkipCombo: true)) return true;
        return false;
    }
}
