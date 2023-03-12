using RotationSolver.Actions;
using RotationSolver.Basic;
using RotationSolver.Data;
using RotationSolver.SigReplacers;

namespace RotationSolver.Helpers;

public static class IActionHelper
{
    public static bool IsLastGCD(bool isAdjust, params IAction[] actions)
    {
        return IsLastGCD(GetIDFromActions(isAdjust, actions));
    }
    public static bool IsLastGCD(params ActionID[] ids)
    {
        return IsActionID(DataCenter.LastGCD, ids);
    }

    public static bool IsLastAbility(bool isAdjust, params IAction[] actions)
    {
        return IsLastAbility(GetIDFromActions(isAdjust, actions));
    }
    public static bool IsLastAbility(params ActionID[] ids)
    {
        return IsActionID(DataCenter.LastAbility, ids);
    }

    public static bool IsLastAction(bool isAdjust, params IAction[] actions)
    {
        return IsLastAction(GetIDFromActions(isAdjust, actions));
    }
    public static bool IsLastAction(params ActionID[] ids)
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

    internal static bool IsMeleeAction(this IBaseAction act)
    {
        return act.Range == 3;
    }
}
