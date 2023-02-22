using ImGuiNET;
using RotationSolver.Helpers;
using RotationSolver.Localization;
using System;

namespace RotationSolver.Commands;

internal static partial class RSCommands
{
    internal static void DisplayCommandHelp<T>(this T command, string extraCommand = "", Func<T, string> getHelp = null, bool sameLine = true) where T : struct, Enum
    {
        var cmdStr = _command + " " + command.ToString();
        if (!string.IsNullOrEmpty(extraCommand))
        {
            cmdStr += " " + extraCommand;
        }

        if (ImGui.Button(cmdStr))
        {
            Service.CommandManager.ProcessCommand(cmdStr);
        }
        if (ImGui.IsItemHovered())
        {
            ImGui.SetTooltip($"{LocalizationManager.RightLang.Configwindow_Helper_RunCommand}: {cmdStr}\n{LocalizationManager.RightLang.Configwindow_Helper_CopyCommand}: {cmdStr}");

            if (ImGui.IsMouseDown(ImGuiMouseButton.Right))
            {
                ImGui.SetClipboardText(cmdStr);
            }
        }

        if (getHelp != null)
        {
            var help = getHelp(command);

            if (!string.IsNullOrEmpty(help))
            {
                if (sameLine) ImGui.SameLine();
                else ImGuiHelper.Spacing();
                ImGui.Text(" → ");
                ImGui.SameLine();
                ImGui.TextWrapped(help);
            }
        }
    }
}
