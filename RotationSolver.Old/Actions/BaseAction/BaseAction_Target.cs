using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Logging;
using FFXIVClientStructs.FFXIV.Client.Game;
using RotationSolver.Commands;
using RotationSolver.Data;
using RotationSolver.Helpers;
using RotationSolver.Updaters;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
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
    private uint _targetId = Service.ClientState.LocalPlayer.ObjectId;

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

    internal Func<IEnumerable<BattleChara>, IEnumerable<BattleChara>> FilterForHostiles { private get; set; } = null;

    public StatusID[] TargetStatus { get; set; } = null;

    internal static bool TankDefenseSelf(BattleChara chara)
    {
        return TargetUpdater.TarOnMeTargets.Any();
    }
    internal static bool TankBreakOtherCheck(ClassJobID id)
    {
        var tankHealth = id.GetHealthForDyingTank();

        return TargetUpdater.HasHostilesInRange
            && Service.ClientState.LocalPlayer.GetHealthRatio() < tankHealth
            && TargetUpdater.PartyMembersAverHP > tankHealth + 0.01f;
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
            target = player;
            return TargetArea(range, mustUse, aoeCount, player);
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
            target = Service.TargetManager.Target is BattleChara battle ? battle : player;
            return true;
        }
    }

    #region TargetArea
    private bool TargetArea(float range, bool mustUse, int aoeCount, PlayerCharacter player)
    {
        //移动
        if (_action.EffectRange == 1 && range >= 15)
        {
            return TargetAreaMove(range, mustUse);
        }
        //其他友方
        else if (_isFriendly)
        {
            return TargetAreaFriend(range, mustUse, player);
        }
        //敌方
        else
        {
            return TargetAreaHostile(aoeCount);
        }
    }

    private bool TargetAreaHostile(int aoeCount)
    {
        var target = GetMostObjects(TargetUpdater.HostileTargets, aoeCount)
            .OrderByDescending(ObjectHelper.GetHealthRatio).FirstOrDefault();
        if (target == null) return false;
        _position = target.Position;
        return true;
    }

    private bool TargetAreaMove(float range, bool mustUse)
    {
        if (Service.Configuration.MoveAreaActionFarthest)
        {
            Vector3 pPosition = Service.ClientState.LocalPlayer.Position;
            float rotation = Service.ClientState.LocalPlayer.Rotation;
            _position = new Vector3(pPosition.X + (float)Math.Sin(rotation) * range, pPosition.Y,
                pPosition.Z + (float)Math.Cos(rotation) * range);
            return true;
        }
        else
        {
            var availableCharas = TargetUpdater.AllTargets.Where(b => b.ObjectId != Service.ClientState.LocalPlayer.ObjectId);
            var target = TargetFilter.GetObjectInRadius(availableCharas, range).FindTargetForMoving(mustUse);
            if (target == null) return false;
            _position = target.Position;
            return true;
        }
    }

    private bool TargetAreaFriend(float range, bool mustUse, PlayerCharacter player)
    {
        //如果用户不想使用自动友方地面放置功能
        if (!Service.Configuration.UseGroundBeneficialAbility) return false;

        //如果当前目标是Boss且有身位，放他身上。
        if (Service.TargetManager.Target is BattleChara b && b.DistanceToPlayer() < range && b.IsBoss() && b.HasPositional())
        {
            _position = b.Position;
            return true;
        }
        //计算玩家和被打的Ｔ之间的关系。
        else
        {
            var attackT = TargetFilter.FindAttackedTarget(TargetUpdater.PartyTanks.GetObjectInRadius(range + _action.EffectRange), mustUse);

            if (attackT == null)
            {
                _position = player.Position;
            }
            else
            {
                var disToTankRound = Math.Max(range, Vector3.Distance(player.Position, attackT.Position) + attackT.HitboxRadius);

                if (disToTankRound < _action.EffectRange
                    || disToTankRound > 2 * _action.EffectRange - player.HitboxRadius
                    || disToTankRound > range)
                {
                    _position = player.Position;
                }
                else
                {
                    Vector3 directionToTank = attackT.Position - player.Position;
                    var MoveDirection = directionToTank / directionToTank.Length() * (disToTankRound - _action.EffectRange);
                    _position = player.Position + MoveDirection;
                }
            }
            return true;
        }
    }
    #endregion

    private bool TargetPartyAndHostile(float range, bool mustUse, out BattleChara target)
    {
        var availableCharas = TargetUpdater.PartyMembers.Union(TargetUpdater.HostileTargets)
            .Where(b => b.ObjectId != Service.ClientState.LocalPlayer.ObjectId);
        availableCharas = TargetFilter.GetObjectInRadius(availableCharas, range).Where(CanTargetTo);

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

        if (Service.Configuration.TargetFriendly ? _action.CanTargetFriendly : (ActionID)ID == ActionID.AethericMimicry)
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
            availableCharas = TargetFilter.GetObjectInRadius(availableCharas, range)
                .Where(CanTargetTo);
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
            if (Service.TargetManager.Target is BattleChara b && b.IsNPCEnemy() && b.DistanceToPlayer() <= range)
            {
                return TargetHostileManual(b, mustUse, aoeCount, out target);
            }

            target = null;
            return false;
        }

        //判断一下AOE攻击的时候如果有攻击目标标记目标
        if (_action.CastType > 1 && NoAOE)
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
        target = b;
        if (!CanTargetTo(b)) return false;

        if (_action.CastType == 1)
        {
            if (!mustUse)
            {
                //No need to dot.
                if (TargetStatus != null && !ObjectHelper.CanDot(b)) return false;

                //Already has status.
                if (!CheckStatus(b)) return false;
            }

            return true;
        }

        if (Service.Configuration.UseAOEAction && Service.Configuration.UseAOEWhenManual || mustUse)
        {
            if (GetMostObjects(TargetFilterFuncEot(TargetUpdater.HostileTargets, mustUse), aoeCount).Contains(b))
            {
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
            if (NoAOE)
            {
                return false;
            }

            //如果不用自动找目标，那就不打AOE
            if (RSCommands.StateType == StateCommandType.Manual)
            {
                if (!Service.Configuration.UseAOEWhenManual && !mustUse) return false;
            }

            var tars = TargetFilter.GetObjectInRadius(TargetFilterFuncEot(TargetUpdater.HostileTargets, mustUse), _action.EffectRange);
            if (tars.Count() < aoeCount) return false;
            
            if (Service.Configuration.NoNewHostiles && TargetFilter.GetObjectInRadius(TargetUpdater.AllHostileTargets, _action.EffectRange)
                .Any(t => t.TargetObject == null)) return false;
        }
        return true;
    }

    #region Get Most Target
    private IEnumerable<BattleChara> GetMostObjects(IEnumerable<BattleChara> targets, int maxCount)
    {
        var range = Range;
        var canAttack = targets.Where(t => t.DistanceToPlayer() <= range + _action.EffectRange);
        var canGetObj = canAttack.Where(t => t.DistanceToPlayer() <= range && CanTargetTo(t));

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
    {
        int count = 0;
        foreach (var t in canAttack)
        {
            if(target == t)
            {
                count++;
            }
            else if(CanGetTarget(target, t))
            {
                count++;
            }
        }
        if (Service.Configuration.NoNewHostiles)
        {
            if (TargetUpdater.AllHostileTargets.Where(t => t.TargetObject == null)
                .Any(t => CanGetTarget(target, t))) return 0;
        }
        return count;
    }

    const double _alpha = Math.PI / 3;

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
                var dis = Vector3.Distance(target.Position, subTarget.Position) - subTarget.HitboxRadius;
                return dis <= _action.EffectRange && dis >= 8;

            case 2: // 圆形范围攻击
                return Vector3.Distance(target.Position, subTarget.Position) - subTarget.HitboxRadius <= _action.EffectRange;

            case 3: // Sector
                if(subTarget.DistanceToPlayer() > _action.EffectRange) return false;
                tdir += dir / dir.Length() * target.HitboxRadius / (float)Math.Sin(_alpha);
                return Vector3.Dot(dir, tdir) / (dir.Length() * tdir.Length()) >= Math.Cos(_alpha);

            case 4: //直线范围攻击
                if (subTarget.DistanceToPlayer() > _action.EffectRange) return false;
                return Vector3.Cross(dir, tdir).Length() / dir.Length() <= 2 + target.HitboxRadius;
        }

        PluginLog.LogDebug(Name + "'s CastType is not valid! The value is " + _action.CastType.ToString());
        return false;
    }
    #endregion

    private IEnumerable<BattleChara> TargetFilterFuncEot(IEnumerable<BattleChara> tars, bool mustUse)
    {
        if (FilterForHostiles != null) return FilterForHostiles(tars);
        if (TargetStatus == null || !_isEot) return tars;

        var dontHave = tars.Where(CheckStatus);
        var canDot = dontHave.Where(ObjectHelper.CanDot);

        if (mustUse)
        {
            if (canDot.Any()) return canDot;
            if (dontHave.Any()) return dontHave;
            return tars;
        }
        else
        {
            return canDot;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="tar"></param>
    /// <returns>True for add Eot.</returns>
    bool CheckStatus(BattleChara tar)
    {
        if (tar == null) return false;

        if (TargetStatus == null) return true;

        return tar.WillStatusEndGCD(GetDotGcdCount?.Invoke() ?? (uint)Service.Configuration.AddDotGCDCount, 
            0, true, TargetStatus);
    }

    unsafe bool CanTargetTo(BattleChara tar)
    {
        if (tar == null) return false;

        var id = ActionManager.GetActionInRangeOrLoS(AdjustedID, (FFXIVClientStructs.FFXIV.Client.Game.Object.GameObject*)Service.Player,
            (FFXIVClientStructs.FFXIV.Client.Game.Object.GameObject*)tar.Address);

        return id is 0 or 565;
    }

    private static bool NoAOE
    {
        get
        {
            if (!Service.Configuration.UseAOEAction) return true;

            return Service.Configuration.ChooseAttackMark
                && !Service.Configuration.CanAttackMarkAOE
                && MarkingHelper.HaveAttackChara(TargetUpdater.HostileTargets);
        }
    }
}
