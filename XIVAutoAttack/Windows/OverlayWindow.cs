using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Interface;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Numerics;
using XIVAutoAttack.Actions;
using XIVAutoAttack.Data;
using XIVAutoAttack.Helpers;

namespace XIVAutoAttack.Windows
{
    internal static class OverlayWindow
    {
        internal static BattleChara EnemyLocationTarget;
        internal static EnemyLocation ShouldLocation { get; set; } = EnemyLocation.None;
        public static void Draw()
        {
            if (Service.GameGui == null || Service.ClientState.LocalPlayer == null || !Service.Configuration.UseOverlayWindow) return;

            ImGui.PushID("AutoAttackOverlay");

            ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new Vector2(0, 0));

            ImGuiHelpers.SetNextWindowPosRelativeMainViewport(new Vector2(0, 0));
            ImGui.Begin("Ring",
                  ImGuiWindowFlags.NoBackground | ImGuiWindowFlags.NoBringToFrontOnFocus
                | ImGuiWindowFlags.NoDecoration | ImGuiWindowFlags.NoDocking
                | ImGuiWindowFlags.NoFocusOnAppearing | ImGuiWindowFlags.NoInputs
                );

            ImGui.SetWindowSize(ImGui.GetIO().DisplaySize);

            DrawLocation();

            ImGui.PopStyleVar();
            ImGui.End();

            ImGui.PopID();
        }

        private static void DrawLocation()
        {
            const int COUNT = 20;

            if (EnemyLocationTarget == null || !Service.Configuration.ShowLocationWrong) return;
            if (Service.ClientState.LocalPlayer.HasStatus(true, StatusID.TrueNorth)) return;
            if (ShouldLocation is EnemyLocation.None or EnemyLocation.Front) return;

            float radius = EnemyLocationTarget.HitboxRadius + 3.5f;
            float rotation = EnemyLocationTarget.Rotation;

            Vector3 pPosition = EnemyLocationTarget.Position;
            if (!Service.GameGui.WorldToScreen(pPosition, out var scrPos)) return;



            List<Vector2> pts = new List<Vector2>(2 * COUNT + 2);

            pts.Add(scrPos);
            switch (ShouldLocation)
            {
                case EnemyLocation.Side:
                    SectorPlots(ref pts, pPosition, radius, Math.PI * 0.25 + rotation, COUNT);
                    pts.Add(scrPos);
                    SectorPlots(ref pts, pPosition, radius, Math.PI * 1.25 + rotation, COUNT);
                    break;
                case EnemyLocation.Back:
                    SectorPlots(ref pts, pPosition, radius, Math.PI * 0.75 + rotation, COUNT);
                    break;
                default:
                    return;
            }
            pts.Add(scrPos);

            bool wrong = ShouldLocation != EnemyLocationTarget.FindEnemyLocation();
            var color = wrong ? new Vector3(0.3f, 0.8f, 0.2f) : new Vector3(1, 1, 1);

            pts.ForEach(pt => ImGui.GetWindowDrawList().PathLineTo(pt));
            ImGui.GetWindowDrawList().PathStroke(ImGui.GetColorU32(new Vector4(color.X, color.Y, color.Z, 1f)), ImDrawFlags.None, 3);
            pts.ForEach(pt => ImGui.GetWindowDrawList().PathLineTo(pt));
            ImGui.GetWindowDrawList().PathFillConvex(ImGui.GetColorU32(new Vector4(color.X, color.Y, color.Z, 0.2f)));
        }

        private static void SectorPlots(ref List<Vector2> pts, Vector3 centre, float radius, double rotation, int segments)
        {
            var step = Math.PI / 2 / segments;
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
}
