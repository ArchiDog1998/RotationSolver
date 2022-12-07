using Dalamud.Game.ClientState.JobGauge.Types;
using System;
using XIVAutoAttack.Actions;
using XIVAutoAttack.Actions.BaseAction;
using XIVAutoAttack.Combos.CustomCombo;
using XIVAutoAttack.Data;
using XIVAutoAttack.Helpers;

namespace XIVAutoAttack.Combos.Basic;

internal abstract class WARCombo_Base<TCmd> : CustomCombo<TCmd> where TCmd : Enum
{
    private static WARGauge JobGauge => Service.JobGauges.Get<WARGauge>();

    public sealed override ClassJobID[] JobIDs => new ClassJobID[] { ClassJobID.Warrior, ClassJobID.Marauder };
    private sealed protected override BaseAction Shield => Defiance;

    /// <summary>
    /// 守护
    /// </summary>
    public static BaseAction Defiance { get; } = new(ActionID.Defiance, shouldEndSpecial: true);

    /// <summary>
    /// 重劈
    /// </summary>
    public static BaseAction HeavySwing { get; } = new(ActionID.HeavySwing);

    /// <summary>
    /// 凶残裂
    /// </summary>
    public static BaseAction Maim { get; } = new(ActionID.Maim);

    /// <summary>
    /// 暴风斩 绿斧
    /// </summary>
    public static BaseAction StormsPath { get; } = new(ActionID.StormsPath);

    /// <summary>
    /// 暴风碎 红斧
    /// </summary>
    public static BaseAction StormsEye { get; } = new(ActionID.StormsEye)
    {
        ActionCheck = b => Player.WillStatusEndGCD(3, 0, true, StatusID.SurgingTempest),
    };

    /// <summary>
    /// 飞斧
    /// </summary>
    public static BaseAction Tomahawk { get; } = new(ActionID.Tomahawk)
    {
        FilterForTarget = b => TargetFilter.ProvokeTarget(b),
    };

    /// <summary>
    /// 猛攻
    /// </summary>
    public static BaseAction Onslaught { get; } = new(ActionID.Onslaught, shouldEndSpecial: true)
    {
        ChoiceTarget = TargetFilter.FindTargetForMoving,
    };

    /// <summary>
    /// 动乱    
    /// </summary>
    public static BaseAction Upheaval { get; } = new(ActionID.Upheaval)
    {
        BuffsNeed = new StatusID[] { StatusID.SurgingTempest },
    };

    /// <summary>
    /// 超压斧
    /// </summary>
    public static BaseAction Overpower { get; } = new(ActionID.Overpower);

    /// <summary>
    /// 秘银暴风
    /// </summary>
    public static BaseAction MythrilTempest { get; } = new(ActionID.MythrilTempest);

    /// <summary>
    /// 群山隆起
    /// </summary>
    public static BaseAction Orogeny { get; } = new(ActionID.Orogeny);

    /// <summary>
    /// 原初之魂
    /// </summary>
    public static BaseAction InnerBeast { get; } = new(ActionID.InnerBeast)
    {
        ActionCheck = b => JobGauge.BeastGauge >= 50 || Player.HasStatus(true, StatusID.InnerRelease),
    };

    /// <summary>
    /// 原初的解放
    /// </summary>
    public static BaseAction InnerRelease { get; } = new(ActionID.InnerRelease)
    {
        ActionCheck = InnerBeast.ActionCheck,
    };

    /// <summary>
    /// 钢铁旋风
    /// </summary>
    public static BaseAction SteelCyclone { get; } = new(ActionID.SteelCyclone)
    {
        ActionCheck = InnerBeast.ActionCheck,
    };

    /// <summary>
    /// 战嚎
    /// </summary>
    public static BaseAction Infuriate { get; } = new(ActionID.Infuriate)
    {
        BuffsProvide = new[] { StatusID.InnerRelease },
        ActionCheck = b => HaveHostilesInRange && JobGauge.BeastGauge < 50 && InCombat,
    };

    /// <summary>
    /// 狂暴
    /// </summary>
    public static BaseAction Berserk { get; } = new(ActionID.Berserk)
    {
        ActionCheck = b => HaveHostilesInRange,
    };

    /// <summary>
    /// 战栗
    /// </summary>
    public static BaseAction ThrillofBattle { get; } = new(ActionID.ThrillofBattle, true, isTimeline: true);

    /// <summary>
    /// 泰然自若
    /// </summary>
    public static BaseAction Equilibrium { get; } = new(ActionID.Equilibrium, true, isTimeline: true);

    /// <summary>
    /// 原初的勇猛
    /// </summary>
    public static BaseAction NascentFlash { get; } = new(ActionID.NascentFlash, isTimeline: true)
    {
        ChoiceTarget = TargetFilter.FindAttackedTarget,
    };

    /// <summary>
    /// 复仇
    /// </summary>
    public static BaseAction Vengeance { get; } = new(ActionID.Vengeance, isTimeline: true)
    {
        BuffsProvide = Rampart.BuffsProvide,
        ActionCheck = BaseAction.TankDefenseSelf,
    };

    /// <summary>
    /// 原初的直觉
    /// </summary>
    public static BaseAction RawIntuition { get; } = new(ActionID.RawIntuition, isTimeline: true)
    {
        ActionCheck = BaseAction.TankDefenseSelf,
    };

    /// <summary>
    /// 摆脱
    /// </summary>
    public static BaseAction ShakeItOff { get; } = new(ActionID.ShakeItOff, true, isTimeline: true);

    /// <summary>
    /// 死斗
    /// </summary>
    public static BaseAction Holmgang { get; } = new(ActionID.Holmgang, isTimeline: true)
    {
        ChoiceTarget =( tars, mustUse) => Player,
    };

    /// <summary>
    /// 蛮荒崩裂
    /// </summary>
    public static BaseAction PrimalRend { get; } = new(ActionID.PrimalRend)
    {
        BuffsNeed = new[] { StatusID.PrimalRendReady }
    };

    private protected override bool EmergencyAbility(byte abilityRemain, IAction nextGCD, out IAction act)
    {
        //死斗 如果血不够了。
        if (Holmgang.ShouldUse(out act) && BaseAction.TankBreakOtherCheck(JobIDs[0], Holmgang.Target)) return true;
        return base.EmergencyAbility(abilityRemain, nextGCD, out act);
    }
}
