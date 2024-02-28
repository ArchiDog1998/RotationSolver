namespace RotationSolver.Basic.Rotations.Duties;
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

partial class DutyRotation
{
    #region GCD
    public virtual bool EmergencyGCD(out IAction? act)
    {
        act = null; return false;
    }

    public virtual bool GeneralGCD(out IAction? act)
    {
        act = null; return false;
    }

    public virtual bool RaiseGCD(out IAction? act)
    {
        act = null; return false;
    }

    public virtual bool DispelGCD(out IAction? act)
    {
        act = null; return false;
    }

    public virtual bool MoveForwardGCD(out IAction? act)
    {
        act = null; return false;
    }

    public virtual bool HealSingleGCD(out IAction? act)
    {
        act = null; return false;
    }

    public virtual bool HealAreaGCD(out IAction? act)
    {
        act = null; return false;
    }

    public virtual bool DefenseSingleGCD(out IAction? act)
    {
        act = null; return false;
    }

    public virtual bool DefenseAreaGCD(out IAction? act)
    {
        act = null; return false;
    }
    #endregion

    #region Ability
    public virtual bool InterruptAbility(out IAction? act)
    {
        act = null; return false;
    }

    public virtual bool AntiKnockbackAbility(out IAction? act)
    {
        act = null; return false;
    }


    public virtual bool ProvokeAbility(out IAction? act)
    {
        act = null; return false;
    }

    public virtual bool EmergencyAbility(IAction nextGCD, out IAction? act)
    {
        act = null; return false;
    }

    public virtual bool MoveForwardAbility(out IAction? act)
    {
        act = null; return false;
    }

    public virtual bool MoveBackAbility(out IAction? act)
    {
        act = null; return false;
    }

    public virtual bool HealSingleAbility(out IAction? act)
    {
        act = null; return false;
    }

    public virtual bool HealAreaAbility(out IAction? act)
    {
        act = null; return false;
    }

    public virtual bool DefenseSingleAbility(out IAction? act)
    {
        act = null; return false;
    }

    public virtual bool DefenseAreaAbility(out IAction? act)
    {
        act = null; return false;
    }

    public virtual bool SpeedAbility(out IAction? act)
    {
        act = null; return false;
    }

    public virtual bool GeneralAbility(out IAction? act)
    {
        act = null; return false;
    }

    public virtual bool AttackAbility(out IAction? act)
    {
        act = null; return false;
    }
    #endregion

    internal IAction[] AllActions
    {
        get
        {
            var properties = this.GetType().GetAllPropertyInfo()
                .Where(p => DataCenter.DutyActions.Contains(p.GetCustomAttribute<IDAttribute>()?.ID ?? 0));

            return [.. properties.Select(p => (IAction)p.GetValue(this)!)];
        }
    }
}
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
