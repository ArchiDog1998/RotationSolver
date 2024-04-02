namespace RotationSolver.Basic.Configuration.Conditions;

[Description("Named Condition")]
internal class NamedCondition : DelayCondition
{
    public string ConditionName = "Not Chosen";
    protected override bool IsTrueInside(ICustomRotation rotation)
    {
        foreach (var (Name, Condition) in DataCenter.RightSet.NamedConditions)
        {
            if (Name != ConditionName) continue;

            return Condition.IsTrue(rotation) ?? false;
        }
        return false;
    }
}
