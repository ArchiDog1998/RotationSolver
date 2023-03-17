namespace RotationSolver.ItemTemplates;

[RotationDesc("Burst Description here", ActionID.None /* Some actions you used in burst. */)]
[LinkDescription("$Your link description here, it is better to link to a png! this attribute can be multiple! $")]
[SourceCode("$If your rotation is open source, please write the link to this attribute! $")]

// Change this base class to your job's base class. It is named like XXX_Base.
internal class Full_Template : AST_Base
{
    //Change this to the game version right now.
    public override string GameVersion => "6.35";

    public override string RotationName => "$safeitemname$";

    public override string Description => "Your description about this rotation.";

    #region If you want to change the auto healing, please change these bools.
    protected override bool CanHealAreaSpell => base.CanHealAreaSpell;
    protected override bool CanHealAreaAbility => base.CanHealAreaAbility;
    protected override bool CanHealSingleSpell => base.CanHealSingleSpell;
    protected override bool CanHealSingleAbility => base.CanHealSingleAbility;
    #endregion

    #region GCD actions
    protected override bool GeneralGCD(out IAction act)
    {
        throw new NotImplementedException();
    }

    //For some gcds very important, even more than healing, defense, interrupt, etc.
    protected override bool EmergencyGCD(out IAction act)
    {
        return base.EmergencyGCD(out act);
    }

    //For some gcds that moving forward.
    [RotationDesc("Optional description for Moving Forward GCD")]
    [RotationDesc(ActionID.None)]
    protected override bool MoveForwardGCD(out IAction act)
    {
        return base.MoveForwardGCD(out act);
    }

    [RotationDesc("Optional description for Defense Area GCD")]
    [RotationDesc(ActionID.None)]
    protected override bool DefenseAreaGCD(out IAction act)
    {
        return base.DefenseAreaGCD(out act);
    }

    [RotationDesc("Optional description for Defense Single GCD")]
    [RotationDesc(ActionID.None)]
    protected override bool DefenseSingleGCD(out IAction act)
    {
        return base.DefenseSingleGCD(out act);
    }

    [RotationDesc("Optional description for Healing Area GCD")]
    [RotationDesc(ActionID.None)]
    protected override bool HealAreaGCD(out IAction act)
    {
        return base.HealAreaGCD(out act);
    }

    [RotationDesc("Optional description for Healing Single GCD")]
    [RotationDesc(ActionID.None)]
    protected override bool HealSingleGCD(out IAction act)
    {
        return base.HealSingleGCD(out act);
    }
    #endregion

    #region 0GCD actions
    protected override bool AttackAbility(byte abilitiesRemaining, out IAction act)
    {
        throw new NotImplementedException();
    }

    //For some 0gcds very important, even more than healing, defense, interrupt, etc.
    protected override bool EmergencyAbility(byte abilitiesRemaining, IAction nextGCD, out IAction act)
    {
        return base.EmergencyAbility(abilitiesRemaining, nextGCD, out act);
    }

    //Some 0gcds that don't need to a hostile target in attack range.
    protected override bool GeneralAbility(byte abilitiesRemaining, out IAction act)
    {
        return base.GeneralAbility(abilitiesRemaining, out act);
    }

    //Some 0gcds that moving forward. In general, it doesn't need to be override.
    [RotationDesc("Optional description for Moving Forward 0GCD")]
    [RotationDesc(ActionID.None)]
    protected override bool MoveForwardAbility(byte abilitiesRemaining, out IAction act, bool recordTarget = true)
    {
        return base.MoveForwardAbility(abilitiesRemaining, out act, recordTarget);
    }

    //Some 0gcds that moving back. In general, it doesn't need to be override.
    [RotationDesc("Optional description for Moving Back 0GCD")]
    [RotationDesc(ActionID.None)]
    protected override bool MoveBackAbility(byte abilitiesRemaining, out IAction act)
    {
        return base.MoveBackAbility(abilitiesRemaining, out act);
    }

    //Some 0gcds that defense area.
    [RotationDesc("Optional description for Defense Area 0GCD")]
    [RotationDesc(ActionID.None)]
    protected override bool DefenseAreaAbility(byte abilitiesRemaining, out IAction act)
    {
        return base.DefenseAreaAbility(abilitiesRemaining, out act);
    }

    //Some 0gcds that defense single.
    [RotationDesc("Optional description for Defense Single 0GCD")]
    [RotationDesc(ActionID.None)]
    protected override bool DefenseSingleAbility(byte abilitiesRemaining, out IAction act)
    {
        return base.DefenseSingleAbility(abilitiesRemaining, out act);
    }

    //Some 0gcds that healing area.
    [RotationDesc("Optional description for Healing Area 0GCD")]
    [RotationDesc(ActionID.None)]
    protected override bool HealAreaAbility(byte abilitiesRemaining, out IAction act)
    {
        return base.HealAreaAbility(abilitiesRemaining, out act);
    }

    //Some 0gcds that healing single.
    [RotationDesc("Optional description for Healing Single 0GCD")]
    [RotationDesc(ActionID.None)]
    protected override bool HealSingleAbility(byte abilitiesRemaining, out IAction act)
    {
        return base.HealSingleAbility(abilitiesRemaining, out act);
    }
    #endregion
}
