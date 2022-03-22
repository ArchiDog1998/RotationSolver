using Dalamud.Game.ClientState.JobGauge.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XIVComboPlus.Attributes;

namespace XIVComboPlus.Combos
{
    internal abstract class CustomComboJob<T>: CustomCombo where T : JobGaugeBase
    {
        private static T _gauge;

        public static T JobGauge
        {
            get
            {
                if (_gauge == null)
                {
                    _gauge = Service.JobGauges.Get<T>();
                }
                return _gauge;
            }
        }

        private ClassJob _myJob = null;
        internal sealed override uint JobID
        {
            get
            {
                if(_myJob == null)
                {
                    _myJob = ClassJob.GetJobByGauge<T>();
                }
                if(_myJob == null)
                {
                    return 0;
                }
                return _myJob.Index;
            }
        }
        internal sealed override string JobName
        {
            get
            {
                if (_myJob == null)
                {
                    _myJob = ClassJob.GetJobByGauge<T>();
                }
                if (_myJob == null)
                {
                    return "";
                }
                return _myJob.Name;
            }
        }
    }
}
