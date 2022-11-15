using FFXIVClientStructs.FFXIV.Client.Game;
using ImGuiScene;
using Lumina.Excel.GeneratedSheets;
using System;
using XIVAutoAttack.Data;
using XIVAutoAttack.Helpers;
using Action = Lumina.Excel.GeneratedSheets.Action;

namespace XIVAutoAttack.Actions.BaseAction
{
    internal partial class BaseAction : IAction
    {
        private bool _shouldEndSpecial;
        private bool _isFriendly;
        private bool _isEot;
        /// <summary>
        /// 玩家当前等级是否大于等于技能可用等级
        /// </summary>
        internal bool EnoughLevel => Service.ClientState.LocalPlayer.Level >= _action.ClassJobLevel;
        public string Name => _action.Name;
        public string Description => string.Empty;
        public string Author => string.Empty;
        internal string CateName
        {
            get
            {
                string result;

                if (_isFriendly)
                {
                    result = "支援";
                    if (_isEot)
                    {
                        result += "Hot";
                    }
                }
                else
                {
                    result = "攻击";

                    if (_isEot)
                    {
                        result += "Dot";
                    }
                }
                result += IsRealGCD ? "-GCD" : "-能力技";
                return result;
            }
        }
        public bool IsEnabled
        {
            get => !Service.Configuration.DiabledActions.Contains((uint)ID);
            set
            {
                if (value)
                {
                    Service.Configuration.DiabledActions.Remove((uint)ID);
                }
                else
                {
                    Service.Configuration.DiabledActions.Add((uint)ID);
                }
            }
        }
        /// <summary>
        /// 技能ID
        /// </summary>
        public uint ID => _action.RowId;
        /// <summary>
        /// 调整后的ID(真实技能ID)
        /// </summary>
        public uint AdjustedID => (uint)Service.IconReplacer.OriginalHook((ActionID)ID);

        public uint IconID { get; }

        private bool IsGeneralGCD { get; }

        internal bool IsRealGCD { get; }

        private byte CoolDownGroup { get; }

        /// <summary>
        /// 真实咏唱时间
        /// </summary>
        internal unsafe float CastTime => ActionManager.GetAdjustedCastTime(ActionType.Spell, AdjustedID) / 1000f;

        internal virtual EnemyLocation EnermyLocation
        {
            get
            {
                if (StatusHelper.ActionLocations.TryGetValue((ActionID)ID, out var location))
                {
                    return location.Loc;
                }
                return EnemyLocation.None;
            }
        }
        internal virtual unsafe uint MPNeed
        {
            get
            {
                var mp = (uint)ActionManager.GetActionCost(ActionType.Spell, AdjustedID, 0, 0, 0, 0);
                if (mp < 100) return 0;
                return mp;
            }
        }

        Action _action;
        internal BaseAction(ActionID actionID, bool isFriendly = false, bool shouldEndSpecial = false, bool isEot = false)
        {
            _action = Service.DataManager.GetExcelSheet<Action>().GetRow((uint)actionID);
            _shouldEndSpecial = shouldEndSpecial;
            _isFriendly = isFriendly;
            _isEot = isEot;

            IconID =_action.Icon;
            IsGeneralGCD = _action.IsGeneralGCD();
            IsRealGCD = _action.IsRealGCD();
            CoolDownGroup = _action.GetCoolDownGroup();
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
