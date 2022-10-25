using System;
using Action = Lumina.Excel.GeneratedSheets.Action;

namespace XIVAutoAttack.Actions.BaseAction
{
    internal partial class BaseAction : IAction
    {
        private bool _isFriendly;
        private bool _shouldEndSpecial;
        private bool _isDot;
        [Obsolete("尽量别访问Level了，用EnoughLevel来判断")]
        private byte Level => Action.ClassJobLevel;
        internal bool EnoughLevel => Service.ClientState.LocalPlayer.Level >= Level;

        public uint ID => Action.RowId;
        public uint AdjustedID => Service.IconReplacer.OriginalHook(ID);
        internal bool IsGeneralGCD { get; }
        internal bool IsRealGCD { get; }

        private byte CoolDownGroup { get; }

        internal Action Action { get; }
        internal BaseAction(uint actionID, bool isFriendly = false, bool shouldEndSpecial = false, bool isDot = false)
        {
            Action = Service.DataManager.GetExcelSheet<Action>().GetRow(actionID);
            _shouldEndSpecial = shouldEndSpecial;
            _isFriendly = isFriendly;
            _isDot = isDot;
            IsGeneralGCD = Action.CooldownGroup == GCDCooldownGroup;
            IsRealGCD = IsGeneralGCD || Action.AdditionalCooldownGroup == GCDCooldownGroup;

            //CoolDownGroup = GCDCooldownGroup;
            CoolDownGroup = IsGeneralGCD ? Action.AdditionalCooldownGroup : Action.CooldownGroup;

            if (Action.PrimaryCostType == 3 || Action.PrimaryCostType == 4)
            {
                MPNeed = Action.PrimaryCostValue * 100u;
            }
            else if (Action.SecondaryCostType == 3 || Action.SecondaryCostType == 4)
            {
                MPNeed = Action.SecondaryCostValue * 100u;
            }
            else
            {
                MPNeed = 0;
            }
            _isDot = isDot;
        }
    }
}
