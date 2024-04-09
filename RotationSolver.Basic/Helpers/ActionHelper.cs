using FFXIVClientStructs.FFXIV.Client.Game;
using Action = Lumina.Excel.GeneratedSheets.Action;


namespace RotationSolver.Basic.Helpers;

internal static class ActionHelper
{
    internal const byte GCDCooldownGroup = 58;

    internal static ActionCate GetActionCate(this Action action) => (ActionCate)(action.ActionCategory.Value?.RowId ?? 0);

    internal static bool IsGeneralGCD(this Action action) => action.CooldownGroup == GCDCooldownGroup;

    internal static bool IsRealGCD(this Action action) => action.IsGeneralGCD() || action.AdditionalCooldownGroup == GCDCooldownGroup;

    internal static byte GetCoolDownGroup(this Action action)
    {
        var group = action.IsGeneralGCD() ? GetAdditionalCooldownGroup(action) : GetFirstCooldownGroup(action);
        if (group < 0) group = GCDCooldownGroup;
        return group;
    }

    private static unsafe byte GetFirstCooldownGroup(Action action)
    {
        return (byte)ActionManager.Instance()->GetRecastGroup((int)ActionType.Action, action.RowId);
    }

    private static unsafe byte GetAdditionalCooldownGroup(Action action)
    {
        return (byte)ActionManager.Instance()->GetAdditionalRecastGroup(ActionType.Action, action.RowId);
    }

    internal static bool IsInJob(this Action i)
    {
        var cate = i.ClassJobCategory.Value;
        if (cate != null)
        {
            var inJob = (bool?)cate.GetType().GetProperty(DataCenter.Job.ToString())?.GetValue(cate);
            if (inJob.HasValue && !inJob.Value) return false;
        }
        return true;
    }
}
