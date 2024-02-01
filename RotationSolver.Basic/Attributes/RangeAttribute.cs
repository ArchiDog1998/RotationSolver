namespace RotationSolver.Basic.Attributes;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class RangeAttribute(float minValue, float maxValue, ConfigUnitType unitType, float speed = 0.005f) : Attribute
{
    public float MinValue => minValue;
    public float MaxValue => maxValue;
    public float Speed => speed;
    public ConfigUnitType UnitType => unitType;
}

public enum ConfigUnitType : byte
{
    None,
    Seconds,
    Degree,
    Yalms,
    Percent,
    Pixels,
}