using Dalamud.Game.Gui.Dtr;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using ECommons.DalamudServices;
using ECommons.GameHelpers;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using FFXIVClientStructs.FFXIV.Client.System.Framework;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;
using RotationSolver.Basic.Configuration;
using RotationSolver.Commands;
using static FFXIVClientStructs.FFXIV.Client.UI.Misc.RaptureHotbarModule;

namespace RotationSolver.Updaters;

internal static class PreviewUpdater
{
    internal static void UpdatePreview()
    {
        UpdateEntry();
        UpdateCancelCast();
    }

    static IDtrBarEntry? _dtrEntry;
    private static void UpdateEntry()
    {
        var showStr = RSCommands.EntryString;
        if (Service.Config.ShowInfoOnDtr
            && !string.IsNullOrEmpty(showStr))
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
            _dtrEntry.OnClick ??= new(() =>
            {
                if (!DataCenter.State)
                {
                    RSCommands.DoStateCommandType(StateCommandType.Auto);
                }
                else if (DataCenter.IsManual)
                {
                    RSCommands.DoStateCommandType(StateCommandType.Cancel);
                }
                else
                {
                    RSCommands.DoStateCommandType(StateCommandType.Manual);
                }
            });
        }
        else if (_dtrEntry != null && _dtrEntry.Shown)
        {
            _dtrEntry.Shown = false;
        }
    }

    static RandomDelay _tarStopCastDelay = new(() =>Service.Config.StopCastingDelay); 

    private static unsafe void UpdateCancelCast()
    {
        if (!Player.Object.IsCasting) return;

        var tarDead = Service.Config.UseStopCasting
            && Svc.Objects.SearchById(Player.Object.CastTargetObjectId) is IBattleChara b
            && b.IsEnemy() && b.CurrentHp == 0;

        var statusTimes = Player.Object.StatusTimes(false, [.. OtherConfiguration.NoCastingStatus.Select(i => (StatusID)i)]);

        var stopDueStatus = statusTimes.Any() && statusTimes.Min() > Player.Object.TotalCastTime - Player.Object.CurrentCastTime && statusTimes.Min() < 5;

        if (_tarStopCastDelay.Delay(tarDead) || stopDueStatus)
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

    private unsafe static bool IsActionSlotRight(ActionBarSlot slot, HotbarSlot? hot, uint actionID)
    {
        if (hot.HasValue)
        {
            if (hot.Value.OriginalApparentSlotType != HotbarSlotType.CraftAction && hot.Value.OriginalApparentSlotType != HotbarSlotType.Action) return false;
            if (hot.Value.ApparentSlotType != HotbarSlotType.CraftAction && hot.Value.ApparentSlotType != HotbarSlotType.Action) return false;
        }

        return Service.GetAdjustedActionId((uint)slot.ActionId) == actionID;
    }

    unsafe delegate bool ActionBarAction(ActionBarSlot bar, HotbarSlot? hot, uint highLightID);
    unsafe delegate bool ActionBarPredicate(ActionBarSlot bar, HotbarSlot* hot);
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
            var hotBar = Framework.Instance()->UIModule->GetRaptureHotbarModule()->Hotbars[hotBarIndex];
            var slotIndex = 0;

            foreach (var slot in actionBar->ActionBarSlotVector.AsSpan())
            {
                var highLightId = 0x53550000 + index;

                if (doingSomething(slot, hotBarIndex > 9 ? null : hotBar.Slots[slotIndex], (uint)highLightId))
                {
                    var iconAddon = slot.Icon;
                    if ((IntPtr)iconAddon == IntPtr.Zero) continue;
                    if (!iconAddon->AtkResNode.IsVisible()) continue;
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
        _dtrEntry?.Remove();
    }
}
