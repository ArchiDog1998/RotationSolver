using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XIVComboPlus.Combos.RDM
{
    internal class RDM_Feature : RDMCombo
    {
        public override string ComboFancyName => "赤魔GCD";

        public override string Description => "替换摇荡相接为自动GCD";

        protected internal override uint[] ActionIDs => new uint[] {Actions.Jolt.ActionID};

        protected override uint Invoke(uint actionID, uint lastComboActionID, float comboTime, byte level)
        {
            uint act;

            //如果还在移动，并且有目标的角度。
            if(IsMoving && HaveTargetAngle)
            {
                if (Actions.Acceleration.TryUseAction(level, out act, mustUse: true)) return act;
                if (GeneralActions.Swiftcast.TryUseAction(level, out act, mustUse: true)) return act;
            }

            //如果没人，就赶紧补一个即刻。
            if (!HaveTargetAngle)
            {
                if (Actions.Vercure.TryUseAction(level, out act, mustUse: true)) return act;
            }

            //如果现在可以增加能力技
            if (CanAddAbility(level, lastComboActionID, out act)) return act;

            //死人了，赶紧救！
            if (TargetHelper.DeathPeopleParty.Length != 0)
            {
                if (Actions.Verraise.TryUseAction(level, out act, mustUse: true)) return act;
            }

            //如果已经在爆发了，那继续！
            if (CanBreak(lastComboActionID, level, out act, false)) return act;


            #region 常规输出
            if (Actions.Verfire.TryUseAction(level, out act)) return act;
            if (Actions.Verstone.TryUseAction(level, out act)) return act;

            //试试看散碎
            if (Actions.Scatter.TryUseAction(level, out act)) return act;
            //平衡魔元
            if (JobGauge.WhiteMana < JobGauge.BlackMana)
            {
                if (Actions.Veraero2.TryUseAction(level, out act)) return act;
                if (Actions.Veraero.TryUseAction(level, out act)) return act;
            }
            else
            {
                if (Actions.Verthunder2.TryUseAction(level, out act)) return act;
                if (Actions.Verthunder.TryUseAction(level, out act)) return act;
            }
            if (Actions.Jolt.TryUseAction(level, out act)) return act;

            #endregion

            //if (Actions.Vercure.TryUseAction(level, out act, mustUse: true)) return act;

            return 0;
        }
    }
}
