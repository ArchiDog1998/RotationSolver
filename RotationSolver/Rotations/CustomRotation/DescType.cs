namespace RotationSolver.Rotations.CustomRotation;

public enum DescType : byte
{
    None,
    BurstActions,

    MoveForwardGCD,
    HealSingleGCD,
    HealAreaGCD,
    DefenseSingleGCD,
    DefenseAreaGCD,

    MoveForwardAbility,
    MoveBackAbility,
    HealSingleAbility,
    HealAreaAbility,
    DefenceSingleAbility,
    DefenceAreaAbility,
}
