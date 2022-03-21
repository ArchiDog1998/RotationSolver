using Dalamud.Game.ClientState.JobGauge.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XIVComboPlus.Combos.BLM
{
    internal class BLMAction : BaseAction
    {
        private static BLMGauge _gauge;

        private static BLMGauge JobGauge
        {
            get
            {
                if (_gauge == null)
                {
                    _gauge = Service.JobGauges.Get<BLMGauge>();
                }
                return _gauge;
            }
        }

        private readonly bool _isFire;

        internal override uint MPNeed
        {
            get
            {
                double multiply = 1;
                //冰状态
                if (JobGauge.InUmbralIce)
                {
                    //火魔法无魔力
                    if (_isFire)
                    {
                        multiply = 0;
                    }
                    //冰魔法三层半魔力，前两层不变。
                    else
                    {
                        if(JobGauge.UmbralIceStacks == 3)
                        {
                            multiply = 0.5;
                        }
                    }
                }
                //火状态
                else if (JobGauge.InAstralFire)
                {
                    //火魔法双倍魔力
                    if (_isFire)
                    {
                        multiply = 2;
                    }
                    //冰魔法无魔力
                    else
                    {
                        multiply = 0;
                    }
                }
                return (uint)(base.MPNeed * multiply);
            }
        }

        internal BLMAction(byte level, uint actionID, uint mpNeed, bool isFire) 
            : base(level, actionID, mpNeed, false)
        {
            this._isFire = isFire;
        }
    }
}
