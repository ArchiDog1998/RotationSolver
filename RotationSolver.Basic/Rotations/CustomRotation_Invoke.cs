namespace RotationSolver.Basic.Rotations;

partial class CustomRotation
{
    /// <inheritdoc/>
    public bool TryInvoke(out IAction? newAction, out IAction? gcdAction)
    {
        newAction = gcdAction = null;
        if (!IsEnabled)
        {
            return false;
        }

        //if (DataCenter.Territory?.IsPvpZone ?? false)
        //{
        //    if (!Type.HasFlag(CombatType.PvP)) return false;
        //}
        //else
        //{
        //    if (!Type.HasFlag(CombatType.PvE)) return false;
        //}

        try
        {
            UpdateInfo();
            UpdateActions(ClassJob.GetJobRole());

            if (Player.HasStatus(true, StatusID.Guard)) return false;

            CountingOfLastUsing = CountingOfCombatTimeUsing = 0;
            newAction = Invoke(out gcdAction);
            if (InCombat || CountOfTracking == 0)
            {
                AverageCountOfLastUsing =
                    (AverageCountOfLastUsing * CountOfTracking + CountingOfLastUsing)
                    / ++CountOfTracking;
                MaxCountOfLastUsing = Math.Max(MaxCountOfLastUsing, CountingOfLastUsing);

                AverageCountOfCombatTimeUsing =
                    (AverageCountOfCombatTimeUsing * (CountOfTracking - 1) + CountingOfCombatTimeUsing)
                    / CountOfTracking;
                MaxCountOfCombatTimeUsing = Math.Max(MaxCountOfCombatTimeUsing, CountingOfCombatTimeUsing);
            }

            if (!IsValid) IsValid = true;
        }
        catch (Exception? ex)
        {
            WhyNotValid = $"Failed to invoke the next action,please contact to \"{{0}}\".";

            while (ex != null)
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
        IBaseAction.ActionPreview = true;

        ActionMoveForwardGCD = MoveForwardGCD(out var act) ? act : null;

        if (!DataCenter.HPNotFull && role == JobRole.Healer)
        {
            ActionHealAreaGCD = ActionHealAreaAbility = ActionHealSingleGCD = ActionHealSingleAbility = null;
        }
        else
        {
            ActionHealAreaGCD = HealAreaGCD(out act) ? act : null;
            ActionHealSingleGCD = HealSingleGCD(out act) ? act : null;

            ActionHealAreaAbility = HealAreaAbility(out act) ? act : null;
            ActionHealSingleAbility = HealSingleAbility(out act) ? act : null;
        }

        ActionDefenseAreaGCD = DefenseAreaGCD(out act) ? act : null;

        ActionDefenseSingleGCD = DefenseSingleGCD(out act) ? act : null;

        ActionDispelStancePositionalGCD = role switch
        {
            JobRole.Healer => DataCenter.WeakenPeople.Any() && DispelGCD(out act) ? act : null,
            _ => null,
        };

        ActionRaiseShirkGCD = role switch
        {
            JobRole.Healer => DataCenter.DeathPeopleAll.Any() && RaiseSpell(out act, true) ? act : null,
            _ => null,
        };

        ActionDefenseAreaAbility = DefenseAreaAbility(out act) ? act : null;

        ActionDefenseSingleAbility = DefenseSingleAbility(out act) ? act : null;

        ActionDispelStancePositionalAbility = role switch
        {
            JobRole.Melee => TrueNorthPvE.CanUse(out act) ? act : null,
            JobRole.Tank => TankStance.CanUse(out act) ? act : null,
            _ => null,
        };

        ActionRaiseShirkAbility = role switch
        {
            JobRole.Tank => ShirkPvE.CanUse(out act) ? act : null,
            _ => null,
        };
        ActionAntiKnockbackAbility = AntiKnockback(role, out act) ? act : null;

        var movingTarget = MoveForwardAbility(out act);
        ActionMoveForwardAbility = movingTarget ? act : null;

        //TODO: that is too complex! 
        if (movingTarget && act is IBaseAction a)
        {
            if (a.Target == null || a.Target?.Target == Player)
            {
                if ((ActionID)a.ID == ActionID.EnAvantPvE)
                {
                    var dir = new Vector3(MathF.Sin(Player.Rotation), 0, MathF.Cos(Player.Rotation));
                    MoveTarget = Player.Position + dir * 10;
                }
                else
                {
                    MoveTarget = a.Target?.Position == a.Target?.Target.Position ? null : a.Target?.Position;
                }
            }
            else
            {
                var dir = Player.Position - a.Target.Value.Position;
                var length = dir.Length();
                if (length != 0)
                {
                    dir /= length;

                    MoveTarget = a.Target.Value.Position + dir * MathF.Min(length, Player.HitboxRadius + a.Target.Value.Target.HitboxRadius);
                }
                else
                {
                    MoveTarget = a.Target.Value.Position;
                }
            }
        }
        else
        {
            MoveTarget = null;
        }

        ActionMoveBackAbility = MoveBackAbility(out act) ? act : null;
        ActionSpeedAbility = SpeedAbility(out act) ? act : null;

        IBaseAction.ActionPreview = false;
    }

    private IAction? Invoke(out IAction? gcdAction)
    {
        var countDown = Service.CountDownTime;
        if (countDown > 0)
        {
            gcdAction = null;
            return CountDownAction(countDown);
        }

        IBaseAction.IgnoreClipping = true;
        gcdAction = GCD();
        IBaseAction.IgnoreClipping = false;

        if (gcdAction != null)
        {
            if (DataCenter.NextAbilityToNextGCD < DataCenter.MinAnimationLock + DataCenter.Ping
                || DataCenter.WeaponTotal < DataCenter.CastingTotal) return gcdAction;

            if (Ability(gcdAction, out var ability)) return ability;

            return gcdAction;
        }
        else
        {
            IBaseAction.IgnoreClipping = true;
            if (Ability(AddlePvE, out var ability)) return ability;
            IBaseAction.IgnoreClipping = false;

            return null;
        }
    }

    /// <summary>
    /// The action in countdown.
    /// </summary>
    /// <param name="remainTime"></param>
    /// <returns></returns>
    protected virtual IAction? CountDownAction(float remainTime) => null;
}
