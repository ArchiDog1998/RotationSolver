using Dalamud.Game.ClientState.Objects.Types;
using Lumina.Excel.GeneratedSheets;
using RotationSolver.Basic.Data;
using System.Data;
using System.Numerics;

namespace RotationSolver.Basic.Helpers;

public static class TargetFilter
{
    #region Find one target
    internal static IEnumerable<BattleChara> MeleeRangeTargetFilter(IEnumerable<BattleChara> availableCharas)
        => availableCharas.Where(t => t.DistanceToPlayer() >= 3 + Service.Config.MeleeRangeOffset);
    internal static BattleChara DefaultChooseFriend(IEnumerable<BattleChara> availableCharas, bool mustUse)
    {
        if (availableCharas == null || !availableCharas.Any()) return null;

        //根据默认设置排序怪且没有大招
        availableCharas = DefaultTargetingType(availableCharas).Where(StatusHelper.NeedHealing);

        var tar = availableCharas.OrderBy(ObjectHelper.GetHealingRatio).FirstOrDefault();

        if (tar.GetHealingRatio() < 1) return tar;

        availableCharas = availableCharas.GetJobCategory(JobRole.Tank);

        return availableCharas.FirstOrDefault(t => t.HasStatus(false, StatusHelper.TankStanceStatus))
           ?? availableCharas.FirstOrDefault();
    }

    internal static BattleChara DefaultFindHostile(IEnumerable<BattleChara> availableCharas, bool mustUse)
    {
        if (availableCharas == null || !availableCharas.Any()) return null;

        //找到被标记攻击的怪
        var b = MarkingHelper.GetAttackMarkChara(availableCharas);
        if (Service.Config.ChooseAttackMark && b != null) return b;

        //去掉停止标记的怪
        if (Service.Config.FilterStopMark)
        {
            var charas = MarkingHelper.FilterStopCharaes(availableCharas);
            if (charas?.Any() ?? false) availableCharas = charas;
        }

        b = availableCharas.FirstOrDefault(ObjectHelper.IsTopPriorityHostile);
        if (b != null) return b;

        if (DataCenter.TreasureCharas.Length > 0)
        {
            b = availableCharas.FirstOrDefault(b => b.ObjectId == DataCenter.TreasureCharas[0]);
            if (b != null) return b;
            availableCharas = availableCharas.Where(b => !DataCenter.TreasureCharas.Contains(b.ObjectId));
        }

        //根据默认设置排序怪
        availableCharas = DefaultTargetingType(availableCharas);

        //找到体积一样小的
        float radius = availableCharas.FirstOrDefault().HitboxRadius;

        return availableCharas.Where(c => c.HitboxRadius == radius)
            .OrderBy(ObjectHelper.DistanceToPlayer).FirstOrDefault();
    }

    internal static T FindTargetForMoving<T>(this IEnumerable<T> charas, bool mustUse) where T : GameObject
    {
        if (mustUse)
        {
            var tar = charas.OrderBy(ObjectHelper.DistanceToPlayer).FirstOrDefault();
            if (tar == null) return null;
            if (tar.DistanceToPlayer() < 1) return tar;
            return null;
        }

        if (Service.Config.MoveTowardsScreenCenter)
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
        Vector3 pPosition = Service.Player.Position;
        float rotation = Service.Player.Rotation;
        Vector2 faceVec = new Vector2((float)Math.Cos(rotation), (float)Math.Sin(rotation));

        var tars = charas.Where(t =>
        {
            if (t.DistanceToPlayer() < DISTANCE_TO_MOVE) return false;

            Vector3 dir = t.Position - pPosition;
            Vector2 dirVec = new Vector2(dir.Z, dir.X);
            double angle = Math.Acos(Vector2.Dot(dirVec, faceVec) / dirVec.Length() / faceVec.Length());
            return angle <= Math.PI * Service.Config.MoveTargetAngle / 360;
        }).OrderByDescending(ObjectHelper.DistanceToPlayer);

        return tars.FirstOrDefault();
    }

    private static T FindMoveTargetScreenCenter<T>(IEnumerable<T> charas) where T : GameObject
    {
        var pPosition = Service.Player.Position;
        if (!Service.GameGui.WorldToScreen(pPosition, out var playerScrPos)) return null;

        var tars = charas.Where(t =>
        {
            if (t.DistanceToPlayer() < DISTANCE_TO_MOVE) return false;

            if (!Service.GameGui.WorldToScreen(t.Position, out var scrPos)) return false;

            var dir = scrPos - playerScrPos;

            if (dir.Y > 0) return false;

            return Math.Abs(dir.X / dir.Y) < Math.Tan(Math.PI * Service.Config.MoveTargetAngle / 360);
        }).OrderByDescending(ObjectHelper.DistanceToPlayer);

        return tars.FirstOrDefault();
    }

    /// <summary>
    /// 发现被攻击的目标
    /// </summary>
    /// <param name="charas"></param>
    /// <returns></returns>
    public static BattleChara FindAttackedTarget(IEnumerable<BattleChara> charas, bool mustUse)
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
    /// 挑衅目标
    /// </summary>
    /// <param name="inputCharas"></param>
    /// <param name="needDistance"></param>
    /// <returns></returns>
    internal static IEnumerable<BattleChara> ProvokeTarget(IEnumerable<BattleChara> inputCharas, bool needDistance = false)
    {
        var tankIDS = DataCenter.AllianceMembers.GetJobCategory(JobRole.Tank).Select(member => (ulong)member.ObjectId);
        var loc = Service.Player.Position;
        var id = Service.Player.ObjectId;

        var targets = inputCharas.Where(target =>
        {
            //有目标
            if (target.TargetObject?.IsValid() ?? false)
            {
                //居然在打非T！
                if (!tankIDS.Contains(target.TargetObjectId) && (!needDistance || Vector3.Distance(target.Position, loc) > 5))
                {
                    return true;
                }
            }
            return false;
        });

        //没有敌对势力，那随便用
        if (!targets.Any()) return inputCharas;
        //返回在打队友的讨厌鬼！
        return targets;
    }

    /// <summary>
    /// 获得死亡的角色
    /// </summary>
    /// <param name="deathAll"></param>
    /// <param name="deathParty"></param>
    /// <returns></returns>
    internal static BattleChara GetDeathPeople(IEnumerable<BattleChara> deathAll, IEnumerable<BattleChara> deathParty)
    {
        if (deathParty.Any())
        {
            //确认一下死了的T有哪些。

            var deathT = deathParty.GetJobCategory(JobRole.Tank);
            int TCount = DataCenter.PartyTanks.Count();

            //如果全死了，赶紧复活啊。
            if (TCount > 0 && deathT.Count() == TCount)
            {
                return deathT.FirstOrDefault();
            }

            //确认一下死了的H有哪些。
            var deathH = deathParty.GetJobCategory(JobRole.Healer);

            //如果H死了，就先救他。
            if (deathH.Any()) return deathH.FirstOrDefault();

            //如果T死了，就再救他。
            if (deathT.Any()) return deathT.FirstOrDefault();

            //T和H都还活着，那就随便救一个。
            return deathParty.FirstOrDefault();
        }

        if (deathAll.Any())
        {
            //确认一下死了的H有哪些。
            var deathAllH = deathAll.GetJobCategory(JobRole.Healer);
            if (deathAllH.Any()) return deathAllH.FirstOrDefault();

            //确认一下死了的T有哪些。
            var deathAllT = deathAll.GetJobCategory(JobRole.Tank);
            if (deathAllT.Any()) return deathAllT.FirstOrDefault();

            return deathAll.FirstOrDefault();
        }

        return null;
    }

    public unsafe static IEnumerable<BattleChara> GetDeath(this IEnumerable<BattleChara> charas) => charas.Where(item =>
        {
            if (!item.IsDead) return false;
            if (item.CurrentHp != 0) return false;

            if (!item.IsTargetable()) return false;

            //如果已经有复活的Buff了，那就算了。
            if (item.HasStatus(false, StatusID.Raise)) return false;

            //如果濒死了，那给我TMD冷静冷静！等着另一个奶大发慈悲吧。
            if (!Service.Config.RaiseBrinkOfDeath && item.HasStatus(false, StatusID.BrinkofDeath)) return false;

            //如果有人在对着他咏唱，那就算了。
            if (DataCenter.AllianceMembers.Any(c => c.CastTargetObjectId == item.ObjectId)) return false;

            return true;
        });

    public static IEnumerable<BattleChara> GetJobCategory(this IEnumerable<BattleChara> objects, params JobRole[] roles)
    {
        return roles.SelectMany(role =>
        {
            return objects.Where(obj =>
            {
                return obj.IsJobCategory(role);
            });
        });
    }

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

    internal static BattleChara ASTRangeTarget(IEnumerable<BattleChara> ASTTargets, bool mustUse)
    {
        ASTTargets = ASTTargets.Where(b => !b.HasStatus(false, StatusID.Weakness, StatusID.BrinkofDeath));

        return ASTTargets.ASTGetTargetByJobs(JobRole.RangedMagical, JobRole.RangedPhysical, JobRole.Melee);
    }

    internal static BattleChara ASTMeleeTarget(IEnumerable<BattleChara> ASTTargets, bool mustUse)
    {
        ASTTargets = ASTTargets.Where(b => !b.HasStatus(false, StatusID.Weakness, StatusID.BrinkofDeath));


        return ASTTargets.ASTGetTargetByJobs(JobRole.Melee, JobRole.RangedMagical, JobRole.RangedPhysical);
    }

    private static BattleChara ASTGetTargetByJobs(this IEnumerable<BattleChara> tars, params JobRole[] roles)
    {
        foreach (var role in roles)
        {
            var targets = GetASTCardTargets(tars.GetJobCategory(role));
            if (targets.Count() > 0) return RandomObject(targets);
        }
        var ts = GetASTCardTargets(tars);
        if (ts.Count() > 0) return RandomObject(ts);

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
        Random ran = new Random(DateTime.Now.Millisecond);
        return objs.ElementAt(ran.Next(objs.Count()));
    }

    #endregion

    /// <summary>
    /// 获得范围<paramref name="radius"/>内对象<paramref name="objects"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="objects"></param>
    /// <param name="radius"></param>
    /// <returns></returns>
    public static IEnumerable<T> GetObjectInRadius<T>(this IEnumerable<T> objects, float radius) where T : GameObject
    {
        return objects.Where(o => o.DistanceToPlayer() <= radius);
    }

    private static IEnumerable<BattleChara> DefaultTargetingType(IEnumerable<BattleChara> charas)
    {
        switch (DataCenter.TargetingType)
        {
            default:
            case TargetingType.Big:
                return charas.OrderByDescending(p => p.HitboxRadius);

            case TargetingType.Small:
                return charas.OrderBy(p => p.HitboxRadius);

            case TargetingType.HighHP:
                return charas.OrderByDescending(p => p.CurrentHp);

            case TargetingType.LowHP:
                return charas.OrderBy(p => p.CurrentHp);

            case TargetingType.HighMaxHP:
                return charas.OrderByDescending(p => p.MaxHp);

            case TargetingType.LowMaxHP:
                return charas.OrderBy(p => p.MaxHp);
        }
    }
}
