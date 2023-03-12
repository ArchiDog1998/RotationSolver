using Dalamud.Game.ClientState.Conditions;
using Dalamud.Hooking;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using RotationSolver.Data;
using RotationSolver.Helpers;
using System;
using System.Numerics;

namespace RotationSolver.Updaters;

internal static class MovingUpdater
{
    private static bool _posLocker = false;
    private static Hook<MovingControllerDelegate> movingHook;
    private delegate bool MovingControllerDelegate(IntPtr ptr);
    internal static void Enable()
    {
        movingHook ??= Hook<MovingControllerDelegate>.FromAddress(Service.Address.MovingController, new MovingControllerDelegate(MovingDetour));
        movingHook.Enable();
    }
    internal static void Dispose()
    {
        movingHook.Disable();
    }

    private static bool MovingDetour(IntPtr ptr)
    {
        if (Service.Conditions[ConditionFlag.OccupiedInEvent])
            return movingHook.Original(ptr);

        if (Service.Configuration.PoslockCasting && _posLocker)
        {
            //没有键盘取消
            if (!Service.KeyState[ConfigurationHelper.Keys[Service.Configuration.PoslockModifier]]
              //也没有手柄取消
              && Service.GamepadState.Raw(Dalamud.Game.ClientState.GamePad.GamepadButtons.L2) <= 0.5f) return false;
        }
        return movingHook.Original(ptr);
    }
    internal static unsafe bool IsMoving
    {
        get => AgentMap.Instance()->IsPlayerMoving > 0;
        set => _posLocker = !value;
    }
}
