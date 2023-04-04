using Dalamud.Interface.Windowing;
using RotationSolver.Localization;

namespace RotationSolver.UI;
internal partial class RotationConfigWindow : Window
{
    const float DRAG_NUMBER_WIDTH = 100;

    public RotationConfigWindow()
        : base(nameof(RotationConfigWindow), 0, false)
    {
        SizeCondition = ImGuiCond.FirstUseEver;
        Size = new Vector2(740f, 490f);
        RespectCloseHotkey = true;
    }

    public override unsafe void Draw()
    {
        if (ImGui.BeginTabBar("RotationSolverSettings"))
        {
            if (ImGui.BeginTabItem(LocalizationManager.RightLang.ConfigWindow_RotationItem))
            {
                DrawRotationTab();
                ImGui.EndTabItem();
            }

            if (ImGui.BeginTabItem(LocalizationManager.RightLang.ConfigWindow_ParamItem))
            {
                DrawParamTab();
                ImGui.EndTabItem();
            }

            if (ImGui.BeginTabItem(LocalizationManager.RightLang.ConfigWindow_EventItem))
            {
                DrawEventTab();
                ImGui.EndTabItem();
            }

            if (ImGui.BeginTabItem(LocalizationManager.RightLang.ConfigWindow_ActionItem))
            {
                DrawActionTab();
                ImGui.EndTabItem();
            }

            if (ImGui.BeginTabItem(LocalizationManager.RightLang.ConfigWindow_ControlItem))
            {
                DrawControlTab();
                ImGui.EndTabItem();
            }

            if (ImGui.BeginTabItem(LocalizationManager.RightLang.ConfigWindow_HelpItem))
            {
                DrawHelpTab();
                ImGui.EndTabItem();
            }

            if (ImGui.BeginTabItem(LocalizationManager.RightLang.ConfigWindow_RotationDev))
            {
                DrawRotationDevTab();
                ImGui.EndTabItem();
            }

            if (Service.Config.InDebug && ImGui.BeginTabItem("Debug"))
            {
                DrawDebugTab();
                ImGui.EndTabItem();
            }

            ImGui.EndTabBar();
        }
        ImGui.End();
    }
}
