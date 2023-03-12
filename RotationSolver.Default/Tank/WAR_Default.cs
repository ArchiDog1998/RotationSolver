using RotationSolver.Actions;
using RotationSolver.Attributes;
using RotationSolver.Commands;
using RotationSolver.Data;
using RotationSolver.Helpers;
using RotationSolver.Rotations.Basic;

namespace RotationSolver.Default.Tank;

internal sealed class WAR_Default : WAR_Base
{
    public override string GameVersion => "6.0";

    public override string RotationName => "Default";

    static WAR_Default()
    {
        InnerBeast.RotationCheck = b => !Player.WillStatusEndGCD(3, 0, true, StatusID.SurgingTempest);
    }

    [RotationDesc(ActionID.ShakeItOff, ActionID.Reprisal)]
    protected override bool DefenseAreaAbility(byte abilitiesRemaining, out IAction act)
    {
        if (ShakeItOff.CanUse(out act, mustUse: true)) return true;
        if (Reprisal.CanUse(out act, mustUse: true)) return true;
        return false;
    }

    [RotationDesc(ActionID.PrimalRend)]
    protected override bool MoveForwardGCD(out IAction act)
    {
        if (PrimalRend.CanUse(out act, mustUse: true)) return true;
        return false;
    }

    protected override bool GeneralGCD(out IAction act)
    {
        //¸ã¸ã¹¥»÷
        if (PrimalRend.CanUse(out act, mustUse: true) && !IsMoving)
        {
            if (PrimalRend.Target.DistanceToPlayer() < 1) return true;
        }

        if (SteelCyclone.CanUse(out act)) return true;
        if (InnerBeast.CanUse(out act)) return true;

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

    protected override bool AttackAbility(byte abilitiesRemaining, out IAction act)
    {
        if (!Player.WillStatusEndGCD(3, 0, true, StatusID.SurgingTempest) || !MythrilTempest.EnoughLevel)
        {
            if (!InnerRelease.IsCoolingDown && Berserk.CanUse(out act)) return true;
        }

        if (Player.GetHealthRatio() < 0.6f)
        {
            if (ThrillofBattle.CanUse(out act)) return true;
            if (Equilibrium.CanUse(out act)) return true;
        }

        if (!HasTankStance && NascentFlash.CanUse(out act)) return true;

        if (Infuriate.CanUse(out act, emptyOrSkipCombo: true)) return true;

        if (Orogeny.CanUse(out act)) return true;
        if (Upheaval.CanUse(out act)) return true;

        if (Onslaught.CanUse(out act, mustUse: true) && !IsMoving) return true;

        return false;
    }
}
