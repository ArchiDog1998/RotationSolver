using Dalamud.Logging;

using RotationSolver.Localization;
using RotationSolver.UI;

namespace RotationSolver.Timeline;

internal class TargetCondition : ICondition
{
    private static BaseStatus[] _allStatus = null;
    private static BaseStatus[] AllStatus
    {
        get
        {
            if (_allStatus == null)
            {
                _allStatus = Enum.GetValues<StatusID>().Select(id => new BaseStatus(id)).ToArray();
            }
            return _allStatus;
        }
    }

    private BaseAction _action;
    public ActionID ID { get; set; } = ActionID.None;

    public bool Condition;
    public bool FromSelf;
    private BaseStatus _status { get; set; }
    public StatusID Status { get; set; }
    public bool IsTarget;
    public TargetConditionType TargetConditionType;

    public float DistanceOrTime;

    public int GCD;
    public int Ability;

    public string CastingActionName = string.Empty;
    public float CastingActionTime = 0.8f;

    public bool IsTrue(ICustomRotation combo)
    {
        if (Service.Player == null) return false;

        BattleChara tar = null;
        if (_action != null)
        {
            _action.CanUse(out _, CanUseOption.EmptyOrSkipCombo | CanUseOption.MustUse);
            tar = _action.Target;
        }
        else
        {
            tar = IsTarget ? Service.TargetManager.Target as BattleChara : Service.Player;
            tar ??= Service.Player;
        }

        if (tar == null) return false;

        var result = false;

        switch (TargetConditionType)
        {
            case TargetConditionType.HaveStatus:
                result = tar.HasStatus(FromSelf, Status);
                break;

            case TargetConditionType.IsBoss:
                result = tar.IsBoss();
                break;

            case TargetConditionType.IsDying:
                result = tar.IsDying();
                break;

            case TargetConditionType.Distance:
                result = tar.DistanceToPlayer() > DistanceOrTime;
                break;

            case TargetConditionType.StatusEnd:
                result = !tar.WillStatusEnd(DistanceOrTime, FromSelf, Status);
                break;

            case TargetConditionType.StatusEndGCD:
                result = !tar.WillStatusEndGCD((uint)GCD, Ability, FromSelf, Status);
                break;

            case TargetConditionType.CastingAction:
                if (string.IsNullOrEmpty(CastingActionName) || tar.CastActionId == 0)
                {
                    result = false;
                    break;
                }

                var castName = Service.GetSheet<Lumina.Excel.GeneratedSheets.Action>().GetRow(tar.CastActionId)?.Name.ToString();

                result = CastingActionName == castName;
                break;

            case TargetConditionType.CastingActionTimeUntil:

                if (!tar.IsCasting || tar.CastActionId == 0)
                {
                    result = false;
                    break;
                }

                float castTime = tar.TotalCastTime - tar.CurrentCastTime;
                result = castTime > CastingActionTime;
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

        if (Status != StatusID.None && (_status == null || _status.ID != Status))
        {
            _status = AllStatus.FirstOrDefault(a => a.ID == Status);
        }

        ImGuiHelper.DrawCondition(IsTrue(combo));
        ImGui.SameLine();

        var name = _action != null ? string.Format(LocalizationManager.RightLang.Timeline_ActionTarget, _action.Name)
            : IsTarget
            ? LocalizationManager.RightLang.Timeline_Target
            : LocalizationManager.RightLang.Timeline_Player;

        ImGui.SetNextItemWidth(Math.Max(80, ImGui.CalcTextSize(name).X + 30));
        if (ImGui.BeginCombo($"##ActionChoice{GetHashCode()}", name, ImGuiComboFlags.HeightLargest))
        {
            if (ImGui.Selectable(LocalizationManager.RightLang.Timeline_Target))
            {
                _action = null;
                ID = ActionID.None;
                IsTarget = true;
            }

            if (ImGui.Selectable(LocalizationManager.RightLang.Timeline_Player))
            {
                _action = null;
                ID = ActionID.None;
                IsTarget = false;
            }

            ImGuiHelper.SearchItems(ref searchTxt, combo.AllBaseActions, i =>
            {
                _action = (BaseAction)i;
                ID = (ActionID)_action.ID;
            });

            ImGui.EndCombo();
        }

        ImGui.SameLine();
        ConditionHelper.DrawIntEnum($"##Category{GetHashCode()}", ref TargetConditionType, EnumTranslations.ToName);

        var condition = Condition ? 1 : 0;
        var combos = new string[0];
        switch (TargetConditionType)
        {
            case TargetConditionType.HaveStatus:
                combos = new string[]
                {
                    LocalizationManager.RightLang.Timeline_Have,
                    LocalizationManager.RightLang.Timeline_Havenot,
                };
                break;
            case TargetConditionType.IsDying:
            case TargetConditionType.IsBoss:
            case TargetConditionType.CastingAction:
                combos = new string[]
                {
                    LocalizationManager.RightLang.Timeline_Is,
                    LocalizationManager.RightLang.Timeline_Isnot,
                };
                break;

            case TargetConditionType.CastingActionTimeUntil:
            case TargetConditionType.Distance:
            case TargetConditionType.StatusEnd:
                combos = new string[] { ">", "<=" };
                break;
        }

        ImGui.SameLine();
        //ImGui.SetNextItemWidth(60);
        ImGui.SetNextItemWidth(Math.Max(80, ImGui.CalcTextSize(name).X + 30));
        if (ImGui.Combo($"##Comparation{GetHashCode()}", ref condition, combos, combos.Length))
        {
            Condition = condition > 0;
        }

        switch (TargetConditionType)
        {
            case TargetConditionType.HaveStatus:
                ImGui.SameLine();
                ImGuiHelper.SetNextWidthWithName(_status?.Name);
                ImGuiHelper.SearchCombo($"##Status{GetHashCode()}", _status?.Name, ref searchTxt, AllStatus, i =>
                {
                    _status = i;
                    Status = _status.ID;
                });

                ImGui.SameLine();

                ImGui.Checkbox($"{LocalizationManager.RightLang.Timeline_StatusSelf}##Self{GetHashCode()}", ref FromSelf);
                if (ImGui.IsItemHovered())
                {
                    ImGui.SetTooltip(LocalizationManager.RightLang.Timeline_StatusSelfDesc);
                }
                break;

            case TargetConditionType.StatusEnd:
                ImGui.SameLine();
                ImGuiHelper.SetNextWidthWithName(_status?.Name);
                ImGuiHelper.SearchCombo($"##Status{GetHashCode()}", _status?.Name, ref searchTxt, AllStatus, i =>
                {
                    _status = i;
                    Status = _status.ID;
                });

                ImGui.SameLine();

                ImGui.Checkbox($"{LocalizationManager.RightLang.Timeline_StatusSelf}##Self{GetHashCode()}", ref FromSelf);
                if (ImGui.IsItemHovered())
                {
                    ImGui.SetTooltip(LocalizationManager.RightLang.Timeline_StatusSelfDesc);
                }

                ConditionHelper.DrawDragFloat($"s##Seconds{GetHashCode()}", ref DistanceOrTime);
                break;


            case TargetConditionType.StatusEndGCD:
                ImGui.SameLine();
                ImGuiHelper.SetNextWidthWithName(_status?.Name);
                ImGuiHelper.SearchCombo($"##Status{GetHashCode()}", _status?.Name, ref searchTxt, AllStatus, i =>
                {
                    _status = i;
                    Status = _status.ID;
                });

                ImGui.SameLine();

                ImGui.Checkbox($"{LocalizationManager.RightLang.Timeline_StatusSelf}##Self{GetHashCode()}", ref FromSelf);
                if (ImGui.IsItemHovered())
                {
                    ImGui.SetTooltip(LocalizationManager.RightLang.Timeline_StatusSelfDesc);
                }

                ConditionHelper.DrawDragInt($"GCD##GCD{GetHashCode()}", ref GCD);
                ConditionHelper.DrawDragInt($"{LocalizationManager.RightLang.Timeline_Ability}##Ability{GetHashCode()}", ref Ability);
                break;

            case TargetConditionType.Distance:
                if (ConditionHelper.DrawDragFloat($"m##m{GetHashCode()}", ref DistanceOrTime))
                {
                    DistanceOrTime = Math.Max(0, DistanceOrTime);
                }
                break;

            case TargetConditionType.CastingAction:
                ImGui.SameLine();
                //ImGui.SetNextItemWidth(Math.Max(150, ImGui.CalcTextSize(CastingActionName).X));
                ImGuiHelper.SetNextWidthWithName(CastingActionName);
                ImGui.InputText($"Ability name##CastingActionName{GetHashCode()}", ref CastingActionName, 100);
                break;

            case TargetConditionType.CastingActionTimeUntil:
                ImGui.SameLine();
                ImGui.SetNextItemWidth(Math.Max(150, ImGui.CalcTextSize(CastingActionTime.ToString()).X));
                ImGui.InputFloat($"Seconds##CastingActionTimeUntil{GetHashCode()}", ref CastingActionTime, .1f);
                //ConditionHelper.DrawDragFloat($"s##Seconds{GetHashCode()}", ref CastingActionTime);

                break;
        }
    }
}

public enum TargetConditionType : int
{
    HaveStatus,
    IsDying,
    IsBoss,
    Distance,
    StatusEnd,
    StatusEndGCD,
    CastingAction,
    CastingActionTimeUntil
}