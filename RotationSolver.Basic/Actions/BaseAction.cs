using ECommons.DalamudServices;
using ECommons.GameHelpers;
using FFXIVClientStructs.FFXIV.Client.Game;
using Action = Lumina.Excel.GeneratedSheets.Action;

namespace RotationSolver.Basic.Actions;

/// <summary>
/// The base action for all actions.
/// </summary>
public class BaseAction : IBaseAction
{
    /// <inheritdoc/>
    public TargetResult Target { get; set; } = new(Player.Object, [], null);

    /// <inheritdoc/>
    public TargetResult? PreviewTarget { get; private set; } = null;

    /// <inheritdoc/>
    public Action Action { get; }

    /// <inheritdoc/>
    public ActionTargetInfo TargetInfo { get; }

    /// <inheritdoc/>
    public ActionBasicInfo Info { get; }

    /// <inheritdoc/>
    public ActionCooldownInfo Cooldown { get; }

    ICooldown IAction.Cooldown => Cooldown;

    /// <inheritdoc/>
    public uint ID => Info.ID;

    /// <inheritdoc/>
    public uint AdjustedID => Info.AdjustedID;

    /// <inheritdoc/>
    public float AnimationLockTime => ActionManagerHelper.GetCurrentAnimationLock();

    /// <inheritdoc/>
    public uint SortKey => Cooldown.CoolDownGroup;

    /// <inheritdoc/>
    public uint IconID => Info.IconID;

    /// <inheritdoc/>
    public string Name => Info.Name;


    /// <inheritdoc/>
    public string Description => string.Empty;

    /// <inheritdoc/>
    public byte Level => Info.Level;

    /// <inheritdoc/>
    public bool IsEnabled 
    { 
        get => Config.IsEnabled; 
        set => Config.IsEnabled = value;
    }

    /// <inheritdoc/>
    public bool IsInCooldown
    {
        get => Config.IsInCooldown;
        set => Config.IsInCooldown = value;
    }

    /// <inheritdoc/>
    public bool EnoughLevel => Info.EnoughLevel;

    /// <inheritdoc/>
    public ActionSetting Setting { get; set; }

    /// <inheritdoc/>
    public ActionConfig Config
    {
        get
        {
            if (!Service.Config.RotationActionConfig.TryGetValue(ID, out var value))
            {
                Service.Config.RotationActionConfig[ID] = value 
                    = Setting.CreateConfig?.Invoke() ?? new();
                if (Action.ClassJob.Value?.GetJobRole() == JobRole.Tank)
                {
                    value.AoeCount = 2;
                }
                if (value.TimeToUntargetable == 0)
                {
                    value.TimeToUntargetable = value.TimeToKill;
                }
                if (Setting.TargetStatusProvide != null)
                {
                    value.TimeToKill = 10;
                }
            }
            return value;
        }
    }

    /// <summary>
    /// The default constructor
    /// </summary>
    /// <param name="actionID">action id</param>
    /// <param name="isDutyAction">is this action a duty action</param>
    public BaseAction(ActionID actionID, bool isDutyAction = false)
    {
        Action = Service.GetSheet<Action>().GetRow((uint)actionID)!;
        TargetInfo = new(this);
        Info = new(this, isDutyAction);
        Cooldown = new(this);

        Setting = new();
    }

    /// <inheritdoc/>
    public bool CanUse(out IAction act, bool skipStatusProvideCheck = false, bool skipComboCheck = false, bool skipCastingCheck = false, 
        bool usedUp = false, bool onLastAbility = false, bool skipClippingCheck = false, bool skipAoeCheck = false, byte gcdCountForAbility = 0)
    {
        act = this!;

        if (IBaseAction.ActionPreview)
        {
            skipCastingCheck = skipClippingCheck = true;
        }
        else
        {
            Setting.EndSpecial = IBaseAction.ShouldEndSpecial;
        }
        if (IBaseAction.AllEmpty)
        {
            usedUp = true;
        }
        if (IBaseAction.IgnoreClipping)
        {
            skipClippingCheck = true;
        }

        if (!Info.BasicCheck(skipStatusProvideCheck, skipComboCheck, skipCastingCheck)) return false;

        if (!Cooldown.CooldownCheck(usedUp, onLastAbility, skipClippingCheck, gcdCountForAbility)) return false;


        if (Setting.SpecialType is SpecialActionType.MeleeRange
            && IActionHelper.IsLastAction(IActionHelper.MovingActions)) return false; //No range actions after moving.

        if (DataCenter.AverageTimeToKill < Config.TimeToKill) return false;
        if (DataCenter.AverageTimeToKill < Config.TimeToUntargetable) return false;

        PreviewTarget = TargetInfo.FindTarget(skipAoeCheck, skipStatusProvideCheck);
        if (PreviewTarget == null) return false;
        if (!IBaseAction.ActionPreview)
        {
            Target = PreviewTarget.Value;
        }

        return true;
    }

    /// <inheritdoc/>
    public bool CanUse(out IAction act, CanUseOption option, byte gcdCountForAbility = 0)
    {
        return CanUse(out act, 
            option.HasFlag(CanUseOption.SkipStatusProvideCheck),
            option.HasFlag(CanUseOption.SkipComboCheck),
            option.HasFlag(CanUseOption.SkipCastingCheck),
            option.HasFlag(CanUseOption.UsedUp),
            option.HasFlag(CanUseOption.OnLastAbility),
            option.HasFlag(CanUseOption.SkipClippingCheck),
            option.HasFlag(CanUseOption.SkipAoeCheck),
            gcdCountForAbility);
    }

    /// <inheritdoc/>
    public unsafe bool Use()
    {
        var target = Target;

        var adjustId = AdjustedID;
        if (TargetInfo.IsTargetArea)
        {
            if (adjustId != ID) return false;
            if (!target.Position.HasValue) return false;

            var loc = (FFXIVClientStructs.FFXIV.Common.Math.Vector3)target.Position;

            return ActionManager.Instance()->UseActionLocation(ActionType.Action, ID, Player.Object.ObjectId, &loc);
        }
        else if (Svc.Objects.SearchById(target.Target?.ObjectId 
            ?? Player.Object?.ObjectId ?? GameObject.InvalidGameObjectId) == null)
        {
            return false;
        }
        else
        {
            return ActionManager.Instance()->UseAction(ActionType.Action, adjustId, target.Target?.ObjectId ?? GameObject.InvalidGameObjectId);
        }
    }

    /// <inheritdoc/>
    public override string ToString() => Name;
}
