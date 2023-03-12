namespace RotationSolver.Data;

public enum DescType : byte
{
    None,
    BurstActions,

    HealAreaGCD,
    HealAreaAbility,

    HealSingleGCD,
    HealSingleAbility,

    DefenseAreaGCD,
    DefenseAreaAbility,

    DefenseSingleGCD,
    DefenseSingleAbility,

    MoveForwardGCD,
    MoveForwardAbility,
    MoveBackAbility,
}
