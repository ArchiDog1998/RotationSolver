namespace RotationSolver.Basic.Data;

/// <summary>
/// Special State.
/// </summary>
public enum SpecialCommandType : byte
{
    /// <summary>
    /// 
    /// </summary>
    EndSpecial,

    /// <summary>
    /// 
    /// </summary>
    HealArea,

    /// <summary>
    /// 
    /// </summary>
    HealSingle,

    /// <summary>
    /// 
    /// </summary>
    DefenseArea,

    /// <summary>
    /// 
    /// </summary>
    DefenseSingle,

    /// <summary>
    /// 
    /// </summary>
    EsunaStanceNorth,

    /// <summary>
    /// 
    /// </summary>
    RaiseShirk,

    /// <summary>
    /// 
    /// </summary>
    MoveForward,

    /// <summary>
    /// 
    /// </summary>
    MoveBack,

    /// <summary>
    /// 
    /// </summary>
    AntiKnockback,

    /// <summary>
    /// 
    /// </summary>
    Burst,

    /// <summary>
    /// 
    /// </summary>
    Speed,

    /// <summary>
    /// 
    /// </summary>
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
    Cancel,

    /// <summary>
    /// 
    /// </summary>
    Auto,

    /// <summary>
    /// 
    /// </summary>
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
