namespace RotationSolver.Default.Tank;

[SourceCode("https://github.com/ArchiDog1998/RotationSolver/blob/main/RotationSolver.Default/Tank/WAR_Default.cs")]
[LinkDescription("https://cdn.discordapp.com/attachments/277962807813865472/963548326433796116/unknown.png")]
public sealed class WAR_Default : WAR_Base
{
    public override string GameVersion => "6.35";

    public override string RotationName => "All-Around";

    private static bool InBurstStatus => !Player.WillStatusEndGCD(0, 0, false, StatusID.InnerStrength);

    protected override IAction CountDownAction(float remainTime)
    {
        if (remainTime <= Service.Config.CountDownAhead)
        {
            if (HasTankStance)
            {
                if (Provoke.CanUse(out var act1)) return act1;
            }
            else
            {
                if (Tomahawk.CanUse(out var act1)) return act1;
            }
        }
        return base.CountDownAction(remainTime);
    }

    protected override bool GeneralGCD(out IAction act)
    {
        if(!Player.WillStatusEndGCD(3, 0, true, StatusID.SurgingTempest))
        {
            if (!IsMoving && InBurstStatus && PrimalRend.CanUse(out act, CanUseOption.MustUse))
            {
                if (PrimalRend.Target.DistanceToPlayer() < 1) return true;
            }
            if (InBurstStatus || !Player.HasStatus(false, StatusID.NascentChaos) || BeastGauge > 80)
            {
                if (SteelCyclone.CanUse(out act)) return true;
                if (InnerBeast.CanUse(out act)) return true;
            }
        }

        if (MythrilTempest.CanUse(out act)) return true;
        if (Overpower.CanUse(out act)) return true;

        if (StormsEye.CanUse(out act)) return true;
        if (StormsPath.CanUse(out act)) return true;
        if (Maim.CanUse(out act)) return true;
        if (HeavySwing.CanUse(out act)) return true;

        if (SpecialType == SpecialCommandType.MoveForward && MoveForwardAbility(1, out act)) return true;
        if (Tomahawk.CanUse(out act)) return true;

        return false;
    }

    protected override bool AttackAbility(byte abilitiesRemaining, out IAction act)
    {
        if (Infuriate.CanUse(out act, gcdCountForAbility: 3)) return true;

        if (CombatElapsedLessGCD(1)) return false;

        if (abilitiesRemaining == 1)
        {
            if (UseBurstMedicine(out act)) return true;
            if (Player.HasStatus(false, StatusID.SurgingTempest)
                && !Player.WillStatusEndGCD(6, 0, true, StatusID.SurgingTempest)
                || !MythrilTempest.EnoughLevel)
            {
                if (Berserk.CanUse(out act)) return true;
            }
        }

        if (InBurstStatus)
        {
            if (Infuriate.CanUse(out act, CanUseOption.EmptyOrSkipCombo)) return true;
        }

        if (CombatElapsedLessGCD(4)) return false;

        if (Orogeny.CanUse(out act)) return true;
        if (Upheaval.CanUse(out act)) return true;

        var option = CanUseOption.MustUse;
        if (InBurstStatus) option |= CanUseOption.EmptyOrSkipCombo;
        if (Onslaught.CanUse(out act, option) && !IsMoving) return true;

        return false;
    }

    protected override bool GeneralAbility(byte abilitiesRemaining, out IAction act)
    {
        //Auto healing
        if (Player.GetHealthRatio() < 0.6f)
        {
            if (ThrillOfBattle.CanUse(out act)) return true;
            if (Equilibrium.CanUse(out act)) return true;
        }

        if (!HasTankStance && NascentFlash.CanUse(out act)) return true;

        return base.GeneralAbility(abilitiesRemaining, out act);
    }

    [RotationDesc(ActionID.RawIntuition, ActionID.Vengeance, ActionID.Rampart, ActionID.RawIntuition, ActionID.Reprisal)]
    protected override bool DefenseSingleAbility(byte abilitiesRemaining, out IAction act)
    {
        if (abilitiesRemaining == 2)
        {
            if (HostileTargets.Count() > 1)
            {
                //10
                if (RawIntuition.CanUse(out act)) return true;
            }

            //30
            if (Vengeance.CanUse(out act)) return true;

            //20
            if (Rampart.CanUse(out act)) return true;

            //10
            if (RawIntuition.CanUse(out act)) return true;
        }
        if (Reprisal.CanUse(out act)) return true;

        return false;
    }

    [RotationDesc(ActionID.ShakeItOff, ActionID.Reprisal)]
    protected override bool DefenseAreaAbility(byte abilitiesRemaining, out IAction act)
    {
        if (ShakeItOff.CanUse(out act, CanUseOption.MustUse)) return true;
        if (Reprisal.CanUse(out act, CanUseOption.MustUse)) return true;
        return false;
    }
}
