namespace RotationSolver.Basic.Configuration.Conditions;

internal class NamedCondition : DelayCondition
{
    public string ConditionName = "Not Chosen";
    protected override bool IsTrueInside(ICustomRotation rotation)
    {
        foreach (var pair in DataCenter.RightSet.NamedConditions)
        {
            if (pair.Name != ConditionName) continue;

            return pair.Condition.IsTrue(rotation);
        }
        return false;
    }
}
