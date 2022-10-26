using Dalamud.Game;
using Dalamud.Logging;
using System;
using System.Runtime.InteropServices;

namespace XIVAutoAttack.SigReplacers;

internal class PluginAddressResolver : BaseAddressResolver
{
    private IntPtr ComboTimer { get; set; }
    public IntPtr ActorMove { get; private set; }
    public unsafe float ComboTime => *(float*)ComboTimer;
    private IntPtr LastComboMove => ComboTimer + 4;
    public unsafe uint LastComboAction => *(uint*)LastComboMove;

    public IntPtr IsActionIdReplaceable { get; private set; }
    public IntPtr CanAttackFunction { get; private set; }
    private IntPtr _playerMoveControllerAddress;
    public IntPtr IsMoving
    {
        get
        {
            IntPtr a = Marshal.ReadIntPtr(_playerMoveControllerAddress + 0x20);

            if (a == IntPtr.Zero)
            {
                return IntPtr.Zero;
            }

            return a + 0x1FD;
        }
    }


    //private IntPtr _cameraAddress;

    //public IntPtr IsMovingSet => _cameraAddress + 0x108;

    protected override void Setup64Bit(SigScanner scanner)
    {
        ComboTimer = scanner.GetStaticAddressFromSig("F3 0F 11 05 ?? ?? ?? ?? F3 0F 10 45 ?? E8");

        // from https://github.com/MiKE41/mMovement/blob/dda61e7cdc210c3b3f5cd9687496747f0b987074/Memory.cs#L14
        _playerMoveControllerAddress = scanner.GetStaticAddressFromSig("48 8D 0D ?? ?? ?? ?? E8 ?? ?? ?? ?? 48 8D 0D ?? ?? ?? ?? E8 ?? ?? ?? ?? 48 8D 0D ?? ?? ?? ?? E8 ?? ?? ?? ?? 48 8D 8B ?? ?? ?? ?? E8 ?? ?? ?? ?? 48 8D 0D ?? ?? ?? ?? E8 ?? ?? ?? ?? 48 83 3D");

        IsActionIdReplaceable = scanner.ScanText("81 F9 ?? ?? ?? ?? 7F 35");
        CanAttackFunction = scanner.ScanText("48 89 5C 24 ?? 57 48 83 EC 20 48 8B DA 8B F9 E8 ?? ?? ?? ?? 4C 8B C3 ");

        //找G大要这个地址，我反正不知道
        ActorMove = scanner.ScanText("40 53 48 83 EC ?? F3 0F 11 89 ?? ?? ?? ?? 48 8B D9 F3 0F 11 91 ?? ?? ?? ??");

        PluginLog.Verbose("===== X I V A U T O A T T C K =====", Array.Empty<object>());
        PluginLog.Verbose($"IsActionIdReplaceable 0x{IsActionIdReplaceable:X}", Array.Empty<object>());
        PluginLog.Verbose($"ComboTimer            0x{ComboTimer:X}", Array.Empty<object>());
        PluginLog.Verbose($"LastComboMove         0x{LastComboMove:X}", Array.Empty<object>());
    }
}
