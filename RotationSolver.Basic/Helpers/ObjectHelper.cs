using Dalamud.Game.ClientState.Objects.Enums;
using ECommons.DalamudServices;
using ECommons.GameFunctions;
using ECommons.GameHelpers;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.Event;
using FFXIVClientStructs.FFXIV.Common.Component.BGCollision;
using Lumina.Excel.GeneratedSheets;
using Microsoft.VisualBasic.Logging;
using RotationSolver.Basic.Configuration;
using System.Text.RegularExpressions;

namespace RotationSolver.Basic.Helpers;

/// <summary>
/// Get the information from object.
/// </summary>
public static class ObjectHelper
{
    static readonly EventHandlerType[] _eventType =
    [
        EventHandlerType.TreasureHuntDirector,
        EventHandlerType.Quest,
    ];

    internal static BNpcBase? GetObjectNPC(this GameObject obj)
    {
        if (obj == null) return null;
        return Service.GetSheet<BNpcBase>().GetRow(obj.DataId);
    }

    public static bool CanProvoke(this GameObject target)
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
                && (Vector3.Distance(target.Position, Player.Object.Position) > 5))
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Is the target have positional.
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static bool HasPositional(this GameObject obj)
    {
        if (obj == null) return false;
        return !(obj.GetObjectNPC()?.Unknown10 ?? false);
    }

    /// <summary>
    /// Is this target belongs to other players.
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static unsafe bool IsOthersPlayers(this GameObject obj)
    {
        //SpecialType but no NamePlateIcon
        if (_eventType.Contains(obj.GetEventType()))
        {
            return obj.GetNamePlateIcon() == 0;
        }
        return false;
    }

    /// <summary>
    /// Is this target an enemy (can be attacked).
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static unsafe bool IsEnemy(this GameObject obj)
        => obj != null
        && ActionManager.CanUseActionOnTarget((uint)ActionID.BlizzardPvE, obj.Struct());

    /// <summary>
    /// Is alliance (can be healed).
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static unsafe bool IsAlliance(this GameObject obj)
        => obj != null
        && (ActionManager.CanUseActionOnTarget((uint)ActionID.CurePvE, obj.Struct())
        || ActionManager.CanUseActionOnTarget((uint)ActionID.RaisePvE, obj.Struct()));

    public static bool IsParty(this GameObject gameObject)
    {
        if (gameObject.ObjectId == Player.Object.ObjectId) return true;
        if (Svc.Party.Any(p => p.GameObject?.ObjectId == gameObject.ObjectId)) return true;
        if (gameObject.SubKind == 9) return true;
        return false;
    }

    public static bool IsDeathToRaise(this GameObject obj)
    {
        if (obj == null) return false;
        if (!obj.IsDead) return false;
        if (obj is BattleChara b && b.CurrentHp != 0) return false;

        if (!obj.IsTargetable) return false;

        if (obj.HasStatus(false, StatusID.Raise)) return false;

        if (!Service.Config.GetValue(PluginConfigBool.RaiseBrinkOfDeath) && obj.HasStatus(false, StatusID.BrinkOfDeath)) return false;

        if (DataCenter.AllianceMembers.Any(c => c.CastTargetObjectId == obj.ObjectId)) return false;

        return true;
    }

    public static bool IsAlive(this GameObject obj)
    {
        if (obj is BattleChara b && b.CurrentHp <= 1) return false;

        if (!obj.IsTargetable) return false;

        return true;
    }

    /// <summary>
    /// Get the object kind.
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static unsafe ObjectKind GetObjectKind(this GameObject obj) => (ObjectKind)obj.Struct()->ObjectKind;

    internal static bool IsTopPriorityHostile(this GameObject obj)
    {
        var fateId = DataCenter.FateId;
        //Fate
        if (Service.Config.GetValue(Configuration.PluginConfigBool.TargetFatePriority) && fateId != 0 && obj.FateId() == fateId) return true;

        var icon = obj.GetNamePlateIcon();

        //Hunting log and weapon.
        if (Service.Config.GetValue(Configuration.PluginConfigBool.TargetHuntingRelicLevePriority) && icon
            is 60092 //Hunting
            or 60096 //Weapon
            or 71244 //Leve
            ) return true;

        if (Service.Config.GetValue(Configuration.PluginConfigBool.TargetQuestPriority) && (icon
            is 71204 //Main Quest
            or 71144 //Major Quest
            or 71224 //Other Quest
            or 71344 //Major Quest
           || obj.GetEventType() is EventHandlerType.Quest)) return true;

        if (obj is BattleChara b && b.StatusList.Any(StatusHelper.IsPriority)) return true;

        return false;
    }

    internal static unsafe uint GetNamePlateIcon(this GameObject obj) => obj.Struct()->NamePlateIconId;
    internal static unsafe EventHandlerType GetEventType(this GameObject obj) => obj.Struct()->EventId.Type;

    /// <summary>
    /// The sub kind of the target.
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static unsafe BattleNpcSubKind GetBattleNPCSubKind(this GameObject obj) => (BattleNpcSubKind)obj.Struct()->SubKind;

    internal static unsafe uint FateId(this GameObject obj) => obj.Struct()->FateId;

    static readonly Dictionary<uint, bool> _effectRangeCheck = new();
    internal static bool CanInterrupt(this GameObject o)
    {
        if (o is not BattleChara b) return false;

        var baseCheck = b.IsCasting && b.IsCastInterruptible && b.TotalCastTime >= 2;

        if (!baseCheck) return false;
        if (!Service.Config.GetValue(Configuration.PluginConfigBool.InterruptibleMoreCheck)) return true;

        var id = b.CastActionId;
        if (_effectRangeCheck.TryGetValue(id, out var check)) return check;

        var act = Service.GetSheet<Lumina.Excel.GeneratedSheets.Action>().GetRow(b.CastActionId);
        if (act == null) return _effectRangeCheck[id] = false;
        if (act.CastType is 3 or 4) return _effectRangeCheck[id] = false;
        if (act.EffectRange is > 0 and < 8) return _effectRangeCheck[id] = false;
        return _effectRangeCheck[id] = true;
    }

    /// <summary>
    /// Is object a dummy.
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static bool IsDummy(this BattleChara obj) => obj?.NameId == 541;

    /// <summary>
    /// Is character a boss? Calculate from ttk.
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static bool IsBossFromTTK(this BattleChara obj)
    {
        if (obj == null) return false;

        if (obj.IsDummy() && !Service.Config.GetValue(Configuration.PluginConfigBool.ShowTargetTimeToKill)) return true;

        //Fate
        if (obj.GetTimeToKill(true) >= Service.Config.GetValue(Configuration.PluginConfigFloat.BossTimeToKill)) return true;

        return false;
    }

    /// <summary>
    /// Is character a boss? Calculated from the icon.
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static bool IsBossFromIcon(this BattleChara obj)
    {
        if (obj == null) return false;

        if (obj.IsDummy() && !Service.Config.GetValue(Configuration.PluginConfigBool.ShowTargetTimeToKill)) return true;

        //Icon
        if (obj.GetObjectNPC()?.Rank is 1 or 2 /*or 4*/ or 6) return true;

        return false;
    }

    /// <summary>
    /// Is character a dying? Current HP is below a certain amount. It is for running out of resources.
    /// </summary>
    /// <param name="b"></param>
    /// <returns></returns>
    public static bool IsDying(this BattleChara b)
    {
        if (b == null) return false;
        if (b.IsDummy() && !Service.Config.GetValue(Configuration.PluginConfigBool.ShowTargetTimeToKill)) return false;
        return b.GetTimeToKill() <= Service.Config.GetValue(Configuration.PluginConfigFloat.DyingTimeToKill) || b.GetHealthRatio() < 0.02f;
    }

    /// <summary>
    /// Whether the character is in combat.
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static unsafe bool InCombat(this BattleChara obj)
    {
        return obj.Struct()->Character.InCombat;
    }

    private static readonly TimeSpan CheckSpan = TimeSpan.FromSeconds(2.5);

    /// <summary>
    /// How many seconds will the target die.
    /// </summary>
    /// <param name="b"></param>
    /// <param name="wholeTime">whole time to die.</param>
    /// <returns></returns>
    public static float GetTimeToKill(this BattleChara b, bool wholeTime = false)
    {
        if (b == null) return float.NaN;
        if (b.IsDummy()) return 999.99f;

        var objectId = b.ObjectId;

        DateTime startTime = DateTime.MinValue;
        float thatTimeRatio = 0;
        foreach (var (time, hpRatios) in DataCenter.RecordedHP)
        {
            if (hpRatios.TryGetValue(objectId, out var ratio) && ratio != 1)
            {
                startTime = time;
                thatTimeRatio = ratio;
                break;
            }
        }

        var timespan = DateTime.Now - startTime;
        if (startTime == DateTime.MinValue || timespan < CheckSpan) return float.NaN;

        var ratioNow = b.GetHealthRatio();

        var ratioReduce = thatTimeRatio - ratioNow;
        if (ratioReduce <= 0) return float.NaN;

        return (float)timespan.TotalSeconds / ratioReduce * (wholeTime ? 1 : ratioNow);
    }

    /// <summary>
    /// Whether the target is attacked.
    /// </summary>
    /// <param name="b"></param>
    /// <returns></returns>
    public static bool IsAttacked(this BattleChara b)
    {
        foreach (var (id, time) in DataCenter.AttackedTargets)
        {
            if (id == b.ObjectId)
            {
                return DateTime.Now - time > TimeSpan.FromSeconds(1);
            }
        }
        return false;
    }

    /// <summary>
    /// Can the player see the object.
    /// </summary>
    /// <param name="b"></param>
    /// <returns></returns>
    public static unsafe bool CanSee(this GameObject b)
    {
        var point = Player.Object.Position + Vector3.UnitY * Player.GameObject->Height;
        var tarPt = b.Position + Vector3.UnitY * b.Struct()->Height;
        var direction = tarPt - point;

        int* unknown = stackalloc int[] { 0x4000, 0, 0x4000, 0 };

        RaycastHit hit = default;

        return !FFXIVClientStructs.FFXIV.Client.System.Framework.Framework.Instance()->BGCollisionModule
            ->RaycastEx(&hit, point, direction, direction.Length(), 1, unknown);
    }

    /// <summary>
    /// Get the <paramref name="b"/>'s current HP percentage.
    /// </summary>
    /// <param name="b"></param>
    /// <returns></returns>
    public static float GetHealthRatio(this GameObject g)
    {
        if (g is not BattleChara b) return 0;
        if (DataCenter.RefinedHP.TryGetValue(b.ObjectId, out var hp)) return hp;
        return (float)b.CurrentHp / b.MaxHp;
    }

    internal static EnemyPositional FindEnemyPositional(this GameObject enemy)
    {
        Vector3 pPosition = enemy.Position;
        Vector2 faceVec = enemy.GetFaceVector();

        Vector3 dir = Player.Object.Position - pPosition;
        Vector2 dirVec = new(dir.Z, dir.X);

        double angle = faceVec.AngleTo(dirVec);

        if (angle < Math.PI / 4) return EnemyPositional.Front;
        else if (angle > Math.PI * 3 / 4) return EnemyPositional.Rear;
        return EnemyPositional.Flank;
    }

    /// <summary>
    /// Get the face vector
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static Vector2 GetFaceVector(this GameObject obj)
    {
        float rotation = obj.Rotation;
        return new((float)Math.Cos(rotation), (float)Math.Sin(rotation));
    }

    /// <summary>
    /// Get two vector's angle
    /// </summary>
    /// <param name="vec1"></param>
    /// <param name="vec2"></param>
    /// <returns></returns>
    public static double AngleTo(this Vector2 vec1, Vector2 vec2)
    {
        return Math.Acos(Vector2.Dot(vec1, vec2) / vec1.Length() / vec2.Length());
    }

    /// <summary>
    /// The distance from <paramref name="obj"/> to the player
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static float DistanceToPlayer(this GameObject obj)
    {
        if (obj == null) return float.MaxValue;
        var player = Player.Object;
        if (player == null) return float.MaxValue;

        var distance = Vector3.Distance(player.Position, obj.Position) - player.HitboxRadius;
        distance -= obj.HitboxRadius;
        return distance;
    }
}
