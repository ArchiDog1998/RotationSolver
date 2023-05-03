using RotationSolver.Basic.Configuration;
using RotationSolver.Localization;

namespace RotationSolver.UI;
internal partial class RotationConfigWindow
{
    private static void DrawEventTab()
    {
        if (ImGui.Button(LocalizationManager.RightLang.ConfigWindow_Events_AddEvent))
        {
            Service.Config.Events.Add(new ActionEventInfo());
            Service.Config.Save();
        }
        ImGui.SameLine();
        ImGuiHelper.Spacing();

        ImGui.TextWrapped(LocalizationManager.RightLang.ConfigWindow_Events_Description);

        ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new Vector2(0f, 5f));

        ImGui.Text(LocalizationManager.RightLang.ConfigWindow_Events_DutyStart);
        ImGui.SameLine();
        ImGuiHelper.Spacing();
        Service.Config.DutyStart.DisplayMacro();

        ImGui.Text(LocalizationManager.RightLang.ConfigWindow_Events_DutyEnd);
        ImGui.SameLine();
        ImGuiHelper.Spacing();
        Service.Config.DutyEnd.DisplayMacro();

        if (ImGui.BeginChild("Events List", new Vector2(0f, -1f), true))
        {
            ActionEventInfo remove = null;
            foreach (var eve in Service.Config.Events)
            {
                eve.DisplayMacro();

                ImGui.SameLine();
                ImGuiHelper.Spacing();

                if (ImGui.Button($"{LocalizationManager.RightLang.ConfigWindow_Events_RemoveEvent}##RemoveEvent{eve.GetHashCode()}"))
                {
                    remove = eve;
                }
                ImGui.Separator();
            }
            if(remove!= null)
            {
                Service.Config.Events.Remove(remove);
                Service.Config.Save();
            }

            ImGui.EndChild();
        }
        ImGui.PopStyleVar();
    }
}
