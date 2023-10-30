namespace RotationSolver.Basic.Configuration.Conditions;

internal class TerritoryCondition : DelayCondition
{
    public TerritoryConditionType TerritoryConditionType = TerritoryConditionType.TerritoryContentType;

    public int Param1 = 0;
    public string Name = "Not Chosen";
    public int Condition;

    protected override bool IsTrueInside(ICustomRotation rotation)
    {
        bool result = TerritoryConditionType switch
        {
            TerritoryConditionType.TerritoryContentType => (int)DataCenter.TerritoryContentType == Param1,
            TerritoryConditionType.DutyName => Name == DataCenter.ContentFinderName,
            TerritoryConditionType.TerritoryName => Name == DataCenter.TerritoryName,
            _ => false,
        };
        return Condition > 0 ? !result : result;
    }
}

internal enum TerritoryConditionType : byte
{
    TerritoryContentType,
    TerritoryName,
    DutyName,
}
