using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XIVComboPlus.Combos.WHM
{
    internal class WHMSingleHealFeature : WHMCombo
    {
        public override string ComboFancyName => "白魔单奶";

        public override string Description => "替换醒梦为白魔的单奶技能。";

        protected internal override uint[] ActionIDs => new uint[] { GeneralActions.LucidDreaming.ActionID };

        protected override bool CanHealSingleAbility => true;
        protected override bool CanHealSingleSpell => true;
    }
}
