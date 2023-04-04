namespace RotationSolver.Basic.Actions;

public interface IBaseItem : IAction
{
    bool CanUse(out IAction item);
}
