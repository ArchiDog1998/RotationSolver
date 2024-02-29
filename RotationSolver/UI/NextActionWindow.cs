using Dalamud.Interface.Colors;
using Dalamud.Interface.Windowing;
using FFXIVClientStructs.FFXIV.Client.Game;
using RotationSolver.Basic.Configuration;
using RotationSolver.Updaters;

namespace RotationSolver.UI;

internal class NextActionWindow : Window
{
    const ImGuiWindowFlags BaseFlags = ControlWindow.BaseFlags
    | ImGuiWindowFlags.AlwaysAutoResize
    | ImGuiWindowFlags.NoResize;

    public NextActionWindow()
        : base(nameof(NextActionWindow), BaseFlags)
    {
    }

    public override void PreDraw()
    {
        ImGui.PushStyleColor(ImGuiCol.WindowBg, Service.Config.InfoWindowBg);

        Flags = BaseFlags;
        if (Service.Config.IsInfoWindowNoInputs)
        {
            Flags |= ImGuiWindowFlags.NoInputs;
        }
        if (Service.Config.IsInfoWindowNoMove)
        {
            Flags |= ImGuiWindowFlags.NoMove;
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

    public override void Draw()
    {
        var width = Service.Config.ControlWindowGCDSize
            * Service.Config.ControlWindowNextSizeRatio;
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
        var height = Service.Config.ControlProgressHeight   ;

        ImGui.ProgressBar(group->Elapsed / group->Total, new Vector2(width, height), string.Empty);

        var actionRemain = DataCenter.ActionRemain;
        if (remain > actionRemain + 0.6f + DataCenter.Ping)
        {
            var value = total - remain + actionRemain;

            if (value > DataCenter.CastingTotal)
            {
                var pt = cursor + new Vector2(width, 0) * value / total;

                ImGui.GetWindowDrawList().AddLine(pt, pt + new Vector2(0, height),
                    ImGui.ColorConvertFloat4ToU32(ImGuiColors.DalamudRed), 2);
            }
        }
    }
}