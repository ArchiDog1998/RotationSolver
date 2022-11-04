using Dalamud.Game.Gui.Dtr;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using FFXIVClientStructs.FFXIV.Client.Graphics;
using FFXIVClientStructs.FFXIV.Client.System.Memory;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using ImGuiNET;
using Lumina.Excel.GeneratedSheets;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.InteropServices;
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
            HigglightAtionBar(ActionUpdater.NextAction?.AdjustedID ?? 0);
        }

        static readonly string[] _barsName = new string[]
        {
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

            LoopAllSlotBar((bar, index) =>
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
            LoopAllSlotBar((bar, index) =>
            {
                int i = -1;
                foreach (var slot in bar->Slot)
                {
                    i++;
                    var highLightId = 0x53550000 + (uint)index * 16 + (uint)i;
                    var iconAddon = slot.Icon;
                    if (!iconAddon->AtkResNode.IsVisible) continue;

                    AtkImageNode* highLightPtr = null;

                    for (int nodeIndex = 0; nodeIndex < iconAddon->Component->UldManager.NodeListCount; nodeIndex++)
                    {
                        var node = iconAddon->Component->UldManager.NodeList[nodeIndex];
                        if(node->NodeID == highLightId)
                        {
                            highLightPtr = (AtkImageNode*)node;
                            break;
                        }
                    }

                    var lastHightLigth = iconAddon->Component->UldManager.NodeList[10];

                    if (highLightPtr == null)
                    {

                        //Create new Addon
                        highLightPtr = CloneNode((AtkImageNode*)lastHightLigth);
                        highLightPtr->AtkResNode.NodeID = highLightId;

                        //Change LinkList
                        var nextAddom = iconAddon->Component->UldManager.NodeList[11];

                        lastHightLigth->PrevSiblingNode = (AtkResNode*)highLightPtr;
                        highLightPtr->AtkResNode.PrevSiblingNode = nextAddom;

                        nextAddom->NextSiblingNode = (AtkResNode*)highLightPtr;
                        highLightPtr->AtkResNode.NextSiblingNode = lastHightLigth;

                        iconAddon->Component->UldManager.UpdateDrawNodeList();
                    }

                    //Change Color
                    var color = Service.Configuration.TeachingModeColor;
                    highLightPtr->AtkResNode.MultiplyRed = (byte)(color.X * 255);
                    highLightPtr->AtkResNode.MultiplyGreen = (byte)(color.Y * 255);
                    highLightPtr->AtkResNode.MultiplyBlue = (byte)(color.Z * 255);

                    //Update Location
                    highLightPtr->AtkResNode.SetPositionFloat(lastHightLigth->X, lastHightLigth->Y);
                    highLightPtr->AtkResNode.SetWidth(lastHightLigth->Width);
                    highLightPtr->AtkResNode.SetHeight(lastHightLigth->Height);

                    //Update Visibility
                    var show = Service.Configuration.TeachingMode && IsActionSlotRight(slot, actionID);
                    highLightPtr->AtkResNode.ToggleVisibility(show);
                }
                return false;
            });
        }

        public unsafe static AtkImageNode* CloneNode(AtkImageNode* original)
        {
            var size = sizeof(AtkImageNode);

            var allocation = Alloc((ulong)size);
            var bytes = new byte[size];
            Marshal.Copy(new IntPtr(original), bytes, 0, bytes.Length);
            Marshal.Copy(bytes, 0, allocation, bytes.Length);

            var newNode = (AtkResNode*)allocation;
            newNode->ChildNode = null;
            newNode->ChildCount = 0;
            newNode->PrevSiblingNode = null;
            newNode->NextSiblingNode = null;
            return (AtkImageNode*)newNode;
        }

        public unsafe static IntPtr Alloc(ulong size)
        {
            return new IntPtr(IMemorySpace.GetUISpace()->Malloc(size, 8UL));
        }

        private static bool IsActionSlotRight(ActionBarSlot slot, uint actionID)
        {
            if (slot.ActionId == IconReplacer.KeyActionID.ID) return false;
            return Service.IconReplacer.OriginalHook((uint)slot.ActionId) == actionID;
        }

        unsafe delegate bool ActionBarAction(AddonActionBarBase* bar, int index);
        private static unsafe void LoopAllSlotBar(ActionBarAction doingSomething)
        {
            for (int i = 0; i < _barsName.Length; i++)
            {
                var name = _barsName[i];
                var actBar = Service.GameGui.GetAddonByName(name, 1);
                if (actBar == IntPtr.Zero) continue;
                if (doingSomething((AddonActionBarBase*)actBar, i)) return;

            }
        }
        public unsafe static void Dispose()
        {
            dtrEntry?.Dispose();
        }
    }
}
