using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XIVComboPlus.Combos.BLM
{
    internal class BlackUnbralSoulFeature : BLMCombo
    {
        public override string ComboFancyName => "转换状态";

        public override string Description => "替换灵极魂为星灵移位置。";

        protected internal override uint[] ActionIDs => new uint[] { Actions.UmbralSoul.ActionID };

        protected override uint Invoke(uint actionID, uint lastComboActionID, float comboTime, byte level)
        {
            if(Actions.UmbralSoul.TryUseAction(level, out uint act))
            {
                if(level < Actions.Paradox.Level)
                {
                    return act;
                }
                else
                {
                    if(JobGauge.UmbralIceStacks > 2 && JobGauge.UmbralHearts > 2)
                    {
                        return act;
                    }
                }
            }
            return Actions.Transpose.ActionID;
        }
    }
}
