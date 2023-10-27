namespace RotationSolver.Basic.Configuration.Conditions;

internal class ConditionSet : DelayCondition
{
    public List<ICondition> Conditions { get; set; } = new List<ICondition>();

    public LogicalType Type;

    protected override bool IsTrueInside(ICustomRotation rotation)
    {
        if (Conditions.Count == 0) return false;

        return Type switch
        {
            LogicalType.And => Conditions.All(c => c.IsTrue(rotation)),
            LogicalType.Or => Conditions.Any(c => c.IsTrue(rotation)),
            LogicalType.NotAnd => !Conditions.All(c => c.IsTrue(rotation)),
            LogicalType.NotOr => !Conditions.Any(c => c.IsTrue(rotation)),
            _ => false,
        };
    }
}

internal enum LogicalType : byte
{
    And,
    Or,
    NotAnd,
    NotOr,
}
