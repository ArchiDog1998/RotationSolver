using Dalamud.Interface.Utility.Raii;
using RotationSolver.Updaters;
using XIVConfigUI;

namespace RotationSolver.UI;

internal class CooldownWindow() : CtrlWindow(nameof(CooldownWindow))
{
    public override void Draw()
    {
        if (DataCenter.RightNowRotation == null) return;

        var width = Service.Config.CooldownWindowIconSize;
        var count = Math.Max(1, (int)MathF.Floor(ImGui.GetColumnWidth() / ((width * (1 + (6 / 82))) + ImGui.GetStyle().ItemSpacing.X)));

        if (RotationUpdater.AllGroupedActions == null) return;

        foreach (var pair in RotationUpdater.AllGroupedActions)
        {
            var showItems = pair.OrderBy(a => a.SortKey).Where(a => a.IsInCD
                && (a is not IBaseAction b || !b.Info.IsLimitBreak));
            if (!Service.Config.ShowGcdCooldown) showItems = showItems.Where(i => !(i is IBaseAction a && a.Info.IsGeneralGCD));

            if (!showItems.Any()) continue;
            if (!Service.Config.ShowItemsCooldown && showItems.Any(i => i is IBaseItem)) continue;

            ImGui.Text(pair.Key);

            uint started = 0;
            foreach (var item in showItems)
            {
                if (started++ % count != 0)
                {
                    ImGui.SameLine();
                }
                DrawActionCooldown(item, width);
            }
        }
    }

    static readonly uint progressCol = ImGui.ColorConvertFloat4ToU32(new Vector4(0.6f, 0.6f, 0.6f, 0.7f));

    private static void DrawActionCooldown(IAction act, float width)
    {
        var recast = act.CD.RecastTimeOneChargeRaw;
        var elapsed = act.CD.RecastTimeElapsedRaw;
        var shouldSkip = recast < 3 && act is IBaseAction a && !a.Info.IsRealGCD;

        using var group = ImRaii.Group();
        if (!group) return;

        var winPos = ImGui.GetWindowPos();

        var r = -1f;
        if (Service.Config.UseOriginalCooldown)
        {
            r = !act.EnoughLevel ? 0 : recast == 0 || !act.CD.IsCoolingDown || shouldSkip ? 1 : elapsed / recast;
        }
        var pair = ControlWindow.DrawIAction(act, width, r, false);
        var pos = pair.Item1;
        var size = pair.Item2;

        if (!act.EnoughLevel)
        {
            if (!Service.Config.UseOriginalCooldown)
            {
                ImGui.GetWindowDrawList().AddRectFilled(new Vector2(pos.X, pos.Y) + winPos,
                    new Vector2(pos.X + size.X, pos.Y + size.Y) + winPos, progressCol);
            }
        }
        else if (act.CD.IsCoolingDown && !shouldSkip)
        {
            if (!Service.Config.UseOriginalCooldown)
            {
                var ratio = recast == 0 || !act.EnoughLevel ? 0 : (elapsed % recast) / recast;
                var startPos = new Vector2(pos.X + (size.X * ratio), pos.Y) + winPos;
                ImGui.GetWindowDrawList().AddRectFilled(startPos,
                    new Vector2(pos.X + size.X, pos.Y + size.Y) + winPos, progressCol);

                ImGui.GetWindowDrawList().AddLine(startPos, startPos + new Vector2(0, size.Y), black);
            }

            using var font = ImRaii.PushFont(ImGuiHelper.GetFont(Service.Config.CooldownFontSize));
            string time = recast == 0 ? "0" : ((int)(recast - (elapsed % recast)) + 1).ToString();
            var strSize = ImGui.CalcTextSize(time);
            var fontPos = new Vector2(pos.X + (size.X / 2) - (strSize.X / 2), pos.Y + (size.Y / 2) - (strSize.Y / 2)) + winPos;

            TextShade(fontPos, time);
        }

        if (act.EnoughLevel && act is IBaseAction bAct && bAct.CD.MaxCharges > 1)
        {
            for (int i = 0; i < bAct.CD.CurrentCharges; i++)
            {
                ImGui.GetWindowDrawList().AddCircleFilled(winPos + pos + ((i + 0.5f) * new Vector2(width / 5, 0)), width / 12, white);
            }
        }
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