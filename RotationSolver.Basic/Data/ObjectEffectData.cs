using ECommons.DalamudServices;

namespace RotationSolver.Basic.Data;

public readonly struct ObjectEffectData
{
    public readonly uint ObjectId;

    public readonly ushort Param1, Param2;

    public readonly DateTime Time;

    public readonly TimeSpan TimeDuration => DateTime.Now - Time;

    public ObjectEffectData(uint objectId, ushort param1, ushort param2)
    {
        Time = DateTime.Now;
        ObjectId = objectId;
        Param1 = param1;
        Param2 = param2;
    }

    public override string ToString() => $"Object Effect: {Svc.Objects.SearchById(ObjectId)?.Name ?? "Object"}, P1: {Param1}, P2: {Param2}";
}
