using Dalamud.Game.ClientState.JobGauge.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XIVAutoAttack.Configuration;

namespace XIVAutoAttack.Combos
{
    internal abstract class CustomComboJob<T> : CustomCombo where T : JobGaugeBase
    {
        private static T _gauge;

        protected BaseItem BreakItem => new BaseItem(config.GetTextByName("BreakingItem"), 2)
        {
            OtherCheck = () => Service.ClientState.LocalPlayer.Level == 90,
        };
        private protected override ActionConfiguration CreateConfiguration()
        {
            return base.CreateConfiguration().SetText("BreakingItem", "", JobName + "爆发药名称");
        }
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
