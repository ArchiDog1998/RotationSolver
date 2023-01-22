using Dalamud.Game.ClientState.JobGauge.Types;
using RotationSolver.Actions;
using RotationSolver.Data;
using RotationSolver.Helpers;
using RotationSolver.Actions.BaseAction;

namespace RotationSolver.Rotations.Basic;

internal abstract class WAR_Base : CustomRotation.CustomRotation
{
    private static WARGauge JobGauge => Service.JobGauges.Get<WARGauge>();

    public sealed override ClassJobID[] JobIDs => new ClassJobID[] { ClassJobID.Warrior, ClassJobID.Marauder };
    private sealed protected override IBaseAction Shield => Defiance;

    /// <summary>
    /// 守护
    /// </summary>
    public static IBaseAction Defiance { get; } = new BaseAction(ActionID.Defiance, shouldEndSpecial: true);

    /// <summary>
    /// 重劈
    /// </summary>
    public static IBaseAction HeavySwing { get; } = new BaseAction(ActionID.HeavySwing);

    /// <summary>
    /// 凶残裂
    /// </summary>
    public static IBaseAction Maim { get; } = new BaseAction(ActionID.Maim);

    /// <summary>
    /// 暴风斩 绿斧
    /// </summary>
    public static IBaseAction StormsPath { get; } = new BaseAction(ActionID.StormsPath);

    /// <summary>
    /// 暴风碎 红斧
    /// </summary>
    public static IBaseAction StormsEye { get; } = new BaseAction(ActionID.StormsEye)
    {
        ActionCheck = b => Player.WillStatusEndGCD(3, 0, true, StatusID.SurgingTempest),
    };

    /// <summary>
    /// 飞斧
    /// </summary>
    public static IBaseAction Tomahawk { get; } = new BaseAction(ActionID.Tomahawk)
    {
        FilterForTarget = b => TargetFilter.ProvokeTarget(b),
    };

    /// <summary>
    /// 猛攻
    /// </summary>
    public static IBaseAction Onslaught { get; } = new BaseAction(ActionID.Onslaught, shouldEndSpecial: true)
    {
        ChoiceTarget = TargetFilter.FindTargetForMoving,
    };

    /// <summary>
    /// 动乱    
    /// </summary>
    public static IBaseAction Upheaval { get; } = new BaseAction(ActionID.Upheaval)
    {
        StatusNeed = new StatusID[] { StatusID.SurgingTempest },
    };

    /// <summary>
    /// 超压斧
    /// </summary>
    public static IBaseAction Overpower { get; } = new BaseAction(ActionID.Overpower);

    /// <summary>
    /// 秘银暴风
    /// </summary>
    public static IBaseAction MythrilTempest { get; } = new BaseAction(ActionID.MythrilTempest);

    /// <summary>
    /// 群山隆起
    /// </summary>
    public static IBaseAction Orogeny { get; } = new BaseAction(ActionID.Orogeny);

    /// <summary>
    /// 原初之魂
    /// </summary>
    public static IBaseAction InnerBeast { get; } = new BaseAction(ActionID.InnerBeast)
    {
        ActionCheck = b => JobGauge.BeastGauge >= 50 || Player.HasStatus(true, StatusID.InnerRelease),
    };

    /// <summary>
    /// 原初的解放
    /// </summary>
    public static IBaseAction InnerRelease { get; } = new BaseAction(ActionID.InnerRelease)
    {
        ActionCheck = InnerBeast.ActionCheck,
    };

    /// <summary>
    /// 钢铁旋风
    /// </summary>
    public static IBaseAction SteelCyclone { get; } = new BaseAction(ActionID.SteelCyclone)
    {
        ActionCheck = InnerBeast.ActionCheck,
    };

    /// <summary>
    /// 战嚎
    /// </summary>
    public static IBaseAction Infuriate { get; } = new BaseAction(ActionID.Infuriate)
    {
        StatusProvide = new[] { StatusID.InnerRelease },
        ActionCheck = b => HaveHostilesInRange && JobGauge.BeastGauge < 50 && InCombat,
    };

    /// <summary>
    /// 狂暴
    /// </summary>
    public static IBaseAction Berserk { get; } = new BaseAction(ActionID.Berserk)
    {
        ActionCheck = b => HaveHostilesInRange && !InnerRelease.IsCoolDown,
    };

    /// <summary>
    /// 战栗
    /// </summary>
    public static IBaseAction ThrillofBattle { get; } = new BaseAction(ActionID.ThrillofBattle, true, isTimeline: true);

    /// <summary>
    /// 泰然自若
    /// </summary>
    public static IBaseAction Equilibrium { get; } = new BaseAction(ActionID.Equilibrium, true, isTimeline: true);

    /// <summary>
    /// 原初的勇猛
    /// </summary>
    public static IBaseAction NascentFlash { get; } = new BaseAction(ActionID.NascentFlash, isTimeline: true)
    {
        ChoiceTarget = TargetFilter.FindAttackedTarget,
    };

    /// <summary>
    /// 复仇
    /// </summary>
    public static IBaseAction Vengeance { get; } = new BaseAction(ActionID.Vengeance, isTimeline: true)
    {
        StatusProvide = Rampart.StatusProvide,
        ActionCheck = BaseAction.TankDefenseSelf,
    };

    /// <summary>
    /// 原初的直觉
    /// </summary>
    public static IBaseAction RawIntuition { get; } = new BaseAction(ActionID.RawIntuition, isTimeline: true)
    {
        ActionCheck = BaseAction.TankDefenseSelf,
    };

    /// <summary>
    /// 摆脱
    /// </summary>
    public static IBaseAction ShakeItOff { get; } = new BaseAction(ActionID.ShakeItOff, true, isTimeline: true);

    /// <summary>
    /// 死斗
    /// </summary>
    public static IBaseAction Holmgang { get; } = new BaseAction(ActionID.Holmgang, isTimeline: true)
    {
        ChoiceTarget = (tars, mustUse) => Player,
    };

    /// <summary>
    /// 蛮荒崩裂
    /// </summary>
    public static IBaseAction PrimalRend { get; } = new BaseAction(ActionID.PrimalRend)
    {
        StatusNeed = new[] { StatusID.PrimalRendReady }
    };

    private protected override bool EmergencyAbility(byte abilityRemain, IAction nextGCD, out IAction act)
    {
        //死斗 如果血不够了。
        if (Holmgang.CanUse(out act) && BaseAction.TankBreakOtherCheck(JobIDs[0], Holmgang.Target)) return true;
        return base.EmergencyAbility(abilityRemain, nextGCD, out act);
    }

    private protected override bool MoveForwardAbility(byte abilityRemain, out IAction act)
    {
        //突进
        if (Onslaught.CanUse(out act, emptyOrSkipCombo: true)) return true;
        return false;
    }
}
