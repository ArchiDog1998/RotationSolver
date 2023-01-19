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

    private BaseAction _action { get; set; }
    public ActionID ID { get; set; } = ActionID.None;
    public bool Condition { get; set; }
    public bool FromSelf { get; set; }
    private BaseStatus _status { get; set; }
    public StatusID Status { get; set; }
    public bool IsTarget { get; set; }
    public TargetConditionType TargetConditionType { get; set; }

    public float DistanceOrTime { get; set; }

    public int GCD { get; set; }
    public int Ability { get; set; }

    public bool IsTrue(IScriptCombo combo)
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
    public void Draw(IScriptCombo combo)
    {
        if (ID != ActionID.None && (_action == null || (ActionID)_action.ID != ID))
        {
            _action = combo.AllActions.OfType<BaseAction>().FirstOrDefault(a => (ActionID)a.ID == ID);
        }
        if (Status != StatusID.None && (_status == null || _status.ID != Status))
        {
            _status = AllStatus.FirstOrDefault(a => a.ID == Status);
        }

        ScriptComboWindow.DrawCondition(IsTrue(combo));
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

            ScriptComboWindow.SearchItems(ref searchTxt, combo.AllActions, i =>
            {
                _action = (BaseAction)i;
                ID = (ActionID)_action.ID;
            });

            ImGui.EndCombo();
        }

        ImGui.SameLine();

        var type = (int)TargetConditionType;
        var names = Enum.GetValues<TargetConditionType>().Select(e => e.ToName()).ToArray();
        ImGui.SetNextItemWidth(100);

        if (ImGui.Combo($"##Category{GetHashCode()}", ref type, names, names.Length))
        {
            TargetConditionType = (TargetConditionType)type;
        }

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

                ScriptComboWindow.SearchCombo($"##Status{GetHashCode()}", _status?.Name, ref searchTxt, AllStatus, i =>
                {
                    _status = i;
                    Status = _status.ID;
                });

                ImGui.SameLine();

                var self = FromSelf;
                if (ImGui.Checkbox($"{LocalizationManager.RightLang.Scriptwindow_StatusSelf}##Self{GetHashCode()}", ref self))
                {
                    FromSelf = self;
                }
                if (ImGui.IsItemHovered())
                {
                    ImGui.SetTooltip(LocalizationManager.RightLang.Scriptwindow_StatusSelfDesc);
                }
                break;

            case TargetConditionType.StatusEnd:
                ImGui.SameLine();
                ImGui.SetNextItemWidth(50);

                ScriptComboWindow.SearchCombo($"##Status{GetHashCode()}", _status?.Name, ref searchTxt, AllStatus, i =>
                {
                    _status = i;
                    Status = _status.ID;
                });

                ImGui.SameLine();

                self = FromSelf;
                if (ImGui.Checkbox($"{LocalizationManager.RightLang.Scriptwindow_StatusSelf}##Self{GetHashCode()}", ref self))
                {
                    FromSelf = self;
                }
                if (ImGui.IsItemHovered())
                {
                    ImGui.SetTooltip(LocalizationManager.RightLang.Scriptwindow_StatusSelfDesc);
                }

                ImGui.SameLine();
                ImGui.SetNextItemWidth(50);
                var time = DistanceOrTime;
                if (ImGui.DragFloat($"s##s{GetHashCode()}", ref time))
                {
                    DistanceOrTime = Math.Max(0, time);
                }
                break;


            case TargetConditionType.StatusEndGCD:
                ImGui.SameLine();
                ImGui.SetNextItemWidth(50);

                ScriptComboWindow.SearchCombo($"##Status{GetHashCode()}", _status?.Name, ref searchTxt, AllStatus, i =>
                {
                    _status = i;
                    Status = _status.ID;
                });

                ImGui.SameLine();

                self = FromSelf;
                if (ImGui.Checkbox($"{LocalizationManager.RightLang.Scriptwindow_StatusSelf}##Self{GetHashCode()}", ref self))
                {
                    FromSelf = self;
                }
                if (ImGui.IsItemHovered())
                {
                    ImGui.SetTooltip(LocalizationManager.RightLang.Scriptwindow_StatusSelfDesc);
                }
                ImGui.SameLine();

                ImGui.SetNextItemWidth(50);
                var gcd = GCD;
                if (ImGui.DragInt($"GCD##GCD{GetHashCode()}", ref gcd))
                {
                    GCD = Math.Max(0, gcd);
                }
                ImGui.SameLine();

                ImGui.SetNextItemWidth(50);
                var ability = Ability;
                if (ImGui.DragInt($"{LocalizationManager.RightLang.Scriptwindow_Ability}##Ability{GetHashCode()}", ref ability))
                {
                    Ability = Math.Max(0, ability);
                }
                break;

            case TargetConditionType.Distance:
                ImGui.SameLine();
                ImGui.SetNextItemWidth(50);
                var distance = DistanceOrTime;
                if (ImGui.DragFloat($"m##m{GetHashCode()}", ref distance))
                {
                    DistanceOrTime = Math.Max(0, distance);
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

internal static class TargetConditionTypeExtension
{
    internal static string ToName(this TargetConditionType type) => type switch
    {
        TargetConditionType.HaveStatus => LocalizationManager.RightLang.TargetConditionType_HaveStatus,
        TargetConditionType.IsDying => LocalizationManager.RightLang.TargetConditionType_IsDying,
        TargetConditionType.IsBoss => LocalizationManager.RightLang.TargetConditionType_IsBoss,
        TargetConditionType.Distance => LocalizationManager.RightLang.TargetConditionType_Distance,
        TargetConditionType.StatusEnd => LocalizationManager.RightLang.TargetConditionType_StatusEnd,
        TargetConditionType.StatusEndGCD => LocalizationManager.RightLang.TargetConditionType_StatusEndGCD,
        _ => string.Empty,
    };

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
