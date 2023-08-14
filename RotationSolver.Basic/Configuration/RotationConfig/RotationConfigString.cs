namespace RotationSolver.Basic.Configuration.RotationConfig;

internal class RotationConfigString : RotationConfigBase
{
    public RotationConfigString(string name, string value, string displayName) : base(name, value, displayName)
    {
    }

    public override bool DoCommand(IRotationConfigSet set, string str)
    {
        if (!base.DoCommand(set, str)) return false;

        string numStr = str[Name.Length..].Trim();

        set.SetValue(Name, numStr.ToString());

        return true;
    }
}
