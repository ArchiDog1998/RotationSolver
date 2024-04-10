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
        var group = action.IsGeneralGCD() ? action.AdditionalCooldownGroup : action.CooldownGroup;
        if (group == 0) group = GCDCooldownGroup;
        return group;
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

    internal static bool CanUseGCD
    {
        get
        {
            var maxAhead = Service.Config.OverrideActionAheadTimer ? Service.Config.Action4Head : 0.4;

            //GCD
            var canUseGCD = DataCenter.WeaponRemain <= maxAhead;
            return canUseGCD;
        }
    }

    internal static bool CanUseOGCD
    {
        get
        {
            var maxAhead = Service.Config.OverrideActionAheadTimer ? Service.Config.Action4Head : Math.Max(DataCenter.Ping, 0.08f);
            
            //OGCD
            var canUseOGCD = ActionManagerHelper.GetCurrentAnimationLock() <= maxAhead;
            return canUseOGCD;
        }
    }
}
