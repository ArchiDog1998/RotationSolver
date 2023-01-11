using Dalamud.Game.ClientState.JobGauge.Types;
using System;
using System.Linq;
using AutoAction.Actions;
using AutoAction.Actions.BaseAction;
using AutoAction.Combos.CustomCombo;
using AutoAction.Data;
using AutoAction.Helpers;
using AutoAction.Updaters;

namespace AutoAction.Combos.Basic;

internal abstract class RDMCombo_Base<TCmd> : CustomCombo<TCmd> where TCmd : Enum
{
    private static RDMGauge JobGauge => Service.JobGauges.Get<RDMGauge>();

    /// <summary>
    /// 白魔元
    /// </summary>
    protected static byte WhiteMana => JobGauge.WhiteMana;

    /// <summary>
    /// 黑魔元
    /// </summary>
    protected static byte BlackMana => JobGauge.BlackMana;


    /// <summary>
    /// 魔元集
    /// </summary>
    protected static byte ManaStacks => JobGauge.ManaStacks;

    /// <summary>
    /// 连续咏唱
    /// </summary>
    protected static bool Dualcast => Player.HasStatus(true, StatusID.Dualcast);

    /// <summary>
    /// 赤火炎预备
    /// </summary>
    protected static bool VerfireReady => Player.HasStatus(true, StatusID.VerfireReady);

    /// <summary>
    /// 赤飞石预备
    /// </summary>
    protected static bool VerstoneReady => Player.HasStatus(true, StatusID.VerstoneReady);
    public sealed override ClassJobID[] JobIDs => new ClassJobID[] { ClassJobID.RedMage };
    protected override bool CanHealSingleSpell => TargetUpdater.PartyMembers.Count() == 1 && base.CanHealSingleSpell;

    private sealed protected override BaseAction Raise => Verraise;

    #region 单体
    /// <summary>
    /// 摇荡 震荡
    /// </summary>
    public static BaseAction Jolt { get; } = new(ActionID.Jolt)
    {
        BuffsProvide = new[] { StatusID.Dualcast },
    };

    /// <summary>
    /// 赤闪雷 赤暴雷
    /// </summary>
    public static BaseAction Verthunder { get; } = new(ActionID.Verthunder);

    /// <summary>
    /// 赤疾风 赤暴风
    /// </summary>
    public static BaseAction Veraero { get; } = new(ActionID.Veraero);

    /// <summary>
    /// 赤火炎
    /// </summary>
    public static BaseAction Verfire { get; } = new(ActionID.Verfire)
    {
        BuffsNeed = new[] { StatusID.VerfireReady },
        BuffsProvide = new[] { StatusID.Dualcast },
    };

    /// <summary>
    /// 赤飞石
    /// </summary>
    public static BaseAction Verstone { get; } = new(ActionID.Verstone)
    {
        BuffsNeed = new[] { StatusID.VerstoneReady },
        BuffsProvide = new[] { StatusID.Dualcast },
    };
    #endregion
    #region AoE
    /// <summary>
    /// 赤震雷
    /// </summary>
    public static BaseAction Verthunder2 { get; } = new(ActionID.Verthunder2)
    {
        BuffsProvide = new[] { StatusID.Dualcast },
    };

    /// <summary>
    /// 赤烈风
    /// </summary>
    public static BaseAction Veraero2 { get; } = new(ActionID.Veraero2)
    {
        BuffsProvide = new[] { StatusID.Dualcast },
    };

    /// <summary>
    /// 散碎
    /// </summary>
    public static BaseAction Scatter { get; } = new(ActionID.Scatter);
    #endregion
    #region 魔六连
    /// <summary>
    /// 回刺 魔回刺
    /// </summary>
    public static BaseAction Riposte { get; } = new(ActionID.Riposte)
    {
        ActionCheck = b => Math.Min(BlackMana, WhiteMana) >= 20,
    };

    /// <summary>
    /// 交击斩 魔交击斩
    /// </summary>
    public static BaseAction Zwerchhau { get; } = new(ActionID.Zwerchhau)
    {
        ActionCheck = b => Math.Min(BlackMana, WhiteMana) >= 15,
    };

    /// <summary>
    /// 连攻 魔连攻
    /// </summary>
    public static BaseAction Redoublement { get; } = new(ActionID.Redoublement)
    {
        ActionCheck = b => Math.Min(BlackMana, WhiteMana) >= 15,
    };

    /// <summary>
    /// 划圆斩
    /// </summary>
    public static BaseAction Moulinet { get; } = new(ActionID.Moulinet)
    {
        ActionCheck = b => Math.Min(BlackMana, WhiteMana) >= 20,
    };

    /// <summary>
    /// 赤核爆
    /// </summary>
    public static BaseAction Verflare { get; } = new(ActionID.Verflare)
    {
        ActionCheck = b => ManaStacks >= 3,
        BuffsProvide = new[] { StatusID.VerfireReady },
    };

    /// <summary>
    /// 赤神圣
    /// </summary>
    public static BaseAction Verholy { get; } = new(ActionID.Verholy)
    {
        ActionCheck = b => ManaStacks >= 3,
        BuffsProvide = new[] { StatusID.VerstoneReady },
    };

    /// <summary>
    /// 焦热
    /// </summary>
    public static BaseAction Scorch { get; } = new(ActionID.Scorch)
    {
        OtherIDsCombo = new[] { ActionID.Verflare, ActionID.Verholy },
    };

    /// <summary>
    /// 决断
    /// </summary>
    public static BaseAction Resolution { get; } = new(ActionID.Resolution)
    {
        OtherIDsCombo = new[] { ActionID.Scorch },
    };
    #endregion
    #region 进攻能力技
    /// <summary>
    /// 短兵相接
    /// </summary>
    public static BaseAction CorpsAcorps { get; } = new(ActionID.CorpsAcorps, shouldEndSpecial: true);

    /// <summary>
    /// 交剑
    /// </summary>
    public static BaseAction Engagement { get; } = new(ActionID.Engagement);

    /// <summary>
    /// 飞刺
    /// </summary>
    public static BaseAction Fleche { get; } = new(ActionID.Fleche);

    /// <summary>
    /// 促进
    /// </summary>
    public static BaseAction Acceleration { get; } = new(ActionID.Acceleration, true)
    {
        BuffsProvide = new[] { StatusID.Acceleration },
    };

    /// <summary>
    /// 六分反击
    /// </summary>
    public static BaseAction ContreSixte { get; } = new(ActionID.ContreSixte);

    /// <summary>
    /// 鼓励
    /// </summary>
    public static BaseAction Embolden { get; } = new(ActionID.Embolden, true)
    {
        BuffsProvide = new[] { StatusID.Embolden }
    };

    /// <summary>
    /// 魔元化
    /// </summary>
    public static BaseAction Manafication { get; } = new(ActionID.Manafication)
    {
        ActionCheck = b => Math.Max(BlackMana, WhiteMana) <= 50 && InCombat,
        OtherIDsNot = new[] { ActionID.Riposte, ActionID.Zwerchhau, ActionID.Verflare, ActionID.Verholy, ActionID.Scorch, ActionID.Resolution },
    };
    #endregion
    #region 其他
    /// <summary>
    /// 赤复活
    /// </summary>
    public static BaseAction Verraise { get; } = new(ActionID.Verraise, true)
    {
        BuffsNeed = new[] { StatusID.Dualcast , StatusID.Swiftcast},
    };

    /// <summary>
    /// 赤治疗
    /// </summary>
    public static BaseAction Vercure { get; } = new(ActionID.Vercure, true)
    {
        BuffsProvide = new[] { StatusID.Dualcast , StatusID.Swiftcast },
    };

    /// <summary>
    /// 抗死
    /// </summary>
    public static BaseAction MagickBarrier { get; } = new(ActionID.MagickBarrier, true)
    {
        BuffsProvide = new[] {StatusID.MagickBarrier },
    };
    #endregion

    private protected override bool HealSingleGCD(out IAction act)
    {
        if (Vercure.ShouldUse(out act, mustUse: true)) return true;
        return false;
    }

    private protected override bool MoveAbility(byte abilityRemain, out IAction act)
    {
        if (CorpsAcorps.ShouldUse(out act, emptyOrSkipCombo: true)) return true;
        return false;
    }
}
