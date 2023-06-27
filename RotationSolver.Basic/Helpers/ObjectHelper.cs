using Dalamud.Game.ClientState.Objects.Enums;
using ECommons.GameFunctions;
using ECommons.GameHelpers;
using FFXIVClientStructs.FFXIV.Client.Game.Event;
using Lumina.Excel.GeneratedSheets;

namespace RotationSolver.Basic.Helpers;

public static class ObjectHelper
{
    static readonly EventHandlerType[] _eventType = new EventHandlerType[]
    {
        EventHandlerType.TreasureHuntDirector,
        EventHandlerType.Quest,
    };

    public static BNpcBase GetObjectNPC(this GameObject obj)
    {
        if (obj == null) return null;
        return Service.GetSheet<BNpcBase>().GetRow(obj.DataId);
    }

    public static bool HasPositional(this GameObject obj)
    {
        if (obj == null) return false;
        return !(obj.GetObjectNPC()?.Unknown10 ?? false);
    }

    public static unsafe bool IsOthersPlayers(this GameObject obj)
    {
        //SpecialType but no NamePlateIcon
        if (_eventType.Contains(obj.GetEventType()))
        {
            return obj.GetNamePlateIcon() == 0;
        }
        return false;
    }

    public static unsafe bool IsNPCEnemy(this GameObject obj)
        => obj != null && obj.GetObjectKind() == ObjectKind.BattleNpc
        && obj.IsHostile();

    public static unsafe ObjectKind GetObjectKind(this GameObject obj) => (ObjectKind)obj.Struct()->ObjectKind;

    public static bool IsTopPriorityHostile(this GameObject obj)
    {
        var icon = obj.GetNamePlateIcon();
        //Hunting log and weapon.
        if (icon
            is 60092 //Hunting
            or 60096 //Weapon
            or 71204 //Main Quest
            or 71144 //Major Quest
            or 71224 //Other Quest
            or 71244 //Leve
            or 71344 //Major Quest
            ) return true;
        if (icon == 0) return false;
        var type = obj.GetEventType();

        if (type is EventHandlerType.Quest) return true;

        return false;
    }

    public static unsafe uint GetNamePlateIcon(this GameObject obj) => obj.Struct()->NamePlateIconId;
    public static unsafe void SetNamePlateIcon(this GameObject obj, uint id) => obj.Struct()->NamePlateIconId = id;
    public static unsafe EventHandlerType GetEventType(this GameObject obj) => obj.Struct()->EventId.Type;

    public static unsafe BattleNpcSubKind GetBattleNPCSubKind(this GameObject obj) => (BattleNpcSubKind)obj.Struct()->SubKind;

    public static unsafe uint FateId(this GameObject obj) => obj.Struct()->FateId;

    static readonly Dictionary<uint, bool> _effectRangeCheck = new();
    public static bool CanInterrupt(this BattleChara b)
    {
        var baseCheck = b.IsCasting && b.IsCastInterruptible && b.TotalCastTime >= 2;

        if (!baseCheck) return false;
        if (!Service.Config.InterruptibleMoreCheck) return true;

        var id = b.CastActionId;
        if (_effectRangeCheck.TryGetValue(id, out var check)) return check;

        var act = Service.GetSheet<Lumina.Excel.GeneratedSheets.Action>().GetRow(b.CastActionId);
        if (act == null) return _effectRangeCheck[id] = false;
        if (act.CastType is 3 or 4) return _effectRangeCheck[id] = false;
        if (act.EffectRange is > 0 and < 8) return _effectRangeCheck[id] = false;
        return _effectRangeCheck[id] = true;
    }

    public static bool IsDummy(this BattleChara obj) => obj?.NameId == 541;
    /// <summary>
    /// Is character a boss? Max HP exceeds a certain amount.
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static bool IsBoss(this BattleChara obj)
    {
        if (obj == null) return false;
        if (obj.IsDummy() && !Service.Config.ShowHealthRatio) return true;
        return obj.MaxHp >= GetHealthFromMulty(Service.Config.HealthRatioBoss)
            || !(obj.GetObjectNPC()?.IsTargetLine ?? true);
    }

    /// <summary>
    /// Is character a dying? Current HP is below a certain amount. It is for running out of resources.
    /// </summary>
    /// <param name="b"></param>
    /// <returns></returns>
    public static bool IsDying(this BattleChara b)
    {
        if (b == null) return false;
        if (b.IsDummy() && !Service.Config.ShowHealthRatio) return false;
        return b.CurrentHp <= GetHealthFromMulty(Service.Config.HealthRatioDying) || b.GetHealthRatio() < 0.02f;
    }

    /// <summary>
    /// Get the <paramref name="b"/>'s current HP percentage.
    /// </summary>
    /// <param name="b"></param>
    /// <returns></returns>
    public static float GetHealthRatio(this BattleChara b)
    {
        if (b == null) return 0;
        if(DataCenter.RefinedHP.TryGetValue(b.ObjectId, out var hp)) return hp;
        return (float)b.CurrentHp / b.MaxHp;
    }

    public static bool CanDot(this BattleChara b)
    {
        if (b == null) return false;
        if (b.IsDummy() && !Service.Config.ShowHealthRatio) return true;
        return b.CurrentHp >= GetHealthFromMulty(Service.Config.HealthRatioDot);
    }

    public static EnemyPositional FindEnemyPositional(this GameObject enemy)
    {
        Vector3 pPosition = enemy.Position;
        float rotation = enemy.Rotation;
        Vector2 faceVec = new((float)Math.Cos(rotation), (float)Math.Sin(rotation));

        Vector3 dir = Player.Object.Position - pPosition;
        Vector2 dirVec = new(dir.Z, dir.X);

        double angle = Math.Acos(Vector2.Dot(dirVec, faceVec) / dirVec.Length() / faceVec.Length());

        if (angle < Math.PI / 4) return EnemyPositional.Front;
        else if (angle > Math.PI * 3 / 4) return EnemyPositional.Rear;
        return EnemyPositional.Flank;
    }

    public static uint GetHealthFromMulty(float mult)
    {
        if (!Player.Available) return 0;

        var role = Service.GetSheet<ClassJob>().GetRow(
                Player.Object.ClassJob.Id).GetJobRole();
        float multi = mult * role switch
        {
            JobRole.Tank => 1,
            JobRole.Healer => 1.6f,
            _ => 1.5f,
        };

        var partyCount = DataCenter.PartyMembers.Count();
        if (partyCount > 4)
        {
            multi *= 6.4f;
        }
        else if (partyCount > 1)
        {
            multi *= 3.5f;
        }

        return (uint)(multi * Player.Object.MaxHp);
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
