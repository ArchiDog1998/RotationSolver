using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Game.ClientState.Objects.Types;
using System;
using System.ComponentModel;
using System.Linq;
using XIVAutoAttack.Actions;
using XIVAutoAttack.Actions.BaseAction;
using XIVAutoAttack.Helpers;
using XIVAutoAttack.SigReplacers;
using XIVAutoAttack.Updaters;
using static XIVAutoAttack.SigReplacers.Watcher;

namespace XIVAutoAttack.Combos.CustomCombo
{
    public abstract partial class CustomCombo<TCmd> where TCmd : Enum
    {
        protected static PlayerCharacter Player => Service.ClientState.LocalPlayer;
        protected static BattleChara Target => Service.TargetManager.Target is BattleChara b ? b : Player;

        protected static bool IsMoving => MovingUpdater.IsMoving;
        protected static bool HaveHostileInRange => TargetUpdater.HaveHostileInRange;
        protected virtual bool CanHealAreaAbility => TargetUpdater.CanHealAreaAbility;
        protected virtual bool CanHealAreaSpell => TargetUpdater.CanHealAreaSpell;

        protected virtual bool CanHealSingleAbility => TargetUpdater.CanHealSingleAbility;
        protected virtual bool CanHealSingleSpell => TargetUpdater.CanHealSingleSpell;

        /// <summary>
        /// 是否处于爆发，自动爆发或者手动点击的爆发
        /// </summary>
        protected bool SettingBreak => CommandController.BreakorProvoke || Service.Configuration.AutoBreak;

        protected static byte Level => Player?.Level ?? 0;

        protected static bool InCombat => ActionUpdater.InCombat;

        /// <summary>
        /// 成功释放的技能，其中第0个为最近成功释放的技能，也就是上一个技能。该列表最大记录64个
        /// </summary>
        protected static ActionRec[] RecordActions => Watcher.RecordActions;

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

        protected static bool RecastAfterGCD(float recast, uint gcdCount = 0, uint abilityCount = 0, bool addWeaponRemain = true)
            => CooldownHelper.RecastAfterGCD(recast, gcdCount, abilityCount, addWeaponRemain);

        protected static bool RecastAfter(float recast, float remain, bool addWeaponRemain = true)
            => CooldownHelper.RecastAfter(recast, remain, addWeaponRemain);
    }
}
