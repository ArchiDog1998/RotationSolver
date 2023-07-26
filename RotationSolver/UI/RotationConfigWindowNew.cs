using Dalamud.Interface.Windowing;

namespace RotationSolver.UI;

public class RotationConfigWindowNew : Window
{
    public RotationConfigWindowNew()
    : base(nameof(RotationConfigWindowNew), 0, false)
    {
        SizeCondition = ImGuiCond.FirstUseEver;
        Size = new Vector2(740f, 490f);
        RespectCloseHotkey = true;
    }

    public override void Draw()
    {
    }
}
