using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XIVComboPlus.Combos.AST
{
    internal class ASTSingleHealFeature : ASTCombo
    {
        public override string ComboFancyName => "占星单奶";

        public override string Description => "替换吉星为必要的单奶。";

        protected internal override uint[] ActionIDs => new uint[] { Actions.Benefic.ActionID};

        protected override uint Invoke(uint actionID, uint lastComboActionID, float comboTime, byte level)
        {
            uint act;

            //在移动，还有光速，肯定用啊！
            if (IsMoving)
            {
                //吉星相位
                if (Actions.AspectedBenefic.TryUseAction(level, out act)) return act;

                if (Actions.Lightspeed.TryUseAction(level, out act)) return act;
            }

            //如果现在可以增加能力技,优先星位合图
            if (CanInsertAbility && Actions.Synastry.TryUseAction(level, out act)) return act;
            if (CanAddAbility(level, true, false, out act)) return act;


            //吉星相位
            if (Actions.AspectedBenefic.TryUseAction(level, out act)) return act;

            //福星
            if (Actions.Benefic2.TryUseAction(level, out act)) return act;

            //吉星
            if (Actions.Benefic.TryUseAction(level, out act)) return act;

            return 0;
        }
    }
}
