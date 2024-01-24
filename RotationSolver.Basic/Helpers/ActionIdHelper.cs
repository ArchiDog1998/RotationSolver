using ECommons.DalamudServices;
using FFXIVClientStructs.FFXIV.Client.Game;
using Action = Lumina.Excel.GeneratedSheets.Action;

namespace RotationSolver.Basic.Helpers;
public static  class ActionIdHelper
{
    public unsafe static bool IsCoolingDown(this ActionID actionID)
    {
        return IsCoolingDown(actionID.GetAction().GetCoolDownGroup());
    }

    public unsafe static bool IsCoolingDown(byte cdGroup)
    {
        var detail = GetCoolDownDetail(cdGroup);
        return detail != null && detail->IsActive != 0;
    }

    public static unsafe RecastDetail* GetCoolDownDetail(byte cdGroup) => ActionManager.Instance()->GetRecastGroupDetail(cdGroup - 1);


    private static Action GetAction(this ActionID actionID)
    {
        return Svc.Data.GetExcelSheet<Action>()!.GetRow((uint)actionID)!;
    }
}
