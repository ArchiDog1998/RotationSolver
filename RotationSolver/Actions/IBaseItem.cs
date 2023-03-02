namespace RotationSolver.Actions
{
    internal interface IBaseItem : IAction, IEnable
    {
        bool CanUse(out IAction item);
    }
}
