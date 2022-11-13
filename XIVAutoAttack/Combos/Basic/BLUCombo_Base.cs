using FFXIVClientStructs.FFXIV.Client.Game;
using System;
using System.Linq;
using XIVAutoAttack.Actions;
using XIVAutoAttack.Actions.BaseAction;
using XIVAutoAttack.Combos.CustomCombo;
using XIVAutoAttack.Data;

namespace XIVAutoAttack.Combos.Basic
{
    internal abstract class BLUCombo_Base<TCmd> : OtherCombo<TCmd> where TCmd : Enum
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

            internal BLUAction(ActionID actionID, bool isFriendly = false, bool shouldEndSpecial = false)
                : base(actionID, isFriendly, shouldEndSpecial)
            {
            }

            public sealed override bool ShouldUse(out IAction act, bool mustUse = false, bool emptyOrSkipCombo = false)
            {
                if (!OnSlot)
                {
                    act = null;
                    return false;
                }
                return base.ShouldUse(out act, mustUse, emptyOrSkipCombo);
            }
        }

        //水炮
        public static BLUAction WaterCannon { get; } = new(ActionID.WaterCannon);


        protected static bool AllOnSlot(params BLUAction[] actions) => actions.All(a => a.OnSlot);
    }
}
