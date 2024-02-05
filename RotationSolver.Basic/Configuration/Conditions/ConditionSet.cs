namespace RotationSolver.Basic.Configuration.Conditions;

[Description("Condition Set")]
internal class ConditionSet : DelayCondition
{
    public List<ICondition> Conditions { get; set; } = [];

    public LogicalType Type;

    protected override bool IsTrueInside(ICustomRotation rotation)
    {
        if (Conditions.Count == 0) return false;

        return Type switch
        {
            LogicalType.And => Conditions.All(c => c.IsTrue(rotation)),
            LogicalType.Or => Conditions.Any(c => c.IsTrue(rotation)),
            _ => false,
        };
    }
}

internal enum LogicalType : byte
{
    [Description("&&")]
    And,

    [Description(" | | ")]
    Or,
}
