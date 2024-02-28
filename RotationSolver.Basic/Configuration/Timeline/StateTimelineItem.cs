namespace RotationSolver.Basic.Configuration.Timeline;

[Description("State Timeline")]
internal class StateTimelineItem : ITimelineItem
{
    public AutoStatus State { get; set; }
    public float Time { get; set; } = 3;
    public float Duration { get; set; } = 3;

    public bool InPeriod(TimelineItem item)
    {
        var time = item.Time - DataCenter.RaidTimeRaw;

        if (time < 0) return false;

        if (time > Time || Time - time > Duration) return false;
        return true;
    }
}
