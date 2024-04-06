using RotationSolver.Localization;

namespace RotationSolver.UI.SearchableConfigs;

internal class DragFloatSearch : Searchable
{
    public float Min { get; }
    public float Max { get; }
    public float Speed { get; }
    public ConfigUnitType Unit { get; }

    protected float Value
    {
        get => (float)_property.GetValue(Service.Config)!;
        set => _property.SetValue(Service.Config, value);
    }

    public DragFloatSearch(PropertyInfo property) : base(property)
    {
        var range = _property.GetCustomAttribute<RangeAttribute>();
        Min = range?.MinValue ?? 0f;
        Max = range?.MaxValue ?? 1f;
        Speed = range?.Speed ?? 0.001f;
        Unit = range?.UnitType ?? ConfigUnitType.None;
    }

    protected override void DrawMain()
    {
        var value = Value;
        ImGui.SetNextItemWidth(Scale * DRAG_WIDTH);

        if (Unit == ConfigUnitType.Percent)
        {
            if (ImGui.SliderFloat($"##Config_{ID}{GetHashCode()}", ref value, Min, Max, $"{value * 100f:F1}{Unit.ToSymbol()}"))
            {
                Value = value;
            }
        }
        else
        {
            if (ImGui.DragFloat($"##Config_{ID}{GetHashCode()}", ref value, Speed, Min, Max, $"{value:F2}{Unit.ToSymbol()}"))
            {
                Value = value;
            }
        }

        if (ImGui.IsItemHovered()) ShowTooltip();

        if (IsJob) DrawJobIcon();

        ImGui.SameLine();

        if (Color != 0) ImGui.PushStyleColor(ImGuiCol.Text, Color);
        ImGui.TextWrapped(Name);
        if (Color != 0) ImGui.PopStyleColor();

        if (ImGui.IsItemHovered()) ShowTooltip(false);
    }

    protected override void TooltipAdditional()
    {
        ImGui.Separator();
        ImGui.TextDisabled(Unit.Local());
        base.TooltipAdditional();
    }
}
