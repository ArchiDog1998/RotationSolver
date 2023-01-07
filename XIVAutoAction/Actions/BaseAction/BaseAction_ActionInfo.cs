using Dalamud.Game.ClientState.Objects.Types;
using FFXIVClientStructs.FFXIV.Client.Game;
using System;
using System.Linq;
using AutoAction.Combos.CustomCombo;
using AutoAction.Data;
using AutoAction.Helpers;
using AutoAction.SigReplacers;
using AutoAction.Updaters;

namespace AutoAction.Actions.BaseAction
{
    internal partial class BaseAction
    {
        static readonly StatusID[] SpellBan = new StatusID[]
        {

        };

        static readonly StatusID[] WeaponskillBan = new StatusID[]
        {

        };

        static readonly StatusID[] AbilityBan = new StatusID[]
        {

        };

        static readonly uint[] BadStatus = new uint[]
        {
            579, //状态限制
            573, //没学会
            572, //一些额外条件未满足
        };

        private float Range => ActionManager.GetActionRange(ID);

        /// <summary>
        /// 如果之前是这些ID，那么就不会执行。
        /// </summary>
        internal ActionID[] OtherIDsNot { private get; set; } = null;

        /// <summary>
        /// 如果之前不是这些ID中的某个，那么就不会执行。
        /// </summary>
        internal ActionID[] OtherIDsCombo { private get; set; } = null;

        /// <summary>
        /// 使用了这个技能会得到的Buff，如果有这些Buff中的一种，那么就不会执行，这个buff是自己提供的。 
        /// </summary>
        internal StatusID[] BuffsProvide { get; set; } = null;

        /// <summary>
        /// 使用这个技能需要的前置Buff，有任何一个就好。
        /// </summary>
        internal virtual StatusID[] BuffsNeed { get; set; } = null;

        /// <summary>
        /// 如果有一些别的需要判断的，可以写在这里。True表示可以使用这个技能。
        /// </summary>
        internal Func<BattleChara, bool> ActionCheck { get; set; } = null;

        /// <summary>
        /// 如果有一些别的需要判断的，可以写在这里。True表示可以使用这个技能。
        /// </summary>
        internal Func<BattleChara, bool> ComboCheck { get; set; } = null;

        private bool WillCooldown
        {
            get
            {
                //如果首个CoolDown不是GCD技能，而且没法释放。
                if (!IsGeneralGCD && IsCoolDown)
                {
                    //会让GCD转的，充能一层的，看看来不来得及下个GCD
                    if (IsRealGCD)
                    {
                        if (!WillHaveOneChargeGCD()) return false;
                    }
                    else
                    {
                        //不是青魔，不能连续使用
                        if ((ClassJobID)Service.ClientState.LocalPlayer.ClassJob.Id != ClassJobID.BlueMage
                            && ChoiceTarget != TargetFilter.FindTargetForMoving
                            && Watcher.LastAction == (ActionID)AdjustedID) return false;

                        //冷却时间没超过一成且下一个Ability前不能转好
                        if (!WillHaveOneCharge(ActionUpdater.AbilityRemain, false)) return false;
                    }
                }

                return true;
            }
        }

        /// <summary>
        /// 判断是否需要使用这个技能
        /// </summary>
        /// <param name="act">返回的技能</param>
        /// <param name="mustUse">必须使用，不判断提供的Buff<seealso cref="BuffsProvide"/>和<seealso cref="TargetStatus">是否已提供，不判断AOE技能的敌人数量是否达标.</param>
        /// <param name="emptyOrSkipCombo">如果有层数，放完所有层数，不判断是否为Combo<seealso cref="OtherIDsCombo"/><seealso cref="OtherIDsNot"/></param>
        /// <returns>这个技能能不能用</returns>
        public unsafe virtual bool ShouldUse(out IAction act, bool mustUse = false, bool emptyOrSkipCombo = false, bool skipDisable = false)
        {
            act = this;

            //玩家都没有。。。
            if (Service.ClientState.LocalPlayer == null) return false;
            var player = Service.ClientState.LocalPlayer;

            //用户不让用！
            if (!skipDisable && !IsEnabled) return false;

            //技能状态不对，可能是没学会。
            if (BadStatus.Contains(ActionManager.Instance()->GetActionStatus(ActionType.Spell, AdjustedID)))
                return false;

            //等级不够
            if (!EnoughLevel) return false;

            //MP不够
            if (Service.ClientState.LocalPlayer.CurrentMp < MPNeed) return false;

            //有可恶的状态。
            switch (_action.GetActinoType())
            {
                case ActionCate.Spell: //魔法
                    if (!player.WillStatusEndGCD(0, 0, false, SpellBan)) return false;
                    break;
                case ActionCate.Weaponskill: //战技
                    if (!player.WillStatusEndGCD(0, 0, false, WeaponskillBan)) return false;
                    break;
                case ActionCate.Ability: //能力
                    if (!player.StatusTime(false, AbilityBan).IsLessThan(ActionUpdater.AbilityRemain)) return false;
                    break;
            }

            //没有前置Buff
            if (BuffsNeed != null)
            {
                if (!Service.ClientState.LocalPlayer.HasStatus(true, BuffsNeed)) return false;
            }

            //已有提供的Buff的任何一种
            if (BuffsProvide != null && !mustUse)
            {
                if (Service.ClientState.LocalPlayer.HasStatus(true, BuffsProvide)) return false;
            }

            //还冷却不下来呢，来不及。
            if (!WillCooldown) return false;

            if (IsGeneralGCD)
            {
                if (!emptyOrSkipCombo)
                {
                    //如果有输入上次的数据，那么上次不能是上述的ID。
                    if (OtherIDsNot != null)
                    {
                        if (OtherIDsNot.Contains(Service.Address.LastComboAction)) return false;
                    }

                    //如果有Combo，有LastAction，而且上次不是连击，那就不触发。
                    var comboActions = _action.ActionCombo?.Row != 0
                        ? new ActionID[] { (ActionID)_action.ActionCombo.Row }
                        : new ActionID[0];
                    if (OtherIDsCombo != null) comboActions = comboActions.Union(OtherIDsCombo).ToArray();

                    if (comboActions.Length > 0)
                    {
                        if (comboActions.Contains(Service.Address.LastComboAction))
                        {
                            if (Service.Address.ComboTime < ActionUpdater.WeaponRemain) return false;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }

                //如果是个法术需要咏唱，并且还在移动，也没有即刻相关的技能。
                if (CastTime > 0 && MovingUpdater.IsMoving)
                {
                    if (!player.HasStatus(true, CustomCombo<Enum>.Swiftcast.BuffsProvide))
                    {
                        return false;
                    }
                }
            }
            else
            {
                //如果是能力技能，还没填满。
                if (!emptyOrSkipCombo && RecastTimeRemain > ActionUpdater.WeaponRemain + ActionUpdater.WeaponTotal)
                    return false;
            }

            //看看有没有目标，如果没有，就说明不符合条件。
            if (!FindTarget(mustUse, out var target)) return false;

            //用于自定义的要求没达到
            if (ActionCheck != null && !ActionCheck(target)) return false;
            if (!skipDisable && ComboCheck != null && !ComboCheck(target)) return false;

            //看看这样能不能不会被清除。
            Target = target;
            return true;
        }

        //internal uint TargetId { get; private set; } = 0xE0000000;

        public unsafe bool Use()
        {
            var loc = new FFXIVClientStructs.FFXIV.Client.Graphics.Vector3() { X = _position.X, Y = _position.Y, Z = _position.Z };

            if (ShouldEndSpecial) CommandController.ResetSpecial(true);

            return _action.TargetArea ? ActionManager.Instance()->UseActionLocation(ActionType.Spell, ID, Service.ClientState.LocalPlayer.ObjectId, &loc) :
                ActionManager.Instance()->UseAction(ActionType.Spell, AdjustedID, Target.ObjectId);
        }
    }
}
