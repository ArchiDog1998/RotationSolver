namespace DefaultRotations.Tank;

[SourceCode(Path = "main/DefaultRotations/Tank/WAR_Default.cs")]
[LinkDescription("https://cdn.discordapp.com/attachments/277962807813865472/963548326433796116/unknown.png")]
public sealed class WAR_Default : WAR_Base
{
    public override CombatType Type => CombatType.PvE;

    public override string GameVersion => "6.35";

    public override string RotationName => "All-Around";

    private static bool IsBurstStatus => !Player.WillStatusEndGCD(0, 0, false, StatusID.InnerStrength);

    protected override IAction CountDownAction(float remainTime)
    {
        if (remainTime <= CountDownAhead)
        {
            if (HasTankStance)
            {
                if (Provoke.CanUse(out var act1, CanUseOption.IgnoreClippingCheck)) return act1;
            }
            else
            {
                if (Tomahawk.CanUse(out var act1, CanUseOption.IgnoreClippingCheck)) return act1;
            }
        }
        return base.CountDownAction(remainTime);
    }

    protected override bool GeneralGCD(out IAction act)
    {
        if (!Player.WillStatusEndGCD(3, 0, true, StatusID.SurgingTempest))
        {
            if (!IsMoving && IsBurstStatus && PrimalRend.CanUse(out act, CanUseOption.MustUse))
            {
                if (PrimalRend.Target.DistanceToPlayer() < 1) return true;
            }
            if (IsBurstStatus || !Player.HasStatus(false, StatusID.NascentChaos) || BeastGauge > 80)
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

        if (IsMoveForward && MoveForwardAbility(out act)) return true;
        if (Tomahawk.CanUse(out act)) return true;

        return base.GeneralGCD(out act);
    }

    protected override bool AttackAbility(out IAction act)
    {
        if (Infuriate.CanUse(out act, gcdCountForAbility: 3)) return true;

        if (CombatElapsedLessGCD(1)) return false;

        if (UseBurstMedicine(out act)) return true;
        if (Player.HasStatus(false, StatusID.SurgingTempest)
            && !Player.WillStatusEndGCD(6, 0, true, StatusID.SurgingTempest)
            || !MythrilTempest.EnoughLevel)
        {
            if (Berserk.CanUse(out act, CanUseOption.OnLastAbility)) return true;
        }

        if (IsBurstStatus)
        {
            if (Infuriate.CanUse(out act, CanUseOption.EmptyOrSkipCombo)) return true;
        }

        if (CombatElapsedLessGCD(4)) return false;

        if (Orogeny.CanUse(out act)) return true;
        if (Upheaval.CanUse(out act)) return true;

        var option = CanUseOption.MustUse;
        if (IsBurstStatus) option |= CanUseOption.EmptyOrSkipCombo;
        if (Onslaught.CanUse(out act, option) && !IsMoving) return true;

        return base.AttackAbility(out act);
    }

    protected override bool GeneralAbility(out IAction act)
    {
        //Auto healing
        if (Player.GetHealthRatio() < 0.6f)
        {
            if (ThrillOfBattle.CanUse(out act)) return true;
            if (Equilibrium.CanUse(out act)) return true;
        }

        if (!HasTankStance && NascentFlash.CanUse(out act)) return true;

        return base.GeneralAbility(out act);
    }

    [RotationDesc(ActionID.RawIntuition, ActionID.Vengeance, ActionID.Rampart, ActionID.RawIntuition, ActionID.Reprisal)]
    protected override bool DefenseSingleAbility(out IAction act)
    {
        //10
        if (RawIntuition.CanUse(out act, CanUseOption.OnLastAbility)) return true;

        //30
        if ((!Rampart.IsCoolingDown || Rampart.ElapsedAfter(60)) && Vengeance.CanUse(out act)) return true;

        //20
        if (Vengeance.IsCoolingDown && Vengeance.ElapsedAfter(60) && Rampart.CanUse(out act)) return true;

        if (Reprisal.CanUse(out act)) return true;

        return false;
    }

    [RotationDesc(ActionID.ShakeItOff, ActionID.Reprisal)]
    protected override bool DefenseAreaAbility(out IAction act)
    {
        if (ShakeItOff.CanUse(out act, CanUseOption.MustUse)) return true;
        if (Reprisal.CanUse(out act, CanUseOption.MustUse)) return true;
        return base.DefenseAreaAbility(out act);
    }
}
