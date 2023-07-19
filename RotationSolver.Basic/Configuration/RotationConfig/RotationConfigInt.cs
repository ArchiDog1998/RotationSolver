namespace RotationSolver.Basic.Configuration.RotationConfig;

internal class RotationConfigInt : RotationConfigBase
{
    public int Min, Max, Speed;

    public RotationConfigInt(string name, int value, string displayName, int min, int max, int speed) : base(name, value.ToString(), displayName)
    {
        Min = min;
        Max = max;
        Speed = speed;
    }
}
