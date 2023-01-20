using Dalamud.Game.ClientState.JobGauge.Types;
using RotationSolver.Actions;
using RotationSolver.Data;
using RotationSolver.Helpers;
using RotationSolver.Actions.BaseAction;

namespace RotationSolver.Rotations.Basic;
internal abstract class DRK_Base : CustomRotation.CustomRotation
{
    private static DRKGauge JobGauge => Service.JobGauges.Get<DRKGauge>();
    protected static ushort DarksideTimeRemaining => JobGauge.DarksideTimeRemaining;
    /// <summary>
    /// 暗血
    /// </summary>
    protected static byte Blood => JobGauge.Blood;

    /// <summary>
    /// 有黑心心
    /// </summary>
    protected static bool HasDarkArts => JobGauge.HasDarkArts;

    /// <summary>
    /// 这个buff还剩多久就要没了啊
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    protected static bool DarkSideEndAfter(float time)
    {
        return EndAfter(JobGauge.DarksideTimeRemaining / 1000f, time);
    }

    /// <summary>
    /// 这个buff还剩多久就要没了啊
    /// </summary>
    /// <param name="abilityCount"></param>
    /// <param name="gctCount"></param>
    /// <returns></returns>
    protected static bool DarkSideEndAfterGCD(uint gctCount = 0, uint abilityCount = 0)
    {
        return EndAfterGCD(JobGauge.DarksideTimeRemaining / 1000f, gctCount, abilityCount);
    }

    public sealed override ClassJobID[] JobIDs => new ClassJobID[] { ClassJobID.DarkKnight };
    private sealed protected override IBaseAction Shield => Grit;

    /// <summary>
    /// 重斩
    /// </summary>
    public static IBaseAction HardSlash { get; } = new BaseAction(ActionID.HardSlash);

    /// <summary>
    /// 吸收斩
    /// </summary>
    public static IBaseAction SyphonStrike { get; } = new BaseAction(ActionID.SyphonStrike);

    /// <summary>
    /// 释放
    /// </summary>
    public static IBaseAction Unleash { get; } = new BaseAction(ActionID.Unleash);

    /// <summary>
    /// 深恶痛绝
    /// </summary>
    public static IBaseAction Grit { get; } = new BaseAction(ActionID.Grit, shouldEndSpecial: true);

    /// <summary>
    /// 伤残
    /// </summary>
    public static IBaseAction Unmend { get; } = new BaseAction(ActionID.Unmend)
    {
        FilterForTarget = b => TargetFilter.ProvokeTarget(b),
    };

    /// <summary>
    /// 噬魂斩
    /// </summary>
    public static IBaseAction Souleater { get; } = new BaseAction(ActionID.Souleater);

    /// <summary>
    /// 暗黑波动
    /// </summary>
    public static IBaseAction FloodofDarkness { get; } = new BaseAction(ActionID.FloodofDarkness);

    /// <summary>
    /// 暗黑锋
    /// </summary>
    public static IBaseAction EdgeofDarkness { get; } = new BaseAction(ActionID.EdgeofDarkness);

    /// <summary>
    /// 嗜血
    /// </summary>
    public static IBaseAction BloodWeapon { get; } = new BaseAction(ActionID.BloodWeapon);

    /// <summary>
    /// 暗影墙
    /// </summary>
    public static IBaseAction ShadowWall { get; } = new BaseAction(ActionID.ShadowWall, true, isTimeline: true)
    {
        StatusProvide = Rampart.StatusProvide,
        ActionCheck = BaseAction.TankDefenseSelf,
    };

    /// <summary>
    /// 弃明投暗
    /// </summary>
    public static IBaseAction DarkMind { get; } = new BaseAction(ActionID.DarkMind, true, isTimeline: true)
    {
        ActionCheck = BaseAction.TankDefenseSelf,
    };

    /// <summary>
    /// 行尸走肉
    /// </summary>
    public static IBaseAction LivingDead { get; } = new BaseAction(ActionID.LivingDead, true, isTimeline: true);

    /// <summary>
    /// 腐秽大地
    /// </summary>
    public static IBaseAction SaltedEarth { get; } = new BaseAction(ActionID.SaltedEarth);

    /// <summary>
    /// 跳斩
    /// </summary>
    public static IBaseAction Plunge { get; } = new BaseAction(ActionID.Plunge, shouldEndSpecial: true)
    {
        ChoiceTarget = TargetFilter.FindTargetForMoving
    };

    /// <summary>
    /// 吸血深渊
    /// </summary>
    public static IBaseAction AbyssalDrain { get; } = new BaseAction(ActionID.AbyssalDrain);

    /// <summary>
    /// 精雕怒斩
    /// </summary>
    public static IBaseAction CarveandSpit { get; } = new BaseAction(ActionID.CarveandSpit);

    /// <summary>
    /// 血溅
    /// </summary>
    public static IBaseAction Bloodspiller { get; } = new BaseAction(ActionID.Bloodspiller)
    {
        ActionCheck = b => JobGauge.Blood >= 50 || Player.HasStatus(true, StatusID.Delirium),
    };

    /// <summary>
    /// 寂灭
    /// </summary>
    public static IBaseAction Quietus { get; } = new BaseAction(ActionID.Quietus)
    {
        ActionCheck = Bloodspiller.ActionCheck,
    };

    /// <summary>
    /// 血乱
    /// </summary>
    public static IBaseAction Delirium { get; } = new BaseAction(ActionID.Delirium);

    /// <summary>
    /// 至黑之夜
    /// </summary>
    public static IBaseAction TheBlackestNight { get; } = new BaseAction(ActionID.TheBlackestNight, isTimeline: true)
    {
        ChoiceTarget = TargetFilter.FindAttackedTarget,
    };

    /// <summary>
    /// 刚魂
    /// </summary>
    public static IBaseAction StalwartSoul { get; } = new BaseAction(ActionID.StalwartSoul);

    /// <summary>
    /// 暗黑布道
    /// </summary>
    public static IBaseAction DarkMissionary { get; } = new BaseAction(ActionID.DarkMissionary, true, isTimeline: true);

    /// <summary>
    /// 掠影示现
    /// </summary>
    public static IBaseAction LivingShadow { get; } = new BaseAction(ActionID.LivingShadow)
    {
        ActionCheck = b => JobGauge.Blood >= 50,
    };

    /// <summary>
    /// 献奉
    /// </summary>
    public static IBaseAction Oblation { get; } = new BaseAction(ActionID.Oblation, true, isTimeline: true)
    {
        ChoiceTarget = TargetFilter.FindAttackedTarget,
    };

    /// <summary>
    /// 暗影使者
    /// </summary>
    public static IBaseAction Shadowbringer { get; } = new BaseAction(ActionID.Shadowbringer)
    {
        ActionCheck = b => JobGauge.DarksideTimeRemaining > 0,
    };

    /// <summary>
    /// 腐秽黑暗
    /// </summary>
    public static IBaseAction SaltandDarkness { get; } = new BaseAction(ActionID.SaltandDarkness)
    {
        StatusNeed = new[] { StatusID.SaltedEarth },
    };

    private protected override bool EmergencyAbility(byte abilityRemain, IAction nextGCD, out IAction act)
    {
        //行尸走肉
        if (LivingDead.ShouldUse(out act) && BaseAction.TankBreakOtherCheck(JobIDs[0], LivingDead.Target)) return true;

        return base.EmergencyAbility(abilityRemain, nextGCD, out act);
    }

    private protected override bool MoveForwardAbility(byte abilityRemain, out IAction act)
    {
        if (Plunge.ShouldUse(out act, emptyOrSkipCombo: true)) return true;

        return false;
    }
}