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
            get => !Service.Configuration.DiabledActions.Contains(ID);
            set
            {
                if (value)
                {
                    Service.Configuration.DiabledActions.Remove(ID);
                }
                else
                {
                    Service.Configuration.DiabledActions.Add(ID);
                }
            }
        }
        public uint ID => _action.RowId;
        public uint AdjustedID => Service.IconReplacer.OriginalHook(ID);

        public uint IconID { get; }
        internal bool IsGeneralGCD { get; }
        internal bool IsRealGCD { get; }

        internal byte CoolDownGroup { get; }

        private Action _action;


        internal BaseAction(uint actionID, bool isFriendly = false, bool shouldEndSpecial = false, bool isEot = false)
        {
            _action = Service.DataManager.GetExcelSheet<Action>().GetRow(actionID);
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
