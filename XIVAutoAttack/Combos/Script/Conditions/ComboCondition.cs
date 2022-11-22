using ImGuiNET;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using XIVAutoAttack.Actions;
using XIVAutoAttack.Actions.BaseAction;
using XIVAutoAttack.Data;
using XIVAutoAttack.Helpers;
using XIVAutoAttack.Windows;

namespace XIVAutoAttack.Combos.Script.Conditions;

internal class ComboCondition : ICondition
{
    public ComboConditionType ComboConditionType { get; set; }
    PropertyInfo _info;
    public string PropertyName { get; set; } = string.Empty;

    MethodInfo _method;
    public string MethodName { get; set; } = string.Empty;

    BaseAction _action { get; set; }

    public ActionID ID { get; set; } = ActionID.None;

    public int Condition { get; set; }

    public int Param1 { get; set; }

    public int Param2 { get; set; }
    public float Time { get; set; }

    public bool IsTrue(IScriptCombo combo)
    {
        if (Service.ClientState.LocalPlayer == null) return false;

        if (!string.IsNullOrEmpty(PropertyName) && (_info == null || _info.Name != PropertyName))
        {
            _info = combo.GetType().GetPropertyInfo(PropertyName);
        }
        if (!string.IsNullOrEmpty(MethodName) && (_method == null || _method.Name != MethodName))
        {
            _method = combo.GetType().GetMethodInfo(MethodName);
        }
        if (ID != ActionID.None && (_action == null || (ActionID)_action.ID != ID))
        {
            _action = combo.AllActions.FirstOrDefault(a => (ActionID)a.ID == ID);
        }

        switch (ComboConditionType)
        {
            case ComboConditionType.Bool:
                if (_info == null) return false;
                if(_info.GetValue(combo) is bool b)
                {
                    return Condition > 0 ? !b : b;
                }
                return false;

            case ComboConditionType.Byte:
                if (_info == null) return false;
                if (_info.GetValue(combo) is byte by)
                {
                    switch (Condition)
                    {
                        case 0:
                            return by > Param1;
                        case 1:
                            return by == Param2;
                        case 2:
                            return by < Param2;
                    }
                }
                return false;

            case ComboConditionType.Time:
                if (_method == null) return false;

                try
                {
                    if (_method.Invoke(combo, new object[] { Time }) is bool bo)
                    {
                        return Condition > 0 ? bo : !bo;
                    }
                    return false;
                }
                catch
                {
                    return false;
                }


            case ComboConditionType.TimeGCD:
                if (_method == null) return false;

                try
                {
                    if (_method.Invoke(combo, new object[] { (uint)Param1, (uint)Param2 }) is bool boo)
                    {
                        return Condition > 0 ? boo : !boo;
                    }
                    return false;
                }
                catch
                {
                    return false;
                }

            case ComboConditionType.Last:
                if (_method == null || _action == null) return false;

                try
                {
                    if (_method.Invoke(combo, new object[] { Param1 > 0, new IAction[] { _action } }) is bool boo)
                    {
                        return Condition > 0 ? boo : !boo;
                    }
                    return false;
                }
                catch
                {
                    return false;
                }

        }

        return false;
    }


    [JsonIgnore]
    public float Height => ICondition.DefaultHeight;

    string searchTxt = string.Empty;
    public void Draw(IScriptCombo combo)
    {
        if (!string.IsNullOrEmpty(PropertyName) && (_info == null || _info.Name != PropertyName))
        {
            _info = combo.GetType().GetPropertyInfo(PropertyName);
        }
        if (!string.IsNullOrEmpty(MethodName) && (_method == null || _method.Name != MethodName))
        {
            _method = combo.GetType().GetMethodInfo(MethodName);
        }
        if (ID != ActionID.None && (_action == null || (ActionID)_action.ID != ID))
        {
            _action = combo.AllActions.FirstOrDefault(a => (ActionID)a.ID == ID);
        }

        ScriptComboWindow.DrawCondition(IsTrue(combo));
        ImGui.SameLine();

        var type = (int)ComboConditionType;
        var names = Enum.GetValues<ComboConditionType>().Select(e => e.ToName()).ToArray();
        ImGui.SetNextItemWidth(100);

        if (ImGui.Combo($"##类型{GetHashCode()}", ref type, names, names.Length))
        {
            ComboConditionType = (ComboConditionType)type;
        }

        switch (ComboConditionType)
        {
            case ComboConditionType.Bool:
                ImGui.SameLine();
                ImGui.SetNextItemWidth(Math.Max(80, ImGui.CalcTextSize(PropertyName).X + 30));

                ScriptComboWindow.SearchItemsReflection($"##布尔选择{GetHashCode()}", _info?.GetMemberName(), ref searchTxt, combo.AllBools, i =>
                {
                    _info = i;
                    PropertyName = i.Name;
                });

                ImGui.SameLine();
                ImGui.SetNextItemWidth(50);
                var isTrue = Condition;
                if (ImGui.Combo($"##是否{GetHashCode()}", ref isTrue, new string[] {"是", "不是"}, 2))
                {
                    Condition = isTrue;
                }
                break;

            case ComboConditionType.Byte:
                ImGui.SameLine();
                ImGui.SetNextItemWidth(Math.Max(80, ImGui.CalcTextSize(PropertyName).X + 30));

                ScriptComboWindow.SearchItemsReflection($"##字节选择{GetHashCode()}", _info?.GetMemberName(), ref searchTxt, combo.AllBytes, i =>
                {
                    _info = i;
                    PropertyName = i.Name;
                });


                ImGui.SameLine();
                ImGui.SetNextItemWidth(50);
                var compare = Condition;
                if (ImGui.Combo($"##比较{GetHashCode()}", ref compare, new string[] { ">", "<", "=" }, 3))
                {
                    Condition = compare;
                }

                ImGui.SameLine();
                ImGui.SetNextItemWidth(50);

                var tag = Param1;
                if (ImGui.DragInt($"##大小{GetHashCode()}", ref tag))
                {
                    Param1 = tag;
                }

                break;
            case ComboConditionType.Time:
                ImGui.SameLine();
                ImGui.SetNextItemWidth(Math.Max(80, ImGui.CalcTextSize(MethodName).X + 30));

                ScriptComboWindow.SearchItemsReflection($"##时间{GetHashCode()}", _method?.GetMemberName(), ref searchTxt, combo.AllTimes, i =>
                {
                    _method = i;
                    MethodName = i.Name;
                });

                ImGui.SameLine();
                ImGui.SetNextItemWidth(50);
                compare = Condition;
                if (ImGui.Combo($"##比较{GetHashCode()}", ref compare, new string[] { ">", "<=" }, 2))
                {
                    Condition = compare;
                }

                ImGui.SameLine();
                ImGui.SetNextItemWidth(50);
                var time = Time;
                if (ImGui.DragFloat($"秒##秒{GetHashCode()}", ref time))
                {
                    Time = time;
                }
                break;

            case ComboConditionType.TimeGCD:
                ImGui.SameLine();
                ImGui.SetNextItemWidth(Math.Max(80, ImGui.CalcTextSize(MethodName).X + 30));

                ScriptComboWindow.SearchItemsReflection($"##时间{GetHashCode()}", _method?.GetMemberName(), ref searchTxt, combo.AllGCDs, i =>
                {
                    _method = i;
                    MethodName = i.Name;
                });

                ImGui.SameLine();
                ImGui.SetNextItemWidth(50);
                compare = Condition;
                if (ImGui.Combo($"##比较{GetHashCode()}", ref compare, new string[] { ">", "<=" }, 2))
                {
                    Condition = compare;
                }

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

            case ComboConditionType.Last:
                ImGui.SameLine();
                ImGui.SetNextItemWidth(Math.Max(80, ImGui.CalcTextSize(MethodName).X + 30));

                ScriptComboWindow.SearchItemsReflection($"##时间{GetHashCode()}", _method?.GetMemberName(), ref searchTxt, combo.AllLast, i =>
                {
                    _method = i;
                    MethodName = i.Name;
                });

                ImGui.SameLine();
                ImGui.SetNextItemWidth(50);
                compare = Condition;
                if (ImGui.Combo($"##是否{GetHashCode()}", ref compare, new string[] { "是", "不是" }, 2))
                {
                    Condition = compare;
                }

                ImGui.SameLine();

                var name = _action?.Name ?? string.Empty;
                ImGui.SetNextItemWidth(Math.Max(80, ImGui.CalcTextSize(name).X + 30));

                ScriptComboWindow.SearchCombo($"##技能选择{GetHashCode()}", name, ref searchTxt, combo.AllActions, i =>
                {
                    _action = i;
                    ID = (ActionID)_action.ID;
                });

                ImGui.SameLine();
                ImGui.SetNextItemWidth(50);
                compare = Param1;
                if (ImGui.Combo($"##调整{GetHashCode()}", ref compare, new string[] { "原始", "调整" }, 2))
                {
                    Param1 = compare;
                }

                break;
        }
    }
}

public enum ComboConditionType : int
{
    Bool,
    Byte,
    Time,
    TimeGCD,
    Last,
}

internal static class ComboConditionTypeExtension
{
    internal static string ToName(this ComboConditionType type) => type switch
    {
        ComboConditionType.Bool => "布尔",
        ComboConditionType.Byte => "整数",
        ComboConditionType.Time => "时间",
        ComboConditionType.TimeGCD => "GCD",
        ComboConditionType.Last => "上一个技能",
        _ => string.Empty,
    };
}
