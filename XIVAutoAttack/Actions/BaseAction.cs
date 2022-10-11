using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Game.ClientState.Statuses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Action = Lumina.Excel.GeneratedSheets.Action;
using FFXIVClientStructs.FFXIV.Client.Game;
using System.Numerics;
using System.Runtime.InteropServices;
using Dalamud.Utility.Signatures;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;
using FFXIVClientStructs.FFXIV.Client.UI.Shell;
using XIVAutoAttack.Combos.Healer;
using XIVAutoAttack.Combos;
using XIVAutoAttack.Combos.CustomCombo;

namespace XIVAutoAttack.Actions
{
    public class BaseAction : IAction
    {
        internal const byte GCDCooldownGroup = 58;

        private bool _isFriendly;
        private bool _shouldEndSpecial;
        internal Action Action { get; }
        internal byte Level => Action.ClassJobLevel;
        public uint ID => Action.RowId;
        internal bool IsGeneralGCD { get; }
        internal bool IsRealGCD { get; }

        internal EnemyLocation EnermyLocation { get; set; } = EnemyLocation.None;
        internal virtual uint MPNeed { get; }

        #region CoolDown
        /// <summary>
        /// 复唱时间
        /// </summary>
        internal unsafe float RecastTime => ActionManager.Instance()->GetRecastTime(ActionType.Spell, ID);
        /// <summary>
        /// 咏唱时间
        /// </summary>
        internal virtual int Cast100 => Action.Cast100ms - (StatusHelper.HaveStatusSelfFromSelf(ObjectStatus.LightSpeed, ObjectStatus.Requiescat) ? 25 : 0);
        internal float RecastTimeRemain => RecastTime - RecastTimeElapsed;
        internal unsafe float RecastTimeElapsed => ActionManager.Instance()->GetRecastTimeElapsed(ActionType.Spell, ID);
        internal unsafe ushort MaxCharges => Math.Max(ActionManager.GetMaxCharges(ID, Service.ClientState.LocalPlayer.Level), (ushort)1);
        internal unsafe bool IsCoolDown => ActionManager.Instance()->IsRecastTimerActive(ActionType.Spell, ID);
        #endregion
        internal BattleChara Target { get; set; } = Service.ClientState.LocalPlayer;
        private Vector3 _position = default;
        /// <summary>
        /// 如果之前是这些ID，那么就不会执行。
        /// </summary>
        internal uint[] OtherIDsNot { private get; set; } = null;

        /// <summary>
        /// 如果之前不是这些ID中的某个，那么就不会执行。
        /// </summary>
        internal uint[] OtherIDsCombo { private get; set; } = null;
        /// <summary>
        /// 给敌人造成的Debuff,如果有这些Debuff，那么不会执行。
        /// </summary>
        internal ushort[] TargetStatus { get; set; } = null;
        /// <summary>
        /// 使用了这个技能会得到的Buff，如果有这些Buff中的一种，那么就不会执行。 
        /// </summary>
        internal ushort[] BuffsProvide { get; set; } = null;

        /// <summary>
        /// 使用这个技能需要的前置Buff，有任何一个就好。
        /// </summary>
        internal virtual ushort[] BuffsNeed { get; set; } = null;
        internal System.Action AfterUse { get; set; } = null;

        /// <summary>
        /// 如果有一些别的需要判断的，可以写在这里。True表示可以使用这个技能。
        /// </summary>
        internal Func<BattleChara, bool> OtherCheck { get; set; } = null;

        internal Func<BattleChara[], BattleChara> ChoiceTarget { private get; set; }

        private Func<BattleChara[], BattleChara> ChoiceTargetPrivate
        {
            get
            {
                if (ChoiceTarget != null) return ChoiceTarget;
                return this._isFriendly ? TargetFilter.DefaultChooseFriend : TargetFilter.DefaultFindHostile;
            }
        }

        internal Func<BattleChara[], BattleChara[]> FilterForTarget { get; set; } = availableCharas => availableCharas;

        internal BaseAction(uint actionID, bool isFriendly = false, bool shouldEndSpecial = false)
        {
            Action = Service.DataManager.GetExcelSheet<Action>().GetRow(actionID);
            _shouldEndSpecial = shouldEndSpecial;
            _isFriendly = isFriendly;
            IsGeneralGCD = Action.CooldownGroup == GCDCooldownGroup;
            IsRealGCD = IsGeneralGCD || Action.AdditionalCooldownGroup == GCDCooldownGroup;

            if (Action.PrimaryCostType == 3 || Action.PrimaryCostType == 4)
            {
                MPNeed = Action.PrimaryCostValue * 100u;
            }
            else if (Action.SecondaryCostType == 3 || Action.SecondaryCostType == 4)
            {
                MPNeed = Action.SecondaryCostValue * 100u;
            }
            else
            {
                MPNeed = 0;
            }
        }


        internal static bool TankBreakOtherCheck(BattleChara chara)
        {
            return (float)Service.ClientState.LocalPlayer.CurrentHp / Service.ClientState.LocalPlayer.MaxHp < Service.Configuration.HealthForDyingTank
                && TargetHelper.PartyMembersAverHP > Service.Configuration.HealthForDyingTank + 0.1f;
        }

        private bool FindTarget(bool mustUse)
        {
            _position = Service.ClientState.LocalPlayer.Position;
            float range = GetRange(Action);

            //如果都没有距离，这个还需要选对象嘛？选自己啊！
            if (range == 0 && Action.EffectRange == 0)
            {
                Target = Service.ClientState.LocalPlayer;
                return true;
            }

            if (Action.TargetArea)
            {
                //缩地
                if (Action.EffectRange == 1 && Action.Range == 20)
                {
                    BattleChara[] availableCharas = Service.ObjectTable.Where(b => b.ObjectId != Service.ClientState.LocalPlayer.ObjectId && b is BattleChara)
                        .Select(b => (BattleChara)b).ToArray();

                    Target = TargetFilter.FindMoveTarget(TargetFilter.GetObjectInRadius(availableCharas, 20));
                }
                else
                {
                    var tars = TargetFilter.GetMostObjectInRadius(_isFriendly ? TargetHelper.PartyMembers : TargetHelper.HostileTargets, range, Action.EffectRange, _isFriendly, mustUse)
                        .OrderByDescending(p => (float)p.CurrentHp / p.MaxHp);
                    Target = tars.Count() > 0 ? tars.First() : Service.ClientState.LocalPlayer;
                }
                _position = Target.Position;
                return true;
            }
            //如果能对友方和敌方都能选中
            else if (Action.CanTargetParty && Action.CanTargetHostile)
            {
                BattleChara[] availableCharas = TargetHelper.PartyMembers.Union(TargetHelper.HostileTargets).Where(b => b.ObjectId != Service.ClientState.LocalPlayer.ObjectId).ToArray();
                availableCharas = TargetFilter.GetObjectInRadius(availableCharas, range);
                //特殊选队友的方法。
                var tar = ChoiceTargetPrivate(availableCharas);
                if (tar == null) return false;
                Target = tar;
                return true;

            }
            //首先看看是不是能对小队成员进行操作的。
            else if (Action.CanTargetParty)
            {
                //还消耗2400的蓝，那肯定是复活的。
                if (Action.PrimaryCostType == 3 && Action.PrimaryCostValue == 24)
                {
                    var tar = TargetFilter.GetDeathPeople(TargetHelper.DeathPeopleAll, TargetHelper.DeathPeopleParty);
                    if (tar == null) return false;
                    Target = tar;
                    return true;
                }

                //找到没死的队友们。
                BattleChara[] availableCharas = TargetHelper.PartyMembers.Where(player => player.CurrentHp != 0).ToArray();

                if (!Action.CanTargetSelf)
                {
                    availableCharas = availableCharas.Where(p => p.ObjectId != Service.ClientState.LocalPlayer.ObjectId).ToArray();
                }
                if (availableCharas.Length == 0) return false;

                //判断是否是范围。
                if (Action.CastType > 1 && ID != SCHCombo.Actions.DeploymentTactics.ID)
                {
                    //找到能覆盖最多的位置，并且选血最少的来。
                    var tar = TargetFilter.GetMostObjectInRadius(availableCharas, range, Action.EffectRange, true, mustUse).OrderBy(p => (float)p.CurrentHp / p.MaxHp).First();
                    if (tar == null) return false;
                    Target = tar;
                    return true;
                }
                else
                {
                    availableCharas = TargetFilter.GetObjectInRadius(availableCharas, range);
                    //特殊选队友的方法。
                    var tar = ChoiceTargetPrivate(availableCharas);
                    if (tar == null) return false;
                    Target = tar;
                    return true;
                }
            }
            //再看看是否可以选中敌对的。
            else if (Action.CanTargetHostile)
            {
                //如果不用自动找目标，那就直接返回。
                if (!IconReplacer.AutoTarget)
                {
                    if (Service.TargetManager.Target is BattleChara b && TargetHelper.CanAttack(b) && TargetFilter.DistanceToPlayer(b) <= range
                        && (Action.CastType == 1 || mustUse))
                    {
                        Target = b;
                        return true;
                    }
                    else
                    {
                        Target = null;
                        return false;
                    }
                }
                switch (Action.CastType)
                {
                    case 1:
                    default:
                        BattleChara[] canReachTars = FilterForTarget(TargetFilter.GetObjectInRadius(TargetHelper.HostileTargets, GetRange(Action)));
                        var tar = ChoiceTargetPrivate(canReachTars);
                        if (tar == null) return false;
                        Target = tar;
                        return true;

                    case 2: // 圆形范围攻击。找到能覆盖最多的位置，并且选血最多的来。
                        tar = ChoiceTargetPrivate(TargetFilter.GetMostObjectInRadius(FilterForTarget(TargetHelper.HostileTargets), range, Action.EffectRange, false, mustUse));
                        if (tar == null) return false;
                        Target = tar;
                        return true;
                    case 3: // 扇形范围攻击。找到能覆盖最多的位置，并且选最远的来。
                        tar = ChoiceTargetPrivate(TargetFilter.GetMostObjectInArc(FilterForTarget(TargetHelper.HostileTargets), Action.EffectRange, mustUse));
                        if (tar == null) return false;
                        Target = tar;
                        return true;
                    case 4: //直线范围攻击。找到能覆盖最多的位置，并且选最远的来。
                        tar = ChoiceTargetPrivate(TargetFilter.GetMostObjectInLine(FilterForTarget(TargetHelper.HostileTargets), range, mustUse));
                        if (tar == null) return false;
                        Target = tar;
                        return true;
                }
            }
            //如果只能选自己，那就选自己吧。
            else if (Action.CanTargetSelf)
            {
                Target = Service.ClientState.LocalPlayer;
                if (Action.EffectRange > 0 && !_isFriendly)
                {
                    var count = TargetFilter.GetObjectInRadius(TargetHelper.HostileTargets, Action.EffectRange).Length;
                    if (count < (mustUse ? 1 : Service.Configuration.HostileCount)) return false;
                }
                return true;
            }

            Target = Service.TargetManager.Target is BattleChara battle ? battle : Service.ClientState.LocalPlayer;
            return true;
        }

        private static float GetRange(Action act)
        {
            return ActionManager.GetActionRange(act.RowId);
        }

        public virtual bool ShouldUseAction(out IAction act, uint lastAct = uint.MaxValue, bool mustUse = false, bool emptyOrSkipCombo = false)
        {
            act = this;
            byte level = Service.ClientState.LocalPlayer.Level;

            //等级不够。
            if (level < Level) return false;

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
                if (!StatusHelper.HaveStatusSelfFromSelf(BuffsNeed)) return false;
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
                    if (RecastTimeElapsed + TargetHelper.WeaponRemain < RecastTime / MaxCharges) return false;
                }
                else
                {
                    //冷却时间没超过一成
                    if (RecastTimeElapsed < RecastTime / MaxCharges) return false;

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

                //如果是个法术需要咏唱，并且还在移动，也没有即刻相关的技能。
                if (Cast100 > 0 && TargetHelper.IsMoving)
                {
                    if (!StatusHelper.HaveStatusSelfFromSelf(CustomCombo.GeneralActions.Swiftcast.BuffsProvide)) return false;
                }

                //目标已有充足的Debuff
                if (!mustUse && TargetStatus != null)
                {
                    var tar = Target == Service.ClientState.LocalPlayer ? TargetHelper.HostileTargets.OrderBy(p => TargetFilter.DistanceToPlayer(p)).First() : Target;
                    var times = StatusHelper.FindStatusFromSelf(tar, TargetStatus);
                    if (times.Length > 0 && times.Max() > 6) return false;
                }
            }
            else
            {
                //如果是能力技能，还没填满。
                if (!(mustUse || emptyOrSkipCombo) && RecastTimeRemain > 4) return false;
            }

            //用于自定义的要求没达到
            if (OtherCheck != null && !OtherCheck(Target)) return false;

            return true;
        }

        public virtual unsafe bool Use()
        {
            var loc = new FFXIVClientStructs.FFXIV.Client.Graphics.Vector3() { X = _position.X, Y = _position.Y, Z = _position.Z };

            bool result = Action.TargetArea ? ActionManager.Instance()->UseActionLocation(ActionType.Spell, ID, Service.ClientState.LocalPlayer.ObjectId, &loc) :
             ActionManager.Instance()->UseAction(ActionType.Spell, Service.IconReplacer.OriginalHook(ID), Target.ObjectId);

            if (_shouldEndSpecial) IconReplacer.ResetSpecial(false);
            if (result && AfterUse != null) AfterUse.Invoke();

            if(EnermyLocation != EnemyLocation.None && EnermyLocation != CustomCombo.FindEnemyLocation(Target) 
                && !StatusHelper.HaveStatusSelfFromSelf(ObjectStatus.TrueNorth))
            {
                CustomCombo.Speak("渣男！这都打错？");
            }

            return result;
        }
    }
}
