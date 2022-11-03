using Dalamud.Game.Gui.Dtr;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using FFXIVClientStructs.FFXIV.Client.Graphics;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using Lumina.Excel.GeneratedSheets;
using System;
using XIVAutoAttack.Data;
using XIVAutoAttack.Helpers;

namespace XIVAutoAttack.Updaters
{
    internal static class PreviewUpdater
    {
        internal static void UpdatePreview()
        {
            UpdateEntry();
            UpdateCastBar();
            UpdateHightLight();
        }

        static DtrBarEntry dtrEntry;
        private static void UpdateEntry()
        {
            if (Service.Configuration.UseDtr && CommandController.StateString != null)
            {
                if (dtrEntry == null)
                {
                    dtrEntry = Service.DtrBar.Get("Auto Attack");
                }
                dtrEntry.Shown = true;
                dtrEntry.Text = new SeString(
                    new IconPayload(BitmapFontIcon.DPS),
                    new TextPayload(CommandController.StateString)
                    );
            }
            else if (dtrEntry != null)
            {
                dtrEntry.Shown = false;
            }
        }

        private static unsafe void UpdateCastBar()
        {
            ByteColor redColor = new ByteColor() { A = 255, R = 120, G = 0, B = 60 };
            ByteColor greenColor = new ByteColor() { A = 255, R = 60, G = 120, B = 30 };

            bool canMove = !Service.Conditions[Dalamud.Game.ClientState.Conditions.ConditionFlag.OccupiedInEvent]
                && !Service.Conditions[Dalamud.Game.ClientState.Conditions.ConditionFlag.Casting];

            ByteColor c = canMove && Service.Configuration.CheckForCasting ? greenColor : redColor;
            MovingUpdater.IsMoving = canMove;

            var castBar = Service.GameGui.GetAddonByName("_CastBar", 1);
            if (castBar == IntPtr.Zero) return;
            AtkResNode* progressBar = ((AtkUnitBase*)castBar)->UldManager.NodeList[5];

            progressBar->AddRed = c.R;
            progressBar->AddGreen = c.G;
            progressBar->AddBlue = c.B;
        }

        private static void UpdateHightLight()
        {
            if (ActionUpdater.NextAction == null || !Service.Configuration.TeachingMode) return;
            HigglightAtionBar(ActionUpdater.NextAction.ID);
        }

        private static readonly string[] _barsName = new string[]
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
                    if (bar->ActionBarSlots[i].ActionId == actionID)
                    {
                        bar->PulseActionBarSlot(i);
                        return true;
                    }
                }
                return false;
            });
        }

        private static unsafe void HigglightAtionBar(uint actionID)
        {
            LoopAllSlotBar(bar =>
            {
                foreach (var slot in bar->Slot)
                {
                    if(slot.ActionId == actionID)
                    {
                        HightlightActionSlot(slot);
                    }
                }
                return false;
            });
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


        private static unsafe void HightlightActionSlot(ActionBarSlot slot)
        {
            var action_addon = (AtkComponentNode*)&slot;
            var dropdrop_addon = (AtkComponentNode*)action_addon->Component->UldManager.NodeList[0];
            var icon_addon = (AtkComponentNode*)dropdrop_addon->Component->UldManager.NodeList[2];
            if (!icon_addon->AtkResNode.IsVisible) return;
            var highlight = (AtkImageNode*)icon_addon->Component->UldManager.NodeList[9];
            highlight->PartId = 10;
            highlight->AtkResNode.ToggleVisibility(true);
        }

        public static void Dispose()
        {
            dtrEntry?.Dispose();
        }
    }
}
