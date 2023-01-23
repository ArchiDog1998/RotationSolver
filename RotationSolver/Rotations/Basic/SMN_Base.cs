using Dalamud.Game.ClientState.JobGauge.Types;
using RotationSolver.Actions;
using RotationSolver.Actions.BaseAction;
using RotationSolver.Data;
using RotationSolver.Updaters;

namespace RotationSolver.Rotations.Basic;

internal abstract class SMN_Base : CustomRotation.CustomRotation
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
    private sealed protected override IBaseAction Raise => Resurrection;

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
    public static IBaseAction SummonRuby { get; } = new BaseAction(ActionID.SummonRuby)
    {
        StatusProvide = new[] { StatusID.IfritsFavor },
        ActionCheck = b => HaveSummon && IsIfritReady
    };

    /// <summary>
    /// 黄宝石召唤 土神召唤
    /// </summary>
    public static IBaseAction SummonTopaz { get; } = new BaseAction(ActionID.SummonTopaz)
    {
        ActionCheck = b => HaveSummon && IsTitanReady,
    };

    /// <summary>
    /// 绿宝石召唤 风神召唤
    /// </summary>
    public static IBaseAction SummonEmerald { get; } = new BaseAction(ActionID.SummonEmerald)
    {
        StatusProvide = new[] { StatusID.GarudasFavor },
        ActionCheck = b => HaveSummon && IsGarudaReady,
    };

    /// <summary>
    /// 宝石兽召唤
    /// </summary>
    public static IBaseAction SummonCarbuncle { get; } = new BaseAction(ActionID.SummonCarbuncle)
    {
        ActionCheck = b => !TargetUpdater.HavePet,
    };
    #endregion
    #region 召唤兽能力
    /// <summary>
    /// 宝石耀 单体
    /// </summary>
    public static IBaseAction Gemshine { get; } = new BaseAction(ActionID.Gemshine)
    {
        ActionCheck = b => Attunement > 0,
    };

    /// <summary>
    /// 宝石辉 AoE
    /// </summary>
    public static IBaseAction PreciousBrilliance { get; } = new BaseAction(ActionID.PreciousBrilliance)
    {
        ActionCheck = b => Attunement > 0,
    };

    /// <summary>
    /// 以太蓄能  龙神附体 
    /// </summary>
    public static IBaseAction Aethercharge { get; } = new BaseAction(ActionID.Aethercharge)
    {
        ActionCheck = b => InCombat && HaveSummon
    };

    /// <summary>
    /// 龙神召唤 不死鸟召唤
    /// </summary>
    public static IBaseAction SummonBahamut { get; } = new BaseAction(ActionID.SummonBahamut)
    {
        ActionCheck = b => InCombat && HaveSummon
    };
    /// <summary>
    /// 龙神迸发 不死鸟迸发
    /// </summary>
    public static IBaseAction EnkindleBahamut { get; } = new BaseAction(ActionID.EnkindleBahamut)
    {
        ActionCheck = b => InBahamut || InPhoenix,
    };

    #endregion
    #region 召唤兽星极超流
    /// <summary>
    /// 死星核爆
    /// </summary>
    public static IBaseAction Deathflare { get; } = new BaseAction(ActionID.Deathflare)
    {
        ActionCheck = b => InBahamut,
    };

    /// <summary>
    /// 苏生之炎
    /// </summary>
    public static IBaseAction Rekindle { get; } = new BaseAction(ActionID.Rekindle, true)
    {
        ActionCheck = b => InPhoenix,
    };

    /// <summary>
    /// 深红旋风
    /// </summary>
    public static IBaseAction CrimsonCyclone { get; } = new BaseAction(ActionID.CrimsonCyclone)
    {
        StatusNeed = new[] { StatusID.IfritsFavor },
    };

    /// <summary>
    /// 深红强袭
    /// </summary>
    public static IBaseAction CrimsonStrike { get; } = new BaseAction(ActionID.CrimsonStrike);

    /// <summary>
    /// 山崩
    /// </summary>
    public static IBaseAction MountainBuster { get; } = new BaseAction(ActionID.MountainBuster)
    {
        StatusNeed = new[] { StatusID.TitansFavor },
    };

    /// <summary>
    /// 螺旋气流
    /// </summary>
    public static IBaseAction Slipstream { get; } = new BaseAction(ActionID.Slipstream)
    {
        StatusNeed = new[] { StatusID.GarudasFavor },
    };

    #endregion
    #region 基础技能
    /// <summary>
    /// 毁灭 毁坏 毁荡
    /// </summary>
    public static IBaseAction Ruin { get; } = new BaseAction(ActionID.RuinSMN);

    /// <summary>
    /// 毁绝
    /// </summary>
    public static IBaseAction RuinIV { get; } = new BaseAction(ActionID.RuinIV)
    {
        StatusNeed = new[] { StatusID.FurtherRuin },
    };

    /// <summary>
    /// 迸裂 三重灾祸
    /// </summary>
    public static IBaseAction Outburst { get; } = new BaseAction(ActionID.Outburst);

    #endregion
    #region 能力技
    /// <summary>
    /// 灼热之光 团辅
    /// </summary>
    public static IBaseAction SearingLight { get; } = new BaseAction(ActionID.SearingLight, true)
    {
        StatusProvide = new[] { StatusID.SearingLight },
        ActionCheck = b => InCombat,
    };

    /// <summary>
    /// 守护之光
    /// </summary>
    public static IBaseAction RadiantAegis { get; } = new BaseAction(ActionID.RadiantAegis, true, isTimeline: true)
    {
        ActionCheck = b => HaveSummon
    };

    /// <summary>
    /// 能量吸收
    /// </summary>
    public static IBaseAction EnergyDrain { get; } = new BaseAction(ActionID.EnergyDrainSMN)
    {
        StatusProvide = new[] { StatusID.FurtherRuin },
        ActionCheck = b => !HasAetherflowStacks
    };

    /// <summary>
    /// 溃烂爆发
    /// </summary>
    public static IBaseAction Fester { get; } = new BaseAction(ActionID.Fester)
    {
        ActionCheck = b => HasAetherflowStacks
    };

    /// <summary>
    /// 能量抽取
    /// </summary>
    public static IBaseAction EnergySiphon { get; } = new BaseAction(ActionID.EnergySiphon)
    {
        StatusProvide = new[] { StatusID.FurtherRuin },
        ActionCheck = b => !HasAetherflowStacks
    };

    /// <summary>
    /// 痛苦核爆
    /// </summary>
    public static IBaseAction Painflare { get; } = new BaseAction(ActionID.Painflare)
    {
        ActionCheck = b => HasAetherflowStacks
    };
    #endregion

    /// <summary>
    /// 复生
    /// </summary>
    public static IBaseAction Resurrection { get; } = new BaseAction(ActionID.ResurrectionSMN, true);

    /// <summary>
    /// 医术
    /// </summary>
    public static IBaseAction Physick { get; } = new BaseAction(ActionID.Physick, true);
}