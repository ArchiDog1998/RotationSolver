using ECommons.DalamudServices;

namespace RotationSolver.Basic.Configuration.Timeline.TimelineCondition;

[Description("Target Count Condition")]
internal class TimelineConditionTargetCount : ITimelineCondition
{
    public int Count { get; set; }
    public ObjectGetter Getter { get; set; } = new();
    public bool IsTrue(TimelineItem item)
    {
        return Svc.Objects.Count(Getter.CanGet) >= Count;
    }
}
