using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Dalamud.Interface;
using Dalamud.Interface.Colors;
using Dalamud.Interface.Windowing;
using Dalamud.Utility;
using ImGuiNET;
using XIVComboPlus.Attributes;

namespace XIVComboPlus;

internal class ConfigWindow : Window
{
    private readonly Dictionary<string, List<(CustomComboPreset Preset, CustomComboInfoAttribute Info)>> groupedPresets;

    private readonly Vector4 shadedColor = new Vector4(0.68f, 0.68f, 0.68f, 1f);

    public ConfigWindow()
        : base("Custom Combo Setup", 0, false)
    {
        RespectCloseHotkey = true;
        groupedPresets = (from preset in Enum.GetValues<CustomComboPreset>()
                          select (preset, preset.GetAttribute<CustomComboInfoAttribute>()) into tpl
                          where tpl.Item2 != null
                          orderby tpl.Item2.JobName, tpl.preset
                          group tpl by tpl.Item2.JobName).ToDictionary((IGrouping<string, (CustomComboPreset Preset, CustomComboInfoAttribute Info)> tpl) => tpl.Key, (IGrouping<string, (CustomComboPreset Preset, CustomComboInfoAttribute Info)> tpl) => tpl.ToList());
        SizeCondition = (ImGuiCond)4;
        Size = new Vector2(740f, 490f);
    }

    public override void Draw()
    {
        //IL_01cb: Unknown result type (might be due to invalid IL or missing references)
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
                foreach (var item4 in groupedPresets[key])
                {
                    CustomComboPreset item = item4.Preset;
                    CustomComboInfoAttribute item2 = item4.Info;
                    bool flag = Service.Configuration.IsEnabled(item);
                    bool flag2 = Service.Configuration.IsSecret(item);
                    CustomComboPreset[] conflicts = Service.Configuration.GetConflicts(item);
                    CustomComboPreset? parent = Service.Configuration.GetParent(item);
                    if (item == CustomComboPreset.Disabled || flag2 && !enableSecretCombos)
                    {
                        continue;
                    }
                    ImGui.PushItemWidth(200f);
                    if (ImGui.Checkbox(item2.FancyName, ref flag))
                    {
                        if (flag)
                        {
                            Service.Configuration.EnabledActions.Add(item);
                            CustomComboPreset[] array = conflicts;
                            foreach (CustomComboPreset item3 in array)
                            {
                                Service.Configuration.EnabledActions.Remove(item3);
                            }
                        }
                        else
                        {
                            Service.Configuration.EnabledActions.Remove(item);
                        }
                        Service.IconReplacer.UpdateEnabledActionIDs();
                        Service.Configuration.Save();
                    }
                    if (flag2)
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
                    string text = $"#{num}: {item2.Description}";
                    if (parent.HasValue)
                    {
                        CustomComboInfoAttribute attribute = item.GetAttribute<CustomComboInfoAttribute>();
                        text = text + "\nRequires " + attribute.FancyName;
                    }
                    ImGui.TextColored(shadedColor, text);
                    ImGui.Spacing();
                    if (conflicts.Length != 0)
                    {
                        string text2 = conflicts.Select(delegate (CustomComboPreset preset)
                        {
                            CustomComboInfoAttribute attribute2 = preset.GetAttribute<CustomComboInfoAttribute>();
                            return "\n - " + attribute2.FancyName;
                        }).Aggregate((t1, t2) => t1 + t2);
                        ImGui.TextColored(shadedColor, "Conflicts with: " + text2);
                        ImGui.Spacing();
                    }
                    if (item == CustomComboPreset.DancerDanceComboCompatibility && flag)
                    {
                        int[] array2 = Service.Configuration.DancerDanceCompatActionIDs.Cast<int>().ToArray();
                        if (false | ImGui.InputInt("Emboite (Red) ActionID", ref array2[0], 0) | ImGui.InputInt("Entrechat (Blue) ActionID", ref array2[1], 0) | ImGui.InputInt("Jete (Green) ActionID", ref array2[2], 0) | ImGui.InputInt("Pirouette (Yellow) ActionID", ref array2[3], 0))
                        {
                            Service.Configuration.DancerDanceCompatActionIDs = array2.Cast<uint>().ToArray();
                            Service.IconReplacer.UpdateEnabledActionIDs();
                            Service.Configuration.Save();
                        }
                        ImGui.Spacing();
                    }
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
