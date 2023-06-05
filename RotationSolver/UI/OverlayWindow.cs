using Dalamud.Logging;
using ECommons.DalamudServices;
using ECommons.GameHelpers;
using FFXIVClientStructs.FFXIV.Client.Graphics.Kernel;
using FFXIVClientStructs.FFXIV.Client.Graphics.Scene;
using FFXIVClientStructs.FFXIV.Common.Component.BGCollision;
using RotationSolver.Updaters;

namespace RotationSolver.UI;

internal static class OverlayWindow
{
    public static void Draw()
    {
        if (!Player.Available || !Service.Config.UseOverlayWindow) return;

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
        DrawMoveTarget();
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

    private unsafe static void DrawMoveTarget()
    {
        //if (!Service.Config.ShowMoveTarget) return;

        //var c = Service.Config.MovingTargetColor;
        //var color = ImGui.GetColorU32(new Vector4(c.X, c.Y, c.Z, 1));

        //var tar = CustomRotation.MoveTarget;
        //if (tar == null || tar == Player.Object) return;

        //DrawTarget(tar, color, 8, out var scrPos);

        //if (Svc.GameGui.WorldToScreen(Player.Object.Position, out var plyPos))
        //{
        //    var dir = scrPos - plyPos;

        //    dir /= dir.Length();
        //    dir *= 50;
        //    var end = dir + plyPos;
        //    ImGui.GetWindowDrawList().AddLine(plyPos, end, color, 3);

        //    var radius = 3;

        //    ImGui.GetWindowDrawList().AddCircle(plyPos, radius, color, COUNT, radius * 2);
        //}
    }

    private static void DrawTarget(BattleChara tar, uint color, float radius, out Vector2 scrPos)
    {
        if (Svc.GameGui.WorldToScreen(tar.Position, out scrPos))
        {
            ImGui.GetWindowDrawList().AddCircle(scrPos, radius, color, COUNT, radius * 0.8f);
        }
    }

    const int COUNT = 20;

    private static IEnumerable<Vector2[]> ConvexPoints(Vector2[] points)
    {
        if(points.Length < 4)
        {
            return new Vector2[][] { points };
        }

        int breakIndex = -1;
        Vector2 dir = Vector2.Zero;
        for (int i = 0; i < points.Length; i++)
        {
            var pt1 = points[(i - 1 + points.Length) % points.Length];
            var pt2 = points[i];
            var pt3 = points[(i + 1) % points.Length];

            var vec1 = pt2 - pt1;
            var vec2 = pt3 - pt2;
            if(Vector3.Cross(new Vector3(vec1.X, vec1.Y, 0), new Vector3(vec2.X, vec2.Y, 0)).Z > 0)
            {
                breakIndex = i;
                dir = vec1 / vec1.Length() - vec2 / vec2.Length();
                dir /= dir.Length();
                break;
            }
        }

        if (breakIndex < 0)
        {
            return new Vector2[][] { points };
        }
        else
        {
            try
            {
                var pt = points[breakIndex];
                var index = 0;
                double maxValue = double.MinValue;
                for (int i = 0; i < points.Length; i++)
                {
                    if (Math.Abs(i - breakIndex) < 2) continue;
                    if (Math.Abs(i + points.Length - breakIndex) < 2) continue;
                    if (Math.Abs(i - points.Length - breakIndex) < 2) continue;
                    var d = points[i] - pt;
                    d /= d.Length();

                    var angle = Vector2.Dot(d, dir);

                    if (angle > maxValue)
                    {
                        maxValue = angle;
                        index = i;
                    }
                }

                var minIndex = Math.Min(breakIndex, index);
                var maxIndex = Math.Max(breakIndex, index);

                var list1 = new List<Vector2>(points.Length);
                var list2 = new List<Vector2>(points.Length);
                for (int i = 0; i < points.Length; i++)
                {
                    if (i <= minIndex || i >= maxIndex)
                    {
                        list1.Add(points[i]);
                    }

                    if (i >= minIndex && i <= maxIndex)
                    {
                        list2.Add(points[i]);
                    }
                }

                return ConvexPoints(list1.ToArray()).Union(ConvexPoints(list2.ToArray())).Where(l => l.Count() > 2);
            }
            catch (Exception ex)
            {
                PluginLog.Warning(ex, "Bad at drawing");
                return new Vector2[][] { points };
            }
        }
    }
}
