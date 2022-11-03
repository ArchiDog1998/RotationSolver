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
using XIVAutoAttack.SigReplacers;

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

        //public enum Sounds : byte
        //{
        //    None = 0x00,
        //    Unknown = 0x01,
        //    Sound01 = 0x25,
        //    Sound02 = 0x26,
        //    Sound03 = 0x27,
        //    Sound04 = 0x28,
        //    Sound05 = 0x29,
        //    Sound06 = 0x2A,
        //    Sound07 = 0x2B,
        //    Sound08 = 0x2C,
        //    Sound09 = 0x2D,
        //    Sound10 = 0x2E,
        //    Sound11 = 0x2F,
        //    Sound12 = 0x30,
        //    Sound13 = 0x31,
        //    Sound14 = 0x32,
        //    Sound15 = 0x33,
        //    Sound16 = 0x34,
        //}

        internal static unsafe void PulseAtionBar(uint actionID)
        {
            LoopAllSlotBar(bar =>
            {
                for (int i = 0; i < bar->SlotCount; i++)
                {
                    if (IsActionSlotRight(bar->ActionBarSlots[i], actionID))
                    {
                        bar->PulseActionBarSlot(i);
                        //UIModule.PlaySound((uint)Sounds.Sound16, 0, 0, 0);
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
                    //if (IsActionSlotRight(slot, actionID))
                    {
                        //var action_addon = (AtkComponentNode*)&slot;
                        //var dropdrop_addon = (AtkComponentNode*)action_addon->Component->UldManager.NodeList[0];
                        //var icon_addon = (AtkComponentNode*)dropdrop_addon->Component->UldManager.NodeList[2];

                        //var icon_addon = slot.Icon;
                        //if (!icon_addon->AtkResNode.IsVisible) continue;
                        //var highlight = (AtkImageNode*)icon_addon->Component->UldManager.NodeList[9];
                        //highlight->PartId = 16;
                        //highlight->AtkResNode.ToggleVisibility(false);
                    }
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

        public static void Dispose()
        {
            dtrEntry?.Dispose();
        }
    }
}
