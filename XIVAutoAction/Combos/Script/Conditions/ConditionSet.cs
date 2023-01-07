using Dalamud.Interface;
using Dalamud.Interface.Components;
using ImGuiNET;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using AutoAction.Localization;
using AutoAction.Windows;

namespace AutoAction.Combos.Script.Conditions;

internal class ConditionSet : ICondition
{
    public bool IsTrue(IScriptCombo combo) => Conditions.Count == 0 ? true :
                          IsAnd ? Conditions.All(c => c.IsTrue(combo))
                                : Conditions.Any(c => c.IsTrue(combo));
    public List<ICondition> Conditions { get; set; } = new List<ICondition>();
    public bool IsAnd { get; set; }

    [JsonIgnore]
    public float Height => Conditions.Sum(c => c is ConditionSet ? c.Height + 10 : c.Height) + ICondition.DefaultHeight + 12;
    public void Draw(IScriptCombo combo)
    {
        if (ImGui.BeginChild(this.GetHashCode().ToString(), new System.Numerics.Vector2(-1f, Height), true))
        {
            AddButton();

            ImGui.SameLine();

            ScriptComboWindow.DrawCondition(IsTrue(combo));

            ImGui.SameLine();

            int isAnd = IsAnd ? 1 : 0;
            if (ImGui.Combo("##Rule" + GetHashCode().ToString(), ref isAnd, new string[]
            {
                LocalizationManager.RightLang.Scriptwindow_OR,
                LocalizationManager.RightLang.Scriptwindow_AND,
            }, 2))
            {
                IsAnd = isAnd != 0;
            }

            ImGui.Separator();

            var relay = Conditions;
            if (ScriptComboWindow.DrawEditorList(relay, i => i.Draw(combo)))
                Conditions = relay;

            ImGui.EndChild();
        }
    }

    private void AddButton()
    {
        if (ImGuiComponents.IconButton(FontAwesomeIcon.Plus))
        {
            ImGui.OpenPopup("Popup" + GetHashCode().ToString());
        }

        if (ImGui.BeginPopup("Popup" + GetHashCode().ToString()))
        {
            AddOneCondition<ConditionSet>(LocalizationManager.RightLang.Scriptwindow_ConditionSet);
            AddOneCondition<ActionCondition>(LocalizationManager.RightLang.Scriptwindow_ActionCondition);
            AddOneCondition<TargetCondition>(LocalizationManager.RightLang.Scriptwindow_TargetCondition);
            AddOneCondition<ComboCondition>(LocalizationManager.RightLang.Scriptwindow_ComboCondition);

            ImGui.EndPopup();
        }
    }

    private void AddOneCondition<T>(string name) where T : ICondition
    {
        if (ImGui.Selectable(name))
        {
            Conditions.Add(Activator.CreateInstance<T>());
            ImGui.CloseCurrentPopup();
        }
    }
}
