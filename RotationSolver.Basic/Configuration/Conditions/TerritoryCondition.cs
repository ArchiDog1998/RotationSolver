namespace RotationSolver.Basic.Configuration.Conditions;

[Description("Territory Condition")]
internal class TerritoryCondition : DelayCondition
{
    public TerritoryConditionType TerritoryConditionType = TerritoryConditionType.TerritoryContentType;

    public int Position = 0;
    public int Param1 = 0, Param2 = 0;
    public string Name = "Not Chosen";
    public float TimeStart, TimeEnd;

    protected override bool IsTrueInside(ICustomRotation rotation)
    {
        bool result = false;
        switch (TerritoryConditionType)
        {
            case TerritoryConditionType.TerritoryContentType:
                result = (int)DataCenter.TerritoryContentType == Param1;
                break;

            case TerritoryConditionType.DutyName:
                result = Name == DataCenter.ContentFinderName;
                break;

            case TerritoryConditionType.TerritoryName:
                result = Name == DataCenter.TerritoryName;
                break;

            case TerritoryConditionType.MapEffect:
                foreach (var effect in DataCenter.MapEffects.Reverse())
                {
                    var time = effect.TimeDuration.TotalSeconds;
                    if (time > TimeStart && time < TimeEnd
                        && effect.Position == Position
                        && effect.Param1 == Param1
                        && effect.Param2 == Param2)
                    {
                        result = true;
                        break;
                    }
                }

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

    [Description("Map Effect")]
    MapEffect,
}
