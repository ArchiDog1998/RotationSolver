namespace RotationSolver.ActionSequencer;

internal interface ICondition
{
    bool IsTrue(ICustomRotation rotation);
    void Draw(ICustomRotation rotation);
}