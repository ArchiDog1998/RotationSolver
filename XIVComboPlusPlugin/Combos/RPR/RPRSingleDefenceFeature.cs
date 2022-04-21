using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XIVComboPlus.Combos.RPR
{
    internal class RPRSingleDefenceFeature : RPRCombo
    {
        public override string ComboFancyName => "镰刀单防";

        public override string Description => "替换铁壁为白魔的镰刀技能。";

        protected internal override uint[] ActionIDs => new uint[] { GeneralActions.Rampart.ActionID };

        private protected override bool FirstActionAbility(byte level, byte abilityRemain, BaseAction nextGCD, out BaseAction act)
        {
            if (base.FirstActionAbility(level, abilityRemain, nextGCD, out act)) return true;
            //神秘纹
            if (Actions.ArcaneCrest.TryUseAction(level, out act)) return true;
            return false;
        }

    }
}
