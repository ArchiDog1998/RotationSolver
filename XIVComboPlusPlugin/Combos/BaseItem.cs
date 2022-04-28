using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FFXIVClientStructs.FFXIV.Client.Game;
using Lumina.Excel.GeneratedSheets;
namespace XIVComboPlus.Combos
{
    internal class BaseItem
    {
        public Item Item { get; } = null;
        public BaseItem(string name)
        {
            var enmu = Service.DataManager.GetExcelSheet<Item>().GetEnumerator();

            while(enmu.MoveNext())
            {
                if(enmu.Current.Name == name)
                {
                    Item = enmu.Current;
                    break;
                }
            }
        }

        public bool ShoudUseItem()
        {
            return true;
        }

        public unsafe bool UseItem()
        {
            if (Item == null) return false;

            return ActionManager.Instance()->UseAction(ActionType.Item, Item.RowId, Service.ClientState.LocalPlayer.ObjectId);
        }
    }
}
