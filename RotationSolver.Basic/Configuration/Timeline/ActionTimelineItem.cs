namespace RotationSolver.Basic.Configuration.Timeline;

[Description("Action Timeline")]
internal class ActionTimelineItem : ITimelineItem
{
    public ActionID ID { get; set; } = ActionID.None;
    public float Time { get; set; } = 3;
    public bool InPeriod(TimelineItem item)
    {
        var time = item.Time - DataCenter.RaidTimeRaw;

        if (time < 0) return false;

        if (time > Time || Time - time > 3) return false;
        return true;
    }
}
