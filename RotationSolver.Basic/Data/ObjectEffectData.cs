namespace RotationSolver.Basic.Data;

internal readonly struct ObjectEffectData
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
}
