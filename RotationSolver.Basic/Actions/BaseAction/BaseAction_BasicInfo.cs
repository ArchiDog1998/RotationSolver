using FFXIVClientStructs.FFXIV.Client.Game;
using RotationSolver.Basic;
using RotationSolver.Basic.Actions;
using RotationSolver.Basic.Data;
using RotationSolver.Basic.Helpers;
using Action = Lumina.Excel.GeneratedSheets.Action;

namespace RotationSolver.Actions.BaseAction;

public partial class BaseAction : IBaseAction
{

    public bool IsFriendly { get; }
    public bool IsEot { get; }
    Action _action;

    public bool ShouldEndSpecial { get; private set; }
    public bool IsTimeline { get; } = false;

    public Func<uint> GetDotGcdCount { private get; set; }

    /// <summary>
    /// EnoughLevel for using.
    /// </summary>
    public bool EnoughLevel => Service.Player.Level >= _action.ClassJobLevel;
    public string Name => _action.Name;

    public string Description => string.Empty;

    public bool IsEnabled
    {
        get => !Service.Config.DisabledActions.Contains(ID);
        set
        {
            if (value)
            {
                Service.Config.DisabledActions.Remove(ID);
            }
            else
            {
                Service.Config.DisabledActions.Add(ID);
            }
        }
    }
    public uint ID => _action.RowId;
    public uint AdjustedID => (uint)Service.GetAdjustedActionId((ActionID)ID);

    public uint IconID => _action.Icon;

    private bool IsGeneralGCD { get; }

    public bool IsRealGCD { get; }

    private byte CoolDownGroup { get; }

    public unsafe float CastTime => ActionManager.GetAdjustedCastTime(ActionType.Spell, AdjustedID) / 1000f;

    public virtual EnemyPositional EnemyPositional
    {
        get
        {
            if (ConfigurationHelper.ActionPositional.TryGetValue((ActionID)ID, out var location))
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
    public BaseAction(ActionID actionID, bool isFriendly = false, bool shouldEndSpecial = false, bool isEot = false, bool isTimeline = false)
    {
        _action = Service.GetSheet<Action>().GetRow((uint)actionID);
        ShouldEndSpecial = shouldEndSpecial;
        IsFriendly = isFriendly;
        IsEot = isEot;
        IsTimeline = isTimeline;

        IsGeneralGCD = _action.IsGeneralGCD();
        IsRealGCD = _action.IsRealGCD();
        CoolDownGroup = _action.GetCoolDownGroup();
    }

    public override string ToString() => Name;
}
