using Dalamud.Game.ClientState.JobGauge.Types;
using System;
using System.Linq;
using XIVAutoAttack.Actions;
using XIVAutoAttack.Actions.BaseAction;
using XIVAutoAttack.Combos.CustomCombo;
using XIVAutoAttack.Data;
using XIVAutoAttack.Helpers;
using XIVAutoAttack.Updaters;

namespace XIVAutoAttack.Combos.Basic;

internal abstract class RDMCombo_Base<TCmd> : CustomCombo<TCmd> where TCmd : Enum
{
    private static RDMGauge JobGauge => Service.JobGauges.Get<RDMGauge>();

    /// <summary>
    /// ��ħԪ
    /// </summary>
    protected static byte WhiteMana => JobGauge.WhiteMana;

    /// <summary>
    /// ��ħԪ
    /// </summary>
    protected static byte BlackMana => JobGauge.BlackMana;


    /// <summary>
    /// ħ����
    /// </summary>
    protected static byte ManaStacks => JobGauge.ManaStacks;

    public sealed override ClassJobID[] JobIDs => new ClassJobID[] { ClassJobID.RedMage };
    protected override bool CanHealSingleSpell => TargetUpdater.PartyMembers.Count() == 1 && base.CanHealSingleSpell;

    private sealed protected override BaseAction Raise => Verraise;

    /// <summary>
    /// �ิ��
    /// </summary>
    public static BaseAction Verraise { get; } = new(ActionID.Verraise, true);

    /// <summary>
    /// ��
    /// </summary>
    public static BaseAction Jolt { get; } = new(ActionID.Jolt)
    {
        BuffsProvide = Swiftcast.BuffsProvide.Union(new[] { StatusID.Acceleration }).ToArray(),
    };

    /// <summary>
    /// �ش�
    /// </summary>
    public static BaseAction Riposte { get; } = new(ActionID.Riposte)
    {
        ActionCheck = b => JobGauge.BlackMana >= 20 && JobGauge.WhiteMana >= 20,
    };

    /// <summary>
    /// ������
    /// </summary>
    public static BaseAction Verthunder { get; } = new(ActionID.Verthunder)
    {
        BuffsNeed = Jolt.BuffsProvide,
    };

    /// <summary>
    /// �̱����
    /// </summary>
    public static BaseAction CorpsAcorps { get; } = new(ActionID.CorpsAcorps, shouldEndSpecial: true)
    {
        ChoiceTarget = TargetFilter.FindTargetForMoving,
    };

    /// <summary>
    /// �༲��
    /// </summary>
    public static BaseAction Veraero { get; } = new(ActionID.Veraero)
    {
        BuffsNeed = Jolt.BuffsProvide,
    };

    /// <summary>
    /// ɢ��
    /// </summary>
    public static BaseAction Scatter { get; } = new(ActionID.Scatter)
    {
        BuffsNeed = Jolt.BuffsProvide,
    };

    /// <summary>
    /// ������
    /// </summary>
    public static BaseAction Verthunder2 { get; } = new(ActionID.Verthunder2)
    {
        BuffsProvide = Jolt.BuffsProvide,
    };

    /// <summary>
    /// ���ҷ�
    /// </summary>
    public static BaseAction Veraero2 { get; } = new(ActionID.Veraero2)
    {
        BuffsProvide = Jolt.BuffsProvide,
    };

    /// <summary>
    /// �����
    /// </summary>
    public static BaseAction Verfire { get; } = new(ActionID.Verfire)
    {
        BuffsNeed = new[] { StatusID.VerfireReady },
        BuffsProvide = Jolt.BuffsProvide,
    };

    /// <summary>
    /// ���ʯ
    /// </summary>
    public static BaseAction Verstone { get; } = new(ActionID.Verstone)
    {
        BuffsNeed = new[] { StatusID.VerstoneReady },
        BuffsProvide = Jolt.BuffsProvide,
    };

    /// <summary>
    /// ����ն
    /// </summary>
    public static BaseAction Zwerchhau { get; } = new(ActionID.Zwerchhau)
    {
        ActionCheck = b => BlackMana >= 15 && WhiteMana >= 15,
    };

    /// <summary>
    /// ����
    /// </summary>
    public static BaseAction Engagement { get; } = new(ActionID.Engagement);

    /// <summary>
    /// �ɽ�
    /// </summary>
    public static BaseAction Fleche { get; } = new(ActionID.Fleche);

    /// <summary>
    /// ����
    /// </summary>
    public static BaseAction Redoublement { get; } = new(ActionID.Redoublement)
    {
        ActionCheck = b => BlackMana >= 15 && WhiteMana >= 15,
    };


    /// <summary>
    /// �ٽ�
    /// </summary>
    public static BaseAction Acceleration { get; } = new(ActionID.Acceleration, true)
    {
        BuffsProvide = new[] { StatusID.Acceleration },
    };

    /// <summary>
    /// ��Բն
    /// </summary>
    public static BaseAction Moulinet { get; } = new(ActionID.Moulinet)
    {
        ActionCheck = b => BlackMana >= 20 && WhiteMana >= 20,
    };

    /// <summary>
    /// ������
    /// </summary>
    public static BaseAction Vercure { get; } = new(ActionID.Vercure, true)
    {
        BuffsProvide = Swiftcast.BuffsProvide.Union(Acceleration.BuffsProvide).ToArray(),
    };

    /// <summary>
    /// ���ַ���
    /// </summary>
    public static BaseAction ContreSixte { get; } = new(ActionID.ContreSixte);

    /// <summary>
    /// ����
    /// </summary>
    public static BaseAction Embolden { get; } = new(ActionID.Embolden, true);

    ///// <summary>
    ///// ��ն
    ///// </summary>
    //public static BaseAction Reprise { get; } = new(ActionID.Reprise);

    /// <summary>
    /// ����
    /// </summary>
    public static BaseAction MagickBarrier { get; } = new(ActionID.MagickBarrier, true);

    /// <summary>
    /// ��˱�
    /// </summary>
    public static BaseAction Verflare { get; } = new(ActionID.Verflare);

    /// <summary>
    /// ����ʥ
    /// </summary>
    public static BaseAction Verholy { get; } = new(ActionID.Verholy);

    /// <summary>
    /// ����
    /// </summary>
    public static BaseAction Scorch { get; } = new(ActionID.Scorch)
    {
        OtherIDsCombo = new[] { ActionID.Verholy },
    };

    /// <summary>
    /// ����
    /// </summary>
    public static BaseAction Resolution { get; } = new(ActionID.Resolution);

    /// <summary>
    /// ħԪ��
    /// </summary>
    public static BaseAction Manafication { get; } = new(ActionID.Manafication)
    {
        ActionCheck = b => WhiteMana <= 50 && BlackMana <= 50 && InCombat && ManaStacks == 0,
        OtherIDsNot = new[] { ActionID.Riposte, ActionID.Zwerchhau, ActionID.Scorch, ActionID.Verflare, ActionID.Verholy },
    };

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
