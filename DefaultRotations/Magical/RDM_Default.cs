namespace DefaultRotations.Magical;

[RotationDesc(ActionID.Embolden)]
[SourceCode(Path = "main/DefaultRotations/Magical/RDM_Default.cs")]
[LinkDescription("https://www.thebalanceffxiv.com/img/jobs/rdm/rdm_ew_opener.png")]
public sealed class RDM_Default : RDM_Base
{
    public override CombatType Type => CombatType.PvE;

    public override string GameVersion => "6.31";

    public override string RotationName => "Standard";

    static IBaseAction VerthunderStartUp { get; } = new BaseAction(ActionID.Verthunder);

    private static bool CanStartMeleeCombo
    {
        get
        {
            if (Player.HasStatus(true, StatusID.Manafication, StatusID.Embolden) ||
                             BlackMana == 100 || WhiteMana == 100) return true;

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

    protected override IRotationConfigSet CreateConfiguration()
    {
        return base.CreateConfiguration()
            .SetBool(CombatType.PvE, "UseVercure", false, "Use Vercure for Dualcast when out of combat.");
    }

    protected override IAction CountDownAction(float remainTime)
    {
        if (remainTime < VerthunderStartUp.CastTime + CountDownAhead
            && VerthunderStartUp.CanUse(out var act, CanUseOption.EmptyOrSkipCombo)) return act;

        //Remove Swift
        StatusHelper.StatusOff(StatusID.DualCast);
        StatusHelper.StatusOff(StatusID.Acceleration);
        StatusHelper.StatusOff(StatusID.SwiftCast);

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

        if (Configs.GetBool("UseVercure") && NotInCombatDelay && Vercure.CanUse(out act)) return true;

        return base.GeneralGCD(out act);
    }

    protected override bool EmergencyGCD(out IAction act)
    {
        if (ManaStacks == 3)
        {
            if (BlackMana > WhiteMana)
            {
                if (Verholy.CanUse(out act, CanUseOption.MustUse)) return true;
            }
            if (Verflare.CanUse(out act, CanUseOption.MustUse)) return true;
        }

        if (Resolution.CanUse(out act, CanUseOption.MustUse)) return true;
        if (Scorch.CanUse(out act, CanUseOption.MustUse)) return true;


        if (IsLastGCD(true, Moulinet) && Moulinet.CanUse(out act, CanUseOption.MustUse)) return true;
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

        return base.EmergencyGCD(out act);
    }

    protected override bool EmergencyAbility(IAction nextGCD, out IAction act)
    {
        act = null;
        if (CombatElapsedLess(4)) return false;

        if (IsBurst && HasHostilesInRange && Embolden.CanUse(out act, CanUseOption.MustUse)) return true;

        //Use Manafication after embolden.
        if ((Player.HasStatus(true, StatusID.Embolden) || IsLastAbility(ActionID.Embolden))
            && Manafication.CanUse(out act)) return true;

        return base.EmergencyAbility(nextGCD, out act);
    }

    protected override bool AttackAbility(out IAction act)
    {
        //Swift
        if (ManaStacks == 0 && (BlackMana < 50 || WhiteMana < 50)
            && (CombatElapsedLess(4) || !Manafication.EnoughLevel || !Manafication.WillHaveOneChargeGCD(0, 1)))
        {
            if (!Player.HasStatus(true, StatusID.VerfireReady, StatusID.VerstoneReady))
            {
                if (Swiftcast.CanUse(out act)) return true;
                if (InCombat && Acceleration.CanUse(out act, CanUseOption.EmptyOrSkipCombo)) return true;
            }
        }

        if (IsBurst && UseBurstMedicine(out act)) return true;

        //Attack abilities.
        if (ContreSixte.CanUse(out act, CanUseOption.MustUse)) return true;
        if (Fleche.CanUse(out act)) return true;

        if (Engagement.CanUse(out act, CanUseOption.EmptyOrSkipCombo)) return true;
        if (CorpsACorps.CanUse(out act, CanUseOption.MustUse) && !IsMoving) return true;

        return base.AttackAbility(out act);
    }
}

