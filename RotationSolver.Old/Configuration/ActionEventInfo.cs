using ImGuiNET;
using RotationSolver.Localization;

namespace RotationSolver.Configuration;

public class ActionEventInfo : MacroInfo
{
    public string Name;

    public ActionEventInfo()
    {
        Name = "";
    }

    public override void DisplayMacro()
    {
        if (ImGui.InputText($"{LocalizationManager.RightLang.Configwindow_Events_ActionName}##ActionName{GetHashCode()}",
            ref Name, 100))
        {
            Service.Configuration.Save();
        }

        base.DisplayMacro();
    }
}
