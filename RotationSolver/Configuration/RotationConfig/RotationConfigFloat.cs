using ImGuiNET;

namespace RotationSolver.Configuration.RotationConfig;

internal class RotationConfigFloat : RotationConfigBase
{
    public float Min, Max, Speed;

    public RotationConfigFloat(string name, float value, string displayName, float min, float max, float speed) : base(name, value.ToString(), displayName)
    {
        Min = min;
        Max = max;
        Speed = speed;
    }

    public override void Draw(RotationConfigSet set, bool canAddButton)
    {
        float val = set.GetFloat(Name);
        ImGui.SetNextItemWidth(100);
        if (ImGui.DragFloat($"{DisplayName}##{GetHashCode()}_{Name}", ref val, Speed, Min, Max))
        {
            set.SetValue(Name, val.ToString());
            Service.Configuration.Save();
        }
    }
}
