using Dalamud.Interface.Windowing;
using RotationSolver.Updaters;

namespace RotationSolver.UI;

public class RotationConfigWindowNew : Window
{
    private static float _scale => ImGuiHelpers.GlobalScale;
    private static float _sideBarWidth => 100;
    public RotationConfigWindowNew()
    : base(nameof(RotationConfigWindowNew), 0, false)
    {
        SizeCondition = ImGuiCond.FirstUseEver;
        Size = new Vector2(740f, 490f);
        RespectCloseHotkey = true;
    }

    public override void Draw()
    {
        ImGui.Columns(2);
        DrawSideBar();
        ImGui.NextColumn();
        DrawBody();
        ImGui.Columns(1);
    }

    private void DrawSideBar()
    {
        //var rotations = RotationUpdater.CustomRotations
    }

    private void DrawBody()
    {
        if (ImGui.BeginChild("Action List", new Vector2(0f, -1f), true))
        {
            ImGui.EndChild();
        }
    }
}
