namespace RotationSolver.Basic.Configuration.Conditions;

internal interface ICondition
{
    bool IsTrue(ICustomRotation rotation);
    bool CheckBefore(ICustomRotation rotation);
}