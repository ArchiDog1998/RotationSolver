using System.Collections.Generic;
using System.Linq;
using AutoAction.Actions;
using AutoAction.Combos.Basic;
using AutoAction.Combos.CustomCombo;
using AutoAction.Data;
using AutoAction.Helpers;
using AutoAction.Updaters;
using static AutoAction.Combos.Tank.PLDCombos.PLDCombo_Default;

namespace AutoAction.Combos.Tank.PLDCombos;

internal sealed class PLDCombo_Default : PLDCombo_Base<CommandType>
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

    protected override bool CanHealSingleSpell => TargetUpdater.PartyMembers.Count() == 1 && base.CanHealSingleSpell;

    public override SortedList<DescType, string> DescriptionDict => new()
    {
        {DescType.HealSingle, $"{Clemency}"},
        {DescType.DefenseArea, $"{DivineVeil}, {PassageofArms}"},
        {DescType.DefenseSingle, $"{Sentinel}, {Sheltron}"},
        {DescType.MoveAction, $"{Intervene}"},
    };

    private protected override bool GeneralGCD(out IAction act)
    {
        //��������
        if (BladeofValor.ShouldUse(out act, mustUse: true)) return true;
        if (BladeofFaith.ShouldUse(out act, mustUse: true)) return true;
        if (BladeofTruth.ShouldUse(out act, mustUse: true)) return true;

        //ħ����������,�а�����buff,��û��ս����
        if (Player.HasStatus(true, StatusID.Requiescat) && !Player.HasStatus(true, StatusID.FightOrFlight, StatusID.SwordOath))
        {
            if (Player.StatusStack(true, StatusID.Requiescat) == 1 && Player.WillStatusEnd(3, false, true, StatusID.Requiescat) || Player.CurrentMp <= 2000)
            {
                if (Confiteor.ShouldUse(out act, mustUse: true)) return true;
            }
            else
            {
                if (HolyCircle.ShouldUse(out act)) return true;
                if (HolySpirit.ShouldUse(out act)) return true;
            }
        }

        //AOE ����
        if (Prominence.ShouldUse(out act)) return true;
        if (TotalEclipse.ShouldUse(out act)) return true;

        //���｣
        if (Atonement.ShouldUse(out act))
        {
            if (Player.HasStatus(true, StatusID.FightOrFlight) && IsLastGCD(true, Atonement, RageofHalone) && !Player.WillStatusEndGCD(2, 0, true, StatusID.FightOrFlight)) return true;

            if (Player.StatusStack(true, StatusID.SwordOath) > 1) return true;
        }

        //��������
        if (GoringBlade.ShouldUse(out act)) return true;
        if (RageofHalone.ShouldUse(out act)) return true;
        if (RiotBlade.ShouldUse(out act)) return true;
        if (FastBlade.ShouldUse(out act)) return true;

        //Ͷ��
        if (ShieldLob.ShouldUse(out act)) return true;
        return false;
    }

    private protected override bool DefenceAreaAbility(byte abilityRemain, out IAction act)
    {
        //ʥ��Ļ��
        if (DivineVeil.ShouldUse(out act)) return true;

        //��װ����
        if (PassageofArms.ShouldUse(out act)) return true;

        //Ѫ��
        if (Reprisal.ShouldUse(out act, mustUse: true)) return true;
        return false;
    }

    private protected override bool AttackAbility(byte abilityRemain, out IAction act)
    {
        if (abilityRemain == 1 && SettingBreak)
        {
            //ս�ӷ�Ӧ ��Buff
            if (FightorFlight.ShouldUse(out act)) return true;

            //������
            if (Requiescat.ShouldUse(out act, mustUse: true) && Player.HasStatus(true, StatusID.FightOrFlight) && Player.WillStatusEnd(17, true, true, StatusID.FightOrFlight) && Target.HasStatus(true, StatusID.GoringBlade)) return true;
        }

        //������ת
        if ((TotalEclipse.ShouldUse(out _) || FightorFlight.ElapsedAfterGCD(2)) && CircleofScorn.ShouldUse(out act, mustUse: true)) return true;

        //���֮��
        if ((TotalEclipse.ShouldUse(out _) || FightorFlight.ElapsedAfterGCD(3)) && SpiritsWithin.ShouldUse(out act, mustUse: true)) return true;

        //��ͣ
        if (Target.HasStatus(true, StatusID.GoringBlade))
        {
            if (FightorFlight.ElapsedAfterGCD(2) && Intervene.ShouldUse(out act, mustUse: true)) return true;

            if (Intervene.ShouldUse(out act, mustUse: true) && !IsMoving) return true;
        }

        //����,����
        if (OathGauge == 100 && Player.CurrentHp < Player.MaxHp)
        {
            //����
            if (HaveShield && Sheltron.ShouldUse(out act)) return true;
            //����
            if (!HaveShield && Cover.ShouldUse(out act)) return true;
        }
        act = null;
        return false;
    }

    private protected override bool DefenceSingleAbility(byte abilityRemain, out IAction act)
    {
        //����
        if (HaveShield && Sheltron.ShouldUse(out act)) return true;
        //����
        if (!HaveShield && Cover.ShouldUse(out act)) return true;

        if (abilityRemain == 1)
        {
            //Ԥ��������30%��
            if (Sentinel.ShouldUse(out act)) return true;

            //���ڣ�����20%��
            if (Rampart.ShouldUse(out act)) return true;
        }

        //ѩ��
        if (Reprisal.ShouldUse(out act)) return true;

        //��Ԥ������10%��
        if (!HaveShield && Intervention.ShouldUse(out act)) return true;
        act = null;
        return false;
    }
}
