namespace RotationSolver.Basic.Configuration.RotationConfig;

internal class RotationConfigFloat : RotationConfigBase
{
    public float Min, Max, Speed;

    public ConfigUnitType UnitType { get; set; }


    [Obsolete("Please use the one with unit type!", true)]
    public RotationConfigFloat(string name, float value, string displayName, float min, float max, float speed)
        :this(name, value, displayName, min, max, speed, ConfigUnitType.None)
    {
        
    }

    public RotationConfigFloat(string name, float value, string displayName, float min, float max, float speed, ConfigUnitType unitType) : base(name, value.ToString(), displayName)
    {
        Min = min;
        Max = max;
        Speed = speed;
        UnitType = unitType;
    }

    public override bool DoCommand(IRotationConfigSet set, string str)
    {
        if (!base.DoCommand(set, str)) return false;

        string numStr = str[Name.Length..].Trim();

        if (float.TryParse(numStr, out _))
        {
            set.SetValue(Name, numStr.ToString());
        }
        return true;
    }
}
