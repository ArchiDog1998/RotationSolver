using Dalamud.Interface.Windowing;
using Dalamud.Plugin;
using FFXIVClientStructs.Interop;
using ImGuiNET;
using Lumina.Excel;
using RotationSolver.Basic;
using RotationSolver.Basic.Actions;
using RotationSolver.Basic.Rotations;
using RotationSolver.Basic.Rotations.Basic;
using RotationSolver.Localization;
using System.Numerics;
using System.Reflection;
using System.Runtime.Serialization;

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

    //private static readonly Dictionary<JobRole, string> _roleDescriptionValue = new Dictionary<JobRole, string>()
    //{
    //    {JobRole.Tank, $"{DescType.DefenseSingle.ToName()} ¡ú {CustomRotation.Rampart}, {CustomRotation.Reprisal}" },
    //    {JobRole.Melee, $"{DescType.DefenseArea.ToName()} ¡ú {CustomRotation.Feint}" },
    //    {JobRole.RangedMagicial, $"{DescType.DefenseArea.ToName()} ¡ú {CustomRotation.Addle}" },
    //};

    static Exception exc;
    static Type[] types;

    public override unsafe void Draw()
    {
        if (ImGui.BeginTabBar("RotationSolverSettings"))
        {
#if DEBUG
            if (ImGui.BeginTabItem("Debug"))
            {
                DrawDebugTab();

                try
                {
                    
                    foreach (var type in from prop in typeof(WAR_Base).GetProperties()
                                         where typeof(IAction).IsAssignableFrom(prop.PropertyType) && !(prop.GetMethod?.IsPrivate ?? true) select prop)
                    {
                        ImGui.Text(type.Name);
                        var a  = (IAction)type.GetValue(null);
                    }
                }
                catch (Exception e)
                {
                    while(e != null)
                    {
                        ImGui.Text(e.Message);
                        e = e.InnerException;
                    }
                }

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
