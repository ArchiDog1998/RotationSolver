using Dalamud.Game.ClientState.Objects.SubKinds;
using ECommons.DalamudServices;
using ECommons.ExcelServices;
using ECommons.GameFunctions;
using ECommons.GameHelpers;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Graphics.Scene;
using RotationSolver.Basic.Configuration;

namespace RotationSolver.Basic.Actions;

public partial class BaseAction
{
    /// <summary>
    /// If it is aoe. How many targets this action needs.
    /// </summary>
    public byte AOECount
    {
        get
        {
            return OtherConfiguration.ActionAOECounts.TryGetValue(ID, out var count)
                ? count : IsFriendly ? (byte)1 : (byte)3;
        }
        set
        {
            OtherConfiguration.ActionAOECounts[ID] = value;
            OtherConfiguration.SaveActionAOECounts();
        }
    }

    /// <summary>
    /// How many time does this action need the target keep in live.
    /// </summary>
    public float TimeToKill
    {
        get
        {
            return OtherConfiguration.ActionTTK.TryGetValue(ID, out var time)
                ? time : 0;
        }
        set
        {
            OtherConfiguration.ActionTTK[ID] = value;
            OtherConfiguration.SaveActionTTK();
        }
    }

    /// <summary>
    /// Is this action single target?
    /// </summary>
    public bool IsSingleTarget => _action.CastType == 1;

    /// <summary>
    /// The action's target.
    /// </summary>
    public BattleChara Target { get; private set; } = Player.Object;

    /// <inheritdoc/>
    public BattleChara[] AffectedTargets { get; private set; } = Array.Empty<BattleChara>();

    internal bool IsTargetArea => _action.TargetArea;

    /// <summary>
    /// The position
    /// </summary>
    public Vector3 Position { get; private set; } = default;
    private uint _targetId = Player.Object?.ObjectId ?? 0;

    private Func<IEnumerable<BattleChara>, bool, BattleChara> _choiceTarget = null;

    /// <summary>
    /// How to choose the target.
    /// </summary>
    public Func<IEnumerable<BattleChara>, bool, BattleChara> ChoiceTarget
    {
        get
        {
            if (_choiceTarget != null) return _choiceTarget;
            return IsFriendly ? TargetFilter.DefaultChooseFriend : TargetFilter.DefaultFindHostile;
        }
        init => _choiceTarget = value;
    }

    /// <summary>
    /// Filter for the targets.
    /// </summary>
    public Func<IEnumerable<BattleChara>, IEnumerable<BattleChara>> FilterForHostiles { get; init; } = null;

    /// <summary>
    /// What status this action could put to the target.
    /// </summary>
    public StatusID[] TargetStatus { get; init; } = null;

    internal static bool TankDefenseSelf(BattleChara _, bool mustUse)
    {
        return (DataCenter.TarOnMeTargets.Any() || mustUse) && DataCenter.AverageTimeToKill > 8;
    }
    internal static bool TankBreakOtherCheck(Job id)
    {
        var tankHealth = id.GetHealthForDyingTank();

        return DataCenter.HasHostilesInRange
            && Player.Object.GetHealthRatio() < tankHealth
            && DataCenter.PartyMembersAverHP > tankHealth + 0.01f;
    }

    /// <inheritdoc/>
    public bool FindTarget(bool mustUse, byte aoeCount, out BattleChara target, out BattleChara[] affectedTargets)
    {
        aoeCount = Math.Max(aoeCount, mustUse ? (byte)1 : AOECount);

        Position = Player.Object.Position;
        var player = Player.Object;

        float range = Range;

        if (range == 0 && EffectRange == 0)
        {
            target = player;
            affectedTargets = Array.Empty<BattleChara>();
            return true;
        }
        else if (IsTargetArea)
        {
            target = player;
            affectedTargets = Array.Empty<BattleChara>();
            return TargetArea(range, mustUse, aoeCount, player);
        }
        else if (_action.CanTargetParty && _action.CanTargetHostile)
        {
            return TargetPartyAndHostile(range, mustUse, out target, out affectedTargets);
        }
        else if (_action.CanTargetParty)
        {
            return TargetParty(range, aoeCount, mustUse, out target, out affectedTargets);
        }
        else if (_action.CanTargetHostile)
        {
            return TargetHostile(range, mustUse, aoeCount, out target, out affectedTargets);
        }
        else if (_action.CanTargetSelf)
        {
            target = player;
            return TargetSelf(mustUse, aoeCount, out affectedTargets);
        }
        else
        {
            target = Svc.Targets.Target is BattleChara battle ? battle : player;
            affectedTargets = Array.Empty<BattleChara>();
            return true;
        }
    }

    private BattleChara[] GetAffectedTargets(IEnumerable<BattleChara> targets, BattleChara target)
    {
        return targets.Where(t =>
        {
            return CanGetTarget(target, t);
        }).ToArray();
    }

    #region TargetArea
    private bool TargetArea(float range, bool mustUse, int aoeCount, PlayerCharacter player)
    {
        //Moving
        if (EffectRange == 1 && range >= 15)
        {
            return TargetAreaMove(range, mustUse);
        }
        else if (IsFriendly)
        {
            return TargetAreaFriend(range, mustUse, player);
        }
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
        Position = target.Position;
        return true;
    }

    private bool TargetAreaMove(float range, bool mustUse)
    {
        if (Service.Config.GetValue(PluginConfigBool.MoveAreaActionFarthest))
        {
            Vector3 pPosition = Player.Object.Position;
            if (Service.Config.GetValue(PluginConfigBool.MoveTowardsScreenCenter)) unsafe
                {
                    var camera = CameraManager.Instance()->CurrentCamera;
                    var tar = camera->LookAtVector - camera->Object.Position;
                    tar.Y = 0;
                    var length = ((Vector3)tar).Length();
                    if (length == 0) return false;
                    tar = tar / length * range;
                    Position = new Vector3(pPosition.X + tar.X, pPosition.Y,
                    pPosition.Z + tar.Z);
                }
            else
            {
                float rotation = Player.Object.Rotation;
                Position = new Vector3(pPosition.X + (float)Math.Sin(rotation) * range, pPosition.Y,
                    pPosition.Z + (float)Math.Cos(rotation) * range);
            }
            return true;
        }
        else
        {
            var availableCharas = DataCenter.AllTargets.Where(b => b.ObjectId != Player.Object.ObjectId);
            var target = TargetFilter.GetObjectInRadius(availableCharas, range).FindTargetForMoving(mustUse);
            if (target == null) return false;
            Position = target.Position;
            return true;
        }
    }

    private bool TargetAreaFriend(float range, bool mustUse, PlayerCharacter player)
    {
        if (!Service.Config.GetValue(PluginConfigBool.UseGroundBeneficialAbility)) return false;
        if (!Service.Config.GetValue(PluginConfigBool.UseGroundBeneficialAbilityWhenMoving) && DataCenter.IsMoving) return false;

        var strategy = Service.Config.GetValue(PluginConfigInt.BeneficialAreaStrategy);
        switch (strategy)
        {
            case 0: // Find from list
            case 1: // Only the list
                OtherConfiguration.BeneficialPositions.TryGetValue(Svc.ClientState.TerritoryType, out var pts);

                pts ??= Array.Empty<Vector3>();

                if (pts.Length == 0)
                {
                    if (DataCenter.TerritoryContentType == TerritoryContentType.Trials ||
                        DataCenter.TerritoryContentType == TerritoryContentType.Raids
                        && DataCenter.AllianceMembers.Count(p => p is PlayerCharacter) == 8)
                    {
                        pts = pts.Union(new Vector3[] { Vector3.Zero, new Vector3(100, 0, 100) }).ToArray();
                    }
                }

                if (pts.Length > 0)
                {
                    var closest = pts.MinBy(p => Vector3.Distance(player.Position, p));
                    var rotation = new Random().NextDouble() * Math.Tau;
                    var radius = new Random().NextDouble() * 1;
                    closest.X += (float)(Math.Sin(rotation) * radius);
                    closest.Z += (float)(Math.Cos(rotation) * radius);
                    if (Vector3.Distance(player.Position, closest) < player.HitboxRadius + EffectRange)
                    {
                        Position = closest;
                        return true;
                    }
                }

                if (strategy == 1) return false;
                break;

            case 2: // Target
                if (Svc.Targets.Target != null && Svc.Targets.Target.DistanceToPlayer() < range)
                {
                    Position = Svc.Targets.Target.Position;
                    return true;
                }
                break;
        }

        if (Svc.Targets.Target is BattleChara b && b.DistanceToPlayer() < range &&
            b.IsBossFromIcon() && b.HasPositional() && b.HitboxRadius <= 8)
        {
            Position = b.Position;
        }
        else
        {
            var effectRange = EffectRange;
            var attackT = TargetFilter.FindAttackedTarget(DataCenter.PartyTanks.GetObjectInRadius(range + effectRange), mustUse);

            if (attackT == null)
            {
                Position = player.Position;
            }
            else
            {
                var disToTankRound = Vector3.Distance(player.Position, attackT.Position) + attackT.HitboxRadius;

                if (disToTankRound < effectRange
                    || disToTankRound > 2 * effectRange - player.HitboxRadius)
                {
                    Position = player.Position;
                }
                else
                {
                    Vector3 directionToTank = attackT.Position - player.Position;
                    var MoveDirection = directionToTank / directionToTank.Length() * Math.Max(0, disToTankRound - effectRange);
                    Position = player.Position + MoveDirection;
                }
            }
        }
        return true;
    }
    #endregion

    private bool TargetPartyAndHostile(float range, bool mustUse, out BattleChara target, out BattleChara[] affectedTargets)
    {
        var availableCharas = DataCenter.PartyMembers.Union(DataCenter.HostileTargets)
            .Where(b => b.ObjectId != Player.Object.ObjectId);
        availableCharas = TargetFilter.GetObjectInRadius(availableCharas, range).Where(CanUseTo);

        target = ChoiceTarget(availableCharas, mustUse);
        if (target == null)
        {
            affectedTargets = Array.Empty<BattleChara>();
            return false;
        }

        affectedTargets = new BattleChara[] { target };
        return true;
    }

    #region Target party
    private bool TargetParty(float range, int aoeCount, bool mustUse, out BattleChara target, out BattleChara[] affectedTargets)
    {
        if (_action.PrimaryCostType == 3 && _action.PrimaryCostValue == 24
            || (ActionID)ID is ActionID.AngelWhisper or ActionID.VariantRaise or ActionID.VariantRaise2)
        {
            return TargetDeath(out target, out affectedTargets);
        }

        var availableCharas = DataCenter.PartyMembers.Where(player => player.CurrentHp != 0);

        if (Service.Config.GetValue(PluginConfigBool.TargetAllForFriendly) ? _action.CanTargetFriendly : (ActionID)ID == ActionID.AethericMimicry)
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
            affectedTargets = Array.Empty<BattleChara>();
            return false;
        }

        if (!IsSingleTarget && (ActionID)ID != ActionID.DeploymentTactics)
        {
            target = ChoiceTarget(GetMostObjects(availableCharas, aoeCount), mustUse);
        }
        else
        {
            if (Service.Config.GetValue(PluginConfigBool.OnlyHotOnTanks) && IsEot)
            {
                availableCharas = availableCharas.Where(b => b.IsJobCategory(JobRole.Tank));
            }
            var targetingCharas = TargetFilter.GetObjectInRadius(availableCharas, range).Where(CanUseTo);
            if (targetingCharas == null || !targetingCharas.Any())
            {
                target = null;
                affectedTargets = Array.Empty<BattleChara>();
                return false;
            }

            target = ChoiceTarget(targetingCharas, mustUse);
        }
        if (target == null)
        {
            affectedTargets = Array.Empty<BattleChara>();
            return false;
        }

        affectedTargets = GetAffectedTargets(availableCharas, target);
        return mustUse || CheckStatus(target);
    }

    private static bool TargetDeath(out BattleChara target, out BattleChara[] affectedTargets)
    {
        target = TargetFilter.GetDeathPeople(DataCenter.DeathPeopleAll, DataCenter.DeathPeopleParty);
        if (target == null)
        {
            affectedTargets = Array.Empty<BattleChara>();
            return false;
        }

        affectedTargets = new BattleChara[] { target };
        return true;
    }
    #endregion

    #region Target Hostile
    private bool TargetHostile(float range, bool mustUse, int aoeCount, out BattleChara target, out BattleChara[] affectedTargets)
    {
        if (DataCenter.IsManual)
        {
            if (Svc.Targets.Target is BattleChara b && b.IsEnemy() && b.DistanceToPlayer() <= range)
            {
                return TargetHostileManual(b, mustUse, aoeCount, out target, out affectedTargets);
            }

            target = null;
            affectedTargets = Array.Empty<BattleChara>();
            return false;
        }

        if (!IsSingleTarget && NoAOE)
        {
            target = null;
            affectedTargets = Array.Empty<BattleChara>();
            return false;
        }

        if (Service.Config.GetValue(PluginConfigBool.ChooseAttackMark))
        {
            var b = MarkingHelper.GetAttackMarkChara(DataCenter.HostileTargets);
            if (b != null && ChoiceTarget(GetMostObjects(TargetFilterFuncEot(new BattleChara[] { b }, mustUse), Service.Config.GetValue(PluginConfigBool.CanAttackMarkAOE) ? aoeCount : int.MaxValue), mustUse) != null)
            {
                target = b;
                affectedTargets = GetAffectedTargets(DataCenter.AllHostileTargets, target);
                return true;
            }
        }

        target = ChoiceTarget(GetMostObjects(TargetFilterFuncEot(DataCenter.HostileTargets, mustUse), aoeCount), mustUse);
        if (target == null)
        {
            affectedTargets = Array.Empty<BattleChara>();
            return false;
        }
        affectedTargets = GetAffectedTargets(DataCenter.AllHostileTargets, target);
        return true;
    }

    private bool TargetHostileManual(BattleChara b, bool mustUse, int aoeCount, out BattleChara target, out BattleChara[] affectedTargets)
    {
        target = b;
        affectedTargets = Array.Empty<BattleChara>();
        if (!CanUseTo(b)) return false;
        if (ChoiceTarget(TargetFilterFuncEot(new BattleChara[] { b }, mustUse), mustUse) == null) return false;

        if (IsSingleTarget)
        {
            if (!mustUse)
            {
                var time = b.GetTimeToKill();

                //No need to dot.
                if (TargetStatus != null && !float.IsNaN(time) && time < TimeToKill) return false;

                //Already has status.
                if (!CheckStatus(b)) return false;
            }

            affectedTargets = new BattleChara[] { target };
            return true;
        }

        if (Service.Config.GetValue(PluginConfigBool.UseAOEAction)
            && Service.Config.GetValue(PluginConfigBool.UseAOEWhenManual) || mustUse)
        {
            if (GetMostObjects(TargetFilterFuncEot(DataCenter.HostileTargets, mustUse), aoeCount).Contains(b))
            {
                affectedTargets = GetAffectedTargets(DataCenter.AllHostileTargets, target);
                return true;
            }
        }
        target = null;
        return false;
    }
    #endregion

    private bool TargetSelf(bool mustUse, int aoeCount, out BattleChara[] affectedTargets)
    {
        if (EffectRange <= 0)
        {
            affectedTargets = new BattleChara[] { Player.Object };
            return true;
        }
        affectedTargets = Array.Empty<BattleChara>();

        if (IsFriendly)
        {
            var tars = TargetFilter.GetObjectInRadius(TargetFilterFuncEot(DataCenter.PartyMembers, mustUse), EffectRange);
            if (tars.Count() < aoeCount)
            {
                return false;
            }

            affectedTargets = GetAffectedTargets(DataCenter.PartyMembers, Player.Object);
        }
        else
        {
            if (NoAOE) return false;

            //not use when aoe.
            if (DataCenter.IsManual)
            {
                if (!Service.Config.GetValue(PluginConfigBool.UseAOEWhenManual) && !mustUse) return false;
            }

            var tars = TargetFilter.GetObjectInRadius(TargetFilterFuncEot(DataCenter.HostileTargets, mustUse), EffectRange);

            affectedTargets = GetAffectedTargets(DataCenter.AllHostileTargets, Player.Object);

            if (tars.Count() < aoeCount) return false;

            if (Service.Config.GetValue(PluginConfigBool.NoNewHostiles) && TargetFilter.GetObjectInRadius(DataCenter.AllHostileTargets, EffectRange)
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

        if (IsSingleTarget) return canGetObj;

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
            if (target == t)
            {
                count++;
            }
            else if (CanGetTarget(target, t))
            {
                count++;
            }
        }
        if (Service.Config.GetValue(PluginConfigBool.NoNewHostiles))
        {
            if (DataCenter.AllHostileTargets.Where(t => t.TargetObject == null)
                .Any(t => CanGetTarget(target, t))) return 0;
        }
        return count;
    }

    const double _alpha = Math.PI / 3;

    private bool CanGetTarget(BattleChara target, BattleChara subTarget)
    {
        if (target == null) return false;
        if (IsSingleTarget) return false;

        var pPos = Player.Object.Position;
        Vector3 dir = target.Position - pPos;
        Vector3 tdir = subTarget.Position - pPos;

        switch (_action.CastType)
        {
            case 2: // Circle
                return Vector3.Distance(target.Position, subTarget.Position) - subTarget.HitboxRadius <= EffectRange;

            case 3: // Sector
                if (subTarget.DistanceToPlayer() > EffectRange) return false;
                tdir += dir / dir.Length() * target.HitboxRadius / (float)Math.Sin(_alpha);
                return Vector3.Dot(dir, tdir) / (dir.Length() * tdir.Length()) >= Math.Cos(_alpha);

            case 4: //Line
                if (subTarget.DistanceToPlayer() > EffectRange) return false;

                return Vector3.Cross(dir, tdir).Length() / dir.Length() <= 2 + target.HitboxRadius
                    && Vector3.Dot(dir, tdir) >= 0;

            case 10: //Donut
                var dis = Vector3.Distance(target.Position, subTarget.Position) - subTarget.HitboxRadius;
                return dis <= EffectRange && dis >= 8;
        }

        Svc.Log.Debug(Name + "'s CastType is not valid! The value is " + _action.CastType.ToString());
        return false;
    }
    #endregion

    private IEnumerable<BattleChara> TargetFilterFuncEot(IEnumerable<BattleChara> tars, bool mustUse)
    {
        if (AttackType == AttackType.Magic)
        {
            tars = tars.Where(t => !t.HasStatus(false, StatusID.MagicResistance));
        }
        else if (Range >= 20)
        {
            tars = tars.Where(t => !t.HasStatus(false, StatusID.RangedResistance, StatusID.EnergyField));
        }

        if (FilterForHostiles != null)
        {
            var filtered = FilterForHostiles(tars);
            if (filtered.Any() || !mustUse) tars = filtered;
        }
        if (TargetStatus == null || !IsEot) return tars;

        var dontHave = tars.Where(CheckStatus);
        var canDot = dontHave.Where(b =>
        {
            var time = b.GetTimeToKill();
            return float.IsNaN(time) || time >= TimeToKill;
        });

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

        return tar.WillStatusEndGCD(GetDotGcdCount?.Invoke() ?? (uint)Service.Config.GetValue(DataCenter.Job, JobConfigInt.AddDotGCDCount),
            0, true, TargetStatus);
    }

    private unsafe bool CanUseTo(GameObject tar)
    {
        if (tar == null || !Player.Available) return false;

        var tarAddress = tar.Struct();

        if (!IsTargetArea && (ActionID)ID != ActionID.AethericMimicry
            && !ActionManager.CanUseActionOnTarget(AdjustedID, tarAddress)) return false;

        return tar.CanSee();
    }

    private static bool NoAOE
    {
        get
        {
            if (!Service.Config.GetValue(PluginConfigBool.UseAOEAction)) return true;

            return Service.Config.GetValue(PluginConfigBool.ChooseAttackMark)
                && !Service.Config.GetValue(PluginConfigBool.CanAttackMarkAOE)
                && MarkingHelper.HaveAttackChara(DataCenter.HostileTargets);
        }
    }
}
