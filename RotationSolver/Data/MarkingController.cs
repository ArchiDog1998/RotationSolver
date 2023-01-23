using Dalamud.Game.ClientState.Objects.Types;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RotationSolver.Data;

public struct MarkingHelper
{
    //public static long Attack1 => GetMarker(0);
    //public static long Attack2 => GetMarker(1);
    //public static long Attack3 => GetMarker(2);
    //public static long Attack4 => GetMarker(3);
    //public static long Attack5 => GetMarker(4);
    //public static long Bind1 => GetMarker(5);
    //public static long Bind2 => GetMarker(6);
    //public static long Bind3 => GetMarker(7);
    //public static long Stop1 => GetMarker(8);
    //public static long Stop2 => GetMarker(9);
    //public static long Square => GetMarker(10);
    //public static long Circle => GetMarker(11);
    //public static long Cross => GetMarker(12);
    //public static long Triangle => GetMarker(13);

    private unsafe static long GetMarker(uint index) => MarkingController.Instance()->MarkerArray[index];


    internal static bool HaveAttackChara(IEnumerable<BattleChara> charas) => GetAttackMarkChara(charas) != null;

    internal static BattleChara GetAttackMarkChara(IEnumerable<BattleChara> charas)
    {
        for (uint i = 0; i < 5; i++)
        {
            var b = GetChara(charas, GetMarker(i));
            if (b != null) return b;
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
