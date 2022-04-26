//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace XIVComboPlus.Combos.RDM
//{
//    internal class RDMAreaDefenceFeature:RDMCombo
//    {
//        public override string ComboFancyName => "赤魔群防";

//        public override string Description => "替换血仇为赤魔的群防技能。";

//        protected internal override uint[] ActionIDs => new uint[] { GeneralActions.Reprisal.ActionID };

//        private protected override bool EmergercyAbility(byte level, byte abilityRemain, BaseAction nextGCD, out BaseAction act)
//        {
//            if (base.EmergercyAbility(level, abilityRemain, nextGCD, out act)) return true;
//            //混乱
//            if (GeneralActions.Addle.TryUseAction(level, out act)) return true;
//            return false;
//        }

//    }
//}
