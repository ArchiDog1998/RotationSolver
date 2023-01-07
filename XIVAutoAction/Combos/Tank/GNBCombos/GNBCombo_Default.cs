using System.Collections.Generic;
using AutoAction.Actions;
using AutoAction.Combos.Basic;
using AutoAction.Combos.CustomCombo;
using AutoAction.Data;
using AutoAction.Helpers;
using static AutoAction.Combos.Tank.GNBCombos.GNBCombo_Default;

namespace AutoAction.Combos.Tank.GNBCombos;

internal sealed class GNBCombo_Default : GNBCombo_Base<CommandType>
{
    public override string GameVersion => "6.18";

    public override string Author => "ϫ��Moon";

    internal enum CommandType : byte
    {
        None,
    }

    protected override SortedList<CommandType, string> CommandDescription => new SortedList<CommandType, string>()
    {
        //{CommandType.None, "" }, //д��ע�Ͱ���������ʾ�û��ġ�
    };
    protected override bool CanHealSingleSpell => false;
    protected override bool CanHealAreaSpell => false;

    public override SortedList<DescType, string> DescriptionDict => new()
    {
        {DescType.HealSingle, $"{Aurora}"},
        {DescType.DefenseArea, $"{HeartofLight}"},
        {DescType.DefenseSingle, $"{HeartofStone}, {Nebula}, {Camouflage}"},
        {DescType.MoveAction, $"{RoughDivide}"},
    };

    private protected override bool GeneralGCD(out IAction act)
    {
        //����
        if (CanUseDoubleDown(out act)) return true;

        //����֮�� AOE
        if (FatedCircle.ShouldUse(out act)) return true;

        //AOE
        if (DemonSlaughter.ShouldUse(out act)) return true;
        if (DemonSlice.ShouldUse(out act)) return true;

        //����
        if (CanUseGnashingFang(out act)) return true;

        //������
        if (CanUseSonicBreak(out act)) return true;

        //���������
        if (WickedTalon.ShouldUse(out act, emptyOrSkipCombo: true)) return true;
        if (SavageClaw.ShouldUse(out act, emptyOrSkipCombo: true)) return true;

        //������   
        if (CanUseBurstStrike(out act)) return true;

        //��������
        //�������ʣ0.5����ȴ��,���ͷŻ�������,��Ҫ��Ϊ���ٲ�ͬ���ܻ�ʹ�����Ӻ�̫�������ж�һ��
        //if (GnashingFang.IsCoolDown && GnashingFang.WillHaveOneCharge(0.5f) && GnashingFang.EnoughLevel) return false;
        if (SolidBarrel.ShouldUse(out act)) return true;
        if (BrutalShell.ShouldUse(out act)) return true;
        if (KeenEdge.ShouldUse(out act)) return true;

        if (CommandController.Move && MoveAbility(1, out act)) return true;

        if (LightningShot.ShouldUse(out act)) return true;

        return false;
    }



    private protected override bool AttackAbility(byte abilityRemain, out IAction act)
    {
        //����,Ŀǰֻ��4GCD���ֵ��ж�
        if (SettingBreak && abilityRemain == 1 && CanUseNoMercy(out act)) return true;

        //Σ������
        if (DangerZone.ShouldUse(out act))
        {
            if (!IsFullParty && !DangerZone.IsTargetBoss) return true;

            //�ȼ�С������,
            if (!GnashingFang.EnoughLevel && (Player.HasStatus(true, StatusID.NoMercy) || !NoMercy.WillHaveOneCharge(15))) return true;

            //������,����֮��
            if (Player.HasStatus(true, StatusID.NoMercy) && GnashingFang.IsCoolDown) return true;

            //�Ǳ�����
            if (!Player.HasStatus(true, StatusID.NoMercy) && !GnashingFang.WillHaveOneCharge(20)) return true;
        }

        //���γ岨
        if (CanUseBowShock(out act)) return true;

        //����
        if (JugularRip.ShouldUse(out act)) return true;
        if (AbdomenTear.ShouldUse(out act)) return true;
        if (EyeGouge.ShouldUse(out act)) return true;
        if (Hypervelocity.ShouldUse(out act)) return true;

        //Ѫ��
        if (GnashingFang.IsCoolDown && Bloodfest.ShouldUse(out act)) return true;

        //��㹥��,�ַ�ն
        if (RoughDivide.ShouldUse(out act, mustUse: true) && !IsMoving) return true;
        if (Player.HasStatus(true, StatusID.NoMercy) && RoughDivide.ShouldUse(out act, mustUse: true, emptyOrSkipCombo: true)) return true;

        act = null;
        return false;
    }

    private protected override bool DefenceAreaAbility(byte abilityRemain, out IAction act)
    {
        if (HeartofLight.ShouldUse(out act, emptyOrSkipCombo: true)) return true;
        if (Reprisal.ShouldUse(out act, mustUse: true)) return true;
        return false;
    }

    private protected override bool DefenceSingleAbility(byte abilityRemain, out IAction act)
    {
        if (abilityRemain == 2)
        {
            //����10%��
            if (HeartofStone.ShouldUse(out act)) return true;

            //���ƣ�����30%��
            if (Nebula.ShouldUse(out act)) return true;

            //���ڣ�����20%��
            if (Rampart.ShouldUse(out act)) return true;

            //αװ������10%��
            if (Camouflage.ShouldUse(out act)) return true;
        }
        //���͹���
        //ѩ��
        if (Reprisal.ShouldUse(out act)) return true;

        act = null;
        return false;
    }

    private protected override bool HealSingleAbility(byte abilityRemain, out IAction act)
    {
        if (Aurora.ShouldUse(out act, emptyOrSkipCombo: true) && abilityRemain == 1) return true;

        return false;
    }

    private bool CanUseNoMercy(out IAction act)
    {
        if (!NoMercy.ShouldUse(out act)) return false;

        if (!IsFullParty && !IsTargetBoss && !IsMoving && DemonSlice.ShouldUse(out _)) return true;

        //�ȼ����ڱ��������ж�
        if (!BurstStrike.EnoughLevel) return true;

        if (BurstStrike.EnoughLevel)
        {
            //4GCD�����ж�
            if (IsLastGCD((ActionID)KeenEdge.ID) && Ammo == 1 && !GnashingFang.IsCoolDown && !Bloodfest.IsCoolDown) return true;

            //3��������
            else if (Ammo == (Level >= 88 ? 3 : 2)) return true;

            //2��������
            else if (Ammo == 2 && GnashingFang.IsCoolDown) return true;
        }

        act = null;
        return false;
    }

    private bool CanUseGnashingFang(out IAction act)
    {
        //�����ж�
        if (GnashingFang.ShouldUse(out act))
        {
            //��4�˱�����ʹ��
            if (DemonSlice.ShouldUse(out _)) return false;

            //������3������
            if (Ammo == (Level >= 88 ? 3 : 2) && (Player.HasStatus(true, StatusID.NoMercy) || !NoMercy.WillHaveOneCharge(55))) return true;

            //����������
            if (Ammo > 0 && !NoMercy.WillHaveOneCharge(17) && NoMercy.WillHaveOneCharge(35)) return true;

            //3���ҽ�������ӵ������,��ǰ������ǰ������
            if (Ammo == 3 && IsLastGCD((ActionID)BrutalShell.ID) && NoMercy.WillHaveOneCharge(3)) return true;

            //1����Ѫ������ȴ����
            if (Ammo == 1 && !NoMercy.WillHaveOneCharge(55) && Bloodfest.WillHaveOneCharge(5)) return true;

            //4GCD���������ж�
            if (Ammo == 1 && !NoMercy.WillHaveOneCharge(55) && (!Bloodfest.IsCoolDown && Bloodfest.EnoughLevel || !Bloodfest.EnoughLevel)) return true;
        }
        return false;
    }

    /// <summary>
    /// ������
    /// </summary>
    /// <param name="act"></param>
    /// <returns></returns>
    private bool CanUseSonicBreak(out IAction act)
    {
        //�����ж�
        if (SonicBreak.ShouldUse(out act))
        {
            //��4�˱����в�ʹ��
            if (DemonSlice.ShouldUse(out _)) return false;

            //if (!IsFullParty && !SonicBreak.IsTargetBoss) return false;

            if (!GnashingFang.EnoughLevel && Player.HasStatus(true, StatusID.NoMercy)) return true;

            //����������ʹ��������
            if (GnashingFang.IsCoolDown && Player.HasStatus(true, StatusID.NoMercy)) return true;

            //�����ж�
            if (!DoubleDown.EnoughLevel && Player.HasStatus(true, StatusID.ReadyToRip)
                && GnashingFang.IsCoolDown) return true;

        }
        return false;
    }

    /// <summary>
    /// ����
    /// </summary>
    /// <param name="act"></param>
    /// <returns></returns>
    private bool CanUseDoubleDown(out IAction act)
    {
        //�����ж�
        if (DoubleDown.ShouldUse(out act, mustUse: true))
        {
            //��4�˱�����
            if (DemonSlice.ShouldUse(out _) && Player.HasStatus(true, StatusID.NoMercy)) return true;

            //�������ƺ�ʹ�ñ���
            if (SonicBreak.IsCoolDown && Player.HasStatus(true, StatusID.NoMercy)) return true;

            //2������������ж�,��ǰʹ�ñ���
            if (Player.HasStatus(true, StatusID.NoMercy) && !NoMercy.WillHaveOneCharge(55) && Bloodfest.WillHaveOneCharge(5)) return true;

        }
        return false;
    }

    /// <summary>
    /// ������
    /// </summary>
    /// <param name="act"></param>
    /// <returns></returns>
    private bool CanUseBurstStrike(out IAction act)
    {
        if (BurstStrike.ShouldUse(out act))
        {
            //��4�˱�������AOEʱ��ʹ��
            if (DemonSlice.ShouldUse(out _)) return false;

            //�������ʣ0.5����ȴ��,���ͷű�����,��Ҫ��Ϊ���ٲ�ͬ���ܻ�ʹ�����Ӻ�̫�������ж�һ��
            if (SonicBreak.IsCoolDown && SonicBreak.WillHaveOneCharge(0.5f) && GnashingFang.EnoughLevel) return false;

            //�����б������ж�
            if (Player.HasStatus(true, StatusID.NoMercy) &&
                AmmoComboStep == 0 &&
                !GnashingFang.WillHaveOneCharge(1)) return true;
            if (Level < 88 && Ammo == 2) return true;
            //�������ֹ���
            if (IsLastGCD((ActionID)BrutalShell.ID) &&
                (Ammo == (Level >= 88 ? 3 : 2) ||
                Bloodfest.WillHaveOneCharge(6) && Ammo <= 2 && !NoMercy.WillHaveOneCharge(10) && Bloodfest.EnoughLevel)) return true;

        }
        return false;
    }

    private bool CanUseBowShock(out IAction act)
    {
        if (BowShock.ShouldUse(out act, mustUse: true))
        {
            if (DemonSlice.ShouldUse(out _) && !IsFullParty) return true;

            if (!SonicBreak.EnoughLevel && Player.HasStatus(true, StatusID.NoMercy)) return true;

            //������,������������������ȴ��
            if (Player.HasStatus(true, StatusID.NoMercy) && SonicBreak.IsCoolDown) return true;

        }
        return false;
    }
}

