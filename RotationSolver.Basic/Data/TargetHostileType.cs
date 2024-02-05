namespace RotationSolver.Basic.Data;

/// <summary>
/// Hostile target.
/// </summary>
public enum TargetHostileType : byte
{
    /// <summary>
    /// All
    /// </summary>
    [Description("All targets that are in range for any abilities")]
    AllTargetsCanAttack,

    /// <summary>
    /// Have target.
    /// </summary>
    [Description("Previously engaged targets (engages on countdown timer)")]
    TargetsHaveTarget,
}
