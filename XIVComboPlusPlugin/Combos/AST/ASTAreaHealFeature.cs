using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XIVComboPlus.Combos.AST
{
    internal class ASTAreaHealFeature : ASTCombo
    {
        public override string ComboFancyName => "占星群奶";

        public override string Description => "替换阳星为必要的群奶。";

        protected internal override uint[] ActionIDs => new uint[] { Actions.Helios.ActionID};
        protected override uint Invoke(uint actionID, uint lastComboActionID, float comboTime, byte level)
        {
            uint act = 0;

            //在移动，还有光速，肯定用啊！
            if (IsMoving)
            {
                //吉星相位
                if (Actions.AspectedHelios.TryUseAction(level, out act)) return act;

                if (Actions.Lightspeed.TryUseAction(level, out act)) return act;
            }

            //如果现在可以增加能力技，天宫图啊
            if (CanInsertAbility && Actions.Horoscope.TryUseAction(level, out act)) return act;
            if (CanAddAbility(level, false, true, out act)) return act;


            //阳星相位
            if (Actions.AspectedHelios.TryUseAction(level, out act)) return act;

            //阳星
            if (Actions.Helios.TryUseAction(level, out act)) return act;

            return 0;
        }
    }
}
