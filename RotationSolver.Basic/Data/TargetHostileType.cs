namespace RotationSolver.Basic.Data;

/// <summary>
/// Hostile target.
/// </summary>
public enum TargetHostileType : byte
{
    /// <summary>
    /// All
    /// </summary>
    AllTargetsCanAttack,

    /// <summary>
    /// Have target then all.
    /// </summary>
    TargetsHaveTargetOrAllTargetsCanAttack,

    /// <summary>
    /// Have target.
    /// </summary>
    TargetsHaveTarget,
}
