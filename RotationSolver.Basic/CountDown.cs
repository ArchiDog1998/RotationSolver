using Dalamud.Hooking;
using Dalamud.Utility.Signatures;
using RotationSolver.Basic;
using System;
using System.Runtime.InteropServices;

namespace RotationSolver.SigReplacers;

public class CountDown: IDisposable
{
    private delegate IntPtr CountdownTimerDelegate(IntPtr p1);

    /// <summary>
    ///https://github.com/xorus/EngageTimer/blob/main/Game/CountdownHook.cs
    /// </summary>
    [Signature("48 89 5C 24 ?? 57 48 83 EC 40 8B 41", DetourName = nameof(CountdownTimerFunc))]
    private readonly Hook<CountdownTimerDelegate> _countdownTimerHook = null;

    private static IntPtr _countDown = IntPtr.Zero;

    public static float CountDownTime
    {
        get
        {
            if (_countDown == IntPtr.Zero) return 0;
            return Math.Max(0, Marshal.PtrToStructure<float>(_countDown + 0x2c));
        }
    }

    public CountDown()
    {
        SignatureHelper.Initialise(this);
        _countdownTimerHook?.Enable();
    }

    private IntPtr CountdownTimerFunc(IntPtr value)
    {
        _countDown = value;
        return _countdownTimerHook!.Original(value);
    }

    public void Dispose()
    {
        _countdownTimerHook?.Dispose();
    }
}
