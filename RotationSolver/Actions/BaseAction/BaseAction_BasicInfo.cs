using FFXIVClientStructs.FFXIV.Client.Game;
using RotationSolver.Data;
using RotationSolver.Helpers;
using RotationSolver.Localization;
using System;
using Action = Lumina.Excel.GeneratedSheets.Action;

namespace RotationSolver.Actions.BaseAction;

internal partial class BaseAction : IBaseAction
{

    private bool _isFriendly;
    private bool _isEot;
    Action _action;

    private bool ShouldEndSpecial { get; set; }
    internal bool IsTimeline { get; } = false;

    public Func<uint> GetDotGcdCount { private get; set; }

    /// <summary>
    /// EnoughLevel for using.
    /// </summary>
    public bool EnoughLevel => Service.ClientState.LocalPlayer.Level >= _action.ClassJobLevel;
    public string Name => _action.Name;

    public string Description => string.Empty;

    public string CateName
    {
        get
        {
            string result;

            if (_isFriendly)
            {
                result = LocalizationManager.RightLang.Action_Friendly;
                if (_isEot)
                {
                    result += "Hot";
                }
            }
            else
            {
                result = LocalizationManager.RightLang.Action_Attack;

                if (_isEot)
                {
                    result += "Dot";
                }
            }
            result += "-" + (IsRealGCD ? "GCD" : LocalizationManager.RightLang.Timeline_Ability);
            return result;
        }
    }
    public bool IsEnabled
    {
        get => !Service.Configuration.DiabledActions.Contains(ID);
        set
        {
            if (value)
            {
                Service.Configuration.DiabledActions.Remove(ID);
            }
            else
            {
                Service.Configuration.DiabledActions.Add(ID);
            }
        }
    }
    public uint ID => _action.RowId;
    public uint AdjustedID => (uint)Service.IconReplacer.OriginalHook((ActionID)ID);

    public uint IconID => _action.Icon;

    private bool IsGeneralGCD { get; }

    public bool IsRealGCD { get; }

    private byte CoolDownGroup { get; }

    public unsafe float CastTime => ActionManager.GetAdjustedCastTime(ActionType.Spell, AdjustedID) / 1000f;

    public virtual EnemyPositional EnermyPositonal
    {
        get
        {
            if (ConfigurationHelper.ActionPositionals.TryGetValue((ActionID)ID, out var location))
            {
                return location.Pos;
            }
            return EnemyPositional.None;
        }
    }

    public virtual unsafe uint MPNeed
    {
        get
        {
            var mp = (uint)ActionManager.GetActionCost(ActionType.Spell, AdjustedID, 0, 0, 0, 0);
            if (mp < 100) return 0;
            return mp;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="actionID"></param>
    /// <param name="isFriendly">is a friendly or supporting action</param>
    /// <param name="shouldEndSpecial">end special after using it</param>
    /// <param name="isEot">is hot or dot action</param>
    /// <param name="isTimeline">should I put it to the timeline (heal and defense only)</param>
    internal BaseAction(ActionID actionID, bool isFriendly = false, bool shouldEndSpecial = false, bool isEot = false, bool isTimeline = false)
    {
        _action = Service.DataManager.GetExcelSheet<Action>().GetRow((uint)actionID);
        ShouldEndSpecial = shouldEndSpecial;
        _isFriendly = isFriendly;
        _isEot = isEot;
        IsTimeline = isTimeline;

        IsGeneralGCD = _action.IsGeneralGCD();
        IsRealGCD = _action.IsRealGCD();
        CoolDownGroup = _action.GetCoolDownGroup();
    }

    public override string ToString() => Name;
}
