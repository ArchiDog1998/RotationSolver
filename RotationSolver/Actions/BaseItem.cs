using FFXIVClientStructs.FFXIV.Client.Game;
using ImGuiNET;
using Lumina.Excel.GeneratedSheets;
using RotationSolver.Helpers;
using RotationSolver.Updaters;
using System;
using System.Linq;

namespace RotationSolver.Actions;

internal class BaseItem : IBaseItem
{
    private Item _item = null;
    private uint A4 { get; } = 0;

    public uint ID => _item.RowId;
    public uint AdjustedID => ID;

    public Func<bool> OtherCheck { private get; set; }
    public unsafe bool HaveIt => InventoryManager.Instance()->GetInventoryItemCount(_item.RowId, false) > 0 ||
            InventoryManager.Instance()->GetInventoryItemCount(_item.RowId, true) > 0;

    public uint IconID { get; }

    public string Name => _item.Name;

    public string CateName => "Items";

    public bool IsEnabled
    {
        get => !Service.Configuration.DiabledItems.Contains(ID);
        set
        {
            if (value)
            {
                Service.Configuration.DiabledItems.Remove(ID);
            }
            else
            {
                Service.Configuration.DiabledItems.Add(ID);
            }
        }
    }

    public string Description => string.Empty;

    public BaseItem(uint row, uint a4 = 65535)
    {
        _item = Service.DataManager.GetExcelSheet<Item>().GetRow(row);
        IconID = _item.Icon;
        A4 = a4;
    }

    public unsafe bool CanUse(out IAction item)
    {
        item = this;

        if (_item == null) return false;

        if (!Service.Configuration.UseItem) return false;

        if (ConfigurationHelper.BadStatus.Contains(ActionManager.Instance()->GetActionStatus(ActionType.Item, ID))) return false;

        var remain = ActionManager.Instance()->GetRecastTime(ActionType.Item, ID) - ActionManager.Instance()->GetRecastTimeElapsed(ActionType.Item, ID);

        if (!CooldownHelper.RecastAfter(ActionUpdater.AbilityRemain, remain, false)) return false;

        if (OtherCheck != null && !OtherCheck()) return false;

        return HaveIt;
    }

    public unsafe bool Use()
    {
        if (_item == null) return false;

        if (InventoryManager.Instance()->GetInventoryItemCount(_item.RowId, true) > 0)
        {
            return ActionManager.Instance()->UseAction(ActionType.Item, _item.RowId + 1000000, Service.ClientState.LocalPlayer.ObjectId, A4);
        }

        return ActionManager.Instance()->UseAction(ActionType.Item, _item.RowId, Service.ClientState.LocalPlayer.ObjectId, A4);
    }

    public unsafe void Display(bool IsActive) => this.DrawEnableTexture(false, null, otherThing: () =>
    {
#if DEBUG
        ImGui.Text("Status: " + ActionManager.Instance()->GetActionStatus(ActionType.Item, ID).ToString());
        var remain = ActionManager.Instance()->GetRecastTime(ActionType.Item, ID) - ActionManager.Instance()->GetRecastTimeElapsed(ActionType.Item, ID);
        ImGui.Text("remain: " + remain.ToString());
#endif
    });
}
