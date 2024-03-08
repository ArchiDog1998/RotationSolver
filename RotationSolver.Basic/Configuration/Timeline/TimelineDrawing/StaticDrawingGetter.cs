using XIVPainter.Vfx;

namespace RotationSolver.Basic.Configuration.Timeline.TimelineDrawing;

[Description("Static Drawing")]
internal class StaticDrawingGetter : BaseDrawingGetter
{
    public string Path { get; set; } = GroundOmenHostile.Circle.Omen();
    public Vector3 Position { get; set; }
    public float Rotation { get; set; }
    public Vector3 Scale { get; set; }
    public TextDrawing Text { get; set; } = new();
    public override IDisposable[] GetDrawing()
    {
        if (string.IsNullOrEmpty(Path)) return [];
        var item = new StaticVfx(Path, Position, Rotation, Scale);
        var text = Text.GetText(Position);
        return text == null ? [item] : [item, text];
    }
}