using ImGuiNET;
using Newtonsoft.Json;
using System;
using System.Linq;
using XIVAutoAttack.Actions.BaseAction;
using XIVAutoAttack.Data;
using XIVAutoAttack.Localization;
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
        if (ID != ActionID.None && (_action == null || (ActionID)_action.ID != ID))
        {
            _action = combo.AllActions.OfType<BaseAction>().FirstOrDefault(a => (ActionID)a.ID == ID);
        }
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
            _action = combo.AllActions.OfType<BaseAction>().FirstOrDefault(a => (ActionID)a.ID == ID);
        }

        ScriptComboWindow.DrawCondition(IsTrue(combo));
        ImGui.SameLine();

        var name = _action?.Name ?? string.Empty;
        ImGui.SetNextItemWidth(Math.Max(80, ImGui.CalcTextSize(name).X + 30));

        ScriptComboWindow.SearchCombo($"##ActionChoice{GetHashCode()}", name, ref searchTxt, combo.AllActions, i =>
        {
            _action = (BaseAction)i;
            ID = (ActionID)_action.ID;
        });


        ImGui.SameLine();

        var type = (int)ActionConditonType;
        var names = Enum.GetValues<ActionConditonType>().Select(e => e.ToName()).ToArray();
        ImGui.SetNextItemWidth(100);

        if (ImGui.Combo($"##Category{GetHashCode()}", ref type, names, names.Length))
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
                combos = new string[]
                {
                    LocalizationManager.RightLang.Scriptwindow_Can,
                    LocalizationManager.RightLang.Scriptwindow_Cannot,
                };
                break;

            case ActionConditonType.EnoughLevel:
            case ActionConditonType.IsCoolDown:
                combos = new string[]
                {
                    LocalizationManager.RightLang.Scriptwindow_Is,
                    LocalizationManager.RightLang.Scriptwindow_Isnot,
                };
                break;
        }
        ImGui.SameLine();
        ImGui.SetNextItemWidth(60);
        if (ImGui.Combo($"##Comparation{GetHashCode()}", ref condition, combos, combos.Length))
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
                if (ImGui.DragFloat($"s##Seconds{GetHashCode()}", ref time))
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
                if (ImGui.DragInt($"{LocalizationManager.RightLang.Scriptwindow_Ability}##Ability{GetHashCode()}", ref ability))
                {
                    Param2 = Math.Max(0, ability);
                }
                break;

            case ActionConditonType.ShouldUse:
                ImGui.SameLine();

                var must = Param1 > 0;
                if (ImGui.Checkbox($"{LocalizationManager.RightLang.Scriptwindow_MustUse}##MustUse{GetHashCode()}", ref must))
                {
                    Param1 = must ? 1 : 0;
                }
                ImGui.SameLine();

                var empty = Param2 > 0;
                if (ImGui.Checkbox($"{LocalizationManager.RightLang.Scriptwindow_Empty}##Empty{GetHashCode()}", ref empty))
                {
                    Param2 = empty ? 1 : 0;
                }
                break;

            case ActionConditonType.CurrentCharges:
            case ActionConditonType.MaxCharges:
                ImGui.SameLine();

                ImGui.SetNextItemWidth(50);
                var charge = Param1;
                if (ImGui.DragInt($"{LocalizationManager.RightLang.Scriptwindow_Charges}##Charges{GetHashCode()}", ref charge))
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
        ActionConditonType.Elapsed => LocalizationManager.RightLang.ActionConditionType_Elapsed,
        ActionConditonType.ElapsedGCD => LocalizationManager.RightLang.ActionConditionType_ElapsedGCD,
        ActionConditonType.Remain => LocalizationManager.RightLang.ActionConditionType_Remain,
        ActionConditonType.RemainGCD => LocalizationManager.RightLang.ActionConditionType_RemainGCD,
        ActionConditonType.ShouldUse => LocalizationManager.RightLang.ActionConditionType_ShouldUse,
        ActionConditonType.EnoughLevel => LocalizationManager.RightLang.ActionConditionType_EnoughLevel,
        ActionConditonType.IsCoolDown => LocalizationManager.RightLang.ActionConditionType_IsCoolDown,
        ActionConditonType.CurrentCharges => LocalizationManager.RightLang.ActionConditionType_CurrentCharges,
        ActionConditonType.MaxCharges => LocalizationManager.RightLang.ActionConditionType_MaxCharges,
        _ => string.Empty,
    };
}
