namespace RotationSolver.Basic.Data;

/// <summary>
/// The type of the combat
/// </summary>
[Flags]
public enum CombatType : byte
{
    /// <summary>
    /// None of them! (Invalid)
    /// </summary>
    None = 0,

    /// <summary>
    /// Only for PvP.
    /// </summary>
    PvP = 1 << 0,

    /// <summary>
    /// Only for PvE.
    /// </summary>
    PvE = 1 << 1,

    /// <summary>
    /// PvP and PvE.
    /// </summary>
    Both = PvP | PvE,
}

internal static class ConbatTypeExtension
{
    public static uint GetIcon(this CombatType type) => type switch
    {
        CombatType.Both => 61540u,
        CombatType.PvE => 61542u,
        CombatType.PvP => 61544u,
        _ => 61523u,
    };
}
