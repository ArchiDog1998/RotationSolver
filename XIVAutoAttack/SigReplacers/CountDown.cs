using Dalamud.Hooking;
using System;
using System.Runtime.InteropServices;

namespace XIVAutoAttack.SigReplacers
{
    internal static class CountDown
    {
        private delegate IntPtr CountdownTimerDelegate(IntPtr p1);

        private static Hook<CountdownTimerDelegate> _countdownTimerHook = null;

        private static IntPtr _countDown = IntPtr.Zero;

        internal static float CountDownTime
        {
            get
            {
                if (_countDown == IntPtr.Zero) return 0;
                return Math.Max(0, Marshal.PtrToStructure<float>(_countDown + 0x2c));
            }
        }

        internal static unsafe void Enable()
        {
            _countdownTimerHook = Hook<CountdownTimerDelegate>.FromAddress(Service.Address.CountdownTimerAdress, CountdownTimerFunc);

            _countdownTimerHook?.Enable();
        }

        private static IntPtr CountdownTimerFunc(IntPtr value)
        {
            _countDown = value;
            return _countdownTimerHook!.Original(value);
        }

        internal static void Dispose()
        {
            _countdownTimerHook?.Dispose();
        }
    }
}
