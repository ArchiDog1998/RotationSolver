using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XIVComboPlus.Combos.WHM
{
    internal class WHMRaiseFeature : WHMCombo
    {
        public override string ComboFancyName => "白魔拉人";

        public override string Description => "替换浴血为拉人GCD";

        protected internal override uint[] ActionIDs => new uint[] { GeneralActions.Bloodbath.ActionID };

        private protected override bool EmergercyGCD(byte level, uint lastComboActionID, out BaseAction act)
        {
            if (TargetHelper.DeathPeopleParty.Length > 0)
            {
                if (Actions.Raise.TryUseAction(level, out act)) return true;
            }
            if (GeneralActions.Esuna.TryUseAction(level, out act)) return true;
            return false;
        }
    }
}
