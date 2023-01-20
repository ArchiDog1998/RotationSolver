namespace RotationSolver.Actions;

public interface IAction : IEnableTexture
{
    bool Use();
    uint ID { get; }
    uint AdjustedID { get; }
    string CateName { get; }

    void Display(bool IsActive);
}
