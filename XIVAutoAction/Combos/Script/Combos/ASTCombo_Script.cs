using System;
using AutoAction.Actions;
using AutoAction.Combos.Basic;
using AutoAction.Combos.Script.Actions;

namespace AutoAction.Combos.Script.Combos
{
    internal class ASTCombo_Script : ASTCombo_Base<Enum>, IScriptCombo
    {
        public override string Author => Set.GetAuthor();
        public override string GameVersion => Set.GameVersion;

        public ComboSet Set { get; set; } = new ComboSet();

        private protected override IAction CountDownAction(float remainTime)
            => Set.CountDown.ShouldUse(this, remainTime);

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
