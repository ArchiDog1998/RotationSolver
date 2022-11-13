using Dalamud.Game.ClientState.JobGauge.Types;
using System;
using XIVAutoAttack.Actions.BaseAction;
using XIVAutoAttack.Combos.CustomCombo;
using XIVAutoAttack.Data;
using XIVAutoAttack.Updaters;

namespace XIVAutoAttack.Combos.Basic;

internal abstract class SMNCombo_Base<TCmd> : JobGaugeCombo<SMNGauge, TCmd> where TCmd : Enum
{
    public sealed override ClassJobID[] JobIDs => new ClassJobID[] { ClassJobID.Summoner, ClassJobID.Arcanist };
    protected override bool CanHealSingleSpell => false;
    private sealed protected override BaseAction Raise => Resurrection;

    protected static bool InBahamut => Service.IconReplacer.OriginalHook(ActionID.AstralFlow) == ActionID.Deathflare;
    protected static bool InPhoenix => Service.IconReplacer.OriginalHook(ActionID.AstralFlow) == ActionID.Rekindle;
    protected static bool InBreak => InBahamut || InPhoenix || !SummonBahamut.EnoughLevel;

    //宝石耀
    public static BaseAction Gemshine { get; } = new(ActionID.Gemshine)
    {
        OtherCheck = b => JobGauge.Attunement > 0,
    };

    //宝石辉
    public static BaseAction PreciousBrilliance { get; } = new(ActionID.PreciousBrilliance)
    {
        OtherCheck = b => JobGauge.Attunement > 0,
    };

    //毁灭 单体攻击
    public static BaseAction Ruin { get; } = new(ActionID.RuinSMN);

    //迸裂 范围伤害
    public static BaseAction Outburst { get; } = new(ActionID.Outburst);

    //宝石兽召唤
    public static BaseAction SummonCarbuncle { get; } = new(ActionID.SummonCarbuncle)
    {
        OtherCheck = b => !TargetUpdater.HavePet,
    };

    //灼热之光 团辅
    public static BaseAction SearingLight { get; } = new(ActionID.SearingLight, true)
    {
        OtherCheck = b => InCombat && !InBahamut && !InPhoenix
    };

    //守护之光 给自己戴套
    public static BaseAction RadiantAegis { get; } = new(ActionID.RadiantAegis, true);

    //医术
    public static BaseAction Physick { get; } = new(ActionID.Physick, true);

    //以太蓄能 
    public static BaseAction Aethercharge { get; } = new(ActionID.Aethercharge)
    {
        OtherCheck = b => InCombat,
    };

    //龙神召唤
    public static BaseAction SummonBahamut { get; } = new(ActionID.SummonBahamut);

    //红宝石召唤
    public static BaseAction SummonRuby { get; } = new(ActionID.SummonRuby)
    {
        OtherCheck = b => JobGauge.IsIfritReady && !IsMoving,
    };

    //黄宝石召唤
    public static BaseAction SummonTopaz { get; } = new(ActionID.SummonTopaz)
    {
        OtherCheck = b => JobGauge.IsTitanReady,
    };

    //绿宝石召唤
    public static BaseAction SummonEmerald { get; } = new(ActionID.SummonEmerald)
    {
        OtherCheck = b => JobGauge.IsGarudaReady,
    };


    //复生
    public static BaseAction Resurrection { get; } = new(ActionID.ResurrectionSMN, true);

    //能量吸收
    public static BaseAction EnergyDrain { get; } = new(ActionID.EnergyDrainSMN);

    //能量抽取
    public static BaseAction EnergySiphon { get; } = new(ActionID.EnergySiphon);

    //溃烂爆发
    public static BaseAction Fester { get; } = new(ActionID.Fester);

    //痛苦核爆
    public static BaseAction Painflare { get; } = new(ActionID.Painflare);

    //毁绝
    public static BaseAction RuinIV { get; } = new(ActionID.RuinIV)
    {
        BuffsNeed = new[] { StatusID.FurtherRuin },
    };

    //龙神迸发
    public static BaseAction EnkindleBahamut { get; } = new(ActionID.EnkindleBahamut)
    {
        OtherCheck = b => InBahamut || InPhoenix,
    };

    //死星核爆
    public static BaseAction Deathflare { get; } = new(ActionID.Deathflare)
    {
        OtherCheck = b => InBahamut,
    };

    //苏生之炎
    public static BaseAction Rekindle { get; } = new(ActionID.Rekindle, true)
    {
        OtherCheck = b => InPhoenix,
    };

    //深红旋风
    public static BaseAction CrimsonCyclone { get; } = new(ActionID.CrimsonCyclone)
    {
        BuffsNeed = new[] { StatusID.IfritsFavor },
    };

    //深红强袭
    public static BaseAction CrimsonStrike { get; } = new(ActionID.CrimsonStrike);

    //山崩
    public static BaseAction MountainBuster { get; } = new(ActionID.MountainBuster)
    {
        BuffsNeed = new[] { StatusID.TitansFavor },
    };

    //螺旋气流
    public static BaseAction Slipstream { get; } = new(ActionID.Slipstream)
    {
        BuffsNeed = new[] { StatusID.GarudasFavor },
    };
}
