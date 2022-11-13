using Dalamud.Game.ClientState.JobGauge.Types;
using System;
using XIVAutoAttack.Actions;
using XIVAutoAttack.Actions.BaseAction;
using XIVAutoAttack.Combos.CustomCombo;
using XIVAutoAttack.Data;
using XIVAutoAttack.Helpers;

namespace XIVAutoAttack.Combos.Basic;
internal abstract class DRKCombo_Base<TCmd> : JobGaugeCombo<DRKGauge, TCmd> where TCmd : Enum
{
    public sealed override ClassJobID[] JobIDs => new ClassJobID[] { ClassJobID.DarkKnight };
    internal sealed override bool HaveShield => Player.HaveStatus(true, StatusID.Grit);
    private sealed protected override BaseAction Shield => Grit;

    /// <summary>
    /// 重斩
    /// </summary>
    public static BaseAction HardSlash { get; } = new(ActionID.HardSlash);

    /// <summary>
    /// 吸收斩
    /// </summary>
    public static BaseAction SyphonStrike { get; } = new(ActionID.SyphonStrike);

    /// <summary>
    /// 释放
    /// </summary>
    public static BaseAction Unleash { get; } = new(ActionID.Unleash);

    /// <summary>
    /// 深恶痛绝
    /// </summary>
    public static BaseAction Grit { get; } = new(ActionID.Grit, shouldEndSpecial: true);

    /// <summary>
    /// 伤残
    /// </summary>
    public static BaseAction Unmend { get; } = new(ActionID.Unmend)
    {
        FilterForTarget = b => TargetFilter.ProvokeTarget(b),
    };

    /// <summary>
    /// 噬魂斩
    /// </summary>
    public static BaseAction Souleater { get; } = new(ActionID.Souleater);

    /// <summary>
    /// 暗黑波动
    /// </summary>
    public static BaseAction FloodofDarkness { get; } = new(ActionID.FloodofDarkness);

    /// <summary>
    /// 暗黑锋
    /// </summary>
    public static BaseAction EdgeofDarkness { get; } = new(ActionID.EdgeofDarkness);

    /// <summary>
    /// 嗜血
    /// </summary>
    public static BaseAction BloodWeapon { get; } = new(ActionID.BloodWeapon);

    /// <summary>
    /// 暗影墙
    /// </summary>
    public static BaseAction ShadowWall { get; } = new(ActionID.ShadowWall, true)
    {
        BuffsProvide = Rampart.BuffsProvide,
        OtherCheck = BaseAction.TankDefenseSelf,
    };

    /// <summary>
    /// 弃明投暗
    /// </summary>
    public static BaseAction DarkMind { get; } = new(ActionID.DarkMind, true)
    {
        OtherCheck = BaseAction.TankDefenseSelf,
    };

    /// <summary>
    /// 行尸走肉
    /// </summary>
    public static BaseAction LivingDead { get; } = new(ActionID.LivingDead, true);

    /// <summary>
    /// 腐秽大地
    /// </summary>
    public static BaseAction SaltedEarth { get; } = new(ActionID.SaltedEarth);

    /// <summary>
    /// 跳斩
    /// </summary>
    public static BaseAction Plunge { get; } = new(ActionID.Plunge, shouldEndSpecial: true)
    {
        ChoiceTarget = TargetFilter.FindTargetForMoving
    };

    /// <summary>
    /// 吸血深渊
    /// </summary>
    public static BaseAction AbyssalDrain { get; } = new(ActionID.AbyssalDrain);

    /// <summary>
    /// 精雕怒斩
    /// </summary>
    public static BaseAction CarveandSpit { get; } = new(ActionID.CarveandSpit);

    /// <summary>
    /// 血溅
    /// </summary>
    public static BaseAction Bloodspiller { get; } = new(ActionID.Bloodspiller)
    {
        OtherCheck = b => JobGauge.Blood >= 50 || Player.HaveStatus(true, StatusID.Delirium),
    };

    /// <summary>
    /// 寂灭
    /// </summary>
    public static BaseAction Quietus { get; } = new(ActionID.Quietus)
    {
        OtherCheck = Bloodspiller.OtherCheck,
    };

    /// <summary>
    /// 血乱
    /// </summary>
    public static BaseAction Delirium { get; } = new(ActionID.Delirium);

    /// <summary>
    /// 至黑之夜
    /// </summary>
    public static BaseAction TheBlackestNight { get; } = new(ActionID.TheBlackestNight)
    {
        ChoiceTarget = TargetFilter.FindAttackedTarget,
    };

    /// <summary>
    /// 刚魂
    /// </summary>
    public static BaseAction StalwartSoul { get; } = new(ActionID.StalwartSoul);

    /// <summary>
    /// 暗黑布道
    /// </summary>
    public static BaseAction DarkMissionary { get; } = new(ActionID.DarkMissionary, true);

    /// <summary>
    /// 掠影示现
    /// </summary>
    public static BaseAction LivingShadow { get; } = new(ActionID.LivingShadow)
    {
        OtherCheck = b => JobGauge.Blood >= 50,
    };

    /// <summary>
    /// 献奉
    /// </summary>
    public static BaseAction Oblation { get; } = new(ActionID.Oblation, true)
    {
        ChoiceTarget = TargetFilter.FindAttackedTarget,
    };

    /// <summary>
    /// 暗影使者
    /// </summary>
    public static BaseAction Shadowbringer { get; } = new(ActionID.Shadowbringer)
    {
        OtherCheck = b => JobGauge.DarksideTimeRemaining > 0,
    };

    /// <summary>
    /// 腐秽黑暗
    /// </summary>
    public static BaseAction SaltandDarkness { get; } = new(ActionID.SaltandDarkness)
    {
        BuffsNeed = new[] { StatusID.SaltedEarth },
    };


    private protected override bool EmergercyAbility(byte abilityRemain, IAction nextGCD, out IAction act)
    {
        //行尸走肉
        if (LivingDead.ShouldUse(out act) && BaseAction.TankBreakOtherCheck(LivingDead.Target)) return true;

        return base.EmergercyAbility(abilityRemain, nextGCD, out act);
    }
}