using Dalamud.Interface;
using Dalamud.Interface.Components;
using ImGuiNET;
using Lumina.Excel.GeneratedSheets;
using System.Collections.Generic;
using System.Numerics;
using XIVAutoAttack.Actions;
using XIVAutoAttack.Data;
using XIVAutoAttack.Windows;

namespace XIVAutoAttack.Combos.Script.Actions
{
    internal class ActionsSet : IDraw
    {
        public string Name { get; set; }

        public string Description { get; set; }

        private bool _openPopup = false;

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
            if (ImGui.Selectable(Name))
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

        private void AddButton(IScriptCombo combo)
        {
            var popId = "Popup" + GetHashCode().ToString();

            if (ImGuiComponents.IconButton(FontAwesomeIcon.Plus))
            {
                ImGui.OpenPopup(popId);
            }



            if (ImGui.BeginPopup("Popup" + GetHashCode().ToString()))
            {
                if (ImGui.Selectable("守卫"))
                {
                    ActionsCondition.Add(new ActionConditions());

                    ImGui.CloseCurrentPopup();
                }

                if (ImGui.BeginChild($"##技能候选列表", new Vector2(150, 500), true))
                {
                    foreach (var item in combo.AllActions)
                    {
                        ImGui.Image(item.GetTexture().ImGuiHandle,
                            new Vector2(24, 24));

                        ImGui.SameLine();
                        if (ImGui.Selectable(item.Name))
                        {
                            ActionsCondition.Add(new ActionConditions(item));

                            ImGui.CloseCurrentPopup();
                        }
                    }
                    ImGui.EndChild();
                }


                ImGui.EndPopup();
            }
        }
    }
}
