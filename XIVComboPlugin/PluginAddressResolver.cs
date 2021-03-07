using Dalamud.Game;
using Dalamud.Game.Internal;
using Dalamud.Plugin;
using System;

namespace XIVComboExpandedestPlugin
{
    internal class PluginAddressResolver : BaseAddressResolver
    {
        public IntPtr ComboTimer { get; private set; }
        public IntPtr LastComboMove { get { return ComboTimer + 0x4; } }
        public IntPtr GetIcon { get; private set; }
        public IntPtr IsIconReplaceable { get; private set; }
        public IntPtr GetActionCooldown { get; private set; }
        public IntPtr ActionManager { get; private set; }

        protected override void Setup64Bit(SigScanner scanner)
        {
            ComboTimer = scanner.GetStaticAddressFromSig("E8 ?? ?? ?? ?? 80 7E 21 00", 0x178);

            // this.GetIcon = scanner.ScanText("48 89 5c 24 08 48 89 6c 24 10 48 89 74 24 18 57 48 83 ec 30 8b da be dd 1c 00 00 bd d3 0d 00 00");  // 5.35
            GetIcon = scanner.ScanText("E8 ?? ?? ?? ?? 8B F8 3B DF"); // 5.4

            // this.IsIconReplaceable = scanner.ScanText("81 f9 2e 01 00 00 7f 39 81 f9 2d 01 00 00 0f 8d 11 02 00 00 83 c1 eb");  // 5.35
            IsIconReplaceable = scanner.ScanText("81 F9 ?? ?? ?? ?? 7F 39 81 F9 ?? ?? ?? ??"); // 5.4

            ActionManager = scanner.GetStaticAddressFromSig("48 89 05 ?? ?? ?? ?? C3 CC C2 00 00 CC CC CC CC CC CC CC CC CC CC CC CC CC 48 8D 0D ?? ?? ?? ?? E9 ?? ?? ?? ??");
            GetActionCooldown = scanner.ScanText("E8 ?? ?? ?? ?? 0F 57 FF 48 85 C0");

            PluginLog.Verbose("===== H O T B A R S =====");
            PluginLog.Verbose($"GetIcon address   0x{GetIcon.ToInt64():X}");
            PluginLog.Verbose($"IsIconReplaceable 0x{IsIconReplaceable.ToInt64():X}");
            PluginLog.Verbose($"ComboTimer        0x{ComboTimer.ToInt64():X}");
            PluginLog.Verbose($"LastComboMove     0x{LastComboMove.ToInt64():X}");
        }
    }
}
