namespace RotationSolver.Basic.Configuration.Conditions;

[Description("Territory Condition")]
internal class TerritoryCondition : DelayCondition
{
    public TerritoryConditionType TerritoryConditionType = TerritoryConditionType.TerritoryContentType;

    public int TerritoryId = 0;
    public string Name = "Not Chosen";

    protected override bool IsTrueInside(ICustomRotation rotation)
    {
        bool result = false;
        switch (TerritoryConditionType)
        {
            case TerritoryConditionType.TerritoryContentType:
                result = (int)DataCenter.TerritoryContentType == TerritoryId;
                break;

            case TerritoryConditionType.DutyName:
                result = Name == DataCenter.ContentFinderName;
                break;

            case TerritoryConditionType.TerritoryName:
                result = Name == DataCenter.TerritoryName;
                break;
        }
        return result;
    }
}

internal enum TerritoryConditionType : byte
{
    [Description("Territory Content Type")]
    TerritoryContentType,

    [Description("Territory Name")]
    TerritoryName,

    [Description("Duty Name")]
    DutyName,
}
