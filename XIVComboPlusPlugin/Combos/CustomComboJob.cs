using Dalamud.Game.ClientState.JobGauge.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
