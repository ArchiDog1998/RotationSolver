using Dalamud.Game;
using Dalamud.Logging;
using System;

namespace XIVAutoAttack;

internal class PluginAddressResolver : BaseAddressResolver
{
    private IntPtr ComboTimer { get; set; }
    public unsafe float ComboTime => *(float*)ComboTimer;
    private IntPtr LastComboMove => ComboTimer + 4;
    public unsafe uint LastComboAction => *(uint*)LastComboMove;

    public IntPtr IsActionIdReplaceable { get; private set; }
    public IntPtr PlaySpecificSound { get; private set; }
    public IntPtr GetResourceSync { get; private set; }
    public IntPtr GetResourceAsync { get; private set; }
    public IntPtr LoadSoundFile { get; private set; }
    public IntPtr CanAttackFunction { get; private set; }

    protected override void Setup64Bit(SigScanner scanner)
    {
        this.ComboTimer = scanner.GetStaticAddressFromSig("F3 0F 11 05 ?? ?? ?? ?? F3 0F 10 45 ?? E8");
        this.IsActionIdReplaceable = scanner.ScanText("81 F9 ?? ?? ?? ?? 7F 35");
        this.PlaySpecificSound = scanner.ScanText("48 89 5C 24 ?? 48 89 74 24 ?? 57 48 83 EC 20 33 F6 8B DA 48 8B F9 0F BA E2 0F");
        this.GetResourceSync = scanner.ScanText("E8 ?? ?? ?? ?? 48 8D 8F ?? ?? ?? ?? 48 89 87 ?? ?? ?? ?? 48 8D 54 24");
        this.GetResourceAsync = scanner.ScanText("E8 ?? ?? ?? ?? 48 8B D8 EB 07 F0 FF 83");
        this.LoadSoundFile = scanner.ScanText("E8 ?? ?? ?? ?? 48 85 C0 75 04 B0 F6");
        this.CanAttackFunction = scanner.ScanText("48 89 5C 24 ?? 57 48 83 EC 20 48 8B DA 8B F9 E8 ?? ?? ?? ?? 4C 8B C3 ");


        PluginLog.Verbose("===== X I V A U T O A T T C K =====", Array.Empty<object>());
        PluginLog.Verbose($"IsActionIdReplaceable 0x{IsActionIdReplaceable:X}", Array.Empty<object>());
        PluginLog.Verbose($"ComboTimer            0x{ComboTimer:X}", Array.Empty<object>());
        PluginLog.Verbose($"LastComboMove         0x{LastComboMove:X}", Array.Empty<object>());
    }
}
