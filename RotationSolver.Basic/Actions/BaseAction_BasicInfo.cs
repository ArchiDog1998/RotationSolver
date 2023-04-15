using FFXIVClientStructs.FFXIV.Client.Game;
using Action = Lumina.Excel.GeneratedSheets.Action;

namespace RotationSolver.Basic.Actions;

public partial class BaseAction : IBaseAction
{
    Action _action;

    ActionOption _option;

    public bool IsFriendly => _option.HasFlag(ActionOption.Friendly);
    public bool IsEot => _option.HasFlag(ActionOption.Eot);
    public bool ShouldEndSpecial => _option.HasFlag(ActionOption.EndSpecial);
    public bool IsTimeline => _option.HasFlag(ActionOption.Timeline) && IsFriendly;
    public bool IsGeneralGCD => _option.HasFlag(ActionOption.GeneralGCD);
    public bool IsRealGCD => _option.HasFlag(ActionOption.RealGCD);


    public Func<uint> GetDotGcdCount { private get; set; }

    /// <summary>
    /// EnoughLevel for using.
    /// </summary>
    public bool EnoughLevel => Service.Player.Level >= Level;
    public byte Level => _action.ClassJobLevel;
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

    public bool IsInCooldown
    {
        get => !Service.Config.NotInCoolDownActions.Contains(ID);
        set
        {
            if (value)
            {
                Service.Config.NotInCoolDownActions.Remove(ID);
            }
            else
            {
                Service.Config.NotInCoolDownActions.Add(ID);
            }
        }
    }
    public uint ID => _action.RowId;
    public uint AdjustedID => (uint)Service.GetAdjustedActionId((ActionID)ID);

    public uint IconID => _action.Icon;

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

    public BaseAction(ActionID actionID)
       : this(actionID, ActionOption.None)
    {

    }


    public BaseAction(ActionID actionID, ActionOption option = ActionOption.None)
    {
        _action = Service.GetSheet<Action>().GetRow((uint)actionID);

        option &= ~(ActionOption.GeneralGCD | ActionOption.RealGCD);
        if(_action.IsGeneralGCD()) option |= ActionOption.GeneralGCD;
        if(_action.IsRealGCD()) option |= ActionOption.RealGCD;
        _option = option;

        CoolDownGroup = _action.GetCoolDownGroup();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="actionID"></param>
    /// <param name="isFriendly">is a friendly or supporting action</param>
    /// <param name="endSpecial">end special after using it</param>
    /// <param name="isEot">is hot or dot action</param>
    /// <param name="isTimeline">should I put it to the timeline (heal and defense only)</param>
    [Obsolete("Please use the ActionOption one", false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public BaseAction(ActionID actionID, bool isFriendly = false, bool endSpecial = false, bool isEot = false, bool isTimeline = false)
        : this(actionID, GetOption(isFriendly, endSpecial, isEot, isTimeline))
    {

    }

    private static ActionOption GetOption(bool isFriendly = false, bool endSpecial = false, bool isEot = false, bool isTimeline = false)
    {
        ActionOption option = ActionOption.None;
        if (isFriendly)  option |= ActionOption.Friendly;
        if (endSpecial)  option |= ActionOption.EndSpecial;
        if (isEot)  option |= ActionOption.Eot;
        if (isTimeline)  option |= ActionOption.Friendly;
        return option;
    }

    public override string ToString() => Name;
}
