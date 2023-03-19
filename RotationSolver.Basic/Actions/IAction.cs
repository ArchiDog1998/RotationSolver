namespace RotationSolver.Basic.Actions;

public interface IAction : ITexture
{
    bool Use();
    uint ID { get; }
    uint AdjustedID { get; }
    float RecastTimeOneCharge { get; }
    float RecastTimeElapsed { get; }
    bool IsCoolingDown { get; }

    /// <summary>
    /// Player's level is enough for this action's usage.
    /// </summary>
    bool EnoughLevel { get; }
}
