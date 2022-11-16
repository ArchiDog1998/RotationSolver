using FFXIVClientStructs.FFXIV.Client.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using XIVAutoAttack.Actions;
using XIVAutoAttack.Actions.BaseAction;
using XIVAutoAttack.Combos.Basic;
using XIVAutoAttack.Combos.CustomCombo;
using XIVAutoAttack.Configuration;
using XIVAutoAttack.Helpers;
using static XIVAutoAttack.Combos.RangedMagicial.BLUCombos.BLUCombo_Default;

namespace XIVAutoAttack.Combos.RangedMagicial.BLUCombos
{
    internal sealed class BLUCombo_Default : BLUCombo_Base<CommandType>
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

        private protected override ActionConfiguration CreateConfiguration()
        {
            return base.CreateConfiguration()
                .SetCombo("BlueId", 2, "职能", "防护", "治疗", "进攻")
                .SetCombo("AttackType", 2, "攻击方式", "魔法", "物理", "我全都要");
        }

        private protected override void UpdateInfo()
        {
            BlueId =  (BLUID)Config.GetComboByName("BlueId");
            AttackType =  (BLUAttackType)Config.GetComboByName("AttackType");
        }

        private protected override bool AttackAbility(byte abilityRemain, out IAction act)
        {
            act = null;
            return false;
        }

        private protected override bool EmergercyAbility(byte abilityRemain, IAction nextGCD, out IAction act)
        {
            if(nextGCD.IsAnySameAction(false, Selfdestruct, FinalSting))
            {
                if (Swiftcast.ShouldUse(out act)) return true;
            }
            return base.EmergercyAbility(abilityRemain, nextGCD, out act);
        }

        private protected override bool GeneralGCD(out IAction act)
        {
            if(BlueId == BLUID.DPS)
            {
                if (AllOnSlot(Whistle, MoonFlute, FinalSting) && FinalSting.ShouldUse(out _))
                {
                    if (Whistle.ShouldUse(out act)) return true;
                    if (MoonFlute.ShouldUse(out act)) return true;

                    act = FinalSting;
                    return true;
                }
            }

            act = null;
            return false;
        }
    }
}
