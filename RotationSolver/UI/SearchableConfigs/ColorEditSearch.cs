namespace RotationSolver.UI.SearchableConfigs;

internal class ColorEditSearch(PropertyInfo property) : Searchable(property)
{
    protected Vector4 Value 
    {
        get => (Vector4)_property.GetValue(Service.Config)!;
        set => _property.SetValue(Service.Config, value);
    }

    protected override void DrawMain()
    {
        var value = Value;
        ImGui.SetNextItemWidth(DRAG_WIDTH * 1.5f * Scale);
        if (ImGui.ColorEdit4($"{Name}##Config_{ID}{GetHashCode()}", ref value))
        {
            Value = value;
        }
        if (ImGui.IsItemHovered()) ShowTooltip();
    }
}
