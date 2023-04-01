namespace RotationSolver.Basic.Data;

[Flags]
public enum CanUseOption : byte
{
    /// <summary>
    /// Nothing serious.
    /// </summary>
    None = 0,

    /// <summary>
    /// AOE only need one target to use.
    /// Moving action don't need to have enough distance to use. 
    /// Skip for StatusProvide and TargetStatus checking.
    /// </summary>
    MustUse = 1 << 0,

    /// <summary>
    /// Use all charges, no keeping one.
    /// Do not need to check the combo.
    /// </summary>
    EmptyOrSkipCombo = 1 << 1,

    /// <summary>
    /// Skip the disable for emergency use. Please always set this to false.
    /// </summary>
    SkipDisable = 1 << 2,

    /// <summary>
    /// Ignore the target data.
    /// </summary>
    IgnoreTarget = 1 << 3,
}
