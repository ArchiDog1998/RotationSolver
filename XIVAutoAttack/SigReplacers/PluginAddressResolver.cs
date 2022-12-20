using Dalamud.Game;
using Dalamud.Logging;
using System;
using System.Runtime.InteropServices;
using XIVAutoAttack.Data;

namespace XIVAutoAttack.SigReplacers;

internal class PluginAddressResolver : BaseAddressResolver
{
    private IntPtr ComboTimer { get; set; }
    public unsafe float ComboTime => *(float*)ComboTimer;
    private IntPtr LastComboMove => ComboTimer + 4;
    public unsafe ActionID LastComboAction => *(ActionID*)LastComboMove;


    public IntPtr IsActionIdReplaceable { get; private set; }
    public IntPtr CanAttackFunction { get; private set; }
    public IntPtr MovingController { get; private set; }
    public IntPtr MarkingController { get; private set; }
    public IntPtr ReceiveAbilty { get; private set; }
    public IntPtr CountdownTimerAdress { get; private set; }

    public delegate void GetChatBoxModuleDelegate(IntPtr uiModule, IntPtr message, IntPtr unused, byte a4);
    public GetChatBoxModuleDelegate GetChatBox { get; private set; }

    protected override void Setup64Bit(SigScanner scanner)
    {
        //Static
        ComboTimer = scanner.GetStaticAddressFromSig("F3 0F 11 05 ?? ?? ?? ?? F3 0F 10 45 ?? E8");
        MarkingController = scanner.GetStaticAddressFromSig("48 8d 0d ?? ?? ?? ?? e8 ?? ?? ?? ?? 48 3b c3 75 ?? ff c7 3b fe");

        //Function
        IsActionIdReplaceable = scanner.ScanText("E8 ?? ?? ?? ?? 84 C0 74 4C 8B D3");
        CanAttackFunction = scanner.ScanText("48 89 5C 24 ?? 57 48 83 EC 20 48 8B DA 8B F9 E8 ?? ?? ?? ?? 4C 8B C3 ");
        MovingController = scanner.ScanText("40 55 53 48 8D 6C 24 ?? 48 81 EC ?? ?? ?? ?? 48 83 79 ?? ??");
        ReceiveAbilty = scanner.ScanText("4C 89 44 24 ?? 55 56 41 54 41 55 41 56");
        CountdownTimerAdress = scanner.ScanText("48 89 5C 24 ?? 57 48 83 EC 40 8B 41");

        GetChatBox = Marshal.GetDelegateForFunctionPointer<GetChatBoxModuleDelegate>(
            scanner.ScanText("48 89 5C 24 ?? 57 48 83 EC 20 48 8B FA 48 8B D9 45 84 C9"));

        PluginLog.Verbose("===== X I V A U T O A T T C K =====", Array.Empty<object>());
        PluginLog.Verbose($"ComboTimer            0x{ComboTimer:X}", Array.Empty<object>());
        PluginLog.Verbose($"LastComboMove         0x{LastComboMove:X}", Array.Empty<object>());
        PluginLog.Verbose($"IsActionIdReplaceable 0x{IsActionIdReplaceable:X}", Array.Empty<object>());
        PluginLog.Verbose($"MovingController      0x{MovingController:X}", Array.Empty<object>());
        PluginLog.Verbose($"MarkingController     0x{MarkingController:X}", Array.Empty<object>());
    }
}
