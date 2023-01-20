using Dalamud.Game.ClientState.Objects.Types;
using ImGuiNET;
using Lumina.Excel.GeneratedSheets;
using Newtonsoft.Json;
using System;
using System.Linq;
using RotationSolver.Actions.BaseAction;
using RotationSolver.Helpers;
using RotationSolver.Data;
using RotationSolver.Windows;
using RotationSolver.Localization;
using RotationSolver.Rotations.CustomRotation;

namespace RotationSolver.Rotations.Script.Conditions;

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

    public bool IsTrue(ICustomRotation combo)
    {
        if (Service.ClientState.LocalPlayer == null) return false;

        BattleChara tar = null;
        if (_action != null)
        {
            _action.ShouldUse(out _, true);
            tar = _action.Target;
        }
        else
        {
            tar = IsTarget ? (BattleChara)Service.TargetManager.Target : Service.ClientState.LocalPlayer;
            tar ??= Service.ClientState.LocalPlayer;
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
                result = !tar.WillStatusEndGCD((uint)GCD, (uint)Ability, FromSelf, Status);
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

        var name = _action != null ? string.Format(LocalizationManager.RightLang.Scriptwindow_ActionTarget, _action.Name)
            : IsTarget
            ? LocalizationManager.RightLang.Scriptwindow_Target
            : LocalizationManager.RightLang.Scriptwindow_Player;

        ImGui.SetNextItemWidth(Math.Max(80, ImGui.CalcTextSize(name).X + 30));
        if (ImGui.BeginCombo($"##ActionChoice{GetHashCode()}", name, ImGuiComboFlags.HeightLargest))
        {
            if (ImGui.Selectable(LocalizationManager.RightLang.Scriptwindow_Target))
            {
                _action = null;
                ID = ActionID.None;
                IsTarget = true;
            }

            if (ImGui.Selectable(LocalizationManager.RightLang.Scriptwindow_Player))
            {
                _action = null;
                ID = ActionID.None;
                IsTarget = false;
            }

            ImGuiHelper.SearchItems(ref searchTxt, combo.AllActions, i =>
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
                    LocalizationManager.RightLang.Scriptwindow_Have,
                    LocalizationManager.RightLang.Scriptwindow_Havenot,
                };
                break;
            case TargetConditionType.IsDying:
            case TargetConditionType.IsBoss:
                combos = new string[]
                {
                    LocalizationManager.RightLang.Scriptwindow_Is,
                    LocalizationManager.RightLang.Scriptwindow_Isnot,
                };
                break;

            case TargetConditionType.Distance:
            case TargetConditionType.StatusEnd:
                combos = new string[] { ">", "<=" };
                break;
        }

        ImGui.SameLine();
        ImGui.SetNextItemWidth(60);
        if (ImGui.Combo($"##Comparation{GetHashCode()}", ref condition, combos, combos.Length))
        {
            Condition = condition > 0;
        }

        switch (TargetConditionType)
        {
            case TargetConditionType.HaveStatus:
                ImGui.SameLine();
                ImGui.SetNextItemWidth(50);
                ImGuiHelper.SearchCombo($"##Status{GetHashCode()}", _status?.Name, ref searchTxt, AllStatus, i =>
                {
                    _status = i;
                    Status = _status.ID;
                });

                ImGui.SameLine();

                ImGui.Checkbox($"{LocalizationManager.RightLang.Scriptwindow_StatusSelf}##Self{GetHashCode()}", ref FromSelf);
                if (ImGui.IsItemHovered())
                {
                    ImGui.SetTooltip(LocalizationManager.RightLang.Scriptwindow_StatusSelfDesc);
                }
                break;

            case TargetConditionType.StatusEnd:
                ImGui.SameLine();
                ImGui.SetNextItemWidth(50);

                ImGuiHelper.SearchCombo($"##Status{GetHashCode()}", _status?.Name, ref searchTxt, AllStatus, i =>
                {
                    _status = i;
                    Status = _status.ID;
                });

                ImGui.SameLine();

                ImGui.Checkbox($"{LocalizationManager.RightLang.Scriptwindow_StatusSelf}##Self{GetHashCode()}", ref FromSelf);
                if (ImGui.IsItemHovered())
                {
                    ImGui.SetTooltip(LocalizationManager.RightLang.Scriptwindow_StatusSelfDesc);
                }

                ConditionHelper.DrawDragFloat($"s##Seconds{GetHashCode()}", ref DistanceOrTime);
                break;


            case TargetConditionType.StatusEndGCD:
                ImGui.SameLine();
                ImGui.SetNextItemWidth(50);

                ImGuiHelper.SearchCombo($"##Status{GetHashCode()}", _status?.Name, ref searchTxt, AllStatus, i =>
                {
                    _status = i;
                    Status = _status.ID;
                });

                ImGui.SameLine();

                ImGui.Checkbox($"{LocalizationManager.RightLang.Scriptwindow_StatusSelf}##Self{GetHashCode()}", ref FromSelf);
                if (ImGui.IsItemHovered())
                {
                    ImGui.SetTooltip(LocalizationManager.RightLang.Scriptwindow_StatusSelfDesc);
                }

                ConditionHelper.DrawDragInt($"GCD##GCD{GetHashCode()}", ref GCD);
                ConditionHelper.DrawDragInt($"{LocalizationManager.RightLang.Scriptwindow_Ability}##Ability{GetHashCode()}", ref Ability);
                break;

            case TargetConditionType.Distance:
                if(ConditionHelper.DrawDragFloat($"m##m{GetHashCode()}", ref DistanceOrTime))
                {
                    DistanceOrTime = Math.Max(0, DistanceOrTime);
                }
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
}


internal class BaseStatus : ITexture
{
    public Status _status;
    public uint IconID => _status.Icon;
    public StatusID ID => (StatusID)_status.RowId;
    public string Name => $"{_status.Name}[{_status.RowId}]";

    public BaseStatus(StatusID id)
    {
        _status = Service.DataManager.GetExcelSheet<Status>().GetRow((uint)id);
    }
}
