using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XIVComboPlus.Combos.RDM
{
    internal class RDMBreakFeature : RDMCombo
    {
        public override string ComboFancyName => "爆发6连";

        public override string Description => "替换短兵相接为爆发用6连";

        protected internal override uint[] ActionIDs => new uint[] { Actions.EnchantedRiposte.ActionID };

        protected override uint Invoke(uint actionID, uint lastComboActionID, float comboTime, byte level)
        {
            uint act;
            if (CanBreak(lastComboActionID, level, out act, true)) return act;

            return Actions.Verraise.ActionID;
        }
    }
}
