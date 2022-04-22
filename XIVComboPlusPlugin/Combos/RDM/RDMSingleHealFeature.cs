using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XIVComboPlus.Combos.RDM
{
    internal class RDMSingleHealFeature : RDMCombo
    {
        public override string ComboFancyName => "赤魔单奶";

        public override string Description => "替换醒梦为必要的单奶。";

        protected internal override uint[] ActionIDs => new uint[] { GeneralActions.LucidDreaming.ActionID };

        private protected override bool GeneralGCD(byte level, uint lastComboActionID, out BaseAction act)
        {
            if (Actions.Vercure.TryUseAction(level, out act, mustUse: true)) return true;
            return base.GeneralGCD(level, lastComboActionID, out act);
        }
    }
}
