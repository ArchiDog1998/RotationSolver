using RotationSolver.Localization;

namespace RotationSolver.UI;

internal partial class RotationConfigWindow
{
    private static void DrawControlTab()
    {
        ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new Vector2(0f, 5f));

        if (ImGui.BeginChild("Action List", new Vector2(0f, -1f), true))
        {
            DrawCheckBox(LocalizationManager.RightLang.ConfigWindow_Control_UseGamepadCommand,
            ref Service.Config.UseGamepadCommand, Service.Default.UseGamepadCommand);

            DrawCheckBox(LocalizationManager.RightLang.ConfigWindow_Control_UseKeyboardCommand,
                ref Service.Config.UseKeyboardCommand, Service.Default.UseKeyboardCommand);

            ImGui.Separator();

            if (Service.Config.ShowNextActionWindow || Service.Config.ShowCooldownWindow || Service.Config.ShowControlWindow)
            {
                DrawCheckBox(LocalizationManager.RightLang.ConfigWindow_Control_OnlyShowWithHostileOrInDuty,
                    ref Service.Config.OnlyShowWithHostileOrInDuty, Service.Default.OnlyShowWithHostileOrInDuty);
            }

            DrawCheckBox(LocalizationManager.RightLang.ConfigWindow_Control_ShowNextActionWindow,
                ref Service.Config.ShowNextActionWindow, Service.Default.ShowNextActionWindow);

            DrawCheckBox(LocalizationManager.RightLang.ConfigWindow_Control_ShowCooldownWindow,
                ref Service.Config.ShowCooldownWindow, Service.Default.ShowCooldownWindow);

            if (Service.Config.ShowNextActionWindow || Service.Config.ShowCooldownWindow)
            {
                DrawColor4(LocalizationManager.RightLang.ConfigWindow_Control_InfoWindowBg,
                    ref Service.Config.InfoWindowBg, Service.Default.InfoWindowBg);

                DrawCheckBox(LocalizationManager.RightLang.ConfigWindow_Control_IsInfoWindowNoInputs,
                    ref Service.Config.IsInfoWindowNoInputs, Service.Default.IsInfoWindowNoInputs);

                DrawCheckBox(LocalizationManager.RightLang.ConfigWindow_Control_IsInfoWindowNoMove,
                    ref Service.Config.IsInfoWindowNoMove, Service.Default.IsInfoWindowNoMove);

                if (Service.Config.ShowCooldownWindow)
                {
                    DrawCheckBox(LocalizationManager.RightLang.ConfigWindow_Control_ShowItemsCooldown,
                        ref Service.Config.ShowItemsCooldown, Service.Default.ShowItemsCooldown);

                    DrawCheckBox(LocalizationManager.RightLang.ConfigWindow_Control_ShowGCDCooldown,
                        ref Service.Config.ShowGCDCooldown, Service.Default.ShowGCDCooldown);

                    DrawCheckBox(LocalizationManager.RightLang.ConfigWindow_Control_UseOriginalCooldown,
                        ref Service.Config.UseOriginalCooldown, Service.Default.UseOriginalCooldown);

                    DrawIntNumber(LocalizationManager.RightLang.ConfigWindow_Control_CooldownActionOneLine, ref Service.Config.CooldownActionOneLine, Service.Default.CooldownActionOneLine, min: 1, max: 30);

                    DrawFloatNumber(LocalizationManager.RightLang.ConfigWindow_Control_CooldownFontSize, ref Service.Config.CooldownFontSize, Service.Default.CooldownFontSize, speed: 0.1f, min: 9.6f, max: 96);

                    DrawFloatNumber(LocalizationManager.RightLang.ConfigWindow_Control_CooldownWindowIconSize,
                        ref Service.Config.CooldownWindowIconSize, Service.Default.CooldownWindowIconSize, speed: 0.2f, max: 80);
                }
            }

            ImGui.Separator();

            DrawCheckBox(LocalizationManager.RightLang.ConfigWindow_Control_ShowControlWindow,
                ref Service.Config.ShowControlWindow, Service.Default.ShowControlWindow);

            if (Service.Config.ShowControlWindow)
            {
                DrawCheckBox(LocalizationManager.RightLang.ConfigWindow_Control_IsInfoWindowNoInputs,
                    ref Service.Config.IsControlWindowLock, Service.Default.IsControlWindowLock);

                if (Service.Config.IsControlWindowLock)
                {
                    DrawColor4(LocalizationManager.RightLang.ConfigWindow_Control_BackgroundColor,
                        ref Service.Config.ControlWindowLockBg, Service.Default.ControlWindowLockBg);
                }
                else
                {
                    DrawColor4(LocalizationManager.RightLang.ConfigWindow_Control_BackgroundColor,
                        ref Service.Config.ControlWindowUnlockBg, Service.Default.ControlWindowUnlockBg);
                }

                DrawFloatNumber(LocalizationManager.RightLang.ConfigWindow_Control_ControlWindowGCDSize,
                    ref Service.Config.ControlWindowGCDSize, Service.Default.ControlWindowGCDSize, speed: 0.2f, max: 80);

                DrawFloatNumber(LocalizationManager.RightLang.ConfigWindow_Control_ControlWindow0GCDSize,
                    ref Service.Config.ControlWindow0GCDSize, Service.Default.ControlWindow0GCDSize, speed: 0.2f, max: 80);

                DrawFloatNumber(LocalizationManager.RightLang.ConfigWindow_Control_ControlWindowNextSizeRatio,
                    ref Service.Config.ControlWindowNextSizeRatio, Service.Default.ControlWindowNextSizeRatio);
            }

            ImGui.EndChild();
        }
        ImGui.PopStyleVar();
    }

    private static void DrawColor4(string name, ref Vector4 value, Vector4 @default, string description = "")
    {
        ImGui.SetNextItemWidth(210);
        if (ImGui.ColorEdit4(name, ref value))
        {
            Service.Config.Save();
        }
        if (ImGuiHelper.HoveredStringReset(description) && value != @default)
        {
            value = @default;
            Service.Config.Save();
        }
    }
}
