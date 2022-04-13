using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XIVComboPlus.Combos.BLM
{
    internal class BLMManipulationFeature : BLMCombo
    {
        public override string ComboFancyName => "黑魔以太步";

        public override string Description => "让黑魔以太步，能适应GCD";

        protected internal override uint[] ActionIDs => new uint[] { Actions.AetherialManipulation.ActionID };

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (CanInsertAbility)
            {
                if (Actions.AetherialManipulation.TryUseAction(level, out uint act)) return act;
            }
            return base.Invoke(actionID, lastComboMove, comboTime, level);
        }

    }
}
