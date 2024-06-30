using Dalamud.Game.ClientState.Objects.SubKinds;
using ECommons.GameHelpers;
using System.Text.RegularExpressions;

namespace RotationSolver.Basic.Configuration.Timeline.TimelineCondition;

internal class ObjectGetter
{
    public ObjectType Type { get; set; }
    public string DataID { get; set; } = "";
    public bool Tank { get; set; } = true;
    public bool Healer { get; set; } = true;
    public bool Melee { get; set; } = true;
    public bool Range { get; set; } = true;
    public bool Caster { get; set; } = true;
    public uint Status { get; set; }
    public float StatusTime { get; set; } = 5;
    public Vector2 TimeDuration { get; set; } = new(0, 2);
    public string VfxPath { get; set; } = string.Empty;
    public ushort ObjectEffect1 { get; set; } = 0;
    public ushort ObjectEffect2 { get; set; } = 0;
    public bool CanGet(IGameObject obj)
    {
        switch (Type)
        {
            case ObjectType.IGameObject:
                if (!string.IsNullOrEmpty(DataID) && !new Regex(DataID).IsMatch(obj.DataId.ToString("X"))) return false;
                break;

            case ObjectType.IBattleCharactor:
                if (obj is not IBattleChara) return false;
                if (!string.IsNullOrEmpty(DataID) && !new Regex(DataID).IsMatch(obj.DataId.ToString("X"))) return false;

                break;

            case ObjectType.PlayerCharactor:
                if (obj is not IPlayerCharacter) return false;

                if (!Tank && obj.IsJobCategory(JobRole.Tank)) return false;
                if (!Healer && obj.IsJobCategory(JobRole.Healer)) return false;
                if (!Melee && obj.IsJobCategory(JobRole.Melee)) return false;
                if (!Range && obj.IsJobCategory(JobRole.RangedPhysical)) return false;
                if (!Caster && obj.IsJobCategory(JobRole.RangedMagical)) return false;
                break;

            case ObjectType.Myself:
                return obj == Player.Object;
        }

        if (Status != 0)
        {
            if (obj is not IBattleChara b) return false;
            var status = b.StatusList.FirstOrDefault(s => s.StatusId == Status);
            if (status == null) return false;
            if (status.RemainingTime > StatusTime) return false;
        }

        if (!string.IsNullOrEmpty(VfxPath))
        {
            if (!DataCenter.VfxNewData.Reverse().Any(effect =>
            {
                if (effect.ObjectId != obj.GameObjectId) return false;

                var time = effect.TimeDuration.TotalSeconds;

                if (time < TimeDuration.X) return false;
                if (time > TimeDuration.Y) return false;

                if (effect.Path != VfxPath) return false;

                return true;
            })) return false;
        }

        if (ObjectEffect1 != 0 || ObjectEffect2 != 0)
        {
            if (!DataCenter.ObjectEffects.Reverse().Any(effect =>
            {
                if (effect.ObjectId != obj.GameObjectId) return false;

                var time = effect.TimeDuration.TotalSeconds;

                if (time < TimeDuration.X) return false;
                if (time > TimeDuration.Y) return false;

                if (effect.Param1 != ObjectEffect1) return false;
                if (effect.Param2 != ObjectEffect2) return false;

                return true;
            })) return false;
        }

        return true;
    }
}

public enum ObjectType : byte
{
    IGameObject,
    IBattleCharactor,
    PlayerCharactor,
    Myself,
}