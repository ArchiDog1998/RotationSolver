namespace RotationSolver.UI.SearchableConfigs;

internal class DragIntSearch : Searchable
{
    public int Min { get; }
    public int Max { get; }
    public float Speed { get; }
    protected int Value
    {
        get => (int)_property.GetValue(Service.Config)!;
        set => _property.SetValue(Service.Config, value);
    }
    public DragIntSearch(PropertyInfo property) : base(property)
    {
        var range = _property.GetCustomAttribute<RangeAttribute>();
        Min = (int?)range?.MinValue ?? 0;
        Max = (int?)range?.MaxValue ?? 1;
        Speed = range?.Speed ?? 0.001f;
    }

    protected override void DrawMain()
    {
        var value = Value;

        ImGui.SetNextItemWidth(Scale * DRAG_WIDTH);
        if (ImGui.DragInt($"##Config_{ID}{GetHashCode()}", ref value, Speed, Min, Max))
        {
            Value = value;
        }

        if (ImGui.IsItemHovered()) ShowTooltip();

        if (IsJob) DrawJobIcon();
        ImGui.SameLine();
        ImGui.TextWrapped(Name);
        if (ImGui.IsItemHovered()) ShowTooltip(false);
    }
}
