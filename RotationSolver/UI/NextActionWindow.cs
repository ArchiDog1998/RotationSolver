using Dalamud.Interface.Colors;
using Dalamud.Interface.Windowing;
using FFXIVClientStructs.FFXIV.Client.Game;
using ImGuiNET;
using RotationSolver.Basic;
using RotationSolver.Basic.Helpers;
using RotationSolver.Updaters;
using System.Numerics;

namespace RotationSolver.UI;

internal class NextActionWindow : InfoWindow
{
    public NextActionWindow()
        : base(nameof(NextActionWindow))
    {
    }

    public override void Draw()
    {
        var width = Service.Config.ControlWindowGCDSize * Service.Config.ControlWindowNextSizeRatio;
        DrawGcdCooldown(width, false);
        ControlWindow.DrawIAction(ActionUpdater.NextAction, width, 1);
    }

    public static unsafe void DrawGcdCooldown(float width, bool drawTittle)
    {
        var group = ActionManager.Instance()->GetRecastGroupDetail(ActionHelper.GCDCooldownGroup - 1);
        var remain = group->Total - group->Elapsed;
        var total = DataCenter.WeaponTotal;

        if (drawTittle)
        {
            var str = $"{remain:F2}s / {total:F2}s";
            ImGui.SetCursorPosX(ImGui.GetCursorPosX() + width / 2 - ImGui.CalcTextSize(str).X / 2);
            ImGui.Text(str);
        }

        var cursor = ImGui.GetCursorPos() + ImGui.GetWindowPos();
        var height = Service.Config.ControlProgressHeight;
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
