using RotationSolver.Actions.BaseAction;
using RotationSolver.Basic;
using RotationSolver.Basic.Actions;
using RotationSolver.Basic.Attributes;
using RotationSolver.Basic.Data;
using RotationSolver.Basic.Helpers;

namespace RotationSolver.Rotations.CustomRotation;

public abstract partial class CustomRotation
{
    private bool Ability(byte abilitiesRemaining, IAction nextGCD, out IAction act, bool helpDefenseAOE, bool helpDefenseSingle)
    {
        act = DataCenter.CommandNextAction;
        if (act is IBaseAction a && a != null && !a.IsRealGCD && a.CanUse(out _, mustUse: true, skipDisable: true)) return true;

        if (!Service.Config.UseAbility || Player.TotalCastTime - Player.CurrentCastTime > Service.Config.AbilitiesInterval)
        {
            act = null;
            return false;
        }

        if (EmergencyAbility(abilitiesRemaining, nextGCD, out act)) return true;
        var role = Job.GetJobRole();

        if (InterruptAbility(role, out act)) return true;

        var specialType = DataCenter.SpecialType;

        if (ShirkOrShield(role, specialType, out act)) return true;
        if (AntiKnockback(role, specialType, out act)) return true;

        if (specialType == SpecialCommandType.EsunaStanceNorth && role == JobRole.Melee)
        {
            if (TrueNorth.CanUse(out act)) return true;
        }

        if (GeneralHealAbility(abilitiesRemaining, specialType, out act)) return true;

        if (AutoDefense(abilitiesRemaining, role, helpDefenseAOE, helpDefenseSingle, out act)) return true;

        if (MovingAbility(abilitiesRemaining, specialType, out act)) return true;

        if (GeneralUsingAbility(role, out act)) return true;

        if (GeneralAbility(abilitiesRemaining, out act)) return true;
        if (HasHostilesInRange && AttackAbility(abilitiesRemaining, out act)) return true;

        //Run!
        if (!InCombat && IsMoving && role == JobRole.RangedPhysical
            && Peloton.CanUse(out act, mustUse: true)) return true;

        return false;
    }

    private bool InterruptAbility(JobRole role, out IAction act)
    {
        act = null;
        if (!DataCenter.CanInterruptTargets.Any()) return false;

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

            case SpecialCommandType.EsunaStanceNorth:
                if (Shield.CanUse(out act)) return true;
                break;
        }

        if (Service.Config.AutoShield)
        {
            if (!DataCenter.AllianceTanks.Any(t => t.CurrentHp != 0 && t.HasStatus(false, StatusHelper.TankStanceStatus)))
            {
                if (!HasTankStance && Shield.CanUse(out act)) return true;
            }
        }

        return false;
    }

    private bool AntiKnockback(JobRole role, SpecialCommandType specialType, out IAction act)
    {
        act = null;

        if (specialType != SpecialCommandType.AntiKnockback) return false;

        switch (role)
        {
            case JobRole.Tank:
            case JobRole.Melee:
                if (ArmsLength.CanUse(out act)) return true;
                break;
            case JobRole.Healer:
                if (SureCast.CanUse(out act)) return true;
                break;
            case JobRole.RangedPhysical:
                if (ArmsLength.CanUse(out act)) return true;
                break;
            case JobRole.RangedMagical:
                if (SureCast.CanUse(out act)) return true;
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
                if (DefenseAreaAbility(abilitiesRemaining, out act)) return true;
                break;

            case SpecialCommandType.DefenseSingle:
                if (DefenseSingleAbility(abilitiesRemaining, out act)) return true;
                break;
        }

        if ((DataCenter.HPNotFull || Job.RowId == (uint)ClassJobID.BlackMage) && InCombat)
        {
            if ((DataCenter.SpecialType == SpecialCommandType.HealArea || CanHealAreaAbility) && HealAreaAbility(abilitiesRemaining, out act)) return true;
            if ((DataCenter.SpecialType == SpecialCommandType.HealSingle || CanHealSingleAbility) && HealSingleAbility(abilitiesRemaining, out act)) return true;
        }

        return false;
    }

    private bool AutoDefense(byte abilitiesRemaining, JobRole role, bool helpDefenseAOE, bool helpDefenseSingle, out IAction act)
    {
        act = null;

        if (!InCombat || !HasHostilesInRange) return false;

        //Auto Provoke
        if (role == JobRole.Tank
            && (Service.Config.AutoProvokeForTank || DataCenter.AllianceTanks.Count() < 2)
            && TargetFilter.ProvokeTarget(DataCenter.HostileTargets, true).Count() != DataCenter.HostileTargets.Count())

        {
            if (!HasTankStance && Shield.CanUse(out act)) return true;
            if (Provoke.CanUse(out act, mustUse: true)) return true;
        }

        //No using defense abilities.
        if (!Service.Config.UseDefenseAbility) return false;

        if (helpDefenseAOE)
        {
            if (DefenseAreaAbility(abilitiesRemaining, out act)) return true;
            if (role is JobRole.Melee or JobRole.RangedPhysical or JobRole.RangedMagical)
            {
                if (DefenseSingleAbility(abilitiesRemaining, out act)) return true;
            }
        }

        //Defense himself.
        if (role == JobRole.Tank && HasTankStance)
        {
            var tarOnMeCount = DataCenter.TarOnMeTargets.Count();

            //A lot targets are targeting on me.
            if (tarOnMeCount > 1 && !IsMoving)
            {
                if (ArmsLength.CanUse(out act)) return true;
                if (DefenseSingleAbility(abilitiesRemaining, out act)) return true;
            }

            //Big damage casting action.
            if (tarOnMeCount == 1 && DataCenter.IsHostileCastingToTank)
            {
                if (DefenseSingleAbility(abilitiesRemaining, out act)) return true;
            }
        }

        if (helpDefenseSingle && DefenseSingleAbility(abilitiesRemaining, out act)) return true;

        return false;
    }

    private bool MovingAbility(byte abilitiesRemaining, SpecialCommandType specialType, out IAction act)
    {
        act = null;
        if (specialType == SpecialCommandType.MoveForward && MoveForwardAbility(abilitiesRemaining, out act))
        {
            if (act is BaseAction b && ObjectHelper.DistanceToPlayer(b.Target) > 5) return true;
        }
        else if (specialType == SpecialCommandType.MoveBack)
        {
            if (MoveBackAbility(abilitiesRemaining, out act)) return true;
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
            case JobRole.RangedMagical:
                if (JobIDs[0] == ClassJobID.BlackMage) break;
                if (LucidDreaming.CanUse(out act)) return true;
                break;

            case JobRole.RangedPhysical:
                if (SecondWind.CanUse(out act)) return true;
                break;
        }
        return false;
    }


    protected virtual bool EmergencyAbility(byte abilitiesRemaining, IAction nextGCD, out IAction act)
    {
        if (nextGCD is BaseAction action)
        {
            if (Job.GetJobRole() is JobRole.Healer or JobRole.RangedMagical &&
            action.CastTime >= 5 && Swiftcast.CanUse(out act, emptyOrSkipCombo: true)) return true;

            if (Service.Config.AutoUseTrueNorth && abilitiesRemaining == 1 && action.EnemyPositional != EnemyPositional.None && action.Target != null)
            {
                if (action.EnemyPositional != action.Target.FindEnemyPositional() && action.Target.HasPositional())
                {
                    if (TrueNorth.CanUse(out act, emptyOrSkipCombo: true)) return true;
                }
            }
        }

        act = null;
        return false;
    }

    [RotationDesc(DescType.MoveForwardAbility)]
    protected virtual bool MoveForwardAbility(byte abilitiesRemaining, out IAction act, bool recordTarget = true)
    {
        act = null; return false;
    }

    [RotationDesc(DescType.MoveBackAbility)]
    protected virtual bool MoveBackAbility(byte abilitiesRemaining, out IAction act)
    {
        act = null; return false;
    }

    [RotationDesc(DescType.HealSingleAbility)]
    protected virtual bool HealSingleAbility(byte abilitiesRemaining, out IAction act)
    {
        act = null; return false;
    }

    [RotationDesc(DescType.HealAreaAbility)]
    protected virtual bool HealAreaAbility(byte abilitiesRemaining, out IAction act)
    {
        act = null; return false;
    }

    [RotationDesc(DescType.DefenseSingleAbility)]
    protected virtual bool DefenseSingleAbility(byte abilitiesRemaining, out IAction act)
    {
        act = null; return false;
    }

    [RotationDesc(DescType.DefenseAreaAbility)]
    protected virtual bool DefenseAreaAbility(byte abilitiesRemaining, out IAction act)
    {
        act = null; return false;
    }

    protected virtual bool GeneralAbility(byte abilitiesRemaining, out IAction act)
    {
        act = null; return false;
    }

    protected abstract bool AttackAbility(byte abilitiesRemaining, out IAction act);
}
