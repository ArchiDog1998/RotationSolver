namespace RotationSolver.Basic.Configuration.Timeline.TimelineCondition;

[Description("Action Condition")]
internal class TimelineConditionAction : ITimelineCondition
{
    public uint ActionID { get; set; }
    public bool IsTrue(TimelineItem item)
    {
        return ActionID == item.LastActionID;
    }
}
