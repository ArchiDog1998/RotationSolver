using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;
using Dalamud.Interface;
using Dalamud.Interface.Colors;
using Dalamud.Interface.Windowing;
using Dalamud.Utility;
using ImGuiNET;
using XIVComboPlus.Attributes;
using XIVComboPlus.Combos;

namespace XIVComboPlus;

internal class ConfigWindow : Window
{
    private readonly Dictionary<string, List<CustomCombo>> groupedPresets;

    private readonly Vector4 shadedColor = new Vector4(0.68f, 0.68f, 0.68f, 1f);

    public ConfigWindow()
        : base("Custom Combo Setup", 0, false)
    {
        RespectCloseHotkey = true;

        groupedPresets = Service.IconReplacer.CustomCombos.GroupBy((combo) => combo.JobName)
                            .ToDictionary(x => x.Key, x => x.ToList());

        SizeCondition = (ImGuiCond)4;
        Size = new Vector2(740f, 490f);
    }

    public override void Draw()
    {
        ImGui.Text("This window allows you to enable and disable custom combos to your liking.");
        bool enableSecretCombos = Service.Configuration.EnableSecretCombos;
        if (ImGui.Checkbox("Enable secret forbidden knowledge", ref enableSecretCombos))
        {
            Service.Configuration.EnableSecretCombos = enableSecretCombos;
            Service.Configuration.Save();
        }
        if (ImGui.IsItemHovered())
        {
            ImGui.BeginTooltip();
            ImGui.TextUnformatted("Combos too dangerous for the common folk");
            ImGui.EndTooltip();
        }
        ImGui.BeginChild("scrolling", new Vector2(0f, -1f), true);
        ImGui.PushStyleVar((ImGuiStyleVar)13, new Vector2(0f, 5f));
        int num = 1;
        foreach (string key in groupedPresets.Keys)
        {
            if (ImGui.CollapsingHeader(key))
            {
                foreach (var combo in groupedPresets[key])
                {
                    bool enable = Service.Configuration.IsEnabled(combo.ComboFancyName);
                    string[] conflicts = combo.ConflictingCombos;
                    string parent = combo.ParentCombo;
                    if (combo.SecretCombo && !enableSecretCombos)
                    {
                        continue;
                    }
                    ImGui.PushItemWidth(200f);
                    if (ImGui.Checkbox(combo.ComboFancyName, ref enable))
                    {
                        if (enable)
                        {
                            Service.Configuration.EnabledActions.Add(combo.ComboFancyName);
                            string[] array = conflicts;
                            foreach (string item3 in array)
                            {
                                Service.Configuration.EnabledActions.Remove(item3);
                            }
                        }
                        else
                        {
                            Service.Configuration.EnabledActions.Remove(combo.ComboFancyName);
                        }
                        Service.IconReplacer.UpdateEnabledActionIDs();
                        Service.Configuration.Save();
                    }
                    if (combo.SecretCombo)
                    {
                        ImGui.SameLine();
                        ImGui.Text("  ");
                        ImGui.SameLine();
                        ImGui.PushFont(UiBuilder.IconFont);
                        ImGui.PushStyleColor((ImGuiCol)0, ImGuiColors.HealerGreen);
                        ImGui.Text(((FontAwesomeIcon)61445).ToIconString());
                        ImGui.PopStyleColor();
                        ImGui.PopFont();
                        if (ImGui.IsItemHovered())
                        {
                            ImGui.BeginTooltip();
                            ImGui.TextUnformatted("Secret");
                            ImGui.EndTooltip();
                        }
                    }
                    ImGui.PopItemWidth();
                    string text = $"#{num}: {combo.Description}";
                    if (!string.IsNullOrEmpty(parent))
                    {
                        text += "\nRequires " + combo.ComboFancyName;
                    }
                    ImGui.TextColored(shadedColor, text);
                    ImGui.Spacing();
                    if (conflicts.Length != 0)
                    {
                        ImGui.TextColored(shadedColor, "Conflicts with: " + string.Join("\n - ", conflicts));
                        ImGui.Spacing();
                    }
                    //if (item == CustomComboPreset.DancerDanceComboCompatibility && enable)
                    //{
                    //    int[] array2 = Service.Configuration.DancerDanceCompatActionIDs.Cast<int>().ToArray();
                    //    if (false | ImGui.InputInt("Emboite (Red) ActionID", ref array2[0], 0) | ImGui.InputInt("Entrechat (Blue) ActionID", ref array2[1], 0) | ImGui.InputInt("Jete (Green) ActionID", ref array2[2], 0) | ImGui.InputInt("Pirouette (Yellow) ActionID", ref array2[3], 0))
                    //    {
                    //        Service.Configuration.DancerDanceCompatActionIDs = array2.Cast<uint>().ToArray();
                    //        Service.IconReplacer.UpdateEnabledActionIDs();
                    //        Service.Configuration.Save();
                    //    }
                    //    ImGui.Spacing();
                    //}
                    num++;
                }
            }
            else
            {
                num += groupedPresets[key].Count;
            }
        }
        ImGui.PopStyleVar();
        ImGui.EndChild();
    }
}
