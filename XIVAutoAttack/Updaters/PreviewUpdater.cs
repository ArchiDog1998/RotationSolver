using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Game.Gui.Dtr;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using FFXIVClientStructs.FFXIV.Client.Graphics;
using FFXIVClientStructs.FFXIV.Client.System.Framework;
using FFXIVClientStructs.FFXIV.Client.System.Memory;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;
using FFXIVClientStructs.FFXIV.Component.GUI;
using System;
using System.Runtime.InteropServices;
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
            var isTarDead = false;
            if (Service.ObjectTable.SearchById(Service.ClientState.LocalPlayer.CastTargetObjectId) is BattleChara b
                && b.CurrentHp == 0)
            {
                isTarDead = true;
            }
            MovingUpdater.IsMoving = canMove || isTarDead;

            var castBar = Service.GameGui.GetAddonByName("_CastBar", 1);
            if (castBar == IntPtr.Zero) return;
            AtkResNode* progressBar = ((AtkUnitBase*)castBar)->UldManager.NodeList[5];

            progressBar->AddRed = c.R;
            progressBar->AddGreen = c.G;
            progressBar->AddBlue = c.B;
        }

        private unsafe static void UpdateHightLight()
        {
            var actId = ActionUpdater.NextAction?.AdjustedID ?? 0;

            HigglightAtionBar((slot, hot) =>
            {
                return Service.Configuration.TeachingMode && IsActionSlotRight(slot, hot, actId);
            });
        }


        internal static unsafe void PulseAtionBar(uint actionID)
        {
            LoopAllSlotBar((bar, hotbar, index) =>
            {
                return IsActionSlotRight(bar, hotbar, actionID);
            });
        }

        private unsafe static bool IsActionSlotRight(ActionBarSlot* slot, HotBarSlot* hot, uint actionID)
        {
            if (hot->IconTypeA != HotbarSlotType.Action) return false;
            if (hot->IconTypeB != HotbarSlotType.Action) return false;
            if (slot->ActionId == (uint)IconReplacer.KeyActionID) return false;

            return Service.IconReplacer.OriginalHook((uint)slot->ActionId) == actionID;
        }

        const int ActionBarSlotsCount = 12;
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
        unsafe delegate bool ActionBarAction(ActionBarSlot* bar, HotBarSlot* hot, uint highLightID);
        unsafe delegate bool ActionBarPredicate(ActionBarSlot* bar, HotBarSlot* hot);
        private static unsafe void LoopAllSlotBar(ActionBarAction doingSomething)
        {
            for (int i = 0; i < _barsName.Length; i++)
            {
                var name = _barsName[i];
                var actBarPtr = Service.GameGui.GetAddonByName(name, 1);
                if (actBarPtr == IntPtr.Zero) continue;
                var actBar = (AddonActionBarBase*)actBarPtr;
                var hotbar = Framework.Instance()->GetUiModule()->GetRaptureHotbarModule()->HotBar[i];

                for (int slotIndex = 0; slotIndex < ActionBarSlotsCount; slotIndex++)
                {
                    var hotBarSlot = hotbar->Slot[slotIndex];
                    var actionBarSlot = &actBar->ActionBarSlots[slotIndex];
                    var highLightId = 0x53550000 + (uint)i * ActionBarSlotsCount + (uint)slotIndex;
                    if (doingSomething(actionBarSlot, hotBarSlot, highLightId))
                    {
                        actBar->PulseActionBarSlot(slotIndex);
                        //键盘按下效果音效
                        UIModule.PlaySound(12, 0, 0, 0);
                        return;
                    }
                }
            }
        }


        private static unsafe void HigglightAtionBar(ActionBarPredicate shouldShow = null)
        {
            LoopAllSlotBar((slot, hotbar, highLightId) =>
            {
                var iconAddon = slot->Icon;
                if (!iconAddon->AtkResNode.IsVisible) return false;

                AtkImageNode* highLightPtr = null;
                AtkResNode* lastHightLigth = null;
                AtkResNode* nextNode = null;

                for (int nodeIndex = 8; nodeIndex < iconAddon->Component->UldManager.NodeListCount; nodeIndex++)
                {
                    var node = iconAddon->Component->UldManager.NodeList[nodeIndex];
                    if (node->NodeID == highLightId)
                    {
                        highLightPtr = (AtkImageNode*)node;
                    }
                    else if (node->Type == NodeType.Image)
                    {
                        var mayLastNode = (AtkImageNode*)node;
                        if (mayLastNode->PartId == 16)
                        {
                            lastHightLigth = node;
                            continue;
                        }
                    }
                    if (lastHightLigth != null && highLightPtr == null)
                    {
                        nextNode = node;
                        break;
                    }
                }

                if (highLightPtr == null)
                {
                    //Create new Addon
                    highLightPtr = CloneNode((AtkImageNode*)lastHightLigth);
                    highLightPtr->AtkResNode.NodeID = highLightId;

                    //Change LinkList
                    lastHightLigth->PrevSiblingNode = (AtkResNode*)highLightPtr;
                    highLightPtr->AtkResNode.PrevSiblingNode = nextNode;

                    nextNode->NextSiblingNode = (AtkResNode*)highLightPtr;
                    highLightPtr->AtkResNode.NextSiblingNode = lastHightLigth;

                    iconAddon->Component->UldManager.UpdateDrawNodeList();
                }

                //Refine Color
                highLightPtr->AtkResNode.AddRed = 0;
                highLightPtr->AtkResNode.AddGreen = 10;
                highLightPtr->AtkResNode.AddBlue = 40;

                //Change Color
                var color = Service.Configuration.TeachingModeColor;
                highLightPtr->AtkResNode.MultiplyRed = (byte)(color.X * 100);
                highLightPtr->AtkResNode.MultiplyGreen = (byte)(color.Y * 100);
                highLightPtr->AtkResNode.MultiplyBlue = (byte)(color.Z * 100);

                //Update Location
                highLightPtr->AtkResNode.SetPositionFloat(lastHightLigth->X, lastHightLigth->Y);
                highLightPtr->AtkResNode.SetWidth(lastHightLigth->Width);
                highLightPtr->AtkResNode.SetHeight(lastHightLigth->Height);

                //Update Visibility
                highLightPtr->AtkResNode.ToggleVisibility(shouldShow?.Invoke(slot, hotbar) ?? false);

                return false;
            });
        }

        private unsafe static AtkImageNode* CloneNode(AtkImageNode* original)
        {
            var size = sizeof(AtkImageNode);

            var allocation = new IntPtr(IMemorySpace.GetUISpace()->Malloc((ulong)size, 8UL));
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
        public unsafe static void Dispose()
        {
            //Hide All highLight.
            HigglightAtionBar();
            dtrEntry?.Dispose();
        }
    }
}
