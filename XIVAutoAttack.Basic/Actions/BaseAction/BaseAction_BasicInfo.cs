using ImGuiScene;
using Lumina.Excel.GeneratedSheets;
using System;
using XIVAutoAttack.Data;
using XIVAutoAttack.Helpers;
using Action = Lumina.Excel.GeneratedSheets.Action;

namespace XIVAutoAttack.Actions.BaseAction
{
    public partial class BaseAction : IAction, IDisposable
    {
        private bool _shouldEndSpecial;
        private bool _isFriendly;
        private bool _isDot;
        public bool EnoughLevel => Service.ClientState.LocalPlayer.Level >= _action.ClassJobLevel;
        public string Name => _action.Name;
        public string Description => string.Empty;   
        internal string CateName
        {
            get
            {
                string result = _isFriendly ? "支援" : "攻击";
                result += IsRealGCD ? "GCD" : "能力技";
                if (_isDot) result += " - Dot";
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

        public TextureWrap Texture { get; }
        public bool IsGeneralGCD { get; }
        public bool IsRealGCD { get; }

        internal byte CoolDownGroup { get; }

        private Action _action;

        internal BaseAction(uint actionID, bool isFriendly = false, bool shouldEndSpecial = false, bool isDot = false)
        {
            _action = Service.DataManager.GetExcelSheet<Action>().GetRow(actionID);
            _shouldEndSpecial = shouldEndSpecial;
            _isFriendly = isFriendly;
            _isDot = isDot;
            
            Texture = Service.DataManager.GetImGuiTextureIcon(_action.Icon);
            IsGeneralGCD = _action.IsGeneralGCD();
            IsRealGCD = _action.IsRealGCD();
            CoolDownGroup = _action.GetCoolDownGroup();
        }

        public override string ToString()
        {
            return Name;
        }

        public void Dispose()
        {
            Texture.Dispose();
        }

        ~BaseAction()
        {
            Dispose();
        }
    }
}
