using Dalamud.Game.ClientState.Conditions;
using Dalamud.Hooking;
using Dalamud.Utility.Signatures;
using RotationSolver.Basic;
using RotationSolver.Basic.Helpers;

namespace RotationSolver.Updaters;

internal class MovingController : IDisposable
{
    private static bool _posLocker = false;
    private delegate bool MovingControllerDelegate(IntPtr ptr);

    [Signature("40 55 53 48 8D 6C 24 ?? 48 81 EC ?? ?? ?? ?? 48 83 79 ?? ??", DetourName = nameof(MovingDetour))]
    private static Hook<MovingControllerDelegate> movingHook = null;
    internal static unsafe bool IsMoving
    {
        set => _posLocker = !value;
    }
    public MovingController()
    {
        SignatureHelper.Initialise(this);
        movingHook?.Enable();
    }
    public void Dispose()
    {
        movingHook.Disable();
    }

    private static bool MovingDetour(IntPtr ptr)
    {
        if (Service.Conditions[ConditionFlag.OccupiedInEvent])
            return movingHook.Original(ptr);

        if (Service.Config.PoslockCasting && _posLocker && DataCenter.InCombat)
        {
            //没有键盘取消
            if (!Service.KeyState[ConfigurationHelper.Keys[Service.Config.PoslockModifier]]
              //也没有手柄取消
              && Service.GamepadState.Raw(Dalamud.Game.ClientState.GamePad.GamepadButtons.L2) <= 0.5f) return false;
        }
        return movingHook.Original(ptr);
    }
}
