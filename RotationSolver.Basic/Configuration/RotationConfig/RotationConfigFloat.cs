namespace RotationSolver.Basic.Configuration.RotationConfig;

internal class RotationConfigFloat : RotationConfigBase
{
    public float Min, Max, Speed;

    public ConfigUnitType UnitType { get; set; }


    public RotationConfigFloat(string name, float value, string displayName, float min, float max, float speed, ConfigUnitType unitType, CombatType type) : base(name, value.ToString(), displayName, type)
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
