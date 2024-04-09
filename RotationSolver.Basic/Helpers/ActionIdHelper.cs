using ECommons.DalamudServices;
using FFXIVClientStructs.FFXIV.Client.Game;
using Action = Lumina.Excel.GeneratedSheets.Action;

namespace RotationSolver.Basic.Helpers;

/// <summary>
/// The helper for the action id.
/// </summary>
public static class ActionIdHelper
{
    /// <summary>
    /// Is this action cooling down.
    /// </summary>
    /// <param name="actionID">the action id.</param>
    /// <returns></returns>
    public unsafe static bool IsCoolingDown(this ActionID actionID)
    {
        return IsCoolingDown(actionID.GetAction().GetCoolDownGroup());
    }

    /// <summary>
    /// Is this action cooling down.
    /// </summary>
    /// <param name="cdGroup"></param>
    /// <returns></returns>
    public unsafe static bool IsCoolingDown(byte cdGroup)
    {
        var detail = GetCoolDownDetail(cdGroup);
        return detail != null && detail->IsActive != 0;
    }

    /// <summary>
    /// The cd details
    /// </summary>
    /// <param name="cdGroup"></param>
    /// <returns></returns>
    public static unsafe RecastDetail* GetCoolDownDetail(byte cdGroup) => ActionManager.Instance()->GetRecastGroupDetail(cdGroup);


    private static Action GetAction(this ActionID actionID)
    {
        return Svc.Data.GetExcelSheet<Action>()!.GetRow((uint)actionID)!;
    }

    /// <summary>
    /// The cast time.
    /// </summary>
    /// <param name="actionID"></param>
    /// <returns></returns>
    public unsafe static float GetCastTime(this ActionID actionID)
    {
        return ActionManager.GetAdjustedCastTime(ActionType.Action, (uint)actionID) / 1000f; ;
    }
}
