namespace RotationSolver.Basic.Configuration.RotationConfig;

internal class RotationConfigFloat : RotationConfigBase
{
    public float Min, Max, Speed;

    public ConfigUnitType UnitType { get; set; }

    public RotationConfigFloat(ICustomRotation rotation, PropertyInfo property)
        : base(rotation, property)
    {
        var attr = property.GetCustomAttribute<RangeAttribute>();
        if(attr != null)
        {
            Min = attr.MinValue;
            Max = attr.MaxValue;
            Speed = attr.Speed;
            UnitType = attr.UnitType;
        }
        else
        {
            Min = 0.0f;
            Max = 1.0f;
            Speed = 0.005f;
            UnitType = ConfigUnitType.Percent;
        }
    }

    public override bool DoCommand(IRotationConfigSet set, string str)
    {
        if (!base.DoCommand(set, str)) return false;

        string numStr = str[Name.Length..].Trim();

        if (float.TryParse(numStr, out _))
        {
            Value = numStr.ToString();
        }
        return true;
    }
}
