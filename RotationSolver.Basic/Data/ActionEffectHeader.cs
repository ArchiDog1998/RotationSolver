using FFXIVClientStructs.FFXIV.Client.Game;
using System.Runtime.InteropServices;

namespace RotationSolver.Basic.Data;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct ActionEffectHeader
{
    /// <summary>
    /// who the animation targets
    /// </summary>
    public ulong animationTargetId;

    /// <summary>
    /// what the casting player casts, shown in battle log / ui
    /// </summary>
    public uint actionId;

    /// <summary>
    /// Number of the effect
    /// </summary>
    public uint globalEffectCounter;
    public float animationLockTime;
    public uint SomeTargetID;

    /// <summary>
    /// 0 = initiated by server, otherwise corresponds to client request sequence id
    /// </summary>
    public ushort SourceSequence; 
    public ushort rotation;
    public ushort actionAnimationId;

    /// <summary>
    /// animation
    /// </summary>
    public byte variation; 

    /// <summary>
    /// Action Type
    /// </summary>
    public ActionType actionType;
    public byte unknown20;
    public byte NumTargets; // machina calls it 'effectCount', but it is misleading imo
    public ushort padding21;
    public ushort padding22;
    public ushort padding23;
    public ushort padding24;
}