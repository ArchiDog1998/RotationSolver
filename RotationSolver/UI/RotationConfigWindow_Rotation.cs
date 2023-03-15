using Dalamud.Interface.Colors;
using Dalamud.Utility;
using ImGuiNET;
using RotationSolver.Basic;
using RotationSolver.Basic.Data;
using RotationSolver.Basic.Helpers;
using RotationSolver.Basic.Rotations;
using RotationSolver.Localization;
using RotationSolver.SigReplacers;
using RotationSolver.UI;
using RotationSolver.Updaters;
using System;
using System.Linq;
using System.Numerics;

namespace RotationSolver.Windows.RotationConfigWindow;

internal partial class RotationConfigWindow
{
    private void DrawRotationTab()
    {
        ImGui.TextWrapped(LocalizationManager.RightLang.Configwindow_Rotation_Description);

        if (ImGui.Button("Dev Wiki"))
        {
            Util.OpenLink("https://archidog1998.github.io/RotationSolver/#/RotationDev/");
        }
        ImGui.SameLine();
        if (ImGui.Button("Load Rotation"))
        {
            RotationUpdater.GetAllCustomRotations();
        }

        ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new Vector2(0f, 5f));

        if (ImGui.BeginTabBar("Job Items"))
        {
            DrawRoleItems();

            ImGui.EndTabBar();
        }
        ImGui.PopStyleVar();
    }

    private static void DrawRoleItems()
    {
        foreach (var key in RotationUpdater.CustomRotationsDict.Keys)
        {
            var rotations = RotationUpdater.CustomRotationsDict[key];
            if (rotations == null || rotations.Length == 0) continue;

            if (ImGui.BeginTabItem(key.ToName()))
            {
                if (ImGui.BeginChild("Rotation Items", new Vector2(0f, -1f), true))
                {
                    DrawRotations(rotations);
                    ImGui.EndChild();
                }
                ImGui.EndTabItem();
            }
        }
    }

    private static void DrawRotations(RotationUpdater.CustomRotationGroup[] rotations)
    {
        for (int i = 0; i < rotations.Length; i++)
        {
            if (i > 0) ImGui.Separator();

            var group = rotations[i];
            Service.Config.RotationChoices.TryGetValue((uint)group.jobId, out var rotationName);
            var rotation = RotationUpdater.GetChooseRotation(group, rotationName);

            var canAddButton = Service.Player != null
                && rotation.JobIDs.Contains((ClassJobID)Service.Player.ClassJob.Id);

            rotation.Display(group.rotations, canAddButton);
        }
    }

    internal static void DrawRotationRole(ICustomRotation rotation)
    {
        DrawTargetHostileTYpe(rotation);
        DrawSpecialRoleSettings(rotation.Job.GetJobRole(), rotation.JobIDs[0]);

        ImGui.Spacing();
    }

    private static void DrawTargetHostileTYpe(ICustomRotation rotation)
    {
        var isAllTargetAsHostile = (int)IconReplacer.GetTargetHostileType(rotation.Job);
        ImGui.SetNextItemWidth(300);
        if (ImGui.Combo(LocalizationManager.RightLang.Configwindow_Param_RightNowTargetToHostileType + $"##HostileType{rotation.GetHashCode()}", ref isAllTargetAsHostile, new string[]
        {
             LocalizationManager.RightLang.Configwindow_Param_TargetToHostileType1,
             LocalizationManager.RightLang.Configwindow_Param_TargetToHostileType2,
             LocalizationManager.RightLang.Configwindow_Param_TargetToHostileType3,
        }, 3))
        {
            Service.Config.TargetToHostileTypes[rotation.Job.RowId] = (byte)isAllTargetAsHostile;
            Service.Config.Save();
        }

        if (isAllTargetAsHostile != 2 && !Service.Config.AutoOffBetweenArea)
        {
            ImGui.TextColored(ImGuiColors.DPSRed, LocalizationManager.RightLang.Configwindow_Param_NoticeUnexpectedCombat);
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
            DrawDragFloat(job, LocalizationManager.RightLang.Configwindow_Param_HealthForDyingTank,
                () => ConfigurationHelper.GetHealthForDyingTank(job),
                (value) => Service.Config.HealthForDyingTanks[job] = value);
        }
    }

    private static void DrawHealerSettings(ClassJobID job)
    {
        DrawDragFloat(job, LocalizationManager.RightLang.Configwindow_Param_HealthAreaAbility,
            () => ConfigurationHelper.GetHealAreaAbility(job),
            (value) => Service.Config.HealthAreaAbilities[job] = value);

        DrawDragFloat(job, LocalizationManager.RightLang.Configwindow_Param_HealthAreaSpell,
            () => ConfigurationHelper.GetHealAreaSpell(job),
            (value) => Service.Config.HealthAreaSpells[job] = value);

        DrawDragFloat(job, LocalizationManager.RightLang.Configwindow_Param_HealingOfTimeSubtractArea,
            () => ConfigurationHelper.GetHealingOfTimeSubtractArea(job),
            (value) => Service.Config.HealingOfTimeSubtractAreas[job] = value);

        DrawDragFloat(job, LocalizationManager.RightLang.Configwindow_Param_HealthSingleAbility,
            () => ConfigurationHelper.GetHealSingleAbility(job),
            (value) => Service.Config.HealthSingleAbilities[job] = value);

        DrawDragFloat(job, LocalizationManager.RightLang.Configwindow_Param_HealthSingleSpell,
            () => ConfigurationHelper.GetHealSingleSpell(job),
            (value) => Service.Config.HealthSingleSpells[job] = value);

        DrawDragFloat(job, LocalizationManager.RightLang.Configwindow_Param_HealingOfTimeSubtractSingle,
            () => ConfigurationHelper.GetHealingOfTimeSubtractSingle(job),
            (value) => Service.Config.HealingOfTimeSubtractSingles[job] = value);
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
            Service.Config.Save();
        }
    }
}
