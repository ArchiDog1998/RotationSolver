namespace RotationSolver.Basic.Data;

/// <summary>
/// The status of auto.
/// </summary>
[Flags]
public enum AutoStatus : ushort
{
    /// <summary>
    /// Nothing.
    /// </summary>
    None = 0,

    /// <summary>
    /// Shall we interrupt.
    /// </summary>
    Interrupt = 1 << 0,

    /// <summary>
    /// Shall we use tank stance.
    /// </summary>
    TankStance = 1 << 1,

    /// <summary>
    /// Shall we provoke some enemy.
    /// </summary>
    Provoke = 1 << 2,

    /// <summary>
    /// Shall we defense single.
    /// </summary>
    DefenseSingle = 1 << 3,

    /// <summary>
    /// Shall we defense are.
    /// </summary>
    DefenseArea = 1 << 4,

    /// <summary>
    /// Shall we heal single by ability.
    /// </summary>
    HealSingleAbility = 1 << 5,

    /// <summary>
    /// Shall we heal single by spell.
    /// </summary>
    HealSingleSpell = 1 << 6,

    /// <summary>
    /// Shall we heal area by ability.
    /// </summary>
    HealAreaAbility = 1 << 7,

    /// <summary>
    /// Shall we heal area by spell.
    /// </summary>
    HealAreaSpell = 1 << 8,

    /// <summary>
    /// Shall we raise.
    /// </summary>
    Raise = 1 << 9,

    /// <summary>
    /// Shall we esuna.
    /// </summary>
    Esuna = 1 << 10,
}
