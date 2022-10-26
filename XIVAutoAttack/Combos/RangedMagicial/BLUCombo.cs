using FFXIVClientStructs.FFXIV.Client.Game;
using System;
using System.Linq;
using XIVAutoAttack.Actions;
using XIVAutoAttack.Actions.BaseAction;
using XIVAutoAttack.Combos.CustomCombo;

namespace XIVAutoAttack.Combos.RangedMagicial
{
    internal sealed class BLUCombo : OtherCombo
    {
        internal class BLUAction : BaseAction
        {
            public unsafe uint[] BLUActions
            {
                get
                {
                    uint[] acts = new uint[24];
                    for (int i = 0; i < 24; i++)
                    {
                        acts[i] = ActionManager.Instance()->GetActiveBlueMageActionInSlot(i);
                    }
                    return acts;
                }
            }

            public bool OnSlot => BLUActions.Contains(this.ID);

            internal BLUAction(uint actionID, bool isFriendly = false, bool shouldEndSpecial = false) 
                : base(actionID, isFriendly, shouldEndSpecial)
            {
            }

            public sealed override bool ShouldUse(out IAction act, uint lastAct = uint.MaxValue, bool mustUse = false, bool emptyOrSkipCombo = false)
            {
                if (!OnSlot)
                {
                    act = null;
                    return false;
                }
                return base.ShouldUse(out act, lastAct, mustUse, emptyOrSkipCombo);
            }
        }
        internal override uint JobID => 36;

        internal struct Actions
        {
            public static readonly BLUAction
                //水炮
                WaterCannon = new(11385);
        }

        private protected override bool AttackAbility(byte abilityRemain, out IAction act)
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
