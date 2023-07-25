using Dalamud.Interface.Colors;
using FFXIVClientStructs.FFXIV.Client.Game;
using RotationSolver.Basic.Configuration;
using RotationSolver.Updaters;

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

        var strs = new List<string>(3);
        if(Service.Config.GetValue(SettingsCommand.UseAOEAction)
            && (!DataCenter.IsManual
            || Service.Config.GetValue(SettingsCommand.UseAOEWhenManual)))
        {
            strs.Add("AOE");
        }
        if (Service.Config.GetValue(SettingsCommand.PreventActions))
        {
            strs.Add("Prevent");
        }
        if (Service.Config.GetValue(SettingsCommand.AutoBurst))
        {
            strs.Add("Burst");
        }
        if(strs.Count > 0)
        {
            ImGui.PushStyleColor(ImGuiCol.Text, ImGuiColors.DalamudWhite2);
            ImGui.TextWrapped(string.Join(", ", strs));
            ImGui.PopStyleColor();
        }
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

        ImGui.ProgressBar(group->Elapsed / group->Total, new Vector2(width, height), string.Empty);

        var actionRemain = DataCenter.ActionRemain;
        if(remain > actionRemain + 0.6f + DataCenter.Ping)
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
