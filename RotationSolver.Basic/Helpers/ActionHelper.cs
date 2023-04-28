using Action = Lumina.Excel.GeneratedSheets.Action;


namespace RotationSolver.Basic.Helpers;

public static class ActionHelper
{
    public const byte GCDCooldownGroup = 58;

    public static ActionCate GetActionType(this Action action) => (ActionCate)action.ActionCategory.Value.RowId;
    public static bool IsGeneralGCD(this Action action) => action.CooldownGroup == GCDCooldownGroup;

    public static bool IsRealGCD(this Action action) => action.IsGeneralGCD() || action.AdditionalCooldownGroup == GCDCooldownGroup;

    public static byte GetCoolDownGroup(this Action action)
    {
        var group = action.IsGeneralGCD() ? action.AdditionalCooldownGroup : action.CooldownGroup;
        if (group == 0) group = GCDCooldownGroup;
        return group;
    }
}
