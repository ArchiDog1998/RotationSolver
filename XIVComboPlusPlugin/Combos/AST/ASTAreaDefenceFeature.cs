using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XIVComboPlus.Combos.AST
{
    internal class ASTAreaDefenceFeature : ASTCombo
    {
        public override string ComboFancyName => "占星群防";

        public override string Description => "替换血仇为占星的群防技能。";

        protected internal override uint[] ActionIDs => new uint[] { GeneralActions.Reprisal.ActionID };

        private protected override bool EmergercyAbility(byte level, byte abilityRemain, BaseAction nextGCD, out BaseAction act)
        {
            if (base.EmergercyAbility(level, abilityRemain, nextGCD, out act)) return true;
            //来个命运之轮
            if(Actions.CollectiveUnconscious.TryUseAction(level, out act)) return true;

            return false;
        }
    }
}
