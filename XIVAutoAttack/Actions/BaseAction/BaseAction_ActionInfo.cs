using Dalamud.Game.ClientState.Objects.Types;
using FFXIVClientStructs.FFXIV.Client.Game;
using System;
using System.Linq;
using XIVAutoAttack.Combos.CustomCombo;
using XIVAutoAttack.Helpers;
using XIVAutoAttack.Helpers.TargetHelper;
using Action = Lumina.Excel.GeneratedSheets.Action;

namespace XIVAutoAttack.Actions.BaseAction
{
    internal partial class BaseAction 
    {
        internal const byte GCDCooldownGroup = 58;

        internal float Range => ActionManager.GetActionRange(ID);


        internal virtual EnemyLocation EnermyLocation
        {
            get
            {
                if (StatusHelper.ActionLocations.TryGetValue(ID, out var location))
                {
                    return location;
                }
                return EnemyLocation.None;
            }
        }
        internal virtual uint MPNeed { get; }


        /// <summary>
        /// 如果之前是这些ID，那么就不会执行。
        /// </summary>
        internal uint[] OtherIDsNot { private get; set; } = null;

        /// <summary>
        /// 如果之前不是这些ID中的某个，那么就不会执行。
        /// </summary>
        internal uint[] OtherIDsCombo { private get; set; } = null;

        /// <summary>
        /// 使用了这个技能会得到的Buff，如果有这些Buff中的一种，那么就不会执行。 
        /// </summary>
        internal ushort[] BuffsProvide { get; set; } = null;

        /// <summary>
        /// 使用这个技能需要的前置Buff，有任何一个就好。
        /// </summary>
        internal virtual ushort[] BuffsNeed { get; set; } = null;

        [Obsolete("尽量别用！慢慢抛弃它！")]
        /// <summary>
        /// 技能使用好后，做点啥，能不用尽量不要用！手动按无效。
        /// </summary>
        internal System.Action AfterUse { get; set; } = null;

        /// <summary>
        /// 如果有一些别的需要判断的，可以写在这里。True表示可以使用这个技能。
        /// </summary>
        internal Func<BattleChara, bool> OtherCheck { get; set; } = null;

        /// <summary>
        /// 判断是否需要使用这个技能
        /// </summary>
        /// <param name="act">返回的技能</param>
        /// <param name="lastAct">上一个Combo技能的值，如果需要算Combo，请输入他！<seealso cref="OtherIDsCombo"/><seealso cref="OtherIDsNot"/></param>
        /// <param name="mustUse">必须使用，不判断提供的Buff<seealso cref="BuffsProvide"/>是否已提供，不判断AOE技能的敌人数量是否达标，并且如果有层数，放完所有层数。</param>
        /// <param name="emptyOrSkipCombo">如果有层数，放完所有层数，不判断是否为Combo<seealso cref="OtherIDsCombo"/><seealso cref="OtherIDsNot"/></param>
        /// <returns>这个技能能不能用</returns>
        public virtual bool ShouldUse(out IAction act, uint lastAct = uint.MaxValue, bool mustUse = false, bool emptyOrSkipCombo = false)
        {
            act = this;
            //byte level = Service.ClientState.LocalPlayer.Level;

            //等级不够。
            if (!EnoughLevel) return false;

            //MP不够
            if (Service.ClientState.LocalPlayer.CurrentMp < MPNeed) return false;

            //GP不够
            if (Action.PrimaryCostType == 7)
            {
                if (Service.ClientState.LocalPlayer.CurrentGp < Action.PrimaryCostValue) return false;
            }

            //没有前置Buff
            if (BuffsNeed != null)
            {
                if (!Service.ClientState.LocalPlayer.HaveStatus(BuffsNeed)) return false;
            }

            if (!mustUse)
            {
                //已有提供的Buff的任何一种
                if (BuffsProvide != null)
                {
                    if (Service.ClientState.LocalPlayer.StatusList.Select(s => (ushort)s.StatusId).Intersect(BuffsProvide).Count() > 0) return false;
                }
            }


            //如果首个CoolDown不是GCD能，而且没法释放。
            if (!IsGeneralGCD && IsCoolDown)
            {
                //会让GCD转的，充能一层的，看看来不来得及下个GCD
                if (IsRealGCD)
                {
                    if (RecastTimeElapsed + TargetUpdater.WeaponRemain < RecastTimeOneCharge) return false;
                }
                else
                {
                    //冷却时间没超过一成
                    if (!HaveOneCharge) return false;

                    ////不能连续两个相同的能力技
                    //if (ID == Watcher.LastAbility) return false;
                }
            }

            //如果有输入上次的数据，那么上次不能是上述的ID。
            if (OtherIDsNot != null)
            {
                foreach (var id in OtherIDsNot)
                {
                    if (lastAct == id) return false;
                }
            }

            //看看有没有目标，如果没有，就说明不符合条件。
            if (!FindTarget(mustUse)) return false;


            //用于自定义的要求没达到
            if (OtherCheck != null && !OtherCheck(Target)) return false;

            if (IsGeneralGCD)
            {
                //如果有Combo，有LastAction，而且上次不是连击，那就不触发。
                uint[] comboActions = Action.ActionCombo.Row == 0 ? new uint[0] : new uint[] { Action.ActionCombo.Row };
                if (OtherIDsCombo != null) comboActions = comboActions.Union(OtherIDsCombo).ToArray();
                bool findCombo = lastAct == uint.MaxValue;
                if (!findCombo) foreach (var comboAction in comboActions)
                    {
                        if (comboAction == lastAct)
                        {
                            findCombo = true;
                            break;
                        }
                    }
                if (!emptyOrSkipCombo && !findCombo && comboActions.Length > 0) return false;

                //目标已有充足的Debuff
                if (!mustUse && TargetStatus != null)
                {
                    var tar = Target == Service.ClientState.LocalPlayer ? TargetUpdater.HostileTargets.OrderBy(p => p.DistanceToPlayer()).First() : Target;
                    var times = tar.FindStatusTimes(TargetStatus);
                    if (times.Length > 0 && times.Max() > 4 + TargetUpdater.WeaponRemain) return false;
                }

                //如果是个法术需要咏唱，并且还在移动，也没有即刻相关的技能。
                if (Cast100 > 0 && XIVAutoAttackPlugin.movingController.IsMoving)
                {
                    if (!Service.ClientState.LocalPlayer.HaveStatus(CustomCombo.GeneralActions.Swiftcast.BuffsProvide))
                        if (Service.Configuration.PoslockCasting) XIVAutoAttackPlugin.movingController.IsMoving = false;
                        else return false;
                }
            }
            else
            {
                //如果是能力技能，还没填满。
                if (!(mustUse || emptyOrSkipCombo) && RecastTimeRemain > 4) return false;
            }

            return true;
        }

        public virtual unsafe bool Use()
        {
            var loc = new FFXIVClientStructs.FFXIV.Client.Graphics.Vector3() { X = _position.X, Y = _position.Y, Z = _position.Z };

            bool result = Action.TargetArea ? ActionManager.Instance()->UseActionLocation(ActionType.Spell, ID, Service.ClientState.LocalPlayer.ObjectId, &loc) :
             ActionManager.Instance()->UseAction(ActionType.Spell, AdjustedID, Target.ObjectId);

            if (_shouldEndSpecial) IconReplacer.ResetSpecial(false);
            if (result && AfterUse != null) AfterUse.Invoke();

            return result;
        }
    }
}
