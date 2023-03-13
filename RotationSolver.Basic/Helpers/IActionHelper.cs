using RotationSolver.Basic;
using RotationSolver.Basic.Actions;
using RotationSolver.Basic.Data;

namespace RotationSolver.Basic.Helpers;

public static class IActionHelper
{
    internal static bool IsLastGCD(bool isAdjust, params IAction[] actions)
    {
        return IsLastGCD(GetIDFromActions(isAdjust, actions));
    }
    internal static bool IsLastGCD(params ActionID[] ids)
    {
        return IsActionID(DataCenter.LastGCD, ids);
    }

    internal static bool IsLastAbility(bool isAdjust, params IAction[] actions)
    {
        return IsLastAbility(GetIDFromActions(isAdjust, actions));
    }
    internal static bool IsLastAbility(params ActionID[] ids)
    {
        return IsActionID(DataCenter.LastAbility, ids);
    }

    internal static bool IsLastAction(bool isAdjust, params IAction[] actions)
    {
        return IsLastAction(GetIDFromActions(isAdjust, actions));
    }
    internal static bool IsLastAction(params ActionID[] ids)
    {
        return IsActionID(DataCenter.LastAction, ids);
    }

    public static bool IsTheSameTo(this IAction action, bool isAdjust, params IAction[] actions)
    {
        return IsActionID(isAdjust ? (ActionID)action.AdjustedID : (ActionID)action.ID, GetIDFromActions(isAdjust, actions));
    }

    private static bool IsActionID(ActionID id, params ActionID[] ids)
    {
        foreach (var i in ids)
        {
            if (i == id) return true;
        }
        return false;
    }

    private static ActionID[] GetIDFromActions(bool isAdjust, params IAction[] actions)
    {
        return actions.Select(a => isAdjust ? (ActionID)a.AdjustedID : (ActionID)a.ID).ToArray();
    }

    public static bool IsMeleeAction(this IBaseAction act)
    {
        return act.Range == 3;
    }
}
