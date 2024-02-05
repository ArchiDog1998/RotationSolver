namespace RotationSolver.Basic.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public class IDAttribute(uint id) : Attribute
{
    public uint ID => id;
}
