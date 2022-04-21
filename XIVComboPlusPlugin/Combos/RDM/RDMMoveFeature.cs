using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XIVComboPlus.Combos.RDM
{
    internal class RDMMoveFeature : RDMCombo
    {
        public override string ComboFancyName => "赤魔突进";

        public override string Description => "替换浴血为赤魔突进技能。";

        protected internal override uint[] ActionIDs => new uint[] { GeneralActions.Bloodbath.ActionID };

        private protected override bool FirstActionAbility(byte level, byte abilityRemain, BaseAction nextGCD, out BaseAction act)
        {
            if (base.FirstActionAbility(level, abilityRemain, nextGCD, out act)) return true;
            //摆脱 队友套盾
            if (Actions.CorpsAcorps.TryUseAction(level, out act)) return true;
            return false;
        }

    }
}
