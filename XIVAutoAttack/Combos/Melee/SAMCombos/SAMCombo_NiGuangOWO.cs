using System.Collections.Generic;
using XIVAutoAttack.Actions;
using XIVAutoAttack.Combos.Basic;
using XIVAutoAttack.Combos.CustomCombo;
using XIVAutoAttack.Data;
using XIVAutoAttack.Helpers;
using static XIVAutoAttack.Combos.Melee.SAMCombos.SAMCombo_Default;

namespace XIVAutoAttack.Combos.Melee.SAMCombos;

internal sealed class SAMCombo_NiGuangOWO : SAMCombo_Base<CommandType>
{
    public override string Author => "逆光";


    /// <summary>
    /// 明镜止水
    /// </summary>
    private static bool haveMeikyoShisui => Player.HasStatus(true, StatusID.MeikyoShisui);

    public SAMCombo_NiGuangOWO()
    {
        //明镜里ban了最基础技能
        Hakaze.ComboCheck = b => !haveMeikyoShisui;
        Fuga.ComboCheck = b => !haveMeikyoShisui;
        Enpi.ComboCheck = b => !haveMeikyoShisui;
        //奥义斩浪保证有双buff
        OgiNamikiri.ComboCheck = b => HaveMoon && HaveFlower && Target.HasStatus(true,StatusID.Higanbana);
        //明镜
        MeikyoShisui.ComboCheck = b => SenCount != 3;
    }

    public override SortedList<DescType, string> DescriptionDict => new()
    {
        {DescType.单体防御, $"{ThirdEye}"},
    };

    private protected override bool GeneralGCD(out IAction act)
    {
        //奥义回返
        if (Service.IconReplacer.OriginalHook(OgiNamikiri.ID) == KaeshiNamikiri.ID)
        {
            if (KaeshiNamikiri.ShouldUse(out act, mustUse: true)) return true;
        }

        //燕回返
        if (Service.IconReplacer.OriginalHook(16483) == KaeshiGoken.ID)
        {
            if (KaeshiGoken.ShouldUse(out act, mustUse: true)) return true;
        }
        if (Service.IconReplacer.OriginalHook(16483) == KaeshiSetsugekka.ID)
        {
            if (KaeshiSetsugekka.ShouldUse(out act, emptyOrSkipCombo:true, mustUse: true)) return true;
        }

        //奥义斩浪
        if (OgiNamikiri.ShouldUse(out act, mustUse: true)) return true;

        //处理居合术
        if(SenCount == 1)
        {
            if (HaveMoon && HaveFlower && Higanbana.ShouldUse(out act)) return true;
        }
        if(SenCount == 2)
        {
            if(TenkaGoken.ShouldUse(out act,mustUse:!MidareSetsugekka.EnoughLevel)) return true;
        }
        if(SenCount == 3)
        {
            if (MidareSetsugekka.ShouldUse(out act)) return true;
        }

        //雪
        if (!HasSetsu && !Fuga.ShouldUse(out _))
        {
            if (Yukikaze.ShouldUse(out act, emptyOrSkipCombo: haveMeikyoShisui && HasGetsu && HasKa)) return true;            
        }

        //月
        if (!HasGetsu)
        {
            if (GetsuGCD(out act, haveMeikyoShisui)) return true;
        }

        //花
        if (!HasKa)
        {
            if (KaGCD(out act, haveMeikyoShisui)) return true;
        }

        if (Fuga.ShouldUse(out act)) return true;
        if (Hakaze.ShouldUse(out act)) return true;
        if (Enpi.ShouldUse(out act)) return true;
        act = null;
        return false;
    }

    //月连击
    private bool GetsuGCD(out IAction act, bool haveMeikyoShisui)
    {
        if (Mangetsu.ShouldUse(out act, emptyOrSkipCombo: haveMeikyoShisui)) return true;
        if (Gekko.ShouldUse(out act, emptyOrSkipCombo: haveMeikyoShisui)) return true;
        if (Jinpu.ShouldUse(out act)) return true;

        act = null;
        return false;
    }

    //花连击
    private bool KaGCD(out IAction act, bool haveMeikyoShisui)
    {
        if (Oka.ShouldUse(out act, emptyOrSkipCombo: haveMeikyoShisui)) return true;
        if (Kasha.ShouldUse(out act, emptyOrSkipCombo: haveMeikyoShisui)) return true;
        if (Shifu.ShouldUse(out act)) return true;

        act = null;
        return false;
    }

    private protected override bool AttackAbility(byte abilityRemain, out IAction act)
    {
        //意气冲天
        if (Kenki <= 50 && Ikishoten.ShouldUse(out act)) return true;

        //叶隐
        if(Target.WillStatusEnd(8,true,StatusID.Higanbana) && SenCount == 2 && !haveMeikyoShisui && HaveMoon && HaveFlower)
        {
            if (Hagakure.ShouldUse(out act)) return true;
        }

        //闪影、红莲
        if (HissatsuGuren.ShouldUse(out act)) return true;
        if (HissatsuSenei.ShouldUse(out act)) return true;

        //照破、无明照破
        if (Shoha2.ShouldUse(out act)) return true;
        if (Shoha.ShouldUse(out act)) return true;

        //震天、红莲
        if((Kenki > 50 && Ikishoten.WillHaveOneCharge(10)) || Kenki >= 85)
        {
            if (HissatsuKyuten.ShouldUse(out act)) return true;
            if (HissatsuShinten.ShouldUse(out act)) return true;
        }

        act = null;
        return false;
    }
    private protected override bool EmergencyAbility(byte abilityRemain, IAction nextGCD, out IAction act)
    {
        //明镜止水
        if(HaveHostilesInRange && ((!IsLastWeaponSkill(true, Shifu) && !IsLastWeaponSkill(true, Jinpu) && !IsLastWeaponSkill(true, Hakaze)) || IsLastWeaponSkill(true,Yukikaze)) && HasSetsu)
        {
            if(MeikyoShisui.ShouldUse(out act, emptyOrSkipCombo: true)) return true;
        }
        return base.EmergencyAbility(abilityRemain, nextGCD, out act);
    }
    private protected override bool DefenceSingleAbility(byte abilityRemain, out IAction act)
    {
        //心眼
        if (ThirdEye.ShouldUse(out act)) return true;
        return false;
    }
    private protected override bool DefenceAreaAbility(byte abilityRemain, out IAction act)
    {
        //心眼
        if (ThirdEye.ShouldUse(out act)) return true;
        //牵制
        if (Feint.ShouldUse(out act)) return true;
        return false;
    }

    private protected override IAction CountDownAction(float remainTime)
    {
        //开局使用明镜
        if (remainTime <= 5 && MeikyoShisui.ShouldUse(out _)) return MeikyoShisui;
        return base.CountDownAction(remainTime);
    }
}