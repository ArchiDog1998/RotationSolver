using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XIVComboPlus.Combos.AST
{
    internal class ASTSingleDefenceFeature : ASTCombo
    {
        public override string ComboFancyName => "占星单防";

        public override string Description => "替换铁壁为占星的单防技能。";

        protected internal override uint[] ActionIDs => new uint[] { GeneralActions.Rampart.ActionID };

        private protected override bool EmergercyAbility(byte level, byte abilityRemain, BaseAction nextGCD, out BaseAction act)
        {
            if (base.EmergercyAbility(level, abilityRemain, nextGCD, out act)) return true;
            //给T减伤，这个很重要。
            if (Actions.Exaltation.TryUseAction(level, out act)) return true;
            return false;

        }
    }
}
