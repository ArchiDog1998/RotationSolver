using ImGuiNET;
using RotationSolver.Basic.Actions;
using RotationSolver.Basic;
using RotationSolver.Updaters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Numerics;

namespace RotationSolver.UI;

internal class CooldownWindow : InfoWindow
{
    public CooldownWindow()
        :base(nameof(CooldownWindow))
    {
        
    }

    public override void Draw()
    {
        if (RotationUpdater.RightNowRotation != null)
        {
            foreach (var pair in RotationUpdater.AllGroupedActions)
            {
                var showItems = pair.Where(i => !(i is IBaseAction a && a.IsGeneralGCD)).OrderBy(a => a.ID);

                if (!showItems.Any()) continue;
                if (!Service.Config.ShowItemsCooldown && showItems.Any(i => i is IBaseItem)) continue;

                ImGui.Text(pair.Key);

                uint started = 0;
                foreach (var item in showItems)
                {
                    if (started % Math.Max(1, Service.Config.CooldownActionOneLine) != 0)
                    {
                        ImGui.SameLine();
                    }
                    DrawActionCooldown(item);
                    started++;
                }
            }
        }
    }

    static readonly uint progressCol = ImGui.ColorConvertFloat4ToU32(new Vector4(0.6f, 0.6f, 0.6f, 0.6f));
    private static void DrawActionCooldown(IAction act)
    {
        var width = Service.Config.ControlWindow0GCDSize;
        var recast = act.RecastTimeOneCharge;
        var elapsed = act.RecastTimeElapsed;

        ImGui.BeginGroup();
        var pos = ImGui.GetCursorPos();
        var winPos = ImGui.GetWindowPos();

        ControlWindow.DrawIAction(act, width);
        ImGuiHelper.HoveredString(act.Name);

        if (!act.EnoughLevel)
        {
            ImGui.GetWindowDrawList().AddRectFilled(new Vector2(pos.X, pos.Y) + winPos,
                new Vector2(pos.X + width, pos.Y + width) + winPos, progressCol);
        }
        else if (act.IsCoolingDown)
        {
            var ratio = recast == 0 ? 0 : elapsed % recast / recast;
            ImGui.GetWindowDrawList().AddRectFilled(new Vector2(pos.X + width * ratio, pos.Y) + winPos,
                new Vector2(pos.X + width, pos.Y + width) + winPos, progressCol);

            string time = recast == 0 || !act.EnoughLevel ? "0" : ((int)(recast - elapsed % recast) + 1).ToString();
            var strSize = ImGui.CalcTextSize(time);
            var fontPos = new Vector2(pos.X + width / 2 - strSize.X / 2, pos.Y + width / 2 - strSize.Y / 2) + winPos;

            TextShade(fontPos, time);
        }

        if (act.EnoughLevel && act is IBaseAction bAct && bAct.MaxCharges > 1)
        {
            for (int i = 0; i < bAct.CurrentCharges; i++)
            {
                ImGui.GetWindowDrawList().AddCircleFilled(winPos + pos + (i + 0.5f) * new Vector2(6, 0), 2.5f, white);
            }
        }

        ImGui.EndGroup();
    }

    static readonly uint black = ImGui.ColorConvertFloat4ToU32(new Vector4(0, 0, 0, 1));
    static readonly uint white = ImGui.ColorConvertFloat4ToU32(new Vector4(1, 1, 1, 1));

    public static void TextShade(Vector2 pos, string text, float width = 1.5f)
    {
        ImGui.GetWindowDrawList().AddText(pos - new Vector2(0, width), black, text);
        ImGui.GetWindowDrawList().AddText(pos - new Vector2(0, -width), black, text);
        ImGui.GetWindowDrawList().AddText(pos - new Vector2(width, 0), black, text);
        ImGui.GetWindowDrawList().AddText(pos - new Vector2(-width, 0), black, text);
        ImGui.GetWindowDrawList().AddText(pos, white, text);
    }
}
