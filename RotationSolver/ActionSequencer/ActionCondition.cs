using Dalamud.Interface.Colors;
using RotationSolver.Localization;
using RotationSolver.UI;

namespace RotationSolver.ActionSequencer;

internal class ActionCondition : ICondition
{
    private BaseAction _action;

    public ActionID ID { get; set; } = ActionID.None;

    public ActionConditionType ActionConditionType = ActionConditionType.Elapsed;

    public bool Condition { get; set; }

    public int Param1;
    public int Param2;
    public float Time;

    public bool IsTrue(ICustomRotation combo, bool isActionSequencer)
    {
        if (!isActionSequencer) return false;
        if (!ConditionHelper.CheckBaseAction(combo, ID, ref _action)) return false;

        var result = false;

        switch (ActionConditionType)
        {
            case ActionConditionType.Elapsed:
                result = _action.ElapsedOneChargeAfter(Time); // Bigger
                break;

            case ActionConditionType.ElapsedGCD:
                result = _action.ElapsedOneChargeAfterGCD((uint)Param1, Param2); // Bigger
                break;

            case ActionConditionType.Remain:
                result = !_action.WillHaveOneCharge(Time); //Smaller
                break;

            case ActionConditionType.RemainGCD:
                result = !_action.WillHaveOneChargeGCD((uint)Param1, Param2); // Smaller
                break;

            case ActionConditionType.CanUse:
                var option = CanUseOption.IgnoreTarget;
                if (Param1 > 0) option |= CanUseOption.MustUse;
                if (Param2 > 0) option |= CanUseOption.EmptyOrSkipCombo;
                result = _action.CanUse(out _, option);
                break;

            case ActionConditionType.EnoughLevel:
                result = _action.EnoughLevel;
                break;

            case ActionConditionType.IsCoolDown:
                result = _action.IsCoolingDown;
                break;

            case ActionConditionType.CurrentCharges:
                result = _action.CurrentCharges > Param1;
                break;

            case ActionConditionType.MaxCharges:
                result = _action.MaxCharges > Param1;
                break;
        }

        return Condition ? !result : result;
    }



    [JsonIgnore]
    public float Height => ICondition.DefaultHeight;

    string searchTxt = string.Empty;

    public void Draw(ICustomRotation combo, bool isActionSequencer)
    {
        ConditionHelper.CheckBaseAction(combo, ID, ref _action);

        ImGuiHelper.DrawCondition(IsTrue(combo, isActionSequencer));
        ImGui.SameLine();

        if (!isActionSequencer)
        {
            ImGui.TextColored(ImGuiColors.DPSRed, LocalizationManager.RightLang.ActionSequencer_NotAllowed);
            return;
        }

        var name = _action?.Name ?? string.Empty;
        ImGui.SetNextItemWidth(Math.Max(80, ImGui.CalcTextSize(name).X + 30));

        ImGuiHelper.SearchCombo($"##ActionChoice{GetHashCode()}", name, ref searchTxt, combo.AllBaseActions, i =>
        {
            _action = (BaseAction)i;
            ID = (ActionID)_action.ID;
        });

        ImGui.SameLine();

        ConditionHelper.DrawByteEnum($"##Category{GetHashCode()}", ref ActionConditionType, EnumTranslations.ToName);

        var condition = Condition ? 1 : 0;
        var combos = new string[0];
        switch (ActionConditionType)
        {
            case ActionConditionType.ElapsedGCD:
            case ActionConditionType.RemainGCD:
            case ActionConditionType.Elapsed:
            case ActionConditionType.Remain:
            case ActionConditionType.CurrentCharges:
            case ActionConditionType.MaxCharges:
                combos = new string[] { ">", "<=" };
                break;

            case ActionConditionType.CanUse:
                combos = new string[]
                {
                    LocalizationManager.RightLang.ActionSequencer_Can,
                    LocalizationManager.RightLang.ActionSequencer_Cannot,
                };
                break;

            case ActionConditionType.EnoughLevel:
            case ActionConditionType.IsCoolDown:
                combos = new string[]
                {
                    LocalizationManager.RightLang.ActionSequencer_Is,
                    LocalizationManager.RightLang.ActionSequencer_Isnot,
                };
                break;
        }
        ImGui.SameLine();
        ImGuiHelper.SetNextWidthWithName(combos[condition]);
        if (ImGui.Combo($"##Comparation{GetHashCode()}", ref condition, combos, combos.Length))
        {
            Condition = condition > 0;
        }


        switch (ActionConditionType)
        {
            case ActionConditionType.Elapsed:
            case ActionConditionType.Remain:
                ConditionHelper.DrawDragFloat($"s##Seconds{GetHashCode()}", ref Time);
                break;

            case ActionConditionType.ElapsedGCD:
            case ActionConditionType.RemainGCD:
                if (ConditionHelper.DrawDragInt($"GCD##GCD{GetHashCode()}", ref Param1))
                {
                    Param1 = Math.Max(0, Param1);
                }
                if (ConditionHelper.DrawDragInt($"{LocalizationManager.RightLang.ActionSequencer_TimeOffset}##Ability{GetHashCode()}", ref Param2))
                {
                    Param2 = Math.Max(0, Param2);
                }
                break;

            case ActionConditionType.CanUse:

                ConditionHelper.DrawCheckBox($"{LocalizationManager.RightLang.ActionSequencer_MustUse}##MustUse{GetHashCode()}", ref Param1, LocalizationManager.RightLang.ActionSequencer_MustUseDesc);
                ConditionHelper.DrawCheckBox($"{LocalizationManager.RightLang.ActionSequencer_Empty}##MustUse{GetHashCode()}", ref Param2, LocalizationManager.RightLang.ActionSequencer_EmptyDesc);
                break;

            case ActionConditionType.CurrentCharges:
            case ActionConditionType.MaxCharges:
                if (ConditionHelper.DrawDragInt($"{LocalizationManager.RightLang.ActionSequencer_Charges}##Charges{GetHashCode()}", ref Param1))
                {
                    Param1 = Math.Max(0, Param1);
                }
                break;
        }
    }
}

public enum ActionConditionType : byte
{
    Elapsed,
    ElapsedGCD,
    Remain,
    RemainGCD,
    CanUse,
    EnoughLevel,
    IsCoolDown,
    CurrentCharges,
    MaxCharges,
}
