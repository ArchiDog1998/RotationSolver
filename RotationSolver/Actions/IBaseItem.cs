namespace RotationSolver.Actions
{
    internal interface IBaseItem : IAction
    {
        bool CanUse(out IAction item);
    }
}
