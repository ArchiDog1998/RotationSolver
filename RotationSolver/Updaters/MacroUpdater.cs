using RotationSolver.Basic;
using RotationSolver.Basic.Data;

namespace RotationSolver.Updaters;

internal static class MacroUpdater
{
    internal static MacroItem DoingMacro;

    public static void UpdateMacro()
    {
        //如果没有有正在运行的宏，弄一个出来
        if (DoingMacro == null && DataCenter.Macros.TryDequeue(out var macro))
        {
            DoingMacro = macro;
        }

        //如果有正在处理的宏
        if (DoingMacro != null)
        {
            //正在跑的话，就尝试停止，停止成功就放弃它。
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
            //否则，始终开始。
            else
            {
                DoingMacro.StartUseMacro();
            }
        }
    }
}
