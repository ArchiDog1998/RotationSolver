using Dalamud.Game.ClientState.Objects.Types;
using System;
using System.Linq;
using System.Numerics;
using XIVAutoAttack.Combos.Healer;

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

        private Func<BattleChara[], BattleChara> _choiceTarget = null;
        internal Func<BattleChara[], BattleChara> ChoiceTarget
        {
            private get
            {
                if (_choiceTarget != null) return _choiceTarget;
                return _isFriendly ? TargetFilter.DefaultChooseFriend : TargetFilter.DefaultFindHostile;
            }
            set => _choiceTarget = value;
        }

        private Func<BattleChara[], BattleChara[]> _filterForTarget = null;
        internal Func<BattleChara[], BattleChara[]> FilterForTarget
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

                    if (!XIVAutoAttackPlugin.movingController.IsMoving) return tars;

                    var ts = tars.Where(t => t.FindStatusTime(TargetStatus) == 0).ToArray();

                    if (ts.Length == 0) return tars;
                    return ts;
                };
            }
            set => _filterForTarget = value;
        }

        /// <summary>
        /// 给敌人造成的Debuff,如果有这些Debuff，那么不会执行。
        /// </summary>
        internal ushort[] TargetStatus { get; set; } = null;

        internal static bool TankDefenseSelf(BattleChara chara)
        {
            return TargetHelper.TarOnMeTargets.Length > 0;
        }
        internal static bool TankBreakOtherCheck(BattleChara chara)
        {
            return (float)Service.ClientState.LocalPlayer.CurrentHp / Service.ClientState.LocalPlayer.MaxHp < Service.Configuration.HealthForDyingTank
                && TargetHelper.PartyMembersAverHP > Service.Configuration.HealthForDyingTank + 0.1f;
        }

        private bool FindTarget(bool mustUse)
        {
            _position = Service.ClientState.LocalPlayer.Position;
            float range = Range;

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
                var tar = ChoiceTarget(availableCharas);
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
                    var tar = ChoiceTarget(availableCharas);
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
                    if (Service.TargetManager.Target is BattleChara b && TargetHelper.CanAttack(b) && b.DistanceToPlayer() <= range
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
                        BattleChara[] canReachTars = FilterForTarget(TargetFilter.GetObjectInRadius(TargetHelper.HostileTargets, range));
                        var tar = ChoiceTarget(canReachTars);
                        if (tar == null) return false;
                        Target = tar;
                        return true;

                    case 2: // 圆形范围攻击。找到能覆盖最多的位置，并且选血最多的来。
                        tar = ChoiceTarget(TargetFilter.GetMostObjectInRadius(FilterForTarget(TargetHelper.HostileTargets), range, Action.EffectRange, false, mustUse));
                        if (tar == null) return false;
                        Target = tar;
                        return true;
                    case 3: // 扇形范围攻击。找到能覆盖最多的位置，并且选最远的来。
                        tar = ChoiceTarget(TargetFilter.GetMostObjectInArc(FilterForTarget(TargetHelper.HostileTargets), Action.EffectRange, mustUse));
                        if (tar == null) return false;
                        Target = tar;
                        return true;
                    case 4: //直线范围攻击。找到能覆盖最多的位置，并且选最远的来。
                        tar = ChoiceTarget(TargetFilter.GetMostObjectInLine(FilterForTarget(TargetHelper.HostileTargets), range, mustUse));
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
                    //如果不用自动找目标，那就不打AOE
                    if (!IconReplacer.AutoTarget) return false;
                    var count = TargetFilter.GetObjectInRadius(TargetHelper.HostileTargets, Action.EffectRange).Length;
                    if (count < (mustUse ? 1 : Service.Configuration.HostileCount)) return false;
                }
                return true;
            }

            Target = Service.TargetManager.Target is BattleChara battle ? battle : Service.ClientState.LocalPlayer;
            return true;
        }
    }
}
