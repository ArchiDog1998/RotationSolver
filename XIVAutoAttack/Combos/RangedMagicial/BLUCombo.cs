using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XIVAutoAttack.Combos.RangedMagicial
{
    internal class BLUCombo : OtherCombo
    {
        internal class BLUAction : BaseAction
        {
            internal BLUAction(uint actionID, bool isFriendly = false, bool shouldEndSpecial = false) 
                : base(actionID, isFriendly, shouldEndSpecial)
            {
            }

            public sealed override bool ShouldUseAction(out IAction act, uint lastAct = uint.MaxValue, bool mustUse = false, bool emptyOrSkipCombo = false)
            {
                if (!TargetHelper.BLUActions.Contains(this.ID))
                {
                    act = null;
                    return false;
                }
                return base.ShouldUseAction(out act, lastAct, mustUse, emptyOrSkipCombo);
            }
        }
        internal override uint JobID => 36;

        internal struct Actions
        {
            public static readonly BLUAction
                //水炮
                WaterCannon = new(11385);
        }

        private protected override bool ForAttachAbility(byte abilityRemain, out IAction act)
        {
            act = null;
            return false;
        }

        private protected override bool GeneralGCD(uint lastComboActionID, out IAction act)
        {
            act = null;
            return false;
        }
    }
}
