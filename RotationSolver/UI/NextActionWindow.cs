using Dalamud.Interface.Colors;
using Dalamud.Interface.Windowing;
using FFXIVClientStructs.FFXIV.Client.Game;
using ImGuiNET;
using RotationSolver.Basic;
using RotationSolver.Basic.Helpers;
using RotationSolver.Updaters;
using System.Numerics;

namespace RotationSolver.UI;

internal class NextActionWindow : Window
{
    public const ImGuiWindowFlags BaseFlags = ImGuiWindowFlags.NoScrollbar
                                | ImGuiWindowFlags.NoCollapse
                                | ImGuiWindowFlags.NoTitleBar
                                | ImGuiWindowFlags.NoNav
                                | ImGuiWindowFlags.NoScrollWithMouse;

    public NextActionWindow()
        : base(nameof(NextActionWindow), BaseFlags 
            | ImGuiWindowFlags.AlwaysAutoResize 
            | ImGuiWindowFlags.NoResize)
    {
    }

    public override void PreDraw()
    {
        ImGui.PushStyleColor(ImGuiCol.WindowBg, Service.Config.NextActionWindowBg);

        //ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new Vector2(0, 0));
        ImGui.PushStyleVar(ImGuiStyleVar.WindowBorderSize, 0);
    }

    public override void PostDraw()
    {
        ImGui.PopStyleColor();
        ImGui.PopStyleVar();
        base.PostDraw();
    }

    public override void Draw()
    {
        var width = Service.Config.ControlWindowGCDSize * Service.Config.ControlWindowNextSizeRatio;
        DrawGcdCooldown(width, false);
        ControlWindow.DrawIAction(ActionUpdater.NextAction, width);
    }

    public static unsafe void DrawGcdCooldown(float width, bool drawTittle)
    {
        var group = ActionManager.Instance()->GetRecastGroupDetail(ActionHelper.GCDCooldownGroup - 1);
        var remain = group->Total - group->Elapsed;

        if(drawTittle)
        {
            var str = $"{remain:F2}s / {group->Total:F2}s";
            ImGui.SetCursorPosX(ImGui.GetCursorPosX() + width / 2 - ImGui.CalcTextSize(str).X / 2);
            ImGui.Text(str);
        }

        var cursor = ImGui.GetCursorPos() + ImGui.GetWindowPos();
        var height = Service.Config.ControlProgressHeight;
        var total = DataCenter.WeaponTotal;
        var interval = Service.Config.AbilitiesInterval;

        ImGui.ProgressBar(group->Elapsed / group->Total, new Vector2(width, height), string.Empty);

        foreach (var value in CalculateValue(total, interval))
        {
            if (value < DataCenter.CastingTotal) continue;
            var pt = cursor + new Vector2(width, 0) * value / total;

            ImGui.GetWindowDrawList().AddLine(pt, pt + new Vector2(0, height),
                ImGui.ColorConvertFloat4ToU32(ImGuiColors.DalamudRed));
        }
    }

    static float[] CalculateValue(float total, float interval)
    {
        if(interval <= 0 || total <= 0 || total <= interval) return new float[0];

        var count = (int)(total / interval);
        var result = new List<float>();

        if(count > 1)
        {
            for (int i = 1; i < count-1; i++)
            {
                result.Add(i * interval);
            }
        }

        result.Add(total - interval);
        return result.ToArray();
    }
}
