namespace RotationSolver.Basic.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class DutyTerritoryAttribute(params uint[] territoryIds) : Attribute
{
    public uint[] TerritoryIds => territoryIds;
}

