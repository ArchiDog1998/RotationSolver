using Dalamud.Logging;
using static FFXIVClientStructs.FFXIV.Client.UI.Misc.ConfigModule;

namespace RotationSolver.Basic.Rotations;

public abstract partial class CustomRotation
{
    Exception _lastException;
    public bool TryInvoke(out IAction newAction, out IAction gcdAction)
    {
        newAction = gcdAction = null;
        if (!IsEnabled)
        {
            return false;
        }
        var role = Job.GetJobRole();

        ActionMoveForwardGCD = MoveForwardGCD(out var act) ? act : null;

        BaseAction.OtherOption = CanUseOption.IgnoreTarget | CanUseOption.EmptyOrSkipCombo | CanUseOption.IgnoreClippingCheck;
        var movingTarget = MoveForwardAbility(out act);
        BaseAction.OtherOption = CanUseOption.None;

        ActionMoveForwardAbility = movingTarget ? act : null;
        MoveTarget = (movingTarget && act is IBaseAction a) ? a.Target : null;

        ActionMoveBackAbility = MoveBackAbility(out act) ? act : null;

        if(!DataCenter.HPNotFull && role == JobRole.Healer)
        {
            ActionHealAreaGCD = ActionHealAreaAbility = ActionHealSingleGCD = ActionHealSingleAbility = null;
        }
        else
        {
            ActionHealAreaGCD = HealAreaGCD(out act) ? act : null;
            ActionHealAreaAbility = HealAreaAbility(out act) ? act : null;

            ActionHealSingleGCD = HealSingleGCD(out act) ? act : null;
            ActionHealSingleAbility = HealSingleAbility(out act) ? act : null;
        }

        ActionDefenseAreaGCD = DefenseAreaGCD(out act) ? act : null;
        ActionDefenseAreaAbility = DefenseAreaAbility(out act) ? act : null;

        ActionDefenseSingleGCD = DefenseSingleGCD(out act) ? act : null;
        ActionDefenseSingleAbility = DefenseSingleAbility(out act) ? act : null;

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

        try
        {
            newAction = Invoke(out gcdAction);
            if (!IsValid) IsValid = true;
        }
        catch (Exception ex)
        {
            if(_lastException?.GetType() != ex.GetType())
            {
                PluginLog.Error(ex, $"Failed to invoke the next action in \"{GetType().FullName}\", please contact to the author.");
            }
            _lastException = ex;
            IsValid = false;
        }

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

        var helpDefenseAOE = Service.Config.GetValue(SettingsCommand.UseDefenseAbility) && DataCenter.IsHostileCastingAOE;

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

        gcdAction = GCD(helpDefenseAOE, helpDefenseSingle);

        if (gcdAction != null)
        {
            if (DataCenter.NextAbilityToNextGCD < DataCenter.MinAnimationLock + DataCenter.Ping 
                || DataCenter.WeaponTotal < DataCenter.CastingTotal) return gcdAction;

            if (Ability(gcdAction, out IAction ability, helpDefenseAOE, helpDefenseSingle)) return ability;

            return gcdAction;
        }
        else
        {
            BaseAction.OtherOption = CanUseOption.IgnoreClippingCheck;
            if (Ability(Addle, out IAction ability, helpDefenseAOE, helpDefenseSingle)) return ability;
            BaseAction.OtherOption = CanUseOption.None;

            return null;
        }
    }

    protected virtual IAction CountDownAction(float remainTime) => null;
}
