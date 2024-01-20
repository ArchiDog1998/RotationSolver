using Action = Lumina.Excel.GeneratedSheets.Action;


namespace RotationSolver.Basic.Helpers;

internal static class ActionHelper
{
    internal const byte GCDCooldownGroup = 58;

    internal static ActionCate GetActionCate(this Action action) => (ActionCate)action.ActionCategory.Value.RowId;

    internal static bool IsGeneralGCD(this Action action) => action.CooldownGroup == GCDCooldownGroup;

    internal static bool IsRealGCD(this Action action) => action.IsGeneralGCD() || action.AdditionalCooldownGroup == GCDCooldownGroup;

    /// <summary>
    /// TODO: should be in code generator!
    /// </summary>
    /// <param name="action"></param>
    /// <returns></returns>
    internal static bool IsDutyAction(this Action action) => !action.IsRoleAction && action.RowId is not 3;

    internal static byte GetCoolDownGroup(this Action action)
    {
        var group = action.IsGeneralGCD() ? action.AdditionalCooldownGroup : action.CooldownGroup;
        if (group == 0) group = GCDCooldownGroup;
        return group;
    }
}
