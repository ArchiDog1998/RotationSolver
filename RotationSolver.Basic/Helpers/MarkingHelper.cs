using FFXIVClientStructs.FFXIV.Client.Game.UI;

namespace RotationSolver.Basic.Helpers;

internal enum HeadMarker : byte
{
    Attack1,
    Attack2,
    Attack3,
    Attack4,
    Attack5,
    Bind1,
    Bind2,
    Bind3,
    Stop1,
    Stop2,
    Square,
    Circle,
    Cross,
    Triangle,
    Attack6,
    Attack7, 
    Attack8,
}

internal class MarkingHelper
{
    internal unsafe static long GetMarker(HeadMarker index) => MarkingController.Instance()->Markers[(int)index].ObjectId;

    internal static bool HaveAttackChara => AttackSignTargets.Any(id => id != 0);

    internal static long[] AttackSignTargets => 
    [
        GetMarker(HeadMarker.Attack1),
        GetMarker(HeadMarker.Attack2),
        GetMarker(HeadMarker.Attack3),
        GetMarker(HeadMarker.Attack4),
        GetMarker(HeadMarker.Attack5),
        GetMarker(HeadMarker.Attack6),
        GetMarker(HeadMarker.Attack7),
        GetMarker(HeadMarker.Attack8),
    ];

    internal static long[] StopTargets =>
    [
        GetMarker(HeadMarker.Stop1),
        GetMarker(HeadMarker.Stop2),
    ];

    internal unsafe static IEnumerable<IBattleChara> FilterStopCharaes(IEnumerable<IBattleChara> charas)
    {
        var ids = StopTargets.Where(id => id != 0);
        return charas.Where(b => !ids.Contains((long)b.GameObjectId));
    }
}
