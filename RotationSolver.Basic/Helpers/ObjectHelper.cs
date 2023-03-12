using Dalamud.Game.ClientState.Objects.Enums;
using Dalamud.Game.ClientState.Objects.Types;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.Event;
using Lumina.Excel.GeneratedSheets;
using RotationSolver.Basic;
using RotationSolver.Data;
using System.Numerics;

namespace RotationSolver.Helpers;

public static class ObjectHelper
{
    static readonly EventHandlerType[] _eventType = new EventHandlerType[]
    {
        EventHandlerType.TreasureHuntDirector,
        EventHandlerType.Quest,
    };

    private unsafe static BNpcBase GetObjectNPC(this GameObject obj)
    {
        if (obj == null) return null;
        return Service.GetSheet<BNpcBase>().GetRow(obj.DataId);
    }

    public static bool HasPositional(this GameObject obj)
    {
        if (obj == null) return false;
        return !(obj.GetObjectNPC()?.Unknown10 ?? false);
    }

    public static unsafe FFXIVClientStructs.FFXIV.Client.Game.Object.GameObject* GetAddress(this GameObject obj)
        => (FFXIVClientStructs.FFXIV.Client.Game.Object.GameObject*)(void*)obj.Address;

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
        => obj.GetObjectKind() == ObjectKind.BattleNpc
        && (byte)obj.GetBattleNPCSubkind() is (byte)BattleNpcSubKind.Enemy or 1;


    public static unsafe ObjectKind GetObjectKind(this GameObject obj) => (ObjectKind)obj.GetAddress()->ObjectKind;

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
        if(icon == 0) return false;
        var type = obj.GetEventType();

        if(type is EventHandlerType.Quest) return true;

        return false;
    }

    public static unsafe uint GetNamePlateIcon(this GameObject obj) => obj.GetAddress()->NamePlateIconId;
    public static unsafe EventHandlerType GetEventType(this GameObject obj) => obj.GetAddress()->EventId.Type;

    public static unsafe BattleNpcSubKind GetBattleNPCSubkind(this GameObject obj) => (BattleNpcSubKind)obj.GetAddress()->SubKind;

    public static unsafe uint FateId(this GameObject obj) => obj.GetAddress()->FateId;

    public static unsafe bool IsTargetable(this GameObject obj) => obj.GetAddress()->GetIsTargetable();

    static readonly Dictionary<uint, bool> _effectRangeCheck = new Dictionary<uint, bool>();
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

    private static bool IsDummy(this BattleChara obj) => obj?.NameId == 541;
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
        return (float)b.CurrentHp / b.MaxHp;
    }

    public static float GetHealingRatio(this BattleChara b)
    {
        return b.GetHealthRatio();
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
        Vector2 faceVec = new Vector2((float)Math.Cos(rotation), (float)Math.Sin(rotation));

        Vector3 dir = Service.Player.Position - pPosition;
        Vector2 dirVec = new Vector2(dir.Z, dir.X);

        double angle = Math.Acos(Vector2.Dot(dirVec, faceVec) / dirVec.Length() / faceVec.Length());

        if (angle < Math.PI / 4) return EnemyPositional.Front;
        else if (angle > Math.PI * 3 / 4) return EnemyPositional.Rear;
        return EnemyPositional.Flank;
    }

    public static uint GetHealthFromMulty(float mult)
    {
        if (Service.Player == null) return 0;

        var role = Service.GetSheet<ClassJob>().GetRow(
                Service.Player.ClassJob.Id).GetJobRole();
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

        return (uint)(multi * Service.Player.MaxHp);
    }

    /// <summary>
    /// 对象<paramref name="obj"/>距玩家的距离
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static float DistanceToPlayer(this GameObject obj)
    {
        if (obj == null) return float.MaxValue;
        var player = Service.Player;
        if (player == null) return float.MaxValue;

        var distance = Vector3.Distance(player.Position, obj.Position) - player.HitboxRadius;
        distance -= Math.Max(obj.HitboxRadius, Service.Config.ObjectMinRadius);
        return distance;
    }
}
