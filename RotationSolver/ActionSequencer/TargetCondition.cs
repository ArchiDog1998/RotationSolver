using ECommons.DalamudServices;
using ECommons.GameHelpers;
using Lumina.Excel.GeneratedSheets;
using RotationSolver.Localization;
using RotationSolver.UI;

namespace RotationSolver.ActionSequencer;

internal class TargetCondition : BaseCondition
{
    private static Status[] _allStatus = null;
    private static Status[] AllStatus
    {
        get
        {
            _allStatus ??= Enum.GetValues<StatusID>().Select(id => Service.GetSheet<Status>().GetRow((uint)id)).ToArray();
            return _allStatus;
        }
    }

    private IBaseAction _action;
    public ActionID ID { get; set; } = ActionID.None;

    public bool Condition;
    public bool FromSelf;
    private Status Status { get; set; }
    public StatusID StatusId { get; set; }
    public bool IsTarget;
    public TargetConditionType TargetConditionType;

    public float DistanceOrTime;
    public int GCD;

    public string CastingActionName = string.Empty;

    public override bool IsTrueInside(ICustomRotation rotation)
    {
        if (!Player.Available) return false;

        BattleChara tar;
        if (_action != null)
        {
            _action.CanUse(out _, CanUseOption.EmptyOrSkipCombo | CanUseOption.MustUse
                |  CanUseOption.IgnoreTarget);
            tar = _action.Target;
        }
        else
        {
            tar = IsTarget ? Svc.Targets.Target as BattleChara : Player.Object;
            tar ??= Player.Object;
        }

        if (tar == null) return false;

        var result = false;

        switch (TargetConditionType)
        {
            case TargetConditionType.HasStatus:
                result = tar.HasStatus(FromSelf, StatusId);
                break;

            case TargetConditionType.IsBoss:
                result = tar.IsBoss();
                break;

            case TargetConditionType.IsDying:
                result = tar.IsDying();
                break;

            case TargetConditionType.InCombat:
                result = tar.InCombat();
                break;

            case TargetConditionType.Distance:
                result = tar.DistanceToPlayer() > DistanceOrTime;
                break;

            case TargetConditionType.StatusEnd:
                result = !tar.WillStatusEnd(DistanceOrTime, FromSelf, StatusId);
                break;

            case TargetConditionType.StatusEndGCD:
                result = !tar.WillStatusEndGCD((uint)GCD, DistanceOrTime, FromSelf, StatusId);
                break;

            case TargetConditionType.DeadTime:
                result = tar.GetDeadTime() > DistanceOrTime;
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
                result = castTime > DistanceOrTime + DataCenter.WeaponRemain;
                break;
        }

        return Condition ? !result : result;
    }

    string searchTxt = string.Empty;
    private readonly CollapsingHeaderGroup _actionsList = new()
    {
        HeaderSize = 12,
    };

    public override void DrawInside(ICustomRotation rotation)
    {
        ConditionHelper.CheckBaseAction(rotation, ID, ref _action);

        if (StatusId != StatusID.None && (Status == null || Status.RowId != (uint)StatusId))
        {
            Status = AllStatus.FirstOrDefault(a => a.RowId == (uint) StatusId);
        }

        var popUpKey = "Target Condition Pop Up" + GetHashCode().ToString();

        ConditionHelper.ActionSelectorPopUp(popUpKey, _actionsList, rotation, item => ID = (ActionID)item.ID, ()=>
        {
            if (ImGui.Selectable(LocalizationManager.RightLang.ActionSequencer_Target))
            {
                _action = null;
                ID = ActionID.None;
                IsTarget = true;
            }

            if (ImGui.Selectable(LocalizationManager.RightLang.ActionSequencer_Player))
            {
                _action = null;
                ID = ActionID.None;
                IsTarget = false;
            }
        });

        if (_action != null ? ( _action.GetTexture(out var icon) || IconSet.GetTexture(4, out icon))
            : IconSet.GetTexture(IsTarget ? 16u : 18u, out icon))
        {
            var cursor = ImGui.GetCursorPos();
            if (ImGuiHelper.NoPaddingNoColorImageButton(icon.ImGuiHandle, Vector2.One * ConditionHelper.IconSize, GetHashCode().ToString()))
            {
                if (!ImGui.IsPopupOpen(popUpKey)) ImGui.OpenPopup(popUpKey);
            }
            ImGuiHelper.DrawActionOverlay(cursor, ConditionHelper.IconSize, 1);

            var description = _action != null ? string.Format(LocalizationManager.RightLang.ActionSequencer_ActionTarget, _action.Name)
                : IsTarget
                ? LocalizationManager.RightLang.ActionSequencer_Target
                : LocalizationManager.RightLang.ActionSequencer_Player;
            ImguiTooltips.HoveredTooltip(description);
        }

        ImGui.SameLine();
        ConditionHelper.DrawByteEnum($"##Category{GetHashCode()}", ref TargetConditionType, EnumTranslations.ToName);

        var condition = Condition ? 1 : 0;
        var combos = Array.Empty<string>();
        switch (TargetConditionType)
        {
            case TargetConditionType.HasStatus:
                combos = new string[]
                {
                    LocalizationManager.RightLang.ActionSequencer_Has,
                    LocalizationManager.RightLang.ActionSequencer_HasNot,
                };
                break;
            case TargetConditionType.IsDying:
            case TargetConditionType.IsBoss:
            case TargetConditionType.InCombat:
            case TargetConditionType.CastingAction:
                combos = new string[]
                {
                    LocalizationManager.RightLang.ActionSequencer_Is,
                    LocalizationManager.RightLang.ActionSequencer_Isnot,
                };
                break;

            case TargetConditionType.CastingActionTimeUntil:
            case TargetConditionType.Distance:
            case TargetConditionType.StatusEnd:
            case TargetConditionType.DeadTime:
                combos = new string[] { ">", "<=" };
                break;
        }

        ImGui.SameLine();

        if(ImGuiHelper.SelectableCombo($"##Comparation{GetHashCode()}", combos, ref condition))
        {
            Condition = condition > 0;
        }

        var popupId = "Status Finding Popup" + GetHashCode().ToString();

        RotationConfigWindow.StatusPopUp(popupId, AllStatus, ref searchTxt, status =>
        {
            Status = status;
            StatusId = (StatusID)Status.RowId;
        }, size: ConditionHelper.IconSizeRaw);

        void DrawStatusIcon()
        {
            if (IconSet.GetTexture(Status?.Icon ?? 16220, out var icon)
                || IconSet.GetTexture(16220, out icon))
            {
                if (ImGuiHelper.NoPaddingNoColorImageButton(icon.ImGuiHandle, new Vector2(ConditionHelper.IconSize * 3 / 4, ConditionHelper.IconSize) * ImGuiHelpers.GlobalScale, GetHashCode().ToString()))
                {
                    if (!ImGui.IsPopupOpen(popupId)) ImGui.OpenPopup(popupId);
                }
                ImguiTooltips.HoveredTooltip(Status?.Name ?? string.Empty);
            }
        }

        switch (TargetConditionType)
        {
            case TargetConditionType.HasStatus:
                ImGui.SameLine();
                DrawStatusIcon();

                ImGui.SameLine();

                var check = FromSelf ? 1 : 0;
                if(ImGuiHelper.SelectableCombo($"From Self {GetHashCode()}", new string[]
                {
                    LocalizationManager.RightLang.ActionSequencer_StatusAll,
                    LocalizationManager.RightLang.ActionSequencer_StatusSelf,
                }, ref check))
                {
                    FromSelf = check != 0;
                }
                break;

            case TargetConditionType.StatusEnd:
                ImGui.SameLine();
                DrawStatusIcon();

                ImGui.SameLine();

                check = FromSelf ? 1 : 0;
                if(ImGuiHelper.SelectableCombo($"From Self {GetHashCode()}", new string[]
                {
                    LocalizationManager.RightLang.ActionSequencer_StatusAll,
                    LocalizationManager.RightLang.ActionSequencer_StatusSelf,
                }, ref check))
                {
                    FromSelf = check != 0;
                }

                ConditionHelper.DrawDragFloat($"s##Seconds{GetHashCode()}", ref DistanceOrTime);
                break;


            case TargetConditionType.StatusEndGCD:
                ImGui.SameLine();
                DrawStatusIcon();

                ImGui.SameLine();

                check = FromSelf ? 1 : 0;
                if(ImGuiHelper.SelectableCombo($"From Self {GetHashCode()}", new string[]
                {
                    LocalizationManager.RightLang.ActionSequencer_StatusAll,
                    LocalizationManager.RightLang.ActionSequencer_StatusSelf,
                }, ref check))
                {
                    FromSelf = check != 0;
                }

                ConditionHelper.DrawDragInt($"GCD##GCD{GetHashCode()}", ref GCD);
                ConditionHelper.DrawDragFloat($"{LocalizationManager.RightLang.ActionSequencer_TimeOffset}##Ability{GetHashCode()}", ref DistanceOrTime);
                break;

            case TargetConditionType.Distance:
                if (ConditionHelper.DrawDragFloat($"yalm##yalm{GetHashCode()}", ref DistanceOrTime))
                {
                    DistanceOrTime = Math.Max(0, DistanceOrTime);
                }
                break;

            case TargetConditionType.CastingAction:
                ImGui.SameLine();
                ImGuiHelper.SetNextWidthWithName(CastingActionName);
                ImGui.InputText($"Ability name##CastingActionName{GetHashCode()}", ref CastingActionName, 128);
                break;

            case TargetConditionType.CastingActionTimeUntil:
                ImGui.SameLine();
                ImGui.SetNextItemWidth(Math.Max(150 * ImGuiHelpers.GlobalScale, ImGui.CalcTextSize(DistanceOrTime.ToString()).X));
                ImGui.InputFloat($"s##CastingActionTimeUntil{GetHashCode()}", ref DistanceOrTime, .1f);
                break;
        }
    }
}

public enum TargetConditionType : byte
{
    HasStatus,
    IsDying,
    IsBoss,
    InCombat,
    Distance,
    StatusEnd,
    StatusEndGCD,
    CastingAction,
    CastingActionTimeUntil,
    DeadTime,
}