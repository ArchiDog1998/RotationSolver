using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XIVComboPlus.Combos.BRD
{
    internal class BRDAreaDefenceFeature : BRDCombo
    {
        public override string ComboFancyName => "诗人群防";

        public override string Description => "替换血仇为诗人的群防技能。";

        protected internal override uint[] ActionIDs => new uint[] { GeneralActions.Reprisal.ActionID };

        private protected override bool FirstActionAbility(byte level, byte abilityRemain, BaseAction nextGCD, out BaseAction act)
        {
            if (base.FirstActionAbility(level, abilityRemain, nextGCD, out act)) return true;
            //行吟
            if (Actions.Troubadour.TryUseAction(level, out act)) return true;

            return false;
        }

    }
}
