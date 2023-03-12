namespace RotationSolver.Configuration.RotationConfig;

public class RotationConfigFloat : RotationConfigBase
{
    public float Min, Max, Speed;

    public RotationConfigFloat(string name, float value, string displayName, float min, float max, float speed) : base(name, value.ToString(), displayName)
    {
        Min = min;
        Max = max;
        Speed = speed;
    }
}
