using Newtonsoft.Json.Linq;
using RotationSolver.Localization;
using System;

namespace RotationSolver.UI.SearchableConfigs;

internal class EnumSearch(PropertyInfo property) : Searchable(property)
{
    protected int Value
    {
        get => Convert.ToInt32(_property.GetValue(Service.Config));
        set => _property.SetValue(Service.Config, Enum.ToObject(_property.PropertyType, value));
    }

    protected override void DrawMain()
    {
        var currentValue = Value;

        var enumValueToNameMap = new Dictionary<int, string>();
        foreach (Enum enumValue in Enum.GetValues(_property.PropertyType))
        {
            enumValueToNameMap[Convert.ToInt32(enumValue)] = enumValue.Local();
        }

        var displayNames = enumValueToNameMap.Values.ToArray();

        if (displayNames.Length > 0)
        {
            ImGui.SetNextItemWidth(Math.Max(displayNames.Max(name => ImGui.CalcTextSize(name).X) + 30, DRAG_WIDTH) * Scale);

            int currentIndex = enumValueToNameMap.Keys.ToList().IndexOf(currentValue);
            if (currentIndex == -1) currentIndex = 0; // Default to first item if not found

            if (ImGui.Combo($"##Config_{ID}{GetHashCode()}", ref currentIndex, displayNames, displayNames.Length))
            {
                Value = enumValueToNameMap.Keys.ElementAt(currentIndex);
            }
        }

        if (ImGui.IsItemHovered()) ShowTooltip();

        if (IsJob) DrawJobIcon();
        ImGui.SameLine();
        ImGui.TextWrapped(Name);
        if (ImGui.IsItemHovered()) ShowTooltip(false);
    }
}