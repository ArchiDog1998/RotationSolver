using System;
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
    public override string GameVersion => "6.28";

    public override string Author => "逆光";

    internal enum CommandType : byte
    {
        None,
    }

    protected override SortedList<CommandType, string> CommandDescription => new SortedList<CommandType, string>()
    {
        //{CommandType.None, "" }, //写好注释啊！用来提示用户的。
    };

    public override SortedList<DescType, string> DescriptionDict => new()
    {
        {DescType.HealSingle, $"{Vercure}"},
        {DescType.DefenseArea, $"{MagickBarrier}"},
        {DescType.MoveAction, $"{CorpsAcorps}"},
    };

    static RDMCombo_Default()
    {
        Jolt.ComboCheck = b => !Player.HasStatus(true, StatusID.Swiftcast, StatusID.Dualcast ,StatusID.Acceleration);
        Verfire.ComboCheck = Jolt.ComboCheck;
        Verstone.ComboCheck = Jolt.ComboCheck;
        Veraero2.ComboCheck = Jolt.ComboCheck;
        Verthunder2.ComboCheck = Jolt.ComboCheck;
        Embolden.ComboCheck = Jolt.ComboCheck;

        Manafication.ComboCheck = b => ManaStacks == 0 || ManaStacks == 3 && !Player.HasStatus(true, StatusID.Swiftcast, StatusID.Dualcast, StatusID.Acceleration);
        Riposte.ComboCheck = b => !Player.HasStatus(true, StatusID.Dualcast);
        Moulinet.ComboCheck = Riposte.ComboCheck;

        Acceleration.ComboCheck = b => !Player.HasStatus(true, StatusID.Swiftcast, StatusID.Dualcast);
    }

    private protected override ActionConfiguration CreateConfiguration()
    {
        return base.CreateConfiguration()
            .SetBool("UseVercure", true, "使用赤治疗获得即刻");
    }

    private protected override bool AttackAbility(byte abilityRemain, out IAction act)
    {
        if (SettingBreak)
        {
            if (((Math.Max(BlackMana, WhiteMana) <= 50 && !Manafication.IsCoolDown) || (!Manafication.EnoughLevel && Math.Min(BlackMana, WhiteMana) >= 50)) && Embolden.ShouldUse(out act)) return true;

            if ((Player.HasStatus(true, StatusID.Embolden) || !Embolden.WillHaveOneCharge(100)) && Manafication.ShouldUse(out act)) return true;

        }

        //飞刺
        if (Fleche.ShouldUse(out act)) return true;

        //六分反击
        if (ContreSixte.ShouldUse(out act, mustUse: true)) return true;

        //交剑
        if (Engagement.ShouldUse(out act, emptyOrSkipCombo: true)) return true;

        //突进
        if (!IsMoving && CorpsAcorps.Target.DistanceToPlayer() <= 3 && CorpsAcorps.ShouldUse(out act, emptyOrSkipCombo: true)) return true;

        //促进
        if (ManaStacks == 0 && 
            ((!VerfireReady && !VerstoneReady) || !Acceleration.IsCoolDown) && 
            Acceleration.ShouldUse(out act, emptyOrSkipCombo: true)) return true;

        //即刻咏唱
        if (ManaStacks == 0 &&
            !VerfireReady && !VerstoneReady && Acceleration.CurrentCharges == 0 &&
            Math.Min(BlackMana, WhiteMana) <= 50 && 
            Swiftcast.ShouldUse(out act, mustUse: true)) return true;
        act = null;
        return false;
    }

    private protected override bool GeneralGCD(out IAction act)
    {
        //决断
        if (Resolution.ShouldUse(out act, mustUse: true)) return true;
        //焦热
        if (Scorch.ShouldUse(out act, mustUse: true)) return true;

        //赤神圣
        if (((BlackMana >= WhiteMana) ||
            !VerstoneReady && VerfireReady && !Player.WillStatusEnd(10, VerfireReady) && BlackMana - WhiteMana <= 18) &&
            Verholy.ShouldUse(out act, mustUse: true)) return true;
        //赤核爆
        if (((BlackMana <= WhiteMana) ||
            (!VerfireReady && VerstoneReady && !Player.WillStatusEnd(10, VerstoneReady) && BlackMana - WhiteMana <= 18) ||
            !Verholy.EnoughLevel) &&
            Verflare.ShouldUse(out act, mustUse: true)) return true;

        if (Verflare.EnoughLevel && IsLastGCD(true, Moulinet) && Moulinet.ShouldUse(out act, mustUse: true)) return true;
        if (((Math.Min(BlackMana, WhiteMana) + (ManaStacks * 20) >= 60) || !Verflare.EnoughLevel) && Moulinet.ShouldUse(out act)) return true;

        //魔三连
        if (Redoublement.ShouldUse(out act)) return true;
        if (Zwerchhau.ShouldUse(out act)) return true;
        if (((Math.Min(BlackMana, WhiteMana) >= 50) || (!Redoublement.EnoughLevel && Math.Min(BlackMana, WhiteMana) >= 35) || (!Zwerchhau.EnoughLevel && Math.Min(BlackMana, WhiteMana) >= 20)) &&
            IsTargetBoss && 
            Riposte.ShouldUse(out act)) return true;

        //散碎
        if (Player.HasStatus(true, StatusID.Dualcast, StatusID.Acceleration, StatusID.Swiftcast) && Scatter.ShouldUse(out act)) return true;
        //Aoe
        if (WhiteMana <= BlackMana && Veraero2.ShouldUse(out act)) return true;
        if ((BlackMana <= WhiteMana || !Veraero2.EnoughLevel) && Verthunder2.ShouldUse(out act)) return true;

        //赤飞石
        if ((WhiteMana <= BlackMana || BlackMana - WhiteMana <= 25) && Verstone.ShouldUse(out act)) return true;
        //赤火炎
        if ((BlackMana<= WhiteMana || WhiteMana - BlackMana <= 25) && Verfire.ShouldUse(out act)) return true;

        //单体
        //白
        if ((WhiteMana <= BlackMana || (VerfireReady && WhiteMana - BlackMana <= 24)) && Player.HasStatus(true, StatusID.Dualcast, StatusID.Acceleration, StatusID.Swiftcast) && Veraero.ShouldUse(out act)) return true;
        //黑
        if ((BlackMana <= WhiteMana || (VerstoneReady && BlackMana - WhiteMana <= 24) || !Veraero.EnoughLevel) && Player.HasStatus(true, StatusID.Dualcast, StatusID.Acceleration, StatusID.Swiftcast) && Verthunder.ShouldUse(out act)) return true;
        //震荡
        if (Jolt.ShouldUse(out act)) return true;

        //赤治疗加即刻，有连续咏唱或者即刻的话就不放了
        if (Config.GetBoolByName("UseVercure") && Vercure.ShouldUse(out act)) return true;
        return false;
    }

    private protected override IAction CountDownAction(float remainTime)
    {
        //5s预读赤暴雷
        if (remainTime <= 5 && Verthunder.ShouldUse(out _)) return Verthunder;
        return base.CountDownAction(remainTime);
    }

    private protected override bool DefenceAreaAbility(byte abilityRemain, out IAction act)
    {
        if (Addle.ShouldUse(out act)) return true;
        if (MagickBarrier.ShouldUse(out act, mustUse: true)) return true;
        return false;
    }
}