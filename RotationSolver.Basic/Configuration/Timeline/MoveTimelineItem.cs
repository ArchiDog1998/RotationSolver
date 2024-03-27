using ECommons.DalamudServices;

namespace RotationSolver.Basic.Configuration.Timeline;

[Description("Move Time line")]
internal class MoveTimelineItem : BaseTimelineItem
{
    public List<Vector3> Points { get; set; } = [];
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
        base.OnEnable();

        if (!Service.Config.EnableTimelineMovement) return;

        var ipc = Svc.PluginInterface.GetIpcSubscriber<List<Vector3>, bool, object>("vnavmesh.Path.MoveTo");

        if (ipc == null)
        {
            Svc.Log.Error("Can't find the vnavmesh to move.");
            return;
        }
        ipc.InvokeAction(new (Points), false);
    }

    internal override void OnDisable()
    {
        base.OnDisable();

        if (!Service.Config.EnableTimelineMovement) return;

        var ipc = Svc.PluginInterface.GetIpcSubscriber<object>("vnavmesh.Path.Stop");
        ipc?.InvokeAction();
    }
}
