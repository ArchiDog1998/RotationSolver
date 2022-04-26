//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace XIVComboPlus.Combos.DRG
//{
//    internal class DRGAreaDefenceFeature : DRGCombo
//    {
//        public override string ComboFancyName => "龙骑群防";

//        public override string Description => "替换血仇为龙骑的群防技能。";

//        protected internal override uint[] ActionIDs => new uint[] { GeneralActions.Reprisal.ActionID };

//        private protected override bool EmergercyAbility(byte level, byte abilityRemain, BaseAction nextGCD, out BaseAction act)
//        {
//            if (base.EmergercyAbility(level, abilityRemain, nextGCD, out act)) return true;
//            //牵制
//            if (GeneralActions.Feint.TryUseAction(level, out act)) return true;
//            return false;
//        }

//    }
//}
