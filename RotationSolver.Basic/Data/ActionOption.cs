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
    /// flag to check this action is heal.
    /// </summary>
    HealFlag = 1 << 6,

    /// <summary>
    /// Is the action a resource taken action.
    /// </summary>
    UseResources = 1 << 7,

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
    Heal = Friendly | HealFlag,

    /// <summary>
    /// Defense action (you need to change the targeting strategy.)
    /// </summary>
    Defense = Friendly,

    /// <summary>
    /// Hot action
    /// </summary>
    Hot = Heal | Eot,

    /// <summary>
    /// Some buff
    /// </summary>
    Buff = Friendly,
}
