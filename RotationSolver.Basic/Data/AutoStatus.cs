namespace RotationSolver.Basic.Data;

[Flags]
public enum AutoStatus : ushort
{
    None = 0,
    Interrupt = 1 << 0,
    TankStance = 1 << 1,
    Provoke = 1 << 2,
    DefenseSingle = 1 << 3,
    DefenseArea = 1 << 4,
    HealSingleAbility = 1 << 5,
    HealSingleSpell = 1 << 6,
    HealAreaAbility = 1 << 7,
    HealAreaSpell = 1 << 8,
    Raise = 1 << 9,
    Esuna = 1 << 10,
}
