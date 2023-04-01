using Dalamud.Game.ClientState.JobGauge.Types;
using RotationSolver.Actions.BaseAction;
using RotationSolver.Basic.Actions;
using RotationSolver.Basic.Attributes;
using RotationSolver.Basic.Data;
using RotationSolver.Basic.Helpers;
using RotationSolver.Rotations.CustomRotation;
using static FFXIVClientStructs.FFXIV.Client.UI.Misc.ConfigModule;

namespace RotationSolver.Basic.Rotations.Basic;

public abstract class PLD_Base : CustomRotation
{
    private static PLDGauge JobGauge => Service.JobGauges.Get<PLDGauge>();
    public override MedicineType MedicineType => MedicineType.Strength;


    protected static bool HasDivineMight => !Player.WillStatusEndGCD(0, 0, true, StatusID.DivineMight);

    protected static bool HasFightOrFlight => !Player.WillStatusEndGCD(0, 0, true, StatusID.FightOrFlight);

    /// <summary>
    /// 忠义度
    /// </summary>
    protected static byte OathGauge => JobGauge.OathGauge;

    public sealed override ClassJobID[] JobIDs => new ClassJobID[] { ClassJobID.Paladin, ClassJobID.Gladiator };

    private sealed protected override IBaseAction TankStance => IronWill;

    protected override bool CanHealSingleSpell => DataCenter.PartyMembers.Count() == 1 && base.CanHealSingleSpell;
    protected override bool CanHealAreaAbility => false;

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
        }
    };

    /// <summary>
    /// 战女神之怒(王权剑)
    /// </summary>
    public static IBaseAction RageOfHalone { get; } = new BaseAction(ActionID.RageOfHalone);

    /// <summary>
    /// 投盾
    /// </summary>
    public static IBaseAction ShieldLob { get; } = new BaseAction(ActionID.ShieldLob)
    {
        FilterForHostiles = TargetFilter.TankRangeTarget,
        ActionCheck = b => !IsLastAction(IActionHelper.MovingActions),
    };

    /// <summary>
    /// 战逃反应
    /// </summary>
    public static IBaseAction FightOrFlight { get; } = new BaseAction(ActionID.FightOrFlight, true);

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
    public static IBaseAction CircleOfScorn { get; } = new BaseAction(ActionID.CircleOfScorn);

    /// <summary>
    /// 深奥之灵
    /// </summary>
    public static IBaseAction SpiritsWithin { get; } = new BaseAction(ActionID.SpiritsWithin);

    /// <summary>
    /// 神圣领域
    /// </summary>
    public static IBaseAction HallowedGround { get; } = new BaseAction(ActionID.HallowedGround, true, isTimeline: true);

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
    public static IBaseAction PassageOfArms { get; } = new BaseAction(ActionID.PassageOfArms, true, isTimeline: true);

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
    public static IBaseAction Sheltron { get; } = new BaseAction(ActionID.Sheltron, true, isTimeline: true)
    {
        ActionCheck = Cover.ActionCheck,
    };

    public static IBaseAction Bulwark { get; } = new BaseAction(ActionID.Bulwark, true, isTimeline: true)
    {
        StatusProvide = Rampart.StatusProvide,
    };


    protected override bool EmergencyAbility(byte abilitiesRemaining, IAction nextGCD, out IAction act)
    {
        if (HallowedGround.CanUse(out act) && BaseAction.TankBreakOtherCheck(JobIDs[0])) return true;
        return base.EmergencyAbility(abilitiesRemaining, nextGCD, out act);
    }

    [RotationDesc(ActionID.Intervene)]
    protected sealed override bool MoveForwardAbility(byte abilitiesRemaining, out IAction act, CanUseOption option = CanUseOption.None)
    {
        if (Intervene.CanUse(out act, CanUseOption.EmptyOrSkipCombo | option)) return true;
        return false;
    }

    [RotationDesc(ActionID.Clemency)]
    protected sealed override bool HealSingleGCD(out IAction act)
    {
        if (Clemency.CanUse(out act)) return true;
        return false;
    }
}
