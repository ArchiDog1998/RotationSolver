using Dalamud.Game.ClientState.Conditions;
using ECommons.DalamudServices;
using ECommons.GameHelpers;
using FFXIVClientStructs.FFXIV.Client.Game;
using RotationSolver.Commands;

namespace RotationSolver.Updaters;

internal static class MovingUpdater
{
    internal unsafe static void UpdateCanMove(bool doNextAction)
    {
        bool canMove = !Svc.Condition[ConditionFlag.OccupiedInEvent]
            && !Svc.Condition[ConditionFlag.Casting];

        var statusList = new List<StatusID>(4);
        var actionList = new List<ActionID>(4);

        if (Service.Config.PosFlameThrower)
        {
            statusList.Add(StatusID.Flamethrower);
            actionList.Add(ActionID.FlameThrower);
        }
        if (Service.Config.PosTenChiJin)
        {
            statusList.Add(StatusID.TenChiJin);
            actionList.Add(ActionID.TenChiJin);
        }
        if (Service.Config.PosPassageOfArms)
        {
            statusList.Add(StatusID.PassageOfArms);
            actionList.Add(ActionID.PassageOfArms);
        }
        if (Service.Config.PosImprovisation)
        {
            statusList.Add(StatusID.Improvisation);
            actionList.Add(ActionID.Improvisation);
        }

        //Action
        var action = DateTime.Now - RSCommands._lastUsedTime < TimeSpan.FromMilliseconds(100)
            ? (ActionID)RSCommands._lastActionID
            : doNextAction ? (ActionID)(ActionUpdater.NextAction?.AdjustedID ?? 0) : 0;
        var specialActions = ActionManager.GetAdjustedCastTime(ActionType.Spell, (uint)action) > 0
            || actionList.Any(id => Service.GetAdjustedActionId(id) == action);

        //Status
        var specialStatus = Player.Object.HasStatus(true, statusList.ToArray());

        Service.CanMove = !specialStatus && !specialActions && canMove;
    }
}
