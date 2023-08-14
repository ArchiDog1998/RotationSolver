using Dalamud.Interface.Windowing;
using RotationSolver.Basic.Configuration;

namespace RotationSolver.UI;

internal abstract class InfoWindow : Window
{
    const ImGuiWindowFlags BaseFlags = ControlWindow.BaseFlags
        | ImGuiWindowFlags.AlwaysAutoResize
        | ImGuiWindowFlags.NoResize;

    public InfoWindow(string name)
                : base(name, BaseFlags)
    {
        
    }

    public override void PreDraw()
    {
        ImGui.PushStyleColor(ImGuiCol.WindowBg, Service.Config.GetValue(PluginConfigVector4.InfoWindowBg));

        Flags = BaseFlags;
        if (Service.Config.GetValue(PluginConfigBool.IsInfoWindowNoInputs))
        {
            Flags |= ImGuiWindowFlags.NoInputs;
        }
        if (Service.Config.GetValue(PluginConfigBool.IsInfoWindowNoMove))
        {
            Flags |= ImGuiWindowFlags.NoMove;
        }
        //ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new Vector2(0, 0));
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
