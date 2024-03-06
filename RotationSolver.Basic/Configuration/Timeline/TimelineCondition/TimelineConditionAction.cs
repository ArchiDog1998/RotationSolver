namespace RotationSolver.Basic.Configuration.Timeline.TimelineCondition;
internal class TimelineConditionAction : ITimelineCondition
{
    public uint ActionID { get; set; }
    public bool IsTrue(TimelineItem item)
    {
        if (ActionID == 0) return true;
        return ActionID == item.LastActionID;
    }
}
