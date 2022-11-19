using Dalamud.Interface.Colors;
using ImGuiNET;
using Newtonsoft.Json;
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

namespace XIVAutoAttack.Combos.Script.Conditions;

internal class ActionCondition : ICondition
{
    private BaseAction _action { get; set; }

    public ActionID ID { get; set; } = ActionID.None;

    public ActionConditonType Type { get; set; } = ActionConditonType.Elapsed;

    public bool Condition { get; set; }

    public int GCD { get; set; }
    public int Ability { get; set; }
    public float Time { get; set; }

    [JsonIgnore]
    public bool IsTrue
    {
        get
        {
            if (_action == null) return false;

            var result = false;

            switch (Type)
            {
                case ActionConditonType.Elapsed:
                    result = _action.ElapsedAfter(Time); // 大于
                    break;

                case ActionConditonType.ElapsedGCD:
                    result = _action.ElapsedAfterGCD((uint)GCD, (uint)Ability); // 大于
                    break;

                case ActionConditonType.Remain:
                    result = !_action.WillHaveOneCharge(Time); //小于
                    break;

                case ActionConditonType.RemainGCD:
                    result = !_action.WillHaveOneChargeGCD((uint)GCD, (uint)Ability); // 小于
                    break;
            }

            return Condition ? !result : result;
        }
    }

    string searchTxt = string.Empty;
    public void Draw(IScriptCombo combo)
    {
        if (ID != ActionID.None && _action == null)
        {
            _action = combo.AllActions.FirstOrDefault(a => (ActionID)a.ID == ID);
        }

        ScriptComboWindow.DrawCondition(IsTrue);

        ImGui.SameLine();

        var name = _action?.Name ?? string.Empty;
        ImGui.SetNextItemWidth(Math.Max(80, ImGui.CalcTextSize(name).X + 30));
        if (ImGui.BeginCombo($"##技能选择{GetHashCode()}", name))
        {
            ScriptComboWindow.SearchItems(ref searchTxt, combo.AllActions, i =>
            {
                _action = i;
                ID = (ActionID)_action.ID;
            });

            ImGui.EndCombo();
        }

        ImGui.SameLine();

        var type = (int)Type;
        var names = Enum.GetValues<ActionConditonType>().Select(e => e.ToName()).ToArray();
        ImGui.SetNextItemWidth(100);

        if (ImGui.Combo($"##类型{GetHashCode()}", ref type, names, names.Length))
        {
            Type = (ActionConditonType)type;
        }

        var condition = Condition ? 1 : 0;

        ImGui.SameLine();
        ImGui.SetNextItemWidth(60);

        if (ImGui.Combo($"##大小情况{GetHashCode()}", ref condition, new string[] {"大于", "小于"}, 2))
        {
            Condition = condition > 0;
        }

        ImGui.SameLine();

        switch (Type)
        {
            case ActionConditonType.Elapsed:
            case ActionConditonType.Remain:
                ImGui.SetNextItemWidth(50);
                var time = Time;
                if(ImGui.DragFloat($"时间##时间{GetHashCode()}", ref time))
                {
                    Time = time;
                }
                break;

            case ActionConditonType.ElapsedGCD:
            case ActionConditonType.RemainGCD:
                ImGui.SetNextItemWidth(50);
                var gcd = GCD;
                if (ImGui.DragInt($"GCD##GCD{GetHashCode()}", ref gcd))
                {
                    GCD = Math.Max(0, gcd);
                }
                ImGui.SameLine();

                ImGui.SetNextItemWidth(50);
                var ability = Ability;
                if (ImGui.DragInt($"能力##AbilityD{GetHashCode()}", ref ability))
                {
                    Ability = Math.Max(0, ability);
                }
                break;
        }
    }
}

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
