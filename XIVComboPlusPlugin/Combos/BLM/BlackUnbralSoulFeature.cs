using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XIVComboPlus.Combos.BLM
{
    internal class BlackUnbralSoulFeature : BLMCombo
    {
        public override string ComboFancyName => "转换状态";

        public override string Description => "替换灵极魂为星灵移位置。";

        protected internal override uint[] ActionIDs => new uint[] {Actions.UmbralSoul};

        protected override uint Invoke(uint actionID, uint lastComboActionID, float comboTime, byte level)
        {

            if(level < Levels.UmbralSoul)
            {
                return Actions.Transpose;
            }
            else  if(level < Levels.Paradox)
            {
                if (JobGauge.InAstralFire)
                {
                    return Actions.Transpose;
                }
                else
                {
                    return Actions.UmbralSoul;
                }
            }
            else
            {
                if (JobGauge.InAstralFire)
                {
                    return Actions.Transpose;
                }
                else
                {
                    if(JobGauge.UmbralHearts == 3)
                    {
                        return Actions.Transpose;
                    }
                    return Actions.UmbralSoul;
                }
            }

            return actionID;
        }
    }
}
