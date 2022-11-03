using ImGuiScene;
using System;
using XIVAutoAttack.Data;
using XIVAutoAttack.Helpers;
using Action = Lumina.Excel.GeneratedSheets.Action;

namespace XIVAutoAttack.Actions.BaseAction
{
    internal partial class BaseAction : IAction, IDisposable
    {
        private bool _isFriendly;
        private bool _shouldEndSpecial;
        private bool _isDot;
        internal bool EnoughLevel => Service.ClientState.LocalPlayer.Level >= _action.ClassJobLevel;

        public uint ID => _action.RowId;
        public uint AdjustedID => Service.IconReplacer.OriginalHook(ID);

        public TextureWrap Icon { get; }
        public ushort IconID => _action.Icon;
        internal bool IsGeneralGCD { get; }
        internal bool IsRealGCD { get; }

        internal byte CoolDownGroup { get; }

        private Action _action;


        internal BaseAction(uint actionID, bool isFriendly = false, bool shouldEndSpecial = false, bool isDot = false)
        {
            _action = Service.DataManager.GetExcelSheet<Action>().GetRow(actionID);
            _shouldEndSpecial = shouldEndSpecial;
            _isFriendly = isFriendly;
            _isDot = isDot;
            
            Icon = Service.DataManager.GetImGuiTextureIcon(IconID);
            IsGeneralGCD = _action.IsGeneralGCD();
            IsRealGCD = _action.IsRealGCD();
            CoolDownGroup = _action.GetCoolDownGroup();
        }

        public override string ToString()
        {
            return _action.Name;
        }

        public void Dispose()
        {
            Icon.Dispose();
        }

        ~BaseAction()
        {
            Dispose();
        }
    }
}
