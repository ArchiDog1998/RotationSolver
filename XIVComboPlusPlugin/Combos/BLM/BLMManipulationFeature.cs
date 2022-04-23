using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XIVComboPlus.Combos.BLM
{
    internal class BLMManipulationFeature : BLMCombo
    {
        public override string ComboFancyName => "黑魔以太步";

        public override string Description => "替换醒梦为黑魔以太步，能适应GCD";

        protected internal override uint[] ActionIDs => new uint[] { GeneralActions.LucidDreaming.ActionID };

        private protected override bool EmergercyAbility(byte level, byte abilityRemain, BaseAction nextGCD, out BaseAction act)
        {
            if (Actions.AetherialManipulation.TryUseAction(level, out act)) return true;
            return base.EmergercyAbility(level, abilityRemain, nextGCD, out act);
        }

    }
}
