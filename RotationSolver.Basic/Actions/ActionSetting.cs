namespace RotationSolver.Basic.Actions;

/// <summary>
/// Setting from the developer.
/// </summary>
public struct ActionSetting()
{
    public bool TargetStatusFromSelf { get; set; } = true;
    public StatusID[]? TargetStatus { get; set; } = null;
    public Func<GameObject, bool> CanTarget { get; set; } = t => true;
    public ActionID[]? ComboIdsNot { get; set; }

    public ActionID[]? ComboIds { get; set; }
    /// <summary>
    /// Status that this action provides.
    /// </summary>
    public StatusID[]? StatusProvide { get; set; } = null;

    /// <summary>
    /// Status that this action needs.
    /// </summary>
    public StatusID[]? StatusNeed { get; set; } = null;

    public Func<bool>? ActionCheck { get; set; } = null;

    public bool IsFriendly { get; set; }

    private TargetType _type = TargetType.Big;
    public TargetType TargetType 
    { 
        readonly get => IBaseAction.TargetOverride ?? _type; 
        set => _type = value; 
    }

    public EnemyPositional EnemyPositional { get; set; } = EnemyPositional.None;
}
