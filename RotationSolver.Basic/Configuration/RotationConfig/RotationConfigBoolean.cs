namespace RotationSolver.Basic.Configuration.RotationConfig;

internal class RotationConfigBoolean(ICustomRotation rotation, PropertyInfo property)
    : RotationConfigBase(rotation, property)
{
    public override bool DoCommand(IRotationConfigSet set, string str)
    {
        if (!base.DoCommand(set, str)) return false;

        string numStr = str[Name.Length..].Trim();

        if (bool.TryParse(numStr, out _))
        {
            Value = numStr.ToString();
        }
        else
        {
            Value = (!bool.Parse(Value)).ToString();
        }
        return true;
    }
}
