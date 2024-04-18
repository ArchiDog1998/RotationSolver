namespace RotationSolver.Basic.Rotations;

partial class CustomRotation
{
    private static DateTime _nextTimeToHeal = DateTime.MinValue;
    private IAction? GCD()
    {
        var act = DataCenter.CommandNextAction;

        IBaseAction.ForceEnable = true;
        if (act is IBaseAction a && a != null && a.Info.IsRealGCD 
            && a.CanUse(out _, usedUp: true, skipAoeCheck: true)) return act;
        IBaseAction.ForceEnable = false;

        IBaseAction.ShouldEndSpecial = true;

        if (DataCenter.MergedStatus.HasFlag(AutoStatus.LimitBreak)
            && UseLimitBreak(out act)) return act;

        IBaseAction.ShouldEndSpecial = false;

        if (EmergencyGCD(out act)) return act;

        IBaseAction.ShouldEndSpecial = true;

        IBaseAction.TargetOverride = TargetType.Death;

        if (RaiseSpell(out act, false)) return act;

        IBaseAction.TargetOverride = null;

        if (DataCenter.MergedStatus.HasFlag(AutoStatus.MoveForward)
            && MoveForwardGCD(out act))
        {
            if (act is IBaseAction b && ObjectHelper.DistanceToPlayer(b.Target.Target) > 5) return act;
        }

        IBaseAction.TargetOverride = TargetType.Heal;

        if (DataCenter.CommandStatus.HasFlag(AutoStatus.HealAreaSpell))
        {
            if (HealAreaGCD(out act)) return act;
        }
        if (DataCenter.AutoStatus.HasFlag(AutoStatus.HealAreaSpell)
            && CanHealAreaSpell)
        {
            IBaseAction.AutoHealCheck = true;
            if (HealAreaGCD(out act)) return act;
            IBaseAction.AutoHealCheck = false;
        }
        if (DataCenter.CommandStatus.HasFlag(AutoStatus.HealSingleSpell)
            && CanHealSingleSpell)
        {
            if (HealSingleGCD(out act)) return act;
        }
        if (DataCenter.AutoStatus.HasFlag(AutoStatus.HealSingleSpell))
        {
            IBaseAction.AutoHealCheck = true;
            if (HealSingleGCD(out act)) return act;
            IBaseAction.AutoHealCheck = false;
        }

        IBaseAction.TargetOverride = null;

        if (DataCenter.MergedStatus.HasFlag(AutoStatus.DefenseArea)
            && DefenseAreaGCD(out act)) return act;

        IBaseAction.TargetOverride = TargetType.BeAttacked;

        if (DataCenter.MergedStatus.HasFlag(AutoStatus.DefenseSingle)
            && DefenseSingleGCD(out act)) return act;

        IBaseAction.TargetOverride = TargetType.Dispel;
        if (DataCenter.MergedStatus.HasFlag(AutoStatus.Dispel)
            && DispelGCD(out act)) return act;

        IBaseAction.ShouldEndSpecial = false;
        IBaseAction.TargetOverride = null;

        if (GeneralGCD(out var action)) return action;

        if (Service.Config.HealWhenNothingTodo && InCombat)
        {
            // Please don't tell me someone's fps is less than 1!!
            if (DateTime.Now - _nextTimeToHeal > TimeSpan.FromSeconds(1))
            {
                var min = Service.Config.HealWhenNothingTodoDelay.X;
                var max = Service.Config.HealWhenNothingTodoDelay.Y;
                _nextTimeToHeal = DateTime.Now + TimeSpan.FromSeconds(new Random().NextDouble() * (max - min) + min);
            }
            else if (_nextTimeToHeal < DateTime.Now)
            {
                _nextTimeToHeal = DateTime.Now;

                if (PartyMembersMinHP < Service.Config.HealWhenNothingTodoBelow)
                {
                    IBaseAction.TargetOverride =  TargetType.Heal;

                    if (DataCenter.PartyMembersDifferHP < Service.Config.HealthDifference 
                        && DataCenter.PartyMembersHP.Count(i => i < 1) > 2
                        && HealAreaGCD(out act)) return act;
                    if (HealSingleGCD(out act)) return act;

                    IBaseAction.TargetOverride = null;
                }
            }
        }

        IBaseAction.TargetOverride = TargetType.Death;

        if (Service.Config.RaisePlayerByCasting && RaiseSpell(out act, true)) return act;

        IBaseAction.TargetOverride = null;

        return null;
    }

    private bool UseLimitBreak(out IAction? act)
    {
        act = null;

        return LimitBreakLevel switch
        {
            1 => ((DataCenter.IsPvP) 
                ? LimitBreakPvP?.CanUse(out act, skipAoeCheck: true)
                : LimitBreak1?.CanUse(out act, skipAoeCheck: true)) ?? false,
            2 => LimitBreak2?.CanUse(out act, skipAoeCheck: true) ?? false,
            3 => LimitBreak3?.CanUse(out act, skipAoeCheck: true) ?? false,
            _ => false,
        };
    }

    private bool RaiseSpell(out IAction? act, bool mustUse)
    {
        if (DataCenter.CommandStatus.HasFlag(AutoStatus.Raise))
        {
            if (RaiseGCD(out act) || RaiseAction(out act, false)) return true;
        }

        act = null;
        if (!DataCenter.AutoStatus.HasFlag(AutoStatus.Raise)) return false;

        if (RaiseGCD(out act)) return true;

        if (RaiseAction(out act, true))
        {
            if (HasSwift)
            {
                return true;
            }
            else if (mustUse)
            {
                var action = act;
                if (SwiftcastPvE.CanUse(out act))
                {
                    return true;
                }
                else if (!IsMoving)
                {
                    act = action;
                    return true;
                }
            }
            else if (Service.Config.RaisePlayerBySwift && !SwiftcastPvE.Cooldown.IsCoolingDown)
            {
                return true;
            }
        }

        return false;

        bool RaiseAction(out IAction act, bool ignoreCastingCheck)
        {
            if (Player.CurrentMp > Service.Config.LessMPNoRaise && (Raise?.CanUse(out act, skipCastingCheck: ignoreCastingCheck) ?? false)) return true;

            act = null!;
            return false;
        }
    }

    /// <summary>
    /// The gcd for raising.
    /// </summary>
    /// <param name="act">the action.</param>
    /// <returns></returns>
    protected virtual bool RaiseGCD(out IAction? act)
    {
        if (DataCenter.RightNowDutyRotation?.RaiseGCD(out act) ?? false) return true;
        act = null; return false;
    }

    /// <summary>
    /// The gcd for dispeling.
    /// </summary>
    /// <param name="act">the action.</param>
    /// <returns></returns>
    protected virtual bool DispelGCD(out IAction? act)
    {
        if (EsunaPvE.CanUse(out act)) return true;
        if (DataCenter.RightNowDutyRotation?.DispelGCD(out act) ?? false) return true;
        return false;
    }

    /// <summary>
    /// The emergency gcd with highest priority.
    /// </summary>
    /// <param name="act"></param>
    /// <returns></returns>
    protected virtual bool EmergencyGCD(out IAction? act)
    {
        #region PvP
        if (GuardPvP.CanUse(out act)
            && (Player.GetHealthRatio() <= Service.Config.HealthForGuard
            || DataCenter.CommandStatus.HasFlag(AutoStatus.Raise | AutoStatus.Shirk))) return true;

        
        if (StandardissueElixirPvP.CanUse(out act)) return true;
        #endregion

        if (DataCenter.RightNowDutyRotation?.EmergencyGCD(out act) ?? false) return true;

        act = null!; return false;
    }

    /// <summary>
    /// Moving forward GCD.
    /// </summary>
    /// <param name="act"></param>
    /// <returns></returns>
    [RotationDesc(DescType.MoveForwardGCD)]
    protected virtual bool MoveForwardGCD(out IAction? act)
    {
        if (DataCenter.RightNowDutyRotation?.MoveForwardGCD(out act) ?? false) return true;
        act = null; return false;
    }

    /// <summary>
    /// Heal single GCD.
    /// </summary>
    /// <param name="act"></param>
    /// <returns></returns>
    [RotationDesc(DescType.HealSingleGCD)]
    protected virtual bool HealSingleGCD(out IAction? act)
    {
        if (DataCenter.RightNowDutyRotation?.HealSingleGCD(out act) ?? false) return true;
        act = null; return false;
    }

    /// <summary>
    /// Heal area GCD.
    /// </summary>
    /// <param name="act"></param>
    /// <returns></returns>
    [RotationDesc(DescType.HealAreaGCD)]
    protected virtual bool HealAreaGCD(out IAction? act)
    {
        if (DataCenter.RightNowDutyRotation?.HealAreaGCD(out act) ?? false) return true;
        act = null!; return false;
    }

    /// <summary>
    /// Defense single gcd.
    /// </summary>
    /// <param name="act"></param>
    /// <returns></returns>
    [RotationDesc(DescType.DefenseSingleGCD)]
    protected virtual bool DefenseSingleGCD(out IAction? act)
    {
        if (DataCenter.RightNowDutyRotation?.DefenseSingleGCD(out act) ?? false) return true;
        act = null!; return false;
    }

    /// <summary>
    /// Defense area gcd.
    /// </summary>
    /// <param name="act"></param>
    /// <returns></returns>
    [RotationDesc(DescType.DefenseAreaGCD)]
    protected virtual bool DefenseAreaGCD(out IAction? act)
    {
        if (DataCenter.RightNowDutyRotation?.DefenseAreaGCD(out act) ?? false) return true;
        act = null; return false;
    }

    /// <summary>
    /// General GCD.
    /// </summary>
    /// <param name="act"></param>
    /// <returns></returns>
    protected virtual bool GeneralGCD(out IAction? act)
    {
        if (DataCenter.RightNowDutyRotation?.GeneralGCD(out act) ?? false) return true;
        act = null; return false;
    }
}
