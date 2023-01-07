using System.Collections.Generic;
using AutoAction.Actions;
using AutoAction.Combos.Basic;
using AutoAction.Combos.CustomCombo;
using AutoAction.Configuration;
using AutoAction.Data;
using AutoAction.Helpers;
using static AutoAction.Combos.RangedMagicial.RDMCombos.RDMCombo_Default;

namespace AutoAction.Combos.RangedMagicial.RDMCombos;

internal sealed class RDMCombo_Default : RDMCombo_Base<CommandType>
{
    public override string GameVersion => "6.0";

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
        {DescType.HealSingle, $"{Vercure}"},
        {DescType.DefenseArea, $"{MagickBarrier}"},
        {DescType.MoveAction, $"{CorpsAcorps}"},
    };

    static RDMCombo_Default()
    {
        Acceleration.ComboCheck = b => InCombat;
    }

    private protected override ActionConfiguration CreateConfiguration()
    {
        return base.CreateConfiguration()
            .SetBool("UseVercure", true, "ʹ�ó����ƻ�ü���");
    }

    private protected override bool EmergencyAbility(byte abilityRemain, IAction nextGCD, out IAction act)
    {
        act = null;
        //����Ҫ�ŵ�ħ�ش̻���ħZն��ħ��Բն֮��
        if (nextGCD.IsAnySameAction(true, Zwerchhau, Redoublement, Moulinet))
        {
            if (Service.Configuration.AutoBreak && Embolden.ShouldUse(out act, mustUse: true)) return true;
        }
        //����������ʱ���ͷš�
        if (Service.Configuration.AutoBreak && GetRightValue(WhiteMana) && GetRightValue(BlackMana))
        {
            if (!canUseMagic(act) && Manafication.ShouldUse(out act)) return true;
            if (Embolden.ShouldUse(out act, mustUse: true)) return true;
        }
        //����Ҫ�ŵ�ħ������֮��
        if (ManaStacks == 3 || Level < 68 && !nextGCD.IsAnySameAction(true, Zwerchhau, Riposte))
        {
            if (!canUseMagic(act) && Manafication.ShouldUse(out act)) return true;
        }

        act = null;
        return false;
    }

    private bool GetRightValue(byte value)
    {
        return value >= 6 && value <= 12;
    }

    private protected override bool AttackAbility(byte abilityRemain, out IAction act)
    {
        act = null;
        if (SettingBreak)
        {
            if (!canUseMagic(act) && Manafication.ShouldUse(out act)) return true;
            if (Embolden.ShouldUse(out act, mustUse: true)) return true;
        }

        if (ManaStacks == 0 && (BlackMana < 50 || WhiteMana < 50) && !Manafication.WillHaveOneChargeGCD(1, 1))
        {
            //�ٽ�������Ԥ��buffû�����á� 
            if (abilityRemain == 2 && Acceleration.ShouldUse(out act, emptyOrSkipCombo: true)
                && (!Player.HasStatus(true, StatusID.VerfireReady) || !Player.HasStatus(true, StatusID.VerstoneReady))) return true;

            //����ӽ��
            if (!Player.HasStatus(true, StatusID.Acceleration)
                && Swiftcast.ShouldUse(out act, mustUse: true)
                && (!Player.HasStatus(true, StatusID.VerfireReady) || !Player.HasStatus(true, StatusID.VerstoneReady))) return true;
        }

        //�����ĸ���������
        if (ContreSixte.ShouldUse(out act, mustUse: true)) return true;
        if (Fleche.ShouldUse(out act)) return true;
        //Empty: BaseAction.HaveStatusSelfFromSelf(1239)
        if (Engagement.ShouldUse(out act, emptyOrSkipCombo: true)) return true;

        if (CorpsAcorps.ShouldUse(out act, mustUse: true) && !IsMoving) return true;

        return false;
    }

    private protected override bool GeneralGCD(out IAction act)
    {
        act = null;
        if (ManaStacks == 3) return false;

        #region �������
        if (!Verthunder2.ShouldUse(out _))
        {
            if (Verfire.ShouldUse(out act)) return true;
            if (Verstone.ShouldUse(out act)) return true;
        }

        //���Կ�ɢ��
        if (Scatter.ShouldUse(out act)) return true;
        //ƽ��ħԪ
        if (WhiteMana < BlackMana)
        {
            if (Veraero2.ShouldUse(out act)) return true;
            if (Veraero.ShouldUse(out act)) return true;
        }
        else
        {
            if (Verthunder2.ShouldUse(out act)) return true;
            if (Verthunder.ShouldUse(out act)) return true;
        }
        if (Jolt.ShouldUse(out act)) return true;
        #endregion

        //��ˢ���׺ͷ�ʯ


        //�����ƣ��Ӽ��̣�������ӽ�����߼��̵Ļ��Ͳ�����
        if (Config.GetBoolByName("UseVercure") && Vercure.ShouldUse(out act)
            ) return true;

        return false;
    }


    private protected override bool DefenceAreaAbility(byte abilityRemain, out IAction act)
    {
        //����
        if (Addle.ShouldUse(out act)) return true;
        if (MagickBarrier.ShouldUse(out act, mustUse: true)) return true;
        return false;
    }

    private protected override bool EmergencyGCD(out IAction act)
    {
        byte level = Level;
        #region Զ������
        //���ħԪ�ᾧ���ˡ�
        if (ManaStacks == 3)
        {
            if (BlackMana > WhiteMana && level >= 70)
            {
                if (Verholy.ShouldUse(out act, mustUse: true)) return true;
            }
            if (Verflare.ShouldUse(out act, mustUse: true)) return true;
        }

        //����
        if (Scorch.ShouldUse(out act, mustUse: true)) return true;

        //����
        if (Resolution.ShouldUse(out act, mustUse: true)) return true;
        #endregion

        #region ��ս����


        if (IsLastGCD(true, Moulinet) && Moulinet.ShouldUse(out act, mustUse: true)) return true;
        if (Zwerchhau.ShouldUse(out act)) return true;
        if (Redoublement.ShouldUse(out act)) return true;

        //����������ˣ�����ħԪ���ˣ��������ڱ��������ߴ��ڿ�������״̬���������ã�
        bool mustStart = Player.HasStatus(true, StatusID.Manafication) ||
                         BlackMana == 100 || WhiteMana == 100 || !Embolden.IsCoolDown;

        //��ħ��Ԫû�����������£�Ҫ���С��ħԪ����������Ҳ����ǿ��Ҫ�������жϡ�
        if (!mustStart)
        {
            if (BlackMana == WhiteMana) return false;

            //Ҫ���С��ħԪ����������Ҳ����ǿ��Ҫ�������жϡ�
            if (WhiteMana < BlackMana)
            {
                if (Player.HasStatus(true, StatusID.VerstoneReady))
                {
                    return false;
                }
            }
            if (WhiteMana > BlackMana)
            {
                if (Player.HasStatus(true, StatusID.VerfireReady))
                {
                    return false;
                }
            }

            //������û�м�����صļ��ܡ�
            if (Player.HasStatus(true, Vercure.BuffsProvide))
            {
                return false;
            }

            //���������ʱ��쵽�ˣ�������û�á�
            if (Embolden.WillHaveOneChargeGCD(10))
            {
                return false;
            }
        }
        #endregion

        if (Player.HasStatus(true, StatusID.Dualcast)) return false;

        #region ��������
        //Ҫ������ʹ�ý�ս�����ˡ�
        if (Moulinet.ShouldUse(out act))
        {
            if (BlackMana >= 60 && WhiteMana >= 60) return true;
        }
        else
        {
            if (BlackMana >= 50 && WhiteMana >= 50 && Riposte.ShouldUse(out act)) return true;
        }
        if (ManaStacks > 0 && Riposte.ShouldUse(out act)) return true;
        #endregion

        return false;
    }

    //�ж����Ⱦ����ܲ���ʹ��
    private bool canUseMagic(IAction act)
    {
        //return IsLastAction(true, Scorch) || IsLastAction(true, Resolution) || IsLastAction(true, Verholy) || IsLastAction(true, Verflare);
        return Scorch.ShouldUse(out act) || Resolution.ShouldUse(out act);
    }
}

