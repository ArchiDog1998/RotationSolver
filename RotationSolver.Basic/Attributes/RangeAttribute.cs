namespace RotationSolver.Basic.Attributes;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class RangeAttribute(double minValue, double maxValue, ConfigUnitType unitType) : Attribute
{
    public double MinValue => minValue;
    public double MaxValue => maxValue;
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