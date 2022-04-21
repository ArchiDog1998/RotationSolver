using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XIVComboPlus.Combos.RPR
{
    internal class RPR_Feature : RPRCombo
    {
        public override string ComboFancyName => "镰刀GCD";

        public override string Description => "替换沉静为镰刀GCD循环";

        protected internal override uint[] ActionIDs => new uint[] { GeneralActions.Repose.ActionID };

        public RPR_Feature()
        {
            this.ShouldSayout = true;
        }

    }
}
