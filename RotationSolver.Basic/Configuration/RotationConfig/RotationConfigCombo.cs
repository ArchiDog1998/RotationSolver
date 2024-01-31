namespace RotationSolver.Basic.Configuration.RotationConfig;

internal class RotationConfigCombo(string name, int value, string displayName, string[] items, CombatType type) 
    : RotationConfigBase(name, value.ToString(), displayName, type)
{
    public string[] Items { get; set; } = items;

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

        int nextId = (set.GetCombo(Name) + 1) % length;
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

        set.SetValue(Name, nextId.ToString());
        return true;
    }
}
