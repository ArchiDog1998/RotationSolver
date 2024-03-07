using ECommons.DalamudServices;

namespace RotationSolver.Basic.Configuration.Timeline;

[Description("Action Timeline")]
internal class ActionTimelineItem : BaseTimelineItem
{
    public ActionID ID { get; set; } = ActionID.None;
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
        var act = DataCenter.RightNowRotation?.AllBaseActions.FirstOrDefault(a => (ActionID)a.ID == ID);

        if (act == null) return;

        DataCenter.AddCommandAction(act, Service.Config.SpecialDuration);

#if DEBUG
        Svc.Log.Debug($"Added the action {act} to timeline.");
#endif
        base.OnEnable();
    }
}
