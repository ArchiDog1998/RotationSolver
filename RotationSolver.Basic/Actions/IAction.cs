namespace RotationSolver.Basic.Actions;

public interface IAction : ITexture
{
    bool Use();
    uint ID { get; }
    uint AdjustedID { get; }
}
