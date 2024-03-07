using ECommons.DalamudServices;

namespace RotationSolver.Basic.Configuration.Timeline;

[Description("State Timeline")]
internal class StateTimelineItem : BaseTimelineItem
{
    public SpecialCommandType State { get; set; } = SpecialCommandType.DefenseArea;
    public override bool InPeriod(TimelineItem item)
    {
        var time = item.Time - DataCenter.RaidTimeRaw;

        if (time < 0) return false;

        if (time > Time || Time - time > 3) return false;

        if (!Condition.IsTrue(item)) return false;

        return true;
    }

    internal override void OnEnable()
    {
        DataCenter.SpecialType = State;
#if DEBUG
        Svc.Log.Debug($"Added the state {State} to timeline.");
#endif
        base.OnEnable();
    }
}
