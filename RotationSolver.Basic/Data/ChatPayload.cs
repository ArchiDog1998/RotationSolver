using System;
using System.Runtime.InteropServices;
using System.Text;

namespace RotationSolver.Basic.Data;

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
        textPointer = Marshal.AllocHGlobal(stringBytes.Length + 30);
        Marshal.Copy(stringBytes, 0, textPointer, stringBytes.Length);
        Marshal.WriteByte(textPointer + stringBytes.Length, 0);

        textLength = (ulong)(stringBytes.Length + 1);

        unk1 = 64;
        unk2 = 0;
    }

    public void Dispose()
    {
        Marshal.FreeHGlobal(textPointer);
    }
}
