namespace RotationSolver.Basic.Actions;

public interface IAction : ITexture, IEnouthLevel
{
    uint ID { get; }
    uint AdjustedID { get; }
    float RecastTimeOneCharge { get; }
    float AnimationLockTime { get; }
    uint SortKey { get; }
    public bool IsActionSequencer { get; }

    /// <summary>
    /// Please don't use it.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    float RecastTimeElapsed { get; }

    /// <summary>
    /// Is action cooling down.
    /// </summary>
    bool IsCoolingDown { get; }

    bool IsInCooldown { get; set; }

    bool Use();
}
