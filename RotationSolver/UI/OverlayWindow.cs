using ECommons.DalamudServices;
using ECommons.GameHelpers;
using FFXIVClientStructs.FFXIV.Client.Graphics.Kernel;
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
        Svc.GameGui.WorldToScreen(pPosition, out var scrPos);

        float radius = target.HitboxRadius + Player.Object.HitboxRadius + 3;
        float rotation = target.Rotation;

        if (Service.Config.DrawMeleeOffset && DataCenter.StateType != StateCommandType.Cancel)
        {
            var offsetColor = new Vector3(0.8f, 0.3f, 0.2f);
            List<Vector2> pts1 = new(4 * COUNT);
            SectorPlots(ref pts1, pPosition, radius, 0, 4 * COUNT, 2 * Math.PI);

            List<Vector2> pts2 = new(4 * COUNT);
            SectorPlots(ref pts2, pPosition, radius + Service.Config.MeleeRangeOffset, 0, 4 * COUNT, 2 * Math.PI);

            DrawFill(pts1.ToArray(), pts2.ToArray(), offsetColor);

            DrawBoundary(pts1, offsetColor);
            DrawBoundary(pts2, offsetColor);
        }

        List<Vector2> pts = new(4 * COUNT);
        bool wrong = target.DistanceToPlayer() > 3;
        if (Service.Config.DrawPositional && !Player.Object.HasStatus(true, StatusID.TrueNorth))
        {
            var shouldPos = ActionUpdater.NextGCDAction?.EnemyPositional ?? EnemyPositional.None;

            switch (shouldPos)
            {
                case EnemyPositional.Flank:
                    pts.Add(scrPos);
                    SectorPlots(ref pts, pPosition, radius, Math.PI * 0.25 + rotation, COUNT);
                    pts.Add(scrPos);
                    SectorPlots(ref pts, pPosition, radius, Math.PI * 1.25 + rotation, COUNT);
                    pts.Add(scrPos);
                    break;
                case EnemyPositional.Rear:
                    pts.Add(scrPos);
                    SectorPlots(ref pts, pPosition, radius, Math.PI * 0.75 + rotation, COUNT);
                    pts.Add(scrPos);
                    break;
            }
            if (!wrong && pts.Count > 0)
            {
                wrong = shouldPos != target.FindEnemyPositional();
            }
        }
        if (pts.Count == 0 && Service.Config.DrawMeleeRange)
        {
            SectorPlots(ref pts, pPosition, radius, 0, 4 * COUNT, 2 * Math.PI);
        }

        if (pts.Count > 0) DrawRange(pts, wrong);
    }

    static void DrawFill(Vector2[] pts1, Vector2[] pts2, Vector3 color)
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

            DrawFill(new Vector2[] { p1, p2, p3, p4}, color);
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
        foreach (var pt in pts)
        {
            ImGui.GetWindowDrawList().PathLineTo(pt);
        }
        ImGui.GetWindowDrawList().PathFillConvex(ImGui.GetColorU32(new Vector4(color.X, color.Y, color.Z, Service.Config.AlphaInFill)));
    }

    static void DrawBoundary(IEnumerable<Vector2> pts, Vector3 color)
    {
        foreach (var pt in pts)
        {
            ImGui.GetWindowDrawList().PathLineTo(pt);
        }
        ImGui.GetWindowDrawList().PathStroke(ImGui.GetColorU32(new Vector4(color.X, color.Y, color.Z, 1f)), ImDrawFlags.None, 2);
    }

    private static void SectorPlots(ref List<Vector2> pts, Vector3 centre, float radius, double rotation, int segments, double round = Math.PI / 2)
    {
        var pts3 = new List<Vector3>();
        var step = round / segments;
        for (int i = 0; i < segments; i++)
        {
            pts3.Add(ChangePoint(centre, radius, rotation + i * step));
        }
        pts.AddRange(pts3.Select(p =>
        {
            Svc.GameGui.WorldToScreen(p, out var pos);
            return pos;
        }));
        pts.AddRange(GetPtsOnScreen(pts3));
    }

    private static Vector3 ChangePoint(Vector3 pt, double radius, double rotation)
    {
        var x = Math.Sin(rotation) * radius + pt.X;
        var z = Math.Cos(rotation) * radius + pt.Z;
        return new Vector3((float)x, pt.Y, (float)z);
    }

    public static IEnumerable<Vector2> GetPtsOnScreen(IEnumerable<Vector3> pts)
    {
        var cameraPts = pts.Select(WorldToCamera).ToArray();
        var changedPts = new List<Vector3>(cameraPts.Length * 2);

        for (int i = 0; i < cameraPts.Length; i++)
        {
            var pt1 = cameraPts[i];
            var pt2 = cameraPts[(i + 1) % cameraPts.Length];

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

        return changedPts.Where(p => p.Z > 0).Select(p =>
        {
            CameraToScreen(p, out var screenPos, out _);
            return screenPos;
        });
    }

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
        var f = Svc.GameGui.GetType().GetRuntimeFields().FirstOrDefault(f => f.Name == "getMatrixSingleton");
        var matrix = (MulticastDelegate)f.GetValue(Svc.GameGui);
        var matrixSingleton = (IntPtr)matrix.DynamicInvoke();

        var viewProjectionMatrix = *(Matrix4x4*)(matrixSingleton + 0x1b4);
        return Vector3.Transform(worldPos, viewProjectionMatrix);
    }

    public static unsafe bool CameraToScreen(Vector3 cameraPos, out Vector2 screenPos, out bool inView)
    {
        screenPos = new Vector2(cameraPos.X / MathF.Abs(cameraPos.Z), cameraPos.Y / MathF.Abs(cameraPos.Z));
        var windowPos = ImGuiHelpers.MainViewport.Pos;

        var device = Device.Instance();
        float width = device->Width;
        float height = device->Height;

        screenPos.X = (0.5f * width * (screenPos.X + 1f)) + windowPos.X;
        screenPos.Y = (0.5f * height * (1f - screenPos.Y)) + windowPos.Y;

        var inFront = cameraPos.Z > 0;
        inView = inFront &&
                 screenPos.X > windowPos.X && screenPos.X < windowPos.X + width &&
                 screenPos.Y > windowPos.Y && screenPos.Y < windowPos.Y + height;

        return inFront;
    }
}
