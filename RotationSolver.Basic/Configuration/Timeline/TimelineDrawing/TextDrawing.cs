using XIVDrawer.Element3D;

namespace RotationSolver.Basic.Configuration.Timeline.TimelineDrawing;
internal class TextDrawing
{
    public string Text { get; set; } = "";
    public Vector3 PositionOffset { get; set; }

    public Vector2 Padding { get; set; } = Vector2.One * 5;

    public float Scale { get; set; } = 1;

    public Vector4 BackgroundColor { get; set; } = new(0, 0, 0, 0.5f);

    public Vector4 Color { get; set; } = new(1, 1, 1, 1);

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
            BackgroundColor = ImGui.ColorConvertFloat4ToU32(BackgroundColor),
            Color = ImGui.ColorConvertFloat4ToU32(Color),
            Scale = Scale,
        };
    }

    public Drawing3DText? GetText(GameObject obj)
    {
        var text = GetText(obj.Position);

        if (text == null) return null;

        text.Text = text.Text.Replace("{Name}", obj.Name.TextValue);
        text.UpdateEveryFrame += () =>
        {
            text.Position = obj.Position + PositionOffset;
        };
        return text;
    }
}
