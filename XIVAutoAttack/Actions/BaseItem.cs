using FFXIVClientStructs.FFXIV.Client.Game;
using ImGuiScene;
using Lumina.Excel.GeneratedSheets;
using System;

namespace XIVAutoAttack.Actions
{
    internal class BaseItem : IAction
    {
        private Item _item = null;
        private uint A4 { get; } = 0;

        public uint ID => _item.RowId;
        public uint AdjustedID => Service.IconReplacer.OriginalHook(ID);

        public Func<bool> OtherCheck { private get; set; }
        public unsafe bool HaveIt => InventoryManager.Instance()->GetInventoryItemCount(_item.RowId, false) > 0 ||
                InventoryManager.Instance()->GetInventoryItemCount(_item.RowId, true) > 0;

        public TextureWrap Icon { get; }

        public BaseItem(uint row, uint a4 = 0)
        {
            _item = Service.DataManager.GetExcelSheet<Item>().GetRow(row);
            Icon = Service.DataManager.GetImGuiTextureIcon(_item.Icon);

            A4 = a4;
        }

        public unsafe bool ShoudUseItem(out IAction item)
        {
            item = this;

            if (_item == null) return false;

            if (!Service.Configuration.UseItem) return false;

            if (ActionManager.Instance()->GetRecastTime(ActionType.Item, _item.RowId) > 0) return false;

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
    }
}
