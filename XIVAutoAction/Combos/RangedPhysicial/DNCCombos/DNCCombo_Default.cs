using System.Collections.Generic;
using AutoAction.Actions;
using AutoAction.Combos.Basic;
using AutoAction.Combos.CustomCombo;
using AutoAction.Data;
using AutoAction.Helpers;
using AutoAction.Updaters;
using static AutoAction.Combos.RangedPhysicial.DNCCombos.DNCCombo_Default;

namespace AutoAction.Combos.RangedPhysicial.DNCCombos;

internal sealed class DNCCombo_Default : DNCCombo_Base<CommandType>
{
    public override string GameVersion => "6.18";
    public override string Author => "��";

    internal enum CommandType : byte
    {
        None,
    }

    protected override SortedList<CommandType, string> CommandDescription => new SortedList<CommandType, string>()
    {
        //{CommandType.None, "" }, //д��ע�Ͱ���������ʾ�û��ġ�
    };

    public override SortedList<DescType, string> DescriptionDict => new()
    {
        {DescType.DefenseArea, $"{ShieldSamba}"},
        {DescType.HealArea, $"{CuringWaltz}, {Improvisation}"},
        {DescType.MoveAction, $"{EnAvant}"},
    };

    private protected override bool AttackAbility(byte abilityRemain, out IAction act)
    {
        if (SettingBreak && !TechnicalStep.EnoughLevel
    && Devilment.ShouldUse(out act, emptyOrSkipCombo: true)) return true;

        //Ӧ�������
        if (Player.HasStatus(true, StatusID.ClosedPosition1))
        {
            foreach (var friend in TargetUpdater.PartyMembers)
            {
                if (friend.HasStatus(true, StatusID.ClosedPosition2))
                {
                    if (ClosedPosition.ShouldUse(out act) && ClosedPosition.Target != friend)
                    {
                        return true;
                    }
                    break;
                }
            }
        }
        else if (ClosedPosition.ShouldUse(out act)) return true;

        //���Ա���
        if (Player.HasStatus(true, StatusID.TechnicalFinish)
        && Devilment.ShouldUse(out act, emptyOrSkipCombo: true)) return true;

        //�ٻ�
        if (Flourish.ShouldUse(out act, emptyOrSkipCombo: true)) return true;

        //���衤��
        if (FanDance4.ShouldUse(out act, mustUse: true)) return true;
        if (FanDance3.ShouldUse(out act, mustUse: true)) return true;

        //����
        if (Player.HasStatus(true, StatusID.Devilment) || Feathers > 3 || !TechnicalStep.EnoughLevel)
        {
            if (FanDance2.ShouldUse(out act)) return true;
            if (FanDance.ShouldUse(out act)) return true;
        }

        return false;
    }

    private protected override bool MoveAbility(byte abilityRemain, out IAction act)
    {
        if (EnAvant.ShouldUse(out act, emptyOrSkipCombo: true)) return true;
        return false;
    }

    private protected override bool HealAreaAbility(byte abilityRemain, out IAction act)
    {
        if (CuringWaltz.ShouldUse(out act, emptyOrSkipCombo: true)) return true;
        if (Improvisation.ShouldUse(out act, emptyOrSkipCombo: true)) return true;
        return false;
    }

    private protected override bool DefenceAreaAbility(byte abilityRemain, out IAction act)
    {
        if (ShieldSamba.ShouldUse(out act, emptyOrSkipCombo: true)) return true;
        return false;
    }

    private protected override bool GeneralGCD(out IAction act)
    {
        if (!InCombat && !Player.HasStatus(true, StatusID.ClosedPosition1)
            && ClosedPosition.ShouldUse(out act)) return true;

        if (FinishStepGCD(out act)) return true;
        if (ExcutionStepGCD(out act)) return true;

        if (SettingBreak)
        {
            if (TechnicalStep.ShouldUse(out act, mustUse: true)) return true;
        }

        if (AttackGCD(out act, Player.HasStatus(true, StatusID.Devilment))) return true;

        return false;
    }

    private bool AttackGCD(out IAction act, bool breaking)
    {

        //����
        if ((breaking || Esprit >= 80) &&
            SaberDance.ShouldUse(out act, mustUse: true)) return true;

        //������
        if (Tillana.ShouldUse(out act, mustUse: true)) return true;
        if (StarfallDance.ShouldUse(out act, mustUse: true)) return true;

        if (IsDancing) return false;

        bool canstandard = !TechnicalStep.WillHaveOneChargeGCD(2);

        if (!Player.HasStatus(true, StatusID.TechnicalFinish))
        {
            //��׼�貽
            if (StandardStep.ShouldUse(out act, mustUse: true)) return true;
        }

        //�õ�Buff
        if (Bloodshower.ShouldUse(out act)) return true;
        if (Fountainfall.ShouldUse(out act)) return true;

        if (RisingWindmill.ShouldUse(out act)) return true;
        if (ReverseCascade.ShouldUse(out act)) return true;


        //��׼�貽
        if (canstandard && StandardStep.ShouldUse(out act, mustUse: true)) return true;


        //aoe
        if (Bladeshower.ShouldUse(out act)) return true;
        if (Windmill.ShouldUse(out act)) return true;

        //single
        if (Fountain.ShouldUse(out act)) return true;
        if (Cascade.ShouldUse(out act)) return true;

        return false;
    }
}
