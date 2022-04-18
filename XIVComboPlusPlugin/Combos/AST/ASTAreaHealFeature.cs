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

            if (Base(level, out act)) return act;
            if (HealArea(level, out act)) return act;

            return 0;
        }
    }
}
