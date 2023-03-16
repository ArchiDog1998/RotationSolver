using Dalamud.Interface.Colors;
using Dalamud.Utility;
using ImGuiNET;
using RotationSolver.Basic;
using RotationSolver.Localization;
using RotationSolver.Updaters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RotationSolver.Windows.RotationConfigWindow;

internal partial class RotationConfigWindow
{
    private void DrawRotationDevTab()
    {
        ImGui.TextWrapped(LocalizationManager.RightLang.ConfigWindow_RotationDev_Description);

        if (ImGui.Button("Load Rotations"))
        {
            RotationUpdater.GetAllCustomRotations();
        }

        ImGui.SameLine();
        if (ImGui.Button("Dev Wiki"))
        {
            Util.OpenLink("https://archidog1998.github.io/RotationSolver/#/RotationDev/");
        }

        ImGui.SameLine();

        DrawCheckBox(LocalizationManager.RightLang.ConfigWindow_Param_InDebug,
            ref Service.Config.InDebug);

        if (Service.Config.InDebug)
        {
            ImGui.TextColored(ImGuiColors.DalamudRed,
                LocalizationManager.RightLang.ConfigWindow_Param_InDebugWarning);
        }

        int removeIndex = -1;
        for (int i = 0; i < Service.Config.OtherLibs.Length; i++)
        {
            ImGui.InputText($"##OtherLib{i}", ref Service.Config.OtherLibs[i], 1024);
            ImGui.SameLine();
            if (ImGui.Button($"X##Remove{i}"))
            {
                removeIndex = i;
            }
        }
        if(removeIndex > -1)
        {
            var list = Service.Config.OtherLibs.ToList();
            list.RemoveAt(removeIndex);
            Service.Config.OtherLibs = list.ToArray();
        }

        string str = string.Empty;
        if(ImGui.InputText($"##OtherLibExtra", ref str, 1024))
        {
            Service.Config.OtherLibs = Service.Config.OtherLibs.Append(str).ToArray();
        }
    }
}
