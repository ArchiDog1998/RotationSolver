using Dalamud.Game.ClientState.JobGauge.Types;
using System;
using System.Collections.Generic;
using XIVAutoAttack.Actions;
using XIVAutoAttack.Actions.BaseAction;
using XIVAutoAttack.Combos.CustomCombo;
using XIVAutoAttack.Configuration;
using XIVAutoAttack.Data;
using XIVAutoAttack.Helpers;
using XIVAutoAttack.Updaters;

namespace XIVAutoAttack.Combos.RangedMagicial;

public  abstract class SMNCombo<TCmd> : JobGaugeCombo<SMNGauge, TCmd> where TCmd : Enum
{
    public sealed override uint[] JobIDs => new uint[] { 27, 26 };
    protected override bool CanHealSingleSpell => false;
    private protected  override BaseAction Raise => Resurrection;

    protected static bool InBahamut => Service.IconReplacer.OriginalHook(25822) == Deathflare.ID;
    protected static bool InPhoenix => Service.IconReplacer.OriginalHook(25822) == Rekindle.ID;
    protected static bool InBreak => InBahamut || InPhoenix || !SummonBahamut.EnoughLevel;


    public static readonly BaseAction
        //宝石耀
        Gemshine = new(25883)
        {
            OtherCheck = b => JobGauge.Attunement > 0,
        },

        //宝石辉
        PreciousBrilliance = new(25884)
        {
            OtherCheck = b => JobGauge.Attunement > 0,
        },

        //毁灭 单体攻击
        Ruin = new(163),

        //迸裂 范围伤害
        Outburst = new(16511),

        //宝石兽召唤
        SummonCarbuncle = new(25798)
        {
            OtherCheck = b => !TargetUpdater.HavePet,
        },

        //灼热之光 团辅
        SearingLight = new(25801)
        {
            OtherCheck = b => InCombat && !InBahamut && !InPhoenix
        },

        //守护之光 给自己戴套
        RadiantAegis = new(25799),

        //医术
        Physick = new(16230, true),

        //以太蓄能 
        Aethercharge = new(25800)
        {
            OtherCheck = b => InCombat,
        },

        //龙神召唤
        SummonBahamut = new(7427),

        //红宝石召唤
        SummonRuby = new(25802)
        {
            OtherCheck = b => JobGauge.IsIfritReady && !IsMoving,
        },

        //黄宝石召唤
        SummonTopaz = new(25803)
        {
            OtherCheck = b => JobGauge.IsTitanReady,
        },

        //绿宝石召唤
        SummonEmerald = new(25804)
        {
            OtherCheck = b => JobGauge.IsGarudaReady,
        },


        //复生
        Resurrection = new(173, true),

        //能量吸收
        EnergyDrain = new(16508),

        //能量抽取
        EnergySiphon = new(16510),

        //溃烂爆发
        Fester = new(181),

        //痛苦核爆
        Painflare = new(3578),

        //毁绝
        RuinIV = new(7426)
        {
            BuffsNeed = new[] { ObjectStatus.FurtherRuin },
        },

        //龙神迸发
        EnkindleBahamut = new(7429)
        {
            OtherCheck = b => InBahamut || InPhoenix,
        },

        //死星核爆
        Deathflare = new(3582)
        {
            OtherCheck = b => InBahamut,
        },

        //苏生之炎
        Rekindle = new(25830, true)
        {
            OtherCheck = b => InPhoenix,
        },

        //深红旋风
        CrimsonCyclone = new(25835)
        {
            BuffsNeed = new[] { ObjectStatus.IfritsFavor },
        },

        //深红强袭
        CrimsonStrike = new(25885),

        //山崩
        MountainBuster = new(25836)
        {
            BuffsNeed = new[] { ObjectStatus.TitansFavor },
        },

        //螺旋气流
        Slipstream = new(25837)
        {
            BuffsNeed = new[] { ObjectStatus.GarudasFavor },
        };
}
