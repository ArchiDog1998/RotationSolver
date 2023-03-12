using Dalamud.Game.ClientState.Objects.Types;
using FFXIVClientStructs.FFXIV.Client.Game.UI;

namespace RotationSolver.Helpers;

public class MarkingHelper
{
    private unsafe static long GetMarker(uint index) => MarkingController.Instance()->MarkerArray[index];


    internal static bool HaveAttackChara(IEnumerable<BattleChara> charas) => GetAttackMarkChara(charas) != null;

    internal static BattleChara GetAttackMarkChara(IEnumerable<BattleChara> charas)
    {
        for (uint i = 0; i < 5; i++)
        {
            var b = GetChara(charas, GetMarker(i));
            if (b?.IsNPCEnemy() ?? false) return b;
        }
        return null;
    }

    private static BattleChara GetChara(IEnumerable<BattleChara> charas, long id)
    {
        if (id == 0xE0000000) return null;
        return charas.FirstOrDefault(item => item.ObjectId == id);
    }

    internal unsafe static IEnumerable<BattleChara> FilterStopCharaes(IEnumerable<BattleChara> charas)
    {
        var ids = new List<long>() { GetMarker(8), GetMarker(9) }.Where(id => id != 0xE0000000);
        return charas.Where(b => !ids.Contains(b.ObjectId));
    }
}
