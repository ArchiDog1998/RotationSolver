namespace RotationSolver.Basic.Actions;

/// <summary>
/// The action.
/// </summary>
public interface IAction : ITexture, IEnoughLevel
{
    /// <summary>
    /// ID of this action.
    /// </summary>
    uint ID { get; }

    /// <summary>
    /// The adjusted Id of this action.
    /// </summary>
    uint AdjustedID { get; }

    /// <summary>
    /// The animation lock time of this action.
    /// </summary>
    float AnimationLockTime { get; }

    /// <summary>
    /// The key of sorting this action.
    /// </summary>
    uint SortKey { get; }

    /// <summary>
    /// Is in cd window.
    /// </summary>
    bool IsInCooldown { get; set; }

    /// <summary>
    /// The cd information.
    /// </summary>
    ICooldown Cooldown { get; }

    /// <summary>
    /// How to use.
    /// </summary>
    /// <returns></returns>
    bool Use();
}
