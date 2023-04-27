namespace RotationSolver.Timeline;

internal interface ICondition
{
    const float DefaultHeight = 33;
    bool IsTrue(ICustomRotation rotation, bool isActionSequencer);
    void Draw(ICustomRotation rotation, bool isActionSequencer);
    float Height { get; }
}