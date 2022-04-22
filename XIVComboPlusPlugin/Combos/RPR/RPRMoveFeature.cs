using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XIVComboPlus.Combos.RPR
{
    internal class RPRMoveFeature : RPRCombo
    {
        public override string ComboFancyName => "镰刀突进";

        public override string Description => "替换即刻咏唱为一个会往前飞的";

        protected internal override uint[] ActionIDs => new uint[] { GeneralActions.Swiftcast.ActionID };

        private protected override bool ForAttachAbility(byte level, byte abilityRemain, out BaseAction act)
        {
            //地狱入境
            if (Actions.HellsIngress.TryUseAction(level, out act, Empty: true)) return true;
            return base.ForAttachAbility(level, abilityRemain, out act);
        }

        private protected override bool GeneralGCD(byte level, uint lastComboActionID, out BaseAction act)
        {
            //地狱入境
            if (Actions.HellsIngress.TryUseAction(level, out act)) return true;
            return base.GeneralGCD(level, lastComboActionID, out act);
        }
    }
}
