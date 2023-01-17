using Dalamud.Interface;
using Dalamud.Interface.Colors;
using Dalamud.Interface.Components;
using ImGuiNET;
using RotationSolver;
using RotationSolver.Combos.CustomCombo;
using RotationSolver.Configuration;
using RotationSolver.Data;
using RotationSolver.Helpers;
using RotationSolver.Localization;
using RotationSolver.SigReplacers;
using System;
using System.IO;
using System.Linq;
using System.Numerics;

namespace RotationSolver.Windows.ComboConfigWindow;

internal partial class ComboConfigWindow
{
    private void DrawAttack()
    {
        ImGui.Text(LocalizationManager.RightLang.Configwindow_AttackItem_Description);

        ImGui.BeginChild("Attack Items", new Vector2(0f, -1f), true);
        ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new Vector2(0f, 5f));

        if (ImGui.BeginTabBar("Job Items"))
        {
            DrawRoleItems();
            ImGui.EndTabBar();
        }
        ImGui.PopStyleVar();
        ImGui.EndChild();
    }

    private static void DrawRoleItems()
    {
        foreach (var key in IconReplacer.CustomRotationsDict.Keys)
        {
            var rotations = IconReplacer.CustomRotationsDict[key];
            if (rotations == null || rotations.Length == 0) continue;

            if (ImGui.BeginTabItem(key.ToName()))
            {
                DrawRotations(rotations);
                ImGui.EndTabItem();
            }

            //Display the tooltip on the header.
            if (ImGui.IsItemHovered() && _roleDescriptionValue.TryGetValue(key, out string roleDesc))
            {
                ImGui.SetTooltip(roleDesc);
            }
        }
    }

    private static void DrawRotations(IconReplacer.CustomRotationGroup[] rotations)
    {
        for (int i = 0; i < rotations.Length; i++)
        {
            if (i > 0) ImGui.Separator();

            var rotation = IconReplacer.GetChoosedRotation(rotations[i]);

            if (ImGui.CollapsingHeader(rotation.Name))
            {
                var canAddButton = Service.ClientState.LocalPlayer != null 
                    && rotation.JobIDs.Contains((ClassJobID)Service.ClientState.LocalPlayer.ClassJob.Id);

                DrawTexture(rotation, () =>
                {
                    DrawRotation(rotation, canAddButton);
                }, rotation.JobIDs[0], rotations[i].combos);
            }
        }
    }


    private static void DrawRotation(ICustomRotation rotation, bool canAddButton)
    {
        ImGui.Spacing();

        DrawTargetHostileTYpe(rotation);
        DrawSpecialRoleSettings(rotation.Job.GetJobRole(), rotation.JobIDs[0]);

        ImGui.Spacing();

        DrawConfig(rotation.Config);
    }

    private static void DrawTargetHostileTYpe(ICustomRotation rotation)
    {
        var isAllTargetAsHostile = (int)IconReplacer.GetTargetHostileType(rotation.Job);
        ImGui.SetNextItemWidth(300);
        if (ImGui.Combo(LocalizationManager.RightLang.Configwindow_Params_RightNowTargetToHostileType + $"##HostileType{rotation.GetHashCode()}", ref isAllTargetAsHostile, new string[]
        {
             LocalizationManager.RightLang.Configwindow_Params_TargetToHostileType1,
             LocalizationManager.RightLang.Configwindow_Params_TargetToHostileType2,
             LocalizationManager.RightLang.Configwindow_Params_TargetToHostileType3,
        }, 3))
        {
            Service.Configuration.TargetToHostileTypes[rotation.Job.RowId] = (byte)isAllTargetAsHostile;
            Service.Configuration.Save();
        }
    }

    private static void DrawSpecialRoleSettings(JobRole role, ClassJobID job)
    {
        if (role == JobRole.Healer)
        {
            DrawHealerSettings(job);
        }
        else if (role == JobRole.Tank)
        {
            DrawDragFloat(job, LocalizationManager.RightLang.Configwindow_Params_HealthForDyingTank,
                () => ConfigurationHelper.GetHealthForDyingTank(job),
                (value) => Service.Configuration.HealthForDyingTanks[job] = value);
        }
    }

    private static void DrawHealerSettings(ClassJobID job)
    {
        DrawDragFloat(job, LocalizationManager.RightLang.Configwindow_Params_HealthAreaAbility,
            () => ConfigurationHelper.GetHealAreaAbility(job),
            (value) => Service.Configuration.HealthAreaAbilities[job] = value);

        DrawDragFloat(job, LocalizationManager.RightLang.Configwindow_Params_HealthAreafSpell,
            () => ConfigurationHelper.GetHealAreafSpell(job),
            (value) => Service.Configuration.HealthAreafSpells[job] = value);

        DrawDragFloat(job, LocalizationManager.RightLang.Configwindow_Params_HealingOfTimeSubtractArea,
            () => ConfigurationHelper.GetHealingOfTimeSubtractArea(job),
            (value) => Service.Configuration.HealingOfTimeSubtractAreas[job] = value);

        DrawDragFloat(job, LocalizationManager.RightLang.Configwindow_Params_HealthSingleAbility,
            () => ConfigurationHelper.GetHealSingleAbility(job),
            (value) => Service.Configuration.HealthSingleAbilities[job] = value);

        DrawDragFloat(job, LocalizationManager.RightLang.Configwindow_Params_HealthSingleSpell,
            () => ConfigurationHelper.GetHealSingleSpell(job),
            (value) => Service.Configuration.HealthSingleSpells[job] = value);

        DrawDragFloat(job, LocalizationManager.RightLang.Configwindow_Params_HealingOfTimeSubtractSingle,
            () => ConfigurationHelper.GetHealingOfTimeSubtractSingle(job),
            (value) => Service.Configuration.HealingOfTimeSubtractSingles[job] = value);

    }

    private static void DrawConfig(ActionConfiguration actions)
    {
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
                //CommandHelp(boolean.name);
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
                        //CommandHelp(comboItem.name + comboIndex.ToString());
                    }
                }
                ImGui.EndCombo();
            }
            if (ImGui.IsItemHovered())
            {
                ImGui.SetTooltip(LocalizationManager.RightLang.Configwindow_AttackItem_KeyName + ": " + comboItem.name);
            }

            //显示可以设置的按键
            if (canAddButton)
            {
                ImGui.SameLine();
                Spacing();
                //CommandHelp(comboItem.name);
            }
        }
    }

    private static void DrawDragFloat(ClassJobID job, string desc, Func<float> getValue, Action<float> setValue)
    {
        const float speed = 0.005f;

        if (getValue == null || setValue == null) return;

        var value = getValue();
        ImGui.SetNextItemWidth(DRAG_NUMBER_WIDTH);
        if (ImGui.DragFloat($"{desc}##{job}{desc}", ref value, speed, 0, 1))
        {
            setValue(value);
            Service.Configuration.Save();
        }
    }

}
