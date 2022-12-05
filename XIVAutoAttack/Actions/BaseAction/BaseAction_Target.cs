using Dalamud.Game.ClientState.Objects.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using XIVAutoAttack.Data;
using XIVAutoAttack.Helpers;
using XIVAutoAttack.Updaters;
using static Lumina.Data.Parsing.Layer.LayerCommon;

namespace XIVAutoAttack.Actions.BaseAction
{
    internal partial class BaseAction
    {
        internal bool IsTargetDying
        {
            get
            {
                if (Target == null) return false;
                return Target.IsDying();
            }
        }

        internal bool IsTargetBoss
        {
            get
            {
                if (Target == null) return false;
                return Target.IsBoss();
            }
        }

        internal BattleChara Target { get; set; } = Service.ClientState.LocalPlayer;
        private Vector3 _position = default;

        private Func<IEnumerable<BattleChara>, bool, BattleChara> _choiceTarget = null;
        internal Func<IEnumerable<BattleChara>, bool, BattleChara> ChoiceTarget
        {
            private get
            {
                if (_choiceTarget != null) return _choiceTarget;
                return _isFriendly ? TargetFilter.DefaultChooseFriend : TargetFilter.DefaultFindHostile;
            }
            set => _choiceTarget = value;
        }

        internal Func<IEnumerable<BattleChara>, IEnumerable<BattleChara>> FilterForTarget { private get; set; } = null;

        /// <summary>
        /// 给敌人造成的Debuff,如果有这些Debuff，那么不会执行，这个status是玩家赋予的。
        /// </summary>
        internal StatusID[] TargetStatus { get; set; } = null;

        internal static bool TankDefenseSelf(BattleChara chara)
        {
            return TargetUpdater.TarOnMeTargets.Any();
        }
        internal static bool TankBreakOtherCheck(ClassJobID id, BattleChara chara)
        {
            var tankHealth = Service.Configuration.HealthForDyingTanks.TryGetValue(id, out var value) ? value : 0.15f;

            return TargetUpdater.HaveHostilesInRange
                && Service.ClientState.LocalPlayer.GetHealthRatio() < tankHealth
                && TargetUpdater.PartyMembersAverHP > tankHealth + 0.1f;
        }

        private bool FindTarget(bool mustUse)
        {
            int aoeCount = mustUse ? 1 : AOECount;

            _position = Service.ClientState.LocalPlayer.Position;
            var player = Service.ClientState.LocalPlayer;

            float range = Range;

            //如果都没有距离，这个还需要选对象嘛？选自己啊！
            if (range == 0 && _action.EffectRange == 0)
            {
                Target = player;
                return true;
            }
            else  if (_action.TargetArea)
            {
                return TargetArea(range, mustUse, aoeCount);
            }
            //如果能对友方和敌方都能选中
            else if (_action.CanTargetParty && _action.CanTargetHostile)
            {
                return TargetPartyAndHostile(range, mustUse);
            }
            //首先看看是不是能对小队成员进行操作的。
            else if (_action.CanTargetParty)
            {
                return TargetParty(range, aoeCount, mustUse);
            }
            //再看看是否可以选中敌对的。
            else if (_action.CanTargetHostile)
            {
                return TargetHostile(range, mustUse, aoeCount);
            }
            //如果只能选自己，那就选自己吧。
            else if (_action.CanTargetSelf)
            {
                return TargetSelf(player, mustUse, aoeCount);
            }
            else
            {
                Target = Service.TargetManager.Target is BattleChara battle ? battle : Service.ClientState.LocalPlayer;
                return true;
            }
        }

        #region TargetArea
        private bool TargetArea(float range, bool mustUse, int aoeCount)
        {
            //移动
            if (_action.EffectRange == 1 && range >= 15)
            {
                return TargetAreaMove(range, mustUse);
            }
            //其他友方
            else if (_isFriendly)
            {
                return TargetAreaFriend(range, mustUse);
            }
            //敌方
            else
            {
                return TargetAreaHostile(range, aoeCount);
            }
        }

        private bool TargetAreaHostile(float range, int aoeCount)
        {
            Target = TargetFilter.GetMostObjectInRadius(TargetUpdater.HostileTargets, range, _action.EffectRange, true, aoeCount)
                .OrderByDescending(p => p.GetHealthRatio()).FirstOrDefault();
            if (Target == null)
            {
                Target = Service.ClientState.LocalPlayer;
                return false;
            }
            _position = Target.Position;
            return true;
        }

        private bool TargetAreaMove(float range, bool mustUse)
        {
            var availableCharas = Service.ObjectTable.Where(b => b.ObjectId != Service.ClientState.LocalPlayer.ObjectId).OfType<BattleChara>();
            Target = TargetFilter.FindTargetForMoving(TargetFilter.GetObjectInRadius(availableCharas, range), mustUse);
            _position = Target.Position;
            return true;
        }

        private bool TargetAreaFriend(float range, bool mustUse)
        {
            //如果用户不想使用自动友方地面放置功能
            if (!Service.Configuration.UseAreaAbilityFriendly) return false;

            //如果当前目标是Boss且有身位，放他身上。
            if (Service.TargetManager.Target is BattleChara b && b.IsBoss() && b.HasLocationSide())
            {
                Target = b;
                _position = Target.Position;
                return true;
            }
            //计算玩家和被打的Ｔ之间的关系。
            else
            {
                var attackT = TargetFilter.FindAttackedTarget(TargetFilter.GetObjectInRadius(TargetUpdater.PartyTanks,
                    range + _action.EffectRange), mustUse);

                Target = Service.ClientState.LocalPlayer;

                if (attackT == null)
                {
                    _position = Target.Position;
                }
                else
                {
                    var disToTankRound = Vector3.Distance(Target.Position, attackT.Position) + attackT.HitboxRadius;

                    if (disToTankRound < _action.EffectRange
                        || disToTankRound > 2 * _action.EffectRange - Target.HitboxRadius
                        || disToTankRound > range)
                    {
                        _position = Target.Position;
                    }
                    else
                    {
                        Vector3 directionToTank = attackT.Position - Target.Position;
                        var MoveDirection = directionToTank / directionToTank.Length() * (disToTankRound - _action.EffectRange);
                        _position = Target.Position + MoveDirection;
                    }
                }
                return true;
            }
        }
        #endregion

        private bool TargetPartyAndHostile(float range, bool mustUse)
        {
            var availableCharas = TargetUpdater.PartyMembers.Union(TargetUpdater.HostileTargets).Where(b => b.ObjectId != Service.ClientState.LocalPlayer.ObjectId);
            availableCharas = TargetFilter.GetObjectInRadius(availableCharas, range);

            //特殊选队友的方法。
            Target = ChoiceTarget(availableCharas, mustUse);
            if (Target == null) return false;
            return true;
        }

        #region Target party
        private bool TargetParty(float range, int aoeCount, bool mustUse)
        {
            //还消耗2400的蓝，那肯定是复活的。
            if (_action.PrimaryCostType == 3 && _action.PrimaryCostValue == 24 || (ActionID)ID == ActionID.AngelWhisper)
            {
                return TargetDeath();
            }

            //找到没死的队友们。
            var availableCharas = TargetUpdater.PartyMembers.Where(player => player.CurrentHp != 0);

            if ((ActionID)ID == ActionID.AetherialMimicry)
            {
                availableCharas = availableCharas.Union(TargetUpdater.AllianceMembers);
            }
            if (!_action.CanTargetSelf)
            {
                availableCharas = availableCharas.Where(p => p.ObjectId != Service.ClientState.LocalPlayer.ObjectId);
            }
            if (!availableCharas.Any()) return false;

            //判断是否是范围。
            if (_action.CastType > 1 && (ActionID)ID != ActionID.DeploymentTactics)
            {
                //找到能覆盖最多的位置，并且选血最少的来。
                Target = ChoiceTarget(TargetFilter.GetMostObjectInRadius(availableCharas, range, _action.EffectRange, true, aoeCount), mustUse);
            }
            else
            {
                availableCharas = TargetFilter.GetObjectInRadius(availableCharas, range);
                //特殊选队友的方法。
                Target = ChoiceTarget(availableCharas, mustUse);
            }

            return CheckStatus(Target, mustUse);
        }

        private bool TargetDeath()
        {
            Target = TargetFilter.GetDeathPeople(TargetUpdater.DeathPeopleAll, TargetUpdater.DeathPeopleParty);
            if (Target == null) return false;
            return true;
        }
        #endregion

        #region Target Hostile
        private bool TargetHostile(float range, bool mustUse, int aoeCount)
        {
            //如果不用自动找目标，那就直接返回。
            if (!CommandController.AutoTarget)
            {
                if (Service.TargetManager.Target is BattleChara b && b.CanAttack() && b.DistanceToPlayer() <= range)
                {
                    return TargetHostileManual(b, mustUse, range, aoeCount);
                }

                Target = null;
                return false;
            }

            //判断一下AOE攻击的时候如果有攻击目标标记目标
            if (_action.CastType > 1 && (NoAOEForAttackMark || Service.Configuration.AttackSafeMode))
            {
                return false;
            }

            switch (_action.CastType)
            {
                case 1:
                default:
                    var canReachTars = TargetFilterFuncEot(TargetFilter.GetObjectInRadius(TargetUpdater.HostileTargets, range), mustUse);

                    Target = ChoiceTarget(canReachTars, mustUse);
                    if (Target == null) return false;
                    return true;
                case 10: //环形范围攻击也就这么判断吧，我烦了。
                case 2: // 圆形范围攻击。找到能覆盖最多的位置，并且选血最多的来。
                    Target = ChoiceTarget(TargetFilter.GetMostObjectInRadius(TargetFilterFuncEot(TargetUpdater.HostileTargets, mustUse), range, _action.EffectRange, true, aoeCount), mustUse);
                    if (Target == null) return false;
                    return true;
                case 3: // 扇形范围攻击。找到能覆盖最多的位置，并且选最远的来。
                    Target = ChoiceTarget(TargetFilter.GetMostObjectInArc(TargetFilterFuncEot(TargetUpdater.HostileTargets, mustUse), _action.EffectRange, true, aoeCount), mustUse);
                    if (Target == null) return false;
                    return true;
                case 4: //直线范围攻击。找到能覆盖最多的位置，并且选最远的来。
                    Target = ChoiceTarget(TargetFilter.GetMostObjectInLine(TargetFilterFuncEot(TargetUpdater.HostileTargets, mustUse), range, true, aoeCount), mustUse);
                    if (Target == null) return false;
                    return true;
            }
        }

        private bool TargetHostileManual(BattleChara b, bool mustUse, float range, int aoeCount)
        {
            if (_action.CastType == 1)
            {
                Target = b;

                //目标已有充足的Debuff
                if (!CheckStatus(b ?? Service.ClientState.LocalPlayer, mustUse)) return false;

                return true;
            }
            else if (Service.Configuration.AttackSafeMode)
            {
                return false;
            }

            if (Service.Configuration.UseAOEWhenManual || mustUse)
            {
                switch (_action.CastType)
                {
                    case 10: //环形范围攻击也就这么判断吧，我烦了。
                    case 2: // 圆形范围攻击。找到能覆盖最多的位置，并且选血最多的来。
                        if (TargetFilter.GetMostObjectInRadius(TargetFilterFuncEot(TargetUpdater.HostileTargets, mustUse), range, _action.EffectRange, false, aoeCount)
                            .Contains(b))
                        {
                            Target = b;
                            return true;
                        }
                        break;
                    case 3: // 扇形范围攻击。找到能覆盖最多的位置，并且选最远的来。
                        if (TargetFilter.GetMostObjectInArc(TargetFilterFuncEot(TargetUpdater.HostileTargets, mustUse), _action.EffectRange, false, aoeCount)
                            .Contains(b))
                        {
                            Target = b;
                            return true;
                        }
                        break;
                    case 4: //直线范围攻击。找到能覆盖最多的位置，并且选最远的来。
                        if (TargetFilter.GetMostObjectInLine(TargetFilterFuncEot(TargetUpdater.HostileTargets, mustUse), range, false, aoeCount)
                            .Contains(b))
                        {
                            Target = b;
                            return true;
                        }
                        break;
                }
            }
            return false;
        }
        #endregion

        private bool TargetSelf(BattleChara player, bool mustUse, int aoeCount)
        {
            Target = player;

            if (_action.EffectRange > 0 && !_isFriendly)
            {
                if (NoAOEForAttackMark || Service.Configuration.AttackSafeMode)
                {
                    return false;
                }

                //如果不用自动找目标，那就不打AOE
                if (!CommandController.AutoTarget)
                {
                    if (!Service.Configuration.UseAOEWhenManual && !mustUse) return false;
                }
                var count = TargetFilter.GetObjectInRadius(TargetFilterFuncEot(TargetUpdater.HostileTargets, mustUse), _action.EffectRange).Count();
                if (count < aoeCount) return false;
            }
            return true;
        }

        private IEnumerable<BattleChara> TargetFilterFuncEot(IEnumerable<BattleChara> tars, bool mustUse)
        {
            if (FilterForTarget != null) return FilterForTarget(tars);
            if (TargetStatus == null || !_isEot) return tars;

            var canDot = tars.Where(ObjectHelper.CanDot);
            var DontHave = canDot.Where(b =>  CheckStatus(b, mustUse));

            if (mustUse)
            {
                if (DontHave.Any()) return DontHave;
                if (canDot.Any()) return canDot;
                return tars;
            }
            else
            {
                return DontHave;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tar"></param>
        /// <returns>True for add Eot.</returns>
        private bool CheckStatus(BattleChara tar, bool mustUse)
        {
            if (tar == null) return false;

            if (mustUse) return true;
            if (TargetStatus == null) return true;

            return tar.WillStatusEndGCD((uint)Service.Configuration.AddDotGCDCount, 0, true, TargetStatus);
        }

        /// <summary>
        /// 开启攻击标记且有攻击标记目标且不开AOE。
        /// </summary>
        private static bool NoAOEForAttackMark =>
            Service.Configuration.ChooseAttackMark && !Service.Configuration.AttackMarkAOE
            && MarkingController.HaveAttackChara(TargetUpdater.HostileTargets);
    }
}
