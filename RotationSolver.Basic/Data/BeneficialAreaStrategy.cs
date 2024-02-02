namespace RotationSolver.Basic.Data;
public enum BeneficialAreaStrategy : byte
{
    [Description("On predefined location")]
    OnLocations,

    [Description("Only on predefined location")]
    OnlyOnLocations,

    [Description("On target")]
    OnTarget,

    [Description("On the calculated location")]
    OnCalculated,
}