using ImGuiNET;
using System.Collections.Generic;
using System.Numerics;
using XIVAutoAction.Actions;
using XIVAutoAction.Localization;
using XIVAutoAction.Windows;
using XIVAutoAction.Actions.BaseAction;
using XIVAutoAction.Windows.ComboConfigWindow;

namespace XIVAutoAction.Combos.Script.Actions
{
    internal class ActionsSet : IDraw
    {
        public bool IsAbility { get; set; } = false;
        public bool IsEmergency { get; set; } = false;

        public List<ActionConditions> ActionsCondition { get; set; } = new List<ActionConditions>();

        public bool ShouldUse(IScriptCombo owner, byte abilityRemain, IAction nextGCD, out IAction act)
        {
            act = null;

            foreach (var condition in ActionsCondition)
            {
                condition.IsEmergency = IsEmergency;
                condition.IsAbility = IsAbility;

                var result = condition.ShouldUse(owner, abilityRemain, nextGCD, out act);

                if (result.HasValue && result.Value) return true;
                else if (!result.HasValue) return false;
            }

            return false;
        }

        public void Draw(IScriptCombo combo)
        {
            AddButton(combo);

            ImGui.SameLine();
            ComboConfigWindow.Spacing();

            ImGui.TextWrapped(LocalizationManager.RightLang.Scriptwindow_ActionSetDescription);

            if (ImGui.BeginChild($"##ActionUsageList", new Vector2(-5f, -1f), true))
            {
                var relay = ActionsCondition;
                if (ScriptComboWindow.DrawEditorList(relay, i =>
                {
                    i.IsEmergency = IsEmergency;
                    i.IsAbility = IsAbility;
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
            ScriptComboWindow.AddPopup("PopupAction" + GetHashCode().ToString(), LocalizationManager.RightLang.Scriptwindow_ActionSetGaurd, () =>
            {
                ActionsCondition.Add(new ActionConditions()
                {
                    IsAbility = IsAbility,
                    IsEmergency = IsEmergency,
                });
            }, ref search, combo.AllActions, item =>
            {
                if (item is not BaseAction act) return;
                ActionsCondition.Add(new ActionConditions(act)
                {
                    IsAbility = IsAbility,
                    IsEmergency = IsEmergency,
                });
            });

            if (ImGui.IsItemHovered())
            {
                ImGui.SetTooltip(LocalizationManager.RightLang.Scriptwindow_AddActionDesc);
            }

            ImGui.SameLine();
            ComboConfigWindow.Spacing();

            ScriptComboWindow.AddPopup("PopupFunction" + GetHashCode().ToString(),
                ref search, combo.AllOther, item =>
            {
                ActionsCondition.Add(new ActionConditions(item));
            });

            if (ImGui.IsItemHovered())
            {
                ImGui.SetTooltip(string.Format(LocalizationManager.RightLang.Scriptwindow_AddFunctionDesc, combo.AllOther.Length));
            }
        }
    }
}
