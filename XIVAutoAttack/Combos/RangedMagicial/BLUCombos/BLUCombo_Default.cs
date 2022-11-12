using FFXIVClientStructs.FFXIV.Client.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using XIVAutoAttack.Actions;
using XIVAutoAttack.Actions.BaseAction;
using XIVAutoAttack.Combos.Attributes;
using XIVAutoAttack.Combos.CustomCombo;
using static XIVAutoAttack.Combos.RangedMagicial.BLUCombos.BLUCombo_Default;

namespace XIVAutoAttack.Combos.RangedMagicial.BLUCombos
{
    [ComboDevInfo(@"https://github.com/ArchiDog1998/XIVAutoAttack/blob/main/XIVAutoAttack/Combos/RangedMagicial/BLUCombo.cs")]
    internal sealed class BLUCombo_Default : BLUCombo<CommandType>
    {
        public override string Author => "秋水";

        internal enum CommandType : byte
        {
            None,
        }

        protected override SortedList<CommandType, string> CommandDescription => new SortedList<CommandType, string>()
        {
            //{CommandType.None, "" }, //写好注释啊！用来提示用户的。
        };



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

        private static bool AllOnSlot(params BLUAction[] actions) => actions.All(a => a.OnSlot);
    }
}
