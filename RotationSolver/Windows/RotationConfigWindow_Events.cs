using ImGuiNET;
using Lumina.Excel.GeneratedSheets;
using RotationSolver.Configuration;
using RotationSolver.Helpers;
using RotationSolver.Localization;
using System.Numerics;

namespace RotationSolver.Windows.RotationConfigWindow;

internal partial class RotationConfigWindow
{
    private void DrawEventTab()
    {
        if (ImGui.Button(LocalizationManager.RightLang.Configwindow_Events_AddEvent))
        {
            Service.Configuration.Events.Add(new ActionEventInfo());
            Service.Configuration.Save();
        }
        ImGui.SameLine();
        ImGuiHelper.Spacing();

        ImGui.TextWrapped(LocalizationManager.RightLang.Configwindow_Events_Description);

        ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new Vector2(0f, 5f));

#if DEBUG
        ImGui.Text(LocalizationManager.RightLang.Configwindow_Events_DutyStart);
        ImGui.SameLine();
        ImGuiHelper.Spacing();
        Service.Configuration.DutyStart.DisplayMacro();

        ImGui.Text(LocalizationManager.RightLang.Configwindow_Events_DutyEnd);
        ImGui.SameLine();
        ImGuiHelper.Spacing();
        Service.Configuration.DutyEnd.DisplayMacro();
#endif

        if (ImGui.BeginChild("Events List", new Vector2(0f, -1f), true))
        {
            ActionEventInfo remove = null;
            foreach (var eve in Service.Configuration.Events)
            {
                eve.DisplayMacro();

                ImGui.SameLine();
                ImGuiHelper.Spacing();

                if (ImGui.Button($"{LocalizationManager.RightLang.Configwindow_Events_RemoveEvent}##RemoveEvent{eve.GetHashCode()}"))
                {
                    remove = eve;
                }
                ImGui.Separator();
            }
            if(remove!= null)
            {
                Service.Configuration.Events.Remove(remove);
                Service.Configuration.Save();
            }

            ImGui.EndChild();
        }
        ImGui.PopStyleVar();
    }
}
