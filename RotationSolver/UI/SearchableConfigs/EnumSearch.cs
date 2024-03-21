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
        var value = Value;

        var names = new List<string>();
        foreach (Enum v in Enum.GetValues(_property.PropertyType))
        {
            names.Add(v.Local());
        }
        var strs = names.ToArray();

        if (strs.Length > 0)
        {
            ImGui.SetNextItemWidth(Math.Max(ImGui.CalcTextSize(strs[value % strs.Length]).X + 30, DRAG_WIDTH) * Scale);
            if (ImGui.Combo($"##Config_{ID}{GetHashCode()}", ref value, strs, strs.Length))
            {
                int index = 0;
                foreach (var v in Enum.GetValues(_property.PropertyType))
                {
                    if (index++ == value)
                    {
                        Value = Convert.ToInt32(v);
                        break;
                    }
                }
            }
        }

        if (ImGui.IsItemHovered()) ShowTooltip();

        if (IsJob) DrawJobIcon();
        ImGui.SameLine();
        ImGui.TextWrapped(Name);
        if (ImGui.IsItemHovered()) ShowTooltip(false);
    }
}