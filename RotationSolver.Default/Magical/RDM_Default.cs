namespace RotationSolver.Default.Magical;

[RotationDesc(ActionID.Embolden)]
[SourceCode("https://github.com/ArchiDog1998/RotationSolver/blob/main/RotationSolver.Default/Magical/RDM_Default.cs")]
public sealed class RDM_Default : RDM_Base
{
    public override string GameVersion => "6.31";

    public override string RotationName => "Default";

    public bool CanStartMeleeCombo
    {
        get
        {
            if (Player.HasStatus(true, StatusID.Manafication, StatusID.Embolden) ||
                             BlackMana == 100 || WhiteMana == 100) return true;

            //在魔法元没有溢出的情况下，要求较小的魔元不带触发。
            if (BlackMana == WhiteMana) return false;

            else if (WhiteMana < BlackMana)
            {
                if (Player.HasStatus(true, StatusID.VerstoneReady)) return false;
            }
            else
            {
                if (Player.HasStatus(true, StatusID.VerfireReady)) return false;
            }

            if (Player.HasStatus(true, Vercure.StatusProvide)) return false;

            //Waiting for embolden.
            if (Embolden.EnoughLevel && Embolden.WillHaveOneChargeGCD(5)) return false;

            return true;
        }
    }

    static RDM_Default()
    {
        Acceleration.RotationCheck = b => InCombat;
    }

    protected override IRotationConfigSet CreateConfiguration()
    {
        return base.CreateConfiguration()
            .SetBool("UseVercure", true, "Use Vercure for Dualcast");
    }

    protected override IAction CountDownAction(float remainTime)
    {
        if (remainTime < Verthunder.CastTime + Service.Config.CountDownAhead
            && Verthunder.CanUse(out var act)) return act;

        //Remove Swift
        StatusHelper.StatusOff(StatusID.Dualcast);
        StatusHelper.StatusOff(StatusID.Acceleration);
        StatusHelper.StatusOff(StatusID.Swiftcast);

        return base.CountDownAction(remainTime);
    }

    protected override bool GeneralGCD(out IAction act)
    {
        act = null;
        if (ManaStacks == 3) return false;

        if (!Verthunder2.CanUse(out _))
        {
            if (Verfire.CanUse(out act)) return true;
            if (Verstone.CanUse(out act)) return true;
        }

        if (Scatter.CanUse(out act)) return true;
        if (WhiteMana < BlackMana)
        {
            if (Veraero2.CanUse(out act) && BlackMana - WhiteMana != 5) return true;
            if (Veraero.CanUse(out act) && BlackMana - WhiteMana != 6) return true;
        }
        if (Verthunder2.CanUse(out act)) return true;
        if (Verthunder.CanUse(out act)) return true;

        if (Jolt.CanUse(out act)) return true;

        if (Configs.GetBool("UseVercure") && Vercure.CanUse(out act)) return true;

        return false;
    }


    protected override bool EmergencyGCD(out IAction act)
    {
        if (ManaStacks == 3)
        {
            if (BlackMana > WhiteMana)
            {
                if (Verholy.CanUse(out act, mustUse: true)) return true;
            }
            if (Verflare.CanUse(out act, mustUse: true)) return true;
        }

        if (Resolution.CanUse(out act, mustUse: true)) return true;
        if (Scorch.CanUse(out act, mustUse: true)) return true;


        if (IsLastGCD(true, Moulinet) && Moulinet.CanUse(out act, mustUse: true)) return true;
        if (Zwerchhau.CanUse(out act)) return true;
        if (Redoublement.CanUse(out act)) return true;

        if (!CanStartMeleeCombo) return false;

        if (Moulinet.CanUse(out act))
        {
            if (BlackMana >= 60 && WhiteMana >= 60) return true;
        }
        else
        {
            if (BlackMana >= 50 && WhiteMana >= 50 && Riposte.CanUse(out act)) return true;
        }
        if (ManaStacks > 0 && Riposte.CanUse(out act)) return true;

        return false;
    }

    protected override bool EmergencyAbility(byte abilityRemain, IAction nextGCD, out IAction act)
    {
        act = null;
        if (CombatElapsedLess(4)) return false;

        if (InBurst && Embolden.CanUse(out act, mustUse: true)) return true;

        //Use Manafication after embolden.
        if ((Player.HasStatus(true, StatusID.Embolden) || IsLastAbility(ActionID.Embolden))
            && Manafication.CanUse(out act)) return true;

        act = null;
        return false;
    }

    protected override bool AttackAbility(byte abilitiesRemaining, out IAction act)
    {
        //Swift
        if (ManaStacks == 0 && (BlackMana < 50 || WhiteMana < 50)
            && (CombatElapsedLess(4) || !Manafication.EnoughLevel || !Manafication.WillHaveOneChargeGCD(0, 1)))
        {
            if (!Player.HasStatus(true, StatusID.VerfireReady, StatusID.VerstoneReady))
            {
                if (Swiftcast.CanUse(out act)) return true;
                if (Acceleration.CanUse(out act, emptyOrSkipCombo: true)) return true;
            }
        }

        if (InBurst && UseBurstMedicine(out act)) return true;

        //Attack abilities.
        if (ContreSixte.CanUse(out act, mustUse: true)) return true;
        if (Fleche.CanUse(out act)) return true;

        if (Engagement.CanUse(out act, emptyOrSkipCombo: true)) return true;
        if (CorpsAcorps.CanUse(out act, mustUse: true) && !IsMoving) return true;

        return false;
    }
}

