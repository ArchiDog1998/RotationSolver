using Dalamud.Utility;

namespace RotationSolver.Basic.Configuration.RotationConfig;

internal class RotationConfigCombo: RotationConfigBase
{
    public string[] Items { get; set; }

    public RotationConfigCombo(ICustomRotation rotation, PropertyInfo property)
        :base(rotation, property)
    {
        var names = new List<string>();
        foreach (Enum v in Enum.GetValues(property.PropertyType))
        {
            names.Add(v.GetAttribute<DescriptionAttribute>()?.Description ?? v.ToString());
        }
        Items = [.. names];
    }

    public override string ToString()
    {
        var indexStr = base.ToString();
        if (!int.TryParse(indexStr, out var index)) return Items[0];
        return Items[index];
    }

    public override bool DoCommand(IRotationConfigSet set, string str)
    {
        if (!base.DoCommand(set, str)) return false;

        string numStr = str[Name.Length..].Trim();
        var length = Items.Length;

        int nextId = (int.Parse(Value) + 1) % length;
        if (int.TryParse(numStr, out int num))
        {
            nextId = num % length;
        }
        else
        {
            for (int i = 0; i < length; i++)
            {
                if (Items[i] == str)
                {
                    nextId = i;
                }
            }
        }

        Value = nextId.ToString();
        return true;
    }
}
