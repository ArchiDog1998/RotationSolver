namespace RotationSolver.Old.Melee;

[RotationDesc(ActionID.RiddleOfFire)]
[SourceCode("https://github.com/ArchiDog1998/RotationSolver/blob/main/RotationSolver.Old/Melee/MNK_Old.cs")]
public sealed class MNK_Old : MNK_Base
{
    public override string GameVersion => "6.0";

    public override string RotationName => "Old";

    protected override IRotationConfigSet CreateConfiguration()
    {
        return base.CreateConfiguration().SetBool("AutoFormShift", true, "Auto use FormShift");
    }

    private bool OpoOpoForm(out IAction act)
    {
        if (ArmoftheDestroyer.CanUse(out act)) return true;
        if (DragonKick.CanUse(out act)) return true;
        if (Bootshine.CanUse(out act)) return true;
        return false;
    }

    private bool RaptorForm(out IAction act)
    {
        if (FourPointFury.CanUse(out act)) return true;

        if (Player.WillStatusEndGCD(3, 0, true, StatusID.DisciplinedFist) && TwinSnakes.CanUse(out act)) return true;

        if (TrueStrike.CanUse(out act)) return true;
        return false;
    }

    private bool CoerlForm(out IAction act)
    {
        if (RockBreaker.CanUse(out act)) return true;
        if (Demolish.CanUse(out act)) return true;
        if (SnapPunch.CanUse(out act)) return true;
        return false;
    }

    private bool LunarNadi(out IAction act)
    {
        if (OpoOpoForm(out act)) return true;
        return false;
    }

    private bool SolarNadi(out IAction act)
    {
        if (!BeastChakras.Contains(BeastChakra.RAPTOR))
        {
            if (RaptorForm(out act)) return true;
        }
        else if (!BeastChakras.Contains(BeastChakra.OPOOPO))
        {
            if (OpoOpoForm(out act)) return true;
        }
        else
        {
            if (CoerlForm(out act)) return true;
        }

        return false;
    }

    protected override bool GeneralGCD(out IAction act)
    {
        //满了的话，放三个大招
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
        //有震脚就阴阳
        else if (Player.HasStatus(true, StatusID.PerfectBalance))
        {
            if (HasSolar && LunarNadi(out act)) return true;
            if (SolarNadi(out act)) return true;
        }

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

    protected override bool AttackAbility(byte abilitiesRemaining, out IAction act)
    {
        if (InBurst)
        {
            if (RiddleOfFire.CanUse(out act)) return true;
            if (Brotherhood.CanUse(out act)) return true;
        }

        //震脚
        if (BeastChakras.Contains(BeastChakra.NONE))
        {
            //有阳斗气
            if (HasSolar)
            {
                //两种Buff都在6s以上
                var dis = Player.WillStatusEndGCD(3, 0, true, StatusID.DisciplinedFist);

                Demolish.CanUse(out _);
                var demo = Demolish.Target.WillStatusEndGCD(3, 0, true, StatusID.Demolish);

                if (!dis && (!demo || !PerfectBalance.IsCoolingDown))
                {
                    if (PerfectBalance.CanUse(out act, emptyOrSkipCombo: true)) return true;
                }
            }
            else
            {
                if (PerfectBalance.CanUse(out act, emptyOrSkipCombo: true)) return true;
            }
        }

        if (RiddleofWind.CanUse(out act)) return true;

        if (HowlingFist.CanUse(out act)) return true;
        if (SteelPeak.CanUse(out act)) return true;
        if (HowlingFist.CanUse(out act, mustUse: true)) return true;

        return false;
    }
}
