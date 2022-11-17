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
using XIVAutoAttack.Updaters;
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

        protected override bool CanHealAreaSpell => base.CanHealAreaSpell && BlueId == BLUID.Healer;
        protected override bool CanHealSingleSpell => base.CanHealSingleSpell && BlueId == BLUID.Healer;

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

        private protected override bool EmergencyAbility(byte abilityRemain, IAction nextGCD, out IAction act)
        {
            if(nextGCD.IsAnySameAction(false, Selfdestruct, FinalSting))
            {
                if (Swiftcast.ShouldUse(out act)) return true;
            }
            return base.EmergencyAbility(abilityRemain, nextGCD, out act);
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

        private protected override bool HealAreaGCD(out IAction act)
        {
            if (BlueId == BLUID.Healer)
            {
                //有某些非常危险的状态。
                if (CommandController.EsunaOrShield && TargetUpdater.WeakenPeople.Length > 0 || TargetUpdater.DyingPeople.Length > 0)
                {
                    if (Exuviation.ShouldUse(out act, mustUse: true)) return true;
                }
                if (AngelsSnack.ShouldUse(out act)) return true;
                if (Stotram.ShouldUse(out act)) return true;
                if (PomCure.ShouldUse(out act)) return true;
            }
            else
            {
                if (WhiteWind.ShouldUse(out act)) return true;
            }

            return base.HealAreaGCD(out act);
        }
    }
}
