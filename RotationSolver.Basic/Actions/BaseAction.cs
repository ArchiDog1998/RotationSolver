using ECommons.DalamudServices;
using ECommons.GameHelpers;
using FFXIVClientStructs.FFXIV.Client.Game;
using Action = Lumina.Excel.GeneratedSheets.Action;

namespace RotationSolver.Basic.Actions;
public class BaseAction : IBaseAction
{
    /// <inheritdoc/>
    public TargetResult? Target { get; set; } = null;

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
    public float AnimationLockTime => Info.AnimationLockTime;

    /// <inheritdoc/>
    public uint SortKey => Cooldown.CoolDownGroup;

    /// <inheritdoc/>
    public uint IconID => ID == 3 ? 104 : Info.IconID;

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
    public bool CanUse(out IAction act, bool skipStatusProvideCheck = false, bool skipCombo = false, bool skipCastingCheck = false, 
        bool usedUp = false, bool onLastAbility = false, bool skipClippingCheck = false, bool skipAoeCheck = false, byte gcdCountForAbility = 0)
    {
        act = this!;

        Setting.EndSpecial = IBaseAction.ShouldEndSpecial;

        if (IBaseAction.ActionPreview)
        {
            skipCastingCheck = skipClippingCheck = true;
        }
        if (IBaseAction.AllEmpty)
        {
            usedUp = true;
        }
        if (IBaseAction.IgnoreClipping)
        {
            skipClippingCheck = true;
        }

        if (!Info.BasicCheck(skipStatusProvideCheck, skipCombo, skipCastingCheck)) return false;

        if (!Cooldown.CooldownCheck(usedUp, onLastAbility, skipClippingCheck, gcdCountForAbility)) return false;


        if (Setting.IsMeleeRange && IActionHelper.IsLastAction(IActionHelper.MovingActions)) return false; //No range actions after moving.
        if (DataCenter.AverageTimeToKill < Config.TimeToKill) return false;

        PreviewTarget = TargetInfo.FindTarget(skipAoeCheck, skipStatusProvideCheck);
        if (PreviewTarget == null) return false;
        if (!IBaseAction.ActionPreview)
        {
            Target = PreviewTarget;
        }

        return true;
    }

    /// <inheritdoc/>
    public bool CanUse(out IAction act, CanUseOption option, byte gcdCountForAbility = 0)
    {
        return CanUse(out act, 
            option.HasFlag(CanUseOption.SkipStatusProvideCheck),
            option.HasFlag(CanUseOption.SkipCombo),
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
        if (!Target.HasValue) return false;

        var target = Target.Value;

        var adjustId = AdjustedID;
        if (TargetInfo.TargetArea)
        {
            if (adjustId != ID) return false;
            if (!target.Position.HasValue) return false;

            var loc = (FFXIVClientStructs.FFXIV.Common.Math.Vector3)target.Position;

            return ActionManager.Instance()->UseActionLocation(ActionType.Action, ID, Player.Object.ObjectId, &loc);
        }
        else if (Svc.Objects.SearchById(target.Target?.ObjectId ?? GameObject.InvalidGameObjectId) == null)
        {
            return false;
        }
        else
        {
            return ActionManager.Instance()->UseAction(ActionType.Action, adjustId, target.Target?.ObjectId ?? GameObject.InvalidGameObjectId);
        }
    }
}
