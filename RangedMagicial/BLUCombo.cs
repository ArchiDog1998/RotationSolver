using FFXIVClientStructs.FFXIV.Client.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using XIVAutoAttack.Actions;
using XIVAutoAttack.Actions.BaseAction;
using XIVAutoAttack.Combos.Attributes;
using XIVAutoAttack.Combos.CustomCombo;
using static XIVAutoAttack.Combos.RangedMagicial.BLUCombo;

namespace XIVAutoAttack.Combos.RangedMagicial
{
    [ComboDevInfo(@"https://github.com/ArchiDog1998/XIVAutoAttack/blob/main/XIVAutoAttack/Combos/RangedMagicial/BLUCombo.cs")]
    internal sealed class BLUCombo : OtherCombo<CommandType>
    {
        internal enum CommandType : byte
        {
            None,
        }

        protected override SortedList<CommandType, string> CommandDescription => new SortedList<CommandType, string>()
        {
            //{CommandType.None, "" }, //写好注释啊！用来提示用户的。
        };

        internal class BLUAction : BaseAction
        {
            public unsafe bool OnSlot
            {
                get
                {
                    for (int i = 0; i < 24; i++)
                    {
                        if(ID == ActionManager.Instance()->GetActiveBlueMageActionInSlot(i)) return true;
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
        public override uint[] JobIDs => new uint[] { 36 };

        public static readonly BLUAction
            //水炮
            WaterCannon = new(11385);

        private protected override bool AttackAbility(byte abilityRemain, out IAction act)
        {
            act = null;
            return false;
        }

        private protected override bool GeneralGCD(out IAction act)
        {
            act = null;
            return false;
        }

        private static bool AllOnSlot(params BLUAction[] actions)  => actions.All(a => a.OnSlot);
    }
}
