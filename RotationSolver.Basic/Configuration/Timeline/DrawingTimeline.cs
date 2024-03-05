using ECommons.Configuration;
using ECommons.DalamudServices;
using ECommons.GameFunctions;
using RotationSolver.Basic.Configuration.Timeline.TimelineCondition;
using XIVPainter;
using XIVPainter.Element;
using XIVPainter.Vfx;
using Action = Lumina.Excel.GeneratedSheets.Action;


namespace RotationSolver.Basic.Configuration.Timeline;

[Description("Drawing Timeline")]
internal class DrawingTimeline : BaseTimelineItem
{
    private IDisposable[] _drawings = [];
    public ITimelineCondition Condition { get; set; } = new TrueTimelineCondition();
    public StaticVfxConfig StaticVfxConfig { get; set; }
    public ObjectVfxConfig ObjectConfig { get; set; }
    public ActionVfxConfig ActionConfig { get; set; }
    public ObjectGetter ObjectGetter { get; set; } = new();
    public DrawingType Type { get; set; }

    public uint ActionID { get; set; }
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
        _drawings = CreateDrawing();

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

    private IDisposable[] CreateDrawing()
    {
        switch (Type)
        {
            case DrawingType.Ground:
                if (string.IsNullOrEmpty(StaticVfxConfig.Path)) break;

                return [new StaticVfx(StaticVfxConfig.Path, StaticVfxConfig.Position, StaticVfxConfig.Rotation, StaticVfxConfig.Scale)];

            case DrawingType.Object:
                return [..ObjectGetter.GetObjects().Select(GetObjectDrawing)];

            case DrawingType.Action:
                return [.. ObjectGetter.GetObjects().Select(GetActionDrawing)];
        }
        return [];
    }

    private IDisposable? GetObjectDrawing(GameObject obj)
    {
        switch (ObjectConfig.Type)
        {
            case ObjectVfxConfig.ObjectType.Single:
                return new Single1(obj, ObjectConfig.Radius);

            case ObjectVfxConfig.ObjectType.Stack2:
                return new Share2(obj, ObjectConfig.Radius);

            case ObjectVfxConfig.ObjectType.Stack4:
                return new Share4(obj, ObjectConfig.Radius);

            case ObjectVfxConfig.ObjectType.General:
                if (string.IsNullOrEmpty(StaticVfxConfig.Path)) break;

                var result = new StaticVfx(StaticVfxConfig.Path, obj, StaticVfxConfig.Scale)
                {
                    LocationOffset = StaticVfxConfig.Position,
                    RotateAddition = StaticVfxConfig.Rotation,
                };
                if (ObjectConfig.TargetOnTarget)
                {
                    result.Target = obj.TargetObject;
                }
                return result;
        }
        return null;
    }

    private IDisposable? GetActionDrawing(GameObject obj)
    {
        if (ActionID == 0) return null;
        var action = Svc.Data.GetExcelSheet<Action>()?.GetRow(ActionID);
        if (action == null) return null;
        var omen = action.Omen.Value?.Path?.RawString;
        omen = string.IsNullOrEmpty(omen) ? StaticVfxConfig.Path : omen.Omen();

        var x = ActionConfig.X ?? (action.XAxisModifier > 0 ? action.XAxisModifier / 2 : action.EffectRange);
        var y = ActionConfig.Y ?? action.EffectRange;
        var scale = new Vector3(x, XIVPainterMain.HeightScale, y);

        if (action.TargetArea)
        {
            var location = StaticVfxConfig.Position;
            if (obj is BattleChara battle)
            {
                unsafe
                {
                    var info = battle.Struct()->GetCastInfo;
                    if (info->IsCasting != 0)
                    {
                        location = info->CastLocation;
                    }
                }
            }
            return new StaticVfx(omen, location, 0, scale);
        }
        else
        {
            return new StaticVfx(omen, obj, scale)
            {
                RotateAddition = StaticVfxConfig.Rotation / 180 * MathF.PI,
                LocationOffset = StaticVfxConfig.Position,
            };
        }
    }
}

public class ObjectGetter
{
    public GameObject[] GetObjects()
    {
        return [];
    }
}

public struct ActionVfxConfig
{
    public float? X { get; set; }
    public float? Y { get; set; }
}

public struct ObjectVfxConfig
{
    public enum ObjectType
    {
        Single,
        Stack2,
        Stack4,
        General,
    }

    public float Radius { get; set; }
    public bool TargetOnTarget { get; set; }
    public ObjectType Type { get; set; }
}

public struct StaticVfxConfig
{
    public string Path { get; set; }
    public Vector3 Position { get; set; }
    public float Rotation { get; set; }
    public Vector3 Scale { get; set; }
}

public enum DrawingType
{
    Ground,
    Object,
    Action,
}
