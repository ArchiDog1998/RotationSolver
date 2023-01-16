using Dalamud.Game.ClientState.JobGauge.Enums;
using Dalamud.Game.ClientState.JobGauge.Types;
using System;
using XIVAutoAction;
using XIVAutoAction.Actions;
using XIVAutoAction.Data;
using XIVAutoAction.Helpers;
using XIVAutoAction.Actions.BaseAction;
using XIVAutoAction.Combos.CustomCombo;

namespace XIVAutoAction.Combos.Basic;

internal abstract class MNKCombo_Base : CustomCombo.CustomCombo
{
    private static MNKGauge JobGauge => Service.JobGauges.Get<MNKGauge>();

    /// <summary>
    /// 查克拉们
    /// </summary>
    protected static BeastChakra[] BeastChakras => JobGauge.BeastChakra;

    /// <summary>
    /// 查克拉数量
    /// </summary>
    protected static byte Chakra => JobGauge.Chakra;

    /// <summary>
    /// 阴阳必杀
    /// </summary>
    protected static Nadi Nadi => JobGauge.Nadi;

    public sealed override ClassJobID[] JobIDs => new ClassJobID[] { ClassJobID.Monk, ClassJobID.Pugilist };

    /// <summary>
    /// 双龙脚
    /// </summary>
    public static BaseAction DragonKick { get; } = new(ActionID.DragonKick)
    {
        BuffsProvide = new[] { StatusID.LeadenFist },
    };

    /// <summary>
    /// 连击
    /// </summary>
    public static BaseAction Bootshine { get; } = new(ActionID.Bootshine);

    /// <summary>
    /// 破坏神冲 aoe
    /// </summary>
    public static BaseAction ArmoftheDestroyer { get; } = new(ActionID.ArmoftheDestroyer);

    /// <summary>
    /// 破坏神脚 aoe
    /// </summary>
    public static BaseAction ShadowoftheDestroyer { get; } = new(ActionID.ShadowoftheDestroyer);

    /// <summary>
    /// 双掌打 伤害提高
    /// </summary>
    public static BaseAction TwinSnakes { get; } = new(ActionID.TwinSnakes, isEot: true);

    /// <summary>
    /// 正拳
    /// </summary>
    public static BaseAction TrueStrike { get; } = new(ActionID.TrueStrike);

    /// <summary>
    /// 四面脚 aoe
    /// </summary>
    public static BaseAction FourpointFury { get; } = new(ActionID.FourpointFury);

    /// <summary>
    /// 破碎拳
    /// </summary>
    public static BaseAction Demolish { get; } = new(ActionID.Demolish, isEot: true)
    {
        TargetStatus = new StatusID[] { StatusID.Demolish },
    };

    /// <summary>
    /// 崩拳
    /// </summary>
    public static BaseAction SnapPunch { get; } = new(ActionID.SnapPunch);

    /// <summary>
    /// 地烈劲 aoe
    /// </summary>
    public static BaseAction Rockbreaker { get; } = new(ActionID.Rockbreaker);

    /// <summary>
    /// 斗气
    /// </summary>
    public static BaseAction Meditation { get; } = new(ActionID.Meditation, true);

    /// <summary>
    /// 铁山靠
    /// </summary>
    public static BaseAction SteelPeak { get; } = new(ActionID.SteelPeak)
    {
        ActionCheck = b => InCombat && Chakra == 5,
    };

    /// <summary>
    /// 空鸣拳
    /// </summary>
    public static BaseAction HowlingFist { get; } = new(ActionID.HowlingFist)
    {
        ActionCheck = SteelPeak.ActionCheck,
    };

    /// <summary>
    /// 义结金兰
    /// </summary>
    public static BaseAction Brotherhood { get; } = new(ActionID.Brotherhood, true);

    /// <summary>
    /// 红莲极意 提高dps
    /// </summary>
    public static BaseAction RiddleofFire { get; } = new(ActionID.RiddleofFire, true);

    /// <summary>
    /// 突进技能
    /// </summary>
    public static BaseAction Thunderclap { get; } = new(ActionID.Thunderclap, shouldEndSpecial: true)
    {
        ChoiceTarget = TargetFilter.FindTargetForMoving,
    };

    /// <summary>
    /// 真言
    /// </summary>
    public static BaseAction Mantra { get; } = new(ActionID.Mantra, true, isTimeline: true);

    /// <summary>
    /// 震脚
    /// </summary>
    public static BaseAction PerfectBalance { get; } = new(ActionID.PerfectBalance)
    {
        BuffsNeed = new StatusID[] { StatusID.RaptorForm },
        ActionCheck = b => InCombat,
    };

    /// <summary>
    /// 苍气炮 阴
    /// </summary>
    public static BaseAction ElixirField { get; } = new(ActionID.ElixirField);

    /// <summary>
    /// 爆裂脚 阳
    /// </summary>
    public static BaseAction FlintStrike { get; } = new(ActionID.FlintStrike);

    /// <summary>
    /// 翻天脚 兔
    /// </summary>
    public static BaseAction CelestialRevolution { get; } = new(ActionID.CelestialRevolution);

    /// <summary>
    /// 凤凰舞
    /// </summary>
    public static BaseAction RisingPhoenix { get; } = new(ActionID.RisingPhoenix);

    /// <summary>
    /// 斗魂旋风脚 阴阳
    /// </summary>
    public static BaseAction TornadoKick { get; } = new(ActionID.TornadoKick);
    public static BaseAction PhantomRush { get; } = new(ActionID.PhantomRush);

    /// <summary>
    /// 演武
    /// </summary>
    public static BaseAction FormShift { get; } = new(ActionID.FormShift, true)
    {
        BuffsProvide = new[] { StatusID.FormlessFist, StatusID.PerfectBalance },
    };

    /// <summary>
    /// 金刚极意 盾
    /// </summary>
    public static BaseAction RiddleofEarth { get; } = new(ActionID.RiddleofEarth, true, shouldEndSpecial: true, isTimeline: true)
    {
        BuffsProvide = new[] { StatusID.RiddleofEarth },
    };

    /// <summary>
    /// 疾风极意
    /// </summary>
    public static BaseAction RiddleofWind { get; } = new(ActionID.RiddleofWind, true);

    private protected override bool MoveAbility(byte abilityRemain, out IAction act)
    {
        if (Thunderclap.ShouldUse(out act, emptyOrSkipCombo: true)) return true;
        return false;
    }
}
