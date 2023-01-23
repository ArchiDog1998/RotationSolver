using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Logging;
using RotationSolver.Commands;
using RotationSolver.Data;
using RotationSolver.Helpers;
using RotationSolver.Updaters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace RotationSolver.Actions.BaseAction;

internal partial class BaseAction
{
    public byte AOECount { private get; set; } = 3;

    /// <summary>
    /// Shortcut for Target.IsDying();
    /// </summary>
    public bool IsTargetDying => Target?.IsDying() ?? false;

    /// <summary>
    /// Shortcut for Target.IsBoss();
    /// </summary>
    public bool IsTargetBoss => Target?.IsBoss() ?? false;

    /// <summary>
    /// The action's target.
    /// </summary>
    public BattleChara Target { get; private set; } = Service.ClientState.LocalPlayer;

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

    public StatusID[] TargetStatus { get; set; } = null;

    internal static bool TankDefenseSelf(BattleChara chara)
    {
        return TargetUpdater.TarOnMeTargets.Any();
    }
    internal static bool TankBreakOtherCheck(ClassJobID id, BattleChara chara)
    {
        var tankHealth = id.GetHealthForDyingTank();

        return TargetUpdater.HasHostilesInRange
            && Service.ClientState.LocalPlayer.GetHealthRatio() < tankHealth
            && TargetUpdater.PartyMembersAverHP > tankHealth + 0.05f;
    }

    private bool FindTarget(bool mustUse, out BattleChara target)
    {
        int aoeCount = mustUse ? 1 : AOECount;

        _position = Service.ClientState.LocalPlayer.Position;
        var player = Service.ClientState.LocalPlayer;

        float range = Range;

        //如果都没有距离，这个还需要选对象嘛？选自己啊！
        if (range == 0 && _action.EffectRange == 0)
        {
            target = player;
            return true;
        }
        else if (_action.TargetArea)
        {
            return TargetArea(range, mustUse, aoeCount, out target);
        }
        //如果能对友方和敌方都能选中
        else if (_action.CanTargetParty && _action.CanTargetHostile)
        {
            return TargetPartyAndHostile(range, mustUse, out target);
        }
        //首先看看是不是能对小队成员进行操作的。
        else if (_action.CanTargetParty)
        {
            return TargetParty(range, aoeCount, mustUse, out target);
        }
        //再看看是否可以选中敌对的。
        else if (_action.CanTargetHostile)
        {
            return TargetHostile(range, mustUse, aoeCount, out target);
        }
        //如果只能选自己，那就选自己吧。
        else if (_action.CanTargetSelf)
        {
            target = player;
            return TargetSelf(mustUse, aoeCount);
        }
        else
        {
            target = Service.TargetManager.Target is BattleChara battle ? battle : Service.ClientState.LocalPlayer;
            return true;
        }
    }

    #region TargetArea
    private bool TargetArea(float range, bool mustUse, int aoeCount, out BattleChara target)
    {
        //移动
        if (_action.EffectRange == 1 && range >= 15)
        {
            return TargetAreaMove(range, mustUse, out target);
        }
        //其他友方
        else if (_isFriendly)
        {
            return TargetAreaFriend(range, mustUse, out target);
        }
        //敌方
        else
        {
            return TargetAreaHostile(aoeCount, out target);
        }
    }

    private bool TargetAreaHostile(int aoeCount, out BattleChara target)
    {
        target = GetMostObjects(TargetUpdater.HostileTargets, aoeCount)
            .OrderByDescending(ObjectHelper.GetHealthRatio).FirstOrDefault();
        if (target == null)
        {
            target = Service.ClientState.LocalPlayer;
            return false;
        }
        _position = target.Position;
        return true;
    }

    private bool TargetAreaMove(float range, bool mustUse, out BattleChara target)
    {
        var availableCharas = Service.ObjectTable.Where(b => b.ObjectId != Service.ClientState.LocalPlayer.ObjectId).OfType<BattleChara>();
        target = ChoiceTarget(TargetFilter.GetObjectInRadius(availableCharas, range), mustUse);
        if (target == null) return false;
        _position = target.Position;
        return true;
    }

    private bool TargetAreaFriend(float range, bool mustUse, out BattleChara target)
    {
        target = null;
        //如果用户不想使用自动友方地面放置功能
        if (!Service.Configuration.UseGroundBeneficialAbility) return false;

        //如果当前目标是Boss且有身位，放他身上。
        if (Service.TargetManager.Target is BattleChara b && b.DistanceToPlayer() < range && b.IsBoss() && b.HasPositional())
        {
            target = b;
            _position = target.Position;
            return true;
        }
        //计算玩家和被打的Ｔ之间的关系。
        else
        {
            var attackT = TargetFilter.FindAttackedTarget(TargetUpdater.PartyTanks.GetObjectInRadius(range + _action.EffectRange), mustUse);

            target = Service.ClientState.LocalPlayer;

            if (attackT == null)
            {
                _position = target.Position;
            }
            else
            {
                var disToTankRound = Math.Max(range, Vector3.Distance(target.Position, attackT.Position) + attackT.HitboxRadius);

                if (disToTankRound < _action.EffectRange
                    || disToTankRound > 2 * _action.EffectRange - target.HitboxRadius
                    || disToTankRound > range)
                {
                    _position = target.Position;
                }
                else
                {
                    Vector3 directionToTank = attackT.Position - target.Position;
                    var MoveDirection = directionToTank / directionToTank.Length() * (disToTankRound - _action.EffectRange);
                    _position = target.Position + MoveDirection;
                }
            }
            return true;
        }
    }
    #endregion

    private bool TargetPartyAndHostile(float range, bool mustUse, out BattleChara target)
    {
        var availableCharas = TargetUpdater.PartyMembers.Union(TargetUpdater.HostileTargets).Where(b => b.ObjectId != Service.ClientState.LocalPlayer.ObjectId);
        availableCharas = TargetFilter.GetObjectInRadius(availableCharas, range);

        //特殊选队友的方法。
        target = ChoiceTarget(availableCharas, mustUse);
        if (target == null) return false;
        return true;
    }

    #region Target party
    private bool TargetParty(float range, int aoeCount, bool mustUse, out BattleChara target)
    {
        //还消耗2400的蓝，那肯定是复活的。
        if (_action.PrimaryCostType == 3 && _action.PrimaryCostValue == 24 || (ActionID)ID == ActionID.AngelWhisper)
        {
            return TargetDeath(out target);
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
        if (!availableCharas.Any())
        {
            target = null;
            return false;
        }

        //判断是否是范围。
        if (_action.CastType > 1 && (ActionID)ID != ActionID.DeploymentTactics)
        {
            //找到能覆盖最多的位置，并且选血最少的来。
            target = ChoiceTarget(GetMostObjects(availableCharas, aoeCount), mustUse);

        }
        else
        {
            availableCharas = TargetFilter.GetObjectInRadius(availableCharas, range);
            //特殊选队友的方法。
            target = ChoiceTarget(availableCharas, mustUse);
        }

        return mustUse || CheckStatus(target);
    }

    private bool TargetDeath(out BattleChara target)
    {
        target = TargetFilter.GetDeathPeople(TargetUpdater.DeathPeopleAll, TargetUpdater.DeathPeopleParty);
        if (target == null) return false;
        return true;
    }
    #endregion

    #region Target Hostile
    private bool TargetHostile(float range, bool mustUse, int aoeCount, out BattleChara target)
    {
        //如果不用自动找目标，那就直接返回。
        if (RSCommands.StateType == StateCommandType.Manual)
        {
            if (Service.TargetManager.Target is BattleChara b && b.CanAttack() && b.DistanceToPlayer() <= range)
            {
                return TargetHostileManual(b, mustUse, aoeCount, out target);
            }

            target = null;
            return false;
        }

        //判断一下AOE攻击的时候如果有攻击目标标记目标
        if (_action.CastType > 1 && (NoAOEForAttackMark || Service.Configuration.AbsSingleTarget))
        {
            target = null;
            return false;
        }

        target = ChoiceTarget(GetMostObjects(TargetFilterFuncEot(TargetUpdater.HostileTargets, mustUse), aoeCount), mustUse);
        if (target == null) return false;
        return true;
    }

    private bool TargetHostileManual(BattleChara b, bool mustUse, int aoeCount, out BattleChara target)
    {
        if (_action.CastType == 1)
        {
            target = b;

            if (!mustUse)
            {
                //No need to dot.
                if (TargetStatus != null && !ObjectHelper.CanDot(b)) return false;

                //Already has status.
                if (!CheckStatus(b)) return false;
            }

            return true;
        }
        else if (Service.Configuration.AbsSingleTarget)
        {
            target = null;
            return false;
        }

        if (Service.Configuration.UseAOEWhenManual || mustUse)
        {
            if (GetMostObjects(TargetFilterFuncEot(TargetUpdater.HostileTargets, mustUse), aoeCount).Contains(b))
            {
                target = b;
                return true;
            }
        }
        target = null;
        return false;
    }
    #endregion

    private bool TargetSelf(bool mustUse, int aoeCount)
    {
        if (_action.EffectRange > 0 && !_isFriendly)
        {
            if (NoAOEForAttackMark || Service.Configuration.AbsSingleTarget)
            {
                return false;
            }

            //如果不用自动找目标，那就不打AOE
            if (RSCommands.StateType == StateCommandType.Manual)
            {
                if (!Service.Configuration.UseAOEWhenManual && !mustUse) return false;
            }
            var count = TargetFilter.GetObjectInRadius(TargetFilterFuncEot(TargetUpdater.HostileTargets, mustUse), _action.EffectRange).Count();

            if (count < aoeCount) return false;
        }
        return true;
    }

    #region Get Most Target
    private IEnumerable<BattleChara> GetMostObjects(IEnumerable<BattleChara> targets, int maxCount)
    {
        var range = Range;
        var canAttack = targets.Where(t => t.DistanceToPlayer() <= range + _action.EffectRange);
        var canGetObj = canAttack.Where(t => t.DistanceToPlayer() <= range);

        if (_action.CastType == 1) return canGetObj;

        //能打到MaxCount以上数量的怪的怪。
        List<BattleChara> objectMax = new List<BattleChara>(canGetObj.Count());

        //循环能打中的目标。
        foreach (var t in canGetObj)
        {
            //计算能达到的所有怪的数量。
            int count = CanGetTargetCount(t, canAttack);

            if (count == maxCount)
            {
                objectMax.Add(t);
            }
            else if (count > maxCount)
            {
                maxCount = count;
                objectMax.Clear();
                objectMax.Add(t);
            }
        }

        return objectMax;
    }

    private int CanGetTargetCount(BattleChara target, IEnumerable<BattleChara> canAttack)
        => canAttack.Count(g => CanGetTarget(target, g));

    internal bool CanGetTarget(BattleChara target, BattleChara subTarget)
    {
        if (target == null) return false;
        if (_action.CastType == 1) return false;
        if (target.DistanceToPlayer() > Range) return false;

        var pPos = Service.ClientState.LocalPlayer.Position;
        Vector3 dir = target.Position - pPos;
        Vector3 tdir = subTarget.Position - pPos;

        switch (_action.CastType)
        {
            case 10: //环形范围攻击也就这么判断吧，我烦了。
            case 2: // 圆形范围攻击
                return Vector3.Distance(target.Position, subTarget.Position) - subTarget.HitboxRadius <= _action.EffectRange;

            case 3: // 扇形范围攻击
                double cos = Vector3.Dot(dir, tdir) / (dir.Length() * tdir.Length());
                return subTarget.DistanceToPlayer() <= _action.EffectRange && cos >= 0.5;

            case 4: //直线范围攻击
                double distance = Vector3.Cross(dir, tdir).Length() / dir.Length();
                return subTarget.DistanceToPlayer() <= _action.EffectRange && distance <= 2;
        }

        PluginLog.LogDebug(Name + "'s CastType is not valid! The value is " + _action.CastType.ToString());
        return false;
    }
    #endregion

    private IEnumerable<BattleChara> TargetFilterFuncEot(IEnumerable<BattleChara> tars, bool mustUse)
    {
        if (FilterForTarget != null) return FilterForTarget(tars);
        if (TargetStatus == null || !_isEot || mustUse) return tars;

        var canDot = mustUse ? tars : tars.Where(ObjectHelper.CanDot);
        var DontHave = canDot.Where(CheckStatus);

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
    private bool CheckStatus(BattleChara tar)
    {
        if (tar == null) return false;

        if (TargetStatus == null) return true;

        return tar.WillStatusEndGCD((uint)Service.Configuration.AddDotGCDCount, 0, true, TargetStatus);
    }

    /// <summary>
    /// 开启攻击标记且有攻击标记目标且不开AOE。
    /// </summary>
    private static bool NoAOEForAttackMark =>
        Service.Configuration.ChooseAttackMark && !Service.Configuration.CanAttackMarkAOE
        && MarkingHelper.HaveAttackChara(TargetUpdater.HostileTargets);
}
