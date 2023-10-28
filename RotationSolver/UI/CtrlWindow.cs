using Dalamud.Interface.Windowing;
using RotationSolver.Basic.Configuration;

namespace RotationSolver.UI;

internal abstract class CtrlWindow : Window
{
    public const ImGuiWindowFlags BaseFlags = ImGuiWindowFlags.NoScrollbar
                        | ImGuiWindowFlags.NoCollapse
                        | ImGuiWindowFlags.NoTitleBar
                        | ImGuiWindowFlags.NoNav
                        | ImGuiWindowFlags.NoScrollWithMouse;
    public CtrlWindow(string name)
                : base(name, BaseFlags)
    {

    }

    public override void PreDraw()
    {
        Vector4 bgColor = Service.Config.GetValue(PluginConfigBool.IsControlWindowLock)
            ? Service.Config.GetValue(PluginConfigVector4.ControlWindowLockBg)
            : Service.Config.GetValue(PluginConfigVector4.ControlWindowUnlockBg);
        ImGui.PushStyleColor(ImGuiCol.WindowBg, bgColor);

        Flags = BaseFlags;

        if (Service.Config.GetValue(PluginConfigBool.IsControlWindowLock))
        {
            Flags |= ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoMove;
        }

        ImGui.PushStyleVar(ImGuiStyleVar.WindowBorderSize, 0);
        base.PreDraw();
    }

    public override void PostDraw()
    {
        ImGui.PopStyleColor();
        ImGui.PopStyleVar();
        base.PostDraw();
    }
}
