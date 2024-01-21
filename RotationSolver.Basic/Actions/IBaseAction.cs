using Action = Lumina.Excel.GeneratedSheets.Action;

namespace RotationSolver.Basic.Actions;
public interface IBaseAction : IAction
{
    Action Action { get; }

    ActionTargetInfo TargetInfo { get; }
    ActionBasicInfo Info { get; }
    ActionCooldownInfo Cooldown { get; }
    ActionSetting Setting { get; set; }
    ActionConfig Config { get; set; }   

    bool CanUse(out IAction act, bool skipStatusProvideCheck = false, bool skipCombo = false, bool ignoreCastingCheck = false,
        bool isEmpty = false, bool onLastAbility = false, bool ignoreClippingCheck = false, byte gcdCountForAbility = 0);
}
