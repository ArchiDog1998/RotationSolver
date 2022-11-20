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

namespace XIVAutoAttack.Combos.Script.Conditions
{
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

        public float Distance { get; set; }

        [JsonIgnore]
        public bool IsTrue
        {
            get
            {
                if (Service.ClientState.LocalPlayer == null) return false;

                BattleChara tar = null;
                if(_action == null)
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
                    case TargetConditionType.Status:
                        result = tar.HasStatus(FromSelf, Status);
                        break;

                    case TargetConditionType.IsBoss:
                        result = tar.IsBoss();
                        break;

                    case TargetConditionType.IsDying:
                        result = tar.IsDying();
                        break;

                    case TargetConditionType.Distance:
                        result = tar.DistanceToPlayer() > Distance;
                        break;
                }

                return Condition ? !result : result;
            }
        }

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

            ScriptComboWindow.DrawCondition(IsTrue);
            ImGui.SameLine();

            var name = _action?.Name ?? (IsTarget ? "目标" : "玩家");
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
                case TargetConditionType.Status:
                    combos = new string[] { "有", "没有" };
                    break;
                case TargetConditionType.IsDying:
                case TargetConditionType.IsBoss:
                    combos = new string[] { "是", "不是" };
                    break;

                case TargetConditionType.Distance:
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
                case TargetConditionType.Status:
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

                case TargetConditionType.Distance:
                    ImGui.SameLine();
                    ImGui.SetNextItemWidth(50);
                    var distance = Distance;
                    if (ImGui.DragFloat($"距离##距离{GetHashCode()}", ref distance))
                    {
                        Distance = Math.Max(0, distance);
                    }
                    ImGui.SameLine();

                    break;

            }

        }
    }

    public enum TargetConditionType : int
    {
        Status,
        IsDying,
        IsBoss,
        Distance,
    }

    internal static class TargetConditionTypeExtension
    {
        internal static string ToName(this TargetConditionType type) => type switch
        {
            TargetConditionType.Status => "状态",
            TargetConditionType.IsDying => "要死了",
            TargetConditionType.IsBoss => "是Boss",
            TargetConditionType.Distance => "距离",
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
}
