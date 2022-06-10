using System;
using Dalamud.Game;
using Dalamud.Logging;

namespace XIVAutoAttack;

internal class PluginAddressResolver : BaseAddressResolver
{
    private IntPtr ComboTimer { get; set; }
    public unsafe float ComboTime => *(float*)ComboTimer;
    private IntPtr LastComboMove => ComboTimer + 4;
    public unsafe uint LastComboAction => *(uint*)LastComboMove;


    //public IntPtr GetAdjustedActionId { get; private set; }

    public IntPtr IsActionIdReplaceable { get; private set; }

    //public IntPtr GetActionCooldown { get; private set; }

    protected override void Setup64Bit(SigScanner scanner)
    {
        ComboTimer = scanner.GetStaticAddressFromSig("48 89 2D ?? ?? ?? ?? 85 C0 74 0F", 0);
        //GetAdjustedActionId = scanner.ScanText("E8 ?? ?? ?? ?? 8B F8 3B DF");
        IsActionIdReplaceable = scanner.ScanText("81 F9 ?? ?? ?? ?? 7F 35");
        //GetActionCooldown = scanner.ScanText("E8 ?? ?? ?? ?? 0F 57 FF 48 85 C0");
        PluginLog.Verbose("===== X I V C O M B O =====", Array.Empty<object>());
        //PluginLog.Verbose($"GetAdjustedActionId   0x{GetAdjustedActionId:X}", Array.Empty<object>());
        PluginLog.Verbose($"IsActionIdReplaceable 0x{IsActionIdReplaceable:X}", Array.Empty<object>());
        PluginLog.Verbose($"ComboTimer            0x{ComboTimer:X}", Array.Empty<object>());
        PluginLog.Verbose($"LastComboMove         0x{LastComboMove:X}", Array.Empty<object>());
        //PluginLog.Verbose($"GetActionCooldown     0x{GetActionCooldown:X}", Array.Empty<object>());
    }
}
