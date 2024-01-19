namespace DefaultRotations.Melee;

[RotationDesc(ActionID.RiddleOfFire)]
[SourceCode(Path = "main/DefaultRotations/Melee/MNK_Default.cs")]
[LinkDescription("https://i.imgur.com/C5lQhpe.png")]
public sealed class MNK_Default : MNK_Base
{
    public override CombatType Type => CombatType.PvE;

    public override string GameVersion => "6.35";

    public override string RotationName => "Lunar Solar";

    protected override IRotationConfigSet CreateConfiguration()
    {
        return base.CreateConfiguration().SetBool(CombatType.PvE, "AutoFormShift", true, "Use Form Shift");
    }

    protected override IAction CountDownAction(float remainTime)
    {
        if (remainTime < 0.2)
        {
            if (Thunderclap.CanUse(out var act, CanUseOption.IgnoreClippingCheck)) return act;
        }
        if (remainTime < 15)
        {
            if (Chakra < 5 && Meditation.CanUse(out var act, CanUseOption.IgnoreClippingCheck)) return act;
            if (FormShift.CanUse(out act, CanUseOption.IgnoreClippingCheck)) return act;
        }

        return base.CountDownAction(remainTime);
    }

    private static bool OpoOpoForm(out IAction act)
    {
        if (ArmOfTheDestroyer.CanUse(out act)) return true;
        if (DragonKick.CanUse(out act)) return true;
        if (BootShine.CanUse(out act)) return true;
        return false;
    }

    private static bool UseLunarPerfectBalance => (HasSolar || Player.HasStatus(false, StatusID.PerfectBalance))
        && (!Player.WillStatusEndGCD(0, 0, false, StatusID.RiddleOfFire) || Player.HasStatus(false, StatusID.RiddleOfFire) || RiddleOfFire.WillHaveOneChargeGCD(2)) && PerfectBalance.WillHaveOneChargeGCD(3);

    private static bool RaptorForm(out IAction act)
    {
        if (FourPointFury.CanUse(out act)) return true;
        if ((Player.WillStatusEndGCD(3, 0, true, StatusID.DisciplinedFist)
            || Player.WillStatusEndGCD(7, 0, true, StatusID.DisciplinedFist)
            && UseLunarPerfectBalance) && TwinSnakes.CanUse(out act)) return true;
        if (TrueStrike.CanUse(out act)) return true;
        return false;
    }

    private static bool CoerlForm(out IAction act)
    {
        if (RockBreaker.CanUse(out act)) return true;
        if (UseLunarPerfectBalance && Demolish.CanUse(out act, CanUseOption.MustUse)
            && (Demolish.Target?.WillStatusEndGCD(7, 0, true, StatusID.Demolish) ?? false)) return true;
        if (Demolish.CanUse(out act)) return true;
        if (SnapPunch.CanUse(out act)) return true;
        return false;
    }

    protected override bool GeneralGCD(out IAction act)
    {
        if (PerfectBalanceActions(out act)) return true;


        if (Player.HasStatus(true, StatusID.CoerlForm))
        {
            if (CoerlForm(out act)) return true;
        }
        if (Player.HasStatus(true, StatusID.RiddleOfFire)
            && !RiddleOfFire.ElapsedAfterGCD(2) && (PerfectBalance.ElapsedAfter(60) || !PerfectBalance.IsCoolingDown))
        {
            if (OpoOpoForm(out act)) return true;
        }
        if (Player.HasStatus(true, StatusID.RaptorForm))
        {
            if (RaptorForm(out act)) return true;
        }
        if (OpoOpoForm(out act)) return true;

        if (IsMoveForward && MoveForwardAbility(out act)) return true;
        if (Chakra < 5 && Meditation.CanUse(out act)) return true;
        if (Configs.GetBool("AutoFormShift") && FormShift.CanUse(out act)) return true;

        return base.GeneralGCD(out act);
    }

    static bool PerfectBalanceActions(out IAction act)
    {
        if (!BeastChakras.Contains(BeastChakra.NONE))
        {
            if (HasSolar && HasLunar)
            {
                if (PhantomRush.CanUse(out act, CanUseOption.MustUse)) return true;
                if (TornadoKick.CanUse(out act, CanUseOption.MustUse)) return true;
            }
            if (BeastChakras.Contains(BeastChakra.RAPTOR))
            {
                if (RisingPhoenix.CanUse(out act, CanUseOption.MustUse)) return true;
                if (FlintStrike.CanUse(out act, CanUseOption.MustUse)) return true;
            }
            else
            {
                if (ElixirField.CanUse(out act, CanUseOption.MustUse)) return true;
            }
        }
        else if (Player.HasStatus(true, StatusID.PerfectBalance) && ElixirField.EnoughLevel)
        {
            //Sometimes, no choice
            if (HasSolar || BeastChakras.Count(c => c == BeastChakra.OPOOPO) > 1)
            {
                if (LunarNadi(out act)) return true;
            }
            else if (BeastChakras.Contains(BeastChakra.COEURL) || BeastChakras.Contains(BeastChakra.RAPTOR))
            {
                if (SolarNadi(out act)) return true;
            }

            //Add status when solar.
            if (Player.WillStatusEndGCD(3, 0, true, StatusID.DisciplinedFist)
                || (HostileTarget?.WillStatusEndGCD(3, 0, true, StatusID.Demolish) ?? false))
            {
                if (SolarNadi(out act)) return true;
            }
            if (LunarNadi(out act)) return true;
        }

        act = null;
        return false;
    }

    static bool LunarNadi(out IAction act)
    {
        if (OpoOpoForm(out act)) return true;
        return false;
    }

    static bool SolarNadi(out IAction act)
    {
        //Emergency usage of status.
        if (!BeastChakras.Contains(BeastChakra.RAPTOR)
            && HasLunar
            && Player.WillStatusEndGCD(1, 0, true, StatusID.DisciplinedFist))
        {
            if (RaptorForm(out act)) return true;
        }
        if (!BeastChakras.Contains(BeastChakra.COEURL)
            && (HostileTarget?.WillStatusEndGCD(1, 0, true, StatusID.Demolish) ?? false))
        {
            if (CoerlForm(out act)) return true;
        }

        if (!BeastChakras.Contains(BeastChakra.OPOOPO))
        {
            if (OpoOpoForm(out act)) return true;
        }
        if (HasLunar && !BeastChakras.Contains(BeastChakra.RAPTOR))
        {
            if (RaptorForm(out act)) return true;
        }
        if (!BeastChakras.Contains(BeastChakra.COEURL))
        {
            if (CoerlForm(out act)) return true;
        }
        if (!BeastChakras.Contains(BeastChakra.RAPTOR))
        {
            if (RaptorForm(out act)) return true;
        }

        return CoerlForm(out act);
    }

    protected override bool EmergencyAbility(IAction nextGCD, out IAction act)
    {
        if (InCombat)
        {
            if (UseBurstMedicine(out act)) return true;
            if (IsBurst && !CombatElapsedLessGCD(2) && RiddleOfFire.CanUse(out act, CanUseOption.OnLastAbility)) return true;
        }
        return base.EmergencyAbility(nextGCD, out act);
    }

    protected override bool AttackAbility(out IAction act)
    {
        act = null;

        if (CombatElapsedLessGCD(3)) return false;

        if (BeastChakras.Contains(BeastChakra.NONE) && Player.HasStatus(true, StatusID.RaptorForm)
            && (!RiddleOfFire.EnoughLevel || Player.HasStatus(false, StatusID.RiddleOfFire) && !Player.WillStatusEndGCD(3, 0, false, StatusID.RiddleOfFire)
            || RiddleOfFire.WillHaveOneChargeGCD(1) && (PerfectBalance.ElapsedAfter(60) || !PerfectBalance.IsCoolingDown)))
        {
            if (PerfectBalance.CanUse(out act, CanUseOption.EmptyOrSkipCombo)) return true;
        }

        if (Brotherhood.CanUse(out act)) return true;

        if (HowlingFist.CanUse(out act)) return true;
        if (SteelPeak.CanUse(out act)) return true;
        if (HowlingFist.CanUse(out act, CanUseOption.MustUse)) return true;

        if (RiddleOfWind.CanUse(out act)) return true;

        return base.AttackAbility(out act);
    }
}
