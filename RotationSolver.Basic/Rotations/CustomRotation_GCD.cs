using RotationSolver.Basic.Configuration;

namespace RotationSolver.Basic.Rotations;

public abstract partial class CustomRotation
{
    private static DateTime _nextTimeToHeal = DateTime.MinValue;
    private IAction GCD(bool helpDefenseAOE, bool helpDefenseSingle)
    {
        IAction act = DataCenter.CommandNextAction;

        BaseAction.SkipDisable = true;
        if (act is IBaseAction a && a != null && a.IsRealGCD && a.CanUse(out _, CanUseOption.MustUse | CanUseOption.EmptyOrSkipCombo)) return act;
        BaseAction.SkipDisable = false;

        if (EmergencyGCD(out act)) return act;

        var specialType = DataCenter.SpecialType;

        if (RaiseSpell(specialType, out act, false)) return act;

        if (specialType == SpecialCommandType.MoveForward && MoveForwardGCD(out act))
        {
            if (act is IBaseAction b && ObjectHelper.DistanceToPlayer(b.Target) > 5) return act;
        }
        
        //General Heal
        if ((DataCenter.HPNotFull || ClassJob.GetJobRole() != JobRole.Healer)
            && (DataCenter.InCombat || Service.Config.GetValue(PluginConfigBool.HealOutOfCombat)))
        {
            if (specialType == SpecialCommandType.HealArea)
            {
                if( HealAreaGCD(out act)) return act;
            }
            if (CanHealAreaSpell)
            {
                BaseAction.AutoHealCheck = true;
                if (HealAreaGCD(out act)) return act;
                BaseAction.AutoHealCheck = false;
            }
            if (specialType == SpecialCommandType.HealSingle)
            {
                if (HealSingleGCD(out act)) return act;
            }
            if (CanHealSingleSpell)
            {
                BaseAction.AutoHealCheck = true;
                if (HealSingleGCD(out act)) return act;
                BaseAction.AutoHealCheck = false;
            }
        }
        if (specialType == SpecialCommandType.DefenseArea && DefenseAreaGCD(out act)) return act;
        if (specialType == SpecialCommandType.DefenseSingle && DefenseSingleGCD(out act)) return act;

        //Auto Defense
        if (DataCenter.SetAutoStatus(AutoStatus.DefenseArea, helpDefenseAOE) && DefenseAreaGCD(out act)) return act;
        if (DataCenter.SetAutoStatus(AutoStatus.DefenseSingle, helpDefenseSingle) && DefenseSingleGCD(out act)) return act;

        //Esuna
        if (DataCenter.SetAutoStatus(AutoStatus.Esuna, (specialType == SpecialCommandType.EsunaStanceNorth 
            || !HasHostilesInRange || Service.Config.GetValue(PluginConfigBool.EsunaAll))
            && DataCenter.WeakenPeople.Any()  || DataCenter.DyingPeople.Any()))
        {
            if (ClassJob.GetJobRole() == JobRole.Healer && Esuna.CanUse(out act, CanUseOption.MustUse)) return act;
        }

        if (GeneralGCD(out var action)) return action;

        if (Service.Config.GetValue(PluginConfigBool.HealWhenNothingTodo) && DataCenter.InCombat)
        {
            // Please don't tell me someone's fps is less than 1!!
            if (DateTime.Now - _nextTimeToHeal > TimeSpan.FromSeconds(1))
            {
                var min = Service.Config.GetValue(PluginConfigFloat.HealWhenNothingTodoMin);
                var max = Service.Config.GetValue(PluginConfigFloat.HealWhenNothingTodoMax);
                _nextTimeToHeal = DateTime.Now + TimeSpan.FromSeconds(new Random().NextDouble() * (max - min) + min);
            }
            else if (_nextTimeToHeal < DateTime.Now)
            {
                _nextTimeToHeal = DateTime.Now;

                if (DataCenter.PartyMembersMinHP < Service.Config.GetValue(PluginConfigFloat.HealWhenNothingTodoBelow))
                {
                    if (DataCenter.PartyMembersDifferHP < DataCenter.PartyMembersDifferHP && HealAreaGCD(out act)) return act;
                    if (HealSingleGCD(out act)) return act;
                }
            }
        }
        
        if (Service.Config.GetValue(PluginConfigBool.RaisePlayerByCasting) && RaiseSpell(specialType, out act, true)) return act;

        return null;
    }

    private bool RaiseSpell(SpecialCommandType specialType, out IAction act, bool mustUse)
    {
        act = null;
        if (specialType == SpecialCommandType.RaiseShirk && DataCenter.DeathPeopleAll.Any())
        {
            if (RaiseAction(out act)) return true;
        }

        if ((Service.Config.GetValue(PluginConfigBool.RaiseAll) ? DataCenter.DeathPeopleAll.Any() : DataCenter.DeathPeopleParty.Any())
            && RaiseAction(out act, CanUseOption.IgnoreCastCheck))
        {
            if (HasSwift)
            {
                return DataCenter.SetAutoStatus(AutoStatus.Raise, true);
            }
            else if (mustUse)
            {
                var action = act;
                if(Swiftcast.CanUse(out act))
                {
                    return DataCenter.SetAutoStatus(AutoStatus.Raise, true);
                }
                else if(!IsMoving)
                {
                    act = action;
                    return DataCenter.SetAutoStatus(AutoStatus.Raise, true);
                }
            }
            else if (Service.Config.GetValue(PluginConfigBool.RaisePlayerBySwift) && !Swiftcast.IsCoolingDown 
                && DataCenter.NextAbilityToNextGCD > DataCenter.MinAnimationLock + DataCenter.Ping)
            {
                return DataCenter.SetAutoStatus(AutoStatus.Raise, true);
            }
        }
        return DataCenter.SetAutoStatus(AutoStatus.Raise, false);
    }

    private bool RaiseAction(out IAction act, CanUseOption option = CanUseOption.None)
    {
        if (VariantRaise.CanUse(out act, option)) return true;
        if (Player.CurrentMp > Service.Config.GetValue(PluginConfigInt.LessMPNoRaise) && (Raise?.CanUse(out act, option) ?? false)) return true;

        return false;
    }

    /// <summary>
    /// The emergency gcd with highest priority.
    /// </summary>
    /// <param name="act"></param>
    /// <returns></returns>
    protected virtual bool EmergencyGCD(out IAction act)
    {
        act = null; return false;
    }

    /// <summary>
    /// Moving forward GCD.
    /// </summary>
    /// <param name="act"></param>
    /// <returns></returns>
    [RotationDesc(DescType.MoveForwardGCD)]
    protected virtual bool MoveForwardGCD(out IAction act)
    {
        act = null; return false;
    }

    /// <summary>
    /// Heal single GCD.
    /// </summary>
    /// <param name="act"></param>
    /// <returns></returns>
    [RotationDesc(DescType.HealSingleGCD)]
    protected virtual bool HealSingleGCD(out IAction act)
    {
        if (VariantCure.CanUse(out act)) return true;
        if (VariantCure2.CanUse(out act)) return true;
        return false;
    }

    /// <summary>
    /// Heal area GCD.
    /// </summary>
    /// <param name="act"></param>
    /// <returns></returns>
    [RotationDesc(DescType.HealAreaGCD)]
    protected virtual bool HealAreaGCD(out IAction act)
    {
        act = null; return false;
    }

    /// <summary>
    /// Defense single gcd.
    /// </summary>
    /// <param name="act"></param>
    /// <returns></returns>
    [RotationDesc(DescType.DefenseSingleGCD)]
    protected virtual bool DefenseSingleGCD(out IAction act)
    {
        act = null; return false;
    }

    /// <summary>
    /// Defense area gcd.
    /// </summary>
    /// <param name="act"></param>
    /// <returns></returns>
    [RotationDesc(DescType.DefenseAreaGCD)]
    protected virtual bool DefenseAreaGCD(out IAction act)
    {
        act = null; return false;
    }

    /// <summary>
    /// General GCD.
    /// </summary>
    /// <param name="act"></param>
    /// <returns></returns>
    protected abstract bool GeneralGCD(out IAction act);
}
