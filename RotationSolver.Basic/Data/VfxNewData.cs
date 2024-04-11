using ECommons.DalamudServices;

namespace RotationSolver.Basic.Data;

/// <summary>
/// The vfx new data.
/// </summary>
public readonly struct VfxNewData
{
    /// <summary>
    /// The id of the object.
    /// </summary>
    public readonly uint ObjectId;

    /// <summary>
    /// The path.
    /// </summary>
    public readonly string Path;

    private readonly DateTime _time;

    /// <summary>
    /// The time duration.
    /// </summary>
    public readonly TimeSpan TimeDuration => DateTime.Now - _time;

    internal VfxNewData(uint objectId, string path)
    {
        _time = DateTime.Now;
        ObjectId = objectId;
        Path = path;
    }

    /// <inheritdoc/>
    public override string ToString() => $"Object Effect: {Svc.Objects.SearchById(ObjectId)?.Name ?? "Object"}: {Path}, {TimeDuration.TotalSeconds}";
}
