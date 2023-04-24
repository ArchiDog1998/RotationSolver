namespace RotationSolver.Timeline;

internal interface ICondition
{
    const float DefaultHeight = 33;
    bool IsTrue(ICustomRotation rotation);
    void Draw(ICustomRotation rotation);
    float Height { get; }
}