using System.Collections.Generic;
using XIVAutoAttack.Actions;
using XIVAutoAttack.Combos.Basic;
using XIVAutoAttack.Combos.CustomCombo;
using XIVAutoAttack.Data;
using XIVAutoAttack.Helpers;
using static XIVAutoAttack.Combos.Melee.SAMCombos.SAMCombo_Default;

namespace XIVAutoAttack.Combos.Melee.SAMCombos;

internal sealed class SAMCombo_Default : SAMCombo_Base<CommandType>
{
    public override string Author => "玖祁";

    internal enum CommandType : byte
    {
        None,
    }

    protected override SortedList<CommandType, string> CommandDescription => new SortedList<CommandType, string>()
    {
        //{CommandType.None, "" }, //写好注释啊！用来提示用户的。
    };

    //命名明镜止水状态，方便下面调用。
    private static bool HaveMeikyoShisui => Player.HasStatus(true, StatusID.MeikyoShisui);

    private static bool HaveEnhancedEnpi => Player.HasStatus(true, StatusID.Enhanced_Enpi);
    public override SortedList<DescType, string> DescriptionDict => new()
    {
        {DescType.单体防御, $"{ThirdEye}"},
        {DescType.移动技能, $"{HissatsuGyoten}"},
    };
    private protected override bool EmergencyGCD(out IAction act)
    {
        if (Service.IconReplacer.OriginalHook(OgiNamikiri.ID) == KaeshiNamikiri.ID)
        {
            if (KaeshiNamikiri.ShouldUse(out act, mustUse: true, emptyOrSkipCombo: true)) return true;
        }
        if (Service.IconReplacer.OriginalHook(Tsubame_gaeshi.ID) == KaeshiGoken.ID)
        {
            if (KaeshiGoken.ShouldUse(out act, mustUse: true, emptyOrSkipCombo: true)) return true;
        }
        if (Service.IconReplacer.OriginalHook(Tsubame_gaeshi.ID) == KaeshiSetsugekka.ID)
        {
            if (KaeshiSetsugekka.ShouldUse(out act, mustUse: true, emptyOrSkipCombo: true)) return true;
        }

        //有明镜时打闪
        if (HaveMeikyoShisui)
        {
            if (!HasGetsu)
            {
                if (Mangetsu.ShouldUse(out act, emptyOrSkipCombo: true)) return true;
                if (Gekko.ShouldUse(out act, emptyOrSkipCombo: true)) return true;
            }
            if (!HasKa)
            {
                if (Oka.ShouldUse(out act, emptyOrSkipCombo: true)) return true;
                if (Kasha.ShouldUse(out act, emptyOrSkipCombo: true)) return true;
            }
            if (!HasSetsu && (!Fuga.ShouldUse(out _) || !Fuko.ShouldUse(out _)))
            {
                if (Yukikaze.ShouldUse(out act, emptyOrSkipCombo: true)) return true;
            }
        }
        //燕飞提高快结束时使用燕飞
        if (HaveEnhancedEnpi && (Hakaze.ShouldUse(out _) || Player.WillStatusEnd(3, true, StatusID.Enhanced_Enpi)))
        {
            if (Enpi.ShouldUse(out act)) return true;
        }
        act = null;
        return false;
    }
    private protected override bool GeneralGCD(out IAction act)
    {

        if (!HaveMeikyoShisui && OgiNamikiri.ShouldUse(out act, mustUse: true)) return true;
        #region 处理居合术
        switch (SenCount)
        {
            case 1:
                if (Higanbana.ShouldUse(out act) && HasSetsu) return true;
                break;
            case 2:
                if (TenkaGoken.ShouldUse(out act)) return true;
                break;
            case 3:
                if (MidareSetsugekka.ShouldUse(out act)) return true;
                break;
            default:
                break;
        }
        #endregion
        #region 处理进闪

        #region 处理雪闪
        if (!HasSetsu && (!Fuga.ShouldUse(out _) || !Fuko.ShouldUse(out _)))
        {
            if (Yukikaze.ShouldUse(out act)) return true;
            act = null;
        }
        #endregion
        #region 获取月闪
        if (!HasGetsu)
        {
            if (Mangetsu.ShouldUse(out act)) return true;
            if (Gekko.ShouldUse(out act)) return true;
            if (Jinpu.ShouldUse(out act)) return true;
            act = null;
        }
        #endregion
        #region 获取花闪
        if (!HasKa)
        {
            if (Oka.ShouldUse(out act)) return true;
            if (Kasha.ShouldUse(out act)) return true;
            if (Shifu.ShouldUse(out act)) return true;
            act = null;
        }
        #endregion

        #endregion
        if (Fuko.ShouldUse(out act)) return true;
        if (Fuga.ShouldUse(out act)) return true;
        if (Hakaze.ShouldUse(out act)) return true;
        if (CommandController.Move && MoveAbility(1, out act)) return true;
        if (Enpi.ShouldUse(out act)) return true;
        act = null;
        return false;
    }
    private protected override bool MoveAbility(byte abilityRemain, out IAction act)
    {
        if (Kenki >= 30 && HissatsuGyoten.ShouldUse(out act)) return true;
        act = null;
        return false;
    }
    private protected override bool AttackAbility(byte abilityRemain, out IAction act)
    {
        #region 处理剑压
        if (MeditationStacks == 3)
        {
            if (Shoha2.ShouldUse(out act)) return true;
            if (Shoha.ShouldUse(out act)) return true;
        }
        #endregion
        #region 处理剑气
        if (Kenki >= 25)
        {
            if (HissatsuGuren.ShouldUse(out act)) return true;
            if (HissatsuKyuten.ShouldUse(out act)) return true;
            if (HissatsuSenei.ShouldUse(out act)) return true;
            if (HissatsuShinten.ShouldUse(out act)) return true;
        }
        #endregion
        if (InCombat && Ikishoten.ShouldUse(out act)) return true;
        act = null;
        return false;
    }
    private protected override bool EmergencyAbility(byte abilityRemain, IAction nextGCD, out IAction act)
    {
        //明镜在非连击途中
        if (HaveHostilesInRange && !IsLastWeaponSkill(true, Hakaze) && !IsLastWeaponSkill(true, Shifu) && !IsLastWeaponSkill(true, Jinpu) &&
            !nextGCD.IsAnySameAction(false, Higanbana, OgiNamikiri, KaeshiNamikiri) && SenCount != 3 &&
            MeikyoShisui.ShouldUse(out act, emptyOrSkipCombo: true)) return true;

        //叶隐吞可能存在的闪溢出 
        if (Level < 84 && SenCount == 1)
        {
            if (!Tsubame_gaeshi.WillHaveOneChargeGCD(2) && Tsubame_gaeshi.WillHaveOneChargeGCD(3) && !Fuga.ShouldUse(out act))
            {
                if (Hagakure.ShouldUse(out act)) return true;
            }
        }

        return base.EmergencyAbility(abilityRemain, nextGCD, out act);
    }
    private protected override bool DefenceSingleAbility(byte abilityRemain, out IAction act)
    {
        if (ThirdEye.ShouldUse(out act)) return true;
        return false;
    }
    private protected override bool DefenceAreaAbility(byte abilityRemain, out IAction act)
    {
        //牵制
        if (Feint.ShouldUse(out act)) return true;
        return false;
    }



    //开局10s使用明镜
    private protected override IAction CountDownAction(float remainTime)
    {
        //明镜喵
        if (remainTime <= 10 && MeikyoShisui.ShouldUse(out _)) return MeikyoShisui;
        return base.CountDownAction(remainTime);
    }
}