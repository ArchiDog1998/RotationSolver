using XIVConfigUI;

namespace RotationSolver.Basic.Configuration.RotationConfig;

internal class RotationConfigInt : RotationConfigBase
{
    public int Min, Max, Speed;

    public RotationConfigInt(ICustomRotation rotation, PropertyInfo property)
        : base(rotation, property)
    {
        var attr = property.GetCustomAttribute<RangeAttribute>();
        if (attr != null)
        {
            Min = (int)attr.MinValue;
            Max = (int)attr.MaxValue;
            Speed = (int)attr.Speed;
        }
        else
        {
            Min = 0;
            Max = 10;
            Speed = 1;
        }
    }

    public override bool DoCommand(IRotationConfigSet set, string str)
    {
        if (!base.DoCommand(set, str)) return false;

        string numStr = str[Name.Length..].Trim();

        if (int.TryParse(numStr, out _))
        {
            Value = numStr.ToString();
        }
        return true;
    }
}
