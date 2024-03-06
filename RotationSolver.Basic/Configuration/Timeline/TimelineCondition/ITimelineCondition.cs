namespace RotationSolver.Basic.Configuration.Timeline.TimelineCondition;
internal interface ITimelineCondition
{
    bool IsTrue(TimelineItem item);
}
