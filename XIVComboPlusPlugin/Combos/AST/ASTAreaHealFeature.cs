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

        public override string Description => "替换营救为必要的群奶。";

        protected override bool CanHealAreaAbility => true;

        protected override bool CanHealAreaSpell => true;

        protected internal override uint[] ActionIDs => new uint[] { GeneralActions.Rescue.ActionID};
    }
}
