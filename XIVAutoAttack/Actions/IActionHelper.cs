using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XIVAutoAttack.Actions
{
    internal static class IActionHelper
    {
        internal static bool IsLastSpell(bool isAdjust, params IAction[] actions)
        {
            return IsLastSpell(GetIDFromActions(isAdjust, actions));
        }
        internal static bool IsLastSpell(params uint[] ids)
        {
            return IsActionID(Watcher.LastSpell, ids);
        }


        internal static bool IsLastAbility(bool isAdjust, params IAction[] actions)
        {
            return IsLastAbility(GetIDFromActions(isAdjust, actions));
        }
        internal static bool IsLastAbility(params uint[] ids)
        {
            return IsActionID(Watcher.LastAbility, ids);
        }

        internal static bool IsLastWeaponSkill(bool isAdjust, params IAction[] actions)
        {
            return IsLastWeaponSkill(GetIDFromActions(isAdjust, actions));
        }
        internal static bool IsLastWeaponSkill(params uint[] ids)
        {
            return IsActionID(Watcher.LastWeaponskill, ids);
        }

        internal static bool IsLastAction(bool isAdjust, params IAction[] actions)
        {
            return IsLastAction(GetIDFromActions(isAdjust, actions));
        }
        internal static bool IsLastAction(params uint[] ids)
        {
            return IsActionID(Watcher.LastAction, ids);
        }

        internal static bool IsAnySameAction(this IAction action, bool isAdjust, params IAction[] actions)
        {
            return IsActionID(isAdjust ? action.AdjustedID : action.ID, GetIDFromActions(isAdjust, actions));
        }

        private static bool IsActionID(uint id, params uint[] ids)
        {
            foreach (var i in ids)
            {
                if (i == id) return true;
            }
            return false;
        }

        private static uint[] GetIDFromActions(bool isAdjust, params IAction[] actions)
        {
            return actions.Select(a => isAdjust ? a.AdjustedID : a.ID).ToArray();
        }
    }
}
