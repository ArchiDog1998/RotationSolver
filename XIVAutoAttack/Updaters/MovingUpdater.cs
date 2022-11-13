using Dalamud.Game.Gui;
using Dalamud.Hooking;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.Keys;
using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Numerics;
using Dalamud.Game.Gui.Toast;
using XIVAutoAttack.Data;
using XIVAutoAttack.Helpers;

namespace XIVAutoAttack.Updaters
{
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
        
        internal static void UpdateLocation()
        {
            if (Service.ClientState.LocalPlayer == null) return;
            var p = Service.ClientState.LocalPlayer.Position;

            _moving = _lastPosition != p;
            _lastPosition = p;

            if (Service.ClientState.LocalPlayer.HaveStatus(true, StatusID.TenChiJin)) MovingUpdater.IsMoving = false;

        }

        private static bool MovingDetour(IntPtr ptr)
        {
            if (Service.Conditions[ConditionFlag.OccupiedInEvent])
                return movingHook.Original(ptr);

            if (Service.Configuration.PoslockCasting && _posLocker)
            {
                //没有键盘取消
                if (!Service.KeyState[Service.Configuration.PoslockModifier]
                  //也没有手柄取消
                  && !(Service.GamepadState.Raw(Dalamud.Game.ClientState.GamePad.GamepadButtons.L2) > 0.5f
                  && Service.GamepadState.Raw(Dalamud.Game.ClientState.GamePad.GamepadButtons.R2) > 0.5f)) return false;
            }
            return movingHook.Original(ptr);
        }

        static Vector3 _lastPosition = Vector3.Zero;
        static bool _moving = false;
        internal static bool IsMoving
        {
            get => _moving;
            set => _posLocker = !value;
        }
    }
}
