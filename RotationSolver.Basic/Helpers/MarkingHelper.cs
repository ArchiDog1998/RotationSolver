using FFXIVClientStructs.FFXIV.Client.Game.UI;
using System;

namespace RotationSolver.Basic.Helpers;

internal class MarkingHelper
{
    internal unsafe static long GetMarker(uint index) => MarkingController.Instance()->MarkerArray[index];

    internal static bool HaveAttackChara => AttackSignTargets.Any(id => id != GameObject.InvalidGameObjectId);

    internal static long[] AttackSignTargets => 
    [
        GetMarker(0),
        GetMarker(1),
        GetMarker(2),
        GetMarker(3),
        GetMarker(4),
        GetMarker(14),
        GetMarker(15),
        GetMarker(16),
    ];

    internal static long[] StopTargets =>
    [
        GetMarker(8),
        GetMarker(9),
    ];

    internal unsafe static IEnumerable<BattleChara> FilterStopCharaes(IEnumerable<BattleChara> charas)
    {
        var ids = StopTargets.Where(id => id != GameObject.InvalidGameObjectId);
        return charas.Where(b => !ids.Contains(b.ObjectId));
    }
}
