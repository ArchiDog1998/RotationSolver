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
    TargetResult? Target { get; }
    TargetResult? PreviewTarget { get; }
    ActionTargetInfo TargetInfo { get; }

    ActionBasicInfo Info { get; }
    ActionCooldownInfo Cooldown { get; }
    ActionSetting Setting { get; set; }
    internal ActionConfig Config { get; }   

    bool CanUse(out IAction act, bool skipStatusProvideCheck = false, bool skipCombo = false, bool ignoreCastingCheck = false,
        bool isEmpty = false, bool onLastAbility = false, bool ignoreClippingCheck = false, bool skipAoeCheck = false, byte gcdCountForAbility = 0);
}
