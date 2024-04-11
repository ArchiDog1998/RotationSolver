namespace RotationSolver.Basic.Data;

/// <summary>
/// The map effect data.
/// </summary>
public readonly struct MapEffectData
{
    /// <summary>
    /// The position id.
    /// </summary>
    public readonly uint Position;

    /// <summary>
    /// The param.
    /// </summary>
    public readonly ushort Param1, Param2;


    private readonly DateTime _time;

    /// <summary>
    /// The time duration.
    /// </summary>
    public readonly TimeSpan TimeDuration => DateTime.Now - _time;

    internal MapEffectData(uint position, ushort param1, ushort param2)
    {
        _time = DateTime.Now;
        Position = position;
        Param1 = param1;
        Param2 = param2;
    }

    /// <inheritdoc/>
    public override string ToString() => $"MapEffect: Pos: {Position}, P1: {Param1}, P2: {Param2}";
}
