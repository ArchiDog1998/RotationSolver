using RotationSolver.Basic;
using RotationSolver.Basic.Actions;
using RotationSolver.Basic.Attributes;
using RotationSolver.Basic.Data;
using RotationSolver.Basic.Helpers;

namespace RotationSolver.Rotations.CustomRotation;

public abstract partial class CustomRotation
{
    private IAction GCD(byte abilityRemain, bool helpDefenseAOE, bool helpDefenseSingle)
    {
        IAction act = DataCenter.CommandNextAction;
        if (act is IBaseAction a && a != null && a.IsRealGCD && a.CanUse(out _, CanUseOption.MustUse | CanUseOption.SkipDisable | CanUseOption.EmptyOrSkipCombo)) return act;

        if (EmergencyGCD(out act)) return act;

        var specialType = DataCenter.SpecialType;

        if (RaiseSpell(specialType, out act, abilityRemain, false)) return act;

        if (specialType == SpecialCommandType.MoveForward && MoveForwardGCD(out act))
        {
            if (act is IBaseAction b && ObjectHelper.DistanceToPlayer(b.Target) > 5) return act;
        }

        //General Heal
        if ((DataCenter.HPNotFull || Job.GetJobRole() != JobRole.Healer)
            && (DataCenter.InCombat || Service.Config.HealOutOfCombat))
        {
            if ((specialType == SpecialCommandType.HealArea || CanHealAreaSpell) && HealAreaGCD(out act)) return act;
            if ((specialType == SpecialCommandType.HealSingle || CanHealSingleSpell) && HealSingleGCD(out act)) return act;
        }
        if (specialType == SpecialCommandType.DefenseArea && DefenseAreaGCD(out act)) return act;
        if (specialType == SpecialCommandType.DefenseSingle && DefenseSingleGCD(out act)) return act;

        //Auto Defense
        if (DataCenter.SetAutoStatus(AutoStatus.DefenseArea, helpDefenseAOE) && DefenseAreaGCD(out act)) return act;
        if (DataCenter.SetAutoStatus(AutoStatus.DefenseSingle, helpDefenseSingle) && DefenseSingleGCD(out act)) return act;

        //Esuna
        if (DataCenter.SetAutoStatus(AutoStatus.Esuna, (specialType == SpecialCommandType.EsunaStanceNorth 
            || !HasHostilesInRange || Service.Config.EsunaAll)
            && DataCenter.WeakenPeople.Any() 
            || DataCenter.DyingPeople.Any()))
        {
            if (Job.GetJobRole() == JobRole.Healer && Esuna.CanUse(out act, CanUseOption.MustUse)) return act;
        }

        if (GeneralGCD(out var action)) return action;

        if (Service.Config.RaisePlayerByCasting && RaiseSpell(specialType, out act, abilityRemain, true)) return act;

        return null;
    }

    private bool RaiseSpell(SpecialCommandType specialType, out IAction act, byte actabilityRemain, bool mustUse)
    {
        act = null;
        if (Raise == null || Player.CurrentMp <= Service.Config.LessMPNoRaise)
        {
            return DataCenter.SetAutoStatus(AutoStatus.Raise, false);
        }

        if (specialType == SpecialCommandType.RaiseShirk && DataCenter.DeathPeopleAll.Any())
        {
            return true;
        }

        if ((Service.Config.RaiseAll ? DataCenter.DeathPeopleAll.Any() : DataCenter.DeathPeopleParty.Any())
            && Raise.CanUse(out act))
        {
            if (HasSwift)
            {
                return DataCenter.SetAutoStatus(AutoStatus.Raise, true);
            }
            else if (mustUse)
            {
                if(Swiftcast.CanUse(out act))
                {
                    return DataCenter.SetAutoStatus(AutoStatus.Raise, true);
                }
                else
                {
                    act = Raise;
                    return DataCenter.SetAutoStatus(AutoStatus.Raise, true);
                }
            }
            else if (Service.Config.RaisePlayerBySwift && !Swiftcast.IsCoolingDown && actabilityRemain > 0)
            {
                return DataCenter.SetAutoStatus(AutoStatus.Raise, true);
            }
        }
        return DataCenter.SetAutoStatus(AutoStatus.Raise, false);
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
