namespace RotationSolver.Default.Melee;

[RotationDesc(ActionID.RiddleOfFire)]
[SourceCode("https://github.com/ArchiDog1998/RotationSolver/blob/main/RotationSolver.Default/Melee/MNK_Default.cs")]
[LinkDescription("https://i.imgur.com/C5lQhpe.png")]
public sealed class MNK_Default : MNK_Base
{
    public override string GameVersion => "6.35";

    public override string RotationName => "LunarSolarOpener";

    protected override IRotationConfigSet CreateConfiguration()
    {
        return base.CreateConfiguration().SetBool("AutoFormShift", true, "Auto use FormShift");
    }

    protected override IAction CountDownAction(float remainTime)
    {
        if (remainTime < 0.2)
        {
            if (Thunderclap.CanUse(out var act, true, true)) return act;
        }
        if (remainTime < 15)
        {
            if (Chakra < 5 && Meditation.CanUse(out var act)) return act;
            if (FormShift.CanUse(out act)) return act;
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

    private static bool RaptorForm(out IAction act)
    {
        if (FourPointFury.CanUse(out act)) return true;
        if ((Player.WillStatusEndGCD(3, 0, true, StatusID.DisciplinedFist)
            || Player.WillStatusEndGCD(6, 0, true, StatusID.DisciplinedFist) 
            && (Player.HasStatus(false, StatusID.RiddleOfFire) || RiddleOfFire.WillHaveOneChargeGCD(3))
            ) && TwinSnakes.CanUse(out act)) return true;
        if (TrueStrike.CanUse(out act)) return true;
        return false;
    }

    private static bool CoerlForm(out IAction act)
    {
        if (RockBreaker.CanUse(out act)) return true;
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
        else if (Player.HasStatus(true, StatusID.RaptorForm))
        {
            if (RaptorForm(out act)) return true;
        }
        if (OpoOpoForm(out act)) return true;

        if (SpecialType == SpecialCommandType.MoveForward && MoveForwardAbility(1, out act)) return true;
        if (Chakra < 5 && Meditation.CanUse(out act)) return true;
        if (Configs.GetBool("AutoFormShift") && FormShift.CanUse(out act)) return true;

        return false;
    }

    static bool PerfectBalanceActions(out IAction act)
    {
        if (!BeastChakras.Contains(BeastChakra.NONE))
        {
            if (HasSolar && HasLunar)
            {
                if (PhantomRush.CanUse(out act, mustUse: true)) return true;
                if (TornadoKick.CanUse(out act, mustUse: true)) return true;
            }
            if (BeastChakras.Contains(BeastChakra.RAPTOR))
            {
                if (RisingPhoenix.CanUse(out act, mustUse: true)) return true;
                if (FlintStrike.CanUse(out act, mustUse: true)) return true;
            }
            else
            {
                if (ElixirField.CanUse(out act, mustUse: true)) return true;
            }
        }
        else if (Player.HasStatus(true, StatusID.PerfectBalance) && ElixirField.EnoughLevel)
        {
            //Some time, no choice
            if (HasSolar)
            {
                if (LunarNadi(out act)) return true;
            }
            else if (BeastChakras.Contains(BeastChakra.COEURL) || BeastChakras.Contains(BeastChakra.RAPTOR))
            {
                if (SolarNadi(out act)) return true;
            }

            //Add status when solar.
            if (Player.WillStatusEndGCD(3, 0, true, StatusID.DisciplinedFist)
                || Target.WillStatusEndGCD(3, 0, true, StatusID.Demolish))
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
            && Player.WillStatusEndGCD(1, 0, true, StatusID.DisciplinedFist))
        {
            if (RaptorForm(out act)) return true;
        }
        if (!BeastChakras.Contains(BeastChakra.COEURL)
            && Target.WillStatusEndGCD(1, 0, true, StatusID.Demolish))
        {
            if (CoerlForm(out act)) return true;
        }

        if (!BeastChakras.Contains(BeastChakra.RAPTOR))
        {
            if (RaptorForm(out act)) return true;
        }
        if (!BeastChakras.Contains(BeastChakra.OPOOPO))
        {
            if (OpoOpoForm(out act)) return true;
        }
        if (!BeastChakras.Contains(BeastChakra.COEURL))
        {
            if (CoerlForm(out act)) return true;
        }

        return CoerlForm(out act);
    }

    protected override bool AttackAbility(byte abilitiesRemaining, out IAction act)
    {
        act = null;
        if (abilitiesRemaining == 1 && InCombat)
        {
            if (UseBurstMedicine(out act)) return true;
            if (InBurst && !CombatElapsedLessGCD(2) && RiddleOfFire.CanUse(out act)) return true;
        }

        if (CombatElapsedLessGCD(3)) return false;

        if (BeastChakras.Contains(BeastChakra.NONE) && Player.HasStatus(true, StatusID.RaptorForm)
            && (!RiddleOfFire.EnoughLevel  || Player.HasStatus(false, StatusID.RiddleOfFire) 
            || RiddleOfFire.WillHaveOneChargeGCD(3) && (PerfectBalance.ElapsedAfter(60) || !PerfectBalance.IsCoolingDown)))
        {
            if (PerfectBalance.CanUse(out act, emptyOrSkipCombo: true)) return true;
        }

        if (Brotherhood.CanUse(out act)) return true;

        if (RiddleOfWind.CanUse(out act)) return true;

        if (HowlingFist.CanUse(out act)) return true;
        if (SteelPeak.CanUse(out act)) return true;
        if (HowlingFist.CanUse(out act, mustUse: true)) return true;

        return false;
    }
}
