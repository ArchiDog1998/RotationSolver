using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XIVComboPlus.Combos.RDM
{
    internal class RDMHealFeature : RDMCombo
    {
        public override string ComboFancyName => "治疗救命";

        public override string Description => "看看整个团队有没有死人，如果有，就救人啦！";

        protected internal override uint[] ActionIDs => new uint[] { Actions.Vercure.ActionID };

        protected override uint Invoke(uint actionID, uint lastComboActionID, float comboTime, byte level)
        {
            uint act;

            //能力技，必须要有
            if (CanAddAbility(level, lastComboActionID, out act)) return act;

            //整个队伍里面有人死了。
            if (TargetHelper.DeathPeopleAll.Length > 0)
            {
                if (Actions.Verraise.TryUseAction(level, out act)) return act;
            }

            return Actions.Vercure.ActionID;
        }
    }
}
