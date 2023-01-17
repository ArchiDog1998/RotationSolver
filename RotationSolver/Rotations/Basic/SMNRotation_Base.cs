using Dalamud.Game.ClientState.JobGauge.Types;
using System;
using RotationSolver.Actions.BaseAction;
using RotationSolver.Combos.CustomCombo;
using RotationSolver.Updaters;
using RotationSolver.Data;

namespace RotationSolver.Combos.Basic;

internal abstract class SMNRotation_Base : CustomCombo.CustomRotation
{
    private static SMNGauge JobGauge => Service.JobGauges.Get<SMNGauge>();

    /// <summary>
    /// 有以太超流
    /// </summary>
    protected static bool HasAetherflowStacks => JobGauge.HasAetherflowStacks;

    /// <summary>
    /// 属性以太
    /// </summary>
    protected static byte Attunement => JobGauge.Attunement;

    /// <summary>
    /// 蛮神多久后会消失
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    protected static bool SummonTimeEndAfter(float time)
    {
        return EndAfter(JobGauge.SummonTimerRemaining / 1000f, time);
    }

    /// <summary>
    /// 蛮神剩余时间
    /// </summary>
    protected static ushort SummonTimerRemaining => JobGauge.SummonTimerRemaining;

    /// <summary>
    /// 属性剩余时间
    /// </summary>
    protected static ushort AttunmentTimerRemaining => JobGauge.AttunmentTimerRemaining;

    public sealed override ClassJobID[] JobIDs => new ClassJobID[] { ClassJobID.Summoner, ClassJobID.Arcanist };
    protected override bool CanHealSingleSpell => false;
    private sealed protected override BaseAction Raise => Resurrection;

    /// <summary>
    /// 宝石兽处于同行状态
    /// </summary>
    protected static bool HaveSummon => TargetUpdater.HavePet && JobGauge.SummonTimerRemaining == 0;

    /// <summary>
    /// 龙神附体状态
    /// </summary>
    protected static bool InBahamut => Service.IconReplacer.OriginalHook(ActionID.AstralFlow) == ActionID.Deathflare;

    /// <summary>
    /// 不死鸟附体状态
    /// </summary>
    protected static bool InPhoenix => Service.IconReplacer.OriginalHook(ActionID.AstralFlow) == ActionID.Rekindle;

    /// <summary>
    /// 火神可用
    /// </summary>
    protected static bool IsIfritReady => JobGauge.IsIfritReady;

    /// <summary>
    /// 土神可用
    /// </summary>
    protected static bool IsTitanReady => JobGauge.IsTitanReady;

    /// <summary>
    /// 风神可用
    /// </summary>
    protected static bool IsGarudaReady => JobGauge.IsGarudaReady;

    /// <summary>
    /// 火神阶段
    /// </summary>
    protected static bool InIfrit => JobGauge.IsIfritAttuned;

    /// <summary>
    /// 土神阶段
    /// </summary>
    protected static bool InTitan => JobGauge.IsTitanAttuned;

    /// <summary>
    /// 风神阶段
    /// </summary>
    protected static bool InGaruda => JobGauge.IsGarudaAttuned;

    #region 召唤兽
    /// <summary>
    /// 红宝石召唤 火神召唤
    /// </summary>
    public static BaseAction SummonRuby { get; } = new(ActionID.SummonRuby)
    {
        BuffsProvide = new[] { StatusID.IfritsFavor },
        ActionCheck = b => HaveSummon && IsIfritReady
    };

    /// <summary>
    /// 黄宝石召唤 土神召唤
    /// </summary>
    public static BaseAction SummonTopaz { get; } = new(ActionID.SummonTopaz)
    {
        ActionCheck = b => HaveSummon && IsTitanReady,
    };

    /// <summary>
    /// 绿宝石召唤 风神召唤
    /// </summary>
    public static BaseAction SummonEmerald { get; } = new(ActionID.SummonEmerald)
    {
        BuffsProvide = new[] { StatusID.GarudasFavor },
        ActionCheck = b => HaveSummon && IsGarudaReady,
    };

    /// <summary>
    /// 宝石兽召唤
    /// </summary>
    public static BaseAction SummonCarbuncle { get; } = new(ActionID.SummonCarbuncle)
    {
        ActionCheck = b => !TargetUpdater.HavePet,
    };
    #endregion
    #region 召唤兽能力
    /// <summary>
    /// 宝石耀 单体
    /// </summary>
    public static BaseAction Gemshine { get; } = new(ActionID.Gemshine)
    {
        ActionCheck = b => Attunement > 0,
    };

    /// <summary>
    /// 宝石辉 AoE
    /// </summary>
    public static BaseAction PreciousBrilliance { get; } = new(ActionID.PreciousBrilliance)
    {
        ActionCheck = b => Attunement > 0,
    };

    /// <summary>
    /// 以太蓄能  龙神附体 
    /// </summary>
    public static BaseAction Aethercharge { get; } = new(ActionID.Aethercharge)
    {
        ActionCheck = b => InCombat && HaveSummon
    };

    /// <summary>
    /// 龙神召唤 不死鸟召唤
    /// </summary>
    public static BaseAction SummonBahamut { get; } = new(ActionID.SummonBahamut)
    {
        ActionCheck = b => InCombat && HaveSummon
    };
    /// <summary>
    /// 龙神迸发 不死鸟迸发
    /// </summary>
    public static BaseAction EnkindleBahamut { get; } = new(ActionID.EnkindleBahamut)
    {
        ActionCheck = b => InBahamut || InPhoenix,
    };

    #endregion
    #region 召唤兽星极超流
    /// <summary>
    /// 死星核爆
    /// </summary>
    public static BaseAction Deathflare { get; } = new(ActionID.Deathflare)
    {
        ActionCheck = b => InBahamut,
    };

    /// <summary>
    /// 苏生之炎
    /// </summary>
    public static BaseAction Rekindle { get; } = new(ActionID.Rekindle, true)
    {
        ActionCheck = b => InPhoenix,
    };

    /// <summary>
    /// 深红旋风
    /// </summary>
    public static BaseAction CrimsonCyclone { get; } = new(ActionID.CrimsonCyclone)
    {
        BuffsNeed = new[] { StatusID.IfritsFavor },
    };

    /// <summary>
    /// 深红强袭
    /// </summary>
    public static BaseAction CrimsonStrike { get; } = new(ActionID.CrimsonStrike);

    /// <summary>
    /// 山崩
    /// </summary>
    public static BaseAction MountainBuster { get; } = new(ActionID.MountainBuster)
    {
        BuffsNeed = new[] { StatusID.TitansFavor },
    };

    /// <summary>
    /// 螺旋气流
    /// </summary>
    public static BaseAction Slipstream { get; } = new(ActionID.Slipstream)
    {
        BuffsNeed = new[] { StatusID.GarudasFavor },
    };

    #endregion
    #region 基础技能
    /// <summary>
    /// 毁灭 毁坏 毁荡
    /// </summary>
    public static BaseAction Ruin { get; } = new(ActionID.RuinSMN);

    /// <summary>
    /// 毁绝
    /// </summary>
    public static BaseAction RuinIV { get; } = new(ActionID.RuinIV)
    {
        BuffsNeed = new[] { StatusID.FurtherRuin },
    };

    /// <summary>
    /// 迸裂 三重灾祸
    /// </summary>
    public static BaseAction Outburst { get; } = new(ActionID.Outburst);

    #endregion
    #region 能力技
    /// <summary>
    /// 灼热之光 团辅
    /// </summary>
    public static BaseAction SearingLight { get; } = new(ActionID.SearingLight, true)
    {
        BuffsProvide = new[] { StatusID.SearingLight },
        ActionCheck = b => InCombat,
    };

    /// <summary>
    /// 守护之光
    /// </summary>
    public static BaseAction RadiantAegis { get; } = new(ActionID.RadiantAegis, true, isTimeline: true)
    {
        ActionCheck = b => HaveSummon
    };

    /// <summary>
    /// 能量吸收
    /// </summary>
    public static BaseAction EnergyDrain { get; } = new(ActionID.EnergyDrainSMN)
    {
        BuffsProvide = new[] { StatusID.FurtherRuin },
        ActionCheck = b => !HasAetherflowStacks
    };

    /// <summary>
    /// 溃烂爆发
    /// </summary>
    public static BaseAction Fester { get; } = new(ActionID.Fester)
    {
        ActionCheck = b => HasAetherflowStacks
    };

    /// <summary>
    /// 能量抽取
    /// </summary>
    public static BaseAction EnergySiphon { get; } = new(ActionID.EnergySiphon)
    {
        BuffsProvide = new[] { StatusID.FurtherRuin },
        ActionCheck = b => !HasAetherflowStacks
    };

    /// <summary>
    /// 痛苦核爆
    /// </summary>
    public static BaseAction Painflare { get; } = new(ActionID.Painflare)
    {
        ActionCheck = b => HasAetherflowStacks
    };
    #endregion

    /// <summary>
    /// 复生
    /// </summary>
    public static BaseAction Resurrection { get; } = new(ActionID.ResurrectionSMN, true);

    /// <summary>
    /// 医术
    /// </summary>
    public static BaseAction Physick { get; } = new(ActionID.Physick, true);
}