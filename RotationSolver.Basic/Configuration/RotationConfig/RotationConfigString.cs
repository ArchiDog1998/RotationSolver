using ImGuiNET;
using RotationSolver.Basic;

namespace RotationSolver.Configuration.RotationConfig;

public class RotationConfigString : RotationConfigBase
{
    public RotationConfigString(string name, string value, string displayName) : base(name, value, displayName)
    {
    }
}
