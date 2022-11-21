using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XIVAutoAttack.Actions;
using XIVAutoAttack.Combos.Basic;
using XIVAutoAttack.Combos.Script.Actions;

namespace XIVAutoAttack.Combos.Script.Combos
{
    internal class SCHCombo_Script : SCHCombo_Base<Enum>, IScriptCombo
    {
        public override string Author => Set.GetAuthor();

        public ComboSet Set { get; set; } = new ComboSet();

        private protected override bool EmergencyGCD(out IAction act)
            => Set.EmergencyGCDSet.ShouldUse(this, 0, null, out act);

        private protected override bool GeneralGCD(out IAction act)
            => Set.GeneralGCDSet.ShouldUse(this, 0, null, out act);

        private protected override bool DefenseAreaGCD(out IAction act)
            => Set.DefenceAreaGCDSet.ShouldUse(this, 0, null, out act);

        private protected override bool DefenseSingleGCD(out IAction act)
            => Set.DefenceSingleGCDSet.ShouldUse(this, 0, null, out act);

        private protected override bool HealAreaGCD(out IAction act)
            => Set.HealAreaGCDSet.ShouldUse(this, 0, null, out act);

        private protected override bool HealSingleGCD(out IAction act)
            => Set.HealSingleGCDSet.ShouldUse(this, 0, null, out act);

        private protected override bool MoveGCD(out IAction act)
            => Set.MoveGCDSet.ShouldUse(this, 0, null, out act);

        private protected override bool EmergencyAbility(byte abilityRemain, IAction nextGCD, out IAction act)
            => Set.EmergencyAbilitySet.ShouldUse(this, abilityRemain, nextGCD, out act);

        private protected override bool GeneralAbility(byte abilityRemain, out IAction act)
            => Set.GeneralAbilitySet.ShouldUse(this, abilityRemain, null, out act);

        private protected override bool AttackAbility(byte abilityRemain, out IAction act)
            => Set.AttackAbilitySet.ShouldUse(this, abilityRemain, null, out act);

        private protected override bool DefenceAreaAbility(byte abilityRemain, out IAction act)
            => Set.DefenceAreaAbilitySet.ShouldUse(this, abilityRemain, null, out act);

        private protected override bool DefenceSingleAbility(byte abilityRemain, out IAction act)
            => Set.DefenceSingleAbilitySet.ShouldUse(this, abilityRemain, null, out act);

        private protected override bool HealAreaAbility(byte abilityRemain, out IAction act)
            => Set.HealAreaAbilitySet.ShouldUse(this, abilityRemain, null, out act);

        private protected override bool HealSingleAbility(byte abilityRemain, out IAction act)
            => Set.HealSingleAbilitySet.ShouldUse(this, abilityRemain, null, out act);

        private protected override bool MoveAbility(byte abilityRemain, out IAction act)
            => Set.MoveAbilitySet.ShouldUse(this, abilityRemain, null, out act);

    }
}
