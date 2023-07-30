using Dalamud.Interface.Windowing;
using Dalamud.Logging;
using RotationSolver.Localization;

namespace RotationSolver.UI;
internal partial class RotationConfigWindow : Window
{
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

            if (ImGui.BeginTabItem(LocalizationManager.RightLang.ConfigWindow_ListItem))
            {
                DrawListTab();
                ImGui.EndTabItem();
            }

            if (ImGui.BeginTabItem(LocalizationManager.RightLang.ConfigWindow_ControlItem))
            {
                DrawControlTab();
                ImGui.EndTabItem();
            }

            if (ImGui.BeginTabItem(LocalizationManager.RightLang.ConfigWindow_ActionItem))
            {
                DrawActionTab();
                ImGui.EndTabItem();
            }

            if (ImGui.BeginTabItem(LocalizationManager.RightLang.ConfigWindow_EventItem))
            {
                DrawEventTab();
                ImGui.EndTabItem();
            }

            if (ImGui.BeginTabItem(LocalizationManager.RightLang.ConfigWindow_HelpItem))
            {
                DrawHelpTab();
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

    internal static void DrawCheckBox(string name, SettingsCommand command, string description = "", Action otherThing = null)
    {
        var value = Service.Config.GetValue(command);
        DrawCheckBox(name, ref value, command.GetDefault(), description, () =>
        {
            Service.Config.SetValue(command, value);
            otherThing?.Invoke();
        });

        ImGui.SameLine();
        ImGuiHelper.Spacing();
        ImGuiHelper.DisplayCommandHelp(OtherCommandType.Settings, command.ToString());
    }

    internal static void DrawIconCheckBox(ActionID actionID, ref bool value, string description = "", Action otherThing = null)
    {
        var name = $"##{actionID}";
        description = actionID.ToString() + "\n" + description;

        ControlWindow.DrawIAction(IconSet.GetTexture(actionID).ImGuiHandle, 40, value ? 1 : - 1);

        ImGuiHelper.HoveredString(description);

        ImGui.SameLine();
        ImGuiHelper.Spacing();

        DrawCheckBox(name, ref value, description, otherThing);
    }

    internal static void DrawCheckBox(string name, ref bool value, bool @default, string description = "", Action otherThing = null)
    {
        DrawCheckBox(name, ref value, description, otherThing);

        if (value != @default)
        {
            ImGuiHelper.UndoValue(name, ref value, @default, otherThing);
        }
    }

    internal static void DrawCheckBox(string name, ref bool value, string description = "", Action otherThing = null)
    {
        if (ImGui.Checkbox(name, ref value))
        {
            otherThing?.Invoke();
            Service.Config.Save();
        }

        ImGuiHelper.HoveredString(description);
    }

    private static void DrawRangedFloat(string name, ref float minValue, ref float maxValue, float defaultMin, float defaultMax, float speed = 0.01f, float min = 0, float max = 3, string description = "")
    {
        ImGui.SetNextItemWidth(100);
        if (ImGui.DragFloatRange2(name, ref minValue, ref maxValue, speed, min, max))
        {
            Service.Config.Save();
        }

        ImGuiHelper.HoveredString(description);

        if (minValue != defaultMin || maxValue != defaultMax)
        {
            ImGuiHelper.UndoValue(name, ref minValue, defaultMin, ref maxValue, defaultMax);
        }
    }

    private static void DrawRangedInt(string name, ref int minValue, ref int maxValue, int defaultMin, int defaultMax, float speed = 0.01f, int min = 0, int max = 3, string description = "", Action otherThing = null)
    {
        ImGui.SetNextItemWidth(100);
        if (ImGui.DragIntRange2(name, ref minValue, ref maxValue, speed, min, max))
        {
            otherThing?.Invoke();
            Service.Config.Save();
        }

        ImGuiHelper.HoveredString(description);

        if (minValue != defaultMin || maxValue != defaultMax)
        {
            ImGuiHelper.UndoValue(name, ref minValue, defaultMin, ref maxValue, defaultMax, otherThing);
        }
    }

    public static void DrawFloatNumber(string name, ref float value, float @default, float speed = 0.002f, float min = 0, float max = 1, string description = "", Action otherThing = null)
    {
        ImGui.SetNextItemWidth(100);
        if (ImGui.DragFloat(name, ref value, speed, min, max))
        {
            otherThing?.Invoke();
            Service.Config.Save();
        }

        ImGuiHelper.HoveredString(description);

        if (value != @default)
        {
            ImGuiHelper.UndoValue(name, ref value, @default, otherThing);
        }
    }

    public static void DrawIntNumber(string name, ref int value, int @default, float speed = 0.2f, int min = 0, int max = 1, string description = "", Action otherThing = null)
    {
        ImGui.SetNextItemWidth(100);
        if (ImGui.DragInt(name, ref value, speed, min, max))
        {
            Service.Config.Save();
            otherThing?.Invoke();
        }

        ImGuiHelper.HoveredString(description);

        if (value != @default)
        {
            ImGuiHelper.UndoValue(name, ref value, @default, otherThing);
        }
    }

    private static void DrawColor3(string name, ref Vector3 value, Vector3 @default, string description = "")
    {
        ImGui.SetNextItemWidth(210);
        if (ImGui.ColorEdit3(name, ref value))
        {
            Service.Config.Save();
        }

        ImGuiHelper.HoveredString(description);

        if (value != @default)
        {
            ImGuiHelper.UndoValue(name, ref value, @default);
        }
    }

    public static void DrawCombo<T>(string name, ref int value, int @default, Func<T, string> toString, T[] choices = null, string description = "") where T : struct, Enum
    {
        choices ??= Enum.GetValues<T>();

        ImGui.SetNextItemWidth(100);
        if (ImGui.BeginCombo(name, toString(choices[value])))
        {
            for (int i = 0; i < choices.Length; i++)
            {
                if (ImGui.Selectable(toString(choices[i])))
                {
                    value = i;
                    Service.Config.Save();
                }
            }
            ImGui.EndCombo();
        }
        ImGuiHelper.HoveredString(description);

        if (value != @default)
        {
            ImGuiHelper.UndoValue(name, ref value, @default);
        }
    }

    private static void DrawInputText(string name, ref string value, uint maxLength, string description = "")
    {
        ImGui.SetNextItemWidth(210);
        if (ImGui.InputText(name, ref value, maxLength))
        {
            Service.Config.Save();
        }
        ImGuiHelper.HoveredString(description);
    }

    private static void DrawParamTabItem(string name, Action act, Action outsideChild = null)
    {
        if (act == null) return;
        if (ImGui.BeginTabItem(name))
        {
            try
            {
                outsideChild?.Invoke();
                if (ImGui.BeginChild("Param", new Vector2(0f, -1f), true))
                {
                    act();
                    ImGui.EndChild();
                }
            }
            catch (Exception ex)
            {
                PluginLog.Warning(ex, $"{name} displays failed.");
            }

            ImGui.EndTabItem();
        }
    }
}
