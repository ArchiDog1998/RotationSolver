namespace RotationSolver.Basic.Configuration.Timeline;

[Description("State Timeline")]
internal class StateTimelineItem : ITimelineItem
{
    public SpecialCommandType State { get; set; } = SpecialCommandType.DefenseArea;
    public float Time { get; set; } = 3;
    public bool InPeriod(TimelineItem item)
    {
        var time = item.Time - DataCenter.RaidTimeRaw;

        if (time < 0) return false;

        if (time > Time || Time - time > 3) return false;
        return true;
    }
}
