using Dalamud.Game.ClientState.Objects.Types;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace XIVAutoAction.Data;

[StructLayout(LayoutKind.Explicit, Size = 688)]
public struct MarkingHelper
{
    public static long Attack1 => GetMarker(0);
    public static long Attack2 => GetMarker(1);
    public static long Attack3 => GetMarker(2);
    public static long Attack4 => GetMarker(3);
    public static long Attack5 => GetMarker(4);
    public static long Bind1 => GetMarker(5); 
    public static long Bind2 => GetMarker(6); 
    public static long Bind3 => GetMarker(7); 
    public static long Stop1 => GetMarker(8);
    public static long Stop2 => GetMarker(9);
    public static long Square => GetMarker(10);
    public static long Circle => GetMarker(11);
    public static long Cross => GetMarker(12);
    public static long Triangle => GetMarker(13);

    private unsafe static long GetMarker(uint index) => MarkingController.Instance()->MarkerArray[index];

    internal static unsafe BattleChara Attack1Chara(IEnumerable<BattleChara> charas) => GetChara(charas, Attack1);
    internal static unsafe BattleChara Attack2Chara(IEnumerable<BattleChara> charas) => GetChara(charas, Attack2);
    internal static unsafe BattleChara Attack3Chara(IEnumerable<BattleChara> charas) => GetChara(charas, Attack3);
    internal static unsafe BattleChara Attack4Chara(IEnumerable<BattleChara> charas) => GetChara(charas, Attack4);
    internal static unsafe BattleChara Attack5Chara(IEnumerable<BattleChara> charas) => GetChara(charas, Attack5);

    internal static bool HaveAttackChara(IEnumerable<BattleChara> charas)
    {
        if (Attack1Chara(charas) != null) return true;
        if (Attack2Chara(charas) != null) return true;
        if (Attack3Chara(charas) != null) return true;
        if (Attack4Chara(charas) != null) return true;
        if (Attack5Chara(charas) != null) return true;
        return false;
    }


    private static BattleChara GetChara(IEnumerable<BattleChara> charas, long id)
    {
        if (id == 0xE0000000) return null;
        return charas.FirstOrDefault(item => item.ObjectId == id);
    }

    internal unsafe static IEnumerable<BattleChara> FilterStopCharaes(IEnumerable<BattleChara> charas)
    {
        List<long> ids = new List<long>(2);
        if (Stop1 != 0xE0000000)
        {
            ids.Add(Stop1);
        }
        if (Stop2 != 0xE0000000)
        {
            ids.Add(Stop2);
        }
        if (ids.Count == 0) return charas;

        return charas.Where(b => !ids.Contains(b.ObjectId));
    }
}
