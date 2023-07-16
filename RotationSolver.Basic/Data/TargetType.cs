namespace RotationSolver.Basic.Data;

/// <summary>
/// The type of targeting.
/// </summary>
public enum TargetingType
{
    /// <summary>
    /// Find the target whose hit box is biggest.
    /// </summary>
    Big,

    /// <summary>
    /// Find the target whose hit box is smallest.
    /// </summary>
    Small,

    /// <summary>
    /// Find the target whose hp is highest.
    /// </summary>
    HighHP,

    /// <summary>
    /// Find the target whose hp is lowest.
    /// </summary>
    LowHP,

    /// <summary>
    /// Find the target whose max hp is highest.
    /// </summary>
    HighMaxHP,

    /// <summary>
    /// Find the target whose max hp is lowest.
    /// </summary>
    LowMaxHP,
}
