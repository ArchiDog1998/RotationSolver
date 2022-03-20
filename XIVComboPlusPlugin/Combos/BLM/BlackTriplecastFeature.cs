using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XIVComboPlus.Combos.BLM
{
    internal class BlackTriplecastFeature : BLMCombo
    {
        public override string ComboFancyName => "防止手残三连";

        public override string Description => "如果当前有三连咏唱，那就赶紧变成黑魔纹！";

        protected internal override uint[] ActionIDs => new uint[] { Actions.Triplecast };

        protected override uint Invoke(uint actionID, uint lastComboActionID, float comboTime, byte level)
        {
            if (BuffDuration(Buffs.Triplecast) != 0)
            {
                if (HasEffect(Buffs.LeyLines))
                {
                    return Actions.BetweenTheLines;
                }
                return Actions.LeyLines;
            }
            return actionID;
        }
    }
}
