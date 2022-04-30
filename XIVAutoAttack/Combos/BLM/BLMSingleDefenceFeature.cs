//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace XIVComboPlus.Combos.BLM
//{
//    internal class BLMSingleDefenceFeature : BLMCombo
//    {
//        public override string ComboFancyName => "黑魔单防";

//        public override string Description => "替换铁壁为黑魔的单防技能。";

//        protected internal override uint[] ActionIDs => new uint[] { GeneralActions.Rampart.ActionID };

//        private protected override bool EmergercyAbility(byte level, byte abilityRemain, BaseAction nextGCD, out BaseAction act)
//        {
//            if (base.EmergercyAbility(level, abilityRemain, nextGCD, out act)) return true;
//            //加个混乱
//            if (GeneralActions.Addle.TryUseAction(level, out act)) return true;

//            //加个魔罩
//            if (Actions.Manaward.TryUseAction(level, out act)) return true; 
//            return false;

//        }

//    }
//}
