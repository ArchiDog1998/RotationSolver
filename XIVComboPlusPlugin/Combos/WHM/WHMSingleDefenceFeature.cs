using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XIVComboPlus.Combos.WHM
{
    internal class WHMSingleDefenceFeature : WHMCombo
    {
        public override string ComboFancyName => "白魔单防";

        public override string Description => "替换铁壁为白魔的单防技能。";

        protected internal override uint[] ActionIDs => new uint[] { GeneralActions.Rampart.ActionID };

        private protected override bool EmergercyAbility(byte level, byte abilityRemain, BaseAction nextGCD, out BaseAction act)
        {
            if (base.EmergercyAbility(level, abilityRemain, nextGCD, out act)) return true;
            //水流幕
            if (Actions.Aquaveil.TryUseAction(level, out act)) return true;
            return false;
        }

    }
}
