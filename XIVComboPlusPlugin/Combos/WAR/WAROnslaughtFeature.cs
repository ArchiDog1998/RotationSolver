using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XIVComboPlus.Combos.WAR
{
    internal class WAROnslaughtFeature : WARCombo
    {
        public override string ComboFancyName => "战士突进";

        public override string Description => "替换浴血为一个会往前飞的GCD";

        protected internal override uint[] ActionIDs => new uint[] { GeneralActions.Bloodbath.ActionID };

        private protected override bool ForAttachAbility(byte level, byte abilityRemain, out BaseAction act)
        {
            //突进
            if (Actions.Onslaught.TryUseAction(level, out act, Empty: true)) return true;
            return base.ForAttachAbility(level, abilityRemain, out act);
        }

        private protected override bool AttackGCD(byte level, uint lastComboActionID, out BaseAction act)
        {
            //放个大 蛮荒崩裂 会往前飞
            if (Actions.PrimalRend.TryUseAction(level, out act)) return true;
            return base.AttackGCD(level, lastComboActionID, out act);
        }
    }
}
