namespace RotationSolver.ActionSequencer;

internal class ActionCondition : DelayCondition
{
    internal IBaseAction _action;

    public ActionID ID { get; set; } = ActionID.None;

    public ActionConditionType ActionConditionType = ActionConditionType.Elapsed;

    public bool Condition { get; set; }

    public int Param1;
    public int Param2;
    public float Time;

    public override bool CheckBefore(ICustomRotation rotation)
    {
        return CheckBaseAction(rotation, ID, ref _action) && base.CheckBefore(rotation);
    }

    public override bool IsTrueInside(ICustomRotation rotation)
    {
        var result = false;

        switch (ActionConditionType)
        {
            case ActionConditionType.Elapsed:
                result = _action.ElapsedOneChargeAfter(Time); // Bigger
                break;

            case ActionConditionType.ElapsedGCD:
                result = _action.ElapsedOneChargeAfterGCD((uint)Param1, Param2); // Bigger
                break;

            case ActionConditionType.Remain:
                result = !_action.WillHaveOneCharge(Time); //Smaller
                break;

            case ActionConditionType.RemainGCD:
                result = !_action.WillHaveOneChargeGCD((uint)Param1, Param2); // Smaller
                break;

            case ActionConditionType.CanUse:
                result = _action.CanUse(out _, (CanUseOption)Param1, (byte)Param2);
                break;

            case ActionConditionType.EnoughLevel:
                result = _action.EnoughLevel;
                break;

            case ActionConditionType.IsCoolDown:
                result = _action.IsCoolingDown;
                break;

            case ActionConditionType.CurrentCharges:
                result = _action.CurrentCharges > Param1;
                break;

            case ActionConditionType.MaxCharges:
                result = _action.MaxCharges > Param1;
                break;
        }

        return Condition ? !result : result;
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
