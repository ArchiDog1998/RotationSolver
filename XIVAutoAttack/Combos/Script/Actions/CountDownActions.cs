using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using XIVAutoAttack.Combos.Script.Conditions;
using XIVAutoAttack.Windows.ComboConfigWindow;
using XIVAutoAttack.Windows;
using XIVAutoAttack.Actions;

namespace XIVAutoAttack.Combos.Script.Actions
{
    internal class CountDownActions : IDraw
    {
        public List<CountDownAction> ActionsCondition { get; set; } = new List<CountDownAction>();

        public void Draw(IScriptCombo combo)
        {
            AddButton(combo);

            ImGui.SameLine();
            ComboConfigWindow.Spacing();

            ImGui.TextWrapped("在下面的框框中输入技能，越上面优先级越高。");

            if (ImGui.BeginChild($"##技能使用列表", new Vector2(-5f, -1f), true))
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
            ScriptComboWindow.AddPopup("PopupAction" + GetHashCode().ToString(), string.Empty, null, ref search, combo.AllActions, item =>
            {
                ActionsCondition.Add(new CountDownAction(item));
            });

            if (ImGui.IsItemHovered())
            {
                ImGui.SetTooltip("倒计时");
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
}
