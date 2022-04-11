using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XIVComboPlus.Combos.BRD
{
    internal class BRD_Feature : BRDCombo
    {
        public override string ComboFancyName => "诗人GCD";

        public override string Description => "替换强力射击为持续GCD循环";

        protected internal override uint[] ActionIDs => new uint[] { Actions.HeavyShoot.ActionID };

        protected override uint Invoke(uint actionID, uint lastComboActionID, float comboTime, byte level)
        {
            uint act;
            if (CanAddAbility(level, out act)) return act;

            //群体GCD
            if (Actions.Shadowbite.TryUseAction(level, out act)) return act;
            if (Actions.QuickNock.TryUseAction(level, out act)) return act;

            //直线射击
            if (Actions.StraitShoot.TryUseAction(level, out act)) return act;

            //上毒
            if (Actions.IronJaws.TryUseAction(level, out act)) return act;
            if (Actions.VenomousBite.TryUseAction(level, out act)) return act;
            if (Actions.Windbite.TryUseAction(level, out act)) return act;

            //放大招！
            if(JobGauge.SoulVoice >= 80 || BaseAction.HaveStatus(BaseAction.FindStatusSelfFromSelf(ObjectStatus.BlastArrowReady)))
            {
                if (Actions.ApexArrow.TryUseAction(level, out act, mustUse: true)) return act;
            }

            if (Actions.HeavyShoot.TryUseAction(level, out act)) return act;

            //强力射击
            return Actions.HeavyShoot.ActionID;
        }
    }
}
