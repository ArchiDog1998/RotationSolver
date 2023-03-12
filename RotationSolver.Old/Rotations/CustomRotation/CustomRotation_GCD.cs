using RotationSolver.Actions;
using RotationSolver.Actions.BaseAction;
using RotationSolver.Attributes;
using RotationSolver.Commands;
using RotationSolver.Data;
using RotationSolver.Helpers;
using RotationSolver.Localization;
using RotationSolver.Updaters;
using System.Linq;

namespace RotationSolver.Rotations.CustomRotation;

public abstract partial class CustomRotation
{
    private IAction GCD(byte abilityRemain, bool helpDefenseAOE, bool helpDefenseSingle)
    {
        IAction act = RSCommands.NextAction;
        if (act is IBaseAction a && a != null && a.IsRealGCD && a.CanUse(out _, mustUse: true, skipDisable: true)) return act;

        if (EmergencyGCD(out act)) return act;

        var specialType = RSCommands.SpecialType;

        if (RaiseSpell(specialType, out act, abilityRemain, false)) return act;

        if (specialType == SpecialCommandType.MoveForward && MoveForwardGCD(out act))
        {
            if (act is BaseAction b && ObjectHelper.DistanceToPlayer(b.Target) > 5) return act;
        }

        //General Heal
        if (TargetUpdater.HPNotFull && (ActionUpdater.InCombat || Service.Configuration.HealOutOfCombat))
        {
            if ((specialType == SpecialCommandType.HealArea || CanHealAreaSpell) && HealAreaGCD(out act)) return act;
            if ((specialType == SpecialCommandType.HealSingle || CanHealSingleSpell) && HealSingleGCD(out act)) return act;
        }
        if (specialType == SpecialCommandType.DefenseArea && DefenseAreaGCD(out act)) return act;
        if (specialType == SpecialCommandType.DefenseSingle && DefenseSingleGCD(out act)) return act;

        //Auto Defence
        if (helpDefenseAOE && DefenseAreaGCD(out act)) return act;
        if (helpDefenseSingle && DefenseSingleGCD(out act)) return act;

        //Esuna
        if ((specialType == SpecialCommandType.EsunaStanceNorth || !HasHostilesInRange || Service.Configuration.EsunaAll)
            && TargetUpdater.WeakenPeople.Any() 
            || TargetUpdater.DyingPeople.Any())
        {
            if (Job.GetJobRole() == JobRole.Healer && Esuna.CanUse(out act, mustUse: true)) return act;
        }

        if (GeneralGCD(out var action)) return action;

        if (Service.Configuration.RaisePlayerByCasting && RaiseSpell(specialType, out act, abilityRemain, true)) return act;

        return null;
    }

    private bool RaiseSpell(SpecialCommandType specialType, out IAction act, byte actabilityRemain, bool mustUse)
    {
        act = null;
        if (Raise == null) return false;
        if (Player.CurrentMp <= Service.Configuration.LessMPNoRaise) return false;

        if ((Service.Configuration.RaiseAll ? TargetUpdater.DeathPeopleAll.Any() : TargetUpdater.DeathPeopleParty.Any())
            && Raise.CanUse(out act))
        {
            if (specialType == SpecialCommandType.RaiseShirk || HasSwift)
            {
                return true;
            }
            else if (mustUse)
            {
                if(Swiftcast.CanUse(out act)) return true;
                else
                {
                    act = Raise;
                    return true;
                }
            }
            else if (Service.Configuration.RaisePlayerBySwift && !Swiftcast.IsCoolingDown && actabilityRemain > 0)
            {
                return true;
            }
        }
        return false;
    }

    protected virtual bool EmergencyGCD(out IAction act)
    {
        act = null; return false;
    }

    [RotationDesc(DescType.MoveForwardGCD)]
    protected virtual bool MoveForwardGCD(out IAction act)
    {
        act = null; return false;
    }

    [RotationDesc(DescType.HealSingleGCD)]
    protected virtual bool HealSingleGCD(out IAction act)
    {
        act = null; return false;
    }

    [RotationDesc(DescType.HealAreaGCD)]
    protected virtual bool HealAreaGCD(out IAction act)
    {
        act = null; return false;
    }

    [RotationDesc(DescType.DefenseSingleGCD)]
    protected virtual bool DefenseSingleGCD(out IAction act)
    {
        act = null; return false;
    }

    [RotationDesc(DescType.DefenseAreaGCD)]
    protected virtual bool DefenseAreaGCD(out IAction act)
    {
        act = null; return false;
    }

    protected abstract bool GeneralGCD(out IAction act);
}
