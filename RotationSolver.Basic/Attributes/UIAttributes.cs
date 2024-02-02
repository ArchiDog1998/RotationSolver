namespace RotationSolver.Basic.Attributes;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class UIAttribute(string name) : Attribute
{
    public string Name => name;
    public string Description { get; set; } = "";
    public string Parent { get; set; } = "";
    public string Filter { get; set; } = "";
    public byte Order { get; set; } = 0;
    public byte Section { get; set; } = 0;

    public JobFilterType PvPFilter { get; set; }
    public JobFilterType PvEFilter { get; set; }
}

public enum JobFilterType : byte
{
    NoJob,
    NoHealer,
    Healer,
    Raise,
    Interrupt,
    Esuna,
    Tank, 
    Melee,
}

[AttributeUsage(AttributeTargets.Field)]
internal class JobConfigAttribute : Attribute
{
}

[AttributeUsage(AttributeTargets.Field)]
internal class JobChoiceConfigAttribute : Attribute
{
}

[AttributeUsage(AttributeTargets.Field)]
internal class ConditionBoolAttribute: Attribute
{
}