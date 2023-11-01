using ECommons.DalamudServices;

namespace RotationSolver.Basic.Data;

internal readonly struct VfxNewData
{
    public readonly uint ObjectId;
    public readonly string Path;

    public readonly DateTime Time;

    public readonly TimeSpan TimeDuration => DateTime.Now - Time;

    public VfxNewData(uint objectId, string path)
    {
        Time = DateTime.Now;
        ObjectId = objectId;
        Path = path;
    }

    public override string ToString() => $"Object Effect: {Svc.Objects.SearchById(ObjectId)?.Name ?? "Object"}: {Path}";
}
