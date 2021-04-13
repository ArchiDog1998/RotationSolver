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

        protected override void Setup64Bit(SigScanner scanner)
        {
            ComboTimer = scanner.GetStaticAddressFromSig("48 89 2D ?? ?? ?? ?? 85 C0 74 0F");

            GetIcon = scanner.ScanText("E8 ?? ?? ?? ?? 8B F8 3B DF");  // Client::Game::ActionManager.GetAdjustedActionId

            IsIconReplaceable = scanner.ScanText("81 F9 ?? ?? ?? ?? 7F 39 81 F9 ?? ?? ?? ??");

            GetActionCooldown = scanner.ScanText("E8 ?? ?? ?? ?? 0F 57 FF 48 85 C0");

            PluginLog.Verbose("===== H O T B A R S =====");
            PluginLog.Verbose($"GetIcon address   0x{GetIcon.ToInt64():X}");
            PluginLog.Verbose($"IsIconReplaceable 0x{IsIconReplaceable.ToInt64():X}");
            PluginLog.Verbose($"ComboTimer        0x{ComboTimer.ToInt64():X}");
            PluginLog.Verbose($"LastComboMove     0x{LastComboMove.ToInt64():X}");
        }
    }
}
