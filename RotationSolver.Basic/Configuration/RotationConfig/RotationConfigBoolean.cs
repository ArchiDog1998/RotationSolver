namespace RotationSolver.Basic.Configuration.RotationConfig;

internal class RotationConfigBoolean : RotationConfigBase
{
    public RotationConfigBoolean(string name, bool value, string displayName) : base(name, value.ToString(), displayName)
    {
    }

    public override bool DoCommand(IRotationConfigSet set, string str)
    {
        if (!base.DoCommand(set, str)) return false;

        string numStr = str[Name.Length..].Trim();

        if (bool.TryParse(numStr, out _))
        {
            set.SetValue(Name, numStr.ToString());
        }
        else
        {
            set.SetValue(Name, (!set.GetBool(Name)).ToString());
        }
        return true;
    }
}
