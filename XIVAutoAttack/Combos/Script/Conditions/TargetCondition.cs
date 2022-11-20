using Dalamud.Game.ClientState.Objects.Types;
using ImGuiNET;
using Lumina.Data.Parsing;
using Lumina.Excel.GeneratedSheets;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XIVAutoAttack.Actions.BaseAction;
using XIVAutoAttack.Data;
using XIVAutoAttack.Helpers;
using XIVAutoAttack.Windows;

namespace XIVAutoAttack.Combos.Script.Conditions;

internal class TargetCondition : ICondition
{
    private static BaseStatus[] _allStatus = null;
    private static BaseStatus[] AllStatus
    {
        get
        {
            if( _allStatus == null)
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
            if(_action != null)
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
            _action = combo.AllActions.FirstOrDefault(a => (ActionID)a.ID == ID);
        }
        if (Status != StatusID.None && (_status == null || _status.ID != Status))
        {
            _status = AllStatus.FirstOrDefault(a => a.ID == Status);
        }

        ScriptComboWindow.DrawCondition(IsTrue(combo));
        ImGui.SameLine();

        var name = _action != null ? _action.Name + "的目标" : IsTarget ? "目标" : "玩家";
        ImGui.SetNextItemWidth(Math.Max(80, ImGui.CalcTextSize(name).X + 30));
        if (ImGui.BeginCombo($"##技能选择{GetHashCode()}", name))
        {
            if (ImGui.Selectable("目标"))
            {
                _action = null;
                ID = ActionID.None;
                IsTarget = true;
            }

            if (ImGui.Selectable("玩家"))
            {
                _action = null;
                ID = ActionID.None;
                IsTarget = false;
            }

            ScriptComboWindow.SearchItems(ref searchTxt, combo.AllActions, i =>
            {
                _action = i;
                ID = (ActionID)_action.ID;
            });

            ImGui.EndCombo();
        }

        ImGui.SameLine();

        var type = (int)TargetConditionType;
        var names = Enum.GetValues<TargetConditionType>().Select(e => e.ToName()).ToArray();
        ImGui.SetNextItemWidth(100);

        if (ImGui.Combo($"##类型{GetHashCode()}", ref type, names, names.Length))
        {
            TargetConditionType = (TargetConditionType)type;
        }

        var condition = Condition ? 1 : 0;
        var combos = new string[0];
        switch (TargetConditionType)
        {
            case TargetConditionType.HaveStatus:
                combos = new string[] { "有", "没有" };
                break;
            case TargetConditionType.IsDying:
            case TargetConditionType.IsBoss:
                combos = new string[] { "是", "不是" };
                break;

            case TargetConditionType.Distance:
            case TargetConditionType.StatusEnd:
                combos = new string[] { ">", "<=" };
                break;
        }

        ImGui.SameLine();
        ImGui.SetNextItemWidth(60);
        if (ImGui.Combo($"##大小情况{GetHashCode()}", ref condition, combos, combos.Length))
        {
            Condition = condition > 0;
        }

        switch (TargetConditionType)
        {
            case TargetConditionType.HaveStatus:
                ImGui.SameLine();
                ImGui.SetNextItemWidth(50);

                if (ImGui.BeginCombo($"##状态{GetHashCode()}", _status?.Name))
                {
                    ScriptComboWindow.SearchItems(ref searchTxt, AllStatus, i =>
                    {
                        _status = i;
                        Status = _status.ID;
                    });

                    ImGui.EndCombo();
                }

                ImGui.SameLine();

                var self = FromSelf;
                if (ImGui.Checkbox($"自身##自身{GetHashCode()}", ref self))
                {
                    FromSelf = self;
                }
                if (ImGui.IsItemHovered())
                {
                    ImGui.SetTooltip("该状态是否是自己赋予的");
                }
                break;

            case TargetConditionType.StatusEnd:
                ImGui.SameLine();
                ImGui.SetNextItemWidth(50);

                if (ImGui.BeginCombo($"##状态{GetHashCode()}", _status?.Name))
                {
                    ScriptComboWindow.SearchItems(ref searchTxt, AllStatus, i =>
                    {
                        _status = i;
                        Status = _status.ID;
                    });

                    ImGui.EndCombo();
                }

                ImGui.SameLine();

                self = FromSelf;
                if (ImGui.Checkbox($"自身##自身{GetHashCode()}", ref self))
                {
                    FromSelf = self;
                }
                if (ImGui.IsItemHovered())
                {
                    ImGui.SetTooltip("该状态是否是自己赋予的");
                }

                ImGui.SameLine();
                ImGui.SetNextItemWidth(50);
                var time = DistanceOrTime;
                if (ImGui.DragFloat($"秒##秒{GetHashCode()}", ref time))
                {
                    DistanceOrTime = Math.Max(0, time);
                }
                break;


            case TargetConditionType.StatusEndGCD:
                ImGui.SameLine();
                ImGui.SetNextItemWidth(50);

                if (ImGui.BeginCombo($"##状态{GetHashCode()}", _status?.Name))
                {
                    ScriptComboWindow.SearchItems(ref searchTxt, AllStatus, i =>
                    {
                        _status = i;
                        Status = _status.ID;
                    });

                    ImGui.EndCombo();
                }

                ImGui.SameLine();

                self = FromSelf;
                if (ImGui.Checkbox($"自身##自身{GetHashCode()}", ref self))
                {
                    FromSelf = self;
                }
                if (ImGui.IsItemHovered())
                {
                    ImGui.SetTooltip("该状态是否是自己赋予的");
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
                if (ImGui.DragInt($"能力##AbilityD{GetHashCode()}", ref ability))
                {
                    Ability = Math.Max(0, ability);
                }
                break;

            case TargetConditionType.Distance:
                ImGui.SameLine();
                ImGui.SetNextItemWidth(50);
                var distance = DistanceOrTime;
                if (ImGui.DragFloat($"米##米{GetHashCode()}", ref distance))
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
        TargetConditionType.HaveStatus => "有状态",
        TargetConditionType.IsDying => "要死了",
        TargetConditionType.IsBoss => "是Boss",
        TargetConditionType.Distance => "距离",
        TargetConditionType.StatusEnd => "状态结束",
        TargetConditionType.StatusEndGCD => "状态结束GCD",
        _ => string.Empty,
    };

}

internal class BaseStatus : ITexture
{
    public Status _status;
    public uint IconID  => _status.Icon;
    public StatusID ID => (StatusID)_status.RowId;
    public string Name => $"{_status.Name}[{_status.RowId}]";

    public BaseStatus(StatusID id)
    {
        _status = Service.DataManager.GetExcelSheet<Status>().GetRow((uint)id);
    }
}
