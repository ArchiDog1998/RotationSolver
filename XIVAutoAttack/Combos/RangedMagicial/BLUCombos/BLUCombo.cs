using FFXIVClientStructs.FFXIV.Client.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using XIVAutoAttack.Actions;
using XIVAutoAttack.Actions.BaseAction;
using XIVAutoAttack.Combos.CustomCombo;
using XIVAutoAttack.Data;

namespace XIVAutoAttack.Combos.RangedMagicial.BLUCombos
{
    internal abstract class BLUCombo<TCmd> : OtherCombo<TCmd> where TCmd : Enum
    {
        public sealed override ClassJobID[] JobIDs => new ClassJobID[] { ClassJobID.BlueMage };

        public class BLUAction : BaseAction
        {
            public unsafe bool OnSlot
            {
                get
                {
                    for (int i = 0; i < 24; i++)
                    {
                        if (ID == ActionManager.Instance()->GetActiveBlueMageActionInSlot(i)) return true;
                    }
                    return false;
                }
            }

            internal BLUAction(uint actionID, bool isFriendly = false, bool shouldEndSpecial = false)
                : base(actionID, isFriendly, shouldEndSpecial)
            {
            }

            public sealed override bool ShouldUse(out IAction act, bool mustUse = false, bool emptyOrSkipCombo = false, bool skipEnable = false)
            {
                if (!OnSlot)
                {
                    act = null;
                    return false;
                }
                return base.ShouldUse(out act, mustUse, emptyOrSkipCombo, skipEnable);
            }
        }

        public static readonly BLUAction
            //水炮
            WaterCannon = new(11385);


        protected static bool AllOnSlot(params BLUAction[] actions) => actions.All(a => a.OnSlot);
    }
}
