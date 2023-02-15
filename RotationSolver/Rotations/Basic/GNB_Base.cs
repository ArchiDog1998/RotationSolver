using Dalamud.Game.ClientState.JobGauge.Types;
using Dalamud.Game.ClientState.Objects.Types;
using RotationSolver.Actions;
using RotationSolver.Actions.BaseAction;
using RotationSolver.Data;
using RotationSolver.Helpers;

namespace RotationSolver.Rotations.Basic;


internal abstract class GNB_Base : CustomRotation.CustomRotation
{
    private static GNBGauge JobGauge => Service.JobGauges.Get<GNBGauge>();

    /// <summary>
    /// 晶囊数量
    /// </summary>
    protected static byte Ammo => JobGauge.Ammo;

    /// <summary>
    /// 烈牙的第几个combo
    /// </summary>
    protected static byte AmmoComboStep => JobGauge.AmmoComboStep;

    public sealed override ClassJobID[] JobIDs => new ClassJobID[] { ClassJobID.Gunbreaker };
    private sealed protected override IBaseAction Shield => RoyalGuard;

    protected override bool CanHealSingleSpell => false;
    protected override bool CanHealAreaSpell => false;

    protected static byte MaxAmmo => Level >= 88 ? (byte)3 : (byte)2;

    /// <summary>
    /// 王室亲卫
    /// </summary>
    public static IBaseAction RoyalGuard { get; } = new BaseAction(ActionID.RoyalGuard, shouldEndSpecial: true);

    /// <summary>
    /// 利刃斩
    /// </summary>
    public static IBaseAction KeenEdge { get; } = new BaseAction(ActionID.KeenEdge);

    /// <summary>
    /// 无情
    /// </summary>
    public static IBaseAction NoMercy { get; } = new BaseAction(ActionID.NoMercy);

    /// <summary>
    /// 残暴弹
    /// </summary>
    public static IBaseAction BrutalShell { get; } = new BaseAction(ActionID.BrutalShell);

    /// <summary>
    /// 伪装
    /// </summary>
    public static IBaseAction Camouflage { get; } = new BaseAction(ActionID.Camouflage, true, isTimeline: true)
    {
        ActionCheck = BaseAction.TankDefenseSelf,
    };

    /// <summary>
    /// 恶魔切
    /// </summary>
    public static IBaseAction DemonSlice { get; } = new BaseAction(ActionID.DemonSlice)
    {
        AOECount = 2,
    };

    /// <summary>
    /// 闪雷弹
    /// </summary>
    public static IBaseAction LightningShot { get; } = new BaseAction(ActionID.LightningShot);

    /// <summary>
    /// 危险领域
    /// </summary>
    public static IBaseAction DangerZone { get; } = new BaseAction(ActionID.DangerZone);

    /// <summary>
    /// 迅连斩
    /// </summary>
    public static IBaseAction SolidBarrel { get; } = new BaseAction(ActionID.SolidBarrel);

    /// <summary>
    /// 爆发击
    /// </summary>
    public static IBaseAction BurstStrike { get; } = new BaseAction(ActionID.BurstStrike)
    {
        ActionCheck = b => JobGauge.Ammo > 0,
    };

    /// <summary>
    /// 星云
    /// </summary>
    public static IBaseAction Nebula { get; } = new BaseAction(ActionID.Nebula, true, isTimeline: true)
    {
        StatusProvide = Rampart.StatusProvide,
        ActionCheck = BaseAction.TankDefenseSelf,
    };

    /// <summary>
    /// 恶魔杀
    /// </summary>
    public static IBaseAction DemonSlaughter { get; } = new BaseAction(ActionID.DemonSlaughter)
    {
        AOECount = 2,
    };

    /// <summary>
    /// 极光
    /// </summary>
    public static IBaseAction Aurora { get; } = new BaseAction(ActionID.Aurora, true, isTimeline: true);

    /// <summary>
    /// 超火流星
    /// </summary>
    public static IBaseAction Superbolide { get; } = new BaseAction(ActionID.Superbolide, true, isTimeline: true);

    /// <summary>
    /// 音速破
    /// </summary>
    public static IBaseAction SonicBreak { get; } = new BaseAction(ActionID.SonicBreak);

    /// <summary>
    /// 粗分斩
    /// </summary>
    public static IBaseAction RoughDivide { get; } = new BaseAction(ActionID.RoughDivide, shouldEndSpecial: true)
    {
        ChoiceTarget = TargetFilter.FindTargetForMoving,
    };

    /// <summary>
    /// 烈牙
    /// </summary>
    public static IBaseAction GnashingFang { get; } = new BaseAction(ActionID.GnashingFang)
    {
        ActionCheck = b => JobGauge.AmmoComboStep == 0 && JobGauge.Ammo > 0,
    };

    /// <summary>
    /// 弓形冲波
    /// </summary>
    public static IBaseAction BowShock { get; } = new BaseAction(ActionID.BowShock);

    /// <summary>
    /// 光之心
    /// </summary>
    public static IBaseAction HeartofLight { get; } = new BaseAction(ActionID.HeartofLight, true, isTimeline: true);

    /// <summary>
    /// 石之心
    /// </summary>
    public static IBaseAction HeartofStone { get; } = new BaseAction(ActionID.HeartofStone, true, isTimeline: true)
    {
        ChoiceTarget = TargetFilter.FindAttackedTarget,
    };

    /// <summary>
    /// 命运之环
    /// </summary>
    public static IBaseAction FatedCircle { get; } = new BaseAction(ActionID.FatedCircle)
    {
        ActionCheck = b => JobGauge.Ammo > 0,
    };

    /// <summary>
    /// 血壤
    /// </summary>
    public static IBaseAction Bloodfest { get; } = new BaseAction(ActionID.Bloodfest, true)
    {
        ActionCheck = b => MaxAmmo - JobGauge.Ammo > 1,
    };

    /// <summary>
    /// 倍攻
    /// </summary>
    public static IBaseAction DoubleDown { get; } = new BaseAction(ActionID.DoubleDown)
    {
        ActionCheck = b => JobGauge.Ammo > 1,
    };

    /// <summary>
    /// 猛兽爪
    /// </summary>
    public static IBaseAction SavageClaw { get; } = new BaseAction(ActionID.SavageClaw)
    {
        ActionCheck = b => Service.IconReplacer.OriginalHook(ActionID.GnashingFang) == ActionID.SavageClaw,
    };

    /// <summary>
    /// 凶禽爪
    /// </summary>
    public static IBaseAction WickedTalon { get; } = new BaseAction(ActionID.WickedTalon)
    {
        ActionCheck = b => Service.IconReplacer.OriginalHook(ActionID.GnashingFang) == ActionID.WickedTalon,
    };

    /// <summary>
    /// 撕喉
    /// </summary>
    public static IBaseAction JugularRip { get; } = new BaseAction(ActionID.JugularRip)
    {
        ActionCheck = b => Service.IconReplacer.OriginalHook(ActionID.Continuation) == ActionID.JugularRip,
    };

    /// <summary>
    /// 裂膛
    /// </summary>
    public static IBaseAction AbdomenTear { get; } = new BaseAction(ActionID.AbdomenTear)
    {
        ActionCheck = b => Service.IconReplacer.OriginalHook(ActionID.Continuation) == ActionID.AbdomenTear,
    };

    /// <summary>
    /// 穿目
    /// </summary>
    public static IBaseAction EyeGouge { get; } = new BaseAction(ActionID.EyeGouge)
    {
        ActionCheck = b => Service.IconReplacer.OriginalHook(ActionID.Continuation) == ActionID.EyeGouge,
    };

    /// <summary>
    /// 超高速
    /// </summary>
    public static IBaseAction Hypervelocity { get; } = new BaseAction(ActionID.Hypervelocity)
    {
        ActionCheck = b => Service.IconReplacer.OriginalHook(ActionID.Continuation)
        == ActionID.Hypervelocity,
    };

    private protected override bool EmergencyAbility(byte abilitiesRemaining, IAction nextGCD, out IAction act)
    {
        //超火流星 如果谢不够了。
        if (Superbolide.CanUse(out act) && BaseAction.TankBreakOtherCheck(JobIDs[0], Superbolide.Target)) return true;
        return base.EmergencyAbility(abilitiesRemaining, nextGCD, out act);
    }

    private protected sealed override bool MoveForwardAbility(byte abilitiesRemaining, out IAction act)
    {
        //突进
        if (RoughDivide.CanUse(out act, emptyOrSkipCombo: true)) return true;
        return false;
    }
}

