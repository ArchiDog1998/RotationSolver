namespace RotationSolver.Basic.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class RotationAttribute(string name, CombatType type) : Attribute
{
    public string Name => name;
    public CombatType Type => type;

    public string? Description {  get; set; }

    public string? GameVersion { get; set; }
}
