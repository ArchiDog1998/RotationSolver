namespace DefaultRotations.Tank;


[RotationDesc(ActionID.BloodWeapon, ActionID.Delirium)]
[SourceCode(Path = "main/DefaultRotations/Tank/DRK_Balance.cs")]
[LinkDescription("https://www.thebalanceffxiv.com/img/jobs/drk/drk_standard_6.2_v1.png")]
public sealed class DRK_Default : DRK_Base
{
    public override CombatType Type => CombatType.PvE;

    public override string GameVersion => "6.38";

    public override string RotationName => "Balance";

    public override string Description => "Please contact Nore#7219 on Discord for questions about this rotation.";

    public override bool CanHealSingleAbility => false;

    private static bool InTwoMIsBurst()
    {
        if (RatioOfMembersIn2minsBurst >= 0.5) return true;
        if (RatioOfMembersIn2minsBurst == -1 && (BloodWeapon.IsCoolingDown && Delirium.IsCoolingDown && ((LivingShadow.IsCoolingDown && !(LivingShadow.ElapsedAfter(15))) || !LivingShadow.EnoughLevel))) return true;
        else return false;
    }

    private static bool CombatLess => CombatElapsedLess(3);

    private bool CheckDarkSide
    {
        get
        {
            if (DarkSideEndAfterGCD(3)) return true;

            if (CombatLess) return false;

            if ((InTwoMIsBurst() && HasDarkArts) || (HasDarkArts && Player.HasStatus(true, StatusID.TheBlackestNight)) || (HasDarkArts && DarkSideEndAfterGCD(3))) return true;

            if ((InTwoMIsBurst() && BloodWeapon.IsCoolingDown && LivingShadow.IsCoolingDown && SaltedEarth.IsCoolingDown && ShadowBringer.CurrentCharges == 0 && CarveAndSpit.IsCoolingDown  && SaltandDarkness.IsCoolingDown)) return true;

            if (Configs.GetBool("TheBlackestNight") && CurrentMp < 6000) return false;

            return CurrentMp >= 8500;
        }
    }

    private static bool UseBlood
    {
        get
        {
            if (!Delirium.EnoughLevel) return true;

            if (Player.HasStatus(true, StatusID.Delirium) && LivingShadow.IsCoolingDown) return true;

            if ((Delirium.WillHaveOneChargeGCD(1) && !LivingShadow.WillHaveOneChargeGCD(3)) || Blood >= 90 && !LivingShadow.WillHaveOneChargeGCD(1)) return true;

            return false;
        }
    }

    protected override IRotationConfigSet CreateConfiguration()
        => base.CreateConfiguration()
            .SetBool(CombatType.PvE, "TheBlackestNight", true, "Keep at least 3000 MP");

    protected override IAction CountDownAction(float remainTime)
    {
        //Provoke when has Shield.
        if (remainTime <= CountDownAhead)
        {
            if (HasTankStance)
            {
                if (Provoke.CanUse(out _, CanUseOption.IgnoreClippingCheck)) return Provoke;
            }
            //else
            //{
            //    if (Unmend.CanUse(out var act1)) return act1;
            //}
        }
        if (remainTime <= 2 && UseBurstMedicine(out var act)) return act;
        if (remainTime <= 3 && TheBlackestNight.CanUse(out act, CanUseOption.IgnoreClippingCheck)) return act;
        if (remainTime <= 4 && BloodWeapon.CanUse(out act, CanUseOption.IgnoreClippingCheck)) return act;
        return base.CountDownAction(remainTime);
    }

    protected override bool EmergencyAbility(IAction nextGCD, out IAction act)
    {
        if (base.EmergencyAbility(nextGCD, out act)) return true;

        //if ((InCombat && CombatElapsedLess(2) || DataCenter.TimeSinceLastAction.TotalSeconds >= 10) && nextGCD.IsTheSameTo(false, HardSlash, SyphonStrike, Souleater, BloodSpiller, Unmend))
        //if ((InCombat && CombatElapsedLess(2) || DataCenter.TimeSinceLastAction.TotalSeconds >= 10) && Target != null && Target.IsNPCEnemy() && NumberOfHostilesIn(25) == 1)
        if ((InCombat && CombatElapsedLess(2) || TimeSinceLastAction.TotalSeconds >= 10))
        {
            //int[] numbers = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            //foreach (int number in numbers)
            //{
            //    if (BloodWeapon.IsCoolingDown)
            //    {
            //        break;
            //    }

            //    BloodWeapon.CanUse(out act, CanUseOption.MustUse);
            //}
            if (BloodWeapon.CanUse(out act, CanUseOption.MustUse)) return true;

        }

        return base.EmergencyAbility(nextGCD, out act);
    }

    [RotationDesc(ActionID.TheBlackestNight)]
    protected override bool HealSingleAbility(out IAction act)
    {
        if (TheBlackestNight.CanUse(out act)) return true;

        return base.HealSingleAbility(out act);
    }

    [RotationDesc(ActionID.DarkMissionary, ActionID.Reprisal)]
    protected override bool DefenseAreaAbility(out IAction act)
    {
        if (!InTwoMIsBurst() && DarkMissionary.CanUse(out act)) return true;
        if (!InTwoMIsBurst() && Reprisal.CanUse(out act, CanUseOption.MustUse)) return true;

        return base.DefenseAreaAbility(out act);
    }

    [RotationDesc(ActionID.TheBlackestNight, ActionID.Oblation, ActionID.Reprisal, ActionID.ShadowWall, ActionID.Rampart, ActionID.DarkMind)]
    protected override bool DefenseSingleAbility(out IAction act)
    {
        act = null;

        if (Player.HasStatus(true, StatusID.TheBlackestNight)) return false;

        //10
        if (Oblation.CanUse(out act, CanUseOption.EmptyOrSkipCombo | CanUseOption.OnLastAbility)) return true;

        if (Reprisal.CanUse(out act, CanUseOption.MustUse | CanUseOption.OnLastAbility)) return true;

        if (TheBlackestNight.CanUse(out act, CanUseOption.OnLastAbility)) return true;
        //30
        if ((!Rampart.IsCoolingDown || Rampart.ElapsedAfter(60)) && ShadowWall.CanUse(out act)) return true;

        //20
        if (ShadowWall.IsCoolingDown && ShadowWall.ElapsedAfter(60) && Rampart.CanUse(out act)) return true;
        if (DarkMind.CanUse(out act)) return true;

        return base.DefenseAreaAbility(out act);
    }

    protected override bool GeneralGCD(out IAction act)
    {
        //Use Blood
        if (UseBlood)
        {
            if (Quietus.CanUse(out act)) return true;
            if (BloodSpiller.CanUse(out act)) return true;
        }

        //AOE
        if (StalwartSoul.CanUse(out act)) return true;
        if (Unleash.CanUse(out act)) return true;

        //单体
        if (Souleater.CanUse(out act)) return true;
        if (SyphonStrike.CanUse(out act)) return true;
        if (HardSlash.CanUse(out act)) return true;

        if (IsMoveForward && MoveForwardAbility(out act)) return true;
        if (BloodWeapon.IsCoolingDown && !Player.HasStatus(true, StatusID.BloodWeapon) && Unmend.CanUse(out act)) return true;

        return base.GeneralGCD(out act);
    }

    protected override bool AttackAbility(out IAction act)
    {
        //if (InCombat && CombatElapsedLess(2) && BloodWeapon.CanUse(out act)) return true;
        if (CheckDarkSide)
        {
            if (FloodOfDarkness.CanUse(out act)) return true;
            if (EdgeOfDarkness.CanUse(out act)) return true;
        }

        if (IsBurst)
        {
            if (UseBurstMedicine(out act)) return true;
            if (Delirium.CanUse(out act)) return true;
            if (Delirium.ElapsedAfterGCD(1) && !Delirium.ElapsedAfterGCD(3) && BloodWeapon.CanUse(out act)) return true;
            if (LivingShadow.CanUse(out act, CanUseOption.MustUse)) return true;
        }

        if (CombatLess)
        {
            act = null;
            return false;
        }

        if (!IsMoving && SaltedEarth.CanUse(out act, CanUseOption.MustUse)) return true;

        if (ShadowBringer.CanUse(out act, CanUseOption.MustUse)) return true;

        if (NumberOfHostilesInRange >= 3 && AbyssalDrain.CanUse(out act)) return true;
        if (CarveAndSpit.CanUse(out act)) return true;

        if (InTwoMIsBurst())
        {
            if (ShadowBringer.CanUse(out act, CanUseOption.MustUse | CanUseOption.EmptyOrSkipCombo)) return true;

        }

        if (Plunge.CanUse(out act, CanUseOption.MustUse) && !IsMoving) return true;

        if (SaltandDarkness.CanUse(out act)) return true;

        if (InTwoMIsBurst())
        {
            if (Plunge.CanUse(out act, CanUseOption.MustUse | CanUseOption.EmptyOrSkipCombo) && !IsMoving) return true;
        }

        return base.AttackAbility(out act);
    }
}
