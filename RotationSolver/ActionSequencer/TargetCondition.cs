using ECommons.DalamudServices;
using ECommons.GameHelpers;
using ECommons.Reflection;
using Lumina.Excel.GeneratedSheets;
using RotationSolver.Localization;
using RotationSolver.TextureItems;
using RotationSolver.UI;
using RotationSolver.Updaters;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace RotationSolver.ActionSequencer;

internal class TargetCondition : ICondition
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

    private BaseAction _action;
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

    public bool IsTrue(ICustomRotation rotation)
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

            case TargetConditionType.Distance:
                result = tar.DistanceToPlayer() > DistanceOrTime;
                break;

            case TargetConditionType.StatusEnd:
                result = !tar.WillStatusEnd(DistanceOrTime, FromSelf, StatusId);
                break;

            case TargetConditionType.StatusEndGCD:
                result = !tar.WillStatusEndGCD((uint)GCD, DistanceOrTime, FromSelf, StatusId);
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
    private float size => 32 * ImGuiHelpers.GlobalScale;
    private int count = 15;
    public void Draw(ICustomRotation rotation)
    {
        ConditionHelper.CheckBaseAction(rotation, ID, ref _action);

        if (StatusId != StatusID.None && (Status == null || Status.RowId != (uint)StatusId))
        {
            Status = AllStatus.FirstOrDefault(a => a.RowId == (uint) StatusId);
        }

        var popUpKey = "Target Condition Pop Up" + GetHashCode().ToString();

        if (_actionsList != null && ImGui.BeginPopup(popUpKey))
        {
            _actionsList.ClearCollapsingHeader();

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

            foreach (var pair in RotationUpdater.GroupActions(rotation.AllBaseActions))
            {
                _actionsList.AddCollapsingHeader(() => pair.Key, () =>
                {
                    var index = 0;
                    foreach (var item in pair.OrderBy(t => t.ID))
                    {
                        if (!item.GetTexture(out var icon)) continue;

                        if (index++ % count != 0)
                        {
                            ImGui.SameLine();
                        }

                        ImGui.BeginGroup();
                        var cursor = ImGui.GetCursorPos();
                        if (RotationConfigWindow.NoPaddingNoColorImageButton(icon.ImGuiHandle, Vector2.One * size, GetHashCode().ToString()))
                        {
                            ID = (ActionID)item.ID;
                            ImGui.CloseCurrentPopup();
                        }
                        RotationConfigWindow.DrawActionOverlay(cursor, size, 1);
                        ImGui.EndGroup();
                    }
                });
            }
            _actionsList.Draw();
            ImGui.EndPopup();
        }

        if (_action != null ? ( _action.GetTexture(out var icon) || IconSet.GetTexture(4, out icon))
            : IconSet.GetTexture(IsTarget ? 16u : 18u, out icon))
        {
            var cursor = ImGui.GetCursorPos();
            if (RotationConfigWindow.NoPaddingNoColorImageButton(icon.ImGuiHandle, Vector2.One * size, GetHashCode().ToString()))
            {
                if (!ImGui.IsPopupOpen(popUpKey)) ImGui.OpenPopup(popUpKey);
            }
            RotationConfigWindow.DrawActionOverlay(cursor, size, 1);

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
                    LocalizationManager.RightLang.ActionSequencer_Have,
                    LocalizationManager.RightLang.ActionSequencer_HaveNot,
                };
                break;
            case TargetConditionType.IsDying:
            case TargetConditionType.IsBoss:
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
                combos = new string[] { ">", "<=" };
                break;
        }

        ImGui.SameLine();

        ImGuiHelper.SetNextWidthWithName(combos[condition]);
        if (ImGui.Combo($"##Comparation{GetHashCode()}", ref condition, combos, combos.Length))
        {
            Condition = condition > 0;
        }

        var popupId = "Status Finding Popup" + GetHashCode().ToString();

        RotationConfigWindow.StatusPopUp(popupId, AllStatus, ref searchTxt, status =>
        {
            Status = status;
            StatusId = (StatusID)Status.RowId;
        });

        void DrawStatusIcon()
        {
            if (IconSet.GetTexture(Status?.Icon ?? 16220, out var icon)
                || IconSet.GetTexture(16220, out icon))
            {
                if (RotationConfigWindow.NoPaddingNoColorImageButton(icon.ImGuiHandle, new Vector2(24, 32) * ImGuiHelpers.GlobalScale, GetHashCode().ToString()))
                {
                    if (!ImGui.IsPopupOpen(popupId)) ImGui.OpenPopup(popupId);
                }
                ImguiTooltips.HoveredTooltip(Status?.Name);
            }
        }

        switch (TargetConditionType)
        {
            case TargetConditionType.HasStatus:
                ImGui.SameLine();
                DrawStatusIcon();

                ImGui.SameLine();

                ImGui.Checkbox($"{LocalizationManager.RightLang.ActionSequencer_StatusSelf}##Self{GetHashCode()}", ref FromSelf);
                ImguiTooltips.HoveredTooltip(LocalizationManager.RightLang.ActionSequencer_StatusSelfDesc);
                break;

            case TargetConditionType.StatusEnd:
                ImGui.SameLine();
                DrawStatusIcon();

                ImGui.SameLine();

                ImGui.Checkbox($"{LocalizationManager.RightLang.ActionSequencer_StatusSelf}##Self{GetHashCode()}", ref FromSelf);

                ImguiTooltips.HoveredTooltip(LocalizationManager.RightLang.ActionSequencer_StatusSelfDesc);

                ConditionHelper.DrawDragFloat($"s##Seconds{GetHashCode()}", ref DistanceOrTime);
                break;


            case TargetConditionType.StatusEndGCD:
                ImGui.SameLine();
                DrawStatusIcon();

                ImGui.SameLine();

                ImGui.Checkbox($"{LocalizationManager.RightLang.ActionSequencer_StatusSelf}##Self{GetHashCode()}", ref FromSelf);

                ImguiTooltips.HoveredTooltip(LocalizationManager.RightLang.ActionSequencer_StatusSelfDesc);

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
                ImGui.InputText($"Ability name##CastingActionName{GetHashCode()}", ref CastingActionName, 100);
                break;

            case TargetConditionType.CastingActionTimeUntil:
                ImGui.SameLine();
                ImGui.SetNextItemWidth(Math.Max(150 * ImGuiHelpers.GlobalScale, ImGui.CalcTextSize(DistanceOrTime.ToString()).X));
                ImGui.InputFloat($"Seconds##CastingActionTimeUntil{GetHashCode()}", ref DistanceOrTime, .1f);
                break;
        }
    }
}

public enum TargetConditionType : byte
{
    HasStatus,
    IsDying,
    IsBoss,
    Distance,
    StatusEnd,
    StatusEndGCD,
    CastingAction,
    CastingActionTimeUntil
}