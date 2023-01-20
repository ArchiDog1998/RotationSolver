using RotationSolver.Actions;
using RotationSolver.Configuration.RotationConfig;
using RotationSolver.Data;
using RotationSolver.Helpers;
using RotationSolver.Rotations.Basic;
using RotationSolver.Rotations.CustomRotation;
using System.Collections.Generic;

namespace RotationSolver.Rotations.Melee.SAM;

internal sealed class SAM_Default : SAM_Base
{
    public override string GameVersion => "6.28";

    public override string RotationName => "Default";

    private protected override IRotationConfigSet CreateConfiguration()
    {
        return base.CreateConfiguration()
            .SetFloat("addKenki", 50, "当剑气大于该值时将必杀技·震天/九天加入循环", min: 0, max: 85, speed: 5);
    }

    /// <summary>
    /// 明镜止水
    /// </summary>
    private static bool haveMeikyoShisui => Player.HasStatus(true, StatusID.MeikyoShisui);

    public SAM_Default()
    {
        //明镜里ban了最基础技能
        Hakaze.RotationCheck = b => !haveMeikyoShisui;
        Fuko.RotationCheck = b => !haveMeikyoShisui;
        Fuga.RotationCheck = b => !haveMeikyoShisui;
        Enpi.RotationCheck = b => !haveMeikyoShisui;
        //保证有buff加成
        Higanbana.RotationCheck = b => HaveMoon && HaveFlower;
        OgiNamikiri.RotationCheck = b => HaveMoon && HaveFlower;
        HissatsuSenei.RotationCheck = b => HaveMoon && HaveFlower;
        HissatsuGuren.RotationCheck = b => HaveMoon && HaveFlower;
    }

    public override SortedList<DescType, string> DescriptionDict => new()
    {
        {DescType.DefenseSingle, $"能力: {ThirdEye}"},
        {DescType.MoveAction, $"能力: {HissatsuGyoten}, "},
    };

    private protected override bool GeneralGCD(out IAction act)
    {
        //奥义回返
        if (KaeshiNamikiri.ShouldUse(out act, mustUse: true)) return true;

        //燕回返
        if (KaeshiGoken.ShouldUse(out act, emptyOrSkipCombo: true, mustUse: true)) return true;
        if (KaeshiSetsugekka.ShouldUse(out act, emptyOrSkipCombo: true, mustUse: true)) return true;

        //奥义斩浪
        if ((!IsTargetBoss || Target.HasStatus(true, StatusID.Higanbana)) && OgiNamikiri.ShouldUse(out act, mustUse: true)) return true;

        //处理居合术
        if (SenCount == 1 && IsTargetBoss && !IsTargetDying)
        {
            if (Higanbana.ShouldUse(out act)) return true;
        }
        if (SenCount == 2)
        {
            if (TenkaGoken.ShouldUse(out act, mustUse: !MidareSetsugekka.EnoughLevel)) return true;
        }
        if (SenCount == 3)
        {
            if (MidareSetsugekka.ShouldUse(out act)) return true;
        }

        //连击2
        if ((!HaveMoon || MoonTime < FlowerTime || !Oka.EnoughLevel) && Mangetsu.ShouldUse(out act, emptyOrSkipCombo: haveMeikyoShisui && !HasGetsu)) return true;
        if ((!HaveFlower || FlowerTime < MoonTime) && Oka.ShouldUse(out act, emptyOrSkipCombo: haveMeikyoShisui && !HasKa)) return true;
        if (!HasSetsu && Yukikaze.ShouldUse(out act, emptyOrSkipCombo: haveMeikyoShisui && HasGetsu && HasKa && !HasSetsu)) return true;

        //连击3
        if (Gekko.ShouldUse(out act, emptyOrSkipCombo: haveMeikyoShisui && !HasGetsu)) return true;
        if (Kasha.ShouldUse(out act, emptyOrSkipCombo: haveMeikyoShisui && !HasKa)) return true;

        //连击2
        if ((!HaveMoon || MoonTime < FlowerTime || !Shifu.EnoughLevel) && Jinpu.ShouldUse(out act)) return true;
        if ((!HaveFlower || FlowerTime < MoonTime) && Shifu.ShouldUse(out act)) return true;

        //连击1
        if (Fuko.ShouldUse(out act)) return true;
        if (!Fuko.EnoughLevel && Fuga.ShouldUse(out act)) return true;
        if (Hakaze.ShouldUse(out act)) return true;

        //燕飞
        if (Enpi.ShouldUse(out act)) return true;
        act = null;
        return false;
    }

    private protected override bool AttackAbility(byte abilityRemain, out IAction act)
    {
        //意气冲天
        if (Kenki <= 50 && Ikishoten.ShouldUse(out act)) return true;

        //叶隐
        if (Target.HasStatus(true, StatusID.Higanbana) && Target.WillStatusEnd(32, true, StatusID.Higanbana) && !Target.WillStatusEnd(28, true, StatusID.Higanbana) && SenCount == 1 && IsLastAction(true, Yukikaze) && !haveMeikyoShisui)
        {
            if (Hagakure.ShouldUse(out act)) return true;
        }

        //闪影、红莲
        if (HissatsuGuren.ShouldUse(out act, mustUse: !HissatsuSenei.EnoughLevel)) return true;
        if (HissatsuSenei.ShouldUse(out act)) return true;

        //照破、无明照破
        if (Shoha2.ShouldUse(out act)) return true;
        if (Shoha.ShouldUse(out act)) return true;

        //震天、九天
        if (Kenki >= 50 && Ikishoten.WillHaveOneCharge(10) || Kenki >= Configs.GetFloat("addKenki") || IsTargetBoss && IsTargetDying)
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
        if (HaveHostilesInRange && IsLastGCD(true, Yukikaze, Mangetsu, Oka) &&
            (!IsTargetBoss || Target.HasStatus(true, StatusID.Higanbana) && !Target.WillStatusEnd(40, true, StatusID.Higanbana) || !HaveMoon && !HaveFlower || IsTargetBoss && IsTargetDying))
        {
            if (MeikyoShisui.ShouldUse(out act, emptyOrSkipCombo: true)) return true;
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
        if (ThirdEye.ShouldUse(out act)) return true;
        if (Feint.ShouldUse(out act)) return true;
        return false;
    }

    private protected override IAction CountDownAction(float remainTime)
    {
        //开局使用明镜
        if (remainTime <= 5 && MeikyoShisui.ShouldUse(out _)) return MeikyoShisui;
        //真北防止boss面向没到位
        if (remainTime <= 2 && TrueNorth.ShouldUse(out _)) return TrueNorth;
        return base.CountDownAction(remainTime);
    }
}