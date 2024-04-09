using ECommons.DalamudServices;
using ECommons.GameFunctions;
using RotationSolver.Basic.Configuration.Timeline.TimelineCondition;
using XIVDrawer;
using XIVDrawer.Vfx;
using XIVPainter.Vfx;
using Action = Lumina.Excel.GeneratedSheets.Action;

namespace RotationSolver.Basic.Configuration.Timeline.TimelineDrawing;

[Description("Action Drawing")]
internal class ActionDrawingGetter : BaseDrawingGetter
{
    public uint ActionID { get; set; }
    public string Path { get; set; } = "";
    public float X { get; set; }
    public float Y { get; set; }
    public Vector3 Position { get; set; }
    public float Rotation { get; set; }
    public ObjectGetter ObjectGetter { get; set; } = new();

    public override IDisposable[] GetDrawing()
    {
        var objs = Svc.Objects.Where(ObjectGetter.CanGet);
        if (objs.Any())
        {
            return [.. objs.Select(GetActionDrawing).OfType<IDisposable>()];
        }

        var item = GetActionDrawing(null);
        if (item == null) return [];
        return [item];
    }

    private IDisposable? GetActionDrawing(GameObject? obj)
    {
        if (ActionID == 0) return null;
        var action = Svc.Data.GetExcelSheet<Action>()?.GetRow(ActionID);
        if (action == null) return null;
        var omen = action.Omen.Value?.Path?.RawString;
        omen = string.IsNullOrEmpty(omen) ? Path : omen.Omen();

        var x = X != 0 ? X : (action.XAxisModifier > 0 ? action.XAxisModifier / 2 : action.EffectRange);
        var y = Y != 0 ? Y : action.EffectRange;
        var scale = new Vector3(x, XIVDrawerMain.HeightScale, y);

        if (action.TargetArea)
        {
            var location = Position;
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
            if(obj != null)
            {
                return new StaticVfx(omen, obj, scale)
                {
                    RotateAddition = Rotation / 180 * MathF.PI,
                    LocationOffset = Position,
                };
            }
            else
            {
                return new StaticVfx(omen, Position, Rotation, scale);
            }
        }
    }
}
