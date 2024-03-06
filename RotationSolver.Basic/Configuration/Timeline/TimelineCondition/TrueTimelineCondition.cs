namespace RotationSolver.Basic.Configuration.Timeline.TimelineCondition;

internal class TrueTimelineCondition : ITimelineCondition
{
    public bool IsTrue(TimelineItem item) => true;
}
