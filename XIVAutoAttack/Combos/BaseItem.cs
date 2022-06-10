using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FFXIVClientStructs.FFXIV.Client.Game;
using Lumina.Excel.GeneratedSheets;

namespace XIVAutoAttack.Combos
{
    internal class BaseItem : IAction
    {
        public Item Item { get; } = null;
        private uint A4 { get; } = 0;

        public uint ID => Item.RowId;
        public Func<bool> OtherCheck {private get; set; }
        public unsafe bool HaveIt => InventoryManager.Instance()->GetInventoryItemCount(Item.RowId, false) > 0 ||
                InventoryManager.Instance()->GetInventoryItemCount(Item.RowId, true) > 0;
        public BaseItem(string name, uint a4 = 0)
        {
            var enmu = Service.DataManager.GetExcelSheet<Item>().GetEnumerator();

            while (enmu.MoveNext())
            {
                if (enmu.Current.Name == name)
                {
                    Item = enmu.Current;
                    break;
                }
            }
            A4 = a4;
        }

        public BaseItem(uint row, uint a4 = 0)
        {
            Item = Service.DataManager.GetExcelSheet<Item>().GetRow(row);
            A4 = a4;
        }

        public unsafe bool ShoudUseItem(out IAction item)
        {
            item = this;

            if (Item == null) return false;

            if (!Service.Configuration.UseItem) return false;

            if (ActionManager.Instance()->GetRecastTime(ActionType.Item, Item.RowId) > 0) return false;

            if (OtherCheck != null &&!OtherCheck()) return false;

            return HaveIt;

        }

        public unsafe bool Use()
        {
            if (Item == null) return false;

            if(InventoryManager.Instance()->GetInventoryItemCount(Item.RowId, true) > 0)
            {
                return ActionManager.Instance()->UseAction(ActionType.Item, Item.RowId + 1000000, Service.ClientState.LocalPlayer.ObjectId, A4);
            }

            return ActionManager.Instance()->UseAction(ActionType.Item, Item.RowId, Service.ClientState.LocalPlayer.ObjectId, A4);
        }
    }
}
