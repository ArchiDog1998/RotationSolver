using System;
using XIVAutoAttack.Helpers;
using Action = Lumina.Excel.GeneratedSheets.Action;

namespace XIVAutoAttack.Actions.BaseAction
{
    internal partial class BaseAction : IAction
    {
        private bool _isFriendly;
        private bool _shouldEndSpecial;
        private bool _isDot;
        internal bool EnoughLevel => Service.ClientState.LocalPlayer.Level >= Action.ClassJobLevel;

        public uint ID => Action.RowId;
        public uint AdjustedID => Service.IconReplacer.OriginalHook(ID);

        internal bool IsGeneralGCD { get; }
        internal bool IsRealGCD { get; }

        internal byte CoolDownGroup { get; }

        private Action Action { get; }

        internal BaseAction(uint actionID, bool isFriendly = false, bool shouldEndSpecial = false, bool isDot = false)
        {
            Action = Service.DataManager.GetExcelSheet<Action>().GetRow(actionID);
            _shouldEndSpecial = shouldEndSpecial;
            _isFriendly = isFriendly;
            _isDot = isDot;

            IsGeneralGCD = Action.IsGeneralGCD();
            IsRealGCD = Action.IsRealGCD();
            CoolDownGroup = Action.GetCoolDownGroup();


            if (Action.PrimaryCostType is 3 or 4)
            {
                MPNeed = Action.PrimaryCostValue * 100u;
            }
            else if (Action.SecondaryCostType is 3 or 4)
            {
                MPNeed = Action.SecondaryCostValue * 100u;
            }
            else
            {
                MPNeed = 0;
            }
        }

        public override string ToString()
        {
            return Action.Name;
        }
    }
}
