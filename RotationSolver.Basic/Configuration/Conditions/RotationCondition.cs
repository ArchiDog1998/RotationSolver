namespace RotationSolver.Basic.Configuration.Conditions;

[Description("Rotation Condition")]
internal class RotationCondition : DelayCondition
{
    public ComboConditionType ComboConditionType = ComboConditionType.Float;
    internal PropertyInfo? _prop;
    public string PropertyName = "Not Chosen";

    MethodInfo? _method;
    public string MethodName = "Not Chosen";

    internal IBaseAction? _action;
    public ActionID ID { get; set; } = ActionID.None;

    public int Condition;

    public int Param1;
    public float Param2;

    public override bool CheckBefore(ICustomRotation rotation)
    {
        CheckBaseAction(rotation, ID, ref _action);
        CheckMemberInfo(rotation, ref PropertyName, ref _prop);
        CheckMemberInfo(rotation, ref MethodName, ref _method);
        return base.CheckBefore(rotation);
    }

    protected override bool IsTrueInside(ICustomRotation rotation)
    {
        switch (ComboConditionType)
        {
            case ComboConditionType.Bool:
                if (_prop == null) return false;
                if (_prop.GetValue(rotation) is bool b)
                {
                    return b;
                }
                return false;

            case ComboConditionType.Integer:
                if (_prop == null) return false;

                var value = _prop.GetValue(rotation);
                if (value is byte by)
                {
                    switch (Condition)
                    {
                        case 0:
                            return by > Param1;
                        case 1:
                            return by < Param1;
                        case 2:
                            return by == Param1;
                    }
                }
                else if (value is int i)
                {
                    switch (Condition)
                    {
                        case 0:
                            return i > Param1;
                        case 1:
                            return i < Param1;
                        case 2:
                            return i == Param1;
                    }
                }
                return false;

            case ComboConditionType.Float:
                if (_prop == null) return false;
                if (_prop.GetValue(rotation) is float fl)
                {
                    switch (Condition)
                    {
                        case 0:
                            return fl > Param2;
                        case 1:
                            return fl < Param2;
                        case 2:
                            return fl == Param2;
                    }
                }
                return false;

            case ComboConditionType.Last:
                try
                {
                    if (_method?.Invoke(rotation, new object[] { Param1 > 0, new IAction?[] { _action } }) is bool boo)
                    {
                        return boo;
                    }
                    return false;
                }
                catch
                {
                    return false;
                }
        }

        return false;
    }
}

internal enum ComboConditionType : byte
{
    [Description("Boolean")]
    Bool,

    [Description("Byte")]
    Integer,

    [Description("Float")]
    Float,

    [Description("Last")]
    Last,
}
