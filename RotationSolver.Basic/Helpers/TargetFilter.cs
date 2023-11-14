using ECommons.DalamudServices;
using ECommons.GameHelpers;
using Lumina.Excel.GeneratedSheets;
using RotationSolver.Basic.Configuration;
using System.Data;
using System.Text.RegularExpressions;

namespace RotationSolver.Basic.Helpers;

/// <summary>
/// The filter for target.
/// </summary>
public static class TargetFilter
{
    #region Find one target
    internal static IEnumerable<BattleChara> MeleeRangeTargetFilter(IEnumerable<BattleChara> availableCharas)
        => availableCharas.Where(t => t.DistanceToPlayer() >= 3 + Service.Config.GetValue(PluginConfigFloat.MeleeRangeOffset));

    internal static BattleChara DefaultChooseFriend(IEnumerable<BattleChara> availableCharas, bool _)
    {
        if (availableCharas == null || !availableCharas.Any()) return null;

        var player = Svc.ClientState.LocalPlayer;
        var onlyHealSelf = Service.Config.GetValue(PluginConfigBool.OnlyHealSelfWhenNoHealer) && player?.ClassJob.GameData?.GetJobRole() != JobRole.Healer;

        if (onlyHealSelf)
        {
            if (player == null) return null;
            return player;
        }

        availableCharas = availableCharas.Where(StatusHelper.NeedHealing);

        var healerTars = availableCharas.GetJobCategory(JobRole.Healer);
        var tankTars = availableCharas.GetJobCategory(JobRole.Tank);

        var healerTar = tankTars.OrderBy(ObjectHelper.GetHealthRatio).FirstOrDefault();
        if (healerTar != null && healerTar.GetHealthRatio() < Service.Config.GetValue(PluginConfigFloat.HealthHealerRatio))
            return healerTar;

        var tankTar = tankTars.OrderBy(ObjectHelper.GetHealthRatio).FirstOrDefault();
        if (tankTar != null && tankTar.GetHealthRatio() < Service.Config.GetValue(PluginConfigFloat.HealthTankRatio))
            return tankTar;

        var tar = availableCharas.OrderBy(ObjectHelper.GetHealthRatio).FirstOrDefault();
        if (tar.GetHealthRatio() < 1) return tar;

        return tankTars.FirstOrDefault(t => t.HasStatus(false, StatusHelper.TankStanceStatus))
           ?? tankTars.FirstOrDefault();
    }

    internal static BattleChara DefaultFindHostile(IEnumerable<BattleChara> availableCharas, bool _)
    {
        if (availableCharas == null || !availableCharas.Any()) return null;

        if (Service.Config.GetValue(PluginConfigBool.FilterStopMark))
        {
            var charas = MarkingHelper.FilterStopCharaes(availableCharas);
            if (charas?.Any() ?? false) availableCharas = charas;
        }

        if (DataCenter.TreasureCharas.Length > 0)
        {
            var b = availableCharas.FirstOrDefault(b => b.ObjectId == DataCenter.TreasureCharas[0]);
            if (b != null) return b;
            availableCharas = availableCharas.Where(b => !DataCenter.TreasureCharas.Contains(b.ObjectId));
        }

        var highPriority = availableCharas.Where(ObjectHelper.IsTopPriorityHostile);
        if (highPriority.Any())
        {
            availableCharas = highPriority;
        }

        availableCharas = DefaultTargetingType(availableCharas);


        return availableCharas.FirstOrDefault();
    }

    internal static T FindTargetForMoving<T>(this IEnumerable<T> charas, bool mustUse) where T : GameObject
    {
        if (mustUse)
        {
            var tar = charas.OrderBy(ObjectHelper.DistanceToPlayer).FirstOrDefault();
            if (tar == null) return null;
            if (tar.DistanceToPlayer() < Service.Config.GetValue(Configuration.PluginConfigFloat.DistanceForMoving)) return tar;
            return null;
        }

        if (Service.Config.GetValue(Configuration.PluginConfigBool.MoveTowardsScreenCenter))
        {
            return FindMoveTargetScreenCenter(charas);
        }
        else
        {
            return FindMoveTargetFaceDirection(charas);
        }
    }

    const float DISTANCE_TO_MOVE = 3;
    private static T FindMoveTargetFaceDirection<T>(IEnumerable<T> charas) where T : GameObject
    {
        Vector3 pPosition = Player.Object.Position;
        Vector2 faceVec = Player.Object.GetFaceVector();

        var tars = charas.Where(t =>
        {
            if (t.DistanceToPlayer() < DISTANCE_TO_MOVE) return false;

            Vector3 dir = t.Position - pPosition;
            Vector2 dirVec = new(dir.Z, dir.X);
            double angle = faceVec.AngleTo(dirVec);
            return angle <= Math.PI * Service.Config.GetValue(Configuration.PluginConfigFloat.MoveTargetAngle) / 360;
        }).OrderByDescending(ObjectHelper.DistanceToPlayer);

        return tars.FirstOrDefault();
    }

    private static T FindMoveTargetScreenCenter<T>(IEnumerable<T> charas) where T : GameObject
    {
        var pPosition = Player.Object.Position;
        if (!Svc.GameGui.WorldToScreen(pPosition, out var playerScrPos)) return null;

        var tars = charas.Where(t =>
        {
            if (t.DistanceToPlayer() < DISTANCE_TO_MOVE) return false;

            if (!Svc.GameGui.WorldToScreen(t.Position, out var scrPos)) return false;

            var dir = scrPos - playerScrPos;

            if (dir.Y > 0) return false;

            return Math.Abs(dir.X / dir.Y) < Math.Tan(Math.PI * Service.Config.GetValue(Configuration.PluginConfigFloat.MoveTargetAngle) / 360);
        }).OrderByDescending(ObjectHelper.DistanceToPlayer);

        return tars.FirstOrDefault();
    }

    /// <summary>
    /// Find the one being attacked.
    /// </summary>
    /// <param name="charas"></param>
    /// <param name="_"></param>
    /// <returns></returns>
    public static BattleChara FindAttackedTarget(IEnumerable<BattleChara> charas, bool _)
    {
        if (!charas.Any()) return null;
        var attachedT = charas.Where(tank => tank.TargetObject?.TargetObject == tank);

        if (!attachedT.Any())
        {
            attachedT = charas.Where(tank => tank.HasStatus(false, StatusHelper.TankStanceStatus));
        }

        if (!attachedT.Any())
        {
            attachedT = charas.GetJobCategory(JobRole.Tank);
        }

        if (!attachedT.Any())
        {
            attachedT = charas;
        }

        return attachedT.OrderBy(ObjectHelper.GetHealthRatio).FirstOrDefault();
    }

    internal static IEnumerable<BattleChara> TankRangeTarget(IEnumerable<BattleChara> inputCharas)
        => ProvokeTarget(MeleeRangeTargetFilter(inputCharas));

    /// <summary>
    /// The target about to be provoked.
    /// </summary>
    /// <param name="inputCharas"></param>
    /// <param name="needDistance"></param>
    /// <returns></returns>
    internal static IEnumerable<BattleChara> ProvokeTarget(IEnumerable<BattleChara> inputCharas, bool needDistance = false)
    {
        var loc = Player.Object.Position;

        var targets = inputCharas.Where(target =>
        {
            //Removed the listed names.
            IEnumerable<string> names = Array.Empty<string>();
            if (OtherConfiguration.NoProvokeNames.TryGetValue(Svc.ClientState.TerritoryType, out var ns1))
                names = names.Union(ns1);

            if (names.Any(n => !string.IsNullOrEmpty(n) && new Regex(n).Match(target.Name.ToString()).Success)) return false;

            //Target can move or two big and has a target
            if ((target.GetObjectNPC()?.Unknown12 == 0 || target.HitboxRadius >= 5)
            && (target.TargetObject?.IsValid() ?? false))
            {
                //the target is not a tank role
                if (Svc.Objects.SearchById(target.TargetObjectId) is BattleChara battle
                    && !battle.IsJobCategory(JobRole.Tank)
                    && (!needDistance || Vector3.Distance(target.Position, loc) > 5))
                {
                    return true;
                }
            }
            return false;
        });

        if (!targets.Any()) return inputCharas;
        return targets;
    }

    /// <summary>
    /// Get the deadth members.
    /// </summary>
    /// <param name="deathAll"></param>
    /// <param name="deathParty"></param>
    /// <returns></returns>
    internal static BattleChara GetDeathPeople(IEnumerable<BattleChara> deathAll, IEnumerable<BattleChara> deathParty)
    {
        if (deathParty.Any())
        {
            var deathT = deathParty.GetJobCategory(JobRole.Tank);
            int TCount = DataCenter.PartyTanks.Count();

            if (TCount > 0 && deathT.Count() == TCount)
            {
                return deathT.FirstOrDefault();
            }

            var deathH = deathParty.GetJobCategory(JobRole.Healer);

            if (deathH.Any()) return deathH.FirstOrDefault();

            if (deathT.Any()) return deathT.FirstOrDefault();

            return deathParty.FirstOrDefault();
        }

        if (deathAll.Any())
        {
            var deathAllH = deathAll.GetJobCategory(JobRole.Healer);
            if (deathAllH.Any()) return deathAllH.FirstOrDefault();

            var deathAllT = deathAll.GetJobCategory(JobRole.Tank);
            if (deathAllT.Any()) return deathAllT.FirstOrDefault();

            return deathAll.FirstOrDefault();
        }

        return null;
    }

    /// <summary>
    /// Get the deadth ones in the list.
    /// </summary>
    /// <param name="charas"></param>
    /// <returns></returns>
    public unsafe static IEnumerable<BattleChara> GetDeath(this IEnumerable<BattleChara> charas) => charas.Where(item =>
        {
            if (item == null) return false;
            if (!item.IsDead) return false;
            if (item.CurrentHp != 0) return false;

            if (!item.IsTargetable) return false;

            if (item.HasStatus(false, StatusID.Raise)) return false;

            if (!Service.Config.GetValue(PluginConfigBool.RaiseBrinkOfDeath) && item.HasStatus(false, StatusID.BrinkOfDeath)) return false;

            if (DataCenter.AllianceMembers.Any(c => c.CastTargetObjectId == item.ObjectId)) return false;

            return true;
        });

    /// <summary>
    /// Get the specific roles members.
    /// </summary>
    /// <param name="objects"></param>
    /// <param name="roles"></param>
    /// <returns></returns>
    public static IEnumerable<BattleChara> GetJobCategory(this IEnumerable<BattleChara> objects, params JobRole[] roles)
        => roles.SelectMany(role => objects.Where(obj => obj.IsJobCategory(role)));

    /// <summary>
    /// Is the target the role.
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="role"></param>
    /// <returns></returns>
    public static bool IsJobCategory(this BattleChara obj, JobRole role)
    {
        SortedSet<byte> validJobs = new(Service.GetSheet<ClassJob>()
            .Where(job => role == job.GetJobRole())
            .Select(job => (byte)job.RowId));

        return obj.IsJobCategory(validJobs);
    }


    private static bool IsJobCategory(this BattleChara obj, SortedSet<byte> validJobs)
    {
        return validJobs.Contains((byte)obj.ClassJob.GameData?.RowId);
    }

    internal static BattleChara ASTRangeTarget(IEnumerable<BattleChara> ASTTargets, bool _)
    {
        ASTTargets = ASTTargets.Where(b => !b.HasStatus(false, StatusID.Weakness, StatusID.BrinkOfDeath));

        return ASTTargets.ASTGetTargetByJobs(JobRole.RangedMagical, JobRole.RangedPhysical, JobRole.Melee);
    }

    internal static BattleChara ASTMeleeTarget(IEnumerable<BattleChara> ASTTargets, bool _)
    {
        ASTTargets = ASTTargets.Where(b => !b.HasStatus(false, StatusID.Weakness, StatusID.BrinkOfDeath));


        return ASTTargets.ASTGetTargetByJobs(JobRole.Melee, JobRole.RangedMagical, JobRole.RangedPhysical);
    }

    private static BattleChara ASTGetTargetByJobs(this IEnumerable<BattleChara> tars, params JobRole[] roles)
    {
        foreach (var role in roles)
        {
            var targets = GetASTCardTargets(tars.GetJobCategory(role));
            if (targets.Any()) return RandomObject(targets);
        }
        var ts = GetASTCardTargets(tars);
        if (ts.Any()) return RandomObject(ts);

        return null;
    }

    private static IEnumerable<BattleChara> GetASTCardTargets(IEnumerable<BattleChara> sources)
    {
        var allStatus = new StatusID[]
        {
        StatusID.TheArrow,
        StatusID.TheBalance,
        StatusID.TheBole,
        StatusID.TheEwer,
        StatusID.TheSpear,
        StatusID.TheSpire,
        };
        return sources.Where((t) => !t.HasStatus(true, allStatus));
    }

    private static BattleChara RandomObject(IEnumerable<BattleChara> objs)
    {
        Random ran = new(DateTime.Now.Millisecond);
        return objs.ElementAt(ran.Next(objs.Count()));
    }

    #endregion

    /// <summary>
    /// Get the <paramref name="objects"/> in <paramref name="radius"/>.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="objects"></param>
    /// <param name="radius"></param>
    /// <returns></returns>
    public static IEnumerable<T> GetObjectInRadius<T>(this IEnumerable<T> objects, float radius) where T : GameObject
        => objects.Where(o => o.DistanceToPlayer() <= radius);

    private static IEnumerable<BattleChara> DefaultTargetingType(IEnumerable<BattleChara> charas)
    {
        if (DataCenter.Territory?.IsPvpZone ?? false)
        {
            return charas.OrderBy(p => p.CurrentHp);
        }
        return DataCenter.TargetingType switch
        {
            TargetingType.Small => charas.OrderBy(p => p.HitboxRadius),
            TargetingType.HighHP => charas.OrderByDescending(p => p.CurrentHp),
            TargetingType.LowHP => charas.OrderBy(p => p.CurrentHp),
            TargetingType.HighMaxHP => charas.OrderByDescending(p => p.MaxHp),
            TargetingType.LowMaxHP => charas.OrderBy(p => p.MaxHp),
            _ => charas.OrderByDescending(p => p.HitboxRadius),
        };
    }
}
