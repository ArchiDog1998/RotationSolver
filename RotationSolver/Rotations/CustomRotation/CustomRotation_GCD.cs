using RotationSolver.Actions;
using RotationSolver.Data;
using RotationSolver.Helpers;
using RotationSolver.Updaters;
using System.Linq;
using RotationSolver.Actions.BaseAction;
using RotationSolver.Commands;

namespace RotationSolver.Combos.CustomCombo;

internal abstract partial class CustomRotation
{
    private IAction GCD(byte abilityRemain, bool helpDefenseAOE, bool helpDefenseSingle)
    {
        IAction act = RSCommands.NextAction;
        if (act is BaseAction a && a != null && a.IsRealGCD && a.ShouldUse(out _, mustUse: true, skipDisable: true)) return act;

        if (EmergencyGCD(out act)) return act;

        var specialType = RSCommands.SpecialType;

        if (RaiseSpell(specialType, out act, abilityRemain, false)) return act;

        if (specialType == SpecialCommandType.MoveForward && MoveGCD(out act))
        {
            if (act is BaseAction b && TargetFilter.DistanceToPlayer(b.Target) > 5) return act;
        }

        //General Heal
        if (TargetUpdater.HPNotFull && ActionUpdater.InCombat)
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
        if (specialType == SpecialCommandType.EsunaShieldNorth && TargetUpdater.WeakenPeople.Any() || TargetUpdater.DyingPeople.Any())
        {
            if (Job.GetJobRole() == JobRole.Healer && Esuna.ShouldUse(out act, mustUse: true)) return act;
        }

        if (GeneralGCD(out var action)) return action;

        //Swift Raise
        if (Service.Configuration.RaisePlayerBySwift && (HaveSwift || !Swiftcast.IsCoolDown)
            && RaiseSpell(specialType, out act, abilityRemain, true)) return act;

        if (Service.Configuration.RaisePlayerByCasting && RaiseSpell(specialType, out act, abilityRemain, true)) return act;

        return null;
    }

    private bool RaiseSpell(SpecialCommandType specialType, out IAction act, byte actabilityRemain, bool mustUse)
    {
        act = null;
        if (Raise == null) return false;
        if (Player.CurrentMp <= Service.Configuration.LessMPNoRaise) return false;

        if (Service.Configuration.RaiseAll ? TargetUpdater.DeathPeopleAll.Any() : TargetUpdater.DeathPeopleParty.Any())
        {
            if (Job.RowId == (uint)ClassJobID.RedMage)
            {
                if (HaveSwift && Raise.ShouldUse(out act)) return true;
            }
            else if (specialType == SpecialCommandType.RaiseShirk || HaveSwift || !Swiftcast.IsCoolDown && actabilityRemain > 0 || mustUse)
            {
                if (Raise.ShouldUse(out act)) return true;
            }
        }
        return false;
    }

    private protected virtual bool EmergencyGCD(out IAction act)
    {
        act = null; return false;
    }

    private protected abstract bool GeneralGCD(out IAction act);

    private protected virtual bool MoveGCD(out IAction act)
    {
        act = null; return false;
    }

    private protected virtual bool HealSingleGCD(out IAction act)
    {
        act = null; return false;
    }

    private protected virtual bool HealAreaGCD(out IAction act)
    {
        act = null; return false;
    }

    private protected virtual bool DefenseSingleGCD(out IAction act)
    {
        act = null; return false;
    }

    private protected virtual bool DefenseAreaGCD(out IAction act)
    {
        act = null; return false;
    }
}
