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

            if (Base(level, out act)) return act;
            if (HealSingle(level, out act)) return act;

            return 0;
        }
    }
}
