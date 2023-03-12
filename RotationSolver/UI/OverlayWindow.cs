using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Interface;
using FFXIVClientStructs.FFXIV.Client.Game.Control;
using ImGuiNET;
using RotationSolver.Actions;
using RotationSolver.Actions.BaseAction;
using RotationSolver.Basic;
using RotationSolver.Data;
using RotationSolver.Helpers;
using RotationSolver.SigReplacers;
using RotationSolver.Updaters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace RotationSolver.Windows;

internal static class OverlayWindow
{
    internal static IBaseAction MeleeAction { get; set; }
    public static void Draw()
    {
        if (Service.GameGui == null || Service.Player == null || !Service.Config.UseOverlayWindow) return;

        if (Service.Conditions[Dalamud.Game.ClientState.Conditions.ConditionFlag.OccupiedInCutSceneEvent]
            || Service.Conditions[Dalamud.Game.ClientState.Conditions.ConditionFlag.BetweenAreas]
            || Service.Conditions[Dalamud.Game.ClientState.Conditions.ConditionFlag.BetweenAreas51]) return;

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

        if (act.Target != Service.Player)
        {
            var c = Service.Config.TargetColor;
            var Tcolor = ImGui.GetColorU32(new Vector4(c.X, c.Y, c.Z, 1));
            DrawTarget(act.Target, Tcolor, 8, out _);
        }

        if (DataCenter.HostileTargets.Contains(act.Target))
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
        if(!Service.Config.ShowHealthRatio) return;

        var calHealth = (double)ObjectHelper.GetHealthFromMulty(1);
        foreach (BattleChara t in DataCenter.AllTargets)
        {
            if (t == null) continue;
            if (Service.GameGui.WorldToScreen(t.Position, out var p))
            {
                ImGui.GetWindowDrawList().AddText(p, HealthRatioColor, $"Health Ratio: {t.CurrentHp / calHealth:F2} / {t.MaxHp / calHealth:F2}");
            }
        }
    }

    private unsafe static void DrawMoveTarget()
    {
        if (!Service.Config.ShowMoveTarget) return;

        var c = Service.Config.MovingTargetColor;
        var color = ImGui.GetColorU32(new Vector4(c.X, c.Y, c.Z, 1));

        var tar = RotationUpdater.RightNowRotation?.MoveTarget;
        if (tar == null || tar == Service.Player) return;

        DrawTarget(tar, color, 8, out var scrPos);

        if (Service.GameGui.WorldToScreen(Service.Player.Position, out var plyPos))
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
        if (Service.GameGui.WorldToScreen(tar.Position, out scrPos))
        {
            ImGui.GetWindowDrawList().AddCircle(scrPos, radius, color, COUNT, radius * 0.8f);
        }
    }

    const int COUNT = 20;
    private static void DrawPositional()
    {
        if (MeleeAction == null) return;

        Vector3 pPosition = MeleeAction.Target.Position;
        Service.GameGui.WorldToScreen(pPosition, out var scrPos);

        float radius = MeleeAction.Target.HitboxRadius + Service.Player.HitboxRadius + 3;
        float rotation = MeleeAction.Target.Rotation;
        bool wrong = MeleeAction.Target.DistanceToPlayer() > 3;
        List<Vector2> pts = new List<Vector2>(4 * COUNT);

        if(Service.Config.DrawPositional && !Service.Player.HasStatus(true, StatusID.TrueNorth))
        {
            var shouldPos = MeleeAction.EnemyPositional;

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
                wrong = shouldPos != MeleeAction.Target.FindEnemyPositional();
            }
        }
        if(pts.Count == 0 && Service.Config.DrawMeleeRange)
        {
            SectorPlots(ref pts, pPosition, radius, 0, 4 * COUNT, 2 * Math.PI);
        }

        if(pts.Count > 0) DrawRange(pts, wrong);
    }

    static void DrawRange(List<Vector2> pts, bool wrong)
    {
        var color = wrong ? new Vector3(0.3f, 0.8f, 0.2f) : new Vector3(1, 1, 1);

        pts.ForEach(pt => ImGui.GetWindowDrawList().PathLineTo(pt));
        ImGui.GetWindowDrawList().PathFillConvex(ImGui.GetColorU32(new Vector4(color.X, color.Y, color.Z, 0.15f)));

        pts.ForEach(pt => ImGui.GetWindowDrawList().PathLineTo(pt));
        ImGui.GetWindowDrawList().PathStroke(ImGui.GetColorU32(new Vector4(color.X, color.Y, color.Z, 1f)), ImDrawFlags.None, 2);
    }

    private static void SectorPlots(ref List<Vector2> pts, Vector3 centre, float radius, double rotation, int segments, double round = Math.PI / 2)
    {
        var step = round / segments;
        for (int i = 0; i <= segments; i++)
        {
            Service.GameGui.WorldToScreen(ChangePoint(centre, radius, rotation + i * step), out var pt);
            pts.Add(pt);
        }
    }

    private static Vector3 ChangePoint(Vector3 pt, double radius, double rotation)
    {
        var x = Math.Sin(rotation) * radius + pt.X;
        var z = Math.Cos(rotation) * radius + pt.Z;
        return new Vector3((float)x, pt.Y, (float)z);
    }
}
