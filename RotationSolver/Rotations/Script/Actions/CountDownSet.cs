using ImGuiNET;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using RotationSolver.Actions.BaseAction;
using RotationSolver.Actions;
using RotationSolver.Windows;
using RotationSolver.Localization;
using RotationSolver.Helpers;

namespace RotationSolver.Rotations.Script.Actions;

internal class CountDownSet : IDraw
{
    public List<CountDownAction> ActionsCondition { get; set; } = new List<CountDownAction>();

    public void Draw(IScriptCombo combo)
    {
        AddButton(combo);

        ImGui.SameLine();
        ImGuiHelper.Spacing();

        ImGui.TextWrapped(LocalizationManager.RightLang.Scriptwindow_CountDownSetDesc);

        if (ImGui.BeginChild($"##ActionUsageList", new Vector2(-5f, -1f), true))
        {
            var relay = ActionsCondition;
            if (ScriptComboWindow.DrawEditorList(relay, i =>
            {
                i.DrawHeader(combo);
            }))
            {
                ActionsCondition = relay;
            }
            ImGui.EndChild();
        }
    }

    string search = string.Empty;
    private void AddButton(IScriptCombo combo)
    {
        ScriptComboWindow.AddPopup("PopupAction" + GetHashCode().ToString(), string.Empty, null, ref search, combo.AllActions.OfType<BaseAction>(), item =>
        {
            ActionsCondition.Add(new CountDownAction(item));
        });

        if (ImGui.IsItemHovered())
        {
            ImGui.SetTooltip(LocalizationManager.RightLang.Scriptwindow_AddActionDesc);
        }

        ImGui.SameLine();
        ImGuiHelper.Spacing();

        ScriptComboWindow.AddPopup("PopupFunction" + GetHashCode().ToString(),
            ref search, combo.AllOther, item =>
            {
                ActionsCondition.Add(new CountDownAction(item));
            });

        if (ImGui.IsItemHovered())
        {
            ImGui.SetTooltip(string.Format(LocalizationManager.RightLang.Scriptwindow_AddFunctionDesc, combo.AllOther.Length));
        }
    }

    public IAction ShouldUse(IScriptCombo owner, float time)
    {
        foreach (var action in ActionsCondition.OrderBy(a => a.Time))
        {
            if (time < action.Time && action.ShouldUse(owner, out var act)) return act;
        }

        return null;
    }
}
