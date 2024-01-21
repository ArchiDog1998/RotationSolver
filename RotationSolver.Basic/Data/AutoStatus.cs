namespace RotationSolver.Basic.Data;

/// <summary>
/// The status of auto.
/// </summary>
[Flags]
public enum AutoStatus : uint
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
    /// Shall we Dispel.
    /// </summary>
    Dispel = 1 << 10,

    /// <summary>
    /// 
    /// </summary>
    Positional = 1 << 11,

    /// <summary>
    /// 
    /// </summary>
    Shirk = 1 << 12,

    /// <summary>
    /// 
    /// </summary>
    MoveForward = 1 << 13,

    /// <summary>
    /// 
    /// </summary>
    MoveBack = 1 << 14,

    /// <summary>
    /// 
    /// </summary>
    AntiKnockback = 1 << 15,

    /// <summary>
    /// 
    /// </summary>
    Burst = 1 << 16,

    /// <summary>
    /// 
    /// </summary>
    Speed = 1 << 17,

    /// <summary>
    /// 
    /// </summary>
    LimitBreak = 1 << 18,
}
