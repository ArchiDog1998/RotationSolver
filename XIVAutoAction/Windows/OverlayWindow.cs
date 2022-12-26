using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Interface;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using XIVAutoAttack.Actions;
using XIVAutoAttack.Actions.BaseAction;
using XIVAutoAttack.Data;
using XIVAutoAttack.Helpers;
using XIVAutoAttack.SigReplacers;
using XIVAutoAttack.Updaters;

namespace XIVAutoAttack.Windows
{
    internal static class OverlayWindow
    {
        internal static BattleChara EnemyLocationTarget;
        internal static EnemyLocation ShouldLocation { get; set; } = EnemyLocation.None;
        public static void Draw()
        {
            if (Service.GameGui == null || Service.ClientState.LocalPlayer == null || !Service.Configuration.UseOverlayWindow) return;

            //转场或者在看片片，不执行。
            if (Service.Conditions[Dalamud.Game.ClientState.Conditions.ConditionFlag.OccupiedInCutSceneEvent]
                || Service.Conditions[Dalamud.Game.ClientState.Conditions.ConditionFlag.BetweenAreas]
                || Service.Conditions[Dalamud.Game.ClientState.Conditions.ConditionFlag.BetweenAreas51]) return;

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
            DrawTarget();
            DrawMoveTarget();

            ImGui.PopStyleVar();
            ImGui.End();

            ImGui.PopID();
        }

        private static void DrawTarget()
        {
            if (!Service.Configuration.ShowTarget) return;

            if (ActionUpdater.NextAction is not BaseAction act) return;

            if (act.Target == null) return;

            if (act.Target != Service.ClientState.LocalPlayer)
            {
                var c = Service.Configuration.TargetColor;
                var Tcolor = ImGui.GetColorU32(new Vector4(c.X, c.Y, c.Z, 1));
                DrawTarget(act.Target, Tcolor, 8, out _);
            }

            if (TargetUpdater.HostileTargets.Contains(act.Target))
            {
                var c = Service.Configuration.SubTargetColor;
                var Scolor = ImGui.GetColorU32(new Vector4(c.X, c.Y, c.Z, 1));

                foreach (var t in TargetUpdater.HostileTargets)
                {
                    if (t == act.Target) continue;
                    if (act.CanGetTarget(act.Target, t))
                    {
                        DrawTarget(t, Scolor, 5, out _);
                    }
                }
            }
        }

        private static void DrawMoveTarget()
        {
            if (!Service.Configuration.ShowMoveTarget) return;

            var c = Service.Configuration.MovingTargetColor;
            var color = ImGui.GetColorU32(new Vector4(c.X, c.Y, c.Z, 1));

#if DEBUG
            Service.GameGui.WorldToScreen(Service.ClientState.LocalPlayer.Position, out var plp);
            var calHealth = (double)ObjectHelper.GetHealthFromMulty(1);
            foreach (var t in TargetUpdater.AllTargets)
            {
                if (Service.GameGui.WorldToScreen(t.Position, out var p))
                {
                    ImGui.GetWindowDrawList().AddText(p, color, $"Boss Ratio (Max): {t.MaxHp / calHealth:F2}\nDying Ratio (Current): {t.CurrentHp / calHealth:F2}");
                }
            }
#endif
            var tar = IconReplacer.RightNowCombo?.MoveTarget;
            if (tar == null || tar == Service.ClientState.LocalPlayer) return;

            DrawTarget(tar, color, 8, out var scrPos);

            if (Service.GameGui.WorldToScreen(Service.ClientState.LocalPlayer.Position, out var plyPos))
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
        private static void DrawLocation()
        {


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
