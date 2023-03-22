using Dalamud.Interface.Windowing;
using ImGuiNET;
using RotationSolver.Basic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        ImGui.PushStyleColor(ImGuiCol.WindowBg, Service.Config.InfoWindowBg);

        Flags = BaseFlags;
        if (Service.Config.IsInfoWindowLock)
        {
            Flags |= ImGuiWindowFlags.NoInputs;
        }

        //ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new Vector2(0, 0));
        ImGui.PushStyleVar(ImGuiStyleVar.WindowBorderSize, 0);
    }

    public override void PostDraw()
    {
        ImGui.PopStyleColor();
        ImGui.PopStyleVar();
        base.PostDraw();
    }
}
