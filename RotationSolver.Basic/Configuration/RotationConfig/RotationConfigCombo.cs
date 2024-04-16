using Dalamud.Utility;

namespace RotationSolver.Basic.Configuration.RotationConfig;

internal class RotationConfigCombo: RotationConfigBase
{
    public string[] DisplayValues { get; }
    public int selectedIdx { get; set; }

    public RotationConfigCombo(ICustomRotation rotation, PropertyInfo property)
        :base(rotation, property)
    {
        var names = new List<string>();
        foreach (Enum v in Enum.GetValues(property.PropertyType))
        {
            names.Add(v.GetAttribute<DescriptionAttribute>()?.Description ?? v.ToString());
        }

        DisplayValues = [.. names];
    }

    public override string ToString()
    {
        var indexStr = base.ToString();
        if (!int.TryParse(indexStr, out var index)) return DisplayValues[0].ToString();
        return DisplayValues[index];
    }

    public override bool DoCommand(IRotationConfigSet set, string str)
    {
        if (!base.DoCommand(set, str)) return false;

        string numStr = str[Name.Length..].Trim();
        var length = DisplayValues.Length;

        int nextId = (int.Parse(Value) + 1) % length;
        if (int.TryParse(numStr, out int num))
        {
            nextId = num % length;
        }
        else
        {
            for (int i = 0; i < length; i++)
            {
                if (DisplayValues[i] == str)
                {
                    nextId = i;
                }
            }
        }

        Value = nextId.ToString();
        return true;
    }
}
