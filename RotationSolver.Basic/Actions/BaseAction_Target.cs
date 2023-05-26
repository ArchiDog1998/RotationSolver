using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Logging;
using ECommons.DalamudServices;
using ECommons.ExcelServices;
using ECommons.GameHelpers;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Common.Component.BGCollision;

namespace RotationSolver.Basic.Actions;

public partial class BaseAction
{
    public byte AOECount { private get; init; } = 3;

    public bool IsTargetDying => Target?.IsDying() ?? false;

    public bool IsTargetBoss => Target?.IsBoss() ?? false;

    public bool IsSingleTarget => _action.CastType == 1;

    /// <summary>
    /// The action's target.
    /// </summary>
    public BattleChara Target { get; private set; } = Player.Object;

    private Vector3 _position = default;
    private uint _targetId = Player.Object?.ObjectId ?? 0;

    private Func<IEnumerable<BattleChara>, bool, BattleChara> _choiceTarget = null;

    public Func<IEnumerable<BattleChara>, bool, BattleChara> ChoiceTarget
    {
        private get
        {
            if (_choiceTarget != null) return _choiceTarget;
            return IsFriendly ? TargetFilter.DefaultChooseFriend : TargetFilter.DefaultFindHostile;
        }
        init => _choiceTarget = value;
    }

    public Func<IEnumerable<BattleChara>, IEnumerable<BattleChara>> FilterForHostiles { get; init; } = null;

    public StatusID[] TargetStatus { get; init; } = null;

    internal static bool TankDefenseSelf(BattleChara chara, bool mustUse)
    {
        return DataCenter.TarOnMeTargets.Any() | mustUse;
    }
    internal static bool TankBreakOtherCheck(Job id)
    {
        var tankHealth = id.GetHealthForDyingTank();

        return DataCenter.HasHostilesInRange
            && Player.Object.GetHealthRatio() < tankHealth
            && DataCenter.PartyMembersAverHP > tankHealth + 0.01f;
    }

    private bool FindTarget(bool mustUse, out BattleChara target)
    {
        int aoeCount = mustUse ? 1 : AOECount;

        _position = Player.Object.Position;
        var player = Player.Object;

        float range = Range;

        if (range == 0 && EffectRange == 0)
        {
            target = player;
            return true;
        }
        else if (_action.TargetArea)
        {
            target = player;
            return TargetArea(range, mustUse, aoeCount, player);
        }
        else if (_action.CanTargetParty && _action.CanTargetHostile)
        {
            return TargetPartyAndHostile(range, mustUse, out target);
        }
        else if (_action.CanTargetParty)
        {
            return TargetParty(range, aoeCount, mustUse, out target);
        }
        else if (_action.CanTargetHostile)
        {
            return TargetHostile(range, mustUse, aoeCount, out target);
        }
        else if (_action.CanTargetSelf)
        {
            target = player;
            return TargetSelf(mustUse, aoeCount);
        }
        else
        {
            target = Svc.Targets.Target is BattleChara battle ? battle : player;
            return true;
        }
    }

    #region TargetArea
    private bool TargetArea(float range, bool mustUse, int aoeCount, PlayerCharacter player)
    {
        //移动
        if (EffectRange == 1 && range >= 15)
        {
            return TargetAreaMove(range, mustUse);
        }
        //其他友方
        else if (IsFriendly)
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
        var target = GetMostObjects(DataCenter.HostileTargets, aoeCount)
            .OrderByDescending(ObjectHelper.GetHealthRatio).FirstOrDefault();
        if (target == null) return false;
        _position = target.Position;
        return true;
    }

    private bool TargetAreaMove(float range, bool mustUse)
    {
        if (Service.Config.MoveAreaActionFarthest)
        {
            Vector3 pPosition = Player.Object.Position;
            float rotation = Player.Object.Rotation;
            _position = new Vector3(pPosition.X + (float)Math.Sin(rotation) * range, pPosition.Y,
                pPosition.Z + (float)Math.Cos(rotation) * range);
            return true;
        }
        else
        {
            var availableCharas = DataCenter.AllTargets.Where(b => b.ObjectId != Player.Object.ObjectId);
            var target = TargetFilter.GetObjectInRadius(availableCharas, range).FindTargetForMoving(mustUse);
            if (target == null) return false;
            _position = target.Position;
            return true;
        }
    }

    private bool TargetAreaFriend(float range, bool mustUse, PlayerCharacter player)
    {
        //如果用户不想使用自动友方地面放置功能
        if (!Configuration.PluginConfiguration.GetValue(SettingsCommand.UseGroundBeneficialAbility)) return false;

        if (Service.Config.BeneficialAreaOnTarget && Svc.Targets.Target != null)
        {
            _position = Svc.Targets.Target.Position;
        }
        else if (Svc.Targets.Target is BattleChara b && b.DistanceToPlayer() < range && 
            b.IsBoss() && b.HasPositional() && b.HitboxRadius <= 8)
        {
            _position = b.Position;
        }
        //计算玩家和被打的Ｔ之间的关系。
        else
        {
            var effectRange = (ActionID)ID == ActionID.LiturgyOfTheBell ? 20 : EffectRange;
            var attackT = TargetFilter.FindAttackedTarget(DataCenter.PartyTanks.GetObjectInRadius(range + effectRange), mustUse);

            if (attackT == null)
            {
                _position = player.Position;
            }
            else
            {
                var disToTankRound = Vector3.Distance(player.Position, attackT.Position) + attackT.HitboxRadius;

                if (disToTankRound < effectRange
                    || disToTankRound > 2 * effectRange - player.HitboxRadius)
                {
                    _position = player.Position;
                }
                else
                {
                    Vector3 directionToTank = attackT.Position - player.Position;
                    var MoveDirection = directionToTank / directionToTank.Length() * Math.Max(0, disToTankRound - effectRange);
                    _position = player.Position + MoveDirection;
                }
            }
        }
        return true;
    }
    #endregion

    private bool TargetPartyAndHostile(float range, bool mustUse, out BattleChara target)
    {
        var availableCharas = DataCenter.PartyMembers.Union(DataCenter.HostileTargets)
            .Where(b => b.ObjectId != Player.Object.ObjectId);
        availableCharas = TargetFilter.GetObjectInRadius(availableCharas, range).Where(CanUseTo);

        target = ChoiceTarget(availableCharas, mustUse);
        if (target == null) return false;
        return true;
    }

    #region Target party
    private bool TargetParty(float range, int aoeCount, bool mustUse, out BattleChara target)
    {
        if (_action.PrimaryCostType == 3 && _action.PrimaryCostValue == 24 || (ActionID)ID == ActionID.AngelWhisper)
        {
            return TargetDeath(out target);
        }

        var availableCharas = DataCenter.PartyMembers.Where(player => player.CurrentHp != 0);

        if (Service.Config.TargetFriendly ? _action.CanTargetFriendly : (ActionID)ID == ActionID.AethericMimicry)
        {
            availableCharas = availableCharas.Union(DataCenter.AllianceMembers);
        }
        if (!_action.CanTargetSelf)
        {
            availableCharas = availableCharas.Where(p => p.ObjectId != Player.Object.ObjectId);
        }
        if (!availableCharas.Any())
        {
            target = null;
            return false;
        }

        if (_action.CastType > 1 && (ActionID)ID != ActionID.DeploymentTactics)
        {
            target = ChoiceTarget(GetMostObjects(availableCharas, aoeCount), mustUse);
        }
        else
        {
            if(Service.Config.OnlyHotOnTanks && IsEot)
            {
                availableCharas = availableCharas.Where(b => b.IsJobCategory(JobRole.Tank));
            }
            availableCharas = TargetFilter.GetObjectInRadius(availableCharas, range).Where(CanUseTo);

            target = ChoiceTarget(availableCharas, mustUse);
        }
        if (target == null) return false;

        return mustUse || CheckStatus(target);
    }

    private static bool TargetDeath(out BattleChara target)
    {
        target = TargetFilter.GetDeathPeople(DataCenter.DeathPeopleAll, DataCenter.DeathPeopleParty);
        if (target == null) return false;
        return true;
    }
    #endregion

    #region Target Hostile
    private bool TargetHostile(float range, bool mustUse, int aoeCount, out BattleChara target)
    {
        if (DataCenter.StateType == StateCommandType.Manual)
        {
            if (Svc.Targets.Target is BattleChara b && b.IsNPCEnemy() && b.DistanceToPlayer() <= range)
            {
                return TargetHostileManual(b, mustUse, aoeCount, out target);
            }

            target = null;
            return false;
        }

        if (_action.CastType > 1 && NoAOE)
        {
            target = null;
            return false;
        }

        if (Service.Config.ChooseAttackMark)
        {
            var b = MarkingHelper.GetAttackMarkChara(DataCenter.HostileTargets);
            if (b != null && GetMostObjects(TargetFilterFuncEot(new BattleChara[] { b }, mustUse), Service.Config.CanAttackMarkAOE ?  aoeCount : int.MaxValue).Any())
            {
                target = b;
                return true;
            }
        }

        target = ChoiceTarget(GetMostObjects(TargetFilterFuncEot(DataCenter.HostileTargets, mustUse), aoeCount), mustUse);
        if (target == null) return false;
        return true;
    }

    private bool TargetHostileManual(BattleChara b, bool mustUse, int aoeCount, out BattleChara target)
    {
        target = b;
        if (!CanUseTo(b)) return false;
        if (!TargetFilterFuncEot(new BattleChara[] { b }, mustUse).Any()) return false;

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

        if (Configuration.PluginConfiguration.GetValue(SettingsCommand.UseAOEAction) 
            && Configuration.PluginConfiguration.GetValue(SettingsCommand.UseAOEWhenManual) || mustUse)
        {
            if (GetMostObjects(TargetFilterFuncEot(DataCenter.HostileTargets, mustUse), aoeCount).Contains(b))
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
        if (EffectRange > 0 && !IsFriendly)
        {
            if (NoAOE)
            {
                return false;
            }

            //not use when aoe.
            if (DataCenter.StateType == StateCommandType.Manual)
            {
                if (!Configuration.PluginConfiguration.GetValue(SettingsCommand.UseAOEWhenManual) && !mustUse) return false;
            }

            var tars = TargetFilter.GetObjectInRadius(TargetFilterFuncEot(DataCenter.HostileTargets, mustUse), EffectRange);
            if (tars.Count() < aoeCount) return false;
            
            if (Service.Config.NoNewHostiles && TargetFilter.GetObjectInRadius(DataCenter.AllHostileTargets, EffectRange)
                .Any(t => t.TargetObject == null)) return false;
        }
        return true;
    }

    #region Get Most Target
    private IEnumerable<BattleChara> GetMostObjects(IEnumerable<BattleChara> targets, int maxCount)
    {
        var range = Range;
        var canAttack = TargetFilter.GetObjectInRadius(targets, range + EffectRange);
        var canGetObj = TargetFilter.GetObjectInRadius(canAttack, range).Where(CanUseTo);

        if (_action.CastType == 1) return canGetObj;

        List<BattleChara> objectMax = new(canGetObj.Count());

        foreach (var t in canGetObj)
        {
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
        if (Service.Config.NoNewHostiles)
        {
            if (DataCenter.AllHostileTargets.Where(t => t.TargetObject == null)
                .Any(t => CanGetTarget(target, t))) return 0;
        }
        return count;
    }

    const double _alpha = Math.PI / 3;

    public bool CanGetTarget(BattleChara target, BattleChara subTarget)
    {
        if (target == null) return false;
        if (IsSingleTarget) return false;
        if (target.DistanceToPlayer() > Range) return false;

        var pPos = Player.Object.Position;
        Vector3 dir = target.Position - pPos;
        Vector3 tdir = subTarget.Position - pPos;

        switch (_action.CastType)
        {
            case 2: // Circle
                return Vector3.Distance(target.Position, subTarget.Position) - subTarget.HitboxRadius <= EffectRange;

            case 3: // Sector
                if(subTarget.DistanceToPlayer() > EffectRange) return false;
                tdir += dir / dir.Length() * target.HitboxRadius / (float)Math.Sin(_alpha);
                return Vector3.Dot(dir, tdir) / (dir.Length() * tdir.Length()) >= Math.Cos(_alpha);

            case 4: //Line
                if (subTarget.DistanceToPlayer() > EffectRange) return false;
                return Vector3.Cross(dir, tdir).Length() / dir.Length() <= 2 + target.HitboxRadius;

            case 10: //Donut
                var dis = Vector3.Distance(target.Position, subTarget.Position) - subTarget.HitboxRadius;
                return dis <= EffectRange && dis >= 8;
        }

        PluginLog.LogDebug(Name + "'s CastType is not valid! The value is " + _action.CastType.ToString());
        return false;
    }
    #endregion

    private IEnumerable<BattleChara> TargetFilterFuncEot(IEnumerable<BattleChara> tars, bool mustUse)
    {
        if (FilterForHostiles != null)
        {
            var filtered = FilterForHostiles(tars);
            if (filtered.Any() || !mustUse) tars = filtered;
        }
        if (TargetStatus == null || !IsEot) return tars;

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

        return tar.WillStatusEndGCD(GetDotGcdCount?.Invoke() ?? (uint)Service.Config.AddDotGCDCount, 
            0, true, TargetStatus);
    }

    public unsafe bool CanUseTo(BattleChara tar)
    {
        if (tar == null || !Player.Available) return false;

        var tarAddress = tar.GetAddress();

        if (!ActionManager.CanUseActionOnTarget(AdjustedID, tarAddress)) return false;

        var point = Player.Object.Position;
        var tarPt = tar.Position;
        var direction = tarPt - point;
        if(BGCollisionModule.Raycast(point, direction, out var _, direction.Length())) return false;

        return true;
    }

    private static bool NoAOE
    {
        get
        {
            if (!Configuration.PluginConfiguration.GetValue(SettingsCommand.UseAOEAction)) return true;

            return Service.Config.ChooseAttackMark
                && !Service.Config.CanAttackMarkAOE
                && MarkingHelper.HaveAttackChara(DataCenter.HostileTargets);
        }
    }
}
