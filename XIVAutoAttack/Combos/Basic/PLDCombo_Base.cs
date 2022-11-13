using Dalamud.Game.ClientState.JobGauge.Types;
using System;
using XIVAutoAttack.Actions.BaseAction;
using XIVAutoAttack.Combos.CustomCombo;
using XIVAutoAttack.Data;
using XIVAutoAttack.Helpers;
using XIVAutoAttack.Updaters;

namespace XIVAutoAttack.Combos.Basic;

internal abstract class PLDCombo_Base<TCmd> : JobGaugeCombo<PLDGauge, TCmd> where TCmd : Enum
{

    public sealed override ClassJobID[] JobIDs => new ClassJobID[] { ClassJobID.Paladin, ClassJobID.Gladiator };

    internal sealed override bool HaveShield => Player.HaveStatus(true, StatusID.IronWill);

    private sealed protected override BaseAction Shield => IronWill;

    protected override bool CanHealSingleSpell => TargetUpdater.PartyMembers.Length == 1 && base.CanHealSingleSpell;

    /// <summary>
    /// 钢铁信念
    /// </summary>
    public static BaseAction IronWill { get; } = new(ActionID.IronWill, shouldEndSpecial: true);

    /// <summary>
    /// 先锋剑
    /// </summary>
    public static BaseAction FastBlade { get; } = new(ActionID.FastBlade);

    /// <summary>
    /// 暴乱剑
    /// </summary>
    public static BaseAction RiotBlade { get; } = new(ActionID.RiotBlade);

    /// <summary>
    /// 沥血剑
    /// </summary>
    public static BaseAction GoringBlade { get; } = new(ActionID.GoringBlade, isEot: true)
    {
        TargetStatus = new[]
        {
                    StatusID.GoringBlade,
                    StatusID.BladeofValor,
            }
    };

    /// <summary>
    /// 战女神之怒
    /// </summary>
    public static BaseAction RageofHalone { get; } = new(ActionID.RageofHalone);

    /// <summary>
    /// 王权剑
    /// </summary>
    public static BaseAction RoyalAuthority { get; } = new(ActionID.RoyalAuthority);

    /// <summary>
    /// 投盾
    /// </summary>
    public static BaseAction ShieldLob { get; } = new(ActionID.ShieldLob)
    {
        FilterForTarget = b => TargetFilter.ProvokeTarget(b),
    };

    /// <summary>
    /// 战逃反应
    /// </summary>
    public static BaseAction FightorFlight { get; } = new(ActionID.FightorFlight)
    {
        OtherCheck = b =>
        {
            return true;
        },
    };

    /// <summary>
    /// 全蚀斩
    /// </summary>
    public static BaseAction TotalEclipse { get; } = new(ActionID.TotalEclipse);

    /// <summary>
    /// 日珥斩
    /// </summary>
    public static BaseAction Prominence { get; } = new(ActionID.Prominence);

    /// <summary>
    /// 预警
    /// </summary>
    public static BaseAction Sentinel { get; } = new(ActionID.Sentinel)
    {
        BuffsProvide = Rampart.BuffsProvide,
        OtherCheck = BaseAction.TankDefenseSelf,
    };

    /// <summary>
    /// 厄运流转
    /// </summary>
    public static BaseAction CircleofScorn { get; } = new(ActionID.CircleofScorn)
    {
        //OtherCheck = b =>
        //{
        //    if (LocalPlayer.HaveStatus(ObjectStatus.FightOrFlight)) return true;

        //    if (FightorFlight.IsCoolDown) return true;

        //    return false;
        //}
    };

    /// <summary>
    /// 深奥之灵
    /// </summary>
    public static BaseAction SpiritsWithin { get; } = new(ActionID.SpiritsWithin)
    {
        //OtherCheck = b =>
        //{
        //    if (LocalPlayer.HaveStatus(ObjectStatus.FightOrFlight)) return true;

        //    if (FightorFlight.IsCoolDown) return true;

        //    return false;
        //}
    };

    /// <summary>
    /// 神圣领域
    /// </summary>
    public static BaseAction HallowedGround { get; } = new(ActionID.HallowedGround)
    {
        OtherCheck = BaseAction.TankBreakOtherCheck,
    };
    
    /// <summary>
    /// 圣光幕帘
    /// </summary>
    public static BaseAction DivineVeil { get; } = new(ActionID.DivineVeil);

    /// <summary>
    /// 深仁厚泽
    /// </summary>
    public static BaseAction Clemency { get; } = new(ActionID.Clemency, true, true);

    /// <summary>
    /// 干预
    /// </summary>
    public static BaseAction Intervention { get; } = new(ActionID.Intervention, true)
    {
        ChoiceTarget = TargetFilter.FindAttackedTarget,
    };

    /// <summary>
    /// 调停
    /// </summary>
    public static BaseAction Intervene { get; } = new(ActionID.Intervene, shouldEndSpecial: true)
    {
        ChoiceTarget = TargetFilter.FindTargetForMoving,
    };

    /// <summary>
    /// 赎罪剑
    /// </summary>
    public static BaseAction Atonement { get; } = new(ActionID.Atonement)
    {
        BuffsNeed = new[] { StatusID.SwordOath },
    };

    /// <summary>
    /// 偿赎剑
    /// </summary>
    public static BaseAction Expiacion { get; } = new(ActionID.Expiacion);

    /// <summary>
    /// 英勇之剑
    /// </summary>
    public static BaseAction BladeofValor { get; } = new(ActionID.BladeofValor);

    /// <summary>
    /// 真理之剑
    /// </summary>
    public static BaseAction BladeofTruth { get; } = new(ActionID.BladeofTruth);

    /// <summary>
    /// 信念之剑
    /// </summary>
    public static BaseAction BladeofFaith { get; } = new(ActionID.BladeofFaith)
    {
        BuffsNeed = new[] { StatusID.ReadyForBladeofFaith },
    };

    /// <summary>
    /// 安魂祈祷
    /// </summary>
    public static BaseAction Requiescat { get; } = new(ActionID.Requiescat);

    /// <summary>
    /// 悔罪
    /// </summary>
    public static BaseAction Confiteor { get; } = new(ActionID.Confiteor)
    {
        OtherCheck = b => Player.CurrentMp >= 1000,
    };

    /// <summary>
    /// 圣环
    /// </summary>
    public static BaseAction HolyCircle { get; } = new(ActionID.HolyCircle)
    {
        OtherCheck = b => Player.CurrentMp >= 2000,
    };

    /// <summary>
    /// 圣灵
    /// </summary>
    public static BaseAction HolySpirit { get; } = new(ActionID.HolySpirit)
    {
        OtherCheck = b => Player.CurrentMp >= 2000,
    };

    /// <summary>
    /// 武装戍卫
    /// </summary>
    public static BaseAction PassageofArms { get; } = new(ActionID.PassageofArms);

    /// <summary>
    /// 保护
    /// </summary>
    public static BaseAction Cover { get; } = new(ActionID.Cover, true)
    {
        ChoiceTarget = TargetFilter.FindAttackedTarget,
    };

    /// <summary>
    /// 盾阵
    /// </summary>
    public static BaseAction Sheltron { get; } = new(ActionID.Sheltron);
    //盾牌猛击
    //ShieldBash = new BaseAction(16),
}
