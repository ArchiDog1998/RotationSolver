namespace RotationSolver.Commands;

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
}

public enum StateCommandType : byte
{
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
