namespace RotationSolver.Basic.Data;

public enum SpecialCommandType : byte
{
    None,
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
}

public enum StateCommandType : byte
{
    None,
    Cancel,
    Smart,
    Manual,
}

public enum OtherCommandType : byte
{
    Settings,
    Rotations,
    DoActions,
    ToggleActions,
}
