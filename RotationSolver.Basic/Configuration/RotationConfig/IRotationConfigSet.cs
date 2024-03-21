namespace RotationSolver.Basic.Configuration.RotationConfig;

/// <summary>
/// The config set about this config..
/// </summary>
public interface IRotationConfigSet : IEnumerable<IRotationConfig>
{
    /// <summary>
    /// The configs.
    /// </summary>
    HashSet<IRotationConfig> Configs { get; }
}
