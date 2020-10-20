using Dalamud.Game;
using Dalamud.Game.Internal;
using System;

namespace XIVComboPlugin
{
    class IconReplacerAddressResolver : BaseAddressResolver
    {
        public IntPtr ComboTimer { get; private set; }
        public IntPtr LastComboMove { get { return ComboTimer + 0x4; } }
        public IntPtr GetIcon { get; private set; }
        public IntPtr IsIconReplaceable { get; private set; }
        public IntPtr BuffVTableAddr { get; private set; }

        protected override void Setup64Bit(SigScanner scanner)
        {
            this.ComboTimer = scanner.GetStaticAddressFromSig("E8 ?? ?? ?? ?? 80 7E 21 00", 0x178);

            this.GetIcon = scanner.ScanText("48 89 5c 24 08 48 89 6c 24 10 48 89 74 24 18 57 48 83 ec 30 8b da be dd 1c 00 00 bd d3 0d 00 00");

            this.IsIconReplaceable = scanner.ScanText("81 f9 2e 01 00 00 7f 39 81 f9 2d 01 00 00 0f 8d 11 02 00 00 83 c1 eb");

            this.BuffVTableAddr = scanner.GetStaticAddressFromSig("48 89 05 ?? ?? ?? ?? 88 05 ?? ?? ?? ?? 88 05 ?? ?? ?? ??");
        }
    }
}
