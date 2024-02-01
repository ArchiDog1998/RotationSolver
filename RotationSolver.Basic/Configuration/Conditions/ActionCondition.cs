namespace RotationSolver.Basic.Configuration.Conditions;

internal class ActionCondition : DelayCondition
{
    internal IBaseAction _action = null!;

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
                return _action.Cooldown.ElapsedOneChargeAfter(Time); // Bigger

            case ActionConditionType.ElapsedGCD:
                return _action.Cooldown.ElapsedOneChargeAfterGCD((uint)Param1, Param2); // Bigger

            case ActionConditionType.Remain:
                return !_action.Cooldown.WillHaveOneCharge(Time); //Smaller

            case ActionConditionType.RemainGCD:
                return !_action.Cooldown.WillHaveOneChargeGCD((uint)Param1, Param2); // Smaller

            case ActionConditionType.CanUse:
                return _action.CanUse(out _);

            case ActionConditionType.EnoughLevel:
                return _action.EnoughLevel;

            case ActionConditionType.IsCoolDown:
                return _action.Cooldown.IsCoolingDown;

            case ActionConditionType.CurrentCharges:
                switch (Param2)
                {
                    case 0:
                        return _action.Cooldown.CurrentCharges > Param1;
                    case 1:
                        return _action.Cooldown.CurrentCharges < Param1;
                    case 2:
                        return _action.Cooldown.CurrentCharges == Param1;
                }
                break;

            case ActionConditionType.MaxCharges:
                switch (Param2)
                {
                    case 0:
                        return _action.Cooldown.MaxCharges > Param1;
                    case 1:
                        return _action.Cooldown.MaxCharges < Param1;
                    case 2:
                        return _action.Cooldown.MaxCharges == Param1;
                }
                break;
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
