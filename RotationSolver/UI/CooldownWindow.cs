using Dalamud.Interface.Utility.Raii;
using RotationSolver.Updaters;

namespace RotationSolver.UI;

internal class CooldownWindow() : CtrlWindow(nameof(CooldownWindow))
{
    public override void Draw()
    {
        if (DataCenter.RightNowRotation == null) return;

        var width = Service.Config.CooldownWindowIconSize;
        var count = Math.Max(1, (int)MathF.Floor(ImGui.GetColumnWidth() / (width * (1 + 6 / 82) + ImGui.GetStyle().ItemSpacing.X)));

        if (RotationUpdater.AllGroupedActions == null) return;

        foreach (var pair in RotationUpdater.AllGroupedActions)
        {
            var showItems = pair.OrderBy(a => a.SortKey).Where(a => a.IsInCooldown
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
                ControlWindow.DrawIAction(item, width, 1f);
            }
        }
    }

}