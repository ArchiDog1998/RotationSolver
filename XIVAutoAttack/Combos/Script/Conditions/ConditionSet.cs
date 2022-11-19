using Dalamud.Interface;
using Dalamud.Interface.Components;
using ImGuiNET;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using XIVAutoAttack.Windows.ComboConfigWindow;

namespace XIVAutoAttack.Combos.Script.Conditions
{
    internal class ConditionSet : ICondition
    {
        [JsonIgnore]
        public bool IsTrue => Conditions.Count == 0 ? true :
                              IsAnd ? Conditions.All(c => c.IsTrue)
                                    : Conditions.Any(c => c.IsTrue);

        public List<ICondition> Conditions { get; set; } = new List<ICondition>();
        public bool IsAnd { get; set; }

        public void Draw(IScriptCombo combo)
        {
            if(ImGui.BeginChild(this.GetHashCode().ToString(), new System.Numerics.Vector2(0, 0), true))
            {
                int isAnd = IsAnd ? 1 : 0;
                if (ImGui.Combo("##Rule" + GetHashCode().ToString(), ref isAnd, new string[]
                {
                "OR (一个条件满足即可)",
                "AND (所有条件均要满足)",
                }, 2))
                {
                    IsAnd = isAnd != 0;
                }

                ImGui.SameLine();
                ComboConfigWindow.Spacing();

                AddButton();

                for (int i = 0; i < Conditions.Count; i++)
                {
                    var condition = Conditions[i];

                    condition.Draw(combo);

                    ImGui.SameLine();
                    ComboConfigWindow.Spacing();

                    if (ImGuiComponents.IconButton(FontAwesomeIcon.Cross))
                    {
                        Conditions.RemoveAt(i);
                    }
                }

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
                AddOneCondition<ConditionSet>("添加组合");

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
}
