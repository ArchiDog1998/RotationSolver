using ECommons.DalamudServices;
using RotationSolver.Basic.Configuration.Timeline.TimelineCondition;
using XIVDrawer.Vfx;

namespace RotationSolver.Basic.Configuration.Timeline.TimelineDrawing;

[Description("Object Drawing")]
internal class ObjectDrawingGetter : BaseDrawingGetter
{
    public string Path { get; set; } = GroundOmenHostile.Circle.Omen();
    public bool IsActorEffect { get; set; } = false;
    public Vector3 Position { get; set; }
    public float Rotation { get; set; }
    public Vector3 Scale { get; set; } = Vector3.One;
    public ObjectGetter ObjectGetter { get; set; } = new();
    public ObjectGetter TargetGetter { get; set; } = new();
    public bool IsTargetByTarget { get; set; } = true;
    public bool GetATarget { get; set; } = false;
    public TextDrawing ObjectText { get; set; } = new();
    public TextDrawing TargetText { get; set; } = new();

    public override IDisposable[] GetDrawing()
    {
        var objs = Svc.Objects.Where(ObjectGetter.CanGet);
        return [..objs.SelectMany(GetTextDrawing),
            ..objs.SelectMany(GetObjectDrawing)];
    }

    private GameObject[] TargetGet(GameObject obj)
    {
        if (!GetATarget) return [];
        if (IsTargetByTarget)
        {
            var tar = obj.TargetObject;
            if (tar == null) return [];
            return [tar];
        }
        else
        {
            return [.. Svc.Objects.Where(TargetGetter.CanGet)];
        }
    }
    private IDisposable[] GetTextDrawing(GameObject obj)
    {
        return [..TargetGet(obj).Select(TargetText.GetText)
            .Append(ObjectText.GetText(obj))
            .OfType<IDisposable>()];
    }

    private IDisposable[] GetObjectDrawing(GameObject obj)
    {
        if (string.IsNullOrEmpty(Path)) return [];

        var targets = TargetGet(obj);
        if (IsActorEffect)
        {
            if (targets.Length > 0)
            {
                return [.. targets.Select(t => new ActorVfx(Path, obj, t))];
            }
            else
            {
                return [new ActorVfx(Path, obj, obj)];
            }
        }
        else
        {
            if (targets.Length > 0)
            {
                return [.. targets.Select(t => new StaticVfx(Path, obj, Scale)
                {
                    LocationOffset = Position,
                    RotateAddition = Rotation,
                    Target = t,
                })];
            }
            else
            {
                return [new StaticVfx(Path, obj, Scale)
                {
                    LocationOffset = Position,
                    RotateAddition = Rotation,
                }];
            }
        }
    }
}