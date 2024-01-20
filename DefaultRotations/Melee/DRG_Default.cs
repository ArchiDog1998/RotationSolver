//namespace DefaultRotations.Melee;

//[SourceCode(Path = "main/DefaultRotations/Melee/DRG_Default.cs")]
//public sealed class DRG_Default : DRG_Base
//{
//    public override CombatType Type => CombatType.PvE;

//    public override string GameVersion => "6.18";

//    public override string RotationName => "Default";

//    [RotationDesc(ActionID.SpineShatterDive, ActionID.DragonFireDive)]
//    protected override bool MoveForwardAbility(out IAction act)
//    {
//        if (SpineShatterDive.CanUse(out act)) return true;
//        if (DragonFireDive.CanUse(out act, CanUseOption.MustUse)) return true;

//        return false;
//    }
//    protected override bool EmergencyAbility(IAction nextGCD, out IAction act)
//    {
//        if (nextGCD.IsTheSameTo(true, FullThrust, CoerthanTorment)
//            || Player.HasStatus(true, StatusID.LanceCharge) && nextGCD.IsTheSameTo(false, FangandClaw))
//        {
//            if (LifeSurge.CanUse(out act, CanUseOption.EmptyOrSkipCombo | CanUseOption.OnLastAbility)) return true;
//        }

//        return base.EmergencyAbility(nextGCD, out act);
//    }

//    protected override bool AttackAbility(out IAction act)
//    {
//        if (IsBurst)
//        {
//            if (LanceCharge.CanUse(out act, CanUseOption.MustUse) && Player.HasStatus(true, StatusID.PowerSurge)) return true;
//            if (LanceCharge.CanUse(out act, CanUseOption.MustUse | CanUseOption.OnLastAbility) && !Player.HasStatus(true, StatusID.PowerSurge)) return true;

//            if (DragonSight.CanUse(out act, CanUseOption.MustUse)) return true;

//            if (BattleLitany.CanUse(out act, CanUseOption.MustUse)) return true;
//        }

//        if (Nastrond.CanUse(out act, CanUseOption.MustUse)) return true;

//        if (StarDiver.CanUse(out act, CanUseOption.MustUse)) return true;

//        if (HighJump.EnoughLevel)
//        {
//            if (HighJump.CanUse(out act)) return true;
//        }
//        else
//        {
//            if (Jump.CanUse(out act)) return true;
//        }

//        if (Geirskogul.CanUse(out act, CanUseOption.MustUse)) return true;

//        if (SpineShatterDive.CanUse(out act, CanUseOption.EmptyOrSkipCombo))
//        {
//            if (Player.HasStatus(true, StatusID.LanceCharge) && LanceCharge.ElapsedOneChargeAfterGCD(3)) return true;
//        }
//        if (Player.HasStatus(true, StatusID.PowerSurge) && SpineShatterDive.CurrentCharges != 1 && SpineShatterDive.CanUse(out act)) return true;

//        if (MirageDive.CanUse(out act)) return true;

//        if (DragonFireDive.CanUse(out act, CanUseOption.MustUse))
//        {
//            if (Player.HasStatus(true, StatusID.LanceCharge) && LanceCharge.ElapsedOneChargeAfterGCD(3)) return true;
//        }

//        if (WyrmwindThrust.CanUse(out act, CanUseOption.MustUse)) return true;

//        return base.AttackAbility(out act);
//    }

//    protected override bool GeneralGCD(out IAction act)
//    {
//        if (CoerthanTorment.CanUse(out act)) return true;
//        if (SonicThrust.CanUse(out act)) return true;
//        if (DoomSpike.CanUse(out act)) return true;


//        if (WheelingThrust.CanUse(out act)) return true;
//        if (FangandClaw.CanUse(out act)) return true;


//        if (FullThrust.CanUse(out act)) return true;
//        if (ChaosThrust.CanUse(out act)) return true;

//        if (Player.WillStatusEndGCD(5, 0, true, StatusID.PowerSurge))
//        {
//            if (Disembowel.CanUse(out act)) return true;
//        }

//        if (VorpalThrust.CanUse(out act)) return true;
//        if (TrueThrust.CanUse(out act)) return true;

//        if (IsMoveForward && MoveForwardAbility(out act)) return true;
//        if (PiercingTalon.CanUse(out act)) return true;

//        return base.GeneralGCD(out act);
//    }
//}
