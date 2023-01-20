using ImGuiNET;
using Newtonsoft.Json;
using System;
using System.Linq;
using RotationSolver.Actions.BaseAction;
using RotationSolver.Data;
using RotationSolver.Windows;
using RotationSolver.Localization;
using RotationSolver.Helpers;
using RotationSolver.Rotations.CustomRotation;
using RotationSolver.Actions;
using Lumina.Data.Parsing;

namespace RotationSolver.Timeline;

internal class ActionCondition : ICondition
{
    private BaseAction _action;

    public ActionID ID { get; set; } = ActionID.None;

    public ActionConditonType ActionConditonType = ActionConditonType.Elapsed;

    public bool Condition { get; set; }

    public int Param1;
    public int Param2;
    public float Time;


    public bool IsTrue(ICustomRotation combo)
    {
        if (!ConditionHelper.CheckBaseAction(combo, ID, ref _action)) return false;

        var result = false;

        switch (ActionConditonType)
        {
            case ActionConditonType.Elapsed:
                result = _action.ElapsedAfter(Time); // Bigger
                break;

            case ActionConditonType.ElapsedGCD:
                result = _action.ElapsedAfterGCD((uint)Param1, (uint)Param2); // Bigger
                break;

            case ActionConditonType.Remain:
                result = !_action.WillHaveOneCharge(Time); //Smaller
                break;

            case ActionConditonType.RemainGCD:
                result = !_action.WillHaveOneChargeGCD((uint)Param1, (uint)Param2); // Smaller
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

    public void Draw(ICustomRotation combo)
    {
        ConditionHelper.CheckBaseAction(combo, ID, ref _action);

        ImGuiHelper.DrawCondition(IsTrue(combo));
        ImGui.SameLine();

        var name = _action?.Name ?? string.Empty;
        ImGui.SetNextItemWidth(Math.Max(80, ImGui.CalcTextSize(name).X + 30));

        ImGuiHelper.SearchCombo($"##ActionChoice{GetHashCode()}", name, ref searchTxt, combo.AllActions, i =>
        {
            _action = (BaseAction)i;
            ID = (ActionID)_action.ID;
        });


        ImGui.SameLine();

        ConditionHelper.DrawIntEnum($"##Category{GetHashCode()}", ref ActionConditonType, EnumTranslations.ToName);

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
                ConditionHelper.DrawDragFloat($"s##Seconds{GetHashCode()}", ref Time);
                break;

            case ActionConditonType.ElapsedGCD:
            case ActionConditonType.RemainGCD:
                if (ConditionHelper.DrawDragInt($"GCD##GCD{GetHashCode()}", ref Param1))
                {
                    Param1 = Math.Max(0, Param1);
                }
                if (ConditionHelper.DrawDragInt($"{LocalizationManager.RightLang.Scriptwindow_Ability}##Ability{GetHashCode()}", ref Param2))
                {
                    Param2 = Math.Max(0, Param2);
                }
                break;

            case ActionConditonType.ShouldUse:

                ConditionHelper.DrawCheckBox($"{LocalizationManager.RightLang.Scriptwindow_MustUse}##MustUse{GetHashCode()}", ref Param1, LocalizationManager.RightLang.Scriptwindow_MustUseDesc);
                ConditionHelper.DrawCheckBox($"{LocalizationManager.RightLang.Scriptwindow_Empty}##MustUse{GetHashCode()}", ref Param2, LocalizationManager.RightLang.Scriptwindow_EmptyDesc);
                break;

            case ActionConditonType.CurrentCharges:
            case ActionConditonType.MaxCharges:
                if (ConditionHelper.DrawDragInt($"{LocalizationManager.RightLang.Scriptwindow_Charges}##Charges{GetHashCode()}", ref Param1))
                {
                    Param1 = Math.Max(0, Param1);
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
