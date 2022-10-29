using Dalamud.Game;
using Dalamud.Logging;
using System;
using System.Runtime.InteropServices;

namespace XIVAutoAttack.SigReplacers;

internal class PluginAddressResolver : BaseAddressResolver
{
    private IntPtr ComboTimer { get; set; }
    public unsafe float ComboTime => *(float*)ComboTimer;
    private IntPtr LastComboMove => ComboTimer + 4;
    public unsafe uint LastComboAction => *(uint*)LastComboMove;


    public IntPtr IsActionIdReplaceable { get; private set; }
    public IntPtr CanAttackFunction { get; private set; }
    public IntPtr MovingController { get; private set; }
    public IntPtr MarkingController { get; private set; }


    protected override void Setup64Bit(SigScanner scanner)
    {
        //Static
        ComboTimer = scanner.GetStaticAddressFromSig("F3 0F 11 05 ?? ?? ?? ?? F3 0F 10 45 ?? E8");
        //Function
        IsActionIdReplaceable = scanner.ScanText("81 F9 ?? ?? ?? ?? 7F 35");
        CanAttackFunction = scanner.ScanText("48 89 5C 24 ?? 57 48 83 EC 20 48 8B DA 8B F9 E8 ?? ?? ?? ?? 4C 8B C3 ");
        MovingController = scanner.ScanText("40 55 53 48 8D 6C 24 ?? 48 81 EC ?? ?? ?? ?? 48 83 79 ?? ??");
        MarkingController = scanner.ScanText("48 8B 94 24 ? ? ? ? 48 8D 0D ? ? ? ? 41 B0 01");

        PluginLog.Verbose("===== X I V A U T O A T T C K =====", Array.Empty<object>());
        PluginLog.Verbose($"ComboTimer            0x{ComboTimer:X}", Array.Empty<object>());
        PluginLog.Verbose($"LastComboMove         0x{LastComboMove:X}", Array.Empty<object>());
        PluginLog.Verbose($"IsActionIdReplaceable 0x{IsActionIdReplaceable:X}", Array.Empty<object>());
        PluginLog.Verbose($"MovingController      0x{MovingController:X}", Array.Empty<object>());
        PluginLog.Verbose($"MarkingController     0x{MarkingController:X}", Array.Empty<object>());
    }
}
