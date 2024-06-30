using FFXIVClientStructs.FFXIV.Client.System.Framework;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using System.Runtime.InteropServices;

namespace RotationSolver.Basic.Data;

/// <summary>
/// The struct about countdown.
/// </summary>
[StructLayout(LayoutKind.Explicit)]
public unsafe struct Countdown
{
    /// <summary>
    /// Timer.
    /// </summary>
    [FieldOffset(0x28)] public float Timer;

    /// <summary>
    /// Is this action active.
    /// </summary>
    [FieldOffset(0x38)] public byte Active;

    /// <summary>
    /// Init.
    /// </summary>
    [FieldOffset(0x3C)] public uint Initiator;

    /// <summary>
    /// The instance about this struct.
    /// </summary>
    public static unsafe Countdown* Instance => (Countdown*)Framework.Instance()->GetUIModule()->GetAgentModule()->GetAgentByInternalId(AgentId.CountDownSettingDialog);

    static RandomDelay _delay = new(() => Service.Config.CountdownDelay);

    /// <summary>
    /// TimeRemaining.
    /// </summary>
    public static float TimeRemaining
    {
        get
        {
            var inst = Instance;
            return _delay.Delay(inst->Active != 0) ? inst->Timer : 0;
        }
    }
}
