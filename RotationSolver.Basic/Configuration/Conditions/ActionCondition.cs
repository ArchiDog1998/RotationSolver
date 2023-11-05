namespace RotationSolver.Basic.Configuration.Conditions;

internal class ActionCondition : DelayCondition
{
    internal IBaseAction _action;

    public ActionID ID { get; set; } = ActionID.None;

    public ActionConditionType ActionConditionType = ActionConditionType.Elapsed;

    public int Param1;
    public int Param2;
    public float Time;

    public override bool CheckBefore(ICustomRotation rotation)
    {
        return CheckBaseAction(rotation, ID, ref _action) && base.CheckBefore(rotation);
    }

    protected override bool IsTrueInside(ICustomRotation rotation)
    {
        switch (ActionConditionType)
        {
            case ActionConditionType.Elapsed:
                return _action.ElapsedOneChargeAfter(Time); // Bigger

            case ActionConditionType.ElapsedGCD:
                return _action.ElapsedOneChargeAfterGCD((uint)Param1, Param2); // Bigger

            case ActionConditionType.Remain:
                return !_action.WillHaveOneCharge(Time); //Smaller

            case ActionConditionType.RemainGCD:
                return !_action.WillHaveOneChargeGCD((uint)Param1, Param2); // Smaller

            case ActionConditionType.CanUse:
                return _action.CanUse(out _, (CanUseOption)Param1, (byte)Param2);

            case ActionConditionType.EnoughLevel:
                return _action.EnoughLevel;

            case ActionConditionType.IsCoolDown:
                return _action.IsCoolingDown;

            case ActionConditionType.CurrentCharges:
                return _action.CurrentCharges > Param1;

            case ActionConditionType.MaxCharges:
                return _action.MaxCharges > Param1;
        }
        return false;
    }
}

internal enum ActionConditionType : byte
{
    Elapsed,
    ElapsedGCD,
    Remain,
    RemainGCD,
    CanUse,
    EnoughLevel,
    IsCoolDown,
    CurrentCharges,
    MaxCharges,
}
