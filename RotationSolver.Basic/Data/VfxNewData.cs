using ECommons.DalamudServices;

namespace RotationSolver.Basic.Data;

internal readonly struct VfxNewData(ulong objectId, string path)
{
    public readonly ulong ObjectId = objectId;
    public readonly string Path = path;

    public readonly DateTime Time = DateTime.Now;

    public readonly TimeSpan TimeDuration => DateTime.Now - Time;

    public override string ToString() => $"Object Effect: {Svc.Objects.SearchById(ObjectId)?.Name ?? "Object"}: {Path}";
}
