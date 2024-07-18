using ECommons.EzIpcManager;
using RotationSolver.Commands;

namespace RotationSolver.Basic;

internal class Ipc
{
    public Ipc()
    {
        EzIPC.Init(this, prefix: nameof(RotationSolver));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="command">
    /// <para>0 for <see cref="StateCommandType.Cancel"/></para>
    /// <para>1 for <see cref="StateCommandType.Auto"/></para>
    /// <para>2 for <see cref="StateCommandType.Manual"/></para>
    /// </param>
    /// <param name="index">the index of the command</param>
    [EzIPC]
    public void SetState(byte command, int index)
    {
        RSCommands.DoStateCommandType((StateCommandType)command, index);
    }
}
