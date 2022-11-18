using System;
using System.Runtime.InteropServices;
using System.Text;

namespace XIVAutoAttack.Data
{
    [StructLayout(LayoutKind.Explicit)]
    public readonly struct ChatPayload : IDisposable
    {
        [FieldOffset(0)]
        private readonly IntPtr textPointer;

        [FieldOffset(16)]
        private readonly ulong textLength;

        [FieldOffset(8)]
        private readonly ulong unk1;

        [FieldOffset(24)]
        private readonly ulong unk2;

        internal ChatPayload(string text)
        {
            byte[] stringBytes = Encoding.UTF8.GetBytes(text);
            this.textPointer = Marshal.AllocHGlobal(stringBytes.Length + 30);
            Marshal.Copy(stringBytes, 0, this.textPointer, stringBytes.Length);
            Marshal.WriteByte(this.textPointer + stringBytes.Length, 0);

            this.textLength = (ulong)(stringBytes.Length + 1);

            this.unk1 = 64;
            this.unk2 = 0;
        }

        public void Dispose()
        {
            Marshal.FreeHGlobal(this.textPointer);
        }
    }
}
