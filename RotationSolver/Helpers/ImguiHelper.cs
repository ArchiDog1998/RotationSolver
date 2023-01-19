using ImGuiNET;
using RotationSolver.Commands;
using RotationSolver.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RotationSolver.Helpers
{
    internal static class ImGuiHelper
    {
        internal static void Spacing(byte count = 1)
        {
            string s = string.Empty;
            for (int i = 0; i < count; i++)
            {
                s += "    ";
            }
            ImGui.Text(s);
            ImGui.SameLine();
        }
    }
}
