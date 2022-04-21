using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XIVComboPlus.Combos.WHM
{
    internal class WHMAreaHealFeature : WHMCombo
    {
        public override string ComboFancyName => "白魔群奶";

        public override string Description => "替换营救为群体奶";

        protected internal override uint[] ActionIDs => new uint[] { GeneralActions.Rescue.ActionID };

        protected override bool CanHealAreaAbility => true;

        protected override bool CanHealAreaSpell => true;
    }
}
