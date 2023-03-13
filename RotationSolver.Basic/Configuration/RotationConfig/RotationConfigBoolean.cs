namespace RotationSolver.Basic.Configuration.RotationConfig;

public class RotationConfigBoolean : RotationConfigBase
{
    public RotationConfigBoolean(string name, bool value, string displayName) : base(name, value.ToString(), displayName)
    {
    }

    public override bool DoCommand(IRotationConfigSet set, string str)
    {
        if (!base.DoCommand(set, str)) return false;
        set.SetValue(Name, (!set.GetBool(Name)).ToString());
        return true;
    }
}
