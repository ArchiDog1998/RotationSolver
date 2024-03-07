using ECommons.Automation;

namespace RotationSolver.Basic.Configuration.Timeline;

[Description("Macro Time line")]
internal class MacroTimelineItem : BaseTimelineItem
{
    public string Macro { get; set; } = "";

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
        if (!string.IsNullOrEmpty(Macro))
        {
            Chat.Instance.SendMessage(Macro);
        }
        base.OnEnable();
    }
}
