using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XIVComboPlus.Combos.WHM
{
    internal class WHMAreaDefenceFeature : WHMCombo
    {
        public override string ComboFancyName => "白魔群防";

        public override string Description => "替换血仇为白魔的群防技能。";

        protected internal override uint[] ActionIDs => new uint[] { GeneralActions.Reprisal.ActionID };

        private protected override bool EmergercyAbility(byte level, byte abilityRemain, BaseAction nextGCD, out BaseAction act)
        {
            if (base.EmergercyAbility(level, abilityRemain, nextGCD, out act)) return true;
            //节制
            if (Actions.Temperance.TryUseAction(level, out act)) return true;
            //礼仪之铃
            if (Actions.LiturgyoftheBell.TryUseAction(level, out act)) return true;
            return false;
        }
    }
}
