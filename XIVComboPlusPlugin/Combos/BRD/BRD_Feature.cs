using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XIVComboPlus.Combos.BRD
{
    internal class BRD_Feature : BRDCombo
    {
        public override string ComboFancyName => "诗人GCD";

        public override string Description => "替换沉静为诗人GCD循环";

        protected internal override uint[] ActionIDs => new uint[] { GeneralActions.Repose.ActionID };

        public BRD_Feature()
        {
            this.ShouldSayout = true;
        }
    }
}
