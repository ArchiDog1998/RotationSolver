using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XIVComboPlus.Combos.WAR
{
    internal class WAROnslaughtFeature : WARCombo
    {
        public override string ComboFancyName => "猛攻GCD";

        public override string Description => "替换猛攻为一个会往前飞的GCD";

        protected internal override uint[] ActionIDs => new uint[] {Actions.Onslaught.ActionID};

        protected override uint Invoke(uint actionID, uint lastComboActionID, float comboTime, byte level)
        {
            if (CanInsertAbility && Actions.Onslaught.TryUseAction(level, out uint action, Empty:true)) return action;
            return base.Invoke(actionID, lastComboActionID, comboTime, level);
        }
    }
}
