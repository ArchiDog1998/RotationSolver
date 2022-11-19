using ImGuiNET;
using System.Collections.Generic;
using System.Numerics;
using XIVAutoAttack.Actions;
using XIVAutoAttack.Windows;
using XIVAutoAttack.Windows.ComboConfigWindow;

namespace XIVAutoAttack.Combos.Script.Actions
{
    internal class ActionsSet : IDraw
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public List<ActionConditions> ActionsCondition { get; set; } = new List<ActionConditions>();

        public bool ShouldUse(IScriptCombo owner, out IAction act)
        {
            act = null;

            foreach (var condition in ActionsCondition)
            {
                var result = condition.ShouldUse(owner, out act);

                if (result.HasValue && result.Value) return true;
                else if (!result.HasValue) return false;
            }

            return false;
        }

        public void DrawHeader()
        {
            if (ImGui.Selectable(Name, this == XIVAutoAttackPlugin._scriptComboWindow.ActiveSet))
            {
                XIVAutoAttackPlugin._scriptComboWindow.ActiveSet = this;
            }
        }

        public void Draw(IScriptCombo combo)
        {
            AddButton(combo);

            ImGui.SameLine();
            ComboConfigWindow.Spacing();

            ImGui.TextWrapped(Description);

            if (ImGui.BeginChild($"##技能使用列表", new Vector2(-5f, -1f), true))
            {
                var relay = ActionsCondition;
                if (ScriptComboWindow.DrawEditorList(relay, i => i.DrawHeader(combo)))
                    ActionsCondition = relay;

                ImGui.EndChild();
            }
        }

        string search = string.Empty;
        private void AddButton(IScriptCombo combo)
        {
            var popId = "Popup" + GetHashCode().ToString();

            ScriptComboWindow.AddPopup(popId, "技能卫士", () =>
            {
                ActionsCondition.Add(new ActionConditions());
            }, ref search, combo.AllActions, item =>
            {
                ActionsCondition.Add(new ActionConditions(item));
            });

        }
    }
}
