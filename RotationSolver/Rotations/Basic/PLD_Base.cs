using Dalamud.Game.ClientState.JobGauge.Types;
using System;
using System.Linq;
using RotationSolver.Actions.BaseAction;
using RotationSolver.Updaters;
using RotationSolver.Actions;
using RotationSolver.Helpers;
using RotationSolver.Data;

namespace RotationSolver.Rotations.Basic;

internal abstract class PLDRotation_Base : CustomRotation.CustomRotation
{
    private static PLDGauge JobGauge => Service.JobGauges.Get<PLDGauge>();

    /// <summary>
    /// 忠义度
    /// </summary>
    protected static byte OathGauge => JobGauge.OathGauge;

    public sealed override ClassJobID[] JobIDs => new ClassJobID[] { ClassJobID.Paladin, ClassJobID.Gladiator };

    private sealed protected override IBaseAction Shield => IronWill;

    protected override bool CanHealSingleSpell => TargetUpdater.PartyMembers.Count() == 1 && base.CanHealSingleSpell;

    /// <summary>
    /// 钢铁信念
    /// </summary>
    public static IBaseAction IronWill { get; } = new BaseAction(ActionID.IronWill, shouldEndSpecial: true);

    /// <summary>
    /// 先锋剑
    /// </summary>
    public static IBaseAction FastBlade { get; } = new BaseAction(ActionID.FastBlade);

    /// <summary>
    /// 暴乱剑
    /// </summary>
    public static IBaseAction RiotBlade { get; } = new BaseAction(ActionID.RiotBlade);

    /// <summary>
    /// 沥血剑
    /// </summary>
    public static IBaseAction GoringBlade { get; } = new BaseAction(ActionID.GoringBlade, isEot: true)
    {
        TargetStatus = new[]
        {
            StatusID.GoringBlade,
            StatusID.BladeofValor,
        }
    };

    /// <summary>
    /// 战女神之怒(王权剑)
    /// </summary>
    public static IBaseAction RageofHalone { get; } = new BaseAction(ActionID.RageofHalone);

    /// <summary>
    /// 投盾
    /// </summary>
    public static IBaseAction ShieldLob { get; } = new BaseAction(ActionID.ShieldLob)
    {
        FilterForTarget = b => TargetFilter.ProvokeTarget(b),
    };

    /// <summary>
    /// 战逃反应
    /// </summary>
    public static IBaseAction FightorFlight { get; } = new BaseAction(ActionID.FightorFlight, true);

    /// <summary>
    /// 全蚀斩
    /// </summary>
    public static IBaseAction TotalEclipse { get; } = new BaseAction(ActionID.TotalEclipse);

    /// <summary>
    /// 日珥斩
    /// </summary>
    public static IBaseAction Prominence { get; } = new BaseAction(ActionID.Prominence);

    /// <summary>
    /// 预警
    /// </summary>
    public static IBaseAction Sentinel { get; } = new BaseAction(ActionID.Sentinel, isTimeline: true, isFriendly: true)
    {
        StatusProvide = Rampart.StatusProvide,
        ActionCheck = BaseAction.TankDefenseSelf,
    };

    /// <summary>
    /// 厄运流转
    /// </summary>
    public static IBaseAction CircleofScorn { get; } = new BaseAction(ActionID.CircleofScorn);

    /// <summary>
    /// 深奥之灵
    /// </summary>
    public static IBaseAction SpiritsWithin { get; } = new BaseAction(ActionID.SpiritsWithin);

    /// <summary>
    /// 神圣领域
    /// </summary>
    public static IBaseAction HallowedGround { get; } = new BaseAction(ActionID.HallowedGround, isTimeline: true);

    /// <summary>
    /// 圣光幕帘
    /// </summary>
    public static IBaseAction DivineVeil { get; } = new BaseAction(ActionID.DivineVeil, true, isTimeline: true);

    /// <summary>
    /// 深仁厚泽
    /// </summary>
    public static IBaseAction Clemency { get; } = new BaseAction(ActionID.Clemency, true, true, isTimeline: true);

    /// <summary>
    /// 干预
    /// </summary>
    public static IBaseAction Intervention { get; } = new BaseAction(ActionID.Intervention, true, isTimeline: true)
    {
        ChoiceTarget = TargetFilter.FindAttackedTarget,
    };

    /// <summary>
    /// 调停
    /// </summary>
    public static IBaseAction Intervene { get; } = new BaseAction(ActionID.Intervene, shouldEndSpecial: true)
    {
        ChoiceTarget = TargetFilter.FindTargetForMoving,
    };

    /// <summary>
    /// 赎罪剑
    /// </summary>
    public static IBaseAction Atonement { get; } = new BaseAction(ActionID.Atonement)
    {
        StatusNeed = new[] { StatusID.SwordOath },
    };

    /// <summary>
    /// 偿赎剑
    /// </summary>
    public static IBaseAction Expiacion { get; } = new BaseAction(ActionID.Expiacion);

    /// <summary>
    /// 英勇之剑
    /// </summary>
    public static IBaseAction BladeofValor { get; } = new BaseAction(ActionID.BladeofValor);

    /// <summary>
    /// 真理之剑
    /// </summary>
    public static IBaseAction BladeofTruth { get; } = new BaseAction(ActionID.BladeofTruth);

    /// <summary>
    /// 信念之剑
    /// </summary>
    public static IBaseAction BladeofFaith { get; } = new BaseAction(ActionID.BladeofFaith)
    {
        StatusNeed = new[] { StatusID.ReadyForBladeofFaith },
    };

    /// <summary>
    /// 安魂祈祷
    /// </summary>
    public static IBaseAction Requiescat { get; } = new BaseAction(ActionID.Requiescat, true);

    /// <summary>
    /// 悔罪
    /// </summary>
    public static IBaseAction Confiteor { get; } = new BaseAction(ActionID.Confiteor);

    /// <summary>
    /// 圣环
    /// </summary>
    public static IBaseAction HolyCircle { get; } = new BaseAction(ActionID.HolyCircle);

    /// <summary>
    /// 圣灵
    /// </summary>
    public static IBaseAction HolySpirit { get; } = new BaseAction(ActionID.HolySpirit);

    /// <summary>
    /// 武装戍卫
    /// </summary>
    public static IBaseAction PassageofArms { get; } = new BaseAction(ActionID.PassageofArms, true, isTimeline: true);

    /// <summary>
    /// 保护
    /// </summary>
    public static IBaseAction Cover { get; } = new BaseAction(ActionID.Cover, true, isTimeline: true)
    {
        ChoiceTarget = TargetFilter.FindAttackedTarget,
        ActionCheck = b => OathGauge >= 50,
    };

    /// <summary>
    /// 盾阵
    /// </summary>
    public static IBaseAction Sheltron { get; } = new BaseAction(ActionID.Sheltron, isTimeline: true)
    {
        ActionCheck = Cover.ActionCheck,
    };

    private protected override bool EmergencyAbility(byte abilityRemain, IAction nextGCD, out IAction act)
    {
        if (HallowedGround.ShouldUse(out act) && BaseAction.TankBreakOtherCheck(JobIDs[0], HallowedGround.Target)) return true;
        //神圣领域 如果谢不够了。
        return base.EmergencyAbility(abilityRemain, nextGCD, out act);
    }

    private protected override bool MoveForwardAbility(byte abilityRemain, out IAction act)
    {
        //调停
        if (Intervene.ShouldUse(out act, emptyOrSkipCombo: true)) return true;
        return false;
    }

    private protected override bool HealSingleGCD(out IAction act)
    {
        //深仁厚泽
        if (Clemency.ShouldUse(out act)) return true;
        return false;
    }
}
