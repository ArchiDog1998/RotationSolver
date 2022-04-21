using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XIVComboPlus.Combos.MCH
{
    internal class MCH_Feature : MCHCombo
    {
        public override string ComboFancyName => "机工GCD";

        public override string Description => "替换沉静为持续GCD循环";

        protected internal override uint[] ActionIDs => new uint[] { GeneralActions.Repose.ActionID };

        public MCH_Feature()
        {
            this.ShouldSayout = true;
        }
    }
}
