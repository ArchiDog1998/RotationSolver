using Dalamud.Game.ClientState.JobGauge.Types;
using System;
using RotationSolver.Helpers;
using RotationSolver.Actions.BaseAction;
using RotationSolver.Combos.CustomCombo;
using RotationSolver.Actions;
using RotationSolver.Data;

namespace RotationSolver.Combos.Basic;

internal abstract class RPRCombo_Base : CustomCombo.CustomCombo
{
    private static RPRGauge JobGauge => Service.JobGauges.Get<RPRGauge>();

    /// <summary>
    /// 红色灵魂
    /// </summary>
    protected static byte Soul => JobGauge.Soul;
    /// <summary>
    /// 蓝色灵魂
    /// </summary>
    protected static byte Shroud => JobGauge.Shroud;
    /// <summary>
    /// 夜游魂
    /// </summary>
    protected static byte LemureShroud => JobGauge.LemureShroud;
    /// <summary>
    /// 虚无魂
    /// </summary>
    protected static byte VoidShroud => JobGauge.VoidShroud;
    /// <summary>
    /// 夜游魂附体
    /// </summary>
    protected static bool Enshrouded => Player.HasStatus(true, StatusID.Enshrouded);
    /// <summary>
    /// 妖异之镰状态
    /// </summary>
    protected static bool SoulReaver => Player.HasStatus(true, StatusID.SoulReaver);

    public sealed override ClassJobID[] JobIDs => new ClassJobID[] { ClassJobID.Reaper };

    public class PRPAction : BaseAction
    {
        internal override EnemyLocation EnermyLocation => Player.HasStatus(true, StatusID.Enshrouded)
            ? EnemyLocation.None : base.EnermyLocation;
        internal PRPAction(ActionID actionID, bool isFriendly = false, bool shouldEndSpecial = false)
            : base(actionID, isFriendly, shouldEndSpecial)
        {
        }
    }

    #region 单体
    /// <summary>
    /// 切割
    /// </summary>
    public static BaseAction Slice { get; } = new(ActionID.Slice)
    {
        ActionCheck = b => !Enshrouded && !SoulReaver,
    };

    /// <summary>
    /// 增盈切割
    /// </summary>
    public static BaseAction WaxingSlice { get; } = new(ActionID.WaxingSlice)
    {
        ActionCheck = Slice.ActionCheck,
    };

    /// <summary>
    /// 地狱切割
    /// </summary>
    public static BaseAction InfernalSlice { get; } = new(ActionID.InfernalSlice)
    {
        ActionCheck = Slice.ActionCheck,
    };

    /// <summary>
    /// 死亡之影
    /// </summary>
    public static BaseAction ShadowofDeath { get; } = new(ActionID.ShadowofDeath, isEot: true)
    {
        TargetStatus = new[] { StatusID.DeathsDesign },
        ActionCheck = b => !SoulReaver,
    };

    /// <summary>
    /// 灵魂切割
    /// </summary>
    public static BaseAction SoulSlice { get; } = new(ActionID.SoulSlice)
    {
        ActionCheck = b => !Enshrouded && !SoulReaver && Soul <= 50,
    };
    #endregion
    #region AoE
    /// <summary>
    /// 旋转钐割
    /// </summary>
    public static BaseAction SpinningScythe { get; } = new(ActionID.SpinningScythe)
    {
        ActionCheck = Slice.ActionCheck,
    };

    /// <summary>
    /// 噩梦钐割
    /// </summary>
    public static BaseAction NightmareScythe { get; } = new(ActionID.NightmareScythe)
    {
        ActionCheck = Slice.ActionCheck,
    };

    /// <summary>
    /// 死亡之涡
    /// </summary>
    public static BaseAction WhorlofDeath { get; } = new(ActionID.WhorlofDeath, isEot: true)
    {
        TargetStatus = new[] { StatusID.DeathsDesign },
        ActionCheck = ShadowofDeath.ActionCheck,
    };

    /// <summary>
    /// 灵魂钐割
    /// </summary>
    public static BaseAction SoulScythe { get; } = new(ActionID.SoulScythe)
    {
        ActionCheck = SoulSlice.ActionCheck,
    };
    #endregion
    #region 妖异之镰状态
    /// <summary>
    /// 绞决
    /// </summary>
    public static BaseAction Gibbet { get; } = new(ActionID.Gibbet)
    {
        BuffsNeed = new[] { StatusID.SoulReaver }
    };

    /// <summary>
    /// 缢杀
    /// </summary>
    public static BaseAction Gallows { get; } = new(ActionID.Gallows)
    {
        BuffsNeed = new[] { StatusID.SoulReaver }
    };

    /// <summary>
    /// 断首
    /// </summary>
    public static BaseAction Guillotine { get; } = new(ActionID.Guillotine)
    {
        BuffsNeed = new[] { StatusID.SoulReaver }
    };
    #endregion
    #region 红条50灵魂
    /// <summary>
    /// 隐匿挥割
    /// </summary>
    public static BaseAction BloodStalk { get; } = new(ActionID.BloodStalk)
    {
        BuffsProvide = new[] { StatusID.SoulReaver },
        ActionCheck = b => !SoulReaver && !Enshrouded && Soul >= 50
    };

    /// <summary>
    /// 束缚挥割
    /// </summary>
    public static BaseAction GrimSwathe { get; } = new(ActionID.GrimSwathe)
    {
        BuffsProvide = new[] { StatusID.SoulReaver },
        ActionCheck = BloodStalk.ActionCheck,
    };

    /// <summary>
    /// 暴食
    /// </summary>
    public static BaseAction Gluttony { get; } = new(ActionID.Gluttony)
    {
        BuffsProvide = new[] { StatusID.SoulReaver },
        ActionCheck = BloodStalk.ActionCheck,
    };
    #endregion
    #region 大爆发
    /// <summary>
    /// 神秘环
    /// </summary>
    public static BaseAction ArcaneCircle { get; } = new(ActionID.ArcaneCircle, true)
    {
        BuffsProvide = new[] { StatusID.CircleofSacrifice, StatusID.BloodsownCircle }
    };

    /// <summary>
    /// 大丰收
    /// </summary>
    public static BaseAction PlentifulHarvest { get; } = new(ActionID.PlentifulHarvest)
    {
        BuffsNeed = new[] { StatusID.ImmortalSacrifice },
        ActionCheck = b => !Player.HasStatus(true, StatusID.BloodsownCircle)
    };
    #endregion
    #region 蓝条50附体
    /// <summary>
    /// 夜游魂衣
    /// </summary>
    public static BaseAction Enshroud { get; } = new(ActionID.Enshroud)
    {
        BuffsProvide = new[] { StatusID.Enshrouded },
        ActionCheck = b => Shroud >= 50 && !SoulReaver && !Enshrouded
    };

    /// <summary>
    /// 团契
    /// </summary>
    public static BaseAction Communio { get; } = new(ActionID.Communio)
    {
        BuffsNeed = new[] { StatusID.Enshrouded },
        ActionCheck = b => LemureShroud == 1
    };

    /// <summary>
    /// 夜游魂切割
    /// </summary>
    public static BaseAction LemuresSlice { get; } = new(ActionID.LemuresSlice)
    {
        BuffsNeed = new[] { StatusID.Enshrouded },
        ActionCheck = b => VoidShroud >= 2,
    };

    /// <summary>
    /// 夜游魂钐割
    /// </summary>
    public static BaseAction LemuresScythe { get; } = new(ActionID.LemuresScythe)
    {
        BuffsNeed = new[] { StatusID.Enshrouded },
        ActionCheck = b => VoidShroud >= 2,
    };

    /// <summary>
    /// 虚无收割
    /// </summary>
    public static BaseAction VoidReaping { get; } = new(ActionID.VoidReaping)
    {
        BuffsNeed = new[] { StatusID.Enshrouded },
    };

    /// <summary>
    /// 交错收割
    /// </summary>
    public static BaseAction CrossReaping { get; } = new(ActionID.CrossReaping)
    {
        BuffsNeed = new[] { StatusID.Enshrouded },
    };

    /// <summary>
    /// 阴冷收割
    /// </summary>
    public static BaseAction GrimReaping { get; } = new(ActionID.GrimReaping)
    {
        BuffsNeed = new[] { StatusID.Enshrouded },
    };
    #endregion
    #region 杂项
    /// <summary>
    /// 勾刃
    /// </summary>
    public static BaseAction Harpe { get; } = new(ActionID.Harpe)
    {
        ActionCheck = b => !SoulReaver
    };

    /// <summary>
    /// 地狱入境
    /// </summary>
    public static BaseAction HellsIngress { get; } = new(ActionID.HellsIngress)
    {
        BuffsProvide = new[] { StatusID.EnhancedHarpe },
        ActionCheck = b => !Player.HasStatus(true, StatusID.Bind1, StatusID.Bind2)
    };

    /// <summary>
    /// 地狱出境
    /// </summary>
    public static BaseAction HellsEgress { get; } = new(ActionID.HellsEgress)
    {
        BuffsProvide = HellsIngress.BuffsProvide,
        ActionCheck = HellsIngress.ActionCheck
    };

    /// <summary>
    /// 播魂种
    /// </summary>
    public static BaseAction Soulsow { get; } = new(ActionID.Soulsow)
    {
        BuffsProvide = new[] { StatusID.Soulsow },
        ActionCheck = b => !InCombat,
    };

    /// <summary>
    /// 收获月
    /// </summary>
    public static BaseAction HarvestMoon { get; } = new(ActionID.HarvestMoon)
    {
        BuffsNeed = new[] { StatusID.Soulsow },
    };

    /// <summary>
    /// 神秘纹 加盾
    /// </summary>
    public static BaseAction ArcaneCrest { get; } = new(ActionID.ArcaneCrest, true, isTimeline: true);
    #endregion


    private protected override bool MoveAbility(byte abilityRemain, out IAction act)
    {
        //E上去
        if (HellsIngress.ShouldUse(out act, emptyOrSkipCombo: true)) return true;
        return false;
    }
}
