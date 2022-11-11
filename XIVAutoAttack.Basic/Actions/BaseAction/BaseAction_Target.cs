using Dalamud.Game.ClientState.Objects.Types;
using Lumina.Excel.GeneratedSheets;
using System;
using System.Linq;
using System.Numerics;
using XIVAutoAttack.Combos.Healer;
using XIVAutoAttack.Data;
using XIVAutoAttack.Helpers;
using XIVAutoAttack.Updaters;

namespace XIVAutoAttack.Actions.BaseAction
{
    public partial class BaseAction 
    {
        public bool IsTargetDying
        {
            get
            {
                if (Target == null) return false;
                return Target.IsDying();
            }
        }
        public bool IsTargetBoss
        {
            get
            {
                if (Target == null) return false;
                return Target.IsBoss();
            }
        }

        public BattleChara Target { get; private set; } = Service.ClientState.LocalPlayer;
        private Vector3 _position = default;

        private Func<BattleChara[], BattleChara> _choiceTarget = null;
        public Func<BattleChara[], BattleChara> ChoiceTarget
        {
            private get
            {
                if (_choiceTarget != null) return _choiceTarget;
                return _isFriendly ? TargetFilter.DefaultChooseFriend : TargetFilter.DefaultFindHostile;
            }
            set => _choiceTarget = value;
        }

        private Func<BattleChara[], BattleChara[]> _filterForTarget = null;
        public Func<BattleChara[], BattleChara[]> FilterForTarget
        {
            private get
            {
                if (_filterForTarget != null) return _filterForTarget;
                return tars =>
                {
                    if (TargetStatus == null) return tars;

                    if (_isDot)
                    {
                        tars = TargetFilter.GetTargetCanDot(tars);
                    }

                    if (!MovingUpdater.IsMoving) return tars;

                    var ts = tars.Where(t => !t.HaveStatus(TargetStatus)).ToArray();

                    if (ts.Length == 0) return tars;
                    return ts;
                };
            }
            set => _filterForTarget = value;
        }

        /// <summary>
        /// 给敌人造成的Debuff,如果有这些Debuff，那么不会执行。
        /// </summary>
        public ushort[] TargetStatus { get; set; } = null;

        internal static bool TankDefenseSelf(BattleChara chara)
        {
            return TargetUpdater.TarOnMeTargets.Length > 0;
        }
        internal static bool TankBreakOtherCheck(BattleChara chara)
        {
            return TargetUpdater.HaveHostileInRange
                && (float)Service.ClientState.LocalPlayer.CurrentHp / Service.ClientState.LocalPlayer.MaxHp < Service.Configuration.HealthForDyingTank
                && TargetUpdater.PartyMembersAverHP > Service.Configuration.HealthForDyingTank + 0.1f;
        }

        private bool FindTarget(bool mustUse)
        {
            if(Service.Configuration.AttackSafeMode) mustUse = false;

            _position = Service.ClientState.LocalPlayer.Position;
            var player = Service.ClientState.LocalPlayer;

            float range = Range;

            //如果都没有距离，这个还需要选对象嘛？选自己啊！
            if (range == 0 && _action.EffectRange == 0)
            {
                Target = player;
                return true;
            }

            if (_action.TargetArea)
            {
                //缩地
                if (_action.EffectRange == 1 && _action.Range == 20)
                {
                    BattleChara[] availableCharas = Service.ObjectTable.Where(b => b.ObjectId != Service.ClientState.LocalPlayer.ObjectId && b is BattleChara)
                        .Select(b => (BattleChara)b).ToArray();

                    Target = TargetFilter.FindTargetForMoving(TargetFilter.GetObjectInRadius(availableCharas, 20));
                    _position = Target.Position;

                }
                else
                {
                    if (_isFriendly)
                    {
                        //如果用户不想使用自动友方地面放置功能
                        if (!Service.Configuration.UseAreaAbilityFriendly) return false;

                        //如果当前目标是Boss且有身位，放他身上。
                        if(Service.TargetManager.Target is BattleChara b && b.IsBoss() && b.HasLocationSide())
                        {
                            Target = b;
                            _position = Target.Position;
                        }
                        //计算玩家和被打的Ｔ之间的关系。
                        else
                        {
                            var attackT = TargetFilter.FindAttackedTarget(TargetFilter.GetObjectInRadius(TargetUpdater.PartyTanks,
                                range + _action.EffectRange));

                            Target = Service.ClientState.LocalPlayer;

                            if (attackT == null)
                            {
                                _position = Target.Position;
                            }
                            else
                            {
                                var disToTankRound = Vector3.Distance(Target.Position, attackT.Position) + attackT.HitboxRadius;

                                if(disToTankRound  < _action.EffectRange 
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
                        }
                    }
                    else
                    {
                        var tars = TargetFilter.GetMostObjectInRadius(TargetUpdater.HostileTargets, range, _action.EffectRange, false, mustUse, true)
                            .OrderByDescending(p => p.GetHealthRatio());
                        Target = tars.Count() > 0 ? tars.First() : Service.ClientState.LocalPlayer;
                        _position = Target.Position;
                    }
                }
                return true;
            }
            //如果能对友方和敌方都能选中
            else if (_action.CanTargetParty && _action.CanTargetHostile)
            {
                BattleChara[] availableCharas = TargetUpdater.PartyMembers.Union(TargetUpdater.HostileTargets).Where(b => b.ObjectId != Service.ClientState.LocalPlayer.ObjectId).ToArray();
                availableCharas = TargetFilter.GetObjectInRadius(availableCharas, range);
                //特殊选队友的方法。
                Target = ChoiceTarget(availableCharas);
                if (Target == null) return false;
                return true;

            }
            //首先看看是不是能对小队成员进行操作的。
            else if (_action.CanTargetParty)
            {
                //还消耗2400的蓝，那肯定是复活的。
                if (_action.PrimaryCostType == 3 && _action.PrimaryCostValue == 24)
                {
                    Target = TargetFilter.GetDeathPeople(TargetUpdater.DeathPeopleAll, TargetUpdater.DeathPeopleParty);
                    if (Target == null) return false;
                    return true;
                }

                //找到没死的队友们。
                BattleChara[] availableCharas = TargetUpdater.PartyMembers.Where(player => player.CurrentHp != 0).ToArray();

                if (!_action.CanTargetSelf)
                {
                    availableCharas = availableCharas.Where(p => p.ObjectId != Service.ClientState.LocalPlayer.ObjectId).ToArray();
                }
                if (availableCharas.Length == 0) return false;

                //判断是否是范围。
                if (_action.CastType > 1 && ID != ActionIDs.DeploymentTactics)
                {
                    //找到能覆盖最多的位置，并且选血最少的来。
                    Target = TargetFilter.GetMostObjectInRadius(availableCharas, range, _action.EffectRange, true, mustUse, true)
                        .OrderBy(p => p.GetHealthRatio()).FirstOrDefault();
                    if (Target == null) return false;
                    return true;
                }
                else
                {
                    availableCharas = TargetFilter.GetObjectInRadius(availableCharas, range);
                    //特殊选队友的方法。
                    Target = ChoiceTarget(availableCharas);
                    if (Target == null) return false;
                    return true;
                }
            }
            //再看看是否可以选中敌对的。
            else if (_action.CanTargetHostile)
            {
                //如果不用自动找目标，那就直接返回。
                if (!CommandController.AutoTarget)
                {
                    if (Service.TargetManager.Target is BattleChara b && b.CanAttack() && b.DistanceToPlayer() <= range)
                    {

                        if (_action.CastType == 1 || mustUse)
                        {
                            Target = b;
                            return true;
                        }

                        if (Service.Configuration.UseAOEWhenManual)
                        {
                            switch (_action.CastType)
                            {
                                case 2: // 圆形范围攻击。找到能覆盖最多的位置，并且选血最多的来。
                                    if(TargetFilter.GetMostObjectInRadius(FilterForTarget(TargetUpdater.HostileTargets), range, _action.EffectRange, false, mustUse, false)
                                        .Contains(b))
                                    {
                                        Target = b;
                                        return true;
                                    }
                                    break;
                                case 3: // 扇形范围攻击。找到能覆盖最多的位置，并且选最远的来。
                                    if (TargetFilter.GetMostObjectInArc(FilterForTarget(TargetUpdater.HostileTargets), _action.EffectRange, mustUse, false)
                                        .Contains(b))
                                    {
                                        Target = b;
                                        return true;
                                    }
                                    break;
                                case 4: //直线范围攻击。找到能覆盖最多的位置，并且选最远的来。
                                    if(TargetFilter.GetMostObjectInLine(FilterForTarget(TargetUpdater.HostileTargets), range, mustUse, false)
                                        .Contains(b))
                                    {
                                        Target = b;
                                        return true;
                                    }
                                    break;
                            }
                        }
                    }

                    Target = null;
                    return false;
                }

                //判断一下AOE攻击的时候如果有攻击目标标记目标
                if (_action.CastType > 1 && NoAOEForAttackMark)
                {
                    if (mustUse)
                    {
                        Target = TargetFilter.GetAttackMarkChara(TargetUpdater.HostileTargets);
                        if (Target == null) return false;
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }

                switch (_action.CastType)
                {
                    case 1:
                    default:
                        BattleChara[] canReachTars = FilterForTarget(TargetFilter.GetObjectInRadius(TargetUpdater.HostileTargets, range));

                        Target = ChoiceTarget(canReachTars);
                        if (Target == null) return false;
                        return true;

                    case 2: // 圆形范围攻击。找到能覆盖最多的位置，并且选血最多的来。
                        Target = ChoiceTarget(TargetFilter.GetMostObjectInRadius(FilterForTarget(TargetUpdater.HostileTargets), range, _action.EffectRange, false, mustUse, true));
                        if (Target == null) return false;
                        return true;
                    case 3: // 扇形范围攻击。找到能覆盖最多的位置，并且选最远的来。
                        Target = ChoiceTarget(TargetFilter.GetMostObjectInArc(FilterForTarget(TargetUpdater.HostileTargets), _action.EffectRange, mustUse, true));
                        if (Target == null) return false;
                        return true;
                    case 4: //直线范围攻击。找到能覆盖最多的位置，并且选最远的来。
                        Target = ChoiceTarget(TargetFilter.GetMostObjectInLine(FilterForTarget(TargetUpdater.HostileTargets), range, mustUse, true));
                        if (Target == null) return false;
                        return true;
                }
            }
            //如果只能选自己，那就选自己吧。
            else if (_action.CanTargetSelf)
            {
                Target = player;

                if (_action.EffectRange > 0 && !_isFriendly)
                {
                    if(NoAOEForAttackMark)
                    {
                        if (mustUse)
                        {
                            Target = TargetFilter.GetAttackMarkChara(TargetUpdater.HostileTargets);
                            if(Target == null) return false;
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }

                    //如果不用自动找目标，那就不打AOE
                    if (!mustUse && !CommandController.AutoTarget && !Service.Configuration.UseAOEWhenManual) return false;

                    var count = TargetFilter.GetObjectInRadius(TargetUpdater.HostileTargets, _action.EffectRange).Length;
                    if (count < (mustUse ? 1 : Service.Configuration.HostileCount)) return false;
                }
                return true;
            }

            Target = Service.TargetManager.Target is BattleChara battle ? battle : Service.ClientState.LocalPlayer;
            return true;
        }
        /// <summary>
        /// 开启攻击标记且有攻击标记目标且不开AOE。
        /// </summary>
        private static bool NoAOEForAttackMark =>
            Service.Configuration.ChooseAttackMark && !Service.Configuration.AttackMarkAOE 
            && MarkingController.HaveAttackChara(TargetUpdater.HostileTargets);
    }
}
