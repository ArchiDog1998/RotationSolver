namespace RotationSolver.Basic.Actions;

/// <summary>
/// Setting from the developer.
/// </summary>
public struct ActionSetting()
{
    public bool TargetStatusFromSelf { get; set; } = true;
    public StatusID[]? TargetStatus { get; set; } = null;
    public Func<GameObject, bool> CanTarget { get; set; } = t => true;
    internal ActionID[]? ComboIdsNot { get; set; }

    internal ActionID[]? ComboIds { get; set; }
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
    public TargetType TargetType { get; set; } = TargetType.Big;
}
