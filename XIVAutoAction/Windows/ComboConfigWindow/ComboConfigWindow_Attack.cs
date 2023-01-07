﻿using Dalamud.Interface;
using Dalamud.Interface.Colors;
using Dalamud.Interface.Components;
using ImGuiNET;
using System;
using System.IO;
using System.Linq;
using System.Numerics;
using AutoAction.Data;
using AutoAction.Localization;
using AutoAction.SigReplacers;

namespace AutoAction.Windows.ComboConfigWindow;

internal partial class ComboConfigWindow
{
    private void DrawAttack()
    {
        ImGui.Text(LocalizationManager.RightLang.Configwindow_AttackItem_Description);

        string folderLocation = Service.Configuration.ScriptComboFolder;
        if (ImGui.InputText(LocalizationManager.RightLang.Configwindow_AttackItem_ScriptFolder, ref folderLocation, 256))
        {
            Service.Configuration.ScriptComboFolder = folderLocation;
            Service.Configuration.Save();
        }

        ImGui.SameLine();
        Spacing();

        if (ImGuiComponents.IconButton(FontAwesomeIcon.FolderOpen))
        {
            IconReplacer.LoadFromFolder();
        }
        if (ImGui.IsItemHovered())
        {
            ImGui.SetTooltip(LocalizationManager.RightLang.Configwindow_AttackItem_LoadScript);
        }

        if (!Directory.Exists(Service.Configuration.ScriptComboFolder))
        {
            ImGui.TextColored(ImGuiColors.DalamudRed,
                LocalizationManager.RightLang.Configwindow_AttackItem_ScriptFolderError);
        }

        ImGui.BeginChild("Attack Items", new Vector2(0f, -1f), true);
        ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new Vector2(0f, 5f));
        int num = 1;

        if (ImGui.BeginTabBar("Job Items"))
        {
            foreach (var key in IconReplacer.CustomCombosDict.Keys)
            {
                var combos = IconReplacer.CustomCombosDict[key];
                if (combos == null || combos.Length == 0) continue;

                if (ImGui.BeginTabItem(key.ToName()))
                {
                    if (ImGui.IsItemHovered() && _roleDescriptionValue.TryGetValue(key, out string roleDesc))
                    {
                        ImGui.SetTooltip(roleDesc);
                    }
                    for (int i = 0; i < combos.Length; i++)
                    {
                        if (i > 0) ImGui.Separator();
                        var combo = IconReplacer.GetChooseCombo(combos[i]);
                        var canAddButton = Service.ClientState.LocalPlayer != null && combo.JobIDs.Contains((ClassJobID)Service.ClientState.LocalPlayer.ClassJob.Id);

                        if (ImGui.CollapsingHeader(combo.Name))
                        {
                            DrawTexture(combo, () =>
                            {
                                ImGui.Spacing();
                                ImGui.Spacing();
                                int isAllTargetAsHostile = IconReplacer.GetTargetHostileType(combo.Job);
                                ImGui.SetNextItemWidth(300);
                                if (ImGui.Combo(LocalizationManager.RightLang.Configwindow_Params_RightNowTargetToHostileType + $"##HostileType{num}", ref isAllTargetAsHostile, new string[]
                                {
                                        LocalizationManager.RightLang.Configwindow_Params_TargetToHostileType1,
                                        LocalizationManager.RightLang.Configwindow_Params_TargetToHostileType2,
                                        LocalizationManager.RightLang.Configwindow_Params_TargetToHostileType3,
                                }, 3))
                                {
                                    Service.Configuration.TargetToHostileTypes[combo.Job.RowId] = (byte)isAllTargetAsHostile;
                                    Service.Configuration.Save();
                                }

                                var role = combo.Job.GetJobRole();
                                const float speed = 0.005f;
                                var job = combo.JobIDs[0];

                                if (role == JobRole.Healer)
                                {

                                    var healAreability = Service.Configuration.HealthAreaAbilitys.TryGetValue(job, out var value) ? value : Service.Configuration.HealthAreaAbility;

                                    ImGui.SetNextItemWidth(DRAG_NUMBER_WIDTH);
                                    if (ImGui.DragFloat(LocalizationManager.RightLang.Configwindow_Params_HealthAreaAbility + $"##{num}HealAreaAbility", ref healAreability, speed, 0, 1))
                                    {
                                        Service.Configuration.HealthAreaAbilitys[job] = healAreability;
                                        Service.Configuration.Save();
                                    }

                                    ImGui.SetNextItemWidth(DRAG_NUMBER_WIDTH);
                                    var healAreaspell = Service.Configuration.HealthAreafSpells.TryGetValue(job, out value) ? value : Service.Configuration.HealthAreafSpell; if (ImGui.DragFloat(LocalizationManager.RightLang.Configwindow_Params_HealthAreafSpell + $"##{num}HealAreaSpell", ref healAreaspell, speed, 0, 1))
                                    {
                                        Service.Configuration.HealthAreafSpells[job] = healAreaspell;
                                        Service.Configuration.Save();
                                    }

                                    ImGui.SetNextItemWidth(DRAG_NUMBER_WIDTH);
                                    var hotSubArea = Service.Configuration.HealingOfTimeSubtractAreas.TryGetValue(job, out value) ? value : 0.3f;
                                    if (ImGui.DragFloat(LocalizationManager.RightLang.Configwindow_Params_HealingOfTimeSubtractArea + $"##{num}HealAreaSubtract", ref hotSubArea, speed, 0, 1))
                                    {
                                        Service.Configuration.HealingOfTimeSubtractAreas[job] = hotSubArea;
                                        Service.Configuration.Save();
                                    }

                                    ImGui.SetNextItemWidth(DRAG_NUMBER_WIDTH);
                                    var healsingAbility = Service.Configuration.HealthSingleAbilitys.TryGetValue(job, out value) ? value : Service.Configuration.HealthSingleAbility;
                                    if (ImGui.DragFloat(LocalizationManager.RightLang.Configwindow_Params_HealthSingleAbility + $"##{num}HealSingleAbility", ref healsingAbility, speed, 0, 1))
                                    {
                                        Service.Configuration.HealthSingleAbilitys[job] = healsingAbility;
                                        Service.Configuration.Save();
                                    }

                                    ImGui.SetNextItemWidth(DRAG_NUMBER_WIDTH);
                                    var healsingSpell = Service.Configuration.HealthSingleSpells.TryGetValue(job, out value) ? value : Service.Configuration.HealthSingleSpell;
                                    if (ImGui.DragFloat(LocalizationManager.RightLang.Configwindow_Params_HealthSingleSpell + $"##{num}HealSingleSpell", ref healsingSpell, speed, 0, 1))
                                    {
                                        Service.Configuration.HealthSingleSpells[job] = healsingSpell;
                                        Service.Configuration.Save();
                                    }

                                    ImGui.SetNextItemWidth(DRAG_NUMBER_WIDTH);
                                    var hotSubSingle = Service.Configuration.HealingOfTimeSubtractSingles.TryGetValue(job, out value) ? value : 0.3f;
                                    if (ImGui.DragFloat(LocalizationManager.RightLang.Configwindow_Params_HealingOfTimeSubtractSingle + $"##{num}HealSingleSubtract", ref hotSubSingle, speed, 0, 1))
                                    {
                                        Service.Configuration.HealingOfTimeSubtractSingles[job] = hotSubSingle;
                                        Service.Configuration.Save();
                                    }
                                }
                                else if (role == JobRole.Tank)
                                {
                                    float healthTank = Service.Configuration.HealthForDyingTanks.TryGetValue(job, out var value) ? value : 0.15f;
                                    ImGui.SetNextItemWidth(DRAG_NUMBER_WIDTH);
                                    if (ImGui.DragFloat(LocalizationManager.RightLang.Configwindow_Params_HealthForDyingTank + $"##{num}HealthForDyingTank", ref healthTank, speed, 0, 1))
                                    {
                                        Service.Configuration.HealthForDyingTanks[job] = healthTank;
                                        Service.Configuration.Save();
                                    }
                                }
                                ImGui.Spacing();
                                var actions = combo.Config;
                                foreach (var boolean in actions.bools)
                                {
                                    bool val = boolean.value;
                                    if (ImGui.Checkbox($"{boolean.description}##{num}_{boolean.description}", ref val))
                                    {
                                        boolean.value = val;
                                        Service.Configuration.Save();
                                    }
                                    if (ImGui.IsItemHovered())
                                    {
                                        ImGui.SetTooltip(LocalizationManager.RightLang.Configwindow_AttackItem_KeyName + ": " + boolean.name);
                                    }

                                    //显示可以设置的案件
                                    if (canAddButton)
                                    {
                                        ImGui.SameLine();
                                        Spacing();
                                        CommandHelp(boolean.name);
                                    }
                                }
                                foreach (var doubles in actions.doubles)
                                {
                                    float val = doubles.value;
                                    ImGui.SetNextItemWidth(DRAG_NUMBER_WIDTH);
                                    if (ImGui.DragFloat($"{doubles.description}##{num}_{doubles.description}", ref val, doubles.speed, doubles.min, doubles.max))
                                    {
                                        doubles.value = val;
                                        Service.Configuration.Save();
                                    }
                                }
                                foreach (var textItem in actions.texts)
                                {
                                    string val = textItem.value;
                                    if (ImGui.InputText($"{textItem.description}##{num}_{textItem.description}", ref val, 15))
                                    {
                                        textItem.value = val;
                                        Service.Configuration.Save();
                                    }
                                }
                                foreach (var comboItem in actions.combos)
                                {
                                    if (ImGui.BeginCombo($"{comboItem.description}##{num}_{comboItem.description}", comboItem.items[comboItem.value]))
                                    {
                                        for (int comboIndex = 0; comboIndex < comboItem.items.Length; comboIndex++)
                                        {
                                            if (ImGui.Selectable(comboItem.items[comboIndex]))
                                            {
                                                comboItem.value = comboIndex;
                                                Service.Configuration.Save();
                                            }
                                            if (canAddButton)
                                            {
                                                ImGui.SameLine();
                                                Spacing();
                                                CommandHelp(comboItem.name + comboIndex.ToString());
                                            }
                                        }
                                        ImGui.EndCombo();
                                    }
                                    if (ImGui.IsItemHovered())
                                    {
                                        ImGui.SetTooltip(LocalizationManager.RightLang.Configwindow_AttackItem_KeyName + ": " + comboItem.name);
                                    }

                                    //显示可以设置的案件
                                    if (canAddButton)
                                    {
                                        ImGui.SameLine();
                                        Spacing();
                                        CommandHelp(comboItem.name);
                                    }
                                }

                                if (canAddButton)
                                {
                                    ImGui.NewLine();

                                    foreach (var customCMD in combo.CommandShow)
                                    {
                                        Spacing();
                                        CommandHelp(customCMD.Key, customCMD.Value);
                                    }
                                }
                            }, combo.JobIDs[0], combos[i].combos);
                        }
                        num++;
                    }
                    ImGui.EndTabItem();
                }
                else
                {
                    if (ImGui.IsItemHovered() && _roleDescriptionValue.TryGetValue(key, out string roleDesc))
                    {
                        ImGui.SetTooltip(roleDesc);
                    }
                    num += combos.Length;
                }

            }
            ImGui.EndTabBar();
        }
        ImGui.PopStyleVar();
        ImGui.EndChild();
    }
}
