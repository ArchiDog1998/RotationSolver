using Dalamud.Game.Gui.Dtr;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using ECommons.DalamudServices;
using ECommons.GameHelpers;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using FFXIVClientStructs.FFXIV.Client.System.Framework;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;
using RotationSolver.Commands;

namespace RotationSolver.Updaters;

internal static class PreviewUpdater
{
    internal static void UpdatePreview()
    {
        UpdateEntry();
        UpdateCancelCast();
    }

    static DtrBarEntry _dtrEntry;
    private static void UpdateEntry()
    {
        var showStr = RSCommands.EntryString;
        if (Service.Config.ShowInfoOnDtr && !string.IsNullOrEmpty(showStr))
        {
            try
            {
                _dtrEntry ??= Svc.DtrBar.Get("Rotation Solver");
            }
            catch
            {
                return;
            }

            if (!_dtrEntry.Shown) _dtrEntry.Shown = true;

            _dtrEntry.Text = new SeString(
                new IconPayload(BitmapFontIcon.DPS),
                new TextPayload(showStr)
                );
        }
        else if (_dtrEntry != null && _dtrEntry.Shown)
        {
            _dtrEntry.Shown = false;
        }
    }

    static RandomDelay _tarStopCastDelay = new(() =>
    (Service.Config.StopCastingDelayMin, Service.Config.StopCastingDelayMax));
    static bool _showCanMove;
    private static unsafe void UpdateCancelCast()
    {
        var tarDead = Service.Config.UseStopCasting 
            && Svc.Objects.SearchById(Player.Object.CastTargetObjectId) is BattleChara b
            && b.IsNPCEnemy() && b.CurrentHp == 0;

        if (_tarStopCastDelay.Delay(tarDead))
        {
            UIState.Instance()->Hotbar.CancelCast();
        }
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
                    if ((IntPtr)iconAddon == IntPtr.Zero) continue;
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

    public unsafe static void Dispose()
    {
        _dtrEntry?.Dispose();
    }
}
