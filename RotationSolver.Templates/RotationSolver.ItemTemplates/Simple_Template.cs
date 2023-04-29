namespace RotationSolver.ItemTemplates;

[LinkDescription("$Your link description here, it is better to link to a png! this attribute can be multiple! $")]
[SourceCode("$If your rotation is open source, please write the link to this attribute! $")]

// Change this base class to your job's base class. It is named like XXX_Base.
public class Simple_Template : AST_Base 
{
    //Change this to the game version right now.
    public override string GameVersion => "6.38";

    public override string RotationName => "$safeitemname$";

    //GCD actions here.
    protected override bool GeneralGCD(out IAction act)
    {
        throw new NotImplementedException();
    }

    //0GCD actions here.
    protected override bool AttackAbility(out IAction act)
    {
        throw new NotImplementedException();
    }
}
