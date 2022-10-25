using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Game.ClientState.Objects.Types;
using System;
using System.ComponentModel;
using System.Linq;
using XIVAutoAttack.Actions;
using XIVAutoAttack.Actions.BaseAction;
using XIVAutoAttack.Helpers;

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
        public static TimeSpan TimeSinceLastAction => Watcher.TimeSinceLastAction;

        protected static PlayerCharacter LocalPlayer => Service.ClientState.LocalPlayer;
        protected static BattleChara Target => Service.TargetManager.Target is BattleChara b ? b : LocalPlayer;

        protected static bool IsMoving => XIVAutoAttackPlugin.movingController.IsMoving;
        protected static bool HaveHostileInRange => TargetHelper.HaveHostileInRange;
        protected virtual bool CanHealAreaAbility => TargetHelper.CanHealAreaAbility;
        protected virtual bool CanHealAreaSpell => TargetHelper.CanHealAreaSpell;

        protected virtual bool CanHealSingleAbility => TargetHelper.CanHealSingleAbility;
        protected virtual bool CanHealSingleSpell => TargetHelper.CanHealSingleSpell;

        protected bool SettingBreak => IconReplacer.BreakorProvoke || Service.Configuration.AutoBreak;

        protected static byte Level => LocalPlayer?.Level ?? 0;

        protected static bool InBattle => TargetHelper.InBattle;

        [Obsolete("该方法已过时，请使用BaseAction.WillHaveOneCharge")]
        /// <summary>
        /// 距离下一个GCD转好还需要多少时间
        /// </summary>
        /// <param name="gcdCount">要隔着多少个GCD</param>
        /// <param name="abilityCount">再多少个能力技之后</param>
        /// <returns>还剩几秒</returns>
        protected static float WeaponRemain(uint gcdCount = 0, uint abilityCount = 0)
            => CooldownHelper.WeaponRemain(gcdCount, abilityCount);

        protected static float Weaponelapsed => TargetHelper.Weaponelapsed;

        protected static bool IsLastSpell(bool isAdjust, params IAction[] actions)
            => IActionHelper.IsLastSpell(isAdjust, actions);
        protected static bool IsLastSpell(params uint[] ids)
            => IActionHelper.IsLastSpell(ids);


        protected static bool IsLastAbility(bool isAdjust, params IAction[] actions)
            => IActionHelper.IsLastAbility(isAdjust, actions);

        protected static bool IsLastAbility(params uint[] ids)
            => IActionHelper.IsLastAbility(ids);


        protected static bool IsLastWeaponSkill(bool isAdjust, params IAction[] actions)
            => IActionHelper.IsLastWeaponSkill(isAdjust, actions);

        protected static bool IsLastWeaponSkill(params uint[] ids)
            => IActionHelper.IsLastWeaponSkill(ids);


        protected static bool IsLastAction(bool isAdjust, params IAction[] actions)
            => IActionHelper.IsLastAction(isAdjust, actions);

        protected static bool IsLastAction(params uint[] ids)
            => IActionHelper.IsLastAction(ids);
    }
}
