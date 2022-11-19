using Dalamud.Interface;
using Dalamud.Interface.Colors;
using Dalamud.Interface.Components;
using ImGuiNET;
using Lumina.Excel.GeneratedSheets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using XIVAutoAttack.Actions;
using XIVAutoAttack.Data;
using XIVAutoAttack.Windows;
using XIVAutoAttack.Windows.ComboConfigWindow;
using Action = System.Action;

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
                int index = -1;
                for (int i = 0; i < ActionsCondition.Count; i++)
                {
                    var item = ActionsCondition[i];

                    if (ImGuiComponents.IconButton(item.GetHashCode(), FontAwesomeIcon.Minus))
                    {
                        index = i;
                    }

                    ImGui.SameLine();
                    ComboConfigWindow.Spacing();

                    item.DrawHeader();

                    ImGui.Separator();
                }

                if (index != -1) ActionsCondition.RemoveAt(index);
                ImGui.EndChild();
            }
        }

        string search = string.Empty;
        private void AddButton(IScriptCombo combo)
        {
            var popId = "Popup" + GetHashCode().ToString();

            ScriptComboWindow.AddPopup(popId, "技能卫士",  () =>
            {
                ActionsCondition.Add(new ActionConditions());
            }, ref search, combo.AllActions, item =>
            {
                ActionsCondition.Add(new ActionConditions(item));
            });

        }
    }
}
