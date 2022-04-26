//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace XIVComboPlus.Combos.WAR
//{
//    internal class WARAreaDefenceFeature : WARCombo
//    {
//        public override string ComboFancyName => "战士群防";

//        public override string Description => "替换血仇为战士的群防技能。";

//        protected internal override uint[] ActionIDs => new uint[] { GeneralActions.Reprisal.ActionID };

//        private protected override bool EmergercyAbility(byte level, byte abilityRemain, BaseAction nextGCD, out BaseAction act)
//        {
//            if (base.EmergercyAbility(level, abilityRemain, nextGCD, out act)) return true;
//            //摆脱 队友套盾
//            if (Actions.ShakeItOff.TryUseAction(level, out act)) return true;
//            return false;
//        }

//    }
//}
