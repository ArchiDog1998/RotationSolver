using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XIVComboPlus.Combos.WAR
{
    internal class WAR_Feature : WARCombo
    {
        public override string ComboFancyName => "战士GCD";

        public override string Description => "替换重劈为连续的GCD";

        protected internal override uint[] ActionIDs => new uint[] {Actions.HeavySwing.ActionID};
    }
}
