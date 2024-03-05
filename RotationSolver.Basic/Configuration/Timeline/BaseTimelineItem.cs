namespace RotationSolver.Basic.Configuration.Timeline;

internal abstract class BaseTimelineItem
{
    public float Time { get; set; } = 3;
    private bool _enable = false;
    internal bool Enable
    {
        get => _enable;
        set
        {
            if (_enable == value) return;
            _enable = value;

            if (_enable)
            {
                OnEnable();
            }
            else
            {
                OnDisable();
            }
        }
    }
    public abstract bool InPeriod(TimelineItem item);

    protected virtual void OnEnable() { }
    protected virtual void OnDisable() { }
}
