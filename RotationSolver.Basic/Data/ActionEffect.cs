using System.Runtime.InteropServices;

namespace RotationSolver.Basic.Data;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct ActionEffect
{
    public ActionEffectType Type;
    public byte Param0;
    public byte Param1;
    public byte Param2;
    public byte Param3;
    public byte Param4;
    public ushort Value;

    public override string ToString()
        => $"Type: {Type}; {Param0},{Param1},{Param2},{Param3},{Param4}; Value: {Value}";
}
 
