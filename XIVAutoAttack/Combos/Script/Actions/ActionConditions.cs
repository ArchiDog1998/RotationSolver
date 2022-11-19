using ImGuiNET;
using System.Linq;
using XIVAutoAttack.Actions;
using XIVAutoAttack.Actions.BaseAction;
using XIVAutoAttack.Combos.Script.Conditions;
using XIVAutoAttack.Data;
using XIVAutoAttack.Windows;
using XIVAutoAttack.Windows.ComboConfigWindow;

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

        public ActionConditions()
        {

        }

        public ActionConditions(BaseAction act)
        {
            _action = act;
            ID = (ActionID)act.ID;
        }

        public void DrawHeader(IScriptCombo combo)
        {
            if (ID != ActionID.None && _action == null)
            {
                _action = combo.AllActions.FirstOrDefault(a => (ActionID)a.ID == ID);
            }

            var tag = ShouldUse(combo, out _);
            ScriptComboWindow.DrawCondition(tag);
            ImGui.SameLine();


            if (_action != null)
            {

                ImGui.Image(_action.GetTexture().ImGuiHandle,
                    new System.Numerics.Vector2(30, 30));

                ImGui.SameLine();
                ComboConfigWindow.Spacing();

                var mustUse = MustUse;
                if (ImGui.Checkbox("必须", ref mustUse))
                {
                    mustUse = MustUse;
                }
                if (ImGui.IsItemHovered())
                {
                    ImGui.SetTooltip("跳过AOE判断，跳过提供的Buff判断。");
                }

                ImGui.SameLine();

                var empty = Empty;
                if (ImGui.Checkbox("用光", ref empty))
                {
                    mustUse = Empty;
                }
                if (ImGui.IsItemHovered())
                {
                    ImGui.SetTooltip("用完所有层数或者跳过连击判断。");
                }

                ImGui.SameLine();
                ComboConfigWindow.Spacing();

                if (ImGui.Selectable(_action.Name, this == XIVAutoAttackPlugin._scriptComboWindow.ActiveAction))
                {
                    XIVAutoAttackPlugin._scriptComboWindow.ActiveAction = this;
                }
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
            ImGui.Text("描述");

            var desc = Description;
            if (ImGui.InputTextMultiline($"##{_action?.Name}的描述", ref desc, 1024, new System.Numerics.Vector2(400, 100)))
            {
                Description = desc;
            }

            Set.Draw(combo);
        }

        public bool? ShouldUse(IScriptCombo owner, out IAction act)
        {
            if (ID != ActionID.None && _action == null)
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
