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
    Smart,
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
