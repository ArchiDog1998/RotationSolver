//namespace DefaultRotations.Melee;

//[SourceCode(Path = "main/DefaultRotations/Melee/SAM_Default.cs")]
//public sealed class SAM_Default : SAM_Base
//{
//    public override CombatType Type => CombatType.PvE;

//    public override string GameVersion => "6.28";

//    public override string RotationName => "Default";

//    protected override IRotationConfigSet CreateConfiguration()
//    {
//        return base.CreateConfiguration()
//            .SetInt(CombatType.PvE, "addKenki", 50, "Use Kenki above.", min: 0, max: 85, speed: 5);
//    }

//    /// <summary>
//    /// 明镜止水
//    /// </summary>
//    private static bool HaveMeikyoShisui => Player.HasStatus(true, StatusID.MeikyoShisui);

//    protected override bool GeneralGCD(out IAction act)
//    {
//        //奥义回返
//        if (KaeshiNamikiri.CanUse(out act, CanUseOption.MustUse)) return true;

//        var IsTargetBoss = HostileTarget?.IsBossFromTTK() ?? false;
//        var IsTargetDying = HostileTarget?.IsDying() ?? false;

//        //燕回返
//        if (KaeshiGoken.CanUse(out act, CanUseOption.MustUse | CanUseOption.EmptyOrSkipCombo)) return true;
//        if (KaeshiSetsugekka.CanUse(out act, CanUseOption.MustUse | CanUseOption.EmptyOrSkipCombo)) return true;

//        //奥义斩浪
//        if ((!IsTargetBoss || (HostileTarget?.HasStatus(true, StatusID.Higanbana) ?? false)) && HasMoon && HasFlower
//            && OgiNamikiri.CanUse(out act, CanUseOption.MustUse)) return true;

//        //处理居合术
//        if (SenCount == 1 && IsTargetBoss && !IsTargetDying)
//        {
//            if (HasMoon && HasFlower && Higanbana.CanUse(out act)) return true;
//        }
//        if (SenCount == 2)
//        {
//            if (TenkaGoken.CanUse(out act, !MidareSetsugekka.EnoughLevel ? CanUseOption.MustUse : CanUseOption.None)) return true;
//        }
//        if (SenCount == 3)
//        {
//            if (MidareSetsugekka.CanUse(out act)) return true;
//        }

//        //连击2
//        if ((!HasMoon || IsMoonTimeLessThanFlower || !Oka.EnoughLevel) && Mangetsu.CanUse(out act, HaveMeikyoShisui && !HasGetsu ? CanUseOption.EmptyOrSkipCombo : CanUseOption.None)) return true;
//        if ((!HasFlower || !IsMoonTimeLessThanFlower) && Oka.CanUse(out act, HaveMeikyoShisui && !HasKa ? CanUseOption.EmptyOrSkipCombo : CanUseOption.None)) return true;
//        if (!HasSetsu && Yukikaze.CanUse(out act, HaveMeikyoShisui && HasGetsu && HasKa && !HasSetsu ? CanUseOption.EmptyOrSkipCombo : CanUseOption.None)) return true;

//        //连击3
//        if (Gekko.CanUse(out act, HaveMeikyoShisui && !HasGetsu ? CanUseOption.EmptyOrSkipCombo : CanUseOption.None)) return true;
//        if (Kasha.CanUse(out act, HaveMeikyoShisui && !HasKa ? CanUseOption.EmptyOrSkipCombo : CanUseOption.None)) return true;

//        //连击2
//        if ((!HasMoon || IsMoonTimeLessThanFlower || !Shifu.EnoughLevel) && Jinpu.CanUse(out act)) return true;
//        if ((!HasFlower || !IsMoonTimeLessThanFlower) && Shifu.CanUse(out act)) return true;

//        if (!HaveMeikyoShisui)
//        {
//            //连击1
//            if (Fuko.CanUse(out act)) return true;
//            if (!Fuko.EnoughLevel && Fuga.CanUse(out act)) return true;
//            if (Hakaze.CanUse(out act)) return true;

//            //燕飞
//            if (Enpi.CanUse(out act)) return true;
//        }

//        return base.GeneralGCD(out act);
//    }

//    protected override bool AttackAbility(out IAction act)
//    {
//        var IsTargetBoss = HostileTarget?.IsBossFromTTK() ?? false;
//        var IsTargetDying = HostileTarget?.IsDying() ?? false;

//        //意气冲天
//        if (Kenki <= 50 && Ikishoten.CanUse(out act)) return true;

//        //叶隐
//        if ((HostileTarget?.HasStatus(true, StatusID.Higanbana) ?? false) && (HostileTarget?.WillStatusEnd(32, true, StatusID.Higanbana) ?? false) && !(HostileTarget?.WillStatusEnd(28, true, StatusID.Higanbana) ?? false) && SenCount == 1 && IsLastAction(true, Yukikaze) && !HaveMeikyoShisui)
//        {
//            if (Hagakure.CanUse(out act)) return true;
//        }

//        //闪影、红莲
//        if (HasMoon && HasFlower)
//        {
//            if (HissatsuGuren.CanUse(out act, !HissatsuSenei.EnoughLevel ? CanUseOption.MustUse : CanUseOption.None)) return true;
//            if (HissatsuSenei.CanUse(out act)) return true;
//        }

//        //照破、无明照破
//        if (Shoha2.CanUse(out act)) return true;
//        if (Shoha.CanUse(out act)) return true;

//        //震天、九天
//        if (Kenki >= 50 && Ikishoten.WillHaveOneCharge(10) || Kenki >= Configs.GetInt("addKenki") || IsTargetBoss && IsTargetDying)
//        {
//            if (HissatsuKyuten.CanUse(out act)) return true;
//            if (HissatsuShinten.CanUse(out act)) return true;
//        }

//        return base.AttackAbility(out act);
//    }
//    protected override bool EmergencyAbility(IAction nextGCD, out IAction act)
//    {
//        var IsTargetBoss = HostileTarget?.IsBossFromTTK() ?? false;
//        var IsTargetDying = HostileTarget?.IsDying() ?? false;

//        //明镜止水
//        if (HasHostilesInRange && IsLastGCD(true, Yukikaze, Mangetsu, Oka) &&
//            (!IsTargetBoss || (HostileTarget?.HasStatus(true, StatusID.Higanbana) ?? false) && !(HostileTarget?.WillStatusEnd(40, true, StatusID.Higanbana) ?? false) || !HasMoon && !HasFlower || IsTargetBoss && IsTargetDying))
//        {
//            if (MeikyoShisui.CanUse(out act, CanUseOption.EmptyOrSkipCombo)) return true;
//        }
//        return base.EmergencyAbility(nextGCD, out act);
//    }


//    protected override IAction CountDownAction(float remainTime)
//    {
//        //开局使用明镜
//        if (remainTime <= 5 && MeikyoShisui.CanUse(out _, CanUseOption.IgnoreClippingCheck)) return MeikyoShisui;
//        //真北防止boss面向没到位
//        if (remainTime <= 2 && TrueNorth.CanUse(out _, CanUseOption.IgnoreClippingCheck)) return TrueNorth;
//        return base.CountDownAction(remainTime);
//    }
//}