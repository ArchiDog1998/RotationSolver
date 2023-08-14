namespace RotationSolver.Basic.Configuration.RotationConfig;

internal class RotationConfigFloat : RotationConfigBase
{
    public float Min, Max, Speed;

    public RotationConfigFloat(string name, float value, string displayName, float min, float max, float speed) : base(name, value.ToString(), displayName)
    {
        Min = min;
        Max = max;
        Speed = speed;
    }

    public override bool DoCommand(IRotationConfigSet set, string str)
    {
        if (!base.DoCommand(set, str)) return false;

        string numStr = str[Name.Length..].Trim();

        if (float.TryParse(numStr, out _))
        {
            set.SetValue(Name, numStr.ToString());
        }
        return true;
    }
}
