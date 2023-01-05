using FFXIVClientStructs.FFXIV.Client.Game;
using Lumina.Excel.GeneratedSheets;
using System;
using XIVAutoAttack.Data;
using XIVAutoAttack.Helpers;
using XIVAutoAttack.Localization;
using XIVAutoAttack.Updaters;
using Action = Lumina.Excel.GeneratedSheets.Action;

namespace XIVAutoAttack.Actions.BaseCraftAction;

public enum CraftActionType : byte
{
    Progress,
    Quality,
    Status,
    Other,
}

public static class CraftActionTypeExtension
{
    public static string ToName(this CraftActionType role) => role switch
    {
        CraftActionType.Progress => LocalizationManager.RightLang.CraftActionType_Progress,
        CraftActionType.Quality => LocalizationManager.RightLang.CraftActionType_Quality,
        CraftActionType.Status => LocalizationManager.RightLang.CraftActionType_Status,
        CraftActionType.Other => LocalizationManager.RightLang.CraftActionType_Other,
        _ => string.Empty,
    };
}

internal partial class BaseCraftAction : IAction
{
    internal StatusID[] BuffsProvide { get; set; } = null;

    public uint ID => _crp;

    public uint AdjustedID
    {
        get
        {
            var player = Service.ClientState.LocalPlayer;
            if (player == null) return _crp;

            return player.ClassJob.Id switch
            {
                8 => _crp,
                9 => _bsm,
                10 => _arm,
                11 => _gsm,
                12 => _ltw,
                13 => _wvr,
                14 => _alc,
                15 => _cul,
                _ => _crp,
            };
        }
    }

    internal unsafe uint CPCost { get; }

    public bool IsEnabled
    {
        get => !Service.Configuration.DiabledActions.Contains((uint)ID);
        set
        {
            if (value)
            {
                Service.Configuration.DiabledActions.Remove((uint)ID);
            }
            else
            {
                Service.Configuration.DiabledActions.Add((uint)ID);
            }
        }
    }
    public uint IconID { get; }

    public string Name { get; }

    uint _crp, _bsm, _arm, _gsm, _ltw, _wvr, _alc, _cul;
    ActionType Type => _crp > 100000 ? ActionType.CraftAction : ActionType.Spell;

    public string CateName => _craftType.ToName();
    CraftActionType _craftType;
    uint _last;
    public BaseCraftAction(uint ActionId, CraftActionType type, Func<byte, double> baseMultiply = null, uint lastCraft = 0)

    {
        GetBaseMultiply = baseMultiply ?? (level => 0);
        _craftType = type;
        _last = lastCraft;

        if (ActionId > 100000)
        {
            var craftAction = Service.DataManager.GetExcelSheet<CraftAction>().GetRow(ActionId);
            _crp = craftAction.CRP.Row;
            _bsm = craftAction.BSM.Row;
            _arm = craftAction.ARM.Row;
            _gsm = craftAction.GSM.Row;
            _ltw = craftAction.LTW.Row;
            _wvr = craftAction.WVR.Row;
            _alc = craftAction.ALC.Row;
            _cul = craftAction.CUL.Row;

            Name = craftAction.Name;
            IconID = craftAction.Icon;
            CPCost = craftAction.Cost;
        }
        else
        {
            var action = Service.DataManager.GetExcelSheet<Action>().GetRow(ActionId);
            _crp = ActionId;
            _bsm = ActionId + 1;
            _arm = ActionId + 2;
            _gsm = ActionId + 3;
            _ltw = ActionId + 4;
            _wvr = ActionId + 5;
            _alc = ActionId + 6;
            _cul = ActionId + 7;

            Name = action.Name;
            IconID = action.Icon;
            CPCost = action.PrimaryCostValue;
        }
    }

    public unsafe bool ShouldUse(out IAction act)
    {
        act = this;

        //用户不让用！
        if (!IsEnabled) return false;

        var player = Service.ClientState.LocalPlayer;
        if (player == null) return false;

        if (BuffsProvide != null && player.HasStatus(false, BuffsProvide)) return false;

        if (_last != 0 && _last != ActionUpdater.LastCraftAction) return false;

        var actionManager = ActionManager.Instance();
        if ((IntPtr)actionManager == IntPtr.Zero) return false;

        if (actionManager->GetActionStatus(Type, AdjustedID) != 0) return false;

        return true;
    }

    public unsafe bool Use()
    {
        var actionManager = ActionManager.Instance();
        if ((IntPtr)actionManager == IntPtr.Zero) return false;

        return actionManager->UseAction(Type, AdjustedID);
    }
}
