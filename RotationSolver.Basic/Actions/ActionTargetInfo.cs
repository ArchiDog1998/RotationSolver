using Dalamud.Game.ClientState.Objects.SubKinds;
using ECommons.DalamudServices;
using ECommons.ExcelServices;
using ECommons.GameFunctions;
using ECommons.GameHelpers;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Graphics.Scene;
using RotationSolver.Basic.Configuration;

namespace RotationSolver.Basic.Actions;

/// <summary>
/// The target info
/// </summary>
/// <param name="action">the input action.</param>
public struct ActionTargetInfo(IBaseAction action)
{
    /// <summary>
    /// The range of this action.
    /// </summary>
    public readonly float Range => ActionManager.GetActionRange(action.Info.ID);

    /// <summary>
    /// The effect range of this action.
    /// </summary>
    public readonly float EffectRange => (ActionID)action.Info.ID == ActionID.LiturgyOfTheBellPvE ? 20 : action.Action.EffectRange;

    /// <summary>
    /// Is this action single target.
    /// </summary>
    public readonly bool IsSingleTarget => action.Action.CastType == 1;
    /// <summary>
    /// Is this action target area.
    /// </summary>
    public readonly bool IsTargetArea => action.Action.TargetArea;

    /// <summary>
    /// Is this action friendly.
    /// </summary>
    public readonly bool IsTargetFriendly => action.Setting.IsFriendly;

    private static bool NoAOE
    {
        get
        {
            if (!Service.Config.UseAoeAction) return true;

            if (DataCenter.IsManual)
            {
                if (!Service.Config.UseAoeWhenManual) return true;
            }

            return Service.Config.ChooseAttackMark
                && !Service.Config.CanAttackMarkAoe
                && MarkingHelper.HaveAttackChara;
        }
    }

    #region Target Finder.
    private readonly IEnumerable<BattleChara> GetCanTargets(bool skipStatusProvideCheck, TargetType type)
    {
        var items = TargetFilter.GetObjectInRadius(DataCenter.AllTargets, Range);
        var objs = new List<BattleChara>(items.Count());

        foreach (var obj in items)
        {
            if (type == TargetType.Heal && obj.GetHealthRatio() == 1) continue;

            if (!GeneralCheck(obj, skipStatusProvideCheck)) continue;
            objs.Add(obj);
        }

        var isAuto = !DataCenter.IsManual || IsTargetFriendly;
        return objs.Where(b => isAuto || b.ObjectId == Svc.Targets.Target?.ObjectId)
            .Where(InViewTarget).Where(CanUseTo).Where(action.Setting.CanTarget);
    }

    private readonly List<BattleChara> GetCanAffects(bool skipStatusProvideCheck, TargetType type)
    {
        if (EffectRange == 0) return [];

        var items = TargetFilter.GetObjectInRadius(action.Setting.IsFriendly
            ? DataCenter.PartyMembers
            : DataCenter.AllHostileTargets,
            Range + EffectRange);

        if (type == TargetType.Heal)
        {
            items = items.Where(i => i.GetHealthRatio() < 1);
        }

        var objs = new List<BattleChara>(items.Count());

        foreach (var obj in items)
        {
            if (!GeneralCheck(obj, skipStatusProvideCheck)) continue;
            objs.Add(obj);
        }

        return objs;
    }

    private static bool InViewTarget(BattleChara gameObject)
    {
        if (Service.Config.OnlyAttackInView)
        {
            if (!Svc.GameGui.WorldToScreen(gameObject.Position, out _)) return false;
        }
        if (Service.Config.OnlyAttackInVisionCone)
        {
            Vector3 dir = gameObject.Position - Player.Object.Position;
            Vector2 dirVec = new(dir.Z, dir.X);
            double angle = Player.Object.GetFaceVector().AngleTo(dirVec);
            if (angle > Math.PI * Service.Config.AngleOfVisionCone / 360)
            {
                return false;
            }
        }
        return true;
    }

    private readonly unsafe bool CanUseTo(GameObject tar)
    {
        if (tar == null || !Player.Available) return false;
        if (tar.ObjectId == GameObject.InvalidGameObjectId) return false;
        if (tar.ObjectId == 0) return false;

        var tarAddress = tar.Struct();
        if (tarAddress == null) return false;

        if ((ActionID)action.Info.ID != ActionID.AethericMimicryPvE
            && !ActionManager.CanUseActionOnTarget(action.Info.AdjustedID, tarAddress)) return false;

        return tar.CanSee();
    }

    private readonly bool GeneralCheck(BattleChara gameObject, bool skipStatusProvideCheck)
    {
        if (!gameObject.IsTargetable) return false;

        if (!Service.Config.TargetAllForFriendly
            && gameObject.IsAlliance() && !gameObject.IsParty())
        {
            return false;
        }

        if (gameObject.IsEnemy())
        {
            //Can't attack.
            if (!gameObject.IsAttackable()) return false;
        }

        return CheckStatus(gameObject, skipStatusProvideCheck) 
            && CheckTimeToKill(gameObject)
            && CheckResistance(gameObject);
    }

    private readonly bool CheckStatus(GameObject gameObject, bool skipStatusProvideCheck)
    {
        if (!action.Config.ShouldCheckStatus) return true;

        if (action.Setting.TargetStatusNeed != null)
        {
            if (gameObject.WillStatusEndGCD(0, 0,
                action.Setting.StatusFromSelf, action.Setting.TargetStatusNeed)) return false;
        }

        if (action.Setting.TargetStatusProvide != null && !skipStatusProvideCheck)
        {
            if (!gameObject.WillStatusEndGCD(action.Config.StatusGcdCount, 0,
                action.Setting.StatusFromSelf, action.Setting.TargetStatusProvide)) return false;
        }

        return true;
    }

    private readonly bool CheckResistance(GameObject gameObject)
    {
        if (action.Info.AttackType == AttackType.Magic)
        {
            if (gameObject.HasStatus(false, StatusHelper.MagicResistance))
            {
                return false;
            }
        }
        else if(action.Info.Aspect != Aspect.Piercing) // Physic
        {
            if (gameObject.HasStatus(false, StatusHelper.PhysicResistancec))
            {
                return false;
            }
        }
        if (Range >= 20) // Range
        {
            if (gameObject.HasStatus(false, StatusID.RangedResistance, StatusID.EnergyField))
            {
                return false;
            }
        }

        return true;
    }

    private readonly bool CheckTimeToKill(GameObject gameObject)
    {
        if (gameObject is not BattleChara b) return false;
        var time = b.GetTimeToKill();
        return float.IsNaN(time) || time >= action.Config.TimeToKill;
    }

    #endregion

    /// <summary>
    /// Take a little long time..
    /// </summary>
    /// <returns></returns>
    internal readonly TargetResult? FindTarget(bool skipAoeCheck, bool skipStatusProvideCheck)
    {
        var range = Range;
        var player = Player.Object;

        if (range == 0 && EffectRange == 0)
        {
            return new(player, [], player.Position);
        }
        var type = action.Setting.TargetType;

        var canTargets = GetCanTargets(skipStatusProvideCheck, type);
        var canAffects = GetCanAffects(skipStatusProvideCheck, type);

        if (IsTargetArea)
        {
            return FindTargetArea(canTargets, canAffects, range, player);
        }

        var targets = GetMostCanTargetObjects(canTargets, canAffects, skipAoeCheck ? 0 : action.Config.AoeCount);
        var target = FindTargetByType(targets, type, action.Config.AutoHealRatio, action.Setting.SpecialType);
        if (target == null) return null;
        return new(target, [.. GetAffects(target, canAffects)], target.Position);
    }

    private readonly TargetResult? FindTargetArea(IEnumerable<BattleChara> canTargets, IEnumerable<BattleChara> canAffects,
        float range, PlayerCharacter player)
    {
        if (action.Setting.TargetType is TargetType.Move)
        {
            return FindTargetAreaMove(range);
        }
        else if (action.Setting.IsFriendly)
        {
            if (!Service.Config.UseGroundBeneficialAbility) return null;
            if (!Service.Config.UseGroundBeneficialAbilityWhenMoving && DataCenter.IsMoving) return null;

            return FindTargetAreaFriend(range, canAffects, player);
        }
        else
        {
            return FindTargetAreaHostile(canTargets, canAffects, action.Config.AoeCount);
        }
    }


    private readonly TargetResult? FindTargetAreaHostile(IEnumerable<BattleChara> canTargets, IEnumerable<BattleChara> canAffects, int aoeCount)
    {
        var target = GetMostCanTargetObjects(canTargets, canAffects, aoeCount)
            .OrderByDescending(ObjectHelper.GetHealthRatio).FirstOrDefault();
        if (target == null) return null;
        return new(target, [..GetAffects(target, canAffects)], target.Position);
    }

    private readonly TargetResult? FindTargetAreaMove(float range)
    {
        if (Service.Config.MoveAreaActionFarthest)
        {
            Vector3 pPosition = Player.Object.Position;
            if (Service.Config.MoveTowardsScreenCenter) unsafe
                {
                    var camera = CameraManager.Instance()->CurrentCamera;
                    var tar = camera->LookAtVector - camera->Object.Position;
                    tar.Y = 0;
                    var length = ((Vector3)tar).Length();
                    if (length == 0) return null;
                    tar = tar / length * range;
                    return new(Player.Object, [], new Vector3(pPosition.X + tar.X,
                        pPosition.Y, pPosition.Z + tar.Z));
                }
            else
            {
                float rotation = Player.Object.Rotation;
                return new(Player.Object, [], new Vector3(pPosition.X + (float)Math.Sin(rotation) * range,
                    pPosition.Y, pPosition.Z + (float)Math.Cos(rotation) * range));
            }
        }
        else
        {
            var availableCharas = DataCenter.AllTargets.Where(b => b.ObjectId != Player.Object.ObjectId);
            var target = FindTargetByType(TargetFilter.GetObjectInRadius(availableCharas, range),
                TargetType.Move, action.Config.AutoHealRatio, action.Setting.SpecialType);
            if (target == null) return null;
            return new(target, [], target.Position);
        }
    }


    private readonly TargetResult? FindTargetAreaFriend(float range, IEnumerable<BattleChara> canAffects, PlayerCharacter player)
    {
        var strategy = Service.Config.BeneficialAreaStrategy;
        switch (strategy)
        {
            case BeneficialAreaStrategy.OnLocations: // Find from list
            case BeneficialAreaStrategy.OnlyOnLocations: // Only the list
                OtherConfiguration.BeneficialPositions.TryGetValue(Svc.ClientState.TerritoryType, out var pts);

                pts ??= [];

                if (pts.Length == 0)
                {
                    if (DataCenter.TerritoryContentType == TerritoryContentType.Trials ||
                        DataCenter.TerritoryContentType == TerritoryContentType.Raids
                        && DataCenter.AllianceMembers.Count(p => p is PlayerCharacter) == 8)
                    {
                        pts = [.. pts, Vector3.Zero, new(100, 0, 100)];
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
                        return new(player, [.. GetAffects(closest, canAffects)], closest);
                    }
                }

                if (strategy == BeneficialAreaStrategy.OnlyOnLocations) return null;
                break;

            case BeneficialAreaStrategy.OnTarget: // Target
                if (Svc.Targets.Target != null && Svc.Targets.Target.DistanceToPlayer() < range)
                {
                    var target = Svc.Targets.Target as BattleChara;
                    return new(target, [.. GetAffects(target?.Position, canAffects)], target?.Position);
                }
                break;
        }

        if (Svc.Targets.Target is BattleChara b && b.DistanceToPlayer() < range &&
            b.IsBossFromIcon() && b.HasPositional() && b.HitboxRadius <= 8)
        {
            return new(b, [.. GetAffects(b.Position, canAffects)], b.Position);
        }
        else
        {
            var effectRange = EffectRange;
            var attackT = FindTargetByType(DataCenter.AllianceMembers.GetObjectInRadius(range + effectRange),
                TargetType.BeAttacked, action.Config.AutoHealRatio, action.Setting.SpecialType);

            if (attackT == null)
            {
                return new(player, [.. GetAffects(player.Position, canAffects)], player.Position);
            }
            else
            {
                var disToTankRound = Vector3.Distance(player.Position, attackT.Position) + attackT.HitboxRadius;

                if (disToTankRound < effectRange
                    || disToTankRound > 2 * effectRange - player.HitboxRadius)
                {
                    return new(player, [.. GetAffects(player.Position, canAffects)], player.Position);
                }
                else
                {
                    Vector3 directionToTank = attackT.Position - player.Position;
                    var MoveDirection = directionToTank / directionToTank.Length() * Math.Max(0, disToTankRound - effectRange);
                    return new(player, [.. GetAffects(player.Position, canAffects)], player.Position + MoveDirection);
                }
            }
        }
    }

    private readonly IEnumerable<BattleChara> GetAffects(Vector3? point, IEnumerable<BattleChara> canAffects)
    {
        if (point == null) yield break;
        foreach (var t in canAffects)
        {
            if (Vector3.Distance(point.Value, t.Position) - t.HitboxRadius <= EffectRange)
            {
                yield return t;
            }
        }
    }

    private readonly IEnumerable<BattleChara> GetAffects(BattleChara tar, IEnumerable<BattleChara> canAffects)
    {
        foreach (var t in canAffects)
        {
            if (CanGetTarget(tar, t))
            {
                yield return t;
            }
        }
    }

    #region Get Most Target
    private readonly IEnumerable<BattleChara> GetMostCanTargetObjects(IEnumerable<BattleChara> canTargets, IEnumerable<BattleChara> canAffects, int aoeCount)
    {
        if (IsSingleTarget || EffectRange <= 0) return canTargets;
        if (!action.Setting.IsFriendly && NoAOE) return [];
        if (aoeCount > 1 && DataCenter.IsManual) return [];

        List<BattleChara> objectMax = new(canTargets.Count());

        foreach (var t in canTargets)
        {
            int count = CanGetTargetCount(t, canAffects);

            if (count == aoeCount)
            {
                objectMax.Add(t);
            }
            else if (count > aoeCount)
            {
                aoeCount = count;
                objectMax.Clear();
                objectMax.Add(t);
            }
        }
        //if (aoeCount > 0 && objectMax.Count > 0 && objectMax.Count < action.Config.AoeCount && !action.Setting.IsFriendly)
        //{
        //    return [];
        //}
        //else
        {
            return objectMax;
        }
    }

    private readonly int CanGetTargetCount(GameObject target, IEnumerable<GameObject> canAffects)
    {
        int count = 0;
        foreach (var t in canAffects)
        {
            if (target != t && !CanGetTarget(target, t)) continue;

            if (Service.Config.NoNewHostiles
                && t.TargetObject == null)
            {
                return 0;
            }
            count++;
        }

        return count;
    }

    const double _alpha = Math.PI / 3;
    private readonly bool CanGetTarget(GameObject target, GameObject subTarget)
    {
        if (target == null) return false;

        var pPos = Player.Object.Position;
        Vector3 dir = target.Position - pPos;
        Vector3 tdir = subTarget.Position - pPos;

        switch (action.Action.CastType)
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

        Svc.Log.Debug(action.Action.Name.RawString + "'s CastType is not valid! The value is " + action.Action.CastType.ToString());
        return false;
    }
    #endregion

    #region TargetFind
    private static BattleChara? FindTargetByType(IEnumerable<BattleChara> gameObjects, TargetType type, float healRatio, SpecialActionType actionType)
    {
        if (type == TargetType.Self) return Player.Object;

        switch (actionType)
        {
            case SpecialActionType.MeleeRange:
                gameObjects = gameObjects.Where(t => t.DistanceToPlayer() >= 3 + Service.Config.MeleeRangeOffset);
                break;

            case SpecialActionType.MovingForward:
                if (DataCenter.MergedStatus.HasFlag(AutoStatus.MoveForward) || Service.CountDownTime > 0)
                {
                    type = TargetType.Move;
                }
                else
                {
                    gameObjects = gameObjects.Where(t => t.DistanceToPlayer() < 1);
                }
                break;
        }

        switch (type) // Filter the objects.
        {
            case TargetType.Death:
                gameObjects = gameObjects.Where(ObjectHelper.IsDeathToRaise);
                break;

            case TargetType.Move:
                break;

            default:
                gameObjects = gameObjects.Where(ObjectHelper.IsAlive);
                break;
        }

        return type switch //Find the object.
        {
            TargetType.Provoke => FindProvokeTarget(),
            TargetType.Dispel => FindDispelTarget(),
            TargetType.Death => FindDeathPeople(),
            TargetType.Move => FindTargetForMoving(),
            TargetType.Heal => FindHealTarget(healRatio),
            TargetType.BeAttacked => FindBeAttackedTarget(),
            TargetType.Interrupt => FindInterruptTarget(),
            TargetType.Tank => FindTankTarget(),
            TargetType.Melee => RandomMeleeTarget(gameObjects),
            TargetType.Range => RandomRangeTarget(gameObjects),
            TargetType.Magical => RandomMagicalTarget(gameObjects),
            TargetType.Physical => RandomPhysicalTarget(gameObjects),
            _ => FindHostile(),
        };

        BattleChara? FindProvokeTarget()
        {
            if (gameObjects.Any(o => o.ObjectId == DataCenter.ProvokeTarget?.ObjectId))
                return DataCenter.ProvokeTarget;
            return null;
        }

        BattleChara? FindDeathPeople()
        {
            if (gameObjects.Any(o => o.ObjectId == DataCenter.DeathTarget?.ObjectId))
                return DataCenter.DeathTarget;
            return null;
        }

        BattleChara? FindTargetForMoving()
        {
            const float DISTANCE_TO_MOVE = 3;

            if (Service.Config.MoveTowardsScreenCenter)
            {
                return FindMoveTargetScreenCenter();
            }
            else
            {
                return FindMoveTargetFaceDirection();
            }

            BattleChara? FindMoveTargetScreenCenter()
            {
                var pPosition = Player.Object.Position;
                if (!Svc.GameGui.WorldToScreen(pPosition, out var playerScrPos)) return null;

                var tars = gameObjects.Where(t =>
                {
                    if (t.DistanceToPlayer() < DISTANCE_TO_MOVE) return false;

                    if (!Svc.GameGui.WorldToScreen(t.Position, out var scrPos)) return false;

                    var dir = scrPos - playerScrPos;

                    if (dir.Y > 0) return false;

                    return Math.Abs(dir.X / dir.Y) <= Math.Tan(Math.PI * Service.Config.MoveTargetAngle / 360);
                }).OrderByDescending(ObjectHelper.DistanceToPlayer);

                return tars.FirstOrDefault();
            }

            BattleChara? FindMoveTargetFaceDirection()
            {
                Vector3 pPosition = Player.Object.Position;
                Vector2 faceVec = Player.Object.GetFaceVector();

                var tars = gameObjects.Where(t =>
                {
                    if (t.DistanceToPlayer() < DISTANCE_TO_MOVE) return false;

                    Vector3 dir = t.Position - pPosition;
                    Vector2 dirVec = new(dir.Z, dir.X);
                    double angle = faceVec.AngleTo(dirVec);
                    return angle <= Math.PI * Service.Config.MoveTargetAngle / 360;
                }).OrderByDescending(ObjectHelper.DistanceToPlayer);

                return tars.FirstOrDefault();
            }
        }

        BattleChara? FindHealTarget(float healRatio)
        {
            if (!gameObjects.Any()) return null;

            if (IBaseAction.AutoHealCheck)
            {
                gameObjects = gameObjects.Where(o => o.GetHealthRatio() < healRatio);
            }

            var partyMembers = gameObjects.Where(ObjectHelper.IsParty);

            return GeneralHealTarget(partyMembers)
                ?? GeneralHealTarget(gameObjects)
                ?? partyMembers.FirstOrDefault(t => t.HasStatus(false, StatusHelper.TankStanceStatus))
                ?? partyMembers.FirstOrDefault()
                ?? gameObjects.FirstOrDefault(t => t.HasStatus(false, StatusHelper.TankStanceStatus))
                ?? gameObjects.FirstOrDefault();

            static BattleChara? GeneralHealTarget(IEnumerable<BattleChara> objs)
            {
                objs = objs.Where(StatusHelper.NeedHealing).OrderBy(ObjectHelper.GetHealthRatio);

                var healerTars = objs.GetJobCategory(JobRole.Healer);
                var tankTars = objs.GetJobCategory(JobRole.Tank);

                var healerTar = healerTars.FirstOrDefault();
                if (healerTar != null && healerTar.GetHealthRatio() < Service.Config.HealthHealerRatio)
                    return healerTar;

                var tankTar = tankTars.FirstOrDefault();
                if (tankTar != null && tankTar.GetHealthRatio() < Service.Config.HealthTankRatio)
                    return tankTar;

                var tar = objs.FirstOrDefault();
                if (tar?.GetHealthRatio() < 1) return tar;

                return null;
            }
        }

        BattleChara? FindInterruptTarget()
        {
            if (gameObjects.Any(o => o.ObjectId == DataCenter.InterruptTarget?.ObjectId))
                return DataCenter.InterruptTarget;
            return null;
        }

        BattleChara? FindHostile()
        {
            if (gameObjects == null || !gameObjects.Any()) return null;

            if (Service.Config.FilterStopMark)
            {
                var cs = MarkingHelper.FilterStopCharaes(gameObjects);
                if (cs?.Any() ?? false) gameObjects = cs;
            }

            if (DataCenter.TreasureCharas.Length > 0)
            {
                var b = gameObjects.FirstOrDefault(b => b.ObjectId == DataCenter.TreasureCharas[0]);
                if (b != null) return b;
                gameObjects = gameObjects.Where(b => !DataCenter.TreasureCharas.Contains(b.ObjectId));
            }

            var highPriority = gameObjects.Where(ObjectHelper.IsTopPriorityHostile);
            if (highPriority.Any())
            {
                gameObjects = highPriority;
            }

            return FindHostileRaw();
        }

        BattleChara? FindHostileRaw()
        {
            gameObjects = DataCenter.TargetingType switch
            {
                TargetingType.Small => gameObjects.OrderBy(p => p.HitboxRadius),
                TargetingType.HighHP => gameObjects.OrderByDescending(p => p is BattleChara b ? b.CurrentHp : 0),
                TargetingType.LowHP => gameObjects.OrderBy(p => p is BattleChara b ? b.CurrentHp : 0),
                TargetingType.HighMaxHP => gameObjects.OrderByDescending(p => p is BattleChara b ? b.MaxHp : 0),
                TargetingType.LowMaxHP => gameObjects.OrderBy(p => p is BattleChara b ? b.MaxHp : 0),
                _ => gameObjects.OrderByDescending(p => p.HitboxRadius),
            };
            return gameObjects.FirstOrDefault();
        }

        BattleChara? FindBeAttackedTarget()
        {
            if (!gameObjects.Any()) return null;
            var attachedT = gameObjects.Where(ObjectHelper.IsTargetOnSelf);

            if (!DataCenter.AutoStatus.HasFlag(AutoStatus.DefenseSingle))
            {
                if (!attachedT.Any())
                {
                    attachedT = gameObjects.Where(tank => tank.HasStatus(false, StatusHelper.TankStanceStatus));
                }

                if (!attachedT.Any())
                {
                    attachedT = gameObjects.GetJobCategory(JobRole.Tank);
                }

                if (!attachedT.Any())
                {
                    attachedT = gameObjects;
                }
            }

            return attachedT.OrderBy(ObjectHelper.GetHealthRatio).FirstOrDefault();
        }

        BattleChara? FindDispelTarget()
        {
            if (gameObjects.Any(o => o.ObjectId == DataCenter.DispelTarget?.ObjectId))
                return DataCenter.DispelTarget;
            return gameObjects.FirstOrDefault(o => o is BattleChara b && b.StatusList.Any(StatusHelper.CanDispel));
        }

        BattleChara? FindTankTarget()
        {
            return RandomPickByJobs(gameObjects, JobRole.Tank);
        }
    }

    internal static BattleChara? RandomPhysicalTarget(IEnumerable<BattleChara> tars)
    {
        return RandomPickByJobs(tars, Job.WAR, Job.GNB, Job.MNK, Job.SAM, Job.DRG, Job.MCH, Job.DNC)
            ?? RandomPickByJobs(tars, Job.PLD, Job.DRK, Job.NIN, Job.BRD, Job.RDM)
            ?? RandomObject(tars);
    }

    internal static BattleChara? RandomMagicalTarget(IEnumerable<BattleChara> tars)
    {
        return RandomPickByJobs(tars, Job.SCH, Job.AST, Job.SGE, Job.BLM, Job.SMN)
            ?? RandomPickByJobs(tars, Job.PLD, Job.DRK, Job.NIN, Job.BRD, Job.RDM)
            ?? RandomObject(tars);
    }

    internal static BattleChara? RandomRangeTarget(IEnumerable<BattleChara> tars)
    {
        return RandomPickByJobs(tars, JobRole.RangedMagical, JobRole.RangedPhysical, JobRole.Melee)
            ?? RandomPickByJobs(tars, JobRole.Tank, JobRole.Healer)
            ?? RandomObject(tars);
    }

    internal static BattleChara? RandomMeleeTarget(IEnumerable<BattleChara> tars)
    {
        return RandomPickByJobs(tars, JobRole.Melee, JobRole.RangedMagical, JobRole.RangedPhysical)
            ?? RandomPickByJobs(tars, JobRole.Tank, JobRole.Healer)
            ?? RandomObject(tars);
    }

    private static BattleChara? RandomPickByJobs(IEnumerable<BattleChara> tars, params JobRole[] roles)
    {
        foreach (var role in roles)
        {
            var tar = RandomPickByJobs(tars, role.ToJobs());
            if (tar != null) return tar;
        }
        return null;
    }

    private static BattleChara? RandomPickByJobs(IEnumerable<BattleChara> tars, params Job[] jobs)
    {
        var targets = tars.Where(t => t.IsJobs(jobs));
        if (targets.Any()) return RandomObject(targets);

        return null;
    }

    private static BattleChara? RandomObject(IEnumerable<BattleChara> objs)
    {
        return objs.FirstOrDefault();
        //Random ran = new(DateTime.Now.Millisecond);
        //var count = objs.Count();
        //if (count == 0) return null;
        //return objs.ElementAt(ran.Next(count));
    }

    #endregion
}

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
public enum TargetType : byte
{
    Big,
    Small,
    HighHP,
    LowHP,
    HighMaxHP,
    LowMaxHP,
    Interrupt,
    Provoke,
    Death,
    Dispel,
    Move,
    BeAttacked,
    Heal,
    Tank,
    Melee,
    Range,
    Physical,
    Magical,
    Self,
}
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

/// <summary>
/// The target result
/// </summary>
/// <param name="Target">the target.</param>
/// <param name="AffectedTargets">the targets that be affected by this action.</param>
/// <param name="Position">the position to use this action.</param>
public readonly record struct TargetResult(BattleChara? Target, BattleChara[] AffectedTargets, Vector3? Position);
