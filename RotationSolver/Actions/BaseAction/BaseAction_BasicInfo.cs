using FFXIVClientStructs.FFXIV.Client.Game;
using RotationSolver;
using RotationSolver.Actions;
using RotationSolver.Data;
using RotationSolver.Helpers;
using RotationSolver.Localization;
using Action = Lumina.Excel.GeneratedSheets.Action;

namespace RotationSolver.Actions.BaseAction;

internal partial class BaseAction : IBaseAction
{

    private bool _isFriendly;
    private bool _isEot;

    public bool ShouldEndSpecial { private get; set; }

    public bool IsTimeline { get; } = false;
    /// <summary>
    /// 玩家当前等级是否大于等于技能可用等级
    /// </summary>
    internal bool EnoughLevel => Service.ClientState.LocalPlayer.Level >= _action.ClassJobLevel;
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
            result += "-" + (IsRealGCD ? "GCD" : LocalizationManager.RightLang.Scriptwindow_Ability);
            return result;
        }
    }
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
    /// <summary>
    /// 技能ID
    /// </summary>
    public uint ID => _action.RowId;
    /// <summary>
    /// 调整后的ID(真实技能ID)
    /// </summary>
    public uint AdjustedID => (uint)Service.IconReplacer.OriginalHook((ActionID)ID);

    public uint IconID => _action.Icon;

    private bool IsGeneralGCD { get; }

    internal bool IsRealGCD { get; }

    private byte CoolDownGroup { get; }

    /// <summary>
    /// 范围类技能至少需要多少个对象才释放
    /// </summary>
    public byte AOECount { private get; set; } = 3;

    /// <summary>
    /// 真实咏唱时间
    /// </summary>
    internal unsafe float CastTime => ActionManager.GetAdjustedCastTime(ActionType.Spell, AdjustedID) / 1000f;

    internal virtual EnemyPositional EnermyPositonal
    {
        get
        {
            if (StatusHelper.ActionLocations.TryGetValue((ActionID)ID, out var location))
            {
                return location.Loc;
            }
            return EnemyPositional.None;
        }
    }
    internal virtual unsafe uint MPNeed
    {
        get
        {
            var mp = (uint)ActionManager.GetActionCost(ActionType.Spell, AdjustedID, 0, 0, 0, 0);
            if (mp < 100) return 0;
            return mp;
        }
    }


    Action _action;
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
