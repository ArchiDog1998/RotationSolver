using Dalamud.Interface.Windowing;
using ImGuiNET;
using RotationSolver.Data;
using RotationSolver.Localization;
using RotationSolver.Rotations.CustomRotation;
using System.Collections.Generic;
using System.Numerics;

namespace RotationSolver.Windows.RotationConfigWindow;

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

    private static readonly Dictionary<JobRole, string> _roleDescriptionValue = new Dictionary<JobRole, string>()
    {
        {JobRole.Tank, $"{DescType.DefenseSingle.ToName()} ¡ú {CustomRotation.Rampart}, {CustomRotation.Reprisal}" },
        {JobRole.Melee, $"{DescType.DefenseArea.ToName()} ¡ú {CustomRotation.Feint}" },
        {JobRole.RangedMagicial, $"{DescType.DefenseArea.ToName()} ¡ú {CustomRotation.Addle}" },
    };

    public override unsafe void Draw()
    {
        if (ImGui.BeginTabBar("RotationSolverSettings"))
        {
#if DEBUG
            if (Service.ClientState.LocalPlayer != null && ImGui.BeginTabItem("Debug"))
            {
                DrawDebugTab();
                ImGui.EndTabItem();
            }
#endif
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

            if (ImGui.BeginTabItem(LocalizationManager.RightLang.ConfigWindow_HelpItem))
            {
                DrawHelpTab();
                ImGui.EndTabItem();
            }

            ImGui.EndTabBar();
        }
        ImGui.End();
    }
}
