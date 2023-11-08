namespace RotationSolver.Basic.Data;

internal readonly struct MapEffectData
{
    public readonly uint Position;

    public readonly ushort Param1, Param2;

    public readonly DateTime Time;

    public readonly TimeSpan TimeDuration => DateTime.Now - Time;

    public MapEffectData(uint position, ushort param1, ushort param2)
    {
        Time = DateTime.Now;
        Position = position;
        Param1 = param1;
        Param2 = param2;
    }

    public override string ToString() => $"MapEffect: Pos: {Position}, P1: {Param1}, P2: {Param2}";
}
