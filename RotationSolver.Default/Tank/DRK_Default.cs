namespace RotationSolver.Default.Tank;


[RotationDesc(ActionID.BloodWeapon, ActionID.Delirium)]
[SourceCode("https://github.com/ArchiDog1998/RotationSolver/blob/main/RotationSolver.Default/Tank/DRK_Default.cs")]
internal sealed class DRK_Default : DRK_Base
{
    public override string GameVersion => "6.31";

    public override string RotationName => "Default";

    protected override bool CanHealSingleAbility => false;

    private static bool InDeliruim => !Delirium.EnoughLevel || Delirium.IsCoolingDown && Delirium.ElapsedAfterGCD(1) && !Delirium.ElapsedAfterGCD(7);

    private static bool CombatLess => CombatElapsedLess(3);

    private bool CheckDrakSide
    {
        get
        {
            if (DarkSideEndAfterGCD(3)) return true;

            if (CombatLess) return false;

            if (Configs.GetBool("TheBlackestNight") && Player.CurrentMp < 6000) return false;

            if (InDeliruim || HasDarkArts) return true;

            return Player.CurrentMp >= 8500;
        }
    }

    private bool UseBlood
    {
        get
        {
            if (!Delirium.EnoughLevel) return true;

            if (Player.HasStatus(true, StatusID.Delirium) && Player.StatusStack(true, StatusID.BloodWeapon) < 2) return true;

            if (BloodWeapon.WillHaveOneChargeGCD(1) || Blood >= 90 && !Player.HasStatus(true, StatusID.Delirium)) return true;

            return false;
        }
    }

    protected override IRotationConfigSet CreateConfiguration()
        => base.CreateConfiguration()
            .SetBool("TheBlackestNight", true, "Keep 3000 MP");

    protected override IAction CountDownAction(float remainTime)
    {
        //Provoke when has Shield.
        if (HasTankStance && remainTime <= Service.Config.AbilitiesInterval && Provoke.CanUse(out var act)) return act;
        if (remainTime <= 2 && UseBurstMedicine(out act)) return act;
        if (remainTime <= 3 && TheBlackestNight.CanUse(out act)) return act;
        if (remainTime <= 4 && BloodWeapon.CanUse(out act)) return act;
        return base.CountDownAction(remainTime);
    }

    [RotationDesc(ActionID.TheBlackestNight)]
    protected override bool HealSingleAbility(byte abilitiesRemaining, out IAction act)
    {
        if (TheBlackestNight.CanUse(out act)) return true;

        return false;
    }

    [RotationDesc(ActionID.DarkMissionary, ActionID.Reprisal)]
    protected override bool DefenseAreaAbility(byte abilitiesRemaining, out IAction act)
    {
        if (DarkMissionary.CanUse(out act)) return true;
        if (Reprisal.CanUse(out act, mustUse: true)) return true;

        return false;
    }

    [RotationDesc(ActionID.TheBlackestNight, ActionID.Oblation, ActionID.ShadowWall, ActionID.Rampart, ActionID.DarkMind, ActionID.Reprisal)]
    protected override bool DefenseSingleAbility(byte abilitiesRemaining, out IAction act)
    {
        if (TheBlackestNight.CanUse(out act)) return true;

        if (abilitiesRemaining == 2)
        {
            //10
            if (HostileTargets.Count() > 1 && Oblation.CanUse(out act)) return true;

            //30
            if (ShadowWall.CanUse(out act)) return true;

            //20
            if (Rampart.CanUse(out act)) return true;
            if (DarkMind.CanUse(out act)) return true;

            //10
            if (Oblation.CanUse(out act, emptyOrSkipCombo: true)) return true;
        }

        if (Reprisal.CanUse(out act)) return true;

        act = null;
        return false;
    }

    protected override bool GeneralGCD(out IAction act)
    {
        //Use Blood
        if (UseBlood)
        {
            if (Quietus.CanUse(out act)) return true;
            if (Bloodspiller.CanUse(out act)) return true;
        }

        //AOE
        if (StalwartSoul.CanUse(out act)) return true;
        if (Unleash.CanUse(out act)) return true;

        //单体
        if (Souleater.CanUse(out act)) return true;
        if (SyphonStrike.CanUse(out act)) return true;
        if (HardSlash.CanUse(out act)) return true;

        if (SpecialType == SpecialCommandType.MoveForward && MoveForwardAbility(1, out act)) return true;
        if (Unmend.CanUse(out act)) return true;

        return false;
    }

    protected override bool AttackAbility(byte abilitiesRemaining, out IAction act)
    {
        if (CheckDrakSide)
        {
            if (FloodofDarkness.CanUse(out act)) return true;
            if (EdgeofDarkness.CanUse(out act)) return true;
        }

        if (InBurst)
        {
            if (BloodWeapon.CanUse(out act)) return true;
            if (Delirium.CanUse(out act)) return true;
        }

        if (CombatLess)
        {
            act = null;
            return false;
        }

        if (LivingShadow.CanUse(out act)) return true;

        if (!IsMoving && SaltedEarth.CanUse(out act, mustUse: true)) return true;

        if (InDeliruim)
        {
            if (Shadowbringer.CanUse(out act, mustUse: true)) return true;

            if (AbyssalDrain.CanUse(out act)) return true;
            if (CarveandSpit.CanUse(out act)) return true;

            if (Shadowbringer.CanUse(out act, mustUse: true, emptyOrSkipCombo: true)) return true;
        }

        if (SaltandDarkness.CanUse(out act)) return true;

        if (Plunge.CanUse(out act, mustUse: true) && !IsMoving) return true;

        return false;
    }
}