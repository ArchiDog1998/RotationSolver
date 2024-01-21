namespace RotationSolver.Basic.Actions;
public struct ActionBasicInfo
{
    private readonly IBaseActionNew _action;

    public readonly uint ID => _action.Action.RowId;

    public readonly uint AdjustedID => (uint)Service.GetAdjustedActionId((ActionID)ID);

    public AttackType AttackType => (AttackType)(_action.Action.AttackType.Value?.RowId ?? byte.MaxValue);


    public bool IsFriendly { get; set; }

    public ActionBasicInfo(IBaseActionNew action)
    {
        _action = action;

        //TODO: better friendly check.
        IsFriendly = _action.Action.CanTargetFriendly;
    }
}

public enum ActionType : byte
{
    Move,
    Heal,
    Defence,
    Attack,
}


