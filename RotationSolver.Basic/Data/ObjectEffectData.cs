using ECommons.DalamudServices;

namespace RotationSolver.Basic.Data;

/// <summary>
/// The object effect data.
/// </summary>
public readonly struct ObjectEffectData
{
    /// <summary>
    /// The id of the object.
    /// </summary>
    public readonly uint ObjectId;

    /// <summary>
    /// The parameters.
    /// </summary>
    public readonly ushort Param1, Param2;

    private readonly DateTime _time;

    /// <summary>
    /// The time duration.
    /// </summary>
    public readonly TimeSpan TimeDuration => DateTime.Now - _time;

    internal ObjectEffectData(uint objectId, ushort param1, ushort param2)
    {
        _time = DateTime.Now;
        ObjectId = objectId;
        Param1 = param1;
        Param2 = param2;
    }

    /// <inheritdoc/>
    public override string ToString() => $"Object Effect: {Svc.Objects.SearchById(ObjectId)?.Name ?? "Object"}, P1: {Param1}, P2: {Param2}";
}
