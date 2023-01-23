using ImGuiNET;

namespace RotationSolver.Configuration.RotationConfig;

internal class RotationConfigString : RotationConfigBase
{
    public RotationConfigString(string name, string value, string displayName) : base(name, value, displayName)
    {
    }

    public override void Draw(RotationConfigSet set, bool canAddButton)
    {
        string val = set.GetString(Name);
        if (ImGui.InputText($"{DisplayName}##{GetHashCode()}_{Name}", ref val, 15))
        {
            set.SetValue(Name, val);
            Service.Configuration.Save();
        }
    }
}
