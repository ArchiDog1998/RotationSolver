namespace RotationSolver.Basic.Attributes;

/// <summary>
/// 
/// </summary>
/// <param name="type"></param>
[AttributeUsage(AttributeTargets.Property)]
public class RotationConfigAttribute(CombatType type) : Attribute
{
    /// <summary>
    /// The type of this config.
    /// </summary>
    public CombatType Type => type;
}
