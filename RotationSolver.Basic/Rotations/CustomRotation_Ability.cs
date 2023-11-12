using RotationSolver.Basic.Configuration;

namespace RotationSolver.Basic.Rotations;

public abstract partial class CustomRotation
{
    private bool Ability(IAction nextGCD, out IAction act, bool helpDefenseAOE, bool helpDefenseSingle)
    {
        act = DataCenter.CommandNextAction;

        BaseAction.SkipDisable = true;
        if (act is IBaseAction a && a != null && !a.IsRealGCD && a.CanUse(out _,
            CanUseOption.MustUse | CanUseOption.EmptyOrSkipCombo)) return true;
        BaseAction.SkipDisable = false;

        if (act is IBaseItem i && i.CanUse(out _, true)) return true;

        if (!Service.Config.GetValue(PluginConfigBool.UseAbility)
            || Player.TotalCastTime > 0)
        {
            act = null;
            return false;
        }

        if (EmergencyAbility(nextGCD, out act)) return true;
        var role = ClassJob.GetJobRole();

        if (InterruptAbility(role, out act)) return true;
        if (PvP_Purify.CanUse(out act)) return true;

        if (ShirkOrShield(role, out act)) return true;
        if (DataCenter.IsAntiKnockback && AntiKnockback(role, out act)) return true;

        if (DataCenter.IsEsunaStanceNorth && role == JobRole.Melee)
        {
            if (TrueNorth.CanUse(out act)) return true;
        }

        if (GeneralHealAbility(out act)) return true;
        if (DataCenter.IsSpeed && SpeedAbility(out act)) return true;

        if (AutoDefense(role, helpDefenseAOE, helpDefenseSingle, out act)) return true;

        BaseAction.OtherOption |= CanUseOption.EmptyOrSkipCombo;
        if (MovingAbility(out act)) return true;
        BaseAction.OtherOption &= ~CanUseOption.EmptyOrSkipCombo;

        if (GeneralUsingAbility(role, out act)) return true;

        if (DataCenter.HPNotFull && InCombat)
        {
            if (DataCenter.IsHealSingle || CanHealSingleAbility)
            {
                if (UseHealPotion(out act)) return true;
            }
        }

        if (HasHostilesInRange && AttackAbility(out act)) return true;
        if (GeneralAbility(out act)) return true;

        //Run!
        if (IsMoving && NotInCombatDelay && Service.Config.GetValue(PluginConfigBool.AutoSpeedOutOfCombat)
            && SpeedAbility(out act)) return true;

        return false;
    }

    private static bool InterruptAbility(JobRole role, out IAction act)
    {
        act = null;
        if (!DataCenter.SetAutoStatus(AutoStatus.Interrupt, DataCenter.CanInterruptTargets.Any()))
            return false;


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

    private bool ShirkOrShield(JobRole role, out IAction act)
    {
        act = null;
        if (role != JobRole.Tank)
        {
            return DataCenter.SetAutoStatus(AutoStatus.TankStance, false);
        }

        if (DataCenter.IsRaiseShirk && Shirk.CanUse(out act)) return true;
        if (DataCenter.IsEsunaStanceNorth && TankStance.CanUse(out act)) return true;

        if (DataCenter.SetAutoStatus(AutoStatus.TankStance, Service.Config.GetValue(PluginConfigBool.AutoTankStance)
            && !DataCenter.AllianceTanks.Any(t => t.CurrentHp != 0 && t.HasStatus(false, StatusHelper.TankStanceStatus))
            && !HasTankStance && TankStance.CanUse(out act, CanUseOption.IgnoreClippingCheck)))
        {
            return true;
        }

        return false;
    }

    private static bool AntiKnockback(JobRole role, out IAction act)
    {
        act = null;
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

    private bool GeneralHealAbility(out IAction act)
    {
        act = null;

        BaseAction.OtherOption |= CanUseOption.MustUse;
        if (DataCenter.IsDefenseArea && DefenseAreaAbility(out act)) return true;
        if (DataCenter.IsDefenseSingle && DefenseSingleAbility(out act)) return true;

        BaseAction.OtherOption &= ~CanUseOption.MustUse;

        if ((DataCenter.HPNotFull || ClassJob.GetJobRole() != JobRole.Healer) && InCombat)
        {
            if (DataCenter.IsHealArea)
            {
                if (HealAreaAbility(out act)) return true;
            }
            if (CanHealAreaAbility)
            {
                BaseAction.AutoHealCheck = true;
                if (HealAreaAbility(out act)) return true;
                BaseAction.AutoHealCheck = false;
            }
            if (DataCenter.IsHealSingle)
            {
                if (HealSingleAbility(out act)) return true;
            }
            if (CanHealSingleAbility)
            {
                BaseAction.AutoHealCheck = true;
                if (HealSingleAbility(out act)) return true;
                BaseAction.AutoHealCheck = false;
            }
        }
        return false;
    }

    private bool AutoDefense(JobRole role, bool helpDefenseAOE, bool helpDefenseSingle, out IAction act)
    {
        act = null;
        if (!InCombat || !HasHostilesInRange)
        {
            DataCenter.SetAutoStatus(AutoStatus.Provoke, false);
            return false;
        }

        //Auto Provoke
        if (DataCenter.SetAutoStatus(AutoStatus.Provoke, role == JobRole.Tank
            && (Service.Config.GetValue(PluginConfigBool.AutoProvokeForTank) || DataCenter.AllianceTanks.Count() < 2)
            && DataCenter.CanProvoke))
        {
            if (!HasTankStance && TankStance.CanUse(out act)) return true;
            if (Provoke.CanUse(out act, CanUseOption.MustUse)) return true;
            if (VariantUltimatum.CanUse(out act, CanUseOption.MustUse)) return true;
        }

        //No using defense abilities.
        if (!Service.Config.GetValue(PluginConfigBool.UseDefenseAbility)) return false;

        if (helpDefenseAOE)
        {
            if (DefenseAreaAbility(out act)) return true;
            if (role is JobRole.Melee or JobRole.RangedPhysical or JobRole.RangedMagical)
            {
                if (DefenseSingleAbility(out act)) return true;
            }
        }

        //Defense himself.
        if (role == JobRole.Tank)
        {
            var movingHere = (float)NumberOfHostilesInRange / NumberOfHostilesInMaxRange > 0.3f;

            var tarOnMe = DataCenter.TarOnMeTargets.Where(t => t.DistanceToPlayer() <= 3);
            var tarOnMeCount = tarOnMe.Count();
            var attackedCount = tarOnMe.Count(ObjectHelper.IsAttacked);
            var attacked = (float)attackedCount / tarOnMeCount > 0.7f;

            //A lot targets are targeting on me.
            if (tarOnMeCount >= Service.Config.GetValue(PluginConfigInt.AutoDefenseNumber)
                && Player.GetHealthRatio() <= Service.Config.GetValue(DataCenter.Job, JobConfigFloat.HealthForAutoDefense)
                && movingHere && attacked)
            {
                if (DefenseSingleAbility(out act)) return true;
                if (ArmsLength.CanUse(out act)) return true;
            }

            //Big damage casting action.
            if (DataCenter.IsHostileCastingToTank)
            {
                if (DefenseSingleAbility(out act)) return true;
            }
        }

        if (helpDefenseSingle && DefenseSingleAbility(out act)) return true;

        return false;
    }

    private bool MovingAbility(out IAction act)
    {
        act = null;
        if (DataCenter.IsMoveForward && MoveForwardAbility(out act)) return true;
        else if (DataCenter.IsMoveBack && MoveBackAbility(out act)) return true;
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
                if (Jobs[0] == ECommons.ExcelServices.Job.BLM) break;
                if (LucidDreaming.CanUse(out act)) return true;
                break;

            case JobRole.RangedPhysical:
                if (SecondWind.CanUse(out act)) return true;
                break;
        }
        return false;
    }


    /// <summary>
    /// It got the highest priority among abilities. 
    /// </summary>
    /// <param name="nextGCD">The next gcd action.</param>
    /// <param name="act">Result action.</param>
    /// <returns>Can we use it.</returns>
    protected virtual bool EmergencyAbility(IAction nextGCD, out IAction act)
    {
        if (nextGCD is BaseAction action)
        {
            if (ClassJob.GetJobRole() is JobRole.Healer or JobRole.RangedMagical &&
            action.CastTime >= 5 && Swiftcast.CanUse(out act, CanUseOption.EmptyOrSkipCombo)) return true;

            if (Service.Config.GetValue(PluginConfigBool.AutoUseTrueNorth)
                && action.EnemyPositional != EnemyPositional.None && action.Target != null)
            {
                if (action.EnemyPositional != action.Target.FindEnemyPositional() && action.Target.HasPositional())
                {
                    if (TrueNorth.CanUse(out act, CanUseOption.EmptyOrSkipCombo | CanUseOption.OnLastAbility)) return true;
                }
            }
        }

        #region PvP
        if (PvP_Guard.CanUse(out act)
            && (Player.GetHealthRatio() <= Service.Config.GetValue(PluginConfigFloat.HealthForGuard)
            || IsRaiseShirk)) return true;
        #endregion

        return false;
    }

    /// <summary>
    /// The ability that makes character moving forward.
    /// </summary>
    /// <param name="act">Result action.</param>
    /// <returns>Can we use it.</returns>
    [RotationDesc(DescType.MoveForwardAbility)]
    protected virtual bool MoveForwardAbility(out IAction act)
    {
        act = null; return false;
    }

    /// <summary>
    /// The ability that makes character moving Back.
    /// </summary>
    /// <param name="act">Result action.</param>
    /// <returns>Can we use it.</returns>
    [RotationDesc(DescType.MoveBackAbility)]
    protected virtual bool MoveBackAbility(out IAction act)
    {
        act = null; return false;
    }

    /// <summary>
    /// The ability that heals single character.
    /// </summary>
    /// <param name="act">Result action.</param>
    /// <returns>Can we use it.</returns>
    [RotationDesc(DescType.HealSingleAbility)]
    protected virtual bool HealSingleAbility(out IAction act)
    {
        if (PvP_Recuperate.CanUse(out act)) return true;
        act = null; return false;
    }

    /// <summary>
    /// The ability that heals area.
    /// </summary>
    /// <param name="act">Result action.</param>
    /// <returns>Can we use it.</returns>
    [RotationDesc(DescType.HealAreaAbility)]
    protected virtual bool HealAreaAbility(out IAction act)
    {
        act = null; return false;
    }

    /// <summary>
    /// The ability that defenses single character.
    /// </summary>
    /// <param name="act">Result action.</param>
    /// <returns>Can we use it.</returns>
    [RotationDesc(DescType.DefenseSingleAbility)]
    protected virtual bool DefenseSingleAbility(out IAction act)
    {
        act = null; return false;
    }

    /// <summary>
    /// The ability that defense area.
    /// </summary>
    /// <param name="act">Result action.</param>
    /// <returns>Can we use it.</returns>

    [RotationDesc(DescType.DefenseAreaAbility)]
    protected virtual bool DefenseAreaAbility(out IAction act)
    {
        act = null; return false;
    }

    /// <summary>
    /// The ability that speeds your character up.
    /// </summary>
    /// <param name="act">Result action.</param>
    /// <returns>Can we use it.</returns>

    [RotationDesc(DescType.SpeedAbility)]
    [RotationDesc(ActionID.Sprint)]
    protected virtual bool SpeedAbility(out IAction act)
    {
        if (PvP_Sprint.CanUse(out act)) return true;

        if (Peloton.CanUse(out act, CanUseOption.MustUse)) return true;
        if (Sprint.CanUse(out act, CanUseOption.MustUse)) return true;
        return false;
    }

    /// <summary>
    /// The ability that can be done anywhere.
    /// </summary>
    /// <param name="act">Result action.</param>
    /// <returns>Can we use it.</returns>

    protected virtual bool GeneralAbility(out IAction act)
    {
        act = null; return false;
    }

    /// <summary>
    /// The ability that attacks some enemy.
    /// </summary>
    /// <param name="act">Result action.</param>
    /// <returns>Can we use it.</returns>
    protected virtual bool AttackAbility(out IAction act)
    {
        if (VariantSpiritDart.CanUse(out act, CanUseOption.MustUse)) return true;
        if (VariantSpiritDart2.CanUse(out act, CanUseOption.MustUse)) return true;
        if (VariantRampart.CanUse(out act)) return true;
        if (VariantRampart2.CanUse(out act)) return true;
        return false;
    }
}
