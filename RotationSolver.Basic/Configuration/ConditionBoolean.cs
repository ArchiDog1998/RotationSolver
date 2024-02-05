namespace RotationSolver.Basic.Configuration;

internal class ConditionBoolean(bool defaultValue, string key)
{
    private readonly bool _defaultValue = defaultValue;
    public bool Value { get; set; } = defaultValue;
    public bool Enable { get; set; }
    public bool Disable { get; set; }

    [JsonIgnore]
    public string Key => key;

    public void ResetValue()
    {
        Value = _defaultValue;
    }

    public static implicit operator bool(ConditionBoolean condition)
    {
        if (!Service.Config.UseAdditionalConditions) return condition.Value;
        var rotation = DataCenter.RightNowRotation;
        var set = DataCenter.RightSet;
        if (rotation != null)
        {
            if (condition.Enable && set.GetEnableCondition(condition.Key).IsTrue(rotation)) return true;
            if (condition.Disable && set.GetDisableCondition(condition.Key).IsTrue(rotation)) return false;
        }
        return condition.Value;
    }

    public override string ToString() => Value.ToString();
}
