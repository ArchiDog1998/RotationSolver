namespace RotationSolver.Basic.Helpers;

/// <summary>
/// Helper about actions.
/// </summary>
public static class IActionHelper
{
    //internal static ActionID[] MovingActions { get; } =
    //[
    //    ActionID.EnAvant,
    //    ActionID.Plunge,
    //    ActionID.RoughDivide,
    //    ActionID.Thunderclap,
    //    ActionID.Shukuchi,
    //    ActionID.Intervene,
    //    //ActionID.CorpsACorps,
    //    ActionID.HellsIngress,
    //    ActionID.HissatsuGyoten,
    //    ActionID.Icarus,
    //    ActionID.Onslaught,
    //    //ActionID.SpineShatterDive,
    //    //ActionID.DragonFireDive,
    //];

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

    /// <summary>
    /// Is this action is the same to any one of this.
    /// </summary>
    /// <param name="action"></param>
    /// <param name="isAdjust"></param>
    /// <param name="actions"></param>
    /// <returns></returns>
    public static bool IsTheSameTo(this IAction action, bool isAdjust, params IAction[] actions)
        => action.IsTheSameTo(GetIDFromActions(isAdjust, actions));

    /// <summary>
    /// Is this action is the same to any one of this.
    /// </summary>
    /// <param name="action"></param>
    /// <param name="actions"></param>
    /// <returns></returns>
    public static bool IsTheSameTo(this IAction action, params ActionID[] actions)
    {
        if (action == null) return false;
        return IsActionID((ActionID)action.AdjustedID, actions);
    }

    private static bool IsActionID(ActionID id, params ActionID[] ids) => ids.Contains(id);

    private static ActionID[] GetIDFromActions(bool isAdjust, params IAction[] actions)
    {
        return actions.Select(a => isAdjust ? (ActionID)a.AdjustedID : (ActionID)a.ID).ToArray();
    }
}
