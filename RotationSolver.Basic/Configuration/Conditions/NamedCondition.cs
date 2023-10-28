namespace RotationSolver.Basic.Configuration.Conditions;

internal class NamedCondition : DelayCondition
{
    public string ConditionName = "Not Chosen";
    public int Condition;
    protected override bool IsTrueInside(ICustomRotation rotation)
    {
        foreach (var pair in DataCenter.RightSet.NamedConditions)
        {
            if (pair.Name != ConditionName) continue;

            var result = pair.Condition.IsTrue(rotation);
            return Condition > 0 ? !result : result;
        }
        return false;
    }
}
