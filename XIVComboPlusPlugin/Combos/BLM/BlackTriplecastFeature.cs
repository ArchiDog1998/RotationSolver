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

        public override string Description => "如果当前有即刻，那就赶紧变成不能按！";

        protected internal override uint[] ActionIDs => new uint[] { Actions.Triplecast.ActionID };

        protected override uint Invoke(uint actionID, uint lastComboActionID, float comboTime, byte level)
        {
            if (Actions.Triplecast.TryUseAction(level, out uint act)) return act;
            if (Actions.BetweenTheLines.TryUseAction(level, out act)) return act;
            return Actions.Leylines.ActionID;
        }
    }
}
