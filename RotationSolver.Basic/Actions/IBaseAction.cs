using Action = Lumina.Excel.GeneratedSheets.Action;

namespace RotationSolver.Basic.Actions;

public interface IBaseAction : IAction
{
    internal static TargetType? TargetOverride { get; set; } = null;
    internal static bool ForceEnable { get; set; } = false;
    internal static bool AutoHealCheck { get; set; } = false;
    internal static bool ActionPreview { get; set; } = false;
    internal static bool IgnoreClipping { get; set; } = false;
    internal static bool AllEmpty { get; set; } = false;
    internal static bool ShouldEndSpecial { get; set; } = false;

    Action Action { get; }
    TargetResult? Target { get; set; }
    TargetResult? PreviewTarget { get; }
    ActionTargetInfo TargetInfo { get; }
    ActionBasicInfo Info { get; }
    new ActionCooldownInfo Cooldown { get; }
    ActionSetting Setting { get; set; }
    internal ActionConfig Config { get; }   

    bool CanUse(out IAction act, bool skipStatusProvideCheck = false, bool skipCombo = false, bool skipCastingCheck = false,
        bool usedUp = false, bool onLastAbility = false, bool skipClippingCheck = false, bool skipAoeCheck = false, byte gcdCountForAbility = 0);

    bool CanUse(out IAction act, CanUseOption option, byte gcdCountForAbility = 0);
}
