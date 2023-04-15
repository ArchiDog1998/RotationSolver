namespace RotationSolver.Basic.Data;

[Flags]
public enum ActionOption : byte
{
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
    /// should I put it to the timeline (heal and defense only)
    /// </summary>
    Timeline = 1 << 3,

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
    Heal = Friendly | Timeline,

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
