namespace RotationSolver.Basic;

/// <summary>
/// The interface about Enough Level.
/// </summary>
public interface IEnoughLevel
{
    /// <summary>
    /// Player's level is enough for this action's usage.
    /// </summary>
    bool EnoughLevel { get; }

    internal byte Level { get; }
}
