using ImGuiNET;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XIVAutoAttack.Actions;
using XIVAutoAttack.Actions.BaseAction;
using XIVAutoAttack.Combos.Script.Conditions;
using XIVAutoAttack.Data;
using XIVAutoAttack.Windows;

namespace XIVAutoAttack.Combos.Script.Actions
{
    /// <summary>
    /// 最右边那栏用来渲染的
    /// </summary>
    internal class ActionConditions : IDraw
    {
        private BaseAction _action { get; set; }

        public ActionID ID { get; set; } = ActionID.None;
        public ConditionSet Set { get; set; } = new ConditionSet();

        public bool MustUse { get; set; }
        public bool Empty { get; set; }

        public string Description { get; set; } = string.Empty;

        public void DrawHeader()
        {
            if(_action != null)
            {
                ImGui.Columns(2);

                var text = _action.GetTexture();

                ImGui.SetColumnWidth(0, text.Width + 5);

                ImGui.Image(text.ImGuiHandle, new System.Numerics.Vector2(text.Width, text.Height));

                ImGui.NextColumn();

                if (ImGui.Selectable(_action.Name))
                {
                    XIVAutoAttackPlugin._scriptComboWindow.ActiveAction = this;
                }

                var mustUse = MustUse;
                if (ImGui.Checkbox("必须使用", ref mustUse))
                {
                    mustUse = MustUse;
                }
                if (ImGui.IsItemHovered())
                {
                    ImGui.SetTooltip("跳过AOE判断，跳过提供的Buff判断。");
                }

                ImGui.SameLine();
                ComboConfigWindow.Spacing();

                var empty = Empty;
                if (ImGui.Checkbox("用光充能", ref empty))
                {
                    mustUse = Empty;
                }
                if (ImGui.IsItemHovered())
                {
                    ImGui.SetTooltip("用完所有层数或者跳过连击判断。");
                }

                ImGui.Columns(1);
            }
            else
            {
                if (ImGui.Selectable("返回条件"))
                {
                    XIVAutoAttackPlugin._scriptComboWindow.ActiveAction = this;
                }
            }
        }

        public void Draw(IScriptCombo combo)
        {
            var desc = Description;
            if(ImGui.InputTextMultiline("描述", ref desc, 1024, new System.Numerics.Vector2(250, 250)))
            {
                Description = desc;
            }

            Set.Draw(combo);
        }

        public bool? ShouldUse(IScriptCombo owner, out IAction act)
        {
            if(ID != ActionID.None && _action == null)
            {
                _action = owner.AllActions.FirstOrDefault(a => (ActionID)a.ID == ID);
            }

            act = _action;

            if (_action != null)
            {
                return _action.ShouldUse(out act, MustUse, Empty) && Set.IsTrue;
            }
            else
            {
                return Set.IsTrue ? null : false;
            }
        }
    }
}
