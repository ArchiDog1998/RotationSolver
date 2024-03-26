using RotationSolver.Basic.Configuration.Timeline.TimelineDrawing;

namespace RotationSolver.Basic.Configuration.Timeline;

[Description("Drawing Timeline")]
internal class DrawingTimeline : BaseTimelineItem
{
    public float Duration { get; set; } = 6;

    public List<BaseDrawingGetter> DrawingGetters { get; set; } = [];

    private IDisposable[] _drawings = [];

    public DrawingTimeline()
    {
        Time = 6;
    }

    public override bool InPeriod(TimelineItem item)
    {
        var time = item.Time - DataCenter.RaidTimeRaw;

        if (time < Time - Duration) return false;
        if (time > Time) return false;

        if (!Condition.IsTrue(item)) return false;

        return true;
    }

    internal override void OnEnable()
    {
        foreach (var item in _drawings)
        {
            item.Dispose();
        }

        if (Service.Config.ShowTimelineDrawing)
        {
            _drawings = [.. DrawingGetters.Where(i => i.Enable).SelectMany(i => i.GetDrawing())];
        }
        else
        {
            _drawings = [];
        }

#if DEBUG
        //Svc.Log.Debug($"Added the state {item2.State} to timeline.");
#endif
        base.OnEnable();
    }

    internal override void OnDisable()
    {
        foreach (var item in _drawings)
        {
            item.Dispose();
        }
        _drawings = [];
        base.OnDisable();
    }
}
