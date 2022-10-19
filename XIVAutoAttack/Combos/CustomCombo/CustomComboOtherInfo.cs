using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Game.ClientState.Objects.Types;
using System;

namespace XIVAutoAttack.Combos.CustomCombo
{
    public abstract partial class CustomCombo
    {
        public enum DescType : byte
        {
            循环说明,
            爆发技能,
            范围治疗,
            单体治疗,
            范围防御,
            单体防御,
            移动,
        }
        public static uint LastAction => Watcher.LastAction;
        public static uint LastWeaponskill => Watcher.LastWeaponskill;
        public static uint LastAbility => Watcher.LastAbility;
        public static uint LastSpell => Watcher.LastSpell;
        public static TimeSpan TimeSinceLastAction => Watcher.TimeSinceLastAction;

        protected static PlayerCharacter LocalPlayer => Service.ClientState.LocalPlayer;
        protected static BattleChara Target => Service.TargetManager.Target is BattleChara b ? b : LocalPlayer;

        protected static bool IsMoving => XIVAutoAttackPlugin.movingController.IsMoving;
        protected static bool HaveHostileInRange => TargetHelper.HaveHostileInRange;
        protected static float WeaponRemain => TargetHelper.WeaponRemain;

        protected virtual bool CanHealAreaAbility => TargetHelper.CanHealAreaAbility;
        protected virtual bool CanHealAreaSpell => TargetHelper.CanHealAreaSpell;

        protected virtual bool CanHealSingleAbility => TargetHelper.CanHealSingleAbility;
        protected virtual bool CanHealSingleSpell => TargetHelper.CanHealSingleSpell;

        protected bool SettingBreak => IconReplacer.BreakorProvoke || Service.Configuration.AutoBreak;

        protected static byte Level => LocalPlayer?.Level ?? 0;

        protected static bool InBattle => TargetHelper.InBattle;
    }
}
