using ECommons.DalamudServices;
using ECommons.GameHelpers;
using RotationSolver.Updaters;

namespace RotationSolver.UI;

internal static class OverlayWindow
{
    public static void Draw()
    {
        if (!Player.Available) return;

        if (Svc.Condition[Dalamud.Game.ClientState.Conditions.ConditionFlag.OccupiedInCutSceneEvent]
            || Svc.Condition[Dalamud.Game.ClientState.Conditions.ConditionFlag.BetweenAreas]
            || Svc.Condition[Dalamud.Game.ClientState.Conditions.ConditionFlag.BetweenAreas51]) return;

        ImGui.PushID("AutoActionOverlay");

        ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new Vector2(0, 0));

        ImGuiHelpers.SetNextWindowPosRelativeMainViewport(new Vector2(0, 0));
        ImGui.Begin("Ring",
              ImGuiWindowFlags.NoBackground | ImGuiWindowFlags.NoBringToFrontOnFocus
            | ImGuiWindowFlags.NoDecoration | ImGuiWindowFlags.NoDocking
            | ImGuiWindowFlags.NoFocusOnAppearing | ImGuiWindowFlags.NoInputs
            );

        ImGui.SetWindowSize(ImGui.GetIO().DisplaySize);

        DrawTarget();
        DrawHealthRatio();

        ImGui.PopStyleVar();
        ImGui.End();

        ImGui.PopID();
    }

    private static void DrawTarget()
    {
        if (!Service.Config.ShowTarget) return;

        if (ActionUpdater.NextAction is not BaseAction act) return;

        if (act.Target == null) return;

        if (act.Target != Player.Object)
        {
            var c = Service.Config.TargetColor;
            var Tcolor = ImGui.GetColorU32(new Vector4(c.X, c.Y, c.Z, 1));
            DrawTarget(act.Target, Tcolor, 8, out _);
        }

        if (DataCenter.HostileTargets.Contains(act.Target) || act.Target == Player.Object && !act.IsFriendly)
        {
            var c = Service.Config.SubTargetColor;
            var Scolor = ImGui.GetColorU32(new Vector4(c.X, c.Y, c.Z, 1));

            foreach (var t in DataCenter.HostileTargets)
            {
                if (t == act.Target) continue;
                if (act.CanGetTarget(act.Target, t))
                {
                    DrawTarget(t, Scolor, 5, out _);
                }
            }
        }
    }

    static readonly uint HealthRatioColor = ImGui.GetColorU32(new Vector4(0, 1, 0.8f, 1));
    private static void DrawHealthRatio()
    {
        if (!Service.Config.ShowHealthRatio) return;

        var calHealth = (double)ObjectHelper.GetHealthFromMulty(1);
        foreach (GameObject t in DataCenter.AllTargets)
        {
            if (t is not BattleChara b) continue;
            if (Svc.GameGui.WorldToScreen(t.Position, out var p))
            {
                ImGui.GetWindowDrawList().AddText(p, HealthRatioColor, $"Health Ratio: {b.CurrentHp / calHealth:F2} / {b.MaxHp / calHealth:F2}");
            }
        }
    }

    private static void DrawTarget(BattleChara tar, uint color, float radius, out Vector2 scrPos)
    {
        if (Svc.GameGui.WorldToScreen(tar.Position, out scrPos))
        {
            ImGui.GetWindowDrawList().AddCircle(scrPos, radius, color, COUNT, radius * 0.8f);
        }
    }

    const int COUNT = 20;
}
