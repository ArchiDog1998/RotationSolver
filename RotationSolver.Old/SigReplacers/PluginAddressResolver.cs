using Dalamud.Game;
using System;
using System.Runtime.InteropServices;

namespace RotationSolver.SigReplacers;

internal class PluginAddressResolver : BaseAddressResolver
{
    public IntPtr IsActionIdReplaceable { get; private set; }
    public IntPtr CanAttackFunction { get; private set; }
    public IntPtr MovingController { get; private set; }
    public IntPtr ReceiveAbilty { get; private set; }
    public IntPtr CountdownTimerAdress { get; private set; }

    public delegate void GetChatBoxModuleDelegate(IntPtr uiModule, IntPtr message, IntPtr unused, byte a4);
    public GetChatBoxModuleDelegate GetChatBox { get; private set; }

    protected override void Setup64Bit(SigScanner scanner)
    {
        //Function
        //https://github.com/attickdoor/XIVComboPlugin/blob/master/XIVComboPlugin/IconReplacerAddressResolver.cs
        IsActionIdReplaceable = scanner.ScanText("E8 ?? ?? ?? ?? 84 C0 74 4C 8B D3");

        //IDK
        //Some day in ClientStructs.
        CanAttackFunction = scanner.ScanText("48 89 5C 24 ?? 57 48 83 EC 20 48 8B DA 8B F9 E8 ?? ?? ?? ?? 4C 8B C3 ");
        MovingController = scanner.ScanText("40 55 53 48 8D 6C 24 ?? 48 81 EC ?? ?? ?? ?? 48 83 79 ?? ??");

        //https://github.com/Tischel/ActionTimeline/blob/master/ActionTimeline/Helpers/TimelineManager.cs#L86
        ReceiveAbilty = scanner.ScanText("4C 89 44 24 ?? 55 56 41 54 41 55 41 56");

        //https://github.com/xorus/EngageTimer/blob/main/Game/CountdownHook.cs
        CountdownTimerAdress = scanner.ScanText("48 89 5C 24 ?? 57 48 83 EC 40 8B 41");

        //https://github.com/BardMusicPlayer/Hypnotoad-Plugin/blob/7928be6735daf28e94121c3cf1c1dbbef0d97bcf/HypnotoadPlugin/Offsets/Offsets.cs#L18
        GetChatBox = Marshal.GetDelegateForFunctionPointer<GetChatBoxModuleDelegate>(
            scanner.ScanText("48 89 5C 24 ?? 57 48 83 EC 20 48 8B FA 48 8B D9 45 84 C9"));
    }
}
