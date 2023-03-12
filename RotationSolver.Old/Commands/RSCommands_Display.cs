using ImGuiNET;
using RotationSolver.Helpers;
using RotationSolver.Localization;
using System;

namespace RotationSolver.Commands;

internal static partial class RSCommands
{
    const float INDENT_WIDTH = 180;
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

        var help = getHelp?.Invoke(command);

        if (!string.IsNullOrEmpty(help))
        {
            if (sameLine)
            {
                ImGui.SameLine();
                ImGui.Indent(INDENT_WIDTH);
            }
            else ImGuiHelper.Spacing();
            ImGui.Text(" → ");
            ImGui.SameLine();
            ImGui.TextWrapped(help);
            if (sameLine)
            {
                ImGui.Unindent(INDENT_WIDTH);
            }
        }
    }
}
