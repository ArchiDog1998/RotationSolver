namespace RotationSolver.Basic.Traits;

/// <summary>
/// Trait.
/// </summary>
public interface IBaseTrait : IEnoughLevel, ITexture
{
    /// <summary>
    /// Traid ID
    /// </summary>
    uint ID { get; }
}
