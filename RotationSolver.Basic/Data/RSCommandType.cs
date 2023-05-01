namespace RotationSolver.Basic.Data;

public enum SpecialCommandType : byte
{
    EndSpecial,
    HealArea,
    HealSingle,
    DefenseArea,
    DefenseSingle,
    EsunaStanceNorth,
    RaiseShirk,
    MoveForward,
    MoveBack,
    AntiKnockback,
    Burst,

    None,
}

public enum StateCommandType : byte
{
    Cancel,
    Auto,
    Manual,

    None,
}

public enum OtherCommandType : byte
{
    Settings,
    Rotations,
    DoActions,
    ToggleActions,
}

public enum SettingsCommand : byte
{
    AutoBurst,
    UseAbility,
    UseDefenseAbility,
    AutoTankStance,
    AutoProvokeForTank,
    AutoUseTrueNorth,
    RaisePlayerBySwift,
    UseGroundBeneficialAbility,
    UseAOEAction,
    UseAOEWhenManual,
    PreventActions
}

public static class SettingsCommandExtension
{
    public static bool GetDefault(this SettingsCommand command) => command switch
    {
        SettingsCommand.AutoBurst => true,
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
        _ => false,
    };
}
