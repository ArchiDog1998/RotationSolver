namespace RotationSolver.Basic.Data;

/// <summary>
/// Special State.
/// </summary>
public enum SpecialCommandType : byte
{
    /// <summary>
    /// 
    /// </summary>
    [Description("To end this special duration before the set time.")]
    EndSpecial,

    /// <summary>
    /// 
    /// </summary>
    [Description("Open a window to use AoE heal.")]
    HealArea,

    /// <summary>
    /// 
    /// </summary>
    [Description("Open a window to use single heal.")]
    HealSingle,

    /// <summary>
    /// 
    /// </summary>
    [Description("Open a window to use AoE defense.")]
    DefenseArea,

    /// <summary>
    /// 
    /// </summary>
    [Description("Open a window to use single defense.")]
    DefenseSingle,

    /// <summary>
    /// 
    /// </summary>
    [Description("Open a window to use Esuna, tank stance actions or True North.")]
    DispelStancePositional,

    /// <summary>
    /// 
    /// </summary>
    [Description("Open a window to use Raise or Shirk.")]
    RaiseShirk,

    /// <summary>
    /// 
    /// </summary>
    [Description("Open a window to move forward.")]
    MoveForward,

    /// <summary>
    /// 
    /// </summary>
    [Description("Open a window to move back.")]
    MoveBack,

    /// <summary>
    /// 
    /// </summary>
    [Description("Open a window to use knockback immunity actions.")]
    AntiKnockback,

    /// <summary>
    /// 
    /// </summary>
    [Description("Open a window to burst.")]
    Burst,

    /// <summary>
    /// 
    /// </summary>
    [Description("Open a window to speed up.")]
    Speed,

    /// <summary>
    /// 
    /// </summary>
    [Description("Open a window to use limit break.")]
    LimitBreak,

    /// <summary>
    /// 
    /// </summary>
    None,
}

/// <summary>
/// The state of the plugin.
/// </summary>
public enum StateCommandType : byte
{
    /// <summary>
    /// 
    /// </summary>
    [Description("Stop the addon. Always remember to turn it off when it is not in use!")]
    Cancel,

    /// <summary>
    /// 
    /// </summary>
    [Description("Start the addon in Auto mode. When out of combat or when combat starts, switches the target according to the set condition.")]
    Auto,

    /// <summary>
    /// 
    /// </summary>
    [Description("Start the addon in Manual mode. You need to choose the target manually. This will bypass any engage settings that you have set up and will start attacking immediately once something is targeted.")]
    Manual,

    /// <summary>
    /// 
    /// </summary>
    None,
}

/// <summary>
/// Some Other Commands.
/// </summary>
public enum OtherCommandType : byte
{
    /// <summary>
    /// 
    /// </summary>
    Settings,

    /// <summary>
    /// 
    /// </summary>
    Rotations,

    /// <summary>
    /// 
    /// </summary>
    DoActions,

    /// <summary>
    /// 
    /// </summary>
    ToggleActions,

    /// <summary>
    /// 
    /// </summary>
    [Description("Do the next action")]
    NextAction,
}

/// <summary>
/// Some settings that can be used in command.
/// </summary>
public enum SettingsCommand : byte
{
    /// <summary>
    /// 
    /// </summary>
    AutoBurst,

    /// <summary>
    /// 
    /// </summary>
    AutoHeal,

    /// <summary>
    /// 
    /// </summary>
    UseAbility,

    /// <summary>
    /// 
    /// </summary>
    UseDefenseAbility,

    /// <summary>
    /// 
    /// </summary>
    AutoTankStance,

    /// <summary>
    /// 
    /// </summary>
    AutoProvokeForTank,

    /// <summary>
    /// 
    /// </summary>
    AutoUseTrueNorth,

    /// <summary>
    /// 
    /// </summary>
    RaisePlayerBySwift,

    /// <summary>
    /// 
    /// </summary>
    UseGroundBeneficialAbility,

    /// <summary>
    /// 
    /// </summary>
    UseAOEAction,

    /// <summary>
    /// 
    /// </summary>
    UseAOEWhenManual,

    /// <summary>
    /// 
    /// </summary>
    PreventActions,

    /// <summary>
    /// 
    /// </summary>
    PreventActionsDuty,

    /// <summary>
    /// 
    /// </summary>
    AutoLoadCustomRotations,

    /// <summary>
    /// 
    /// </summary>
    AutoSpeedOutOfCombat,

    /// <summary>
    /// 
    /// </summary>
    TargetAllForFriendly,
}

/// <summary>
/// Extension
/// </summary>
public static class SettingsCommandExtension
{
    /// <summary>
    /// Get the default value of the command.
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    public static bool GetDefault(this SettingsCommand command) => command switch
    {
        SettingsCommand.AutoBurst => true,
        SettingsCommand.AutoHeal => true,
        SettingsCommand.UseAbility => true,
        SettingsCommand.UseDefenseAbility => true,
        SettingsCommand.AutoTankStance => true,
        SettingsCommand.AutoProvokeForTank => true,
        SettingsCommand.AutoUseTrueNorth => true,
        SettingsCommand.RaisePlayerBySwift => true,
        SettingsCommand.UseGroundBeneficialAbility => true,
        SettingsCommand.UseAOEAction => true,
        SettingsCommand.UseAOEWhenManual => false,
        SettingsCommand.PreventActions => false,
        SettingsCommand.PreventActionsDuty => false,
        SettingsCommand.AutoSpeedOutOfCombat => true,
        SettingsCommand.TargetAllForFriendly => false,
        _ => false,
    };
}
