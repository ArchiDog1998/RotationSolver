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

namespace XIVAutoAttack.Controllers
{
    internal class MovingController : IDisposable
    {
        private static bool PosLocker = false;
        private static Hook<MovingControllerDelegate> movingHook;
        private delegate bool MovingControllerDelegate(IntPtr ptr);
        public MovingController()
        {
            movingHook ??= Hook<MovingControllerDelegate>.FromAddress(Service.Address.MovingController, new MovingControllerDelegate(MovingDetour));
            movingHook.Enable();
        }
        public void Dispose()
        {
            movingHook.Disable();
        }
        private static bool MovingDetour(IntPtr ptr)
        {
            if (Service.Configuration.PoslockCasting && PosLocker && !Service.KeyState[Service.Configuration.PoslockModifier]) return false;
            return movingHook.Original(ptr);
        }
        internal bool IsMoving
        {
            get => Service.Conditions[ConditionFlag.BeingMoved];
            set => PosLocker = !value;
        }
    }
}
