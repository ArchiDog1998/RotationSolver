using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XIVComboPlus.Combos.RDM
{
    internal class RDM_Feature : RDMCombo
    {
        public override string ComboFancyName => "赤魔GCD";

        public override string Description => "替换沉静相接为自动GCD";

        protected internal override uint[] ActionIDs => new uint[] { GeneralActions.Repose.ActionID };

        public RDM_Feature()
        {
            this.ShouldSayout = true;
        }
    }
}
