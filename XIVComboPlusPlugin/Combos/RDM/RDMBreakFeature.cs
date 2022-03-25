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

            //能力技，必须要有
            if (CanAddAbility(level, lastComboActionID, out act)) return act;

            //如果是合适的时候爆发，那赶紧的！
            if (CanBreak(lastComboActionID, level, out act, true)) return act;


            //整个队伍里面有人死了。
            if (TargetHelper.DeathPeopleAll.Length > 0)
            {
                if (Actions.Verraise.TryUseAction(level, out act)) return act;
            }

            return Actions.Vercure.ActionID;
        }
    }
}
