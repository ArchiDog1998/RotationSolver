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


    public static readonly BaseAction
        //双龙脚
        DragonKick = new(74)
        {
            BuffsProvide = new[] { StatusID.LeadenFist },
        },

        //连击
        Bootshine = new(53),

        //破坏神冲 aoe
        ArmoftheDestroyer = new(62),

        //双掌打 伤害提高
        TwinSnakes = new(61),

        //正拳
        TrueStrike = new(54),

        //四面脚 aoe
        FourpointFury = new(16473),

        //破碎拳
        Demolish = new(66, isEot: true)
        {
            TargetStatus = new StatusID[] { StatusID.Demolish },
        },

        //崩拳
        SnapPunch = new(ActionID.SnapPunch),

        //地烈劲 aoe
        Rockbreaker = new(70),

        //斗气
        Meditation = new(3546),

        //铁山靠
        SteelPeak = new(25761)
        {
            OtherCheck = b => InCombat,
        },

        //空鸣拳
        HowlingFist = new(25763)
        {
            OtherCheck = b => InCombat,
        },

        //义结金兰
        Brotherhood = new(7396, true),

        //红莲极意 提高dps
        RiddleofFire = new(7395),

        //突进技能
        Thunderclap = new(25762, shouldEndSpecial: true)
        {
            ChoiceTarget = TargetFilter.FindTargetForMoving,
        },

        //真言
        Mantra = new(65, true),

        //震脚
        PerfectBalance = new(69)
        {
            BuffsNeed = new StatusID[] { StatusID.RaptorForm },
            OtherCheck = b => InCombat,
        },

        //苍气炮 阴
        ElixirField = new(3545),

        //爆裂脚 阳
        FlintStrike = new(25882),

        //凤凰舞
        RisingPhoenix = new(25768),

        //斗魂旋风脚 阴阳
        TornadoKick = new(3543),
        PhantomRush = new(25769),

        //演武
        FormShift = new(4262)
        {
            BuffsProvide = new[] { StatusID.FormlessFist, StatusID.PerfectBalance },
        },

        //金刚极意 盾
        RiddleofEarth = new(7394, shouldEndSpecial: true)
        {
            BuffsProvide = new[] { StatusID.RiddleofEarth },
        },

        //疾风极意
        RiddleofWind = new(25766);

}
