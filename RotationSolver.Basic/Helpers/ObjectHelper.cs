using Dalamud.Game.ClientState.Objects.Enums;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Game.ClientState.Statuses;
using ECommons.DalamudServices;
using ECommons.GameFunctions;
using ECommons.GameHelpers;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.Event;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Common.Component.BGCollision;
using Lumina.Excel.GeneratedSheets;
using RotationSolver.Basic.Configuration;
using System.Security.Cryptography;
using System.Text;
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

    internal static BNpcBase? GetObjectNPC(this IGameObject obj)
    {
        if (obj == null) return null;
        return Service.GetSheet<BNpcBase>().GetRow(obj.DataId);
    }

    internal static bool CanProvoke(this IGameObject target)
    {
        //Removed the listed names.
        IEnumerable<string> names = [];
        if (OtherConfiguration.NoProvokeNames.TryGetValue(Svc.ClientState.TerritoryType, out var ns1))
            names = names.Union(ns1);

        if (names.Any(n => !string.IsNullOrEmpty(n) && new Regex(n).Match(target.Name.ToString()).Success)) return false;

        //Target can move or two big and has a target
        if ((target.GetObjectNPC()?.Unknown12 == 0 || target.HitboxRadius >= 5)
        && (target.TargetObject?.IsValid() ?? false))
        {
            //the target is not a tank role
            if (Svc.Objects.SearchById(target.TargetObjectId) is IBattleChara battle
                && !battle.IsJobCategory(JobRole.Tank)
                && (Vector3.Distance(target.Position, Player.Object.Position) > 5))
            {
                return true;
            }
        }
        return false;
    }

    internal static bool HasPositional(this IGameObject obj)
    {
        if (obj == null) return false;
        return !(obj.GetObjectNPC()?.Unknown10 ?? false);
    }

    internal static unsafe bool IsOthersPlayers(this IGameObject obj)
    {
        //SpecialType but no NamePlateIcon
        if (_eventType.Contains(obj.GetEventType()))
        {
            return obj.GetNamePlateIcon() == 0;
        }
        return false;
    }

    internal static bool IsAttackable(this IBattleChara IBattleChara)
    {
        //Dead.
        if (IBattleChara.CurrentHp <= 1) return false;

        if (IBattleChara.StatusList.Any(StatusHelper.IsInvincible)) return false;

        if (Svc.ClientState == null) return false;

        //In No Hostiles Names
        IEnumerable<string> names = [];
        if (OtherConfiguration.NoHostileNames.TryGetValue(Svc.ClientState.TerritoryType, out var ns1))
            names = names.Union(ns1);

        if (names.Any(n => !string.IsNullOrEmpty(n) && new Regex(n).Match(IBattleChara.Name.TextValue).Success)) return false;
        
        //Fate
        if (DataCenter.TerritoryContentType != TerritoryContentType.Eureka)
        {
            var tarFateId = IBattleChara.FateId();
            if (tarFateId != 0 && tarFateId != DataCenter.FateId) return false;
        }

        if (Service.Config.AddEnemyListToHostile)
        {
            if (IBattleChara.IsInEnemiesList()) return true;
            //Only attack
            if (Service.Config.OnlyAttackInEnemyList) return false;
        }

        //Tar on me
        if (IBattleChara.TargetObject == Player.Object
        || IBattleChara.TargetObject?.OwnerId == Player.Object.GameObjectId) return true;

        //Remove other's treasure.
        if (IBattleChara.IsOthersPlayers()) return false;

        if (IBattleChara.IsTopPriorityHostile()) return true;

        if (Service.CountDownTime > 0 || DataCenter.IsPvP) return true;

        return DataCenter.RightNowTargetToHostileType switch
        {
            TargetHostileType.AllTargetsCanAttack => true,
            TargetHostileType.TargetsHaveTarget => IBattleChara.TargetObject is IBattleChara,
            TargetHostileType.AllTargetsWhenSolo => DataCenter.PartyMembers.Length < 2 
                || IBattleChara.TargetObject is IBattleChara,
            _ => true,
        };
    }


    internal static string EncryptString(this IPlayerCharacter player)
    {
        if (player == null) return string.Empty;

        try
        {
            byte[] inputByteArray = Encoding.UTF8.GetBytes(player.HomeWorld.GameData!.InternalName.ToString()
    + " - " + player.Name.ToString() + "U6Wy.zCG");

            var tmpHash = MD5.HashData(inputByteArray);
            var retB = Convert.ToBase64String(tmpHash);
            return retB;
        }
        catch (Exception ex)
        {
            Svc.Log.Warning(ex, "Failed to read the player's name and world.");
            return string.Empty;
        }
    }

    internal static unsafe bool IsInEnemiesList(this IBattleChara IBattleChara)
    {
        var addons = Service.GetAddons<AddonEnemyList>();

        if (!addons.Any()) return false;
        var addon = addons.FirstOrDefault();
        var enemy = (AddonEnemyList*)addon;

        var numArray = FFXIVClientStructs.FFXIV.Client.System.Framework.Framework.Instance()->GetUIModule()->GetRaptureAtkModule()->AtkModule.AtkArrayDataHolder.NumberArrays[19];
        List<uint> list = new(enemy->EnemyCount);
        for (var i = 0; i < enemy->EnemyCount; i++)
        {
            var id = (uint)numArray->IntArray[8 + i * 6];

            if (IBattleChara.GameObjectId == id) return true;
        }
        return false;
    }

    internal static unsafe bool IsEnemy(this IGameObject obj)
    => obj != null
    && ActionManager.CanUseActionOnTarget((uint)ActionID.BlizzardPvE, obj.Struct());

    internal static unsafe bool IsAlliance(this IGameObject obj)
        => obj.GameObjectId is not 0
        && (!(DataCenter.IsPvP) && obj is IPlayerCharacter 
        || ActionManager.CanUseActionOnTarget((uint)ActionID.CurePvE, obj.Struct()));

    internal static bool IsParty(this IGameObject IGameObject)
    {
        if (IGameObject.GameObjectId == Player.Object.GameObjectId) return true;
        if (Svc.Party.Any(p => p.GameObject?.GameObjectId == IGameObject.GameObjectId)) return true;
        if (IGameObject.SubKind == 9) return true;
        return false;
    }

    internal static bool IsTargetOnSelf(this IBattleChara IBattleChara)
    {
        return IBattleChara.TargetObject?.TargetObject == IBattleChara;
    }

    internal static bool IsDeathToRaise(this IGameObject obj)
    {
        if (obj == null) return false;
        if (!obj.IsDead) return false;
        if (obj is IBattleChara b && b.CurrentHp != 0) return false;

        if (!obj.IsTargetable) return false;

        if (obj.HasStatus(false, StatusID.Raise)) return false;

        if (!Service.Config.RaiseBrinkOfDeath && obj.HasStatus(false, StatusID.BrinkOfDeath)) return false;

        if (DataCenter.AllianceMembers.Any(c => c.CastTargetObjectId == obj.GameObjectId)) return false;

        return true;
    }

    internal static bool IsAlive(this IGameObject obj)
    {
        if (obj is IBattleChara b && b.CurrentHp <= 1) return false;
        if (!obj.IsTargetable) return false;
        return true;
    }

    /// <summary>
    /// Get the object kind.
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static unsafe ObjectKind GetObjectKind(this IGameObject obj) => (ObjectKind)obj.Struct()->ObjectKind;

    internal static bool IsTopPriorityHostile(this IGameObject obj)
    {
        var fateId = DataCenter.FateId;
        //Fate
        if (Service.Config.TargetFatePriority && fateId != 0 && obj.FateId() == fateId) return true;

        var icon = obj.GetNamePlateIcon();

        //Hunting log and weapon.
        if (Service.Config.TargetHuntingRelicLevePriority && icon
            is 60092 //Hunting
            or 60096 //Weapon
            or 71244 //Leve
            ) return true;

        if (Service.Config.TargetQuestPriority && (icon
            is 71204 //Main Quest
            or 71144 //Major Quest
            or 71224 //Other Quest
            or 71344 //Major Quest
           || obj.GetEventType() is EventHandlerType.Quest)) return true;

        if (obj is IBattleChara b && b.StatusList != null && b.StatusList.Any(StatusHelper.IsPriority)) return true;

        if (Service.Config.ChooseAttackMark && MarkingHelper.AttackSignTargets.FirstOrDefault(id => id != 0) == (long)obj.GameObjectId) return true;


        var npc = obj as IBattleChara;
        if (npc != null && DataCenter.PrioritizedNameIds.Contains(npc.NameId)) return true;

        return false;
    }

    internal static unsafe uint GetNamePlateIcon(this IGameObject obj) => obj.Struct()->NamePlateIconId;
    internal static unsafe EventHandlerType GetEventType(this IGameObject obj) => obj.Struct()->EventId.ContentId;

    internal static unsafe BattleNpcSubKind GetBattleNPCSubKind(this IGameObject obj) => (BattleNpcSubKind)obj.Struct()->SubKind;

    internal static unsafe uint FateId(this IGameObject obj) => obj.Struct()->FateId;

    static readonly Dictionary<uint, bool> _effectRangeCheck = [];
    internal static bool CanInterrupt(this IGameObject o)
    {
        if (o is not IBattleChara b) return false;

        var baseCheck = b.IsCasting && b.IsCastInterruptible && b.TotalCastTime >= 2;

        if (!baseCheck) return false;
        if (!Service.Config.InterruptibleMoreCheck) return false;

        var id = b.CastActionId;
        if (_effectRangeCheck.TryGetValue(id, out var check)) return check;

        var act = Service.GetSheet<Lumina.Excel.GeneratedSheets.Action>().GetRow(b.CastActionId);
        if (act == null) return _effectRangeCheck[id] = false;
        if (act.CastType is 3 or 4) return _effectRangeCheck[id] = false;
        if (act.EffectRange is > 0 and < 8) return _effectRangeCheck[id] = false;
        return _effectRangeCheck[id] = true;
    }

    internal static bool IsDummy(this IBattleChara obj) => obj?.NameId == 541;

    /// <summary>
    /// Is target a boss depends on the ttk.
    /// </summary>
    /// <param name="obj">the object.</param>
    /// <returns></returns>
    public static bool IsBossFromTTK(this IBattleChara obj)
    {
        if (obj == null) return false;

        if (obj.IsDummy()) return true;

        //Fate
        if (obj.GetTimeToKill(true) >= Service.Config.BossTimeToKill) return true;

        return false;
    }
    /// <summary>
    /// Is target a boss depends on the icon.
    /// </summary>
    /// <param name="obj">the object.</param>
    /// <returns></returns>
    public static bool IsBossFromIcon(this IBattleChara obj)
    {
        if (obj == null) return false;

        if (obj.IsDummy()) return true;

        //Icon
        if (obj.GetObjectNPC()?.Rank is 1 or 2 /*or 4*/ or 6) return true;

        return false;
    }

    /// <summary>
    /// Is object dying.
    /// </summary>
    /// <param name="b"></param>
    /// <returns></returns>
    public static bool IsDying(this IBattleChara b)
    {
        if (b == null) return false;
        if (b.IsDummy()) return false;
        return b.GetTimeToKill() <= Service.Config.DyingTimeToKill || b.GetHealthRatio() < 0.02f;
    }

    internal static unsafe bool InCombat(this IBattleChara obj)
    {
        return obj.Struct()->Character.InCombat;
    }

    private static readonly TimeSpan CheckSpan = TimeSpan.FromSeconds(2.5);

    internal static float GetTimeToKill(this IBattleChara b, bool wholeTime = false)
    {
        if (b == null) return float.NaN;
        if (b.IsDummy()) return 999.99f;

        var objectId = b.GameObjectId;

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

    internal static bool IsAttacked(this IBattleChara b)
    {
        foreach (var (id, time) in DataCenter.AttackedTargets)
        {
            if (id == b.GameObjectId)
            {
                return DateTime.Now - time > TimeSpan.FromSeconds(1);
            }
        }
        return false;
    }

    internal static unsafe bool CanSee(this IGameObject b)
    {
        var point = Player.Object.Position + Vector3.UnitY * Player.IGameObject->Height;
        var tarPt = b.Position + Vector3.UnitY * b.Struct()->Height;
        var direction = tarPt - point;

        int* unknown = stackalloc int[] { 0x4000, 0, 0x4000, 0 };

        RaycastHit hit = default;

        return !FFXIVClientStructs.FFXIV.Client.System.Framework.Framework.Instance()->BGCollisionModule
            ->RaycastMaterialFilter(&hit, point, direction, direction.Length(), 1, unknown);
    }

    /// <summary>
    /// Get the <paramref name="g"/>'s current HP percentage.
    /// </summary>
    /// <param name="g"></param>
    /// <returns></returns>
    public static float GetHealthRatio(this IGameObject g)
    {
        if (g is not IBattleChara b) return 0;
        if (DataCenter.RefinedHP.TryGetValue(b.GameObjectId, out var hp)) return hp;
        return (float)b.CurrentHp / b.MaxHp;
    }

    internal static EnemyPositional FindEnemyPositional(this IGameObject enemy)
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

    internal static Vector2 GetFaceVector(this IGameObject obj)
    {
        float rotation = obj.Rotation;
        return new((float)Math.Cos(rotation), (float)Math.Sin(rotation));
    }

    internal static double AngleTo(this Vector2 vec1, Vector2 vec2)
    {
        return Math.Acos(Vector2.Dot(vec1, vec2) / vec1.Length() / vec2.Length());
    }

    /// <summary>
    /// The distance from <paramref name="obj"/> to the player
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static float DistanceToPlayer(this IGameObject? obj)
    {
        if (obj == null) return float.MaxValue;
        var player = Player.Object;
        if (player == null) return float.MaxValue;

        var distance = Vector3.Distance(player.Position, obj.Position) - player.HitboxRadius;
        distance -= obj.HitboxRadius;
        return distance;
    }
}
