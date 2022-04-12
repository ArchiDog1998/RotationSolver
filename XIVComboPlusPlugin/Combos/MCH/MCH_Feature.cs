using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XIVComboPlus.Combos.MCH
{
    internal class MCH_Feature : MCHCombo
    {
        public override string ComboFancyName => "机工GCD";

        public override string Description => "替换分裂弹为持续GCD循环";

        protected internal override uint[] ActionIDs => new uint[] {Actions.SplitShot.ActionID};

        protected override uint Invoke(uint actionID, uint lastComboActionID, float comboTime, byte level)
        {
            uint act;

            // 四个牛逼的技能。
            if (Actions.Bioblaster.TryUseAction(level, out _))
            {
                if (CanAddAbility(level, out act)) return act;
                return Actions.Bioblaster.ActionID;
            }

            if (Actions.Drill.TryUseAction(level, out _, mustUse: true))
            {
                if (Actions.Reassemble.TryUseAction(level, out act, mustUse: true)) return act;
                if (CanAddAbility(level, out act)) return act;
                return Actions.Drill.ActionID;
            }

            if(level >= Actions.AirAnchor.Level)
            {
                if (Actions.AirAnchor.TryUseAction(level, out _, mustUse: true))
                {
                    if (Actions.Reassemble.TryUseAction(level, out act, mustUse: true)) return act;
                    if (CanAddAbility(level, out act)) return act;
                    return Actions.AirAnchor.ActionID;
                }
            }
            else
            {
                if (CanAddAbility(level, out act)) return act;
                if (Actions.HotShow.TryUseAction(level, out act)) return act;
            }
            if (Actions.ChainSaw.TryUseAction(level, out _, mustUse:true))
            {
                if (Actions.Reassemble.TryUseAction(level, out act, mustUse: true)) return act;
                if (CanAddAbility(level, out act)) return act;
                return Actions.ChainSaw.ActionID;
            }

            if(CanAddAbility(level, out act)) return act;

            //群体常规GCD
            if (JobGauge.IsOverheated && Actions.AutoCrossbow.TryUseAction(level, out act)) return act;
            if (Actions.SpreadShot.TryUseAction(level, out act)) return act;

            //单体常规GCD
            if (JobGauge.IsOverheated && Actions.HeatBlast.TryUseAction(level, out act)) return act;
            if (Actions.CleanShot.TryUseAction(level, out act, lastComboActionID)) return act;
            if (Actions.SlugShot.TryUseAction(level, out act, lastComboActionID)) return act;
            if (Actions.SplitShot.TryUseAction(level, out act, lastComboActionID)) return act;

            return 0;
        }
    }
}
