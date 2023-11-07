using ECommons.ExcelServices;

namespace RotationSolver.Basic.Configuration.RotationConfig;

internal class RotationConfigCombo : RotationConfigBase
{
    public string[] Items { get; set; }
    public RotationConfigCombo(string name, int value, string displayName, string[] items, CombatType type) : base(name, value.ToString(), displayName, type)
    {
        Items = items;
    }

    public override string GetDisplayValue(Job job, string rotationName)
    {
        var indexStr = base.GetDisplayValue(job, rotationName);
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
