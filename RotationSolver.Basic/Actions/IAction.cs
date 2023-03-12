namespace RotationSolver.Actions;

public interface IAction : ITexture
{
    bool Use();
    uint ID { get; }
    uint AdjustedID { get; }
}
