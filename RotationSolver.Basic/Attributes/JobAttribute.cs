namespace RotationSolver.Basic.Attributes;

/// <summary>
/// The attribute for the ui configs.
/// </summary>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class JobFilterAttribute() : Attribute
{
    /// <summary>
    /// The filter for the pvp.
    /// </summary>
    public JobFilterType PvPFilter { get; set; }

    /// <summary>
    /// The filter for the pve.
    /// </summary>
    public JobFilterType PvEFilter { get; set; }
}

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
public enum JobFilterType : byte
{
    None,
    NoJob,
    NoHealer,
    Healer,
    Raise,
    Interrupt,
    Dispel,
    Tank,
    Melee,
}
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
internal class JobConfigAttribute : Attribute
{
}

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
internal class JobChoiceConfigAttribute : Attribute
{
}