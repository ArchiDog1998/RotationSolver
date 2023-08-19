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
    /// Please don't use it.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    internal float RecastTimeElapsedRaw { get; }

    /// <summary>
    /// Please don't use it.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    internal float RecastTimeOneChargeRaw { get; }

    /// <summary>
    /// Is action cooling down.
    /// </summary>
    bool IsCoolingDown { get; }

    /// <summary>
    /// Is in the cd window.
    /// </summary>
    bool IsInCooldown { get; set; }

    /// <summary>
    /// How to use.
    /// </summary>
    /// <returns></returns>
    bool Use();
}
