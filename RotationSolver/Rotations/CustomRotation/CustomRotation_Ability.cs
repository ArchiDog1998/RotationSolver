using RotationSolver.Actions;
using RotationSolver.Data;
using RotationSolver.Helpers;
using RotationSolver.Updaters;
using System.Data;
using System.Linq;
using RotationSolver.Actions.BaseAction;
using RotationSolver.Commands;

namespace RotationSolver.Rotations.CustomRotation;

internal abstract partial class CustomRotation
{
    private bool Ability(byte abilitiesRemaining, IAction nextGCD, out IAction act, bool helpDefenseAOE, bool helpDefenseSingle)
    {
        act = RSCommands.NextAction;
        if (act is IBaseAction a && a != null && !a.IsRealGCD && a.CanUse(out _, mustUse: true, skipDisable: true)) return true;

        if (!Service.Configuration.UseAbility || Player.TotalCastTime - Player.CurrentCastTime > Service.Configuration.WeaponInterval)
        {
            act = null;
            return false;
        }

        if (EmergencyAbility(abilitiesRemaining, nextGCD, out act)) return true;
        var role = Job.GetJobRole();

        if (InterruptAbility(role, out act)) return true;

        var specialType = RSCommands.SpecialType;

        if (ShirkOrShield(role, specialType, out act)) return true;
        if (AntiRepulsion(role, specialType, out act)) return true;

        if (specialType == SpecialCommandType.EsunaShieldNorth && role == JobRole.Melee)
        {
            if (TrueNorth.CanUse(out act)) return true;
        }

        if (GeneralHealAbility(abilitiesRemaining, specialType, out act)) return true;

        if (AutoDefense(abilitiesRemaining, role, helpDefenseAOE, helpDefenseSingle, out act)) return true;

        if (MovingAbility(abilitiesRemaining, specialType, out act)) return true;

        if (GeneralUsingAbility(role, out act)) return true;

        if (GeneralAbility(abilitiesRemaining, out act)) return true;
        if (HaveHostilesInRange && AttackAbility(abilitiesRemaining, out act)) return true;

        //Run!
        if (!InCombat && IsMoving && role == JobRole.RangedPhysical
            && !Service.ClientState.LocalPlayer.HasStatus(false, StatusID.Peloton)
            && Peloton.CanUse(out act, mustUse: true)) return true;

        return false;
    }

    private bool InterruptAbility(JobRole role, out IAction act)
    {
        act = null;
        if (!TargetUpdater.CanInterruptTargets.Any()) return false;

        switch (role)
        {
            case JobRole.Tank:
                if (Interject.CanUse(out act)) return true;
                break;

            case JobRole.Melee:
                if (LegSweep.CanUse(out act)) return true;
                break;

            case JobRole.RangedPhysical:
                if (HeadGraze.CanUse(out act)) return true;
                break;
        }
        return false;
    }

    private bool ShirkOrShield(JobRole role, SpecialCommandType specialType, out IAction act)
    {
        act = null;
        if (role != JobRole.Tank) return false;

        switch (specialType)
        {
            case SpecialCommandType.RaiseShirk:
                if (Shirk.CanUse(out act)) return true;
                break;

            case SpecialCommandType.EsunaShieldNorth:
                if (Shield.CanUse(out act)) return true;
                break;
        }

        if (Service.Configuration.AutoShield)
        {
            if (!TargetUpdater.AllianceTanks.Any(t => t.CurrentHp != 0 && t.HasStatus(false, StatusHelper.SheildStatus)))
            {
                if (!HaveShield && Shield.CanUse(out act)) return true;
            }
        }

        return false;
    }

    private bool AntiRepulsion(JobRole role, SpecialCommandType specialType, out IAction act)
    {
        act = null;

        if (specialType != SpecialCommandType.AntiRepulsion) return false;

        switch (role)
        {
            case JobRole.Tank:
            case JobRole.Melee:
                if (ArmsLength.CanUse(out act)) return true;
                break;
            case JobRole.Healer:
                if (Surecast.CanUse(out act)) return true;
                break;
            case JobRole.RangedPhysical:
                if (ArmsLength.CanUse(out act)) return true;
                break;
            case JobRole.RangedMagicial:
                if (Surecast.CanUse(out act)) return true;
                break;
        }

        return false;
    }

    private bool GeneralHealAbility(byte abilitiesRemaining, SpecialCommandType specialType, out IAction act)
    {
        act = null;
        switch (specialType)
        {
            case SpecialCommandType.DefenseArea:
                if (DefenceAreaAbility(abilitiesRemaining, out act)) return true;
                break;

            case SpecialCommandType.DefenseSingle:
                if (DefenceSingleAbility(abilitiesRemaining, out act)) return true;
                break;
        }

        if ((TargetUpdater.HPNotFull || Job.RowId == (uint)ClassJobID.BlackMage) && ActionUpdater.InCombat)
        {
            if ((RSCommands.SpecialType == SpecialCommandType.HealArea || CanHealAreaAbility) && HealAreaAbility(abilitiesRemaining, out act)) return true;
            if ((RSCommands.SpecialType == SpecialCommandType.HealSingle || CanHealSingleAbility) && HealSingleAbility(abilitiesRemaining, out act)) return true;
        }

        return false;
    }

    private bool AutoDefense(byte abilitiesRemaining, JobRole role, bool helpDefenseAOE, bool helpDefenseSingle, out IAction act)
    {
        act = null;

        if (!ActionUpdater.InCombat || !HaveHostilesInRange) return false;

        //Auto Provoke
        if (role == JobRole.Tank
            && (Service.Configuration.AutoProvokeForTank || TargetUpdater.AllianceTanks.Count() < 2)
            && TargetFilter.ProvokeTarget(TargetUpdater.HostileTargets, true).Count() != TargetUpdater.HostileTargets.Count())

        {
            if (!HaveShield && Shield.CanUse(out act)) return true;
            if (Provoke.CanUse(out act, mustUse: true)) return true;
        }

        //No using defence abilities.
        if (!Service.Configuration.UseDefenceAbility) return false;

        if (helpDefenseAOE)
        {
            if (DefenceAreaAbility(abilitiesRemaining, out act)) return true;
            if (role is JobRole.Melee or JobRole.RangedPhysical or JobRole.RangedMagicial)
            {
                if (DefenceSingleAbility(abilitiesRemaining, out act)) return true;
            }
        }

        //Defnece himself.
        if (role == JobRole.Tank && HaveShield)
        {
            var tarOnmeCount = TargetUpdater.TarOnMeTargets.Count();

            //A lot targets are targeting on me.
            if (tarOnmeCount > 1 && !IsMoving)
            {
                if (ArmsLength.CanUse(out act)) return true;
                if (DefenceSingleAbility(abilitiesRemaining, out act)) return true;
            }

            //Big damage cating action.
            if (tarOnmeCount == 1 && TargetUpdater.IsHostileCastingToTank)
            {
                if (DefenceSingleAbility(abilitiesRemaining, out act)) return true;
            }
        }

        if (helpDefenseSingle && DefenceSingleAbility(abilitiesRemaining, out act)) return true;

        return false;
    }

    private bool MovingAbility(byte abilitiesRemaining, SpecialCommandType specialType, out IAction act)
    {
        act = null;
        if (specialType == SpecialCommandType.MoveForward && MoveForwardAbility(abilitiesRemaining, out act))
        {
            if (act is BaseAction b && TargetFilter.DistanceToPlayer(b.Target) > 5) return true;
        }
        else if (specialType == SpecialCommandType.MoveBack)
        {
            if(MoveBackAbility(abilitiesRemaining, out act)) return true;
        }
        return false;
    }

    private bool GeneralUsingAbility(JobRole role, out IAction act)
    {
        act = null;
        switch (role)
        {
            case JobRole.Tank:
                if (LowBlow.CanUse(out act)) return true;
                break;

            case JobRole.Melee:
                if (SecondWind.CanUse(out act)) return true;
                if (Bloodbath.CanUse(out act)) return true;
                break;

            case JobRole.Healer:
            case JobRole.RangedMagicial:
                if (JobIDs[0] == ClassJobID.BlackMage) break;
                if (LucidDreaming.CanUse(out act)) return true;
                break;

            case JobRole.RangedPhysical:
                if (SecondWind.CanUse(out act)) return true;
                break;
        }
        return false;
    }


    private protected virtual bool EmergencyAbility(byte abilitiesRemaining, IAction nextGCD, out IAction act)
    {
        if (nextGCD is BaseAction action)
        {
            if (Job.GetJobRole() is JobRole.Healer or JobRole.RangedMagicial &&
            action.CastTime >= 5 && Swiftcast.CanUse(out act, emptyOrSkipCombo: true)) return true;

            if (Service.Configuration.AutoUseTrueNorth && abilitiesRemaining == 1 && action.EnermyPositonal != EnemyPositional.None && action.Target != null)
            {
                if (action.EnermyPositonal != action.Target.FindEnemyLocation() && action.Target.HasLocationSide())
                {
                    if (TrueNorth.CanUse(out act, emptyOrSkipCombo: true)) return true;
                }
            }
        }

        act = null;
        return false;
    }

    private protected virtual bool MoveForwardAbility(byte abilitiesRemaining, out IAction act)
    {
        act = null; return false;
    }

    private protected virtual bool MoveBackAbility(byte abilitiesRemaining, out IAction act)
    {
        act = null; return false;
    }

    private protected virtual bool HealSingleAbility(byte abilitiesRemaining, out IAction act)
    {
        act = null; return false;
    }

    private protected virtual bool HealAreaAbility(byte abilitiesRemaining, out IAction act)
    {
        act = null; return false;
    }

    private protected virtual bool DefenceSingleAbility(byte abilitiesRemaining, out IAction act)
    {
        act = null; return false;
    }

    private protected virtual bool DefenceAreaAbility(byte abilitiesRemaining, out IAction act)
    {
        act = null; return false;
    }

    private protected virtual bool GeneralAbility(byte abilitiesRemaining, out IAction act)
    {
        act = null; return false;
    }

    private protected abstract bool AttackAbility(byte abilitiesRemaining, out IAction act);
}
