using Dalamud.Game.ClientState.JobGauge.Types;
using System;
using XIVAutoAttack.Actions.BaseAction;
using XIVAutoAttack.Combos.CustomCombo;
using XIVAutoAttack.Data;
using XIVAutoAttack.Helpers;

namespace XIVAutoAttack.Combos.Basic;

internal abstract class MNKCombo_Base<TCmd> : JobGaugeCombo<MNKGauge, TCmd> where TCmd : Enum
{
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
    /// 双掌打 伤害提高
    /// </summary>
    public static BaseAction TwinSnakes { get; } = new(ActionID.TwinSnakes);

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
    public static BaseAction Meditation { get; } = new(ActionID.Meditation);

    /// <summary>
    /// 铁山靠
    /// </summary>
    public static BaseAction SteelPeak { get; } = new(ActionID.SteelPeak)
    {
        OtherCheck = b => InCombat,
    };

    /// <summary>
    /// 空鸣拳
    /// </summary>
    public static BaseAction HowlingFist { get; } = new(ActionID.HowlingFist)
    {
        OtherCheck = b => InCombat,
    };

    /// <summary>
    /// 义结金兰
    /// </summary>
    public static BaseAction Brotherhood { get; } = new(ActionID.Brotherhood, true);

    /// <summary>
    /// 红莲极意 提高dps
    /// </summary>
    public static BaseAction RiddleofFire { get; } = new(ActionID.RiddleofFire);

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
    public static BaseAction Mantra { get; } = new(ActionID.Mantra, true);

    /// <summary>
    /// 震脚
    /// </summary>
    public static BaseAction PerfectBalance { get; } = new(ActionID.PerfectBalance)
    {
        BuffsNeed = new StatusID[] { StatusID.RaptorForm },
        OtherCheck = b => InCombat,
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
    public static BaseAction FormShift { get; } = new(ActionID.FormShift)
    {
        BuffsProvide = new[] { StatusID.FormlessFist, StatusID.PerfectBalance },
    };

    /// <summary>
    /// 金刚极意 盾
    /// </summary>
    public static BaseAction RiddleofEarth { get; } = new(ActionID.RiddleofEarth, shouldEndSpecial: true)
    {
        BuffsProvide = new[] { StatusID.RiddleofEarth },
    };

    /// <summary>
    /// 疾风极意
    /// </summary>
    public static BaseAction RiddleofWind { get; } = new(ActionID.RiddleofWind);

}
