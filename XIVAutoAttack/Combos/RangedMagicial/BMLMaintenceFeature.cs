//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace XIVComboPlus.Combos.BLM
//{
//    internal class BMLMaintenceFeature : BLMCombo
//    {
//        public override string ComboFancyName => "黑魔保持状态";

//        public override string Description => "把营救变成维持状态的好东西";

//        protected internal override uint[] ActionIDs => new uint[] { GeneralActions.Rescue.ActionID };
//        private protected override bool GeneralGCD(byte level, uint lastComboActionID, out BaseAction act)
//        {
//            if (Actions.UmbralSoul.TryUseAction(level, out act)) return true;

//            if (level >= Actions.UmbralSoul.Level)
//            {
//                if (JobGauge.InAstralFire)
//                {
//                    if (Actions.Transpose.TryUseAction(level, out act)) return true;
//                }
//            }
//            else
//            {
//                if (Actions.Transpose.TryUseAction(level, out act)) return true;
//            }

//            return base.GeneralGCD(level, lastComboActionID, out act);
//        }
//    }
//}
