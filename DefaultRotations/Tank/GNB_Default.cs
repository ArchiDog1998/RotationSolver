//namespace DefaultRotations.Tank;

//[SourceCode(Path = "main/DefaultRotations/Tank/GNB_Default.cs")]
//public sealed class GNB_Default : GNB_Base
//{
//    public override CombatType Type => CombatType.PvE;

//    public override string GameVersion => "6.38";

//    public override string RotationName => "Default";

//    public override bool CanHealSingleSpell => false;

//    public override bool CanHealAreaSpell => false;

//    protected override IAction CountDownAction(float remainTime)
//    {
//        if (remainTime <= 0.7 && LightningShot.CanUse(out var act)) return act;
//        if (remainTime <= 1.2 && UseBurstMedicine(out act)) return act;
//        return base.CountDownAction(remainTime);
//    }

//    protected override bool EmergencyAbility(IAction nextGCD, out IAction act)
//    {
//        if (base.EmergencyAbility(nextGCD, out act)) return true;

//        if (InCombat && CombatElapsedLess(30))
//        {
//            if (!CombatElapsedLessGCD(2) && NoMercy.CanUse(out act, CanUseOption.MustUse | CanUseOption.IgnoreClippingCheck)) return true;
//            if (Player.HasStatus(true, StatusID.NoMercy) && BloodFest.CanUse(out act, CanUseOption.MustUse | CanUseOption.IgnoreClippingCheck)) return true;
//        }

//        return base.EmergencyAbility(nextGCD, out act);
//    }

//    protected override bool GeneralGCD(out IAction act)
//    {
//        if (FatedCircle.CanUse(out act, aoeCount: 4)) return true;
//        if (CanUseGnashingFang(out act)) return true;
//        if (FatedCircle.CanUse(out act)) return true;

//        if (DemonSlaughter.CanUse(out act)) return true;
//        if (DemonSlice.CanUse(out act)) return true;

//        if (Player.HasStatus(true, StatusID.NoMercy) && CanUseSonicBreak(out act)) return true;

//        if (Player.HasStatus(true, StatusID.NoMercy) && CanUseDoubleDown(out act)) return true;

//        if (SavageClaw.CanUse(out act, CanUseOption.EmptyOrSkipCombo)) return true;
//        if (WickedTalon.CanUse(out act, CanUseOption.EmptyOrSkipCombo)) return true;

//        if (CanUseBurstStrike(out act)) return true;

//        if (SolidBarrel.CanUse(out act)) return true;
//        if (BrutalShell.CanUse(out act)) return true;
//        if (KeenEdge.CanUse(out act)) return true;

//        if (IsMoveForward && MoveForwardAbility(out act)) return true;

//        if (LightningShot.CanUse(out act)) return true;

//        return base.GeneralGCD(out act);
//    }

//    protected override bool AttackAbility(out IAction act)
//    {
//        //if (IsBurst && CanUseNoMercy(out act)) return true;

//        if (!CombatElapsedLessGCD(5) && NoMercy.CanUse(out act, CanUseOption.MustUse | CanUseOption.IgnoreClippingCheck)) return true;

//        if (JugularRip.CanUse(out act)) return true;

//        if (DangerZone.CanUse(out act))
//        {
//            if (!IsFullParty && !(DangerZone.Target?.IsBossFromTTK() ?? false)) return true;

//            if (!GnashingFang.EnoughLevel && (Player.HasStatus(true, StatusID.NoMercy) || !NoMercy.WillHaveOneCharge(15))) return true;

//            if (Player.HasStatus(true, StatusID.NoMercy) && GnashingFang.IsCoolingDown) return true;

//            if (!Player.HasStatus(true, StatusID.NoMercy) && !GnashingFang.WillHaveOneCharge(20)) return true;
//        }

//        if (Player.HasStatus(true, StatusID.NoMercy) && CanUseBowShock(out act)) return true;

//        if (RoughDivide.CanUse(out act, CanUseOption.MustUse) && !IsMoving) return true;
//        if (GnashingFang.IsCoolingDown && DoubleDown.IsCoolingDown && Ammo == 0 && BloodFest.CanUse(out act)) return true;

//        if (AbdomenTear.CanUse(out act)) return true;

//        if (Player.HasStatus(true, StatusID.NoMercy))
//        {
//            if (RoughDivide.CanUse(out act, CanUseOption.MustUse | CanUseOption.EmptyOrSkipCombo) && !IsMoving) return true;
//        }

//        if (EyeGouge.CanUse(out act)) return true;
//        if (Hypervelocity.CanUse(out act)) return true;

//        return base.AttackAbility(out act);
//    }

//    [RotationDesc(ActionID.HeartOfLight, ActionID.Reprisal)]
//    protected override bool DefenseAreaAbility(out IAction act)
//    {
//        if (!Player.HasStatus(true, StatusID.NoMercy) && HeartOfLight.CanUse(out act, CanUseOption.EmptyOrSkipCombo)) return true;
//        if (!Player.HasStatus(true, StatusID.NoMercy) && Reprisal.CanUse(out act, CanUseOption.MustUse)) return true;
//        return base.DefenseAreaAbility(out act);
//    }

//    [RotationDesc(ActionID.HeartOfStone, ActionID.Nebula, ActionID.Rampart, ActionID.Camouflage, ActionID.Reprisal)]
//    protected override bool DefenseSingleAbility(out IAction act)
//    {
//        //10
//        if (Camouflage.CanUse(out act, CanUseOption.OnLastAbility)) return true;
//        //10
//        if (HeartOfStone.CanUse(out act, CanUseOption.OnLastAbility)) return true;

//        //30
//        if ((!Rampart.IsCoolingDown || Rampart.ElapsedAfter(60)) && Nebula.CanUse(out act)) return true;
//        //20
//        if (Nebula.IsCoolingDown && Nebula.ElapsedAfter(60) && Rampart.CanUse(out act)) return true;

//        if (Reprisal.CanUse(out act)) return true;

//        return base.DefenseSingleAbility(out act);
//    }

//    [RotationDesc(ActionID.Aurora)]
//    protected override bool HealSingleAbility(out IAction act)
//    {
//        if (Aurora.CanUse(out act, CanUseOption.EmptyOrSkipCombo | CanUseOption.OnLastAbility)) return true;
//        return base.HealSingleAbility(out act);
//    }

//    //private bool CanUseNoMercy(out IAction act)
//    //{
//    //    if (!NoMercy.CanUse(out act, CanUseOption.OnLastAbility)) return false;

//    //    if (!IsFullParty && !IsTargetBoss && !IsMoving && DemonSlice.CanUse(out _)) return true;

//    //    if (!BurstStrike.EnoughLevel) return true;

//    //    if (BurstStrike.EnoughLevel)
//    //    {
//    //        if (IsLastGCD((ActionID)KeenEdge.ID) && Ammo == 1 && !GnashingFang.IsCoolingDown && !BloodFest.IsCoolingDown) return true;
//    //        else if (Ammo == (Level >= 88 ? 3 : 2)) return true;
//    //        else if (Ammo == 2 && GnashingFang.IsCoolingDown) return true;
//    //    }

//    //    act = null;
//    //    return false;
//    //}

//    private static bool CanUseGnashingFang(out IAction act)
//    {
//        if (GnashingFang.CanUse(out act))
//        {
//            if (DemonSlice.CanUse(out _)) return false;

//            if (Ammo == MaxAmmo && (Player.HasStatus(true, StatusID.NoMercy) || !NoMercy.WillHaveOneCharge(55))) return true;

//            if (Ammo > 0 && !NoMercy.WillHaveOneCharge(17) && NoMercy.WillHaveOneCharge(35)) return true;

//            if (Ammo == 3 && IsLastGCD((ActionID)BrutalShell.ID) && NoMercy.WillHaveOneCharge(3)) return true;

//            if (Ammo == 1 && !NoMercy.WillHaveOneCharge(55) && BloodFest.WillHaveOneCharge(5)) return true;

//            if (Ammo == 1 && !NoMercy.WillHaveOneCharge(55) && (!BloodFest.IsCoolingDown && BloodFest.EnoughLevel || !BloodFest.EnoughLevel)) return true;
//        }
//        return false;
//    }

//    private static bool CanUseSonicBreak(out IAction act)
//    {
//        if (SonicBreak.CanUse(out act))
//        {
//            if (DemonSlice.CanUse(out _)) return false;

//            //if (!IsFullParty && !SonicBreak.IsTargetBoss) return false;

//            if (!GnashingFang.EnoughLevel && Player.HasStatus(true, StatusID.NoMercy)) return true;

//            if (GnashingFang.IsCoolingDown && Player.HasStatus(true, StatusID.NoMercy)) return true;

//            if (!DoubleDown.EnoughLevel && Player.HasStatus(true, StatusID.ReadyToRip)
//                && GnashingFang.IsCoolingDown) return true;

//        }
//        return false;
//    }

//    private static bool CanUseDoubleDown(out IAction act)
//    {
//        if (DoubleDown.CanUse(out act, CanUseOption.MustUse))
//        {
//            if (DemonSlice.CanUse(out _) && Player.HasStatus(true, StatusID.NoMercy)) return true;

//            if (SonicBreak.IsCoolingDown && Player.HasStatus(true, StatusID.NoMercy)) return true;

//            if (Player.HasStatus(true, StatusID.NoMercy) && !NoMercy.WillHaveOneCharge(55) && BloodFest.WillHaveOneCharge(5)) return true;

//        }
//        return false;
//    }

//    private static bool CanUseBurstStrike(out IAction act)
//    {
//        if (BurstStrike.CanUse(out act))
//        {
//            if (DemonSlice.CanUse(out _)) return false;

//            if (SonicBreak.IsCoolingDown && SonicBreak.WillHaveOneCharge(0.5f) && GnashingFang.EnoughLevel) return false;

//            if (Player.HasStatus(true, StatusID.NoMercy) &&
//                AmmoComboStep == 0 &&
//                !GnashingFang.WillHaveOneCharge(1)) return true;
//            if (!CartridgeCharge2.EnoughLevel && Ammo == 2) return true;

//            if (IsLastGCD((ActionID)BrutalShell.ID) &&
//                (Ammo == MaxAmmo ||
//                BloodFest.WillHaveOneCharge(6) && Ammo <= 2 && !NoMercy.WillHaveOneCharge(10) && BloodFest.EnoughLevel)) return true;

//        }
//        return false;
//    }

//    private static bool CanUseBowShock(out IAction act)
//    {
//        if (BowShock.CanUse(out act, CanUseOption.MustUse))
//        {
//            if (DemonSlice.CanUse(out _) && !IsFullParty) return true;

//            if (!SonicBreak.EnoughLevel && Player.HasStatus(true, StatusID.NoMercy)) return true;

//            if (Player.HasStatus(true, StatusID.NoMercy) && SonicBreak.IsCoolingDown) return true;
//        }
//        return false;
//    }
//}