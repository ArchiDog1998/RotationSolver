using ECommons.DalamudServices;
using ECommons.GameFunctions;
using XIVPainter.Vfx;
using Action = Lumina.Excel.GeneratedSheets.Action;

namespace RotationSolver.Updaters.Omens;
internal static class CastingOmen
{
    private static readonly Dictionary<uint, StaticVfx?> _castingVfxs = [];

    public static void Init()
    {
        Svc.ClientState.TerritoryChanged += ClientState_TerritoryChanged;
    }

    public static void Dispose()
    {
        Svc.ClientState.TerritoryChanged -= ClientState_TerritoryChanged;
    }

    private static void ClientState_TerritoryChanged(ushort obj)
    {
        ClearDict();
    }

    private static void ClearDict()
    {
        foreach (var item in _castingVfxs.Values)
        {
            item?.Dispose();
        }
        _castingVfxs.Clear();
    }

    public static void Update()
    {
        if (!Service.Config.ShowOmen)
        {
            ClearDict();
            return;
        }

        foreach (var item in Svc.Objects.OfType<BattleChara>())
        {
            UpdateOneObject(item);
        }
    }

    private static unsafe void UpdateOneObject(BattleChara battleChara)
    {
        var id = battleChara.ObjectId;
        var castInfo = battleChara.Struct()->GetCastInfo;

        if (_castingVfxs.TryGetValue(id, out var vfx)) // Last is casting.
        {
            if (castInfo->IsCasting == 0) //But not now.
            {
                vfx?.Dispose();
                _castingVfxs.Remove(id);
            }
        }
        else // Last isn't casting.
        {
            if (castInfo->IsCasting != 0) //But it is now.
            {
                _castingVfxs[id] = CreateVfx(battleChara, castInfo);
            }
        }
    }

    private static unsafe StaticVfx? CreateVfx(BattleChara chara, FFXIVClientStructs.FFXIV.Client.Game.Character.Character.CastInfo* castInfo)
    {
        var action = Svc.Data.GetExcelSheet<Action>()?.GetRow(castInfo->ActionID);

        if (action == null) return null;

        var castObj = Svc.Objects.SearchById(castInfo->CastTargetID) ?? chara;

        return OmenMain.CreateVfx(castObj, action, castInfo->CastLocation, castInfo->AdjustedTotalCastTime - castInfo->CurrentCastTime);
    }
}
