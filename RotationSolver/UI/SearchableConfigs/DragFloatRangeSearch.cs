using RotationSolver.Localization;

namespace RotationSolver.UI.SearchableConfigs;

internal class DragFloatRangeSearch : Searchable
{
    public float Min { get; }
    public float Max { get; }
    public float Speed { get; }
    public ConfigUnitType Unit { get; }

    public override string Description
    {
        get
        {
            var baseDesc = base.Description;
            if (!string.IsNullOrEmpty(baseDesc))
            {
                return baseDesc + "\n" + Unit.Local();
            }
            else
            {
                return Unit.Local();
            }
        }
    }

    protected Vector2 Value
    {
        get => (Vector2)_property.GetValue(Service.Config)!;
        set => _property.SetValue(Service.Config, value);
    }
    protected float MinValue 
    {
        get => Value.X;
        set
        {
            var v = Value;
            v.X = value;
            Value = v;
        }
    }
    protected float MaxValue
    {
        get => Value.Y;
        set
        {
            var v = Value;
            v.Y = value;
            Value = v;
        }
    }
    public DragFloatRangeSearch(PropertyInfo property) : base(property)
    {
        var range = _property.GetCustomAttribute<RangeAttribute>();
        Min = range?.MinValue ?? 0f;
        Max = range?.MaxValue ?? 1f;
        Speed = range?.Speed ?? 0.001f;
        Unit = range?.UnitType ?? ConfigUnitType.None;
    }

    protected override void DrawMain()
    {
        var minValue = MinValue;
        var maxValue = MaxValue;
        ImGui.SetNextItemWidth(Scale * DRAG_WIDTH);

        if (ImGui.DragFloatRange2($"##Config_{ID}{GetHashCode()}", ref minValue, ref maxValue, Speed, Min, Max,
     Unit == ConfigUnitType.Percent ? $"{minValue * 100:F1}{Unit.ToSymbol()}" : $"{minValue:F2}{Unit.ToSymbol()}",
    Unit == ConfigUnitType.Percent ? $"{maxValue * 100:F1}{Unit.ToSymbol()}" : $"{maxValue:F2}{Unit.ToSymbol()}"))
        {
            MinValue = Math.Min(minValue, maxValue);
            MaxValue = Math.Max(minValue, maxValue);
        }

        if (ImGui.IsItemHovered()) ShowTooltip();

        if (IsJob) DrawJobIcon();
        ImGui.SameLine();
        if (Color != 0) ImGui.PushStyleColor(ImGuiCol.Text, Color);
        ImGui.TextWrapped(Name);
        if (Color != 0) ImGui.PopStyleColor();
        if (ImGui.IsItemHovered()) ShowTooltip(false);
    }
}
