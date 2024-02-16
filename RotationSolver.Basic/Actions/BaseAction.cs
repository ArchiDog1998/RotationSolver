using ECommons.DalamudServices;
using ECommons.GameHelpers;
using FFXIVClientStructs.FFXIV.Client.Game;
using Action = Lumina.Excel.GeneratedSheets.Action;

namespace RotationSolver.Basic.Actions;
public class BaseAction : IBaseAction
{
    public TargetResult? Target { get; private set; } = null;
    public TargetResult? PreviewTarget { get; private set; } = null;

    public Action Action { get; }

    public ActionTargetInfo TargetInfo { get; }

    public ActionBasicInfo Info { get; }

    public ActionCooldownInfo Cooldown { get; }

    ICooldown IAction.Cooldown => Cooldown;

    public uint ID => Info.ID;

    public uint AdjustedID => Info.AdjustedID;

    public float AnimationLockTime => Info.AnimationLockTime;

    public uint SortKey => Cooldown.CoolDownGroup;

    public bool IsCoolingDown => Cooldown.IsCoolingDown;


    public uint IconID => Info.IconID;

    public string Name => Info.Name;

    public string Description => string.Empty;

    public byte Level => Info.Level;
    public bool IsEnabled 
    { 
        get => Config.IsEnabled; 
        set => Config.IsEnabled = value;
    }
    public bool IsInCooldown
    {
        get => Config.IsInCooldown;
        set => Config.IsInCooldown = value;
    }

    public bool EnoughLevel => Info.EnoughLevel;

    public ActionSetting Setting { get; set; }

    public ActionConfig Config
    {
        get
        {
            if (!Service.Config.RotationActionConfig.TryGetValue(ID, out var value))
            {
                Service.Config.RotationActionConfig[ID] = value 
                    = Setting.CreateConfig?.Invoke() ?? new();
            }
            return value;
        }
    }

    public BaseAction(ActionID actionID, bool isDutyAction = false)
    {
        Action = Service.GetSheet<Action>().GetRow((uint)actionID)!;
        TargetInfo = new(this);
        Info = new(this, isDutyAction);
        Cooldown = new(this);

        Setting = new();
    }

    public bool CanUse(out IAction act, bool skipStatusProvideCheck = false, bool skipCombo = false, bool ignoreCastingCheck = false, 
        bool isEmpty = false, bool onLastAbility = false, bool ignoreClippingCheck = false, bool skipAoeCheck = false, byte gcdCountForAbility = 0)
    {
        act = this!;
        Setting.EndSpecial = IBaseAction.ShouldEndSpecial;

        if (IBaseAction.ActionPreview)
        {
            ignoreCastingCheck = ignoreClippingCheck = true;
        }
        if (IBaseAction.AllEmpty)
        {
            isEmpty = true;
        }
        if (IBaseAction.IgnoreClipping)
        {
            ignoreClippingCheck = true;
        }

        if (!Info.BasicCheck(skipStatusProvideCheck, skipCombo, ignoreCastingCheck)) return false;
        if (!Cooldown.CooldownCheck(isEmpty, onLastAbility, ignoreClippingCheck, gcdCountForAbility)) return false;

        if (Setting.IsMeleeRange && IActionHelper.IsLastAction(IActionHelper.MovingActions)) return false; //No range actions after moving.
        if (Setting.IsFriendly && DataCenter.AverageTimeToKill < Config.TimeToKill) return false;

        PreviewTarget = TargetInfo.FindTarget(skipAoeCheck);
        if (PreviewTarget == null) return false;
        if (!IBaseAction.ActionPreview)
        {
            Target = PreviewTarget;
        }

        return true;
    }

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
