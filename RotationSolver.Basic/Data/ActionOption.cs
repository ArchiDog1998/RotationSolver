namespace RotationSolver.Basic.Data;

/// <summary>
/// The type of action.
/// </summary>
[Flags]
public enum ActionOption : byte
{
    /// <summary>
    /// The normal one.
    /// </summary>
    None = 0,

    /// <summary>
    /// is a friendly or supporting action
    /// </summary>
    Friendly = 1 << 0,

    /// <summary>
    /// is hot or dot action
    /// </summary>
    Eot = 1 << 1,

    /// <summary>
    /// end special after using it
    /// </summary>
    EndSpecial = 1 << 2,

    /// <summary>
    /// Is a GCD action.
    /// </summary>
    GeneralGCD = 1 << 3,

    /// <summary>
    /// Is a simple gcd action, without other cooldown.
    /// </summary>
    RealGCD = 1 << 4,

    /// <summary>
    /// The duty action
    /// </summary>
    DutyAction = 1 << 5,

    /// <summary>
    /// Dot action
    /// </summary>
    Dot = Eot,

    /// <summary>
    /// Attack action
    /// </summary>
    Attack = None,

    /// <summary>
    /// Heal action
    /// </summary>
    Heal = Friendly,

    /// <summary>
    /// Defense action (you need to change the targeting strategy.
    /// </summary>
    Defense = Heal,

    /// <summary>
    /// Hot action
    /// </summary>
    Hot = Heal | Eot,

    /// <summary>
    /// Some buff
    /// </summary>
    Buff = Friendly,
}
