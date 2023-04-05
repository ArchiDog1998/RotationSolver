using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Game.Gui.Dtr;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using FFXIVClientStructs.FFXIV.Client.Graphics;
using FFXIVClientStructs.FFXIV.Client.System.Framework;
using FFXIVClientStructs.FFXIV.Client.System.Memory;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;
using FFXIVClientStructs.FFXIV.Component.GUI;
using RotationSolver.Commands;
using System.Runtime.InteropServices;

namespace RotationSolver.Updaters;

internal static class PreviewUpdater
{
    internal static void UpdatePreview()
    {
        UpdateEntry();
        UpdateCastBar();
        UpdateHightLight();
    }

    static DtrBarEntry _dtrEntry;
    static string _showValue;
    private static void UpdateEntry()
    {
        var showStr = RSCommands.EntryString;
        if (Service.Config.ShowInfoOnDtr && !string.IsNullOrEmpty(showStr))
        {
            try
            {
                _dtrEntry ??= Service.DtrBar.Get("Rotation Solver");
            }
            catch
            {
                return;
            }

            if (!_dtrEntry.Shown) _dtrEntry.Shown = true;
            if (_showValue != showStr)
            {
                _showValue = showStr;
                _dtrEntry.Text = new SeString(
                    new IconPayload(BitmapFontIcon.DPS),
                    new TextPayload(_showValue)
                    );
            }
        }
        else if (_dtrEntry != null && _dtrEntry.Shown)
        {
            _dtrEntry.Shown = false;
        }
    }

    static bool _canMove;
    static bool _isTarNoNeedCast;
    static RandomDelay _tarStopCastDelay = new RandomDelay(() =>
    (Service.Config.StopCastingDelayMin, Service.Config.StopCastingDelayMax));
    internal static void UpdateCastBarState()
    {
        var tardead = Service.Config.UseStopCasting ?
            Service.ObjectTable.SearchById(Service.Player.CastTargetObjectId) is BattleChara b
            && (b is PlayerCharacter ? b.HasStatus(false, StatusID.Raise) : b.CurrentHp == 0) : false;
        _isTarNoNeedCast = _tarStopCastDelay.Delay(tardead);

        bool canMove = !Service.Conditions[Dalamud.Game.ClientState.Conditions.ConditionFlag.OccupiedInEvent]
            && !Service.Conditions[Dalamud.Game.ClientState.Conditions.ConditionFlag.Casting];

        //For lock
        var specialStatus = Service.Player.HasStatus(true, StatusID.PhantomFlurry, StatusID.TenChiJin);

        MovingController.IsMoving = _canMove = specialStatus ? false : canMove;
    }

    static bool _showCanMove;
    static readonly ByteColor _redColor = new ByteColor() { A = 255, R = 120, G = 0, B = 60 };
    static readonly ByteColor _greenColor = new ByteColor() { A = 255, R = 60, G = 120, B = 30 };
    private static unsafe void UpdateCastBar()
    {
        if (_isTarNoNeedCast)
        {
            UIState.Instance()->Hotbar.CancelCast();
        }

        var nowMove = _canMove && Service.Config.CastingDisplay;
        if (nowMove == _showCanMove) return;
        _showCanMove = nowMove;

        ByteColor c = _showCanMove ? _greenColor : _redColor;

        var castBars = Service.GetAddons<AddonCastBar>();
        if (!castBars.Any()) return;
        var castBar = castBars.FirstOrDefault();

        AtkResNode* progressBar = ((AtkUnitBase*)castBar)->UldManager.NodeList[5];

        progressBar->AddRed = c.R;
        progressBar->AddGreen = c.G;
        progressBar->AddBlue = c.B;
    }

    static uint _highLightId;
    private unsafe static void UpdateHightLight()
    {
        var actId = ActionUpdater.NextAction?.AdjustedID ?? 0;
        if (_highLightId == actId) return;
        _highLightId = actId;

        HighLightActionBar((slot, hot) =>
        {
            return Service.Config.TeachingMode && IsActionSlotRight(slot, hot, _highLightId);
        });
    }

    internal static unsafe void PulseActionBar(uint actionID)
    {
        LoopAllSlotBar((bar, hot, index) =>
        {
            return IsActionSlotRight(bar, hot, actionID);
        });
    }

    private unsafe static bool IsActionSlotRight(ActionBarSlot slot, HotBarSlot* hot, uint actionID)
    {
        if ((IntPtr)hot != IntPtr.Zero)
        {
            if (hot->IconTypeA != HotbarSlotType.CraftAction && hot->IconTypeA != HotbarSlotType.Action) return false;
            if (hot->IconTypeB != HotbarSlotType.CraftAction && hot->IconTypeB != HotbarSlotType.Action) return false;
        }

        return Service.GetAdjustedActionId((uint)slot.ActionId) == actionID;
    }

    unsafe delegate bool ActionBarAction(ActionBarSlot bar, HotBarSlot* hot, uint highLightID);
    unsafe delegate bool ActionBarPredicate(ActionBarSlot bar, HotBarSlot* hot);
    private static unsafe void LoopAllSlotBar(ActionBarAction doingSomething)
    {
        var index = 0;
        var hotBarIndex = 0;
        foreach (var intPtr in Service.GetAddons<AddonActionBar>()
            .Union(Service.GetAddons<AddonActionBarX>())
            .Union(Service.GetAddons<AddonActionCross>())
            .Union(Service.GetAddons<AddonActionDoubleCrossBase>()))
        {
            if (intPtr == IntPtr.Zero) continue;
            var actionBar = (AddonActionBarBase*)intPtr;
            var hotBar = Framework.Instance()->GetUiModule()->GetRaptureHotbarModule()->HotBar[hotBarIndex];
            var slotIndex = 0;
            foreach (var slot in actionBar->Slot)
            {
                var highLightId = 0x53550000 + index;

                if (doingSomething(slot, hotBarIndex > 9 ? null: hotBar->Slot[slotIndex], (uint)highLightId))
                {
                    var iconAddon = slot.Icon;
                    if (!iconAddon->AtkResNode.IsVisible) continue;
                    actionBar->PulseActionBarSlot(slotIndex);
                    UIModule.PlaySound(12, 0, 0, 0);
                }
                slotIndex++;
                index++;
            }
            hotBarIndex++;
        }
    }

    private static unsafe void HighLightActionBar(ActionBarPredicate shouldShow = null)
    {
        LoopAllSlotBar((slot, hot, highLightId) =>
        {
            var iconAddon = slot.Icon;
            if (!iconAddon->AtkResNode.IsVisible) return false;

            AtkImageNode* highLightPtr = null;
            AtkResNode* lastHightLight = null;
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
                        lastHightLight = node;
                        continue;
                    }
                }
                if (lastHightLight != null && highLightPtr == null)
                {
                    nextNode = node;
                    break;
                }
            }

            if (highLightPtr == null)
            {
                //Create new Addon
                highLightPtr = CloneNode((AtkImageNode*)lastHightLight);
                highLightPtr->AtkResNode.NodeID = highLightId;

                //Change LinkList
                lastHightLight->PrevSiblingNode = (AtkResNode*)highLightPtr;
                highLightPtr->AtkResNode.PrevSiblingNode = nextNode;

                nextNode->NextSiblingNode = (AtkResNode*)highLightPtr;
                highLightPtr->AtkResNode.NextSiblingNode = lastHightLight;

                iconAddon->Component->UldManager.UpdateDrawNodeList();
            }

            //Refine Color
            highLightPtr->AtkResNode.AddRed = 0;
            highLightPtr->AtkResNode.AddGreen = 10;
            highLightPtr->AtkResNode.AddBlue = 40;

            //Change Color
            var color = Service.Config.TeachingModeColor;
            highLightPtr->AtkResNode.MultiplyRed = (byte)(color.X * 100);
            highLightPtr->AtkResNode.MultiplyGreen = (byte)(color.Y * 100);
            highLightPtr->AtkResNode.MultiplyBlue = (byte)(color.Z * 100);

            //Update Location
            highLightPtr->AtkResNode.SetPositionFloat(lastHightLight->X, lastHightLight->Y);
            highLightPtr->AtkResNode.SetWidth(lastHightLight->Width);
            highLightPtr->AtkResNode.SetHeight(lastHightLight->Height);

            //Update Visibility
            highLightPtr->AtkResNode.ToggleVisibility(shouldShow?.Invoke(slot, hot) ?? false);

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
        HighLightActionBar();
        _dtrEntry?.Dispose();
    }
}
