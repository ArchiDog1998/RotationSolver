namespace RotationSolver.Basic;

public interface IEnouthLevel
{
    /// <summary>
    /// Player's level is enough for this action's usage.
    /// </summary>
    bool EnoughLevel { get; }

    internal byte Level { get; }
}
