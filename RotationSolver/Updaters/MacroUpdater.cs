namespace RotationSolver.Updaters;

internal static class MacroUpdater
{
    internal static MacroItem? DoingMacro;

    public static void UpdateMacro()
    {
        if (DoingMacro == null && DataCenter.Macros.TryDequeue(out var macro))
        {
            DoingMacro = macro;
        }

        if (DoingMacro != null)
        {
            if (DoingMacro.IsRunning)
            {
                if (DoingMacro.EndUseMacro())
                {
                    DoingMacro = null;
                }
                else
                {
                    return;
                }
            }
            else
            {
                DoingMacro.StartUseMacro();
            }
        }
    }
}
