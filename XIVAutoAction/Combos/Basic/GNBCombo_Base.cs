using Dalamud.Game.ClientState.JobGauge.Types;
using System;
using XIVAutoAttack.Actions;
using XIVAutoAttack.Actions.BaseAction;
using XIVAutoAttack.Combos.CustomCombo;
using XIVAutoAttack.Data;
using XIVAutoAttack.Helpers;

namespace XIVAutoAttack.Combos.Basic;


internal abstract class GNBCombo_Base<TCmd> : CustomCombo<TCmd> where TCmd : Enum
{
    private static GNBGauge JobGauge => Service.JobGauges.Get<GNBGauge>();

    /// <summary>
    /// ��������
    /// </summary>
    protected static byte Ammo => JobGauge.Ammo;

    /// <summary>
    /// �����ĵڼ���combo
    /// </summary>
    protected static byte AmmoComboStep => JobGauge.AmmoComboStep;

    public sealed override ClassJobID[] JobIDs => new ClassJobID[] { ClassJobID.Gunbreaker };
    private sealed protected override BaseAction Shield => RoyalGuard;

    protected override bool CanHealSingleSpell => false;
    protected override bool CanHealAreaSpell => false;

    protected static byte MaxAmmo => Level >= 88 ? (byte)3 : (byte)2;

    /// <summary>
    /// ��������
    /// </summary>
    public static BaseAction RoyalGuard { get; } = new(ActionID.RoyalGuard, shouldEndSpecial: true);

    /// <summary>
    /// ����ն
    /// </summary>
    public static BaseAction KeenEdge { get; } = new(ActionID.KeenEdge);

    /// <summary>
    /// ����
    /// </summary>
    public static BaseAction NoMercy { get; } = new(ActionID.NoMercy);

    /// <summary>
    /// �б���
    /// </summary>
    public static BaseAction BrutalShell { get; } = new(ActionID.BrutalShell);

    /// <summary>
    /// αװ
    /// </summary>
    public static BaseAction Camouflage { get; } = new(ActionID.Camouflage, true)
    {
        ActionCheck = BaseAction.TankDefenseSelf,
    };

    /// <summary>
    /// ��ħ��
    /// </summary>
    public static BaseAction DemonSlice { get; } = new(ActionID.DemonSlice);

    /// <summary>
    /// ���׵�
    /// </summary>
    public static BaseAction LightningShot { get; } = new(ActionID.LightningShot);

    /// <summary>
    /// Σ������
    /// </summary>
    public static BaseAction DangerZone { get; } = new(ActionID.DangerZone);

    /// <summary>
    /// Ѹ��ն
    /// </summary>
    public static BaseAction SolidBarrel { get; } = new(ActionID.SolidBarrel);

    /// <summary>
    /// ������
    /// </summary>
    public static BaseAction BurstStrike { get; } = new(ActionID.BurstStrike)
    {
        ActionCheck = b => JobGauge.Ammo > 0,
    };

    /// <summary>
    /// ����
    /// </summary>
    public static BaseAction Nebula { get; } = new(ActionID.Nebula, true)
    {
        BuffsProvide = Rampart.BuffsProvide,
        ActionCheck = BaseAction.TankDefenseSelf,
    };

    /// <summary>
    /// ��ħɱ
    /// </summary>
    public static BaseAction DemonSlaughter { get; } = new(ActionID.DemonSlaughter);

    /// <summary>
    /// ����
    /// </summary>
    public static BaseAction Aurora { get; } = new BaseAction(ActionID.Aurora, true);

    /// <summary>
    /// ��������
    /// </summary>
    public static BaseAction Superbolide { get; } = new(ActionID.Superbolide, true);

    /// <summary>
    /// ������
    /// </summary>
    public static BaseAction SonicBreak { get; } = new(ActionID.SonicBreak);

    /// <summary>
    /// �ַ�ն
    /// </summary>
    public static BaseAction RoughDivide { get; } = new(ActionID.RoughDivide, shouldEndSpecial: true)
    {
        ChoiceTarget = TargetFilter.FindTargetForMoving,
    };

    /// <summary>
    /// ����
    /// </summary>
    public static BaseAction GnashingFang { get; } = new(ActionID.GnashingFang)
    {
        ActionCheck = b => JobGauge.AmmoComboStep == 0 && JobGauge.Ammo > 0,
    };

    /// <summary>
    /// ���γ岨
    /// </summary>
    public static BaseAction BowShock { get; } = new(ActionID.BowShock);

    /// <summary>
    /// ��֮��
    /// </summary>
    public static BaseAction HeartofLight { get; } = new(ActionID.HeartofLight, true);

    /// <summary>
    /// ʯ֮��
    /// </summary>
    public static BaseAction HeartofStone { get; } = new(ActionID.HeartofStone, true)
    {
        ChoiceTarget = TargetFilter.FindAttackedTarget,
    };

    /// <summary>
    /// ����֮��
    /// </summary>
    public static BaseAction FatedCircle { get; } = new(ActionID.FatedCircle)
    {
        ActionCheck = b => JobGauge.Ammo > 0,
    };

    /// <summary>
    /// Ѫ��
    /// </summary>
    public static BaseAction Bloodfest { get; } = new(ActionID.Bloodfest, true)
    {
        ActionCheck = b => MaxAmmo - JobGauge.Ammo > 1,
    };

    /// <summary>
    /// ����
    /// </summary>
    public static BaseAction DoubleDown { get; } = new(ActionID.DoubleDown)
    {
        ActionCheck = b => JobGauge.Ammo > 1,
    };

    /// <summary>
    /// ����צ
    /// </summary>
    public static BaseAction SavageClaw { get; } = new(ActionID.SavageClaw)
    {
        ActionCheck = b => Service.IconReplacer.OriginalHook(ActionID.GnashingFang) == ActionID.SavageClaw,
    };

    /// <summary>
    /// ����צ
    /// </summary>
    public static BaseAction WickedTalon { get; } = new(ActionID.WickedTalon)
    {
        ActionCheck = b => Service.IconReplacer.OriginalHook(ActionID.GnashingFang) == ActionID.WickedTalon,
    };

    /// <summary>
    /// ˺��
    /// </summary>
    public static BaseAction JugularRip { get; } = new(ActionID.JugularRip)
    {
        ActionCheck = b => Service.IconReplacer.OriginalHook(ActionID.Continuation) == ActionID.JugularRip,
    };

    /// <summary>
    /// ����
    /// </summary>
    public static BaseAction AbdomenTear { get; } = new(ActionID.AbdomenTear)
    {
        ActionCheck = b => Service.IconReplacer.OriginalHook(ActionID.Continuation) == ActionID.AbdomenTear,
    };

    /// <summary>
    /// ��Ŀ
    /// </summary>
    public static BaseAction EyeGouge { get; } = new(ActionID.EyeGouge)
    {
        ActionCheck = b => Service.IconReplacer.OriginalHook(ActionID.Continuation) == ActionID.EyeGouge,
    };

    /// <summary>
    /// ������
    /// </summary>
    public static BaseAction Hypervelocity { get; } = new(ActionID.Hypervelocity)
    {
        ActionCheck = b => Service.IconReplacer.OriginalHook(ActionID.Continuation)
        == ActionID.Hypervelocity,
    };

    private protected override bool EmergencyAbility(byte abilityRemain, IAction nextGCD, out IAction act)
    {
        //�������� ���л�����ˡ�
        if (Superbolide.ShouldUse(out act) && BaseAction.TankBreakOtherCheck(JobIDs[0], Superbolide.Target)) return true;
        return base.EmergencyAbility(abilityRemain, nextGCD, out act);
    }

    private protected override bool MoveAbility(byte abilityRemain, out IAction act)
    {
        //ͻ��
        if (RoughDivide.ShouldUse(out act, emptyOrSkipCombo: true)) return true;
        return false;
    }
}

