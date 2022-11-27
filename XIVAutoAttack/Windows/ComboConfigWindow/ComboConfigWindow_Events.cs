using ImGuiNET;
using System.Numerics;
using XIVAutoAttack.Configuration;
using XIVAutoAttack.Localization;

namespace XIVAutoAttack.Windows.ComboConfigWindow;

internal partial class ComboConfigWindow
{
    private void DrawEvent()
    {
        if (ImGui.Button(LocalizationManager.RightLang.Configwindow_Events_AddEvent))
        {
            Service.Configuration.Events.Add(new ActionEventInfo());
        }
        ImGui.SameLine();
        Spacing();

        ImGui.Text(LocalizationManager.RightLang.Configwindow_Events_Description);

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
                }

                bool isShared = Service.Configuration.Events[i].IsShared;
                if (ImGui.Checkbox($"{LocalizationManager.RightLang.Configwindow_Events_ShareMacro}##ShareMacro{i}",
                    ref isShared))
                {
                    Service.Configuration.Events[i].IsShared = isShared;
                    Service.Configuration.Save();
                }

                ImGui.SameLine();
                Spacing();
                if (ImGui.Button($"{LocalizationManager.RightLang.Configwindow_Events_RemoveEvent}##RemoveEvent{i}"))
                {
                    Service.Configuration.Events.RemoveAt(i);
                }
                ImGui.Separator();
            }
            ImGui.EndChild();
        }
        ImGui.PopStyleVar();

    }
}
