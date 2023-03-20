using ImGuiNET;
using RotationSolver.Basic;
using RotationSolver.Localization;
using System.Numerics;

namespace RotationSolver.UI;

internal partial class RotationConfigWindow
{
    private void DrawControlTab()
    {
        DrawCheckBox(LocalizationManager.RightLang.ConfigWindow_Control_UseGamepadCommand,
            ref Service.Config.UseGamepadCommand);


        DrawCheckBox(LocalizationManager.RightLang.ConfigWindow_Control_UseKeyboardCommand,
            ref Service.Config.UseKeyboardCommand);

        ImGui.Separator();

        DrawCheckBox(LocalizationManager.RightLang.ConfigWindow_Control_ShowControlWindow,
            ref Service.Config.ShowControlWindow);

        if (!Service.Config.ShowControlWindow) return;

        DrawCheckBox(LocalizationManager.RightLang.ConfigWindow_Control_IsControlWindowLock,
            ref Service.Config.IsControlWindowLock);

        DrawCheckBox(LocalizationManager.RightLang.ConfigWindow_Control_ControlShowCooldown,
            ref Service.Config.ControlShowCooldown);

        if (Service.Config.IsControlWindowLock)
        {
            DrawColor4(LocalizationManager.RightLang.ConfigWindow_Control_BackgroundColor,
                ref Service.Config.ControlWindowLockBg);
        }
        else
        {
            DrawColor4(LocalizationManager.RightLang.ConfigWindow_Control_BackgroundColor,
                ref Service.Config.ControlWindowUnlockBg);
        }

        DrawFloatNumber(LocalizationManager.RightLang.ConfigWindow_Control_ControlWindowGCDSize,
            ref Service.Config.ControlWindowGCDSize, speed: 0.2f , max: 80);

        DrawFloatNumber(LocalizationManager.RightLang.ConfigWindow_Control_ControlWindow0GCDSize,
            ref Service.Config.ControlWindow0GCDSize, speed: 0.2f, max: 80);


        DrawFloatNumber(LocalizationManager.RightLang.ConfigWindow_Control_ControlWindowNextSizeRatio,
            ref Service.Config.ControlWindowNextSizeRatio);
    }

    private static void DrawColor4(string name, ref Vector4 value, string description = "")
    {
        ImGui.SetNextItemWidth(210);
        if (ImGui.ColorEdit4(name, ref value))
        {
            Service.Config.Save();
        }
        if (!string.IsNullOrEmpty(description) && ImGui.IsItemHovered())
        {
            ImGui.SetTooltip(description);
        }
    }
}
