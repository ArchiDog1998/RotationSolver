using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XIVComboPlus.Combos.AST
{
    internal class AST_Feature : ASTCombo
    {
        public override string ComboFancyName => "占星GCD";

        public override string Description => "替换凶星为自动占星GCD";

        protected internal override uint[] ActionIDs => new uint[] {Actions.Malefic.ActionID};
    }
}
