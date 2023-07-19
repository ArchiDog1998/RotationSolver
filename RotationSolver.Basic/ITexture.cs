namespace RotationSolver.Basic;

/// <summary>
/// The things that has texture.
/// </summary>
public interface ITexture
{
    /// <summary>
    /// The icon ID.
    /// </summary>
    uint IconID { get; }

    /// <summary>
    /// Name about this.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Description.
    /// </summary>
    string Description { get; }

    /// <summary>
    /// Is this one enabled.
    /// </summary>
    bool IsEnabled { get; set; }
}
