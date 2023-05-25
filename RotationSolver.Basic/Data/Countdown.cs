using FFXIVClientStructs.FFXIV.Client.System.Framework;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using System.Runtime.InteropServices;

namespace RotationSolver.Basic.Data;

[StructLayout(LayoutKind.Explicit)]
public unsafe struct Countdown
{
    [FieldOffset(0x28)] public float Timer;
    [FieldOffset(0x38)] public byte Active;
    [FieldOffset(0x3C)] public uint Initiator;

    public static unsafe Countdown* Instance => (Countdown*)Framework.Instance()->GetUiModule()->GetAgentModule()->GetAgentByInternalId(AgentId.CountDownSettingDialog);

    public static float TimeRemaining
    {
        get
        {
            var inst = Instance;
            return inst->Active != 0 ? inst->Timer : 0;
        }
    }
}
