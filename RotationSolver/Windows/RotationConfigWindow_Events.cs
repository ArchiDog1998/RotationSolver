using ImGuiNET;
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

        if (ImGui.BeginChild("Events List", new Vector2(0f, -1f), true))
        {
            for (int i = 0; i < Service.Configuration.Events.Count; i++)
            {
                string name = Service.Configuration.Events[i].Name;
                if (ImGui.InputText($"{LocalizationManager.RightLang.Configwindow_Events_ActionName}##ActionName{i}",
                    ref name, 50))
                {
                    Service.Configuration.Events[i].Name = name;
                    Service.Configuration.Save();
                }

                int macroindex = Service.Configuration.Events[i].MacroIndex;
                if (ImGui.DragInt($"{LocalizationManager.RightLang.Configwindow_Events_MacroIndex}##MacroIndex{i}",
                    ref macroindex, 1, 0, 99))
                {
                    Service.Configuration.Events[i].MacroIndex = macroindex;
                    Service.Configuration.Save();
                }

                bool isShared = Service.Configuration.Events[i].IsShared;
                if (ImGui.Checkbox($"{LocalizationManager.RightLang.Configwindow_Events_ShareMacro}##ShareMacro{i}",
                    ref isShared))
                {
                    Service.Configuration.Events[i].IsShared = isShared;
                    Service.Configuration.Save();
                }

                ImGui.SameLine();
                ImGuiHelper.Spacing();
                if (ImGui.Button($"{LocalizationManager.RightLang.Configwindow_Events_RemoveEvent}##RemoveEvent{i}"))
                {
                    Service.Configuration.Events.RemoveAt(i);
                    Service.Configuration.Save();
                }
                ImGui.Separator();
            }
            ImGui.EndChild();
        }
        ImGui.PopStyleVar();
    }
}
