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

    [Description("Time Unit, in seconds.")]
    Seconds,

    [Description("Angle Unit, in degrees.")]
    Degree,

    [Description("Distance Unit, in yalms.")]
    Yalms,

    [Description("Ratio Unit, as percentage.")]
    Percent,

    [Description("Display Unit, in pixels.")]
    Pixels,
}