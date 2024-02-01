namespace RotationSolver.Basic.Attributes;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class UIAttribute(string name) : Attribute
{
    public string Name => name;
    public string Description { get; set; } = "";
    public string Parent { get; set; } = "";
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
internal class ConditionBoolAttribute: Attribute, iattribute
{
}