namespace RotationSolver.ActionSequencer;

internal interface ICondition
{
    bool IsTrue(ICustomRotation rotation);
    bool CheckBefore(ICustomRotation rotation);
}