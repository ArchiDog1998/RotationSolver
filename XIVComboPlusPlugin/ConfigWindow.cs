using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;
using Dalamud.Game.ClientState.Objects.Types;
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
    private readonly Vector4 shadedColor = new Vector4(0.68f, 0.68f, 0.68f, 1f);

    public ConfigWindow()
        : base("自定义连击设置", 0, false)
    {
        RespectCloseHotkey = true;

        SizeCondition = (ImGuiCond)4;
        Size = new Vector2(740f, 490f);
    }

    public override void Draw()
    {
        ImGui.Text("在这个窗口，你可以设定自己喜欢的连击设定。");

        bool enableSecretCombos = Service.Configuration.EnableSecretCombos;
        if (ImGui.Checkbox("可以使用一些禁忌的知识", ref enableSecretCombos))
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

        foreach (uint key in IconReplacer.CustomCombosDict.Keys)
        {
            var combos = IconReplacer.CustomCombosDict[key];
            if (combos == null || combos.Length == 0) continue;

            string jobName = combos[0].JobName;
            if (ImGui.CollapsingHeader(jobName))
            {
                foreach (var combo in combos)
                {
                    //ImGui.Text(combo.ComboFancyName);

                    bool enable = combo.IsEnabled;
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
                            combo.IsEnabled = true;
                            string[] array = conflicts;
                            foreach (string item3 in array)
                            {
                                IconReplacer.SetEnable(item3, false);
                            }
                        }
                        else
                        {
                            combo.IsEnabled = false;
                        }
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
                num += combos.Length;
            }
        }
        ImGui.PopStyleVar();
        ImGui.EndChild();
    }
}
