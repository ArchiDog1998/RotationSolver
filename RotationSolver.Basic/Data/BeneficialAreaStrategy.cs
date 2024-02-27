namespace RotationSolver.Basic.Data;

/// <summary>
/// The way to place the beneficial area action.
/// </summary>
public enum BeneficialAreaStrategy : byte
{
    /// <summary>
    /// On predefined location
    /// </summary>
    [Description("On predefined location")]
    OnLocations,

    /// <summary>
    /// Only on predefined location
    /// </summary>
    [Description("Only on predefined location")]
    OnlyOnLocations,

    /// <summary>
    /// On target
    /// </summary>
    [Description("On target")]
    OnTarget,

    /// <summary>
    /// On the calculated location
    /// </summary>
    [Description("On the calculated location")]
    OnCalculated,
}