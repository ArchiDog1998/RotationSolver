using Dalamud.Interface.Colors;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using XIVAutoAttack.Actions.BaseAction;
using XIVAutoAttack.Data;
using XIVAutoAttack.Windows;
using XIVAutoAttack.Windows.ComboConfigWindow;

namespace XIVAutoAttack.Combos.Script.Conditions
{
    internal enum ActionConditonType : int
    {
        Elapsed,
        ElapsedGCD,
        Remain,
        RemainGCD,
    }

    internal static class ActionConditionTypeExtension
    {
        internal static string ToName(this ActionConditonType type) => type switch
        {
            ActionConditonType.Elapsed => "冷却时长",
            ActionConditonType.ElapsedGCD => "冷却时长GCD",
            ActionConditonType.Remain => "剩余时间",
            ActionConditonType.RemainGCD => "剩余时间GCD",
            _ => string.Empty,
        };
    }

    internal class ActionCondition : ICondition
    {
        private BaseAction _action { get; set; }

        public ActionID ID { get; set; } = ActionID.None;

        public ActionConditonType Type { get; set; }

        public bool IsTrue
        {
            get
            {
                if (_action == null) return false;

                return true;
            }
        }

        string searchTxt = string.Empty;
        public void Draw(IScriptCombo combo)
        {
            if (ID != ActionID.None && _action == null)
            {
                _action = combo.AllActions.FirstOrDefault(a => (ActionID)a.ID == ID);
            }

            var name = _action?.Name ?? string.Empty;
            ImGui.SetNextItemWidth(ImGui.CalcTextSize(name).X + 30);
            if (ImGui.BeginCombo("##技能选择", name))
            {
                ScriptComboWindow.SearchItems(ref searchTxt, combo.AllActions, i =>
                {
                    _action = i;
                    ID = (ActionID)_action.ID;
                });

                ImGui.EndCombo();
            }

            ImGui.SameLine();
            ComboConfigWindow.Spacing();

            var type = (int)Type;
            var names = Enum.GetValues<ActionConditonType>().Select(e => e.ToName()).ToArray();
            ImGui.SetNextItemWidth(100);

            if (ImGui.Combo("##类型", ref type, names, names.Length))
            {
                Type = (ActionConditonType)type;
            }
        }
    }
}
