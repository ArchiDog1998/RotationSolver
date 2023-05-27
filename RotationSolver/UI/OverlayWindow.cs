using Dalamud.Interface.Colors;
using Dalamud.Logging;
using ECommons.DalamudServices;
using ECommons.GameHelpers;
using FFXIVClientStructs.FFXIV.Client.Game.Event;
using FFXIVClientStructs.FFXIV.Client.Graphics;
using FFXIVClientStructs.FFXIV.Client.Graphics.Kernel;
using FFXIVClientStructs.FFXIV.Client.Graphics.Scene;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;
using FFXIVClientStructs.FFXIV.Common.Component.BGCollision;
using RotationSolver.Updaters;
using System.Drawing;

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

        DrawPositional();
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
        if (!Service.Config.ShowMoveTarget) return;

        var c = Service.Config.MovingTargetColor;
        var color = ImGui.GetColorU32(new Vector4(c.X, c.Y, c.Z, 1));

        var tar = CustomRotation.MoveTarget;
        if (tar == null || tar == Player.Object) return;

        DrawTarget(tar, color, 8, out var scrPos);

        if (Svc.GameGui.WorldToScreen(Player.Object.Position, out var plyPos))
        {
            var dir = scrPos - plyPos;

            dir /= dir.Length();
            dir *= 50;
            var end = dir + plyPos;
            ImGui.GetWindowDrawList().AddLine(plyPos, end, color, 3);

            var radius = 3;

            ImGui.GetWindowDrawList().AddCircle(plyPos, radius, color, COUNT, radius * 2);
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
    private static void DrawPositional()
    {
        if (!Player.Object.IsJobCategory(JobRole.Tank)
            && !Player.Object.IsJobCategory(JobRole.Melee)) return;

        var target = ActionUpdater.NextGCDAction?.Target?.IsNPCEnemy() ?? false
            ? ActionUpdater.NextGCDAction.Target
            : Svc.Targets.Target?.IsNPCEnemy() ?? false
            ? Svc.Targets.Target
            : null;

        if (target == null) return;

        if (ActionUpdater.NextGCDAction != null
            && !ActionUpdater.NextGCDAction.IsSingleTarget) return;

        Vector3 pPosition = target.Position;

        float radius = target.HitboxRadius + Player.Object.HitboxRadius + 3;
        float rotation = target.Rotation;

        if (Service.Config.DrawMeleeOffset && DataCenter.StateType != StateCommandType.Cancel)
        {
            var offsetColor = new Vector3(0.8f, 0.3f, 0.2f);
            var pts1 = SectorPlots(pPosition, radius, 0, 4 * COUNT, 2 * Math.PI);

            var pts2 = SectorPlots(pPosition, radius + Service.Config.MeleeRangeOffset, 0, 4 * COUNT, 2 * Math.PI);

            DrawFill(pts1.ToArray(), pts2.ToArray(), offsetColor);

            DrawBoundary(pts1, offsetColor);
            DrawBoundary(pts2, offsetColor);
        }

        bool wrong = target.DistanceToPlayer() > 3;

        var shouldPos = ActionUpdater.NextGCDAction?.EnemyPositional ?? EnemyPositional.None;
        if (!wrong && shouldPos is EnemyPositional.Rear or EnemyPositional.Flank)
        {
            wrong = shouldPos != target.FindEnemyPositional();
        }

        switch (shouldPos)
        {
            case EnemyPositional.Flank when Service.Config.DrawPositional && CanDrawPositional(target):
                DrawRange(ClosePoints(GetPtsOnScreen(SectorPlots(pPosition, radius, Math.PI * 0.25 + rotation, COUNT).Append(pPosition))), wrong);
                DrawRange(ClosePoints(GetPtsOnScreen(SectorPlots(pPosition, radius, Math.PI * 1.25 + rotation, COUNT).Append(pPosition))), wrong);
                break;
            case EnemyPositional.Rear when Service.Config.DrawPositional && CanDrawPositional(target):
                DrawRange(ClosePoints(GetPtsOnScreen(SectorPlots(pPosition, radius, Math.PI * 0.75 + rotation, COUNT).Append(pPosition))), wrong);
                break;

            default:
                if (Service.Config.DrawMeleeRange)
                {
                    DrawRange(ClosePoints(GetPtsOnScreen(SectorPlots(pPosition, radius, 0, 4 * COUNT, 2 * Math.PI))), wrong);
                }
                break;
        }
    }

    private static bool CanDrawPositional(GameObject target)
    {
        return !Player.Object.HasStatus(true, CustomRotation.TrueNorth.StatusProvide) && target.HasPositional();
    }

    static void DrawFill(Vector3[] pts1, Vector3[] pts2, Vector3 color)
    {
        if (pts1 == null || pts2 == null) return;
        if (pts1.Length != pts2.Length) return;
        var length = pts1.Length;
        for (int i = 0; i < length; i++)
        {
            var p1 = pts1[i];
            var p2 = pts2[i];
            var p3 = pts2[(i + 1) % length];
            var p4 = pts1[(i + 1) % length];

            DrawFill(GetPtsOnScreen(new Vector3[] { p1, p2, p3, p4}), color);
        }
    }

    static void DrawRange(IEnumerable<Vector2> pts, bool wrong)
    {
        var color = wrong ? new Vector3(0.3f, 0.8f, 0.2f) : new Vector3(1, 1, 1);
        DrawFill(pts, color);
        DrawBoundary(pts, color);
    }

    static void DrawFill(IEnumerable<Vector2> pts, Vector3 color)
    {
        foreach (var set in ConvexPoints(pts.ToArray()))
        {
            foreach (var pt in set)
            {
                ImGui.GetWindowDrawList().PathLineTo(pt);
            }
            ImGui.GetWindowDrawList().PathFillConvex(ImGui.GetColorU32(new Vector4(color.X, color.Y, color.Z, Service.Config.AlphaInFill)));
        }
    }

    static void DrawBoundary(IEnumerable<Vector3> pts, Vector3 color)
    {
        DrawBoundary(ClosePoints(GetPtsOnScreen(pts)), color);
    }

    static void DrawBoundary(IEnumerable<Vector2> pts, Vector3 color)
    {
        foreach (var pt in pts)
        {
            ImGui.GetWindowDrawList().PathLineTo(pt);
        }
        ImGui.GetWindowDrawList().PathStroke(ImGui.GetColorU32(new Vector4(color.X, color.Y, color.Z, 1f)), ImDrawFlags.None, 2);
    }

    private static List<Vector3> SectorPlots(Vector3 center, float radius, double rotation, int segments, double round = Math.PI / 2)
    {
        var pts = new List<Vector3>(4 * COUNT);
        var step = round / segments;
        for (int i = 0; i < segments; i++)
        {
            pts.Add(ChangePoint(center, radius, rotation + i * step));
        }
        return pts;
    }

    private static Vector3 ChangePoint(Vector3 pt, double radius, double rotation)
    {
        var x = Math.Sin(rotation) * radius + pt.X;
        var z = Math.Cos(rotation) * radius + pt.Z;
        return new Vector3((float)x, pt.Y, (float)z);
    }

    public static IEnumerable<Vector2> ClosePoints(IEnumerable<Vector2> pts)
    {
        if(pts.Count() < 3) return pts;
        if (Vector2.Distance(pts.First(), pts.Last()) < 0.001f) return pts;
        pts = pts.Append(pts.First());
        return pts;
    }

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

    public static unsafe IEnumerable<Vector2> GetPtsOnScreen(IEnumerable<Vector3> pts)
    {
        var camera = (Vector3)CameraManager.Instance()->CurrentCamera->Object.Position;
        var cameraPts = ProjectPtsOnGround(pts, 3)
            //.Where(p =>
            //{
            //    var vec = p - camera;
            //    var dis = vec.Length() - 0.1f;
            //    return !BGCollisionModule.Raycast(camera, vec, out _, dis);
            //})
            .Select(WorldToCamera).ToArray();
        var changedPts = ChangePtsBehindCamera(cameraPts);

        return changedPts.Select(CameraToScreen);
    }

    private static IEnumerable<Vector3> ChangePtsBehindCamera(Vector3[] cameraPts)
    {
        var changedPts = new List<Vector3>(cameraPts.Length * 2);

        for (int i = 0; i < cameraPts.Length; i++)
        {
            var pt1 = cameraPts[(i - 1 + cameraPts.Length) % cameraPts.Length];
            var pt2 = cameraPts[i];

            if (pt1.Z > 0 && pt2.Z <= 0)
            {
                GetPointOnPlane(pt1, ref pt2);
            }
            if (pt2.Z > 0 && pt1.Z <= 0)
            {
                GetPointOnPlane(pt2, ref pt1);
            }

            if (changedPts.Count > 0 && Vector3.Distance(pt1, changedPts[changedPts.Count - 1]) > 0.001f)
            {
                changedPts.Add(pt1);
            }

            changedPts.Add(pt2);
        }

        return changedPts.Where(p => p.Z > 0);
    }

    private static IEnumerable<Vector3> ProjectPtsOnGround(IEnumerable<Vector3> pts, float height)
        => pts.Select(pt =>
        {
            Vector3? result;
            var pUp = pt + Vector3.UnitY * height;
            if (BGCollisionModule.Raycast(pt + Vector3.UnitY * 100, -Vector3.UnitY, out var hit))
            {
                var p = hit.Point;
                p.Y = Math.Max(p.Y, pt.Y - height);
                p.Y = Math.Min(p.Y, pt.Y + height);
                result = p;
            }
            else
            {
                result = null;
            }
            return result;
        }).Where(pt => pt.HasValue).Select(pt => pt.Value);

    const float PLANE_Z = 0.001f;
    public static void GetPointOnPlane(Vector3 front, ref Vector3 back)
    {
        if (front.Z < 0) return;
        if (back.Z > 0) return;

        var ratio = (PLANE_Z - back.Z) / (front.Z - back.Z);
        back.X = (front.X - back.X) * ratio + back.X;
        back.Y = (front.Y - back.Y) * ratio + back.Y;
        back.Z = PLANE_Z;
    }

    public static unsafe Vector3 WorldToCamera(Vector3 worldPos)
    {
        var camera = CameraManager.Instance()->CurrentCamera;
        return Vector3.Transform(worldPos, camera->ViewMatrix * camera->RenderCamera->ProjectionMatrix);
    }

    public static unsafe Vector2 CameraToScreen(Vector3 cameraPos)
    {
        var screenPos = new Vector2(cameraPos.X / MathF.Abs(cameraPos.Z), cameraPos.Y / MathF.Abs(cameraPos.Z));
        var windowPos = ImGuiHelpers.MainViewport.Pos;

        var device = Device.Instance();
        float width = device->Width;
        float height = device->Height;

        screenPos.X = (0.5f * width * (screenPos.X + 1f)) + windowPos.X;
        screenPos.Y = (0.5f * height * (1f - screenPos.Y)) + windowPos.Y;

        return screenPos;
    }
}
