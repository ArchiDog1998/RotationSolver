namespace RotationSolver.Basic.Configuration.RotationConfig;

internal class RotationConfigString(ICustomRotation rotation, PropertyInfo property)
    : RotationConfigBase(rotation, property)
{
    public override bool DoCommand(IRotationConfigSet set, string str)
    {
        if (!base.DoCommand(set, str)) return false;

        Value = str[Name.Length..].Trim();

        return true;
    }
}
