using RotationSolver.Basic.Configuration.Timeline.TimelineCondition;

namespace RotationSolver.Basic.Configuration.Timeline;

internal abstract class BaseTimelineItem
{
    public TimelineConditionSet Condition { get; set; } = new();

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

    internal virtual void OnEnable() { }
    internal virtual void OnDisable() { }
}
