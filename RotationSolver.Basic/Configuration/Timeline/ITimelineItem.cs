namespace RotationSolver.Basic.Configuration.Timeline;

internal interface ITimelineItem
{
    public float Time { get; set; }
    public float Duration { get; set; }
    public bool InPeriod(TimelineItem item);
}
