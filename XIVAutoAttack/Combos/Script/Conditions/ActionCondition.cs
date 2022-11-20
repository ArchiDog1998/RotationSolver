using ImGuiNET;
using Newtonsoft.Json;
using System;
using System.Linq;
using XIVAutoAttack.Actions.BaseAction;
using XIVAutoAttack.Data;
using XIVAutoAttack.Windows;

namespace XIVAutoAttack.Combos.Script.Conditions;

internal class ActionCondition : ICondition
{
    private BaseAction _action { get; set; }

    public ActionID ID { get; set; } = ActionID.None;

    public ActionConditonType ActionConditonType { get; set; } = ActionConditonType.Elapsed;

    public bool Condition { get; set; }

    public int Param1 { get; set; }
    public int Param2 { get; set; }
    public float Time { get; set; }

    public bool IsTrue(IScriptCombo combo)
    {

            if (_action == null || Service.ClientState.LocalPlayer == null) return false;

            var result = false;

            switch (ActionConditonType)
            {
                case ActionConditonType.Elapsed:
                    result = _action.ElapsedAfter(Time); // 大于
                    break;

                case ActionConditonType.ElapsedGCD:
                    result = _action.ElapsedAfterGCD((uint)Param1, (uint)Param2); // 大于
                    break;

                case ActionConditonType.Remain:
                    result = !_action.WillHaveOneCharge(Time); //小于
                    break;

                case ActionConditonType.RemainGCD:
                    result = !_action.WillHaveOneChargeGCD((uint)Param1, (uint)Param2); // 小于
                    break;

                case ActionConditonType.ShouldUse:
                    result = _action.ShouldUse(out _, Param1 > 0, Param2 > 0);
                    break;

                case ActionConditonType.EnoughLevel:
                    result = _action.EnoughLevel;
                    break;

                case ActionConditonType.IsCoolDown:
                    result = _action.IsCoolDown;
                    break;

                case ActionConditonType.CurrentCharges:
                    result = _action.CurrentCharges > Param1;
                    break;

                case ActionConditonType.MaxCharges:
                    result = _action.MaxCharges > Param1;
                    break;
            }
            
            return Condition ? !result : result;
    }

    [JsonIgnore]
    public float Height => ICondition.DefaultHeight;

    string searchTxt = string.Empty;
    public void Draw(IScriptCombo combo)
    {
        if (ID != ActionID.None && (_action == null || (ActionID)_action.ID != ID))
        {
            _action = combo.AllActions.FirstOrDefault(a => (ActionID)a.ID == ID);
        }

        ScriptComboWindow.DrawCondition(IsTrue(combo));
        ImGui.SameLine();

        var name = _action?.Name ?? string.Empty;
        ImGui.SetNextItemWidth(Math.Max(80, ImGui.CalcTextSize(name).X + 30));

        ScriptComboWindow.SearchItems($"##技能选择{GetHashCode()}", name, ref searchTxt, combo.AllActions, i =>
        {
            _action = i;
            ID = (ActionID)_action.ID;
        });


        ImGui.SameLine();

        var type = (int)ActionConditonType;
        var names = Enum.GetValues<ActionConditonType>().Select(e => e.ToName()).ToArray();
        ImGui.SetNextItemWidth(100);

        if (ImGui.Combo($"##类型{GetHashCode()}", ref type, names, names.Length))
        {
            ActionConditonType = (ActionConditonType)type;
        }

        var condition = Condition ? 1 : 0;
                var combos = new string[0];
        switch (ActionConditonType)
        {
            case ActionConditonType.ElapsedGCD:
            case ActionConditonType.RemainGCD:
            case ActionConditonType.Elapsed:
            case ActionConditonType.Remain:
            case ActionConditonType.CurrentCharges:
            case ActionConditonType.MaxCharges:
                combos = new string[] { ">", "<=" };
                break;

            case ActionConditonType.ShouldUse:
                combos = new string[] { "能", "不能" };
                break;

            case ActionConditonType.EnoughLevel:
            case ActionConditonType.IsCoolDown:
                combos = new string[] { "是", "不是" };
                break;
        }
        ImGui.SameLine();
        ImGui.SetNextItemWidth(60);
        if (ImGui.Combo($"##大小情况{GetHashCode()}", ref condition, combos, combos.Length))
        {
            Condition = condition > 0;
        }


        switch (ActionConditonType)
        {
            case ActionConditonType.Elapsed:
            case ActionConditonType.Remain:
                ImGui.SameLine();

                ImGui.SetNextItemWidth(50);
                var time = Time;
                if (ImGui.DragFloat($"秒##秒{GetHashCode()}", ref time))
                {
                    Time = time;
                }
                break;

            case ActionConditonType.ElapsedGCD:
            case ActionConditonType.RemainGCD:
                ImGui.SameLine();

                ImGui.SetNextItemWidth(50);
                var gcd = Param1;
                if (ImGui.DragInt($"GCD##GCD{GetHashCode()}", ref gcd))
                {
                    Param1 = Math.Max(0, gcd);
                }
                ImGui.SameLine();

                ImGui.SetNextItemWidth(50);
                var ability = Param2;
                if (ImGui.DragInt($"能力##AbilityD{GetHashCode()}", ref ability))
                {
                    Param2 = Math.Max(0, ability);
                }
                break;

            case ActionConditonType.ShouldUse:
                ImGui.SameLine();

                var must = Param1 > 0;
                if (ImGui.Checkbox($"必须##必须{GetHashCode()}", ref must))
                {
                    Param1 = must ? 1 : 0;
                }
                ImGui.SameLine();

                var empty = Param2 > 0;
                if (ImGui.Checkbox($"用光##用光{GetHashCode()}", ref empty))
                {
                    Param2 = empty ? 1 : 0;
                }
                break;

            case ActionConditonType.CurrentCharges:
            case ActionConditonType.MaxCharges:
                ImGui.SameLine();

                ImGui.SetNextItemWidth(50);
                var charge = Param1;
                if (ImGui.DragInt($"层##层{GetHashCode()}", ref charge))
                {
                    Param1 = Math.Max(0, charge);
                }
                break;
        }
    }
}

public enum ActionConditonType : int
{
    Elapsed,
    ElapsedGCD,
    Remain,
    RemainGCD,
    ShouldUse,
    EnoughLevel,
    IsCoolDown,
    CurrentCharges,
    MaxCharges,
}

internal static class ActionConditionTypeExtension
{
    internal static string ToName(this ActionConditonType type) => type switch
    {
        ActionConditonType.Elapsed => "冷却时长",
        ActionConditonType.ElapsedGCD => "冷却时长GCD",
        ActionConditonType.Remain => "剩余时间",
        ActionConditonType.RemainGCD => "剩余时间GCD",
        ActionConditonType.ShouldUse => "能否被使用",
        ActionConditonType.EnoughLevel => "等级足够",
        ActionConditonType.IsCoolDown => "正在冷却",
        ActionConditonType.CurrentCharges => "当前层数",
        ActionConditonType.MaxCharges => "最大层数",
        _ => string.Empty,
    };
}
