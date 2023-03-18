using Dalamud.Interface.Windowing;
using ImGuiNET;
using RotationSolver.Basic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace RotationSolver.UI;

internal class ControlWindow : Window
{
    const ImGuiWindowFlags _baseFlags = ImGuiWindowFlags.NoScrollbar
                                    | ImGuiWindowFlags.NoCollapse
                                    | ImGuiWindowFlags.NoTitleBar
                                    | ImGuiWindowFlags.NoNav
                                    | ImGuiWindowFlags.NoScrollWithMouse;

    public ControlWindow()
        : base(nameof(ControlWindow), _baseFlags)
    {
        this.IsOpen = true;
    }

    public override void PreDraw()
    {
        Flags = _baseFlags;

        if (Service.Config.IsControlWindowLock)
        {
            Flags |= ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoMouseInputs;
        }
    }

    public override void Draw()
    {
        ImGui.Text("Hello!");
    }
}
