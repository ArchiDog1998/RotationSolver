using XIVPainter.Element3D;

namespace RotationSolver.Basic.Configuration.Timeline.TimelineDrawing;
internal class TextDrawing
{
    public string? Text { get; set; }
    public Vector3 PositionOffset { get; set; }

    public Vector2 Padding { get; set; } = Vector2.One * 5;

    public float Scale { get; set; } = 1;

    public uint BackgroundColor { get; set; } = 0x00000080;

    public uint Color { get; set; } = uint.MaxValue;

    /// <summary>
    /// The corner of the background.
    /// </summary>
    public float Corner { get; set; } = 5;

    public Drawing3DText? GetText(Vector3 position)
    {
        if (string.IsNullOrEmpty(Text)) return null;
        return new(Text, position + PositionOffset)
        {
            HideIfInvisible = false,
            Padding = Padding,
            BackgroundColor = BackgroundColor,
            Color = Color,
            Scale = Scale,
        };
    }

    public Drawing3DText? GetText(GameObject obj)
    {
        var text = GetText(obj.Position);
        if (text == null) return null;
        text.UpdateEveryFrame += () =>
        {
            text.Position = obj.Position + PositionOffset;
        };
        return text;
    }
}
