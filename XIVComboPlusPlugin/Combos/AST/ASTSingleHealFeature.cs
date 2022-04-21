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

        public override string Description => "替换醒梦为必要的单奶。";

        protected override bool CanHealSingleAbility => true;
        protected override bool CanHealSingleSpell => true;

        protected internal override uint[] ActionIDs => new uint[] { GeneralActions.LucidDreaming.ActionID};
    }
}
