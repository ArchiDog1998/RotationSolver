using Dalamud.Interface;
using Dalamud.Interface.Windowing;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using XIVAutoAttack.Actions;
using XIVAutoAttack.Combos.CustomCombo;
using XIVAutoAttack.Helpers;
using XIVAutoAttack.Data;
using System.Xml.Linq;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using XIVAutoAttack.SigReplacers;
using XIVAutoAttack.Updaters;

namespace XIVAutoAttack.Windows
{
    internal static class OverlayWindow
    {

        public static void Draw()
        {
            if (Service.GameGui == null || !Service.Configuration.UseOverlayWindow) return;

            ImGui.PushID("AutoAttackOverlay");

            ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new Vector2(0, 0));

            ImGuiHelpers.SetNextWindowPosRelativeMainViewport(new Vector2(0, 0));
            ImGui.Begin("Ring", ImGuiWindowFlags.NoInputs | ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoBackground);

            ImGui.SetWindowSize(ImGui.GetIO().DisplaySize);

            DrawLocation();
            UpdateHightLight();

            ImGui.PopStyleVar();
            ImGui.End();

            ImGui.PopID();
        }

        private static void DrawLocation()
        {
            const int COUNT = 20;

            if (CustomCombo.EnemyLocationTarget == null || !Service.Configuration.SayoutLocationWrong) return;
            if (Service.ClientState.LocalPlayer.HaveStatus(ObjectStatus.TrueNorth)) return;
            if (CustomCombo.ShouldLocation is EnemyLocation.None or EnemyLocation.Front) return;

            float radius = CustomCombo.EnemyLocationTarget.HitboxRadius + 3.5f;
            float rotation = CustomCombo.EnemyLocationTarget.Rotation;

            Vector3 pPosition = CustomCombo.EnemyLocationTarget.Position;
            if (!Service.GameGui.WorldToScreen(pPosition, out var scrPos)) return;



            List<Vector2> pts = new List<Vector2>(2 * COUNT + 2);

            pts.Add(scrPos);
            switch (CustomCombo.ShouldLocation)
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

            bool wrong = CustomCombo.ShouldLocation != CustomCombo.EnemyLocationTarget.FindEnemyLocation();
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

        private static void UpdateHightLight()
        {
            if (ActionUpdater.NextAction == null || !Service.Configuration.TeachingMode) return;
            HigglightAtionBar(ActionUpdater.NextAction.AdjustedID);
        }

        static readonly string[] _barsName = new string[]
        {
            "_ActionCross",
            "_ActionDoubleCrossL",
            "_ActionDoubleCrossR",
            "_ActionBar",
            "_ActionBar01",
            "_ActionBar02",
            "_ActionBar03",
            "_ActionBar04",
            "_ActionBar05",
            "_ActionBar06",
            "_ActionBar07",
            "_ActionBar08",
            "_ActionBar09",
        };

        internal static unsafe void PulseAtionBar(uint actionID)
        {

            LoopAllSlotBar(bar =>
            {
                for (int i = 0; i < bar->SlotCount; i++)
                {
                    if (IsActionSlotRight(bar->ActionBarSlots[i], actionID))
                    {
                        bar->PulseActionBarSlot(i);
                        //键盘按下效果音效
                        UIModule.PlaySound(12, 0, 0, 0);
                        return true;
                    }
                }
                return false;
            });
        }

        private static unsafe void HigglightAtionBar(uint actionID)
        {
            var color = ImGui.GetColorU32(new Vector4(0.7f, 1f, 0.4f, 1));
            var offset = 2;

            LoopAllSlotBar(bar =>
            {
                var baseX = (float)bar->AtkUnitBase.X;
                var baseY = (float)bar->AtkUnitBase.Y;

                int i = 0;
                foreach (var slot in bar->Slot)
                {
                    if (IsActionSlotRight(slot, actionID))
                    {
                        var realSlot = (AtkComponentNode*)bar->AtkUnitBase.UldManager.NodeList[20 - i];
                        var dropDown = (AtkComponentNode*)realSlot->Component->UldManager.NodeList[0];

                        var x = realSlot->AtkResNode.X + dropDown->AtkResNode.X + baseX;
                        var y = realSlot->AtkResNode.Y + dropDown->AtkResNode.Y + baseY;

                        if (!dropDown->AtkResNode.IsVisible) continue;

                        var width = (float)dropDown->AtkResNode.Width;
                        var height = (float)dropDown->AtkResNode.Height;

                        ImGui.GetWindowDrawList().AddRect(new Vector2(x - offset, y - offset), new Vector2(x + width + offset, y + height + offset), 
                            color, 8, ImDrawFlags.RoundCornersAll, 5);
                    }
                    i++;
                }
                return false;
            });
        }

        private static bool IsActionSlotRight(ActionBarSlot slot, uint actionID)
        {
            if (slot.ActionId == IconReplacer.KeyActionID.ID) return false;
            return Service.IconReplacer.OriginalHook((uint)slot.ActionId) == actionID;
        }

        unsafe delegate bool ActionBarAction(AddonActionBarBase* bar);
        private static unsafe void LoopAllSlotBar(ActionBarAction doingSomething)
        {
            foreach (var name in _barsName)
            {
                var actBar = Service.GameGui.GetAddonByName(name, 1);
                if (actBar == IntPtr.Zero) continue;
                if (doingSomething((AddonActionBarBase*)actBar)) return;
            }
        }


    }
}
