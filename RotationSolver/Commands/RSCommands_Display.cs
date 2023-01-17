using ImGuiNET;
using RotationSolver.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RotationSolver.Commands;

internal static partial class RSCommands
{
    internal static void DisplayCommandHelp<T>(this T command, Func<T, string> getHelp = null) where T : struct, Enum
    {
        var cmdStr = _command + " " + command.ToString();
        if (ImGui.Button(cmdStr))
        {
            Service.CommandManager.ProcessCommand(cmdStr);
        }
        if (ImGui.IsItemHovered())
        {
            ImGui.SetTooltip($"{LocalizationManager.RightLang.Configwindow_Helper_RunCommand}: {command}\n{LocalizationManager.RightLang.Configwindow_Helper_CopyCommand}: {command}");

            if (ImGui.IsMouseDown(ImGuiMouseButton.Right))
            {
                ImGui.SetClipboardText(cmdStr);
            }
        }

        if (getHelp!= null)
        {
            var help = getHelp(command);

            if(!string.IsNullOrEmpty(help))
            {
                ImGui.SameLine();
                ImGui.Text(" → " + help);
            }
        }
    }
}
