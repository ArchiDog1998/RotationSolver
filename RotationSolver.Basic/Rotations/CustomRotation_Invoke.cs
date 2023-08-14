using Dalamud.Logging;

namespace RotationSolver.Basic.Rotations;

public abstract partial class CustomRotation
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public bool TryInvoke(out IAction newAction, out IAction gcdAction)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    {
        newAction = gcdAction = null;
        if (!IsEnabled)
        {
            return false;
        }

        try
        {
            UpdateInfo();
            UpdateActions(ClassJob.GetJobRole());
            newAction = Invoke(out gcdAction);
            if (!IsValid) IsValid = true;
        }
        catch (Exception ex)
        {
            WhyNotValid = $"Failed to invoke the next action,please contact to \"{{0}}\".";

            while(ex != null)
            {
                if (!string.IsNullOrEmpty(ex.Message)) WhyNotValid += "\n" + ex.Message;
                if (!string.IsNullOrEmpty(ex.StackTrace)) WhyNotValid += "\n" + ex.StackTrace;
                ex = ex.InnerException;
            }
            IsValid = false;
        }

        return newAction != null;
    }

    private void UpdateActions(JobRole role)
    {
        BaseAction.OtherOption = CanUseOption.IgnoreTarget;

        ActionMoveForwardGCD = MoveForwardGCD(out var act) ? act : null;

        if (!DataCenter.HPNotFull && role == JobRole.Healer)
        {
            ActionHealAreaGCD = ActionHealAreaAbility = ActionHealSingleGCD = ActionHealSingleAbility = null;
        }
        else
        {
            ActionHealAreaGCD = HealAreaGCD(out act) ? act : null;
            ActionHealSingleGCD = HealSingleGCD(out act) ? act : null;

            BaseAction.OtherOption |= CanUseOption.IgnoreClippingCheck;

            ActionHealAreaAbility = HealAreaAbility(out act) ? act : null;
            ActionHealSingleAbility = HealSingleAbility(out act) ? act : null;

            BaseAction.OtherOption &= ~CanUseOption.IgnoreClippingCheck;
        }

        ActionDefenseAreaGCD = DefenseAreaGCD(out act) ? act : null;

        ActionDefenseSingleGCD = DefenseSingleGCD(out act) ? act : null;

        EsunaStanceNorthGCD = role switch
        {
            JobRole.Healer => DataCenter.WeakenPeople.Any() && Esuna.CanUse(out act, CanUseOption.MustUse) ? act : null,
            _ => null,
        };

        RaiseShirkGCD = role switch
        {
            JobRole.Healer => DataCenter.DeathPeopleAll.Any() && Raise.CanUse(out act) ? act : null,
            _ => null,
        };

        BaseAction.OtherOption |= CanUseOption.IgnoreClippingCheck;

        ActionDefenseAreaAbility = DefenseAreaAbility(out act) ? act : null;

        ActionDefenseSingleAbility = DefenseSingleAbility(out act) ? act : null;

        EsunaStanceNorthAbility = role switch
        {
            JobRole.Melee => TrueNorth.CanUse(out act) ? act : null,
            JobRole.Tank => TankStance.CanUse(out act) ? act : null,
            _ => null,
        };

        RaiseShirkAbility = role switch
        {
            JobRole.Tank => Shirk.CanUse(out act) ? act : null,
            _ => null,
        };
        AntiKnockbackAbility = AntiKnockback(role, SpecialCommandType.AntiKnockback, out act) ? act : null;

        BaseAction.OtherOption |= CanUseOption.EmptyOrSkipCombo;

        var movingTarget = MoveForwardAbility(out act);
        ActionMoveForwardAbility = movingTarget ? act : null;

        if (movingTarget && act is IBaseAction a)
        {
            if(a.Target == null || a.Target == Player)
            {
                if((ActionID)a.ID == ActionID.EnAvant)
                {
                    var dir =  new Vector3(MathF.Sin(Player.Rotation), 0, MathF.Cos(Player.Rotation));
                    MoveTarget = Player.Position + dir * 10;
                }
                else
                {
                    MoveTarget = a.Position == a.Target.Position ? null : a.Position;
                }
            }
            else
            {
                var dir = Player.Position - a.Target.Position;
                var length = dir.Length();
                if(length != 0)
                {
                    dir /= length;

                    MoveTarget = a.Target.Position + dir * MathF.Min(length, Player.HitboxRadius + a.Target.HitboxRadius);
                }
                else
                {
                    MoveTarget = a.Target.Position;
                }
            }
        }
        else
        {
            MoveTarget = null;
        }

        ActionMoveBackAbility = MoveBackAbility(out act) ? act : null;
        ActionSpeedAbility = SpeedAbility(out act) ? act : null;

        BaseAction.OtherOption = CanUseOption.None;
    }

    private IAction Invoke(out IAction gcdAction)
    {
        var countDown = Service.CountDownTime;
        if (countDown > 0)
        {
            gcdAction = null;
            return CountDownAction(countDown);
        }

        var helpDefenseAOE = Service.Config.GetValue(Configuration.PluginConfigBool.UseDefenseAbility) && DataCenter.IsHostileCastingAOE;

        bool helpDefenseSingle = false;
        if (ClassJob.GetJobRole() == JobRole.Healer || ClassJob.RowId == (uint)ECommons.ExcelServices.Job.PLD)
        {
            if (DataCenter.PartyTanks.Any((tank) =>
            {
                var attackingTankObj = DataCenter.HostileTargets.Where(t => t.TargetObjectId == tank.ObjectId);

                if (attackingTankObj.Count() != 1) return false;

                return DataCenter.IsHostileCastingToTank;
            })) helpDefenseSingle = true;
        }

        BaseAction.OtherOption = CanUseOption.IgnoreClippingCheck;
        gcdAction = GCD(helpDefenseAOE, helpDefenseSingle);
        BaseAction.OtherOption = CanUseOption.None;

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

    /// <summary>
    /// The action in countdown.
    /// </summary>
    /// <param name="remainTime"></param>
    /// <returns></returns>
    protected virtual IAction CountDownAction(float remainTime) => null;
}
