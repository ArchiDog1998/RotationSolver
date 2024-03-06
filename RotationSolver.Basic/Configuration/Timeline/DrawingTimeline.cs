using RotationSolver.Basic.Configuration.Timeline.TimelineCondition;
using RotationSolver.Basic.Configuration.Timeline.TimelineDrawing;


namespace RotationSolver.Basic.Configuration.Timeline;

[Description("Drawing Timeline")]
internal class DrawingTimeline : BaseTimelineItem
{
    public ITimelineCondition Condition { get; set; } = new TrueTimelineCondition();
    public List<IDrawingGetter> DrawingGetters { get; set; } = [];

    private IDisposable[] _drawings = [];

    public DrawingTimeline()
    {
        Time = 5;
    }

    public override bool InPeriod(TimelineItem item)
    {
        var time = item.Time - DataCenter.RaidTimeRaw;

        if (time < 0) return false;
        if (time > Time) return false;

        if (!Condition.IsTrue()) return false;

        return true;
    }

    protected override void OnEnable()
    {
        foreach (var item in _drawings)
        {
            item.Dispose();
        }

        _drawings = [.. DrawingGetters.SelectMany(i => i.GetDrawing())];

#if DEBUG
        //Svc.Log.Debug($"Added the state {item2.State} to timeline.");
#endif
        base.OnEnable();
    }

    protected override void OnDisable()
    {
        foreach (var item in _drawings)
        {
            item.Dispose();
        }
        _drawings = [];
        base.OnDisable();
    }
}
