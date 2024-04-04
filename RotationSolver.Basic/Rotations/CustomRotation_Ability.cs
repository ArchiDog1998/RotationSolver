﻿namespace RotationSolver.Basic.Rotations;

partial class CustomRotation
{
    private bool Ability(IAction nextGCD, out IAction? act)
    {
        act = DataCenter.CommandNextAction;

        IBaseAction.ForceEnable = true;
        if (act is IBaseAction a && a != null && !a.Info.IsRealGCD && a.CanUse(out _,
            usedUp: true, skipAoeCheck: true)) return true;
        IBaseAction.ForceEnable = false;

        if (act is IBaseItem i && i.CanUse(out _, true)) return true;

        if (!Service.Config.UseAbility || Player.TotalCastTime > 0)
        {
            act = null!;
            return false;
        }

        if (EmergencyAbility(nextGCD, out act)) return true;
        var role = DataCenter.Role;

        IBaseAction.TargetOverride = TargetType.Interrupt;

        if (DataCenter.MergedStatus.HasFlag(AutoStatus.Interrupt)
            && MyInterruptAbility(role, out act)) return true;

        IBaseAction.TargetOverride = TargetType.Tank;
        IBaseAction.ShouldEndSpecial = true;

        if (DataCenter.MergedStatus.HasFlag(AutoStatus.Shirk)
            && ShirkPvE.CanUse(out act)) return true;

        IBaseAction.TargetOverride = null;

        if (DataCenter.MergedStatus.HasFlag(AutoStatus.TankStance)
            && (TankStance?.CanUse(out act) ?? false)) return true;


        if (DataCenter.MergedStatus.HasFlag(AutoStatus.AntiKnockback)
            && AntiKnockback(role, out act)) return true;

        if (DataCenter.MergedStatus.HasFlag(AutoStatus.Positional))
        {
            if (TrueNorthPvE.CanUse(out act, usedUp: true, onLastAbility: true)) return true;
        }

        IBaseAction.TargetOverride = TargetType.Heal;
        IBaseAction.ShouldEndSpecial = false;

        if (DataCenter.CommandStatus.HasFlag(AutoStatus.HealAreaAbility))
        {
            IBaseAction.AllEmpty = true;
            if (HealAreaAbility(out act)) return true;
            IBaseAction.AllEmpty = false;
        }
        if (DataCenter.AutoStatus.HasFlag(AutoStatus.HealAreaAbility)
            && CanHealAreaAbility)
        {
            IBaseAction.AutoHealCheck = true;
            if (HealAreaAbility(out act)) return true;
            IBaseAction.AutoHealCheck = false;
        }
        if (DataCenter.CommandStatus.HasFlag(AutoStatus.HealSingleAbility))
        {
            IBaseAction.AllEmpty = true;
            if (HealSingleAbility(out act)) return true;
            IBaseAction.AllEmpty = false;
        }
        if (DataCenter.AutoStatus.HasFlag(AutoStatus.HealSingleAbility)
            && CanHealSingleAbility)
        {
            IBaseAction.AutoHealCheck = true;
            if (HealSingleAbility(out act)) return true;
            IBaseAction.AutoHealCheck = false;
        }

        IBaseAction.TargetOverride = null;
        IBaseAction.ShouldEndSpecial = true;

        if (DataCenter.CommandStatus.HasFlag(AutoStatus.Speed)
            && SpeedAbility(out act)) return true;

        if (DataCenter.MergedStatus.HasFlag(AutoStatus.Provoke))
        {
            if (!HasTankStance && (TankStance?.CanUse(out act) ?? false)) return true;

            IBaseAction.TargetOverride = TargetType.Provoke;

            if (ProvokePvE.CanUse(out act)) return true;
            if (ProvokeAbility(out act)) return true;
        }

        IBaseAction.TargetOverride = TargetType.BeAttacked;

        if (DataCenter.MergedStatus.HasFlag(AutoStatus.DefenseArea))
        {
            if (DefenseAreaAbility(out act)) return true;
            if (role is JobRole.Melee or JobRole.RangedPhysical or JobRole.RangedMagical)
            {
                if (DefenseSingleAbility(out act)) return true;
            }
        }

        if (DataCenter.MergedStatus.HasFlag(AutoStatus.DefenseSingle))
        {
            if (DefenseSingleAbility(out act)) return true;
            if (!DataCenter.IsHostileCastingToTank
                && ArmsLengthPvE.CanUse(out act)) return true;
        }

        IBaseAction.TargetOverride = null;

        IBaseAction.AllEmpty = true;
        if (DataCenter.MergedStatus.HasFlag(AutoStatus.MoveForward)
            && Player != null
            && !Player.HasStatus(true, StatusID.Bind)
            && MoveForwardAbility(out act)) return true;

        if (DataCenter.MergedStatus.HasFlag(AutoStatus.MoveBack)
                && MoveBackAbility(out act)) return true;
        IBaseAction.AllEmpty = false;

        if (DataCenter.MergedStatus.HasFlag(AutoStatus.HealSingleAbility))
        {
            if (UseHpPotion(out act)) return true;
        }
        IBaseAction.ShouldEndSpecial = false;

        if (GeneralUsingAbility(role, out act)) return true;

        if (HasHostilesInRange && AttackAbility(out act)) return true;
        if (GeneralAbility(out act)) return true;

        if (UseMpPotion(out act)) return true;

        //Run!
        if (DataCenter.AutoStatus.HasFlag(AutoStatus.Speed))
        {
            if (SpeedAbility(out act)) return true;
        }

        return false;
    }

    private bool MyInterruptAbility(JobRole role, out IAction? act)
    {
        switch (role)
        {
            case JobRole.Tank:
                if (InterjectPvE.CanUse(out act)) return true;
                break;

            case JobRole.Melee:
                if (LegSweepPvE.CanUse(out act)) return true;
                break;

            case JobRole.RangedPhysical:
                if (HeadGrazePvE.CanUse(out act)) return true;
                break;
        }
        return InterruptAbility(out act);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="act"></param>
    /// <returns></returns>
    protected virtual bool InterruptAbility(out IAction? act)
    {
        if (DataCenter.RightNowDutyRotation?.InterruptAbility(out act) ?? false) return true;
        act = null; return false;
    }

    private bool AntiKnockback(JobRole role, out IAction? act)
    {
        switch (role)
        {
            case JobRole.Tank:
            case JobRole.Melee:
                if (ArmsLengthPvE.CanUse(out act)) return true;
                break;
            case JobRole.Healer:
            case JobRole.RangedMagical:
                if (SurecastPvE.CanUse(out act)) return true;
                break;
            case JobRole.RangedPhysical:
                if (ArmsLengthPvE.CanUse(out act)) return true;
                break;
        }

        return AntiKnockbackAbility(out act);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="act"></param>
    /// <returns></returns>
    protected virtual bool AntiKnockbackAbility(out IAction? act)
    {
        if (DataCenter.RightNowDutyRotation?.AntiKnockbackAbility(out act) ?? false) return true;
        act = null; return false;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="act"></param>
    /// <returns></returns>
    protected virtual bool ProvokeAbility(out IAction? act)
    {
        if (DataCenter.RightNowDutyRotation?.ProvokeAbility(out act) ?? false) return true;
        act = null; return false;
    }


    private bool GeneralUsingAbility(JobRole role, out IAction? act)
    {
        act = null;
        switch (role)
        {
            case JobRole.Tank:
                if (LowBlowPvE.CanUse(out act)) return true;
                break;

            case JobRole.Melee:
                if (SecondWindPvE.CanUse(out act)) return true;
                if (BloodbathPvE.CanUse(out act)) return true;
                break;

            case JobRole.Healer:
            case JobRole.RangedMagical:
                if (Job == ECommons.ExcelServices.Job.BLM) break;
                if (LucidDreamingPvE.CanUse(out act)) return true;
                break;

            case JobRole.RangedPhysical:
                if (SecondWindPvE.CanUse(out act)) return true;
                break;
        }
        return false;
    }


    /// <summary>
    /// It got the highest priority among abilities. 
    /// </summary>
    /// <param name="nextGCD">The next gcd action.</param>
    /// <param name="act">Result action.</param>
    //<returns>Can we use it.</returns>
    protected virtual bool EmergencyAbility(IAction nextGCD, out IAction? act)
    {
        if (nextGCD is BaseAction action)
        {
            if (Role is JobRole.Healer or JobRole.RangedMagical &&
            action.Info.CastTime >= 5 && SwiftcastPvE.CanUse(out act)) return true;
        }

        if (DataCenter.RightNowDutyRotation?.EmergencyAbility(nextGCD, out act) ?? false) return true;

        #region PvP
        if (GuardPvP.CanUse(out act)
            && (Player.GetHealthRatio() <= Service.Config.HealthForGuard
            || DataCenter.CommandStatus.HasFlag(AutoStatus.Raise | AutoStatus.Shirk))) return true;
        #endregion

        return false;
    }

    /// <summary>
    /// The ability that makes character moving forward.
    /// </summary>
    /// <param name="act">Result action.</param>
    /// <returns>Can we use it.</returns>
    [RotationDesc(DescType.MoveForwardAbility)]
    protected virtual bool MoveForwardAbility(out IAction? act)
    {
        if (DataCenter.RightNowDutyRotation?.MoveForwardAbility(out act) ?? false) return true;
        act = null; return false;
    }

    /// <summary>
    /// The ability that makes character moving Back.
    /// </summary>
    /// <param name="act">Result action.</param>
    /// <returns>Can we use it.</returns>
    [RotationDesc(DescType.MoveBackAbility)]
    protected virtual bool MoveBackAbility(out IAction? act)
    {
        if (DataCenter.RightNowDutyRotation?.MoveBackAbility(out act) ?? false) return true;
        act = null; return false;
    }

    /// <summary>
    /// The ability that heals single character.
    /// </summary>
    /// <param name="act">Result action.</param>
    /// <returns>Can we use it.</returns>
    [RotationDesc(DescType.HealSingleAbility)]
    protected virtual bool HealSingleAbility(out IAction? act)
    {
        if (RecuperatePvP.CanUse(out act)) return true;
        if (DataCenter.RightNowDutyRotation?.HealSingleAbility(out act) ?? false) return true;
        act = null; return false;
    }

    /// <summary>
    /// The ability that heals area.
    /// </summary>
    /// <param name="act">Result action.</param>
    /// <returns>Can we use it.</returns>
    [RotationDesc(DescType.HealAreaAbility)]
    protected virtual bool HealAreaAbility(out IAction? act)
    {
        if (DataCenter.RightNowDutyRotation?.HealAreaAbility(out act) ?? false) return true;
        act = null; return false;
    }

    /// <summary>
    /// The ability that defenses single character.
    /// </summary>
    /// <param name="act">Result action.</param>
    /// <returns>Can we use it.</returns>
    [RotationDesc(DescType.DefenseSingleAbility)]
    protected virtual bool DefenseSingleAbility(out IAction? act)
    {
        if (DataCenter.RightNowDutyRotation?.DefenseSingleAbility(out act) ?? false) return true;
        act = null; return false;
    }

    /// <summary>
    /// The ability that defense area.
    /// </summary>
    /// <param name="act">Result action.</param>
    /// <returns>Can we use it.</returns>

    [RotationDesc(DescType.DefenseAreaAbility)]
    protected virtual bool DefenseAreaAbility(out IAction? act)
    {
        if (DataCenter.RightNowDutyRotation?.DefenseAreaAbility(out act) ?? false) return true;
        act = null; return false;
    }

    /// <summary>
    /// The ability that speeds your character up.
    /// </summary>
    /// <param name="act">Result action.</param>
    /// <returns>Can we use it.</returns>

    [RotationDesc(DescType.SpeedAbility)]
    [RotationDesc(ActionID.SprintPvE)]
    protected virtual bool SpeedAbility(out IAction? act)
    {
        if (SprintPvP.CanUse(out act)) return true;

        if (PelotonPvE.CanUse(out act, skipAoeCheck: true)) return true;
        if (SprintPvE.CanUse(out act)) return true;

        if (DataCenter.RightNowDutyRotation?.SpeedAbility(out act) ?? false) return true;
        return false;
    }

    /// <summary>
    /// The ability that can be done anywhere.
    /// </summary>
    /// <param name="act">Result action.</param>
    /// <returns>Can we use it.</returns>

    protected virtual bool GeneralAbility(out IAction? act)
    {
        if (DataCenter.RightNowDutyRotation?.GeneralAbility(out act) ?? false) return true;
        act = null; return false;
    }

    /// <summary>
    /// The ability that attacks some enemy.
    /// </summary>
    /// <param name="act">Result action.</param>
    /// <returns>Can we use it.</returns>
    protected virtual bool AttackAbility(out IAction? act)
    {
        if (DataCenter.RightNowDutyRotation?.AttackAbility(out act) ?? false) return true;
        act = null; return false;
    }
}
