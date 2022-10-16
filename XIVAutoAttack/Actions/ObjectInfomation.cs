using Dalamud.Game.ClientState.Objects.Types;
using Lumina.Excel.GeneratedSheets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XIVAutoAttack.Actions
{
    internal static class ObjectInfomation
    {
        private unsafe static BNpcBase GetObjectNPC(this GameObject obj)
        {
            var ptr = (FFXIVClientStructs.FFXIV.Client.Game.Object.GameObject*)(void*)obj.Address;
            return Service.DataManager.GetExcelSheet<BNpcBase>().GetRow(ptr->GetNpcID());
        }

        internal static bool HasLocationSide(this GameObject obj)
        {
            return !obj.GetObjectNPC().Unknown10;
        }

        internal static bool IsBoss(this GameObject obj)
        {
            return !obj.GetObjectNPC().IsTargetLine;
        }

        internal static float GetHealthRatio(this BattleChara b)
        {
            return (float)b.CurrentHp / b.MaxHp;
        }
    }
}
