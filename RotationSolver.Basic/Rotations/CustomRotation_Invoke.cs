namespace RotationSolver.Basic.Rotations;

public abstract partial class CustomRotation
{
    public bool TryInvoke(out IAction newAction, out IAction gcdAction)
    {
        newAction = gcdAction = null;
        if (!IsEnabled)
        {
            return false;
        }
        var role = Job.GetJobRole();

        ActionMoveForwardGCD = MoveForwardGCD(out var act) ? act : null;
        var movingTarget = MoveForwardAbility(1, out act, CanUseOption.IgnoreTarget);
        ActionMoveForwardAbility = movingTarget ? act : null;
        MoveTarget = (movingTarget && act is IBaseAction a) ? a.Target : null;

        ActionMoveBackAbility = MoveBackAbility(DataCenter.AbilityRemainCount, out act) ? act : null;

        if(!DataCenter.HPNotFull && role == JobRole.Healer)
        {
            ActionHealAreaGCD = ActionHealAreaAbility = ActionHealSingleGCD = ActionHealSingleAbility = null;
        }
        else
        {
            ActionHealAreaGCD = HealAreaGCD(out act) ? act : null;
            ActionHealAreaAbility = HealAreaAbility(DataCenter.AbilityRemainCount, out act) ? act : null;

            ActionHealSingleGCD = HealSingleGCD(out act) ? act : null;
            ActionHealSingleAbility = HealSingleAbility(DataCenter.AbilityRemainCount, out act) ? act : null;
        }

        ActionDefenseAreaGCD = DefenseAreaGCD(out act) ? act : null;
        ActionDefenseAreaAbility = DefenseAreaAbility(DataCenter.AbilityRemainCount, out act) ? act : null;

        ActionDefenseSingleGCD = DefenseSingleGCD(out act) ? act : null;
        ActionDefenseSingleAbility = DefenseSingleAbility(DataCenter.AbilityRemainCount, out act) ? act : null;

        EsunaStanceNorthGCD = role switch
        {
            JobRole.Healer => DataCenter.WeakenPeople.Any() && Esuna.CanUse(out act, CanUseOption.MustUse) ? act : null,
            _ => null,
        };
        EsunaStanceNorthAbility = role switch
        {
            JobRole.Melee => TrueNorth.CanUse(out act) ? act : null,
            JobRole.Tank => TankStance.CanUse(out act) ? act : null,
            _ => null,
        };
        RaiseShirkGCD = role switch
        {
            JobRole.Healer => DataCenter.DeathPeopleAll.Any() && Raise.CanUse(out act) ? act : null,
            _ => null,
        };
        RaiseShirkAbility = role switch
        {
            JobRole.Tank => Shirk.CanUse(out act) ? act : null,
            _ => null,
        };
        AntiKnockbackAbility = AntiKnockback(role, SpecialCommandType.AntiKnockback, out act) ? act : null;
        UpdateInfo();

        newAction = Invoke(out gcdAction);

        return newAction != null;
    }

    private IAction Invoke(out IAction gcdAction)
    {
        var countDown = Service.CountDownTime;
        if (countDown > 0)
        {
            gcdAction = null;
            return CountDownAction(countDown);
        }

        byte abilityRemain = DataCenter.AbilityRemainCount;
        var helpDefenseAOE = Service.Config.UseDefenseAbility && DataCenter.IsHostileCastingAOE;

        bool helpDefenseSingle = false;
        if (Job.GetJobRole() == JobRole.Healer || Job.RowId == (uint)ClassJobID.Paladin)
        {
            if (DataCenter.PartyTanks.Any((tank) =>
            {
                var attackingTankObj = DataCenter.HostileTargets.Where(t => t.TargetObjectId == tank.ObjectId);

                if (attackingTankObj.Count() != 1) return false;

                return DataCenter.IsHostileCastingToTank;
            })) helpDefenseSingle = true;
        }

        gcdAction = GCD(abilityRemain, helpDefenseAOE, helpDefenseSingle);

        if (gcdAction != null)
        {
            if (abilityRemain == 0 || DataCenter.WeaponTotal < DataCenter.CastingTotal) return gcdAction;

            if (Ability(abilityRemain, gcdAction, out IAction ability, helpDefenseAOE, helpDefenseSingle)) return ability;

            return gcdAction;
        }
        else if (gcdAction == null)
        {
            if (Ability(abilityRemain, Addle, out IAction ability, helpDefenseAOE, helpDefenseSingle)) return ability;
            return null;
        }
        return gcdAction;
    }

    protected virtual IAction CountDownAction(float remainTime) => null;
}
