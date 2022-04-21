//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace XIVComboPlus.Combos.BLM
//{
//    internal class BML_MovingFeature : BLMCombo
//    {
//        public override string ComboFancyName => "黑魔保持状态";

//        public override string Description => "把灵极魂变成维持状态的好东西";

//        protected internal override uint[] ActionIDs => new uint[] { Actions.UmbralSoul.ActionID };

//        protected override uint Invoke(uint actionID, uint lastComboActionID, float comboTime, byte level)
//        {
//            //bool isFull = JobGauge.UmbralIceStacks > 2 && JobGauge.UmbralHearts > 2;

//            if (Actions.UmbralSoul.TryUseAction(level, out uint act)) return act;

//            if(level >= Actions.UmbralSoul.Level)
//            {
//                if(JobGauge.InAstralFire)
//                {
//                    return Actions.Transpose.ActionID;

//                }
//            }
//            else
//            {
//                if (JobGauge.InAstralFire || JobGauge.InUmbralIce)
//                    return Actions.Transpose.ActionID;
//            }

//            return Actions.UmbralSoul.ActionID;

//        }
//    }
//}
