using ImGuiNET;
using Newtonsoft.Json;
using RotationSolver.Actions.BaseAction;
using RotationSolver.Basic.Data;
using RotationSolver.Basic.Rotations;
using RotationSolver.Localization;
using RotationSolver.UI;
using System;

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
                result = _action.ElapsedOneChargeAfter(Time); // Bigger
                break;

            case ActionConditonType.ElapsedGCD:
                result = _action.ElapsedOneChargeAfterGCD((uint)Param1, (uint)Param2); // Bigger
                break;

            case ActionConditonType.Remain:
                result = !_action.WillHaveOneCharge(Time); //Smaller
                break;

            case ActionConditonType.RemainGCD:
                result = !_action.WillHaveOneChargeGCD((uint)Param1, (uint)Param2); // Smaller
                break;

            case ActionConditonType.ShouldUse:
                var option = CanUseOption.None;
                if (Param1 > 0) option |= CanUseOption.MustUse;
                if (Param2 > 0) option |= CanUseOption.EmptyOrSkipCombo;
                result = _action.CanUse(out _, option);
                break;

            case ActionConditonType.EnoughLevel:
                result = _action.EnoughLevel;
                break;

            case ActionConditonType.IsCoolDown:
                result = _action.IsCoolingDown;
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

        ImGuiHelper.SearchCombo($"##ActionChoice{GetHashCode()}", name, ref searchTxt, combo.AllBaseActions, i =>
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
                    LocalizationManager.RightLang.Timeline_Can,
                    LocalizationManager.RightLang.Timeline_Cannot,
                };
                break;

            case ActionConditonType.EnoughLevel:
            case ActionConditonType.IsCoolDown:
                combos = new string[]
                {
                    LocalizationManager.RightLang.Timeline_Is,
                    LocalizationManager.RightLang.Timeline_Isnot,
                };
                break;
        }
        ImGui.SameLine();
        ImGuiHelper.SetNextWidthWithName(combos[condition]);
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
                if (ConditionHelper.DrawDragInt($"{LocalizationManager.RightLang.Timeline_Ability}##Ability{GetHashCode()}", ref Param2))
                {
                    Param2 = Math.Max(0, Param2);
                }
                break;

            case ActionConditonType.ShouldUse:

                ConditionHelper.DrawCheckBox($"{LocalizationManager.RightLang.Timeline_MustUse}##MustUse{GetHashCode()}", ref Param1, LocalizationManager.RightLang.Timeline_MustUseDesc);
                ConditionHelper.DrawCheckBox($"{LocalizationManager.RightLang.Timeline_Empty}##MustUse{GetHashCode()}", ref Param2, LocalizationManager.RightLang.Timeline_EmptyDesc);
                break;

            case ActionConditonType.CurrentCharges:
            case ActionConditonType.MaxCharges:
                if (ConditionHelper.DrawDragInt($"{LocalizationManager.RightLang.Timeline_Charges}##Charges{GetHashCode()}", ref Param1))
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
