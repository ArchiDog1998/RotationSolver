using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XIVComboPlus.Combos.AST
{
    internal class ASTRaiseCombo : ASTCombo
    {
        public override string ComboFancyName => "占星拉人";

        public override string Description => "替换浴血为拉人GCD";

        protected internal override uint[] ActionIDs => new uint[] { GeneralActions.Bloodbath.ActionID };

        private protected override bool EmergercyGCD(byte level, uint lastComboActionID, out BaseAction act)
        {
            if (Actions.Ascend.TryUseAction(level, out act)) return true;
            return false;
        }
    }
}
