using ECommons.DalamudServices;
using ECommons.GameHelpers;
using FFXIVClientStructs.FFXIV.Client.Game;
using Action = Lumina.Excel.GeneratedSheets.Action;

namespace RotationSolver.Basic.Actions;
public class BaseAction : IBaseAction
{
    public TargetResult? Target { get; private set; } = null;

    public Action Action { get; }

    public ActionTargetInfo TargetInfo { get; }

    public ActionBasicInfo Info { get; }

    public ActionCooldownInfo Cooldown { get; }

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
        get => Config.IsEnable; 
        set
        {
            var config = Config;
            config.IsEnable = value;
            Config = config;
        }
    }
    public bool IsInCooldown
    {
        get => Config.IsInCooldown;
        set
        {
            var config = Config;
            config.IsInCooldown = value;
            Config = config;
        }
    }

    public bool EnoughLevel => Info.EnoughLevel;
    public virtual unsafe uint MPNeed
    {
        get
        {
            var mp = (uint)ActionManager.GetActionCost(ActionType.Action, AdjustedID, 0, 0, 0, 0);
            if (mp < 100) return 0;
            return mp;
        }
    }

    public ActionSetting Setting { get; set; }

    public ActionConfig Config { get; set; }

    public BaseAction(ActionID actionID, bool isDutyAction)
    {
        Action = Service.GetSheet<Action>().GetRow((uint)actionID)!;
        TargetInfo = new(this);
        Info = new(this, isDutyAction);
        Cooldown = new(this);

        Setting = new();
        Config = new();
    }

    public bool CanUse(out IAction act, bool skipStatusProvideCheck = false, bool skipCombo = false, bool ignoreCastingCheck = false, 
        bool isEmpty = false, bool onLastAbility = false, bool ignoreClippingCheck = false, byte gcdCountForAbility = 0)
    {
        act = this!;

        if (!Info.BasicCheck(skipStatusProvideCheck, skipCombo, ignoreCastingCheck)) return false;
        if (!Cooldown.CooldownCheck(isEmpty, onLastAbility, ignoreClippingCheck, gcdCountForAbility)) return false;

        if (DataCenter.CurrentMp < MPNeed) return false;
        if (Setting.IsFriendly && DataCenter.AverageTimeToKill < Config.TimeToKill) return false;

        Target = TargetInfo.FindTarget();
        if (Target == null) return false;
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

            var loc = (FFXIVClientStructs.FFXIV.Common.Math.Vector3)target.Position;

            return ActionManager.Instance()->UseActionLocation(ActionType.Action, ID, Player.Object.ObjectId, &loc);
        }
        else if (Svc.Objects.SearchById(target.Target.ObjectId) == null)
        {
            return false;
        }
        else
        {
            return ActionManager.Instance()->UseAction(ActionType.Action, adjustId, target.Target.ObjectId);
        }
    }
}
