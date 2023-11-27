using ECommons.GameHelpers;
using FFXIVClientStructs.FFXIV.Client.Game;
using RotationSolver.Basic.Configuration;
using Action = Lumina.Excel.GeneratedSheets.Action;

namespace RotationSolver.Basic.Actions;

/// <summary>
/// The action things around <see cref="Action"/>.
/// </summary>
public partial class BaseAction : IBaseAction
{
    internal static CanUseOption OtherOption { get; set; } = CanUseOption.None;

    /// <summary>
    /// The action itself.
    /// </summary>
    protected readonly Action _action;

    readonly ActionOption _option;

    /// <summary>
    /// Is a heal action.
    /// </summary>
    public bool IsHeal => _option.HasFlag(ActionOption.HealFlag);

    /// <summary>
    /// Is a friendly action.
    /// </summary>
    public bool IsFriendly
    {
        get
        {
            if (_action.CanTargetFriendly) return true;
            if (_action.CanTargetHostile) return false;
            return _option.HasFlag(ActionOption.Friendly);
        }
    }

    /// <summary>
    /// Is effect of time.
    /// </summary>
    public bool IsEot => _option.HasFlag(ActionOption.Eot);

    /// <summary>
    /// Should end special window after used it.
    /// </summary>
    public bool ShouldEndSpecial => _option.HasFlag(ActionOption.EndSpecial);

    /// <summary>
    /// Has a normal gcd action.
    /// </summary>
    public bool IsGeneralGCD => _option.HasFlag(ActionOption.GeneralGCD);

    /// <summary>
    /// Is a real gcd action, that makes gcd work.
    /// </summary>
    public bool IsRealGCD => _option.HasFlag(ActionOption.RealGCD);

    /// <inheritdoc/>
    public bool IsDutyAction => _option.HasFlag(ActionOption.DutyAction);

    /// <summary>
    /// Is a pvp action.
    /// </summary>
    public bool IsPvP => _action.IsPvP;

    /// <inheritdoc/>
    public bool IsLimitBreak => _action.ActionCategory?.Value?.RowId == 9;

    /// <inheritdoc/>
    public bool IsOnSlot
    {
        get
        {
            if (IsDutyAction)
            {
                return DataCenter.DutyActions.Contains(AdjustedID);
            }

            return IsPvP == DataCenter.Territory?.IsPvpZone;
        }
    }

    /// <summary>
    /// How many gcd left to add the dot.
    /// </summary>
    public Func<uint> GetDotGcdCount { private get; init; }

    /// <summary>
    /// Get enough level for using this action.
    /// </summary>
    public bool EnoughLevel => Player.Level >= Level;

    /// <summary>
    /// The level of this action.
    /// </summary>
    public byte Level => _action.ClassJobLevel;

    /// <summary>
    /// Attack Type
    /// </summary>
    public AttackType AttackType => (AttackType)(_action.AttackType.Value?.RowId ?? byte.MaxValue);

    /// <summary>
    /// The Aspect.
    /// </summary>
    public Aspect Aspect => (Aspect)_action.Aspect;

    /// <summary>
    /// The name of this action.
    /// </summary>
    public string Name => _action.Name;

    /// <summary>
    /// Description about this action.
    /// </summary>
    public virtual string Description => string.Empty;

    /// <summary>
    /// Is Enabled.
    /// </summary>
    public bool IsEnabled
    {
        get => !Service.Config.GlobalConfig.DisabledActions.Contains(ID);
        set
        {
            if (value)
            {
                Service.Config.GlobalConfig.DisabledActions.Remove(ID);
            }
            else
            {
                Service.Config.GlobalConfig.DisabledActions.Add(ID);
            }
        }
    }

    /// <inheritdoc/>
    public bool IsInCooldown
    {
        get => !Service.Config.GlobalConfig.NotInCoolDownActions.Contains(ID);
        set
        {
            if (value)
            {
                Service.Config.GlobalConfig.NotInCoolDownActions.Remove(ID);
            }
            else
            {
                Service.Config.GlobalConfig.NotInCoolDownActions.Add(ID);
            }
        }
    }

    /// <inheritdoc/>
    public bool IsInMistake
    {
        get => !Service.Config.GlobalConfig.NotInMistakeActions.Contains(ID);
        set
        {
            if (value)
            {
                Service.Config.GlobalConfig.NotInMistakeActions.Remove(ID);
            }
            else
            {
                Service.Config.GlobalConfig.NotInMistakeActions.Add(ID);
            }
        }
    }

    /// <summary>
    /// Action ID.
    /// </summary>
    public uint ID => _action.RowId;

    /// <summary>
    /// Adjusted action Id.
    /// </summary>
    public uint AdjustedID => (uint)Service.GetAdjustedActionId((ActionID)ID);

    /// <summary>
    /// Icon Id.
    /// </summary>
    public uint IconID => ID == (uint)ActionID.Sprint ? 104u : _action.Icon;

    private byte CoolDownGroup { get; }

    /// <summary>
    /// Casting time.
    /// </summary>
    public unsafe float CastTime => ActionManager.GetAdjustedCastTime(ActionType.Action, AdjustedID) / 1000f;

    /// <summary>
    /// Action Positional.
    /// </summary>
    public EnemyPositional EnemyPositional
    {
        get
        {
            if (ConfigurationHelper.ActionPositional.TryGetValue((ActionID)AdjustedID, out var location))
            {
                return location;
            }
            return EnemyPositional.None;
        }
    }

    /// <summary>
    /// Mp it needs.
    /// </summary>
    public virtual unsafe uint MPNeed
    {
        get
        {
            var mp = (uint)ActionManager.GetActionCost(ActionType.Action, AdjustedID, 0, 0, 0, 0);
            if (mp < 100) return 0;
            return mp;
        }
    }

    /// <summary>
    /// Sort for CD window.
    /// </summary>
    public uint SortKey => CoolDownGroup;

    /// <summary>
    /// Animation Lock Time.
    /// </summary>
    public float AnimationLockTime => OtherConfiguration.AnimationLockTime?.TryGetValue(AdjustedID, out var time) ?? false ? time : 0.6f;

    /// <summary>
    /// General Constructer.
    /// </summary>
    /// <param name="actionID"></param>
    /// <param name="option"></param>
    public BaseAction(ActionID actionID, ActionOption option = ActionOption.None)
    {
        _action = Service.GetSheet<Action>().GetRow((uint)actionID);

        option &= ~(ActionOption.GeneralGCD | ActionOption.RealGCD);
        if (_action.IsGeneralGCD()) option |= ActionOption.GeneralGCD;
        if (_action.IsRealGCD()) option |= ActionOption.RealGCD;
        _option = option;

        CoolDownGroup = _action.GetCoolDownGroup();
    }

    internal static void CleanSpecial()
    {
        OtherOption = CanUseOption.None;
        AutoHealCheck = SkipDisable = false;
    }

    /// <summary>
    /// To string.
    /// </summary>
    /// <returns>string.</returns>
    public override string ToString() => Name;
}
